using System.Text;
using Buddy.Enum;

namespace Buddy.Web
{
    public class TitleElem : IHtmlElem
    {
        public TitleElem()
        {
        }

        public string Title { get; set; }

        public virtual string Html
        {
            get
            {
                return $"<title>{Title}</title>";
            }
        }
    }
}