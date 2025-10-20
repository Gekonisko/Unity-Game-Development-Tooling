using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using UnityGameDevelopmentTooling.Extensions;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;
using UnityGameDevelopmentTooling.Utils;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.Services
{
    public class UnityProjectAnalizer
    {
        public readonly string Assets = "Assets";
        private readonly ISceneDeserializer _deserializer;
        private readonly string _projectPath;
        private readonly string _assetPath;

        public UnityProjectAnalizer(string projectPath, ISceneDeserializer deserializer)
        {
            _projectPath = projectPath;
            _deserializer = deserializer;
            _assetPath = Path.Combine(_projectPath, Assets);
        }

        public AnalysisResult Analize()
        {
            var scenes = new ConcurrentBag<SceneData>();
            var monoBehavioursInScenesByGuid = new ConcurrentDictionary<ScriptGuid, List<(SceneName SceneName, MonoBehaviour MonoBehaviour)>>();

            FileResult[] sceneFiles = FileUtils.FindFilesByExtension(_assetPath, "unity");

            Parallel.ForEach(sceneFiles, sceneFile =>
            {
                var sceneObjects = _deserializer.DeserializeScene(sceneFile.AbsolutePath);
                var sceneName = new SceneName(sceneFile.Name);
                scenes.Add(new SceneData(sceneName, sceneObjects));

                var monoBehaviours = sceneObjects
                    .Where(sb => sb.Key.ClassId == 114)
                    .Select(sb => (MonoBehaviour)sb.Value);

                foreach (var mb in monoBehaviours)
                {
                    var guid = new ScriptGuid(mb.Script.guid);
                    monoBehavioursInScenesByGuid.AddOrUpdate(
                        guid,
                        _ => new List<(SceneName, MonoBehaviour)> { (sceneName, mb) },
                        (_, existing) => { lock (existing) existing.Add((sceneName, mb)); return existing; });
                }
            });

            return new AnalysisResult(scenes.ToList(), monoBehavioursInScenesByGuid.ToDictionary());
        }

        public List<(string Name, string Hierarchy)> GetScenesHierarchies(AnalysisResult analysisResult)
        {
            if (analysisResult.Scenes is null)
                throw new InvalidOperationException("You must call Analize() before GetScenesHierarchies().");

            var result = new ConcurrentBag<(string, string)>();

            Parallel.ForEach(analysisResult.Scenes, scene =>
            {
                var builder = new UnityHierarchyBuilder(scene.Objects);
                var hierarchy = builder.Build();
                result.Add((scene.Name.Value, hierarchy));
            });

            return result.ToList();
        }

        public List<FileResult> GetUnusedScripts(AnalysisResult analysisResult)
        {
            if (analysisResult.MonoBehavioursInScenesByGuid is null)
                throw new InvalidOperationException("You must call Analize() before GetUnusedScripts().");

            FileResult[] scriptFiles = FileUtils.FindFilesByExtension(_assetPath, "cs");
            List<FileResult> unusedScripts = new();

            foreach (FileResult scriptFile in scriptFiles)
            {
                if (analysisResult.MonoBehavioursInScenesByGuid.ContainsKey(new ScriptGuid(scriptFile.Guid)) == false)
                {
                    unusedScripts.Add(scriptFile);
                }
            }
            return unusedScripts;
        }

        public List<FileResult> GetUnusedSerializableScripts(AnalysisResult analysisResult, IDeserializer deserializer)
        {
            var result = new ConcurrentBag<FileResult>();

            FileResult[] scriptFiles = FileUtils.FindFilesByExtension(_assetPath, "cs");
            var guidPerFile = scriptFiles.ToDictionary(f => f.Guid, f => f);

            Parallel.ForEach(analysisResult.MonoBehavioursInScenesByGuid.Values, monoBehaviours =>
            {
                var guid = monoBehaviours.GetFirstScript();

                if (!guidPerFile.TryGetValue(guid.Value, out var file))
                    return;

                var serializableFields = FileUtils.GetSerializableFieldsFromMonoBehaviourScript(file.AbsolutePath);

                foreach (var monoBehaviour in monoBehaviours)
                {
                    var fields = YamlUtils.GetKeys(monoBehaviour.MonoBehaviour.Yaml, deserializer)
                                          .RemoveUnityPrefixedFields()
                                          .ToList();

                    bool allPresent = serializableFields.All(f => fields.Contains(f));

                    if (!allPresent)
                    {
                        result.Add(file);
                        var missing = serializableFields.Except(fields);
                        Console.WriteLine($"[WARN] MonoBehaviour '{monoBehaviour.MonoBehaviour.GetScriptGuid()}' in script '{file.Name}' inside Unity Scene `{monoBehaviour.SceneName}` is missing serialized fields: {string.Join(", ", missing)}");
                        break;
                    }
                }
            });

            return result.Distinct().ToList();
        }
    }
}