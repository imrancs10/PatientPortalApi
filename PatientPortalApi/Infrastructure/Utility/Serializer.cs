using PatientPortalApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PatientPortalApi.Infrastructure.Utility
{
    public class Serializer
    {
        public T Deserialize<T>(string input, string rootElementName) where T : class
        {
            var stringReader = new System.IO.StringReader(input);
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = rootElementName;
            xRoot.IsNullable = true;
            XmlSerializer xs = new XmlSerializer(typeof(T), xRoot);
            return xs.Deserialize(stringReader) as T;
        }

        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }

        public string SerializeToXML<T>(T ObjectToSerialize)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, ObjectToSerialize);
                    xml = sww.ToString();
                }
            }
            return xml;
        }
        public static XmlAttributeOverrides CreateAttributeOverrides(Type objectType)
        {
            Func<Type, bool> IsList = t => t.IsGenericType &&
                t.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));

            HashSet<Type> workTypes = GetTypesToOverride(objectType);
            XmlAttributeOverrides theBag = new XmlAttributeOverrides();
            var classNames = new HashSet<string>();
            classNames.Add(objectType.FullName);

            while (workTypes.Count > 0)
            {
                Type singleType = workTypes.First();
                workTypes.Remove(singleType);
                if (HasXmlAttributes(singleType))
                    continue;

                var wrapperAttr = singleType.GetCustomAttribute<XmlObjectWrapperAttribute>(true/*inherit*/);
                if (wrapperAttr != null)
                {
                    string wrapperTagName = wrapperAttr.ElementName;
                    Type listType = typeof(List<>);
                    Type listOfTType = listType.MakeGenericType(singleType);
                    string elementTagName = singleType.Name.ToCamel();
                    theBag.Add(
                        singleType,
                        new XmlAttributes()
                        {
                            XmlType = new XmlTypeAttribute(elementTagName),
                            XmlRoot = new XmlRootAttribute("OUTEROBJECT")
                        });

                    theBag.Add(
                        listOfTType,
                        new XmlAttributes() { XmlRoot = new XmlRootAttribute(wrapperTagName) });

                    classNames.Add(singleType.FullName);
                    workTypes.Remove(singleType);
                }
                else if (singleType == objectType)
                {
                    theBag.Add(objectType, new XmlAttributes() { XmlRoot = new XmlRootAttribute("OUTEROBJECT") });
                }

                PropertyInfo[] allPropsInfo = singleType.GetProperties();
                foreach (PropertyInfo propInfo in allPropsInfo)
                {
                    HashSet<Type> overridableTypes = GetTypesToOverride(propInfo.PropertyType);
                    bool propHasXmlAttributes = HasXmlAttributes(propInfo);
                    if (propHasXmlAttributes)
                        overridableTypes.Remove(propInfo.PropertyType);

                    List<String> overridableNames = overridableTypes.Select(t => t.FullName).ToList();
                    overridableTypes.RemoveWhere(t => classNames.Contains(t.FullName));
                    classNames.UnionWith(overridableTypes.Select(t => t.FullName));
                    workTypes.UnionWith(overridableTypes);
                    if (propHasXmlAttributes)
                        continue;

                    string camelName = propInfo.Name.ToCamel();
                    var propOverrides = new XmlAttributes();
                    Type propType = propInfo.PropertyType;
                    if (propType.IsArray || IsList(propType))
                    {
                        string pascalName = propInfo.Name.ToPascal();
                        propOverrides.XmlArray = new XmlArrayAttribute("COLLECTION" + pascalName);
                        propOverrides.XmlArrayItems.Add(new XmlArrayItemAttribute(camelName));
                    }
                    else
                    {
                        propOverrides.XmlElements.Add(new XmlElementAttribute(camelName));
                    }

                    theBag.Add(singleType, propInfo.Name, propOverrides);
                }
            }

            return theBag;
        }

        private static HashSet<Type> GetTypesToOverride(Type objectType)
        {
            var returnValue = new HashSet<Type>();
            returnValue.Add(objectType);
            Type elementType = objectType.GetElementType();
            if (elementType != null)
                returnValue.UnionWith(GetTypesToOverride(elementType));

            objectType.GetGenericArguments()
                .Where(t => t != null)
                .ToList()
                .ForEach(t => returnValue.UnionWith(GetTypesToOverride(t)));

            returnValue.RemoveWhere(t => t == null || t.FullName.StartsWith("System"));
            return returnValue;
        }

        private static bool HasXmlAttributes(MemberInfo minfo)
        {
            List<Attribute> xmlAttributes = minfo.GetCustomAttributes()
                .Where(t => {
                    string typeName = t.GetType().Name;
                    return typeName.StartsWith("Xml") && !"XmlObjectWrapperAttribute".Equals(typeName);
                })
                .ToList();

            return xmlAttributes.Count > 0;
        }

        public static T DeserializeDischargeSummary<T>(string s)
        {
            T returnValue = default(T);
            Type returnType = typeof(T);
            XmlAttributeOverrides xmlOverrides = CreateAttributeOverrides(returnType);
            XmlSerializer serializer = new XmlSerializer(returnType, xmlOverrides);
            using (TextReader reader = new StringReader(s))
            {
                returnValue = (T)serializer.Deserialize(reader);
            }

            return returnValue;
        }
    }
    public static class Overrides
    {
        public static string ToPascal(this string s) { return ChangeCasing(s, Char.ToUpper); }
        public static string ToCamel(this string s) { return ChangeCasing(s, Char.ToLower); }

        private static string ChangeCasing(string s, Func<Char, Char> convert)
        {
            return string.IsNullOrWhiteSpace(s) ? s : string.Format("{0}{1}", convert(s[0]), s.Substring(1));
        }
    }
}