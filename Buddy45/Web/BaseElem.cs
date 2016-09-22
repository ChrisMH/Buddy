using System.Text;
using Buddy.Enum;

namespace Buddy.Web
{
    public class BaseElem : IHtmlElem
    {
        public BaseElem()
        {
        }

        public string Href { get; set; }

        public string Target { get; set; }

        public virtual string Html
        {
            get
            {
                var html = new StringBuilder("<base");
                if (!string.IsNullOrWhiteSpace(Href))
                    html.AppendFormat(@" href=""{0}""", Href);
                if (!string.IsNullOrWhiteSpace(Target))
                    html.AppendFormat(@" target=""{0}""", Target);
                html.Append(">");

                return html.ToString();
            }
        }
    }
}