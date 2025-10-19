using System.Text;
using System.Text.RegularExpressions;
using Unity_Game_Development_tooling.Models;
using UnityGameDevelopmentTooling.Interfaces;

namespace UnityGameDevelopmentTooling
{
    public class SceneParser : ISceneParser
    {
        public IEnumerable<(UnityObjectInfo Header, string Yaml)> Parse(string path)
        {
            using var reader = new StreamReader(path);
            StringBuilder buffer = new();
            UnityObjectInfo? header = null;
            bool isYaml = false;

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("--- !u!"))
                {
                    if (isYaml && header != null && header.IsStripped == false)
                        yield return (header, buffer.ToString());

                    buffer.Clear();
                    isYaml = true;
                    header = DeserializerHeader(line);
                }
                else if (isYaml)
                {
                    buffer.AppendLine(line);
                }
            }

            if (isYaml && header != null)
                yield return (header, buffer.ToString());
        }

        private UnityObjectInfo? DeserializerHeader(string headerLine)
        {
            var match = Regex.Match(headerLine, @"^--- !u!(\d+) &(\d+)");
            if (match.Success)
            {
                string classId = match.Groups[1].Value;
                string fileId = match.Groups[2].Value;

                return new UnityObjectInfo(int.Parse(classId), long.Parse(fileId), headerLine.Contains("stripped"));
            }
            return null;
        }
    }
}