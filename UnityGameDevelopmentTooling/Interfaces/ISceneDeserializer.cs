using UnityGameDevelopmentTooling.Models;

namespace UnityGameDevelopmentTooling.Interfaces
{
    public interface ISceneDeserializer
    {
        Dictionary<UnityObjectInfo, UnityObject> DeserializeScene(string path);
    }
}