namespace Ardenfall.Mlf
{
    public class MlfProcessor
    {
        //Override to format a script once it has been loaded / instanced.
        public virtual void OnMlfFormatPre(MlfInstance instance) { }

        //Override to modify a script once it has been loaded / instanced.
        public virtual void OnMlfInstanceInterpret(MlfInstance instance) { }

        //Override to format a script after it has been interpreted.
        public virtual void OnMlfFormatPost(MlfInstance instance) { }

        //Override to modify the engine once it has been initialized
        public virtual void OnEngineInit(Snek.SnekScriptEngine engine) { }

        public virtual string DebugInstance(MlfInstance instance) { return null; }
    }
    
}