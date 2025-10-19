using UnityGameDevelopmentTooling;
using UnityGameDevelopmentTooling.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentToolingTests
{
    public class UnityHierarchyBuilderTests
    {
        [Fact]
        public void Build_ShouldCreateHierarchy_FromSceneMain()
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var sceneObjects = deserializer.DeserializeScene("UnityScenes\\Main.unity");

            IHierarchyBuilder builder = new UnityHierarchyBuilder(sceneObjects);
            var result = builder.Build();

            string expected = File.ReadAllText("Hierarchy\\Main.txt");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Build_ShouldCreateHierarchy_FromSceneGame()
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var sceneObjects = deserializer.DeserializeScene("UnityScenes\\Game.unity");

            IHierarchyBuilder builder = new UnityHierarchyBuilder(sceneObjects);
            var result = builder.Build();

            string expected = File.ReadAllText("Hierarchy\\Game.txt");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Build_ShouldCreateHierarchy_FromSceneGame2()
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var sceneObjects = deserializer.DeserializeScene("UnityScenes\\Game2.unity");

            IHierarchyBuilder builder = new UnityHierarchyBuilder(sceneObjects);
            var result = builder.Build();

            string expected = File.ReadAllText("Hierarchy\\Game2.txt");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Build_ShouldCreateHierarchy_FromSceneStudentCity()
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var sceneObjects = deserializer.DeserializeScene("UnityScenes\\Student City.unity");

            IHierarchyBuilder builder = new UnityHierarchyBuilder(sceneObjects);
            var result = builder.Build();

            string expected = File.ReadAllText("Hierarchy\\Student City.txt");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Build_ShouldCreateHierarchy_FromScenePlayer()
        {
            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();

            var deserializer = new SceneDeserializer(sceneParser, yaml);
            var sceneObjects = deserializer.DeserializeScene("UnityScenes\\Player.unity");

            IHierarchyBuilder builder = new UnityHierarchyBuilder(sceneObjects);
            var result = builder.Build();

            string expected = File.ReadAllText("Hierarchy\\Player.txt");
            Assert.Equal(expected, result);
        }
    }
}