using System.Collections.Generic;
using System.Text;
using Buddy.Enum;

namespace Buddy.Web
{
    public class ScriptBuilder : List<ScriptDef>
    {
        public ScriptBuilder()
        {
        }

        public ScriptBuilder(IEnumerable<ScriptDef> collection)
            : base(collection)
        {
        }
        
        public string Html
        {
            get
            {
                var strScriptBlock = new StringBuilder();

                foreach (var script in this)
                {
                    var strScript = new StringBuilder("<script");
                    if (!string.IsNullOrWhiteSpace(script.Src))
                        strScript.AppendFormat(@" src=""{0}""", script.Src);
                    if (script.Type != ScriptType.None)
                        strScript.AppendFormat(@" type=""{0}""", script.Type.GetDescription());
                    strScript.AppendFormat(">{0}</script>", script.Body);

                    strScriptBlock.AppendLine(strScript.ToString());
                }

                return strScriptBlock.ToString();
            }
        }
    }
}