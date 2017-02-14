using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Buddy.Utility;

namespace Buddy.Web.UrlQuery
{
    
    /// <summary>
    /// Attribute used to decorate properties in query classes to allow serialization to/from query strings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UrlQueryParamAttribute : Attribute
    {
        public readonly string UrlKey;
        public IUrlConverter Converter { get; set; }
        
        public UrlQueryParamAttribute(Type converterType, string urlKey)
        {
            UrlKey = urlKey;

            if(!typeof(IUrlConverter).IsAssignableFrom(converterType))
                throw new ArgumentException($"converterType is not an instance of IUrlConverter {converterType.Name}");
            
            Converter = new ReflectionType(converterType).CreateObject<IUrlConverter>();
            if(Converter == null)
                throw new ArgumentException($"Could not create instance of converterType ({converterType.Name})");
        }
    }


    /// <summary>
    /// Extensions that make use of UrlQueryParamAttribute to serialize to/from query strings
    /// </summary>
    public static class UrlQueryExtensions
    {
        /// <summary>
        /// Converts a query string into a query object using the parameters in the query
        /// to populate the object
        /// </summary>
        /// <param name="queryString">The source query string</param>
        /// <param name="queryObjectType">The target object type</param>
        /// <returns>The target object</returns>
        public static object ToQueryObject(this string queryString, Type queryObjectType)
        {
            var source = QueryStringToDictionary(HttpUtility.UrlDecode(queryString));
            var target = new ReflectionType(queryObjectType).CreateObject();

            var properties = target.GetType().GetProperties();
            
            foreach (var pi in properties)
            {
                var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
                if (queryAttr != null)
                {
                    queryAttr.Converter.FromUrl(source, target, pi, queryAttr);
                }
            }

            return target;
        }

        /// <summary>
        /// Converts a query string into a query object using the parameters in the query
        /// to populate the object.  Convenience method for when the target object type is
        /// known at runtime
        /// </summary>
        /// <typeparam name="TQuery">The target object type</typeparam>
        /// <param name="queryString">The source query string</param>
        /// <returns>The target object</returns>
        public static TQuery ToQueryObject<TQuery>(this string queryString)
        {
            return (TQuery)queryString.ToQueryObject(typeof(TQuery));
        }
               

        /// <summary>
        /// Converts query parameters in an HttpRequest to a search criteria object. 
        /// Convenience method for when the target object type is known at runtime
        /// </summary>
        /// <typeparam name="TQuery">The type of Search Criteria object to be created</typeparam>
        /// <param name="nvc">The NameValueCollection containing the query parameters</param>
        /// <returns>The search criteria object populated with the query parameter values</returns>
        public static TQuery ToQueryObject<TQuery>(this HttpRequest request)
            where TQuery : class, new()
        {
            var pathQuery = request.RawUrl.Split(new[] {'?'}, StringSplitOptions.RemoveEmptyEntries);
            if (pathQuery.Length == 2)
                return (TQuery)pathQuery[1].ToQueryObject(typeof(TQuery));
            else
                return (TQuery)string.Empty.ToQueryObject(typeof(TQuery));
        }
        
        /// <summary>
        /// Converts a search criteria object to a URL format collection of name/value pairs
        /// separated by an ampersand
        /// </summary>
        /// <param name="query">The query object to convert</param>
        /// <returns>A URL query string</returns>
        public static string ToQueryString<TQuery>(this TQuery query)
            where TQuery : class, new()
        {
            var properties = query.GetType().GetProperties();
            var queryElements = new List<string>();

            // For each property with the UrlQueryParam attribut set, attempt to use the property's
            // converter to turn it into a query element.
            foreach (var pi in properties)
            {
                var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
                if (queryAttr == null)
                    continue;

                var queryElement = queryAttr.Converter.ToUrl(query, pi, queryAttr);
                if(!string.IsNullOrWhiteSpace(queryElement))
                    queryElements.Add(queryElement);
            }

            //var encoded = Nancy.Helpers.HttpUtility.UrlEncode(queryElements.Aggregate((c, n) => string.Concat(c, "&", n)));
            return queryElements.Aggregate((c, n) => string.Concat(c, "&", n));
        }


        public static Dictionary<string, string> QueryStringToDictionary(string queryString)
        {
            var result = new Dictionary<string, string>();
            var queryElements = queryString.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var queryElement in queryElements)
            {
                var keyValue = queryElement.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                if(keyValue.Length == 1)
                    result.Add(keyValue[0], keyValue[0]);
                else if(keyValue.Length == 2)
                    result.Add(keyValue[0], keyValue[1]);
            }
            return result;
        }

