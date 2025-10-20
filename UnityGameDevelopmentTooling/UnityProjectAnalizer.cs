using Microsoft.CodeAnalysis;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;
using UnityGameDevelopmentTooling.Utils;

namespace UnityGameDevelopmentTooling
{
    public class UnityProjectAnalizer
    {
        private readonly ISceneDeserializer _deserializer;
        private readonly string _projectPath;
        private List<(string, Dictionary<UnityObjectInfo, UnityObject>)> sceneObjectsPerScene;
        private Dictionary<string, List<MonoBehaviour>> monoBehavioursInScenesByGuid;

        public UnityProjectAnalizer(string projectPath, ISceneDeserializer deserializer)
        {
            _projectPath = projectPath;
            _deserializer = deserializer;
        }

        public void Analize()
        {
            sceneObjectsPerScene = new();
            monoBehavioursInScenesByGuid = new();
            FileResult[] sceneFiles = FileUtils.FindFilesByExtension(_projectPath + "\\Assets", "unity");

            foreach (FileResult sceneFile in sceneFiles)
            {
                var sceneObjects = _deserializer.DeserializeScene(sceneFile.AbsolutePath);
                sceneObjectsPerScene.Add((sceneFile.Name, sceneObjects));

                sceneObjects
                    .Where(sb => sb.Key.ClassId == 114)
                    .Select(sb => (MonoBehaviour)sb.Value)
                    .ToList()
                    .ForEach(mb =>
                    {
                        if (monoBehavioursInScenesByGuid.ContainsKey(mb.Script.guid))
                            monoBehavioursInScenesByGuid[mb.Script.guid].Add(mb);
                        else
                            monoBehavioursInScenesByGuid.Add(mb.Script.guid, [mb]);
                    });
            }
        }

        public List<(string Name, string Hierarchy)> GetScenesHierarchies()
        {
            if (sceneObjectsPerScene == null)
                throw new InvalidOperationException("You must call Analize() before GetScenesHierarchies().");

            List<(string, string)> result = new();
            foreach (var (Name, sceneObjects) in sceneObjectsPerScene)
            {
                var builder = new UnityHierarchyBuilder(sceneObjects);
                var hierarchy = builder.Build();
                result.Add((Name, hierarchy));
            }
            return result;
        }

        public List<FileResult> GetUnusedScripts()
        {
            if (monoBehavioursInScenesByGuid == null)
                throw new InvalidOperationException("You must call Analize() before GetUnusedScripts().");

            FileResult[] scriptFiles = FileUtils.FindFilesByExtension(_projectPath + "\\Assets", "cs");
            List<FileResult> unusedScripts = new();

            foreach (FileResult scriptFile in scriptFiles)
            {
                if (monoBehavioursInScenesByGuid.ContainsKey(scriptFile.Guid) == false)
                {
                    unusedScripts.Add(scriptFile);
                }
            }
            return unusedScripts;
        }

        public List<FileResult> GetUnusedSerializableScripts()
        {
            List<FileResult> result = new();

            FileResult[] scriptFiles = FileUtils.FindFilesByExtension(_projectPath + "\\Assets", "cs");
            Dictionary<string, FileResult> guidPerFile = scriptFiles.ToDictionary(f => f.Guid, f => f);

            foreach (var monoBehavioursList in monoBehavioursInScenesByGuid.Values)
            {
                var guid = monoBehavioursList.First().Script.guid;
                if (guidPerFile.TryGetValue(guid, out FileResult file) == false)
                    continue;

                List<string> fileSerializableFields = FileUtils.GetSerializableFieldsFromMonoBehaviourScript(file.AbsolutePath);

                foreach (var monoBehaviour in monoBehavioursList)
                {
                }
            }

            return result;
        }
    }
}