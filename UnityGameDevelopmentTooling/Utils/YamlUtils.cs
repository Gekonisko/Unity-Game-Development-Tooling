using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace UnityGameDevelopmentTooling.Utils
{
    public static class YamlUtils
    {
        public static List<string> GetKeys(string yaml, IDeserializer deserializer)
        {
            var result = deserializer.Deserialize<object>(yaml);
            var map = result as Dictionary<object, object>;

            return map.Keys.Select(k => k.ToString()).ToList();
        }

        public static (string FirstLine, string Body) SplitYaml(string yaml)
        {
            var parts = yaml.Split(new[] { '\n' }, 2);
            string first = parts[0];
            string body = parts.Length > 1 ? "\n" + parts[1] : string.Empty;
            return (first, body);
        }

        public static List<string> ExtractGuidsFromYaml(string yaml)
        {
            var guids = new List<string>();
            var matches = Regex.Matches(yaml, @"guid:\s*([0-9a-fA-F]{32})");

            foreach (Match match in matches)
            {
                guids.Add(match.Groups[1].Value);
            }

            return guids;
        }
    }
}