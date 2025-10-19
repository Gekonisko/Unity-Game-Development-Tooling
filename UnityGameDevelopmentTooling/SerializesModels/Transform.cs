using Unity_Game_Development_tooling.Models;
using UnityGameDevelopmentTooling.Models;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.SerializesModels
{
    public class Transform : UnityObject
    {
        [YamlMember(Alias = "m_GameObject", ApplyNamingConventions = false)]
        public FileID GameObject { get; set; }
        [YamlMember(Alias = "m_Children", ApplyNamingConventions = false)]
        public List<FileID> Children { get; set; }
        [YamlMember(Alias = "m_Father", ApplyNamingConventions = false)]
        public FileID Father { get; set; }
    }
}
