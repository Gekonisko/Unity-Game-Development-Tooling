
namespace Unity_Game_Development_tooling.Models
{
    public class SceneData
    {
        public string SceneName { get; set; }
        public List<GameObjectData> RootObjects { get; set; } = new();
    }
}
