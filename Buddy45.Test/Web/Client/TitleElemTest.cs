using Buddy.Web.Client;
using NUnit.Framework;

namespace Buddy45.Test.Web.Client
{
    public class TitleElemTest
    {
        [Test]
        public void BuildsEmptyTitle()
        {
            var title = new TitleElem();

            var html = title.Html;

            var expected = "<title></title>";

            Assert.AreEqual(expected, html);
        }

        [Test]
        public void BuildsTitleWithTitle()
        {
            var title = new TitleElem { Title = "Application Title" };

            var html = title.Html;

            var expected = "<title>Application Title</title>";

            Assert.AreEqual(expected, html);
        }
        
    }
}