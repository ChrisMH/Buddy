using System.ComponentModel;

namespace Buddy.Web
{
    public enum LinkRelType
    {
        [Description("text/javascript")]
        None,

        [Description("stylesheet")]
        Stylesheet
    }
}