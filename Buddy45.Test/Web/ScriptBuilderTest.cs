using Buddy.Web;
using NUnit.Framework;

namespace Buddy.Test.Web
{
    public class ScriptBuilderTest
    {
        [Test]
        public void BuildsSingleScript()
        {
            var scriptBuilder = new ScriptBuilder
            {
                new ScriptDef()
            };

            var html = scriptBuilder.Html;

            var expected = "<script></script>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleScriptWithSrc()
        {
            var scriptBuilder = new ScriptBuilder
            {
                new ScriptDef { Src="../src/test.js" }
            };

            var html = scriptBuilder.Html;

            var expected = "<script src=\"../src/test.js\"></script>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleScriptWithSrcAndJsType()
        {
            var scriptBuilder = new ScriptBuilder
            {
                new ScriptDef { Src="http://cdn.com/test.js", Type = ScriptType.Javascript }
            };

            var html = scriptBuilder.Html;

            var expected = "<script src=\"http://cdn.com/test.js\" type=\"text/javascript\"></script>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleScriptWithBody()
        {
            var scriptBuilder = new ScriptBuilder
            {
                new ScriptDef { Body = "function() {}" }
            };

            var html = scriptBuilder.Html;

            var expected = "<script>function() {}</script>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleScriptWithJsTypeAndBody()
        {
            var scriptBuilder = new ScriptBuilder
            {
                new ScriptDef { Type = ScriptType.Javascript, Body = "function() {}" }
            };

            var html = scriptBuilder.Html;

            var expected = "<script type=\"text/javascript\">function() {}</script>\r\n";

            Assert.AreEqual(expected, html);
        }
        

        [Test]
        public void BuildsMultipleScripts()
        {
            var scriptBuilder = new ScriptBuilder
            {
                new ScriptDef(),
                new ScriptDef { Src="../src/test.js" },
                new ScriptDef { Src="http://cdn.com/test.js", Type = ScriptType.Javascript },
                new ScriptDef { Body = "function() {}" },
                new ScriptDef { Type = ScriptType.Javascript, Body = "function() {}" }
            };

            var html = scriptBuilder.Html;

            var expected = "<script></script>\r\n";
            expected += "<script src=\"../src/test.js\"></script>\r\n";
            expected += "<script src=\"http://cdn.com/test.js\" type=\"text/javascript\"></script>\r\n";
            expected += "<script>function() {}</script>\r\n";
            expected += "<script type=\"text/javascript\">function() {}</script>\r\n";

            Assert.AreEqual(expected, html);
        }

    }
}