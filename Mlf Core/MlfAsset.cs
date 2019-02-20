using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ardenfall.Mlf
{
    [CreateAssetMenu(menuName = "Ardenfall/MLF/MLF Asset"),ShowOdinSerializedPropertiesInInspector]
    public class MlfAsset : SerializedScriptableObject
    {
         [OdinSerialize,System.NonSerialized,HideReferenceObjectPicker]
       // [SerializeField]
        public MlfObject mlfObject = new MlfObject();

#if UNITY_EDITOR

        //Manage MLF path
        private void Awake()
        {
            if(!Application.isPlaying)
            {
                string path = AssetDatabase.GetAssetPath(GetInstanceID());

                if(mlfObject.MlfInstance.path != path)
                {
                    mlfObject.MlfInstance.path = path;
                    EditorUtility.SetDirty(this);
                }
                
            }
        }

#endif
    }

    public class MLFAssetAttribute : System.Attribute
    {
        public enum MLFAssetEditMode
        {
            referenceAndInstance,referenceOnly,instanceOnly
        }

        public MLFAssetEditMode mode;

        public MLFAssetAttribute(MLFAssetEditMode mode = MLFAssetEditMode.referenceAndInstance)
        {
            this.mode = mode;
        }
    }
}