using System.Diagnostics;
using System.Text;
using UnityGameDevelopmentTooling.Interfaces;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentTooling
{
    internal class Program
    {
        private static readonly IDeserializer _camelCaseDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

        private static readonly IDeserializer _defaultDeserializer = new DeserializerBuilder()
        .Build();

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

            IYamlDeserializer yaml = new YamlDotNetDeserializer(_camelCaseDeserializer);
            ISceneParser sceneParser = new SceneParser();
            ISceneDeserializer deserializer = new SceneDeserializer(sceneParser, yaml);

            UnityProjectAnalizer analizer = new UnityProjectAnalizer(projectPath, deserializer);

            var sw = Stopwatch.StartNew();
            AnalysisResult analysisResult = analizer.Analize();

            analizer.GetScenesHierarchies(analysisResult).ForEach(scene =>
            {
                var filePath = outputPath + "\\" + scene.Name + ".dump";
                File.WriteAllText(filePath, scene.Hierarchy);
            });

            StringBuilder unusedScriptsBuffer = new StringBuilder();
            unusedScriptsBuffer.AppendLine("Relative Path,GUID");
            foreach (FileResult unusedScript in analizer.GetUnusedScripts(analysisResult))
            {
                unusedScriptsBuffer.AppendLine(unusedScript.RelativePath + "," + unusedScript.Guid);
            }
            foreach (FileResult unusedScript in analizer.GetUnusedSerializableScripts(analysisResult, _defaultDeserializer))
            {
                unusedScriptsBuffer.AppendLine(unusedScript.RelativePath + "," + unusedScript.Guid);
            }
            File.WriteAllText(outputPath + "\\UnusedScripts.csv", unusedScriptsBuffer.ToString());
            sw.Stop();
            Console.WriteLine($"Parallel analysis took {sw.ElapsedMilliseconds} ms");
        }
    }
}