using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;

namespace UnityGameDevelopmentTooling
{
    public class SceneDeserializer : ISceneDeserializer
    {
        private readonly ISceneParser _parser;
        private readonly IYamlDeserializer _yaml;

        public SceneDeserializer(ISceneParser parser, IYamlDeserializer yaml)
        {
            _parser = parser;
            _yaml = yaml;
        }

        public Dictionary<UnityObjectInfo, UnityObject> DeserializeScene(string path)
        {
            Dictionary<UnityObjectInfo, UnityObject> result = new();
            foreach (var (header, yamlText) in _parser.Parse(path))
            {
                var split = SplitYaml(yamlText);
                var headerDict = _yaml.DeserializeHeader(split.FirstLine);
                string className = headerDict.Keys.First();

                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == className);

                if (type != null)
                {
                    var obj = _yaml.Deserialize(split.Body, type);
                    result.Add(header, (UnityObject)obj);
                }
            }

            return result;
        }

        private static (string FirstLine, string Body) SplitYaml(string yaml)
        {
            var parts = yaml.Split(new[] { '\n' }, 2);
            string first = parts[0];
            string body = parts.Length > 1 ? "\n" + parts[1] : string.Empty;
            return (first, body);
        }
    }
}