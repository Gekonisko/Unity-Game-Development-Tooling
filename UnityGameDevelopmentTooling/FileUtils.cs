using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityGameDevelopmentTooling.Models;
using UnityGameDevelopmentTooling.SerializesModels;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityGameDevelopmentTooling
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

                    var fileResult = new FileResult
                    {
                        AbsolutePath = file,
                        RelativePath = relative,
                        Guid = guid,
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
    }
}