using Buddy.Web;
using NUnit.Framework;

namespace Buddy.Test.Web
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