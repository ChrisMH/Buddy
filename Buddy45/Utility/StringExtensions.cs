namespace Buddy.Utility
{
    public static class StringExtensions
    {
        public static string ToLowerCamelCase(this string convert)
        {
            return char.ToLowerInvariant(convert[0]) + convert.Substring(1);
        }

        public static string ToUpperCamelCase(this string convert)
        {
            return char.ToUpperInvariant(convert[0]) + convert.Substring(1);
        }
    }
}