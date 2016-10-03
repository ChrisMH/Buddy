using System.ComponentModel;

namespace Buddy.Web.Client
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