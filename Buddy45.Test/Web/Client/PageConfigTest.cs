using System;
using System.Reflection;
using System.Web;
using Buddy.Web.Client;
using NUnit.Framework;

namespace Buddy.Test.Web.Client
{
    public class PageConfigTest
    {
        [Test]
        public void ExportsCorrectJson()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly());

            var json = pageConfig.Json;

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = HttpUtility.HtmlEncode($"{{\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\"}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJsonInDerivedPageConfig()
        {
            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly()) {UserName = "chogan"};

            var json = pageConfig.Json;

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected =
                HttpUtility.HtmlEncode(
                    $"{{\"userName\":\"chogan\",\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\"}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJavascript()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly());

            var javascript = pageConfig.ToJavascript();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"page\")) window.page={};" +
                           "window.page.config={" +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"" +
                           "};})();";


            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectNestedObjectNameJavascript()
        {
            var pageConfig = new PageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly());

            var javascript = pageConfig.ToJavascript("App.config.sectionConfig.lineConfig");

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"App\")) window.App={};" +
                           "if(!window.App.hasOwnProperty(\"config\")) window.App.config={};" +
                           "if(!window.App.config.hasOwnProperty(\"sectionConfig\")) window.App.config.sectionConfig={};" +
                           "window.App.config.sectionConfig.lineConfig={" +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"" +
                           "};})();";


            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectJavascriptInDerivedPageConfig()
        {
            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly()) {UserName = "chogan"};

            var javascript = pageConfig.ToJavascript();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"page\")) window.page={};" +
                           "window.page.config={" +
                           "userName:\"chogan\"," +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"" +
                           "};})();";

            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectJavascriptWhenContainsClass()
        {
            var pageConfig = new ContainedClassPageConfig("http://www.test.com/", "http://www.test.com/Virtual", Assembly.GetExecutingAssembly())
                {ContainedClass = new ContainedClass("god", "1234567")};

            var javascript = pageConfig.ToJavascript();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            Console.WriteLine(javascript);
            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"page\")) window.page={};" +
                           "window.page.config={containedClass:{password:\"god\",phoneNumber:\"1234567\"}," +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"" +
                           "};})();";

            Assert.AreEqual(expected, javascript);
        }


        internal class DerivedPageConfig : PageConfig
        {
            public DerivedPageConfig(string originUrl, string rootUrl, Assembly versionAssembly)
                : base(originUrl, rootUrl, versionAssembly)
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
            public ContainedClassPageConfig(string originUrl, string rootUrl, Assembly versionAssembly)
                : base(originUrl, rootUrl, versionAssembly)
            {
            }

            public ContainedClass ContainedClass { get; set; }
        }
    }
}