using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ardenfall.Mlf
{
    public class MlfUtility
    {

        /// <summary>
        /// Helper function that runs a function for every type that has a certain attribute
        /// </summary>
        public static void ForEachTypeWith<TAttribute>(bool inherit, Action<Type, TAttribute> function) where TAttribute : System.Attribute
        {
            foreach (Type type in GetTypesWith<TAttribute>(inherit))
            {
                TAttribute attribute =
                    type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

                function(type, attribute);
            }
        }

        /// <summary>
        /// Simple helper function that returns types that have a certain attribute
        /// </summary>
        public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
                                     where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }

        /// <summary>
        /// Helper function that runs a function for every field that has a certain attribute
        /// </summary>
        public static void ForEachFieldWith<TAttribute>(bool inherit, Action<FieldInfo, TAttribute> function) where TAttribute : System.Attribute
        {
            foreach (FieldInfo field in GetFieldsWith<TAttribute>(inherit))
            {
                TAttribute attribute =
                    field.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

                function(field, attribute);
            }
        }

        /// <summary>
        /// Simple helper function that returns fields that have a certain attribute
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWith<TAttribute>(bool inherit)
                                     where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   from f in t.GetType().GetFields()
                   let attr = f.GetCustomAttributes(typeof(TAttribute), true)
                   where attr.Length == 1
                   select f;
        }

        /// <summary>
        /// Helper function that runs a function for every field that has a certain attribute
        /// </summary>
        public static void ForEachPropertyWith<TAttribute>(bool inherit, Action<PropertyInfo, TAttribute> function) where TAttribute : System.Attribute
        {
            foreach (PropertyInfo property in GetPropertiesWith<TAttribute>(inherit))
            {
                TAttribute attribute =
                    property.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

                function(property, attribute);
            }
        }

        /// <summary>
        /// Simple helper function that returns fields that have a certain attribute
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertiesWith<TAttribute>(bool inherit)
                                     where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   from f in t.GetType().GetProperties()
                   let attr = f.GetCustomAttributes(typeof(TAttribute), true)
                   where attr.Length == 1
                   select f;
        }


        /// <summary>
        /// Helper function that runs a function for every type with a specific attribute in order, using IOrderedAttribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static void ForEachTypeWithOrdered<TAttribute>(bool inherit, Action<Type, TAttribute> function)
                                     where TAttribute : System.Attribute, IOrderedAttribute
        {
            List<Type> types = new List<Type>();
            List<TAttribute> attributes = new List<TAttribute>();

            ForEachTypeWith<TAttribute>(true, (type, attribute) =>
            {
                //No set index, don't care about it then
                if (attribute.GetIndex() < 0)
                {
                    types.Add(type);
                    attributes.Add(attribute);
                }
                else
                {
                    //Add type at correct index
                    //TODO: Make types not override each other
                    if (types.Count >= attribute.GetIndex())
                    {
                        types.Insert(attribute.GetIndex(), type);
                        attributes.Insert(attribute.GetIndex(), attribute);
                    }

                    else if (types.Count == attribute.GetIndex())
                    {
                        types.Add(type);
                        attributes.Add(attribute);
                    }

                    else
                    {
                        while (types.Count <= attribute.GetIndex())
                        {
                            types.Add(null);
                            attributes.Add(null);
                        }

                        types[attribute.GetIndex()] = type;
                        attributes[attribute.GetIndex()] = attribute;
                    }
                }
            });

            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] == null)
                    continue;

                function(types[i], attributes[i]);
            }
        }
    }
}