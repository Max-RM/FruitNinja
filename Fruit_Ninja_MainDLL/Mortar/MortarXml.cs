using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Mortar;

public class MortarXml
{
	public static void Save(XDocument doc, string name)
	{
	}

	public static XDocument Load(string name)
	{
		string text;
		try
		{
			text = ((Game)TheGame.instance).Content.Load<string>(name);
		}
		catch (Exception)
		{
			return null;
		}
		if (text == null)
		{
			return null;
		}
		return XDocument.Parse(text);
	}
}
