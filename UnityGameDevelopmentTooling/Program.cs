using System.Text;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentTooling
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: tool.exe <unity_project_path> <output_folder_path>");
                return;
            }

            string projectPath = args[0];
            string outputPath = args[1];

            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine($"Project path not found: {projectPath}");
                return;
            }
            Directory.CreateDirectory(outputPath);

            IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_deserializer);
            ISceneParser sceneParser = new SceneParser();
            ISceneDeserializer deserializer = new SceneDeserializer(sceneParser, yaml);

            UnityProjectAnalizer analizer = new UnityProjectAnalizer(projectPath, deserializer);
            analizer.Analize();

            analizer.GetScenesHierarchies().ForEach(scene =>
            {
                var filePath = outputPath + "\\" + scene.Name + ".dump";
                File.WriteAllText(filePath, scene.Hierarchy);
            });

            StringBuilder unusedScriptsBuffer = new StringBuilder();
            unusedScriptsBuffer.AppendLine("Relative Path,GUID");
            foreach (FileResult unusedScript in analizer.GetUnusedScripts())
            {
                unusedScriptsBuffer.AppendLine(unusedScript.RelativePath + "," + unusedScript.Guid);
            }
            foreach (FileResult unusedScript in analizer.GetUnusedSerializableScripts())
            {
                unusedScriptsBuffer.AppendLine(unusedScript.RelativePath + "," + unusedScript.Guid);
            }
            File.WriteAllText(outputPath + "\\UnusedScripts.csv", unusedScriptsBuffer.ToString());
        }
    }
}