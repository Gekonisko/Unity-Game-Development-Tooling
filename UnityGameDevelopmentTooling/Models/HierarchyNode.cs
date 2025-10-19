namespace UnityGameDevelopmentTooling.Models
{
    public class HierarchyNode
    {
        public string Name { get; set; }
        public List<HierarchyNode> Children { get; set; } = new();
    }
}