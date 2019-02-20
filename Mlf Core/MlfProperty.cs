using Sirenix.Serialization;
using UnityEngine;

namespace Ardenfall.Mlf
{
    [System.Serializable]
    public class MlfProperty
    {
        [OdinSerialize] public System.Type type;
        [OdinSerialize] private object value;
        [SerializeField] public string tooltip;
        [SerializeField] public string specialType;
        [SerializeField] public bool staticVar;

        public MlfProperty()
        {

        }

        public MlfProperty(MlfProperty copy)
        {
            type = copy.type;

            //TODO: Make sure this doesn't cause reference issues
            Value = copy.value;
        }

        //Public access to property's value
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public bool CanAssignValue(object value)
        {
            if (type == null || value == null)
                return true;
            
            if (type.IsAssignableFrom(value.GetType()))
                return true;
            return false;
        }

        public string Type
        {
            get
            {
                return type.AssemblyQualifiedName;
            }
            set
            {
                type = System.Type.GetType(value);
            }
        }
    }
}