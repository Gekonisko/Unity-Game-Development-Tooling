using Microsoft.CodeAnalysis;
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
            var scenes = new List<SceneData>();
            var monoBehavioursInScenesByGuid = new Dictionary<ScriptGuid, List<MonoBehaviour>>();

            FileResult[] sceneFiles = FileUtils.FindFilesByExtension(_assetPath, "unity");

            foreach (FileResult sceneFile in sceneFiles)
            {
                var sceneObjects = _deserializer.DeserializeScene(sceneFile.AbsolutePath);
                scenes.Add(new SceneData(new SceneName(sceneFile.Name), sceneObjects));

                sceneObjects
                    .Where(sb => sb.Key.ClassId == 114) // https://docs.unity3d.com/6000.2/Documentation/Manual/ClassIDReference.html
                    .Select(sb => (MonoBehaviour)sb.Value)
                    .ToList()
                    .ForEach(mb =>
                    {
                        var guid = new ScriptGuid(mb.Script.guid);
                        if (monoBehavioursInScenesByGuid.ContainsKey(guid))
                            monoBehavioursInScenesByGuid[guid].Add(mb);
                        else
                            monoBehavioursInScenesByGuid.Add(guid, [mb]);
                    });
            }
            return new AnalysisResult(scenes, monoBehavioursInScenesByGuid);
        }

        public List<(string Name, string Hierarchy)> GetScenesHierarchies(AnalysisResult analysisResult)
        {
            if (analysisResult.Scenes is null)
                throw new InvalidOperationException("You must call Analize() before GetScenesHierarchies().");

            List<(string, string)> result = new();
            foreach (var (Name, sceneObjects) in analysisResult.Scenes)
            {
                var builder = new UnityHierarchyBuilder(sceneObjects);
                var hierarchy = builder.Build();
                result.Add((Name.Value, hierarchy));
            }
            return result;
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
            List<FileResult> result = new();

            FileResult[] scriptFiles = FileUtils.FindFilesByExtension(_assetPath, "cs");
            Dictionary<string, FileResult> guidPerFile = scriptFiles.ToDictionary(f => f.Guid, f => f);

            foreach (var monoBehaviours in analysisResult.MonoBehavioursInScenesByGuid.Values)
            {
                var guid = monoBehaviours.GetFirstScript();

                if (guidPerFile.TryGetValue(guid.Value, out var file) is false) continue;

                List<string> serializableFields = FileUtils.GetSerializableFieldsFromMonoBehaviourScript(file.AbsolutePath);

                foreach (var monoBehaviour in monoBehaviours)
                {
                    var fields =
                        YamlUtils.GetKeys(monoBehaviour.Yaml, deserializer)
                        .RemoveUnityPrefixedFields()
                        .ToList();

                    bool allPresent = serializableFields.All(f => fields.Contains(f));

                    if (!allPresent)
                    {
                        result.Add(file);
                        var missing = serializableFields.Except(fields);
                        Console.WriteLine($"[WARN] MonoBehaviour '{monoBehaviour.GetScriptGuid()}' in script '{file.Name}' is missing serialized fields: {string.Join(", ", missing)}");
                        break;
                    }
                }
            }

            return result;
        }
    }
}