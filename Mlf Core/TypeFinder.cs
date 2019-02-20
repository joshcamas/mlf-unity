using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using IronPython.Hosting;
using IronPython.Runtime.Operations;
using Ardenfall.Mlf;


namespace Ardenfall.Mlf
{
    public class TypeFinder
    {
        private static Dictionary<string, Type> typeCache;

        private static List<System.Reflection.Assembly> loadedAssemblies;

        //Builds the supported assemblies for engine
        public static List<System.Reflection.Assembly> LoadedAssemblies
        {
            get
            {
                if(loadedAssemblies == null)
                {
                    loadedAssemblies = new List<System.Reflection.Assembly>();
                    loadedAssemblies.Add(typeof(UnityEngine.GameObject).Assembly);
                    loadedAssemblies.Add(typeof(UnityEngine.Physics).Assembly);
                    loadedAssemblies.Add(typeof(System.Type).Assembly);

                }

                return loadedAssemblies;
            }
        }

        //Convert a string into type - cached
        public static bool TryFindType(string typeName, out Type t)
        {
            if (typeCache == null)
                typeCache = new Dictionary<string, Type>();

            //Trim
            typeName = typeName.Trim();

            if (!typeCache.TryGetValue(typeName, out t))
            {
                //If type contains a period
                int spl = typeName.LastIndexOf('.');
                string namespaceStr = "";
                string typeStr = typeName;

                if (spl != -1)
                {
                    namespaceStr = typeName.Substring(0, spl);
                    typeStr = typeName.Substring(spl + 1);
                }

                foreach (System.Reflection.Assembly a in LoadedAssemblies)
                {
                    foreach (Type type in a.GetTypes())
                    {
                        if ((namespaceStr == "" || type.Namespace == null || type.Namespace.Equals(namespaceStr)) && type.Name.Equals(typeStr))
                        {
                            t = type;
                            break;
                        }
                    }
                }
                typeCache[typeName] = t; // perhaps null
            }

            return t != null;
        }

    }
}