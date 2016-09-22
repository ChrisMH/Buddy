using System;
using System.Collections;
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

        public void Serialize(TextWriter writer, object source)
        {
            if (source == null)
            {
                writer.Write("null");
                return;
            }

            var sourceType = source.GetType();

            if (sourceType == typeof(string)
                || sourceType == typeof(bool)
                || sourceType == typeof(long)
                || sourceType == typeof(ulong)
                || sourceType == typeof(int)
                || sourceType == typeof(uint)
                || sourceType == typeof(short)
                || sourceType == typeof(ushort)
                || sourceType == typeof(byte)
                || sourceType == typeof(sbyte)
                || sourceType == typeof(double)
                || sourceType == typeof(float))
            {
                SerializeSimpleType(writer, source);
            }
            else if (typeof(IDictionary).IsAssignableFrom(sourceType))
            {
                SerializeDictionary(writer, source);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(sourceType))
            {
                SerializeEnumerable(writer, source);
            }
            else if (sourceType.IsClass)
            {
                SerializeClass(writer, source);
            }
        }

        protected void SerializeClass(TextWriter writer, object source)
        {
            writer.Write("{");

            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var first = true;
            foreach(var property in properties)
            {
                if (property.GetCustomAttribute(typeof(JsIgnoreAttribute)) != null)
                    continue;

                if (!first)
                    writer.Write(",");

                writer.Write(ToCamelCase(property.Name));
                writer.Write(":");
                Serialize(writer, property.GetValue(source));

                first = false;
            }
            writer.Write("}");
        }
        
        protected void SerializeEnumerable(TextWriter writer, object source)
        {
            writer.Write("[");
            var first = true;
            foreach (var value in (IEnumerable)source)
            {
                if (!first)
                    writer.Write(",");
                Serialize(writer, value);
                first = false;
            }

            writer.Write("]");
        }

        protected void SerializeDictionary(TextWriter writer, object source)
        {
            writer.Write("{");
            var first = true;
            foreach (var value in (IDictionary)source)
            {
                if (!first)
                    writer.Write(",");

                var de = (DictionaryEntry)value;
                Serialize(writer, de.Key);
                writer.Write(":");
                Serialize(writer, de.Value);
                first = false;
            }
            writer.Write("}");
        }


        protected void SerializeSimpleType(TextWriter writer, object source)
        {
            var propertyValue = source.ToString();

            if (source is string)
            {
                propertyValue = $"\"{propertyValue}\"";
            }
            else if (source is bool)
            {
                propertyValue = Convert.ToBoolean(propertyValue) ? "true" : "false";
            }
            
            writer.Write(propertyValue);
        }

        private string ToCamelCase(string value)
        {
            var firstChar = value.Substring(0, 1);
            return firstChar.ToLower() + value.Substring(1);
        }
    }
}