using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace Ardenfall.Mlf
{
    //Adds code block: Properties
    //Adds properties to script instance
    //Usage: [[Properties]]
    public class PropertiesBlock : MlfProcessor
    {
        public override void OnMlfInstanceInterpret(MlfInstance instance)
        {
            MlfBlock propertyBlock = instance.GetBlock("Properties");

            if (propertyBlock == null)
            {
                instance.defaultProperties = null;
                return;
            }

            //Only support yaml format
            if (propertyBlock.format != "yaml")
            {
                Debug.LogError("[MLF] Properties block only supports the 'yaml' format \n" + instance.path);
                return;
            }

            Dictionary<string, MlfProperty> properties = new Dictionary<string, MlfProperty>();

            Yaml.YamlObject yaml = (Yaml.YamlObject)propertyBlock.GetFormatData(YamlFormat.formatDataKey);
            
            //Root
            foreach (YamlNode node in ((YamlSequenceNode)yaml.RootNode).Children)
            {
                //Should only be a single child for this entry
                foreach(var entry in ((YamlMappingNode)node).Children)
                {
                    MlfProperty property = new MlfProperty();
                    
                    //Scalar -> value = Type
                    if(entry.Value.NodeType == YamlNodeType.Scalar)
                    {
                        System.Type type = null;
                        
                        if (!TypeFinder.TryFindType(((YamlScalarNode)entry.Value).Value, out type))
                            continue;

                        property.type = type;
                    }

                    //Mapping -> read children values
                    else if (entry.Value.NodeType == YamlNodeType.Mapping)
                    {
                        IDictionary<YamlNode, YamlNode> entries = ((YamlMappingNode)entry.Value).Children;

                        if (entries.ContainsKey("specialtype"))
                        {
                            property.specialType = ((YamlScalarNode)entries["specialtype"]).Value;
                        }

                        if (entries.ContainsKey("type"))
                        {
                            System.Type type = null;
                            
                            if (!TypeFinder.TryFindType(((YamlScalarNode)entries["type"]).Value, out type))
                                continue;

                            property.type = type;
                        }

                        //Default value
                        if(entries.ContainsKey("default"))
                            property.Value = ((YamlScalarNode)entries["default"]).Value;

                        //Value value, equivilant to default
                        if (entries.ContainsKey("value"))
                            property.Value = ((YamlScalarNode)entries["value"]).Value;
                        
                        if (entries.ContainsKey("tooltip"))
                            property.tooltip = ((YamlScalarNode)entries["tooltip"]).Value;

                        if (entries.ContainsKey("static"))
                        {
                            //TODO: Use actual resolver instead of crappy check
                            string staticVar = ((YamlScalarNode)entries["static"]).Value.ToLower().Trim();

                            if (staticVar == "yes" || staticVar == "true")
                                property.staticVar = true;
                            else
                                property.staticVar = false;
                        }

                    }

                    properties[((YamlScalarNode)entry.Key).Value.TrimStart('$')] = property;
                }
            }

            Dictionary<string, MlfProperty> oldProperties = instance.defaultProperties;

            instance.defaultProperties = properties;

            //Replace old values if they exist
            if (oldProperties != null)
            {
                foreach (KeyValuePair<string, MlfProperty> pair in oldProperties)
                {
                    if (instance.defaultProperties.ContainsKey(pair.Key))
                        if (instance.defaultProperties[pair.Key].CanAssignValue(pair.Value.Value))
                            instance.defaultProperties[pair.Key].Value = pair.Value.Value;
                }
            }

            if (instance.defaultProperties.Count == 0)
                return;

            //Replace properties in contents
            //TODO: Make individual formats handle this in a "post post processor", as well as 
            //fix issues with parts of words being replaced!
            foreach (MlfBlock block in instance.Blocks)
            {
                foreach (KeyValuePair<string, MlfProperty> pair in instance.defaultProperties)
                {
                    block.Content = Regex.Replace(block.Content, @"\s*\$" + pair.Key + @"\s*", Snek.SnekScriptEngine.propertyPrefix + pair.Key);
                }

            }

        }
    }
}