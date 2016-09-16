using System.Collections.Generic;
using System.Text;
using Buddy.Enum;

namespace Buddy.Web
{
    public class LinkBuilder : List<LinkDef>
    {
        public LinkBuilder()
        {
        }

        public LinkBuilder(IEnumerable<LinkDef> collection)
            : base(collection)
        {
        }
        
        public string Html
        {
            get
            {
                var strLinkBlock = new StringBuilder();

                foreach (var link in this)
                {
                    var strLink = new StringBuilder("<link");
                    if (!string.IsNullOrWhiteSpace(link.Href))
                        strLink.AppendFormat(@" href=""{0}""", link.Href);
                    if (link.Type != LinkType.None)
                        strLink.AppendFormat(@" type=""{0}""", link.Type.GetDescription());
                    if (link.Rel != LinkRelType.None)
                        strLink.AppendFormat(@" rel=""{0}""", link.Rel.GetDescription());
                    strLink.Append("/>");

                    strLinkBlock.AppendLine(strLink.ToString());
                }

                return strLinkBlock.ToString();
            }
        }
    }
}