        /// <summary>
        /// Create a dictionary of query parameter to property containing all the properties that
        /// have the urlKey attribute.
        /// </summary>
        /// <param name="target">The target object instance</param>
        /// <returns>A dictionary of query parameter names to properties to be set for that name</returns>
        //private static IReadOnlyDictionary<string, Tuple<PropertyInfo, IUrlConverter>> GetQueryProperties<TQuery>(TQuery target)
        //    where TQuery : class, new()
        //{
        //    // Create a dictionary of query parameter to property containing all the properties that
        //    // have the urlKey attribute.
        //    return target.GetType().GetProperties()
        //        .Where(p => p.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) != null)
        //        .ToDictionary(p => ((UrlQueryParamAttribute) p.GetCustomAttributes(true).First(a => a is UrlQueryParamAttribute)).UrlKey,
        //                      p => new Tuple<PropertyInfo, IUrlConverter>(p, ((UrlQueryParamAttribute) p.GetCustomAttributes(true).First(a => a is UrlQueryParamAttribute)).Converter));
        //}


        /// <summary>
        /// Writes source properties to a target dictionary
        /// Does not write empty values
        /// </summary>
        /// <param name="source">The source instance</param>
        /// <param name="target">The target dictionary</param>
        //private static void ToQueryParameters<TQuery>(TQuery source, Dictionary<string, string> target)
        //    where TQuery : class, new()
        //{
        //    var queryProps = GetQueryProperties(source);

        //    foreach (var prop in queryProps)
        //    {
        //        var value = prop.Value.Item2.ToUrl(source, prop.Value.Item1);
        //        if(!string.IsNullOrWhiteSpace(value))
        //            target.Add(prop.Key, value);
        //    }
        //}


