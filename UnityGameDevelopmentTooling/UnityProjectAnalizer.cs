using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;

namespace UnityGameDevelopmentTooling
{
    public class UnityProjectAnalizer
    {
        private readonly ISceneDeserializer _deserializer;
        private readonly string _projectPath;
        private List<(string, Dictionary<UnityObjectInfo, UnityObject>)> sceneObjectsPerScene;
        private Dictionary<string, MonoBehaviour> monoBehavioursInScenes;

        public UnityProjectAnalizer(string projectPath, ISceneDeserializer deserializer)
        {
            _projectPath = projectPath;
            _deserializer = deserializer;
        }

        public void Analize()
        {
            sceneObjectsPerScene = new();
            monoBehavioursInScenes = new();
            FileResult[] sceneFiles = FileUtils.FindFilesByExtension(_projectPath + "\\Assets", "unity");

            foreach (FileResult sceneFile in sceneFiles)
            {
                var sceneObjects = _deserializer.DeserializeScene(sceneFile.AbsolutePath);
                sceneObjectsPerScene.Add((sceneFile.Name, sceneObjects));

                sceneObjects
                    .Where(sb => sb.Value is MonoBehaviour)
                    .Select(sb => (MonoBehaviour)sb.Value)
                    .ToList()
                    .ForEach(mb => monoBehavioursInScenes.TryAdd(mb.Script.guid, mb));
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
            if (monoBehavioursInScenes == null)
                throw new InvalidOperationException("You must call Analize() before GetUnusedScripts().");

            FileResult[] scriptFiles = FileUtils.FindFilesByExtension(_projectPath + "\\Assets", "cs");
            List<FileResult> unusedScripts = new();

            foreach (FileResult scriptFile in scriptFiles)
            {
                if (monoBehavioursInScenes.ContainsKey(scriptFile.Guid) == false)
                {
                    unusedScripts.Add(scriptFile);
                }
            }
            return unusedScripts;
        }
    }
}