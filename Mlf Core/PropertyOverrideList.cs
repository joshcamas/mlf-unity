using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;

namespace Ardenfall.Mlf
{

    [System.Serializable, ShowOdinSerializedPropertiesInInspector]
    public class PropertyOverrideList
    {
        [OdinSerialize]
        private Dictionary<string, MlfProperty> properties;

        [OdinSerialize]
        private List<string> overridenProperties;

        //Cache
        private Dictionary<string, MlfProperty> cachedCombinedProperties;
        
        private void UpdateProperties(Dictionary<string, MlfProperty> parentProperties)
        {
            if (parentProperties == null)
            {
                properties = null;
                return;
            }

            if (properties == null)
            {
                properties = new Dictionary<string, MlfProperty>();

                //Copy
                foreach (KeyValuePair<string, MlfProperty> pair in parentProperties)
                {
                    properties.Add(pair.Key, new MlfProperty(pair.Value));
                }
                return;
            }

            if (overridenProperties == null)
                overridenProperties = new List<string>();
            
            //Remove from dictionary
            List<string> toRemove = new List<string>();

            foreach (KeyValuePair<string, MlfProperty> pair in properties)
            {
                if (!parentProperties.ContainsKey(pair.Key))
                    toRemove.Add(pair.Key);
            }

            foreach (string key in toRemove)
                properties.Remove(key);

            //Add to dictionary
            foreach(KeyValuePair<string, MlfProperty> pair in parentProperties)
            {
                //Add to dictionary
                if (!properties.ContainsKey(pair.Key))
                    properties.Add(pair.Key, new MlfProperty(pair.Value));

                //Parent overrides
                else if(!overridenProperties.Contains(pair.Key))
                {
                    properties[pair.Key] = new MlfProperty(pair.Value);
                }
            }

            //Reset cache
            cachedCombinedProperties = null;
        }

        public bool PropertyOverrides(string id)
        {
            if (overridenProperties == null)
                return false;

            return overridenProperties.Contains(id);
        }

        public void SetPropertyOverride(string id,bool overrideValue)
        {
            if (overridenProperties == null)
                overridenProperties = new List<string>();

            if (overrideValue && !overridenProperties.Contains(id))
                overridenProperties.Add(id);

            if (!overrideValue && overridenProperties.Contains(id))
                overridenProperties.Remove(id);
        }

        public Dictionary<string, MlfProperty> GetProperties(Dictionary<string, MlfProperty> parent,bool cache=true)
        {
            if (cachedCombinedProperties == null || !cache)
            {
                UpdateProperties(parent);
                cachedCombinedProperties = properties;
            }

            return cachedCombinedProperties;
        }

    }
}