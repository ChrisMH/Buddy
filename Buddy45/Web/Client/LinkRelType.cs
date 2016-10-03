using System.ComponentModel;

namespace Buddy.Web.Client
{
    public enum LinkRelType
    {
        [Description("text/javascript")]
        None,

        [Description("stylesheet")]
        Stylesheet,

        [Description("icon")]
        Icon,

        [Description("shortcut icon")]
        ShortcutIcon
    }
}