using System.Collections.Generic;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

internal class Bonus
{
	private int totalMin;

	private int totalMax;

	private Dictionary<uint, int> individualMins = new Dictionary<uint, int>();

	private Dictionary<uint, int> individualMaxs = new Dictionary<uint, int>();

	private int multipleOf;

	public int points;

	private string description;

	private string text;

	private List<uint> equalTypes = new List<uint>();

	public Texture texture;

	public Bonus()
	{
		totalMin = 0;
		totalMax = 10000000;
		points = 5;
		multipleOf = 0;
		texture = null;
	}

	public void Parse(XElement parent)
	{
		parent.QueryIntAttribute("min", ref totalMin);
		parent.QueryIntAttribute("max", ref totalMax);
		int value = -1;
		parent.QueryIntAttribute("equals", ref value);
		if (value >= 0)
		{
			totalMin = (totalMax = value);
		}
		parent.QueryIntAttribute("points", ref points);
		parent.QueryIntAttribute("multiple", ref multipleOf);
		texture = StringFunctions.LoadTexture(parent.AttributeStr("texture"));
		List<string> words = new List<string>();
		int num = StringFunctions.SplitWords(parent.AttributeStr("valuesEqual"), ref words);
		for (int i = 0; i < num; i++)
		{
			equalTypes.Add(StringFunctions.StringHash(words[i]));
		}
		for (XAttribute xAttribute = parent.FirstAttribute; xAttribute != null; xAttribute = xAttribute.NextAttribute)
		{
			string localName = xAttribute.Name.LocalName;
			if (StringFunctions.StartsWithWord(localName, "min-"))
			{
				uint key = StringFunctions.StringHash(localName.Substring(4));
				individualMins[key] = MParser.ParseInt(xAttribute.Value);
			}
			else if (StringFunctions.StartsWithWord(localName, "max-"))
			{
				uint key2 = StringFunctions.StringHash(localName.Substring(4));
				individualMaxs[key2] = MParser.ParseInt(xAttribute.Value);
			}
		}
		if (parent.Value != null && parent.Value.Length > 0)
		{
			description = parent.Value;
		}
		else
		{
			description = "";
		}
	}

	public int GetPoints()
	{
		return points;
	}

	public string GetText()
	{
		return text;
	}

	public int IsAchieved(int total, Dictionary<uint, int> individualTotals)
	{
		if (total < totalMin || total > totalMax || (multipleOf > 0 && total % multipleOf != 0))
		{
			return 0;
		}
		foreach (KeyValuePair<uint, int> individualTotal in individualTotals)
		{
			int num = 0;
			int num2 = 1000000;
			if (individualMins.TryGetValue(individualTotal.Key, out var value))
			{
				num = value;
			}
			if (individualMaxs.TryGetValue(individualTotal.Key, out value))
			{
				num2 = value;
			}
			if (individualTotal.Value < num || individualTotal.Value > num2)
			{
				return 0;
			}
		}
		int num3 = -1;
		bool flag = true;
		foreach (uint equalType in equalTypes)
		{
			if (individualTotals.TryGetValue(equalType, out var value2))
			{
				if (flag)
				{
					num3 = value2;
					if (num3 <= 0)
					{
						return 0;
					}
					flag = false;
				}
				else if (num3 != value2)
				{
					return 0;
				}
				continue;
			}
			return 0;
		}
		text = string.Format(TheGame.instance.stringTable.GetString(description), total);
		return points;
	}
}
