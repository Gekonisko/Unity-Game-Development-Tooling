namespace Unity_Game_Development_tooling.Models
{
    public class GameObjectData
    {
        public required string Name { get; set; }
        public int? FileId { get; set; }
        public List<string> ScriptGuids { get; set; } = new();
        public List<GameObjectData> Children { get; set; } = new();
    }
}
