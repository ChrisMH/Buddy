namespace Buddy.Web
{
    public class ScriptDef
    {
        public ScriptDef()
        {
            Type = ScriptType.None;
        }

        public string Src { get; set; }

        public ScriptType Type { get; set; }

        public string Body { get; set; }
    }
}