using System;
using System.Reflection;
using System.Web;
using Buddy.Web.Client;
using NUnit.Framework;

namespace Buddy45.Test.Web.Client
{
    public class PageConfigTest
    {
        [Test]
        public void ExportsCorrectJson()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var pageConfig = new AppSettings("http://www.test.com/", "http://www.test.com/Virtual", version);

            var json = pageConfig.Json;


            var expected = HttpUtility.HtmlEncode($"{{\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\"}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJsonInDerivedPageConfig()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", version) {UserName = "chogan"};

            var json = pageConfig.Json;
            
            var expected =
                HttpUtility.HtmlEncode(
                    $"{{\"userName\":\"chogan\",\"originUrl\":\"http://www.test.com/\",\"rootUrl\":\"http://www.test.com/Virtual\",\"version\":\"{version}\"}}");

            Assert.AreEqual(expected, json);
        }

        [Test]
        public void ExportsCorrectJavascript()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var pageConfig = new AppSettings("http://www.test.com/", "http://www.test.com/Virtual", version);

            var javascript = pageConfig.ToJavascript();


            var expected = "(function(){" +
                           "if(!window.hasOwnProperty(\"app\")) window.app={};" +
                           "window.app.settings={" +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"" +
                           "};})();";


            Assert.AreEqual(expected, javascript);
        }

        [Test]
        public void ExportsCorrectNestedObjectNameJavascript()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var pageConfig = new AppSettings("http://www.test.com/", "http://www.test.com/Virtual", version);

            var javascript = pageConfig.ToJavascript("App.config.sectionConfig.lineConfig");
            
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
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var pageConfig = new DerivedPageConfig("http://www.test.com/", "http://www.test.com/Virtual", version) {UserName = "chogan"};

            var javascript = pageConfig.ToJavascript();
            
            var expected = "(function(){" +
						   "if(!window.hasOwnProperty(\"app\")) window.app={};" +
						   "window.app.settings={" +
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
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var pageConfig = new ContainedClassPageConfig("http://www.test.com/", "http://www.test.com/Virtual", version)
                {ContainedClass = new ContainedClass("god", "1234567")};

            var javascript = pageConfig.ToJavascript();
            
            Console.WriteLine(javascript);
            var expected = "(function(){" +
						   "if(!window.hasOwnProperty(\"app\")) window.app={};" +
						   "window.app.settings={containedClass:{password:\"god\",phoneNumber:\"1234567\"}," +
                           "originUrl:\"http://www.test.com/\"," +
                           "rootUrl:\"http://www.test.com/Virtual\"," +
                           $"version:\"{version}\"" +
                           "};})();";

            Assert.AreEqual(expected, javascript);
        }


        internal class DerivedPageConfig : AppSettings
        {
            public DerivedPageConfig(string originUrl, string rootUrl, string version)
                : base(originUrl, rootUrl, version)
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

        internal class ContainedClassPageConfig : AppSettings
        {
            public ContainedClassPageConfig(string originUrl, string rootUrl, string version)
                : base(originUrl, rootUrl, version)
            {
            }

            public ContainedClass ContainedClass { get; set; }
        }
    }
}