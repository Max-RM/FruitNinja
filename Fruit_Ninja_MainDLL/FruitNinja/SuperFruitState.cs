using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class SuperFruitState
{
	public float time;

	public int hits;

	public float sliceTime;

	public float rotation;

	private bool QueryIntAttribute(XElement element, string AttributeName, ref int value)
	{
		XAttribute xAttribute = element.Attribute(AttributeName);
		if (xAttribute != null)
		{
			value = MParser.ParseInt(xAttribute.Value);
			return true;
		}
		return false;
	}

	private bool QueryFloatAttribute(XElement element, string AttributeName, ref float value)
	{
		XAttribute xAttribute = element.Attribute(AttributeName);
		if (xAttribute != null)
		{
			value = MParser.ParseFloat(xAttribute.Value);
			return true;
		}
		return false;
	}

	public void Parse(XElement element)
	{
		if (element != null)
		{
			QueryFloatAttribute(element, "time", ref time);
			QueryFloatAttribute(element, "sliceTime", ref sliceTime);
			QueryIntAttribute(element, "hits", ref hits);
			QueryFloatAttribute(element, "rot", ref rotation);
		}
	}

	public XElement WriteToElement()
	{
		XElement xElement = new XElement("superFruitState");
		xElement.Add(new XAttribute("time", time));
		xElement.Add(new XAttribute("sliceTime", sliceTime));
		xElement.Add(new XAttribute("hits", hits));
		xElement.Add(new XAttribute("rot", rotation));
		return xElement;
	}
}
