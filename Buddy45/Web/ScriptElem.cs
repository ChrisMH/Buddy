using System.Text;
using Buddy.Enum;

namespace Buddy.Web
{
    public class ScriptElem : IHtmlElem
    {
        public ScriptElem()
        {
            Type = ScriptType.None;
        }

        public string Src { get; set; }

        public ScriptType Type { get; set; }

        public string Body { get; set; }

        public virtual string Html
        {
            get
            {
                var html = new StringBuilder("<script");
                if (!string.IsNullOrWhiteSpace(Src))
                    html.AppendFormat(@" src=""{0}""", Src);
                if (Type != ScriptType.None)
                    html.AppendFormat(@" type=""{0}""", Type.GetDescription());
                html.AppendFormat(">{0}</script>", Body);

                return html.ToString();
            }
        }
    }
}