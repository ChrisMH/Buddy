using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Buddy.JsSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Buddy.Web
{
    /// <summary>
    /// Base class for web page configuration information, exporting as a JSON object or Javascript
    /// </summary>
    public class PageConfig
    {
        public PageConfig(string originUrl, string rootUrl, Assembly versionAssembly, bool debug)
        {
            OriginUrl = originUrl;
            RootUrl = rootUrl;
            if (versionAssembly != null)
            {
                var version = versionAssembly.GetName().Version.ToString();
                Version = version.Substring(0, version.LastIndexOf('.'));
            }
            Debug = debug;
        }

        public string OriginUrl { get; set; }
        public string RootUrl { get; set; }
        public string Version { get; set; }
        public bool Debug { get; set; }

        /// <summary>
        /// Exports all properties as a JSON object
        /// </summary>
        [JsonIgnore]
        [JsIgnore]
        public string Json
        {
            get
            {
                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        var serializer = new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                        serializer.Serialize(writer, this);
                    }
                }

                return HttpUtility.HtmlEncode(sb.ToString());
            }
        }

        /// <summary>
        /// Export all public properties as Javascript in an object on window.
        /// </summary>
        /// <param name="objectName">Name of the enclosing struct</param>
        /// <returns>Javascript</returns>
        public string ToJavascript(string objectName = "App.pageConfig")
        {
            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("objectName cannot be empty", nameof(objectName));

            var propertyPairs = new List<string>();

            var properties = this.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance);
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

                propertyPairs.Add(string.Format("{0}:{1}", propertyName, propertyValue));
            }
            
            var sb = new StringBuilder();

            sb.Append("(function(){");
            
            var subObjects = objectName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var objSoFar = "window";
            for (var i = 0; i < subObjects.Length - 1; i++)
            {
                sb.AppendFormat("if(!{0}.hasOwnProperty(\"{1}\")) {0}.{1}={{}};", objSoFar, subObjects[i]);
                objSoFar = string.Concat(objSoFar, ".", subObjects[i]);
            }
            sb.AppendFormat("window.{0}={{", objectName);
            sb.Append(string.Join(",", propertyPairs));
            sb.Append("};})();");

            return sb.ToString();
        }
    }
}