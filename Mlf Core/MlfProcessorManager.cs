using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ardenfall.Mlf
{
    public class MlfProcessorManager
    {
        private static List<MlfProcessor> processorCache;

        private static List<MlfProcessor> Processors
        {
            get
            {
                //Scan for processors
                if(processorCache == null)
                {
                    processorCache = new List<MlfProcessor>();

                    foreach(Type t in FindDerivedTypes(typeof(MlfProcessorManager).Assembly,typeof(MlfProcessor)))
                    {
                        processorCache.Add((MlfProcessor)Activator.CreateInstance(t));
                    }
                }
                return processorCache;

            }
        }

        public static void AddProcessor(MlfProcessor processor)
        {
            Processors.Add(processor);
        }

        public static void OnEngineInit(Snek.SnekScriptEngine engine)
        {
            foreach (MlfProcessor processor in Processors)
            {
                processor.OnEngineInit(engine);
            }
        }

        public static void OnInstancePreInterpret(MlfInstance instance)
        {
            foreach (MlfProcessor processor in Processors)
            {
                processor.OnMlfFormatPre(instance);
            }
        }

        public static void OnInstanceInterpret(MlfInstance instance)
        {
            foreach(MlfProcessor processor in Processors)
            {
                processor.OnMlfInstanceInterpret(instance);
            }
        }

        public static void OnInstancePostInterpret(MlfInstance instance)
        {
            foreach (MlfProcessor processor in Processors)
            {
                processor.OnMlfFormatPost(instance);
            }
        }

        public static List<string> DebugInstance(MlfInstance instance)
        {
            List<string> instanceDebugs = new List<string>();

            foreach (MlfProcessor processor in Processors)
            {
                instanceDebugs.Add(processor.DebugInstance(instance));
            }

            return instanceDebugs;
        }

        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => t != baseType &&
                                                  baseType.IsAssignableFrom(t));
        }

    }

}