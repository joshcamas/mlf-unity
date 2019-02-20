using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using System.IO;
using YamlDotNet.Core;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Ardenfall.Yaml
{
    public class YamlUtility
    {
        //(Line: 11, Col: 9, Idx: 149) - (Line: 11, Col: 9, Idx: 149): Exception during deserialization
        public static string GetYamlSubstringFromError(string yamlText,string errorMessage)
        {
            MatchCollection mc = Regex.Matches(errorMessage, @"([0-9]+)");

            if(mc.Count < 6)
            {
                Debug.LogError("Could not read error string:\n" + errorMessage);
                return null;

            }
            string[] lines = yamlText.Replace("\n\r", "\n").Replace("\r\n", "\n").Split('\n');

            string substring = "";

            for(int i = int.Parse(mc[0].Value) - 1;i< int.Parse(mc[3].Value); i++)
            {
                substring += lines[i];
            }

            return substring;
        } 

    }
    public class YamlObject
    {
        private string content;
        private YamlNode rootNode;
        private Deserializer deserializer;

        //Cached Parser
        public IParser Parser
        {
            get
            {
                return new MergingParser(new MergingParser(new Parser(new StringReader(content))));
            }
        }

        //Cached Deserializer
        public Deserializer Deserializer
        {
            get
            {
                if(deserializer == null)
                    deserializer = new DeserializerBuilder().Build();

                return deserializer;
            }
        }

        //Cached Yaml Stream
        public YamlNode RootNode
        {
            get
            {
                if (rootNode == null)
                {
                    YamlStream stream = new YamlStream();
                    stream.Load(Parser);

                    rootNode = stream.Documents[0].RootNode;
                }

                return rootNode;
            }
        }

        //Constructor
        public YamlObject(string content)
        {
            this.content = content;
        }

        //Simple Deserialize helper using cached functions
        public T Deserialize<T>()
        {
            return Deserializer.Deserialize<T>(Parser);
        }

        //Sets content value and resets any generated values that are dependant on it
        public void SetContent(string content)
        {
            if(content != this.content)
            {
                this.content = content;
                Reset();
            }
        }

        //Resets any generated values that are dependent on content 
        private void Reset()
        {
            rootNode = null;
            deserializer = null;
        }
    }
}