using System.Collections.Generic;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

internal class BonusType
{
	private Dictionary<uint, int> totals = new Dictionary<uint, int>();

	private List<Bonus> bonuses = new List<Bonus>();

	public void Parse(XElement parent)
	{
		List<string> words = new List<string>();
		int num = StringFunctions.SplitWords(parent.AttributeStr("total"), ref words);
		for (int i = 0; i < num; i++)
		{
			totals[StringFunctions.StringHash(words[i])] = 0;
		}
		Texture texture = StringFunctions.LoadTexture(parent.AttributeStr("texture"));
		for (XElement xElement = parent.FirstChildElement("bonus"); xElement != null; xElement = xElement.NextSiblingElement("bonus"))
		{
			Bonus bonus = new Bonus();
			bonus.Parse(xElement);
			if (bonus.texture == null)
			{
				bonus.texture = texture;
			}
			bonuses.Add(bonus);
		}
	}

	public Bonus GetBest()
	{
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
		foreach (KeyValuePair<uint, int> total in totals)
		{
			int bonusTotal = BonusManager.GetBonusTotal(total.Key);
			dictionary[total.Key] = bonusTotal;
			num3 += bonusTotal;
		}
		totals = dictionary;
		for (int i = 0; i < bonuses.Count; i++)
		{
			int points = bonuses[i].GetPoints();
			if (points > num && bonuses[i].IsAchieved(num3, totals) != 0)
			{
				num2 = i;
				num = points;
			}
		}
		if (num2 < 0)
		{
			return null;
		}
		return bonuses[num2];
	}
}
