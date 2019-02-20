using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ardenfall.Mlf
{
    public class MlfInspectorWindow : EditorWindow
    {
        private string scriptString = null;
        private bool instanced = false;
        private MlfInstance instance;

        Vector2 scrollPosition;
        string textInputText = "";
        MlfAsset scriptAsset;

        bool inputScriptDopdown = true;
        bool exportedCodeDropdown = false;

        bool clean = true;

        //Regex Tester 
        bool regexTesterDropdown = false;
        string regex_pattern = "";
        string regex_text = "";
        string regex_output;

        List<object> compiledCode;


        [MenuItem("Ardenfall/Script Inspector", false, 110)]
        private static void ShowWindow()
        {
            MlfInspectorWindow.GetWindow(typeof(MlfInspectorWindow), false, "Script Inspector");
        }

        //Opens a window with a script asset
        private static void ShowWindow(MlfAsset asset)
        {
            MlfInspectorWindow window = (MlfInspectorWindow)MlfInspectorWindow.GetWindow(typeof(MlfInspectorWindow), false, "Script Inspector");
            window.SetAsset(asset);
        }

        //Opens a window with a script string
        private static void ShowWindow(string scriptString)
        {
            MlfInspectorWindow window = (MlfInspectorWindow)MlfInspectorWindow.GetWindow(typeof(MlfInspectorWindow), false, "Script Inspector");
            window.SetString(scriptString);
        }

        public void SetAsset(MlfAsset asset)
        {
            this.scriptString = asset.mlfObject.MlfInstance.RawScript;
            instanced = false;
            compiledCode = null;
            instance = null;
        }

        public void SetString(string scriptString)
        {
            this.scriptString = scriptString;
            instanced = false;
            compiledCode = null;
            instance = null;
        }

        public void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);


            //Regex:
            regexTesterDropdown = EditorGUILayout.Foldout(regexTesterDropdown, "Regex tester", true);

            if(regexTesterDropdown)
            {
                EditorGUI.indentLevel++;

                regex_pattern = EditorGUILayout.TextField("Pattern",regex_pattern);
                EditorGUILayout.LabelField("Text:");
                regex_text = EditorGUILayout.TextArea(regex_text, GUILayout.MinHeight(400));

                EditorGUILayout.BeginHorizontal();

                if(GUILayout.Button("Match",GUILayout.ExpandWidth(false)))
                {
                    MatchCollection matches = Regex.Matches(regex_text, regex_pattern);
                    regex_output = "(Match)\n";
                    regex_output += "count: " + matches.Count + "\n";

                    for (int i = 0;i< matches.Count;i++)
                    {
                        regex_output += i + ": " + matches[i].Value + "\n";
                    }
                }

                if (GUILayout.Button("Split", GUILayout.ExpandWidth(false)))
                {
                    string[] splits = Regex.Split(regex_text, regex_pattern);
                    regex_output = "(Split)\n";
                    regex_output += "count: " + splits.Length + "\n";

                    for (int i = 0; i < splits.Length; i++)
                    {
                        regex_output += i + ": " + splits[i] + "\n";
                    }
                }


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Output:");
                EditorGUILayout.SelectableLabel(regex_output, EditorStyles.helpBox, GUILayout.MinHeight(400));
            }


            inputScriptDopdown = EditorGUILayout.Foldout(inputScriptDopdown, "Input Script", true);

            if(inputScriptDopdown)
            {
                EditorGUI.indentLevel++;

                //Asset Selection
                EditorGUILayout.BeginHorizontal();

                scriptAsset = (MlfAsset)EditorGUILayout.ObjectField(scriptAsset, typeof(MlfAsset), false);

                if (GUILayout.Button("Use Asset", GUILayout.ExpandWidth(false)) && scriptAsset != null)
                {
                    SetAsset(scriptAsset);
                }

                EditorGUILayout.EndHorizontal();

                //Text sekection
                EditorGUILayout.BeginHorizontal();

                textInputText = EditorGUILayout.TextArea(textInputText);

                if (GUILayout.Button("Use Text", GUILayout.ExpandWidth(false)) && textInputText.Replace(" ", "") != "")
                {
                    SetString(textInputText);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
            
            if (scriptString == null)
            {
                EditorGUILayout.EndScrollView();
                return;
            }

            clean = EditorGUILayout.Toggle("Clean", clean);

            if(instance == null && instanced == false)
            {
                instance = new MlfInstance(scriptString);
                instance.Interpret(clean);
                instanced = true;
            }
                
            if (instance != null && instance.Blocks != null)
            {
                DisplayScriptInfo();
            }
            EditorGUILayout.EndScrollView();

        }

        private void DisplayScriptInfo()
        {

            EditorGUILayout.LabelField("Blocks", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            foreach (MlfBlock block in instance.Blocks)
            {
                EditorGUILayout.LabelField(block.id, EditorStyles.boldLabel);

                EditorGUILayout.LabelField("Format:" + block.format);

                if (block.tags.Count > 0)
                {
                    string tags = "Tags: ";
                    foreach (string str in block.tags)
                        tags += "'" + str + "',";

                    EditorGUILayout.LabelField(tags);
                }

                if (block.arguments.Count > 0)
                {
                    string arguments = "Arguments: ";
                    foreach (string str in block.arguments)
                        arguments += "'" + str + "',";

                    EditorGUILayout.LabelField(arguments);
                }

            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Flags", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            foreach (MlfFlag flag in instance.Flags)
            {
                EditorGUILayout.LabelField(flag.id, EditorStyles.boldLabel);

                if (flag.tags.Count > 0)
                {
                    string tags = "Tags: ";
                    foreach (string str in flag.tags)
                        tags += "'" + str + "',";

                    EditorGUILayout.LabelField(tags);
                }

                if (flag.arguments.Count > 0)
                {
                    string arguments = "Arguments: ";
                    foreach (string str in flag.arguments)
                        arguments += "'" + str + "',";

                    EditorGUILayout.LabelField(arguments);
                }
            }
            EditorGUI.indentLevel--;

            if (instance.defaultProperties != null)
            {
                EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (KeyValuePair<string, MlfProperty> pair in instance.defaultProperties)
                {
                    EditorGUILayout.LabelField(pair.Key + " : " + pair.Value.Type);
                }
                EditorGUI.indentLevel--;

            }

            exportedCodeDropdown = EditorGUILayout.Foldout(exportedCodeDropdown, "Content", true);
            EditorGUI.indentLevel++;

            if (exportedCodeDropdown)
            {
                int i = 0;
                foreach (MlfBlock block in instance.Blocks)
                {
                    EditorGUILayout.LabelField(block.GetQuickName(), EditorStyles.boldLabel);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextArea(block.Content);
                    EditorGUI.EndDisabledGroup();

                    /*
                    //Compile code
                    if (compiledCode == null)
                        compiledCode = new List<object>();

                    if (compiledCode.Count <= i)
                    {
                        object comp = Snek.SnekScriptEngine.QuickCompile(block.Content);
                        compiledCode.Add(comp);
                    }
                    
                    if(compiledCode[i] as System.Exception != null)
                    {
                        EditorGUILayout.LabelField("Syntax Error: " + ((System.Exception)compiledCode[i]).Message);
                    } else
                    {
                        EditorGUILayout.LabelField("Success");
                    }*/

                    i++;
                }

                
            }
            EditorGUI.indentLevel--;
        }

    }
}