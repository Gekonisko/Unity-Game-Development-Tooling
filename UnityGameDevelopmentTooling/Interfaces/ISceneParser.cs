using UnityGameDevelopmentTooling.Models;

namespace UnityGameDevelopmentTooling.Interfaces
{
    public interface ISceneParser
    {
        IEnumerable<(UnityObjectInfo Header, string Yaml)> Parse(string path);
    }
}