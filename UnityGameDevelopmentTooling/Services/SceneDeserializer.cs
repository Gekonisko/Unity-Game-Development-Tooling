using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.Utils;

namespace UnityGameDevelopmentTooling.Services
{
    public class SceneDeserializer : ISceneDeserializer
    {
        private readonly ISceneParser _parser;
        private readonly IYamlDeserializer _deserializer;

        public SceneDeserializer(ISceneParser parser, IYamlDeserializer deserializer)
        {
            _parser = parser;
            _deserializer = deserializer;
        }

        public Dictionary<UnityObjectInfo, UnityObject> DeserializeScene(string path)
        {
            Dictionary<UnityObjectInfo, UnityObject> result = new();
            foreach (var (header, yamlText) in _parser.Parse(path))
            {
                var split = YamlUtils.SplitYaml(yamlText);
                var headerDict = _deserializer.DeserializeHeader(split.FirstLine);
                string className = headerDict.Keys.First();

                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == className);

                if (type is not null)
                {
                    var obj = (UnityObject)_deserializer.Deserialize(split.Body, type);
                    obj.Yaml = split.Body;
                    result.Add(header, obj);
                }
            }
            return result;
        }
    }
}