        /// <summary>
        ///     Writes query parameters from a source dictionary to the target object
        /// </summary>
        /// <param name="source">Source dictionary of query parameters</param>
        /// <param name="target">The target object instance</param>
        private static void ToQueryObject(IReadOnlyDictionary<string, string> source, object target)
        {
            var properties = target.GetType().GetProperties();
            
            foreach (var pi in properties)
            {
                var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
                if (queryAttr != null)
                {
                    queryAttr.Converter.FromUrl(source, target, pi, queryAttr);
                }
            }
        }
    }


    public interface IUrlConverter
    {
        string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr);
        void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr);
    }


    public class StringConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(string))
                throw new ArgumentException("Expecting a string type");

            var value = Convert.ToString(pi.GetValue(source));
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return $"{queryAttr.UrlKey}={value}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(string))
                throw new ArgumentException("Expecting a string type");
            
            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, string.Empty);
                return;
            }

            pi.SetValue(target, source[queryAttr.UrlKey]);
        }
    }

    public class Int32Converter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(int))
                throw new ArgumentException("Expecting a Int32 type");
            
            return $"{queryAttr.UrlKey}={pi.GetValue(source)}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(int))
                throw new ArgumentException("Expecting a Int32 type");

            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, 0);
                return;
            }

            int parsed = 0;
            if(int.TryParse(source[queryAttr.UrlKey], out parsed))
                pi.SetValue(target, parsed);
            else
                pi.SetValue(target, 0);
        }
    }

    public class BoolConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(bool))
                throw new ArgumentException("Expecting a bool type");
            
            var value = Convert.ToBoolean(pi.GetValue(source)) ? "t" : "f";
            return $"{queryAttr.UrlKey}={value}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(bool))
                throw new ArgumentException("Expecting a bool type");

            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, false);
                return;
            }
            
            if(source[queryAttr.UrlKey] == "True" || source[queryAttr.UrlKey] == "true" || source[queryAttr.UrlKey] == "t")
                pi.SetValue(target, true);
            else
                pi.SetValue(target, false);
        }
    }

    public class IsoDateConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(DateTime))
                throw new ArgumentException("Expecting a DateTime type");

            var value = Convert.ToDateTime(pi.GetValue(source));
            if (value == default(DateTime))
                return "";
            
            return $"{queryAttr.UrlKey}={Convert.ToDateTime(pi.GetValue(source)).ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (pi.PropertyType != typeof(DateTime))
                throw new ArgumentException("Expecting a DateTime type");

            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, default(DateTime));
                return;
            }

            pi.SetValue(target, DateTime.Parse(source[queryAttr.UrlKey]).ToLocalTime());
        }
    }

    public class EnumIntConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!pi.PropertyType.IsEnum)
                throw new ArgumentException("Expecting an enumerated type");
            
            return $"{queryAttr.UrlKey}={Convert.ToInt32(pi.GetValue(source))}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!pi.PropertyType.IsEnum)
                throw new ArgumentException("Expecting an enumerated type");

            if (!source.ContainsKey(queryAttr.UrlKey))
                return;

            pi.SetValue(target, System.Enum.Parse(pi.PropertyType, source[queryAttr.UrlKey]));
        }
    }

    public class EnumStringConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!pi.PropertyType.IsEnum)
                throw new ArgumentException("Expecting an enumerated type");
            
            return $"{queryAttr.UrlKey}={pi.GetValue(source).ToString()}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!pi.PropertyType.IsEnum)
                throw new ArgumentException("Expecting an enumerated type");

            if (!source.ContainsKey(queryAttr.UrlKey))
                return;

            pi.SetValue(target, System.Enum.Parse(pi.PropertyType, source[queryAttr.UrlKey]));
        }
    }


    /// <summary>
    /// Converts an integer array to/from a url value.  Uses a semicolon as the separator.
    /// </summary>
    public class IntArrayConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!typeof(int[]).IsAssignableFrom(pi.PropertyType))
                throw new ArgumentException("Expecting an int[] type");

            var value = pi.GetValue(source) as int[];
            if (value == null || value.Length == 0)
                return "";
            
            return $"{queryAttr.UrlKey}={value.Select(v => v.ToString()).Aggregate((c, n) => string.Concat(c, ";", n))}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!typeof(int[]).IsAssignableFrom(pi.PropertyType))
                throw new ArgumentException("Expecting an int[] type");
            
            var result = new List<int>();

            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, result.ToArray());
                return;
            }

            var elements = source[queryAttr.UrlKey].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach(var element in elements)
            {
                int intValue;
                if(Int32.TryParse(element, out intValue))
                    result.Add(intValue);
            }

            pi.SetValue(target, result.ToArray());
        }
    }

    /// <summary>
    /// Converts an string array to/from a url value.  Uses a semicolon as the separator.
    /// </summary>
    public class StringArrayConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!typeof(string[]).IsAssignableFrom(pi.PropertyType))
                throw new ArgumentException("Expecting a string[] type");

            var value = pi.GetValue(source) as string[];
            if (value == null || value.Length == 0)
                return "";
            
            return $"{queryAttr.UrlKey}={value.Aggregate((c, n) => string.Concat(c, ";", n))}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!typeof(string[]).IsAssignableFrom(pi.PropertyType))
                throw new ArgumentException("Expecting a string[] type");
            
            var result = new List<string>();

            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, result.ToArray());
                return;
            }

            result = source[queryAttr.UrlKey].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).ToList();

            pi.SetValue(target, result.ToArray());
        }
    }


    /// <summary>
    /// Converts an enum array to/from a url value.  Uses a semicolon as the separator.
    /// </summary>
    public class EnumIntArrayConverter : IUrlConverter
    {
        public string ToUrl(object source, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!pi.PropertyType.IsArray || !pi.PropertyType.GetElementType().IsEnum)
                throw new ArgumentException("Expecting an Enum[] type");

            var value = pi.GetValue(source) as Array;
            if (value == null || value.Length == 0)
                return "";

            return $"{queryAttr.UrlKey}={value.Cast<object>().Select(v => Convert.ToInt32(v).ToString()).Aggregate((c, n) => string.Concat(c, ";", n))}";
        }

        public void FromUrl(IReadOnlyDictionary<string, string> source, object target, PropertyInfo pi, UrlQueryParamAttribute queryAttr)
        {
            if (!pi.PropertyType.IsArray || !pi.PropertyType.GetElementType().IsEnum)
                throw new ArgumentException("Expecting an Enum[] type");
            
            if (!source.ContainsKey(queryAttr.UrlKey))
            {
                pi.SetValue(target, Array.CreateInstance(pi.PropertyType.GetElementType(), 0));
                return;
            }
            
            var elements = source[queryAttr.UrlKey].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            var result = Array.CreateInstance(pi.PropertyType.GetElementType(), elements.Length);

            for(var i = 0 ; i < elements.Length ; i++)
            {
                result.SetValue(Convert.ChangeType(System.Enum.Parse(pi.PropertyType.GetElementType(), elements[i]), pi.PropertyType.GetElementType()), i);
            }

            pi.SetValue(target, result);
        }
    }

}