using UnityGameDevelopmentTooling;
using UnityGameDevelopmentTooling.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentToolingTests
{
    public class UnityHierarchyBuilderTests
    {
        [Theory]
        [InlineData("UnityScenes\\Main.unity", "Hierarchy\\Main.txt")]
        [InlineData("UnityScenes\\Game.unity", "Hierarchy\\Game.txt")]
        [InlineData("UnityScenes\\Game2.unity", "Hierarchy\\Game2.txt")]
        [InlineData("UnityScenes\\Student City.unity", "Hierarchy\\Student City.txt")]
        [InlineData("UnityScenes\\Player.unity", "Hierarchy\\Player.txt")]
        public void Build_ShouldCreateHierarchy_FromScenes(string scene, string expectedResult)
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var sceneObjects = deserializer.DeserializeScene(scene);

            IHierarchyBuilder builder = new UnityHierarchyBuilder(sceneObjects);
            var result = builder.Build();

            string expected = File.ReadAllText(expectedResult);
            Assert.Equal(expected, result);
        }
    }
}