using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Buddy.Enum
{
    public static class EnumExtensions
    {
        public static string GetDescription(this System.Enum source)
        {
            return GetDescription(source.GetType().GetField(source.ToString()));         
        }

        public static TEnum GetEnumValueFromDescription<TEnum>(this string description, TEnum defaultValue)
        {
            if(!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an enumeratedType", nameof(TEnum));

            foreach(var value in System.Enum.GetValues(typeof(TEnum)))
            {
                var field = typeof(TEnum).GetField(value.ToString());
                if(GetDescription(field).Equals(description))
                    return (TEnum)value;
            }
            return defaultValue;
        }

        public static string GetDescription(FieldInfo fieldInfo)
        {
            var descriptionAttrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().ToArray();
            if (descriptionAttrs.Any())
                return descriptionAttrs[0].Description;

            return string.Empty;
        }

    }
}