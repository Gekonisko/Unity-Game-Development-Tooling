using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;

namespace UnityGameDevelopmentTooling.Extensions
{
    internal static class MonoBehavioursList
    {
        public static ScriptGuid GetFirstScript(this List<MonoBehaviour> list)
        {
            return new ScriptGuid(list.First().Script.guid);
        }
    }
}