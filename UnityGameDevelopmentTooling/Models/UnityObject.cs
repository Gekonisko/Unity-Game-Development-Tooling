using UnityGameDevelopmentTooling.Utils;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.Models
{
    public abstract class UnityObject
    {
        public List<string> SerializeFields { get; set; }

        public virtual void OnDeserializeYaml(string yaml, IDeserializer deserializer)
        {
            SerializeFields = YamlUtils.GetKeys(yaml, deserializer);
        }
    }
}