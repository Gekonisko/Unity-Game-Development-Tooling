namespace UnityGameDevelopmentTooling
{
    internal class Program
    {
        private static void Main(string[] args)
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
                //List<string> names = ExtractGameObjectNames(scene);
                //Console.WriteLine($"scene: {scene}");
                //names.ForEach(Console.WriteLine);
                //var serializer = new UnitySceneSerializer(scene);
            }
        }
    }
}