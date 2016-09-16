using Buddy.Web;
using NUnit.Framework;

namespace Buddy.Test.Web
{
    public class LinkBuilderTest
    {
        [Test]
        public void BuildsSingleLink()
        {
            var linkBuilder = new LinkBuilder
            {
                new LinkDef()
            };

            var html = linkBuilder.Html;

            var expected = "<link/>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleLinkWithHref()
        {
            var linkBuilder = new LinkBuilder
            {
                new LinkDef { Href="../css/test.css" }
            };

            var html = linkBuilder.Html;

            var expected = "<link href=\"../css/test.css\"/>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleLinkWithHrefAndCssType()
        {
            var linkBuilder = new LinkBuilder
            {
                new LinkDef { Href="http://localhost/test.css", Type = LinkType.Css }
            };

            var html = linkBuilder.Html;

            var expected = "<link href=\"http://localhost/test.css\" type=\"text/css\"/>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleLinkWithHrefAndRel()
        {
            var linkBuilder = new LinkBuilder
            {
                new LinkDef { Href="https://cdn.net/test/test.css", Rel = LinkRelType.Stylesheet }
            };

            var html = linkBuilder.Html;

            var expected = "<link href=\"https://cdn.net/test/test.css\" rel=\"stylesheet\"/>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsSingleLinkWithHrefAndTypeAndRel()
        {
            var linkBuilder = new LinkBuilder
            {
                new LinkDef { Href="test.css", Type = LinkType.Css, Rel = LinkRelType.Stylesheet }
            };

            var html = linkBuilder.Html;

            var expected = "<link href=\"test.css\" type=\"text/css\" rel=\"stylesheet\"/>\r\n";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsMultipleLinks()
        {
            var linkBuilder = new LinkBuilder
            {
                new LinkDef { Href = "img/icon.png", Type = LinkType.Icon },
                new LinkDef(),
                new LinkDef { Href="../css/test.css" },
                new LinkDef { Href="http://localhost/test.css", Type = LinkType.Css },
                new LinkDef { Href="https://cdn.net/test/test.css", Rel = LinkRelType.Stylesheet },
                new LinkDef { Href="test.css", Type = LinkType.Css, Rel = LinkRelType.Stylesheet }
            };

            var html = linkBuilder.Html;

            var expected = "<link href=\"img/icon.png\" type=\"image/icon\"/>\r\n";
            expected += "<link/>\r\n";
            expected += "<link href=\"../css/test.css\"/>\r\n";
            expected += "<link href=\"http://localhost/test.css\" type=\"text/css\"/>\r\n";
            expected += "<link href=\"https://cdn.net/test/test.css\" rel=\"stylesheet\"/>\r\n";
            expected += "<link href=\"test.css\" type=\"text/css\" rel=\"stylesheet\"/>\r\n";

            Assert.AreEqual(expected, html);
        }

    }
}