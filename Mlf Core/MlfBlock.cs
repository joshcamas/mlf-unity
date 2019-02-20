using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ardenfall.Mlf
{
    public class MlfBlock
    {
        public string format;
        public string id;
        public List<string> tags;
        public List<string> arguments;
        public List<MlfContextReference> contexts;
        public string path;
        public int line;
        public MlfInstance owningInstance;

        private string content;

        private Dictionary<string, object> formatData;

        public System.Action<string> OnContentChange;

        public bool enableFunctionWrapping = true;
        public string Content
        {
            set
            {
                content = value;

                //Trigger content change
                if (OnContentChange != null)
                    OnContentChange(content);
            }
            get
            {
                return content;
            }
        }

        public Dictionary<string, MlfProperty> MlfProperties
        {
            get
            {
                return owningInstance.owningObject.MlfProperties;
            }
        }

        public MlfBlock(MlfInstance mlfInstance)
        {
            this.owningInstance = mlfInstance;
            this.formatData = new Dictionary<string, object>();
        }

        public object GetFormatData(string id)
        {
            if (formatData.ContainsKey(id))
                return formatData[id];

            return null;
        }

        public void SetFormatData(string id, object value)
        {
            formatData[id] = value;
        }

        public void AddPrefixText(string defaultBlock)
        {
            Content = defaultBlock + "\n" + Content;
        }

        public int GetFileLine(int relativeLine)
        {
            return line + relativeLine;
        }

        public string GetQuickName()
        {
            string name = id;

            if (tags.Count > 0)
            {
                name += "\"";
                foreach (string tag in tags)
                    name += tag + ",";
                name += "\"";
            }

            if (arguments.Count > 0)
            {
                name += "(";
                foreach (string arg in arguments)
                    name += arg + ",";
                name += ")";

            }

            return name;
        }
    }
}