using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.Interfaces
{
    public interface IYamlDeserializer
    {
        object Deserialize(string yaml, Type type);

        Dictionary<string, object> DeserializeHeader(string yamlHeader);

        IDeserializer GetDeserializer();
    }
}