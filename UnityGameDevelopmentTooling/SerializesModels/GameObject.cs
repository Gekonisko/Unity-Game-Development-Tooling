using UnityGameDevelopmentTooling.Models;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.SerializesModels
{
    public class GameObject : UnityObject
    {
        [YamlMember(Alias = "m_Component", ApplyNamingConventions = false)]
        public List<ComponentRef> Components { get; set; }

        [YamlMember(Alias = "m_Name", ApplyNamingConventions = false)]
        public string Name { get; set; }
    }
}