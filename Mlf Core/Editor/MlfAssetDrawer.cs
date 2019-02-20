using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;
using System.Collections;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;

namespace Ardenfall.Mlf
{
    [CustomEditor(typeof(MlfAsset))]
    public class MlfAssetDrawer : OdinEditor
    {
        bool debugDropdown = false;

        public override void OnInspectorGUI()
        {

            Tree.UpdateTree();

            MlfAsset scriptAsset = (MlfAsset)target;

            this.Tree.GetPropertyAtPath("mlfObject").Draw(null);

            EditorUtility.SetDirty(target);

            Tree.ApplyChanges();

            if (scriptAsset.mlfObject.MlfInstance == null)
                return;

            debugDropdown = EditorGUILayout.Foldout(debugDropdown, "Debug Info",true);

            if (debugDropdown)
                DrawDebug(scriptAsset);

        }

        private void DrawDebug(MlfAsset scriptAsset)
        {
            foreach (string ds in MlfProcessorManager.DebugInstance(scriptAsset.mlfObject.MlfInstance))
            {
                if (ds == null)
                    continue;

                EditorGUILayout.LabelField(ds, EditorStyles.helpBox);
            }
        }

    }
}