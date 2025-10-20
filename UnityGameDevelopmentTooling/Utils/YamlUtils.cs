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
    }
}