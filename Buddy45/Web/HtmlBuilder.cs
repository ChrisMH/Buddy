using System.Collections.Generic;
using System.Text;

namespace Buddy.Web
{
    public class HtmlBuilder : List<IHtmlElem>, IHtmlElem
    
    {
        public HtmlBuilder()
        {
        }

        public HtmlBuilder(IEnumerable<IHtmlElem> collection)
            : base(collection)
        {
        }

        public virtual string Html
        {
            get
            {
                var html = new StringBuilder();
                foreach (var htmlElement in this)
                    html.AppendLine(htmlElement.Html);
                return html.ToString();
            }
        }
    }
}