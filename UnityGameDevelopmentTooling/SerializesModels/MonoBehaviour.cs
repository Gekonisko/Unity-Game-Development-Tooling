using UnityGameDevelopmentTooling.Models;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.SerializesModels
{
    public class MonoBehaviour : UnityObject
    {
        [YamlMember(Alias = "m_Script", ApplyNamingConventions = false)]
        public ScriptRef Script { get; set; }
    }
}