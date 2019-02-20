using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Ardenfall.Yaml;

namespace Ardenfall.Mlf
{
    //Adds the format: "python".
    public class YamlFormat : MlfProcessor
    {
        public static string formatDataKey = "yamlObject";

        //Helper function that makes it easier to get the stored yaml object
        public static YamlObject GetYamlObject(MlfBlock block)
        {
            if (block == null)
                return null;

            return (YamlObject)block.GetFormatData(formatDataKey);
        }

        //Helper function that formats a string with properties
        public static string ApplyPropertiesToString(string str,MlfBlock block)
        {
            string appliedString = str;

            foreach(KeyValuePair<string,MlfProperty> property in block.MlfProperties)
            {
                string v = property.Value.Value.ToString();
                appliedString = appliedString.Replace("$" + property.Key, v);
            }
            return appliedString;
        }

        public override void OnMlfFormatPre(MlfInstance instance)
        {
            foreach (MlfBlock block in instance.Blocks)
            {
                if (block.format != "yaml")
                    continue;

                //Detect tabs
                if (block.Content.Contains("\t"))
                {
                    Debug.LogError("YAML Does not support tabs.");
                    return;
                }
                    

                YamlObject yaml = new YamlObject(block.Content);
                block.SetFormatData(formatDataKey, yaml);

                block.OnContentChange += (content) =>
                {
                    yaml.SetContent(content);
                };

            }
        }

    }

}