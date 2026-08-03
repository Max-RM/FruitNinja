using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class BonusManager
{
	private List<BonusType> m_bonusTypes = new List<BonusType>();

	private LinkedList<Bonus> m_bestBonuses = new LinkedList<Bonus>();

	private List<int> m_comboBonusPoints = new List<int>();

	public static BonusManager instance = null;

	private static Color[] cols = new Color[3]
	{
		new Color(173, 126, 0),
		new Color(160, 5, 5),
		new Color(1, 92, 149)
	};

	private static int[] totes = new int[7] { 1, 2, 3, 5, 8, 13, 21 };

	public static BonusManager GetInstance()
	{
		if (instance == null)
		{
			instance = new BonusManager();
		}
		return instance;
	}

	public void Init()
	{
		XDocument xDocument = MortarXml.Load("xmlwp7/bonusAwards.xml");
		if (xDocument == null)
		{
			return;
		}
		if (true)
		{
			XElement element = xDocument.FirstChildElement("bonusAwardsFile");
			for (XElement xElement = element.FirstChildElement("bonusType"); xElement != null; xElement = xElement.NextSiblingElement("bonusType"))
			{
				BonusType bonusType = new BonusType();
				bonusType.Parse(xElement);
				m_bonusTypes.Add(bonusType);
			}
			int value = 0;
			for (XElement xElement2 = element.FirstChildElement("combo"); xElement2 != null; xElement2 = xElement2.NextSiblingElement("combo"))
			{
				xElement2.QueryIntAttribute("bonus", ref value);
				m_comboBonusPoints.Add(value);
			}
		}
		xDocument = null;
	}

	public void SetUpBonusScreen(BonusScreen screen)
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		Game.game_work.scoreBeforeBonuses = Game.game_work.currentScore;
		Game.game_work.inBonusScreen = true;
		BonusScreen.bomb_magnet = false;
		m_bestBonuses.Clear();
		List<int> list = new List<int>();
		for (int i = 0; i < m_bonusTypes.Count; i++)
		{
			list.Add(i);
		}
		List<int> list2 = new List<int>();
		for (int j = 0; j < m_bonusTypes.Count; j++)
		{
			int index = Math.g_random.Rand32(list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		for (int k = 0; k < m_bonusTypes.Count; k++)
		{
			Bonus best = m_bonusTypes[list2[k]].GetBest();
			if (best != null)
			{
				m_bestBonuses.AddLast(best);
			}
		}
		LinkedList<Bonus> linkedList = new LinkedList<Bonus>();
		foreach (Bonus item in m_bestBonuses.OrderBy((Bonus fred) => fred.points))
		{
			linkedList.AddLast(item);
		}
		m_bestBonuses = linkedList;
		int num = -m_bestBonuses.Count + 3;
		LinkedListNode<Bonus> linkedListNode = m_bestBonuses.First;
		if (screen == null)
		{
			return;
		}
		while (linkedListNode != null)
		{
			if (num >= 0)
			{
				screen.AddAward(cols[num], linkedListNode.Value.texture, linkedListNode.Value.GetText(), linkedListNode.Value.GetPoints());
				linkedListNode = linkedListNode.Next;
			}
			else
			{
				LinkedListNode<Bonus> next = linkedListNode.Next;
				m_bestBonuses.Remove(linkedListNode);
				linkedListNode = next;
			}
			num++;
		}
	}

	public void AddCombo(int length)
	{
		Game.game_work.saveData.AddToTotal("combo_bonus", StringFunctions.StringHash("combo_bonus"), m_comboBonusPoints[Math.CLAMP(length - 3, 0, m_comboBonusPoints.Count - 1)], universal: false, achievementUpload: false);
		uint hash = StringFunctions.StringHash("best_combo");
		int total = Game.game_work.saveData.GetTotal(hash);
		Game.game_work.saveData.AddToTotal("best_combo", hash, Math.MAX(0, length - total), universal: false, achievementUpload: false);
	}

	public static int GetBonusTotal(uint hash)
	{
		uint num = StringFunctions.StringHash("score");
		if (hash == num)
		{
			return Game.game_work.currentScore;
		}
		return Game.game_work.saveData.GetTotal(hash);
	}

	public Bonus GetFirstBestBonus(ref LinkedListNode<Bonus> it)
	{
		it = m_bestBonuses.First;
		if (it != null)
		{
			return it.Value;
		}
		return null;
	}

	public Bonus GetNextBestBonus(ref LinkedListNode<Bonus> it)
	{
		it = it.Next;
		if (it != null)
		{
			return it.Value;
		}
		return null;
	}
}
