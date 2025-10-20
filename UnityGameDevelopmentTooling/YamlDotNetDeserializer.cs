using UnityGameDevelopmentTooling.Interfaces;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling
{
    public class YamlDotNetDeserializer : IYamlDeserializer
    {
        private readonly IDeserializer _deserializer;

        public YamlDotNetDeserializer(IDeserializer deserializer)
        {
            _deserializer = deserializer;
        }

        public object Deserialize(string yaml, Type type)
            => _deserializer.Deserialize(yaml, type);

        public Dictionary<string, object> DeserializeHeader(string yamlHeader)
            => _deserializer.Deserialize<Dictionary<string, object>>(yamlHeader);
    }
}