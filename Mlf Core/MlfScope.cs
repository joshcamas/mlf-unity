using Microsoft.Scripting.Hosting;
namespace Ardenfall.Mlf
{
    public class SnekScope
    {
        public SnekScope(ScriptScope scriptScope)
        {
            this.scriptScope = scriptScope;
        }

        public SnekScope()
        {
            this.scriptScope = Snek.SnekScriptEngine.Instance.Engine.CreateScope();
        }

        public ScriptScope scriptScope;

    }
}