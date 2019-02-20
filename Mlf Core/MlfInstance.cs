using UnityEngine;
using System.Collections.Generic;
using Sirenix.Serialization;
using Microsoft.Scripting.Hosting;
using Sirenix.OdinInspector;

namespace Ardenfall.Mlf
{
    [System.Serializable,ShowOdinSerializedPropertiesInInspector]
    public class MlfInstance
    {
        [System.NonSerialized,OdinSerialize, HideInInspector]
        public Dictionary<string, MlfProperty> defaultProperties = new Dictionary<string, MlfProperty>();

        [SerializeField,Multiline(15),HideLabel]//, PythonEditor]
        private string rawText = "";

        [SerializeField, HideInInspector]
        public string path;

        [System.NonSerialized]
        private List<string> tags;

        [System.NonSerialized]
        private string lastRawText;

        [System.NonSerialized]
        private MlfFlag[] flags;

        [System.NonSerialized]
        private MlfBlock[] blocks;

        [System.NonSerialized]
        public MlfObject owningObject;

        public bool IsDirty()
        {
            return (rawText != lastRawText);
        }

        public string RawScript
        {
            get { return rawText; }
            set { rawText = value; }
        }

        public MlfFlag[] Flags
        {
            get
            {
                InterpretIfDirty();

                return flags;
            }
        }

        public MlfBlock[] Blocks
        {
            get
            {
                InterpretIfDirty();

                return blocks;
            }
        }

        public MlfInstance() { }

        public MlfInstance(string rawText, string path)
        {
            this.path = path;
            this.rawText = rawText;
        }

        public MlfInstance(string rawText)
        {
            this.rawText = rawText;
        }

        public void InterpretIfDirty()
        {
            if (IsDirty())
                Interpret();

        }

        public void Interpret(bool clean=false)
        {
            //Cache interpreted text
            lastRawText = rawText;

            string commentedText = MlfInterpretor.CommentCode(rawText);

            flags = MlfInterpretor.FindFlags(commentedText).ToArray();
            blocks = MlfInterpretor.FindBlocks(this,commentedText, path).ToArray();

            //Process
            MlfProcessorManager.OnInstancePreInterpret(this);
            MlfProcessorManager.OnInstanceInterpret(this);
            MlfProcessorManager.OnInstancePostInterpret(this);

            //Clean
            if (clean)
                MlfInterpretor.CleanCode(this);
        }
        
        //Returns block with specific set of parameters
        public MlfBlock GetBlock(string id = null, string tag = null, string format = null)
        {
            foreach (MlfBlock block in Blocks)
            {
                if (id == null || block.id == id)
                    if (format == null || block.format == format)
                        if (tag == null || block.tags.Contains(tag))
                            return block;

            }
            return null;
        }

        //Returns flag with specific id
        public MlfFlag GetFlag(string id)
        {
            foreach(MlfFlag flag in Flags)
            {
                if (flag.id == id)
                    return flag;
            }
            return null;
        }

        //Returns flag with specific id
        public List<MlfFlag> GetFlags(string id)
        {
            List<MlfFlag> flags = new List<MlfFlag>();

            foreach (MlfFlag flag in Flags)
            {
                if (flag.id == id)
                    flags.Add(flag);
            }
            return flags;
        }

        //Returns flag with a specific group of tags
        public MlfFlag GetFlag(string[] tags)
        {
            foreach (MlfFlag flag in Flags)
            {
                bool success = true;
                foreach(string tag in tags)
                {
                    if(!flag.tags.Contains(tag))
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                    return flag;
            }
            return null;
        }

        //Returns blocks that fit certain parameters
        public List<MlfBlock> GetBlocks(string id=null, string tag=null, string format = null)
        {
            List<MlfBlock> outblocks = new List<MlfBlock>();

            foreach (MlfBlock block in Blocks)
            {
                if (id == null || block.id == id)
                    if (format == null || block.format == format)
                        if (tag == null || block.tags.Contains(tag))
                            outblocks.Add(block);
            }

            return outblocks;
        }

        //Run a function for each block that fits certain parameters
        public void ForEachBlock(System.Action<MlfBlock> forBlock,string id = null, string tag = null, string format = null)
        {
            foreach (MlfBlock block in Blocks)
            {
                if (id == null || block.id == id)
                    if (format == null || block.format == format)
                        if (tag == null || block.tags.Contains(tag))
                            forBlock(block);
            }
            
        }
        
    }
}