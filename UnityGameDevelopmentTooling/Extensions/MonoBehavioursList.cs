using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;

namespace UnityGameDevelopmentTooling.Extensions
{
    internal static class MonoBehavioursList
    {
        public static ScriptGuid GetFirstScript(this List<(SceneName SceneName, MonoBehaviour MonoBehaviour)> list)
        {
            return new ScriptGuid(list.First().MonoBehaviour.Script.guid);
        }
    }
}