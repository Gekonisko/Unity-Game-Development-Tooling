using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity_Game_Development_tooling.Models;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;

namespace UnityGameDevelopmentTooling
{
    public class SceneDeserializer
    {
        private readonly ISceneParser _parser;
        private readonly IYamlDeserializer _yaml;
        private readonly Dictionary<UnityObjectInfo, UnityObject> _unityObjects = new();

        public SceneDeserializer(ISceneParser parser, IYamlDeserializer yaml)
        {
            _parser = parser;
            _yaml = yaml;
        }

        public Dictionary<UnityObjectInfo, UnityObject> DeserializeScene(string path)
        {
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
                    _unityObjects.Add(header, (UnityObject)obj);
                }
            }

            return _unityObjects;
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