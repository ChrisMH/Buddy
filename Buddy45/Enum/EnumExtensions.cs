using System.ComponentModel;
using System.Linq;

namespace Buddy.Enum
{
    public static class EnumExtensions
    {
        public static string GetDescription(this System.Enum source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString());

            var descriptionAttrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().ToArray();
            if (descriptionAttrs.Any())
                return descriptionAttrs[0].Description;

            return string.Empty;
        }
    }
}