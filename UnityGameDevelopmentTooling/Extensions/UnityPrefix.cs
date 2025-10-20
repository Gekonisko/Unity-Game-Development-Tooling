namespace UnityGameDevelopmentTooling.Extensions
{
    internal static class UnityPrefix
    {
        public static IEnumerable<string> RemoveUnityPrefixedFields(this IEnumerable<string> fields)
        {
            return fields.Where(field => !field.StartsWith("m_", StringComparison.Ordinal));
        }
    }
}