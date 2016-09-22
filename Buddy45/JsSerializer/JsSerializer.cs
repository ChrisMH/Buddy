using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Buddy.JsSerializer
{
    public class JsSerializer
    {
        public JsSerializer()
        {
            
        }

        public void Serialize<T>(TextWriter writer, T source)
        {
            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            var propertyPairs = new List<string>();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(JsIgnoreAttribute)) != null)
                    continue;

                var propertyName = property.Name;
                var firstChar = propertyName.Substring(0, 1);
                propertyName = firstChar.ToLower() + propertyName.Substring(1);

                var propertyValue = property.GetValue(this).ToString();

                if (property.PropertyType == typeof(string))
                {
                    propertyValue = string.Format("\"{0}\"", propertyValue);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    propertyValue = Convert.ToBoolean(property.GetValue(this)) ? "true" : "false";
                }

                propertyPairs.Add(string.Format("{0}={1}", propertyName, propertyValue));
            }

            writer.Write("{");
            writer.Write(string.Join(",", propertyPairs));
            writer.Write("}");
        }
    }
}