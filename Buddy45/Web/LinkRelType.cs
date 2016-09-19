using System.ComponentModel;

namespace Buddy.Web
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