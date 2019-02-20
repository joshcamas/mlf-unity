using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Ardenfall.Mlf
{
    [System.Serializable,ShowOdinSerializedPropertiesInInspector]
    public class MlfObject
    {
        [SerializeField]
        private MlfAsset parentAsset;

        [OdinSerialize, System.NonSerialized,HideInInspector]
        public PropertyOverrideList overrideProperties;

        [OdinSerialize, HideReferenceObjectPicker]
        private MlfInstance mlfInstance = new MlfInstance();

        public MlfAsset ParentAsset
        {
            set
            {
                /*
                if (value == parentAsset)
                    return;

                if (value == null)
                {
                    parentAsset = null;
                    return;
                }
                */
                parentAsset = value;

                if (DetectInfiniteParentChain())
                {
                    Debug.LogError("Infinite Parent Chain Detected!");
                    parentAsset = null;
                    return;
                }

            }
            get { return parentAsset; }
        }

        public bool EnableInstance { get { return parentAsset == null; } }

        public MlfObject()
        {

        }

        public MlfObject(MlfAsset parentAsset)
        {
            this.parentAsset = parentAsset;
            this.overrideProperties = new PropertyOverrideList();
        }

        public MlfObject(string rawText, string path)
        {
            this.mlfInstance = new MlfInstance(rawText, path);
        }

        public MlfObject(string rawText)
        {
            this.mlfInstance = new MlfInstance(rawText);
        }

        public MlfObject(MlfInstance mlfInstance)
        {
            this.mlfInstance = mlfInstance;
        }

        public bool DetectInfiniteParentChain(List<MlfObject> objects=null)
        {
            //No parent, so no infinity
            if (parentAsset == null)
                return false;

            if (objects == null)
                objects = new List<MlfObject>();

            objects.Add(this);

            if (objects.Contains(parentAsset.mlfObject))
                return true;

            return parentAsset.mlfObject.DetectInfiniteParentChain(objects);
        }

        public void SetMlfInstance(MlfInstance instance)
        {
            parentAsset = null;
            mlfInstance = instance;
        }

        public MlfInstance MlfInstance
        {
            get
            {
                if (EnableInstance)
                {
                    mlfInstance.owningObject = this;
                    return mlfInstance;
                }
                    

                return parentAsset.mlfObject.MlfInstance;
            }
        }

        public Dictionary<string,MlfProperty> MlfProperties
        {
            get
            {
                if (overrideProperties != null && !EnableInstance)
                    return overrideProperties.GetProperties(parentAsset.mlfObject.MlfProperties);

                return MlfInstance.defaultProperties;
            }
        }

        /*
        //Executes "Script" Block
        public object ExecuteDefault()
        {
            if (MlfInstance == null)
            {
                Debug.LogError("Cannot Execute Default Block on null ScriptInstance");
                return null;
            }

            MlfInstance.InterpretIfDirty();

            foreach (MlfBlock block in MlfInstance.Blocks)
            {
                if (block.id != "Script")
                    continue;

                //Execute block
                return Snek.SnekScriptEngine.Instance.Execute(block.ScriptSource,ScriptProperties);
            }
            return null;
        }

        //Execute a specific script block
        public object ExecuteBlock(string id, string tag = null, params object[] arguments)
        {
            if (MlfInstance == null)
            {
                Debug.LogError("Cannot Execute " + id + " Block on null ScriptInstance");
                return null;
            }

            MlfInstance.InterpretIfDirty();

            foreach (MlfBlock block in MlfInstance.Blocks)
            {
                if (block.id != id)
                    continue;

                if (tag != null && !block.tags.Contains(tag))
                    continue;

                //Execute block
                return Snek.SnekScriptEngine.Instance.Execute(block.ScriptSource, ScriptProperties);
            }
            return null;
        }*/


    }
}