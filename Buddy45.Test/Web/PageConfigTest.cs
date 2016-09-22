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

            var expected =
                HttpUtility.HtmlEncode(
                    $"{{\"userName\":\"chogan\",\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\",\"debug\":true}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJavascript()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true);

            var javascript = pageConfig.ToJavascript();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"App\")) window.App={};" +
                           "window.App.pageConfig={" +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"," +
                           "debug:true" +
                           "};})();";


            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectNextedObjectJavascript()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true);

            var javascript = pageConfig.ToJavascript("App.pageConfig.sectionConfig.lineConfig");

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"App\")) window.App={};" +
                           "if(!window.App.hasOwnProperty(\"pageConfig\")) window.App.pageConfig={};" +
                           "if(!window.App.pageConfig.hasOwnProperty(\"sectionConfig\")) window.App.pageConfig.sectionConfig={};" +
                           "window.App.pageConfig.sectionConfig.lineConfig={" +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"," +
                           "debug:true" +
                           "};})();";


            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectJavascriptInDerivedPageConfig()
        {
            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true) {UserName = "chogan"};

            var javascript = pageConfig.ToJavascript();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"App\")) window.App={};" +
                           "window.App.pageConfig={" +
                           "userName:\"chogan\"," +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"," +
                           "debug:true" +
                           "};})();";

            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectJavascriptWhenContainsClass()
        {
            var pageConfig = new ContainedClassPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly(), true)
                {ContainedClass = new ContainedClass("god", "1234567")};

            var javascript = pageConfig.ToJavascript();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(pageConfig){pageConfig={" +
                           "containedClass={password=\"god\",phoneNumber=\"1234567\"}," +
                           "originUrl=\"http://www.test.com/\"," +
                           "rootUrl=\"http://www.test.com/Virtual\"," +
                           $"version=\"{version}\"," +
                           "debug=true" +
                           "};})(window.pageConfig=window.pageConfig||{});";

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

        internal class ContainedClass
        {
            public ContainedClass(string password, string phoneNumber)
            {
                Password = password;
                PhoneNumber = phoneNumber;
            }

            public string Password { get; set; }
            public string PhoneNumber { get; set; }
        }

        internal class ContainedClassPageConfig : PageConfig
        {
            public ContainedClassPageConfig(string originUrl, string rootUrl, Assembly versionAssembly, bool debug)
                : base(originUrl, rootUrl, versionAssembly, debug)
            {
            }

            public ContainedClass ContainedClass { get; set; }
        }
    }
}