using System;
using System.IO;
using System.Text.RegularExpressions;
using Unity_Game_Development_tooling;
using Unity_Game_Development_tooling.Models;
using YamlDotNet.RepresentationModel;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: tool.exe <unity_project_path>");
                return;
            }

            string projectPath = args[0];

            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine($"Project path not found: {projectPath}");
                return;
            }

            string projectAssetsPath = projectPath + "\\Assets";

            List<string> sceneFiles = new List<string>();
            foreach (var file in Directory.EnumerateFiles(projectAssetsPath, "*.unity", SearchOption.AllDirectories))
            {
                sceneFiles.Add(file);
            }

            Console.WriteLine("Found scenes:");
            foreach (var scene in sceneFiles)
            {
                List<string> names = ExtractGameObjectNames(scene);
                Console.WriteLine($"scene: {scene}");
                names.ForEach(Console.WriteLine);
            }

        }

        static List<string> ExtractGameObjectNames(string scenePath)
        {
            List<string> names = new();

            string yamlText = File.ReadAllText(scenePath);

            // 1️⃣ Preprocess Unity short tags -> valid YAML tags
            yamlText = Regex.Replace(yamlText, @"!u!(\d+)", "{tag:unity3d.com,2011:$1}");

            // 2️⃣ Load the YAML as multi-document stream
            var yaml = new YamlStream();
            using (var reader = new StringReader(yamlText))
            {
                yaml.Load(reader);
            }

            foreach (var doc in yaml.Documents)
            {
                if (doc.RootNode is not YamlMappingNode root)
                    continue;

                foreach (var entry in root.Children)
                {
                    var keyNode = entry.Key as YamlScalarNode;
                    if (keyNode == null || keyNode.Value != "GameObject")
                        continue;

                    var valueNode = entry.Value as YamlMappingNode;
                    if (valueNode == null)
                        continue;

                    foreach (var field in valueNode.Children)
                    {
                        if (field.Key is YamlScalarNode key && key.Value == "m_Name")
                        {
                            if (field.Value is YamlScalarNode val && !string.IsNullOrEmpty(val.Value))
                                names.Add(val.Value);
                        }
                    }
                }
            }

            return names;
        }
    }
}