using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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
        /// Export all public properties as Javascript
        /// </summary>
        [JsonIgnore]
        public string Javascript
        {
            get
            {
                var sb = new StringBuilder();

                sb.AppendLine("(function(pageConfig) {");


                var properties = this.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance);
                foreach (var property in properties)
                {
                    if (property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null)
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

                    sb.AppendLine(string.Format("    {0} = {1}", propertyName, propertyValue));
                }

                sb.AppendLine("})(window.pageConfig = window.pageConfig || {});");

                return sb.ToString();
            }
        }
    }
}