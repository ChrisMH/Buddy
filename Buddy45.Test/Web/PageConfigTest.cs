using System.Reflection;
using System.Web;
using Buddy.Web;
using NUnit.Framework;

namespace Buddy.Test.Web
{
    
    public class PageConfigTest
    {
        [Test]
        public void ExportsCorrectJson()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true);

            var json = pageConfig.Json;

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = HttpUtility.HtmlEncode($"{{\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\",\"debug\":true}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJsonInDerivedPageConfig()
        {
            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true) {UserName = "chogan"};

            var json = pageConfig.Json;

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = HttpUtility.HtmlEncode($"{{\"userName\":\"chogan\",\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\",\"debug\":true}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJavascript()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true);

            var javascript = pageConfig.Javascript;

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = $"(function(pageConfig) {{\r\n" +
                           $"    originUrl = \"http://www.test.com/\"\r\n" +
                           $"    rootUrl = \"http://www.test.com/Virtual\"\r\n" +
                           $"    version = \"{version}\"\r\n" +
                           $"    debug = true\r\n" +
                           $"}})(window.pageConfig = window.pageConfig || {{}});\r\n";
                

            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectJavascriptInDerivedPageConfig()
        {
            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true) { UserName = "chogan" };

            var javascript = pageConfig.Javascript;

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = $"(function(pageConfig) {{\r\n" +
                           $"    userName = \"chogan\"\r\n" +
                           $"    originUrl = \"http://www.test.com/\"\r\n" +
                           $"    rootUrl = \"http://www.test.com/Virtual\"\r\n" +
                           $"    version = \"{version}\"\r\n" +
                           $"    debug = true\r\n" +
                           $"}})(window.pageConfig = window.pageConfig || {{}});\r\n";
            
            Assert.AreEqual(expected, javascript);
        }

        internal class DerivedPageConfig : PageConfig
        {
            public DerivedPageConfig(string originUrl, string rootUrl, Assembly versionAssembly, bool debug)
                : base(originUrl, rootUrl, versionAssembly, debug)
            {
            }

            public string UserName { get; set; }
        }
    }
}