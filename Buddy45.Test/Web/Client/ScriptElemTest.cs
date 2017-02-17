using Buddy.Web.Client;
using NUnit.Framework;

namespace Buddy45.Test.Web.Client
{
    public class ScriptElemTest
    {
        [Test]
        public void BuildsEmptyScript()
        {
            var script = new ScriptElem();

            var html = script.Html;

            var expected = "<script></script>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsScriptWithSrc()
        {
            var script = new ScriptElem {Src = "../src/test.js"};

            var html = script.Html;

            var expected = "<script src=\"../src/test.js\"></script>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsScriptWithSrcAndJsType()
        {
            var script = new ScriptElem {Src = "http://cdn.com/test.js", Type = ScriptType.Javascript};

            var html = script.Html;

            var expected = "<script src=\"http://cdn.com/test.js\" type=\"text/javascript\"></script>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsScriptWithBody()
        {
            var script = new ScriptElem {Body = "function() {}"};

            var html = script.Html;

            var expected = "<script>function() {}</script>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsScriptWithJsTypeAndBody()
        {
            var script = new ScriptElem {Type = ScriptType.Javascript, Body = "function() {}"};

            var html = script.Html;

            var expected = "<script type=\"text/javascript\">function() {}</script>";

            Assert.AreEqual(expected, html);
        }


        [Test]
        public void BuildsMultipleScripts()
        {
            var htmlBuilder = new HtmlBuilder
            {
                new ScriptElem(),
                new ScriptElem {Src = "../src/test.js"},
                new ScriptElem {Src = "http://cdn.com/test.js", Type = ScriptType.Javascript},
                new ScriptElem {Body = "function() {}"},
                new ScriptElem {Type = ScriptType.Javascript, Body = "function() {}"}
            };

            var html = htmlBuilder.Html;

            var expected = "<script></script>\r\n" +
                           "<script src=\"../src/test.js\"></script>\r\n" +
                           "<script src=\"http://cdn.com/test.js\" type=\"text/javascript\"></script>\r\n" +
                           "<script>function() {}</script>\r\n" +
                           "<script type=\"text/javascript\">function() {}</script>\r\n";

            Assert.AreEqual(expected, html);
        }
    }
}