using UnityGameDevelopmentTooling.SerializesModels;

namespace UnityGameDevelopmentTooling.Models
{
    public record SceneName(string Value)
    {
        public override string ToString() => Value;
    }

    public record ScriptGuid(string Value)
    {
        public override string ToString() => Value;
    }

    public record SceneData(SceneName Name, Dictionary<UnityObjectInfo, UnityObject> Objects);

    public record AnalysisResult(List<SceneData> Scenes, Dictionary<ScriptGuid, List<(SceneName SceneName, MonoBehaviour MonoBehaviour)>> MonoBehavioursInScenesByGuid);
}