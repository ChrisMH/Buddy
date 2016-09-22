using Buddy.Web;
using NUnit.Framework;

namespace Buddy.Test.Web
{
    public class BaseElemTest
    {
        [Test]
        public void BuildsEmptyBase()
        {
            var link = new BaseElem();

            var html = link.Html;

            var expected = "<base>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsBaseWithHref()
        {
            var link = new BaseElem {Href = "http://a.website.com/"};

            var html = link.Html;

            var expected = "<base href=\"http://a.website.com/\">";

            Assert.AreEqual(expected, html);
        }
        [Test]
        public void BuildsBaseWithTarget()
        {
            var link = new BaseElem { Target = "_self" };

            var html = link.Html;

            var expected = "<base target=\"_self\">";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsBaseWithHrefAndTarget()
        {
            var link = new BaseElem { Href = "http://a.website.com/", Target="_self" };

            var html = link.Html;

            var expected = "<base href=\"http://a.website.com/\" target=\"_self\">";

            Assert.AreEqual(expected, html);
        }

    }
}