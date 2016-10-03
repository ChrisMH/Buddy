using Buddy.Web.Client;
using NUnit.Framework;

namespace Buddy.Test.Web.Client
{
    public class HtmlBuliderTest
    {
        [Test]
        public void BuildsMultipleHtmlDefTypes()
        {
            var htmlBuilder = new HtmlBuilder
            {
                new BaseElem {Href = "http://a.website.com/"},
                new LinkElem { Href="favicon.ico", Type = LinkType.XIcon, Rel= LinkRelType.ShortcutIcon },
                new LinkElem {Href = "img/icon.png", Type = LinkType.Icon},
                new LinkElem(),
                new LinkElem {Href = "../css/test.css"},
                new LinkElem {Href = "http://localhost/test.css", Type = LinkType.Css},
                new LinkElem {Href = "https://cdn.net/test/test.css", Rel = LinkRelType.Stylesheet},
                new LinkElem {Href = "test.css", Type = LinkType.Css, Rel = LinkRelType.Stylesheet},
                new ScriptElem(),
                new ScriptElem {Src = "../src/test.js"},
                new ScriptElem {Src = "http://cdn.com/test.js", Type = ScriptType.Javascript},
                new ScriptElem {Body = "function() {}"},
                new ScriptElem {Type = ScriptType.Javascript, Body = "function() {}"}
            };

            var html = htmlBuilder.Html;

            var expected = "<base href=\"http://a.website.com/\">\r\n" +
                           "<link href=\"favicon.ico\" type=\"image/x-icon\" rel=\"shortcut icon\">\r\n" +
                           "<link href=\"img/icon.png\" type=\"image/icon\">\r\n" +
                           "<link>\r\n" +
                           "<link href=\"../css/test.css\">\r\n" +
                           "<link href=\"http://localhost/test.css\" type=\"text/css\">\r\n" +
                           "<link href=\"https://cdn.net/test/test.css\" rel=\"stylesheet\">\r\n" +
                           "<link href=\"test.css\" type=\"text/css\" rel=\"stylesheet\">\r\n" +
                           "<script></script>\r\n" +
                           "<script src=\"../src/test.js\"></script>\r\n" +
                           "<script src=\"http://cdn.com/test.js\" type=\"text/javascript\"></script>\r\n" +
                           "<script>function() {}</script>\r\n" +
                           "<script type=\"text/javascript\">function() {}</script>\r\n";

            Assert.AreEqual(expected, html);

        }
    }
}