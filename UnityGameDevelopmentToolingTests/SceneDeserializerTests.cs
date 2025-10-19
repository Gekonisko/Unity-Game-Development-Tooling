using Unity_Game_Development_tooling.Models;
using UnityGameDevelopmentTooling;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentToolingTests
{
    public class SceneDeserializerTests
    {
        [Fact]
        public void DeserializeScene_ShouldCreateObjects_FromSceneMain()
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var result = deserializer.DeserializeScene("UnityScenes\\Main.unity");

            var expectedResult = new Dictionary<UnityObjectInfo, UnityObject>
            {
                { new UnityObjectInfo(4, 519420032), new Transform() },
                { new UnityObjectInfo(1, 519420028), new GameObject() },
                { new UnityObjectInfo(1, 619394800), new GameObject() },
                { new UnityObjectInfo(4, 619394802), new Transform() }
            };

            foreach (var kvp in expectedResult)
            {
                Assert.True(result.ContainsKey(kvp.Key));
            }
        }
    }
}