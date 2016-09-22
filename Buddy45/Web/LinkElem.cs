using System.Text;
using Buddy.Enum;

namespace Buddy.Web
{
    public class LinkElem : IHtmlElem
    {
        public LinkElem()
        {
            Type = LinkType.None;
            Rel = LinkRelType.None;
        }

        public string Href { get; set; }
        public LinkType Type { get; set; }
        public LinkRelType Rel { get; set; }

        public virtual string Html
        {
            get
            {
                var html = new StringBuilder("<link");
                if (!string.IsNullOrWhiteSpace(Href))
                    html.AppendFormat(@" href=""{0}""", Href);
                if (Type != LinkType.None)
                    html.AppendFormat(@" type=""{0}""", Type.GetDescription());
                if (Rel != LinkRelType.None)
                    html.AppendFormat(@" rel=""{0}""", Rel.GetDescription());
                html.Append(">");

                return html.ToString();
            }
        }
    }
}