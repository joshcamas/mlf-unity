using System.Collections.Generic;

namespace Ardenfall.Mlf
{
    public class MlfContextReference
    {
        //Define a context reference
        //ID is the actual context ID
        //Key is what the context scope will be named when activated
        //A wildcard "*" means it will be added into the scope without a "namespace"
        public MlfContextReference(string id,string key="*")
        {
            this.id = id;
            this.key = key;
        }

        public string id;
        public string key;

    }


}
