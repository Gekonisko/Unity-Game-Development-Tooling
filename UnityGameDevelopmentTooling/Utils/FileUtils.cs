using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentTooling.Utils
{
    public static class FileUtils
    {
        public static FileResult[] FindFilesByExtension(string path, string extension)
        {
            try
            {
                if (!extension.StartsWith('.'))
                    extension = "." + extension;

                var files = Directory.EnumerateFiles(path, "*" + extension, SearchOption.AllDirectories);

                List<FileResult> results = new List<FileResult>();

                foreach (var file in files)
                {
                    string relative = Path.GetRelativePath(path, file);
                    string guid = ReadGuidFromMeta(file);
                    string name = Path.GetFileName(file);

                    var fileResult = new FileResult
                    {
                        AbsolutePath = file,
                        RelativePath = relative,
                        Guid = guid,
                        Name = name
                    };

                    results.Add(fileResult);
                }

                return results.ToArray();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied in one of the directories: {ex.Message}");
                return Array.Empty<FileResult>();
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory not found: {ex.Message}");
                return Array.Empty<FileResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<FileResult>();
            }
        }

        public static string ReadGuidFromMeta(string filePath)
        {
            string metaFile = filePath + ".meta";
            if (!File.Exists(metaFile))
                return string.Empty;

            try
            {
                var deserializer = new DeserializerBuilder()
                 .WithNamingConvention(CamelCaseNamingConvention.Instance)
                 .IgnoreUnmatchedProperties()
                 .Build();

                using var reader = new StreamReader(metaFile);
                var meta = deserializer.Deserialize<MetaFile>(reader);

                return string.IsNullOrEmpty(meta?.Guid) ? string.Empty : meta.Guid;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<string> GetSerializableFieldsFromMonoBehaviourScript(string scriptPath)
        {
            var serializableFields = new List<string>();

            if (!File.Exists(scriptPath))
                return serializableFields;

            string sourceCode = File.ReadAllText(scriptPath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = tree.GetRoot();

            var classNodes = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cls => cls.BaseList?.Types
                    .Any(bt => bt.ToString().Contains("MonoBehaviour")) == true);

            foreach (var classNode in classNodes)
            {
                var fieldDeclarations = classNode.Members.OfType<FieldDeclarationSyntax>();

                foreach (var field in fieldDeclarations)
                {
                    bool isPublic = field.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
                    bool hasSerializeField = field.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .Any(a => a.Name.ToString().Contains("SerializeField"));

                    if (isPublic || hasSerializeField)
                    {
                        foreach (var variable in field.Declaration.Variables)
                            serializableFields.Add(variable.Identifier.Text);
                    }
                }
            }
            return serializableFields;
        }
    }
}