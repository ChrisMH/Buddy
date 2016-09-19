using System.ComponentModel;

namespace Buddy.Web
{
    public enum LinkType
    {
        [Description("text/javascript")]
        None,

        [Description("text/css")]
        Css,

        [Description("image/icon")]
        Icon,

        [Description("image/x-icon")]
        XIcon
    }
}