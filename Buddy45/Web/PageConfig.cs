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
        public PageConfig(string originUrl, string rootUrl, Assembly versionAssembly)
        {
            OriginUrl = originUrl;
            RootUrl = rootUrl;
            if (versionAssembly != null)
            {
                var version = versionAssembly.GetName().Version.ToString();
                Version = version.Substring(0, version.LastIndexOf('.'));
            }
        }

        public string OriginUrl { get; set; }
        public string RootUrl { get; set; }
        public string Version { get; set; }

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
        public string ToJavascript(string objectName = "app.pageConfig")
        {
            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("objectName cannot be empty", nameof(objectName));

            var serialized = new StringBuilder();
            using (var writer = new StringWriter(serialized))
            {
                var serializer = new JsSerializer.JsSerializer();
                serializer.Serialize(writer, this);
            }
            
            var javascript = new StringBuilder();

            javascript.Append("(function(){");
            
            var subObjects = objectName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var objSoFar = "window";
            for (var i = 0; i < subObjects.Length - 1; i++)
            {
                javascript.AppendFormat("if(!{0}.hasOwnProperty(\"{1}\")) {0}.{1}={{}};", objSoFar, subObjects[i]);
                objSoFar = string.Concat(objSoFar, ".", subObjects[i]);
            }
            javascript.AppendFormat("window.{0}=", objectName);
            javascript.Append(serialized);
            javascript.Append(";})();");

            return javascript.ToString();
        }
    }
}