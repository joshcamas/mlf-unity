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
    //For now, the fancy inspector is too buggy to use. So just default to multiline editor built into editor

        
    [DrawerPriority(DrawerPriorityLevel.ValuePriority)]
    public class MlfDrawer : OdinValueDrawer<MlfObject>
    {
        private bool interpretedOnce = false;

        protected override void DrawPropertyLayout(GUIContent label)
        {

            ValueEntry.Property.Tree.UpdateTree();

            MlfObject mlfObject = ValueEntry.SmartValue;

            mlfObject.ParentAsset = (MlfAsset)EditorGUILayout.ObjectField("Parent:",mlfObject.ParentAsset, typeof(MlfAsset), false);

            //Make sure interpretation is updated
            if(!interpretedOnce)
            {
                mlfObject.MlfInstance.InterpretIfDirty();
                interpretedOnce = true;
            }

            //Draw instance
            if (mlfObject.EnableInstance)
            {
                if(ValueEntry.Property.Children["mlfInstance"] != null)
                {
                    ValueEntry.Property.Children["mlfInstance"].Draw(null);
                }
                   
                DrawProperties(mlfObject);
            } else
            {
                DrawOverridingProperties(mlfObject);
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Interpret", GUILayout.ExpandWidth(false)))
            {
                mlfObject.MlfInstance.Interpret(false);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            ValueEntry.Property.Tree.ApplyChanges();
            ValueEntry.ApplyChanges();
        }

        private void DrawProperties(MlfObject script)
        {
            GUILayout.Space(5);

            Dictionary<string, MlfProperty> properties = script.MlfInstance.defaultProperties;

            if (properties == null || properties.Count == 0)
                return;

            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

            //Loop all properties
            foreach (KeyValuePair<string, MlfProperty> pair in properties)
            {
                //Do not draw static properties
                if (pair.Value.staticVar)
                    continue;

                EditorGUILayout.BeginHorizontal();
                pair.Value.Value = DrawProperty(pair.Key,pair.Value);
                EditorGUILayout.EndHorizontal();
            }

        }

        private void DrawOverridingProperties(MlfObject script)
        {
            GUILayout.Space(5);

            if (script.overrideProperties == null)
                script.overrideProperties = new PropertyOverrideList();

            Dictionary<string, MlfProperty> properties = script.overrideProperties.GetProperties(script.ParentAsset.mlfObject.MlfProperties, false);

            if (properties == null || properties.Count == 0)
                return;

            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

            //Loop all properties
            foreach (KeyValuePair<string, MlfProperty> pair in properties)
            {
                //Do not draw static properties
                if (pair.Value.staticVar)
                    continue;

                EditorGUILayout.BeginHorizontal();

                bool overrides = script.overrideProperties.PropertyOverrides(pair.Key);

                EditorGUI.BeginDisabledGroup(!overrides);
                pair.Value.Value = DrawProperty(pair.Key,pair.Value);
                EditorGUI.EndDisabledGroup();

                overrides = EditorGUILayout.Toggle(overrides, GUILayout.Width(30));
                script.overrideProperties.SetPropertyOverride(pair.Key, overrides);

                EditorGUILayout.EndHorizontal();
            }


        }

        private object DrawProperty(string label,MlfProperty property)
        {
            GUIContent con = new GUIContent(label, property.tooltip);

            //Special case: Python 
            if(property.specialType == "python")
            {
                string v = EditorGUILayout.TextField(con, (string)property.Value);
                EditorGUILayout.LabelField("()", GUILayout.Width(20));
                return v;
            }

            if (property.type == typeof(float))
                return EditorGUILayout.FloatField(con,(float)property.Value);

            if (property.type == typeof(int))
            {
                if (property.Value == null)
                    property.Value = 0;

                return EditorGUILayout.IntField(con, (int)property.Value);
            }
                
            if (property.type == typeof(string))
                return EditorGUILayout.TextField(con,(string)property.Value);

            //Asset object (scriptable object)
            if (typeof(ScriptableObject).IsAssignableFrom(property.type))
                return EditorGUILayout.ObjectField(con,(ScriptableObject)property.Value, property.type,false);

            //Scene allowed object (any other object)
            if (typeof(UnityEngine.Object).IsAssignableFrom(property.type))
                return EditorGUILayout.ObjectField(con,(UnityEngine.Object)property.Value, property.type, true);

            EditorGUILayout.LabelField("Cannot draw type of " + property.Type);
            return null;
        }

    }

}