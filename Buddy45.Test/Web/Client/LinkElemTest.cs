using Buddy.Web.Client;
using NUnit.Framework;

namespace Buddy45.Test.Web.Client
{
    public class LinkElemTest
    {
        [Test]
        public void BuildsEmptyLink()
        {
            var link = new LinkElem();

            var html = link.Html;

            var expected = "<link>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsLinkWithHref()
        {
            var link = new LinkElem {Href = "../css/test.css"};

            var html = link.Html;

            var expected = "<link href=\"../css/test.css\">";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsLinkWithHrefAndCssType()
        {
            var link = new LinkElem {Href = "http://localhost/test.css", Type = LinkType.Css};

            var html = link.Html;

            var expected = "<link href=\"http://localhost/test.css\" type=\"text/css\">";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsLinkWithHrefAndRel()
        {
            var link = new LinkElem {Href = "https://cdn.net/test/test.css", Rel = LinkRelType.Stylesheet};

            var html = link.Html;

            var expected = "<link href=\"https://cdn.net/test/test.css\" rel=\"stylesheet\">";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsLinkWithHrefAndTypeAndRel()
        {
            var link = new LinkElem {Href = "test.css", Type = LinkType.Css, Rel = LinkRelType.Stylesheet};

            var html = link.Html;

            var expected = "<link href=\"test.css\" type=\"text/css\" rel=\"stylesheet\">";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsMultipleLinks()
        {
            var htmlBuilder = new HtmlBuilder
            {
                new LinkElem {Href = "img/icon.png", Type = LinkType.Icon},
                new LinkElem(),
                new LinkElem {Href = "../css/test.css"},
                new LinkElem {Href = "http://localhost/test.css", Type = LinkType.Css},
                new LinkElem {Href = "https://cdn.net/test/test.css", Rel = LinkRelType.Stylesheet},
                new LinkElem {Href = "test.css", Type = LinkType.Css, Rel = LinkRelType.Stylesheet}
            };

            var html = htmlBuilder.Html;

            var expected = "<link href=\"img/icon.png\" type=\"image/icon\">\r\n" +
                           "<link>\r\n" +
                           "<link href=\"../css/test.css\">\r\n" +
                           "<link href=\"http://localhost/test.css\" type=\"text/css\">\r\n" +
                           "<link href=\"https://cdn.net/test/test.css\" rel=\"stylesheet\">\r\n" +
                           "<link href=\"test.css\" type=\"text/css\" rel=\"stylesheet\">\r\n";

            Assert.AreEqual(expected, html);
        }
    }
}