using System.Collections.Generic;
using System.Xml.Linq;

namespace Mortar;

public static class XAttributeExtensions
{
	public static Dictionary<T, G> Duplicate<T, G>(this Dictionary<T, G> d)
	{
		Dictionary<T, G> dictionary = new Dictionary<T, G>();
		foreach (KeyValuePair<T, G> item in d)
		{
			dictionary.Add(item.Key, item.Value);
		}
		return dictionary;
	}

	public static LinkedList<T> Duplicate<T>(this LinkedList<T> l)
	{
		LinkedList<T> linkedList = new LinkedList<T>();
		foreach (T item in l)
		{
			linkedList.AddLast(item);
		}
		return linkedList;
	}

	public static XElement FirstChildElement(this XDocument element)
	{
		using (IEnumerator<XElement> enumerator = element.Elements().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public static XElement FirstChildElement(this XDocument element, string ename)
	{
		return element.Element(ename);
	}

	public static XElement FirstChildElement(this XElement element)
	{
		using (IEnumerator<XElement> enumerator = element.Elements().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public static XElement FirstChildElement(this XElement element, string ename)
	{
		return element.Element(ename);
	}

	public static bool QueryIntAttribute(this XElement element, string AttributeName, ref int value)
	{
		XAttribute xAttribute = element.Attribute(AttributeName);
		if (xAttribute != null)
		{
			value = MParser.ParseInt(xAttribute.Value);
			return true;
		}
		return false;
	}

	public static bool QueryFloatAttribute(this XElement element, string AttributeName, ref float value)
	{
		XAttribute xAttribute = element.Attribute(AttributeName);
		if (xAttribute != null)
		{
			value = MParser.ParseFloat(xAttribute.Value);
			return true;
		}
		return false;
	}

	public static string AttributeStr(this XElement element, string attribName)
	{
		return element.Attribute(attribName)?.Value;
	}

	public static XElement NextSiblingElement(this XElement element, string elementName)
	{
		using (IEnumerator<XElement> enumerator = element.ElementsAfterSelf(elementName).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public static XElement NextSiblingElement(this XElement element)
	{
		using (IEnumerator<XElement> enumerator = element.ElementsAfterSelf().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public static void SetAttribute(this XElement element, XName name, object obj)
	{
		XAttribute content = new XAttribute(name, obj);
		element.Add(content);
	}

	public static void LinkEndChild(this XElement element, XElement child)
	{
		element.Add(child);
	}

	public static string GetText(this XElement element)
	{
		return element.Value;
	}
}
