namespace Buddy.Web
{
    public class LinkDef
    {
        public LinkDef()
        {
            Type = LinkType.None;
            Rel = LinkRelType.None;
        }

        public string Href { get; set; }
        public LinkType Type { get; set; }
        public LinkRelType Rel { get; set; }
    }
}