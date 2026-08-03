using System.Collections.Generic;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

internal class GlobalProbabilityOveride
{
	public class TypeChance
	{
		public string type;

		public int chance;

		public int chanceTotal;

		public TypeChance()
		{
			chance = 100;
		}
	}

	public int chanceTotal;

	public List<TypeChance> types = new List<TypeChance>();

	public string totalName;

	public uint totalNameHash;

	public uint modeMask;

	public int minWait;

	public int maxWait;

	private bool canSpawnWithPowers;

	private int dontSpawnBeforeWave;

	public GlobalProbabilityOveride()
	{
		totalName = "";
		totalNameHash = 0u;
		modeMask = uint.MaxValue;
		minWait = 0;
		maxWait = 0;
		chanceTotal = 0;
		canSpawnWithPowers = true;
		dontSpawnBeforeWave = 0;
	}

	public virtual bool CanSpawn()
	{
		if (WaveManager.GetInstance().GetCurrentWave() != null && WaveManager.GetInstance().GetCurrentWave().gamesMin == 0)
		{
			return true;
		}
		if (WaveManager.GetInstance().GetNumberOfWavesSpawned() > dontSpawnBeforeWave)
		{
			if (!canSpawnWithPowers)
			{
				if (Fruit.NumberOfPowerupFruits() <= 0)
				{
					return PowerUpManager.GetInstance().GetActiveProgression() >= 2f;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public void Parse(XElement element)
	{
		totalName = element.AttributeStr("name");
		totalNameHash = StringFunctions.StringHash(totalName);
		modeMask = Game.ParseModeMask(element.AttributeStr("mode"));
		element.QueryIntAttribute("minWait", ref minWait);
		element.QueryIntAttribute("maxWait", ref maxWait);
		element.QueryIntAttribute("dontSpawnBeforeWave", ref dontSpawnBeforeWave);
		if (element.AttributeStr("canSpawnWithPowers") != null)
		{
			canSpawnWithPowers = string.Compare("true", element.AttributeStr("canSpawnWithPowers")) == 0;
		}
		chanceTotal = 0;
		for (XElement xElement = element.FirstChildElement("fruit"); xElement != null; xElement = xElement.NextSiblingElement("fruit"))
		{
			TypeChance typeChance = new TypeChance();
			typeChance.type = xElement.AttributeStr("type");
			xElement.QueryIntAttribute("chance", ref typeChance.chance);
			chanceTotal += typeChance.chance;
			typeChance.chanceTotal = chanceTotal;
			types.Add(typeChance);
		}
		ParseSpecific(element);
	}

	public virtual void ParseSpecific(XElement element)
	{
	}

	public virtual bool CheckForOverride(ref int type)
	{
		if ((Game.GetModeBitMask(Game.game_work.gameMode) & modeMask) != modeMask)
		{
			return false;
		}
		int total = Game.game_work.saveData.GetTotal(totalNameHash);
		if (total == 0)
		{
			Game.game_work.saveData.AddToTotal(totalName, totalNameHash, (int)TransitionFunctions.LerpF((float)minWait, (float)maxWait, Math.g_random.RandF(1f)), universal: true, achievementUpload: true);
		}
		else if (total > 0)
		{
			if (total > 1)
			{
				Game.game_work.saveData.AddToTotal(totalName, totalNameHash, -1, universal: true, achievementUpload: true);
			}
			if (total == 1 && CanSpawn())
			{
				Game.game_work.saveData.AddToTotal(totalName, totalNameHash, -1, universal: true, achievementUpload: true);
				type = PickFruit();
				return true;
			}
		}
		return false;
	}

	public int PickFruit()
	{
		int num = Math.g_random.Rand32(chanceTotal);
		for (int i = 0; i < types.Count; i++)
		{
			if (num < types[i].chanceTotal)
			{
				int num2 = Fruit.FruitType(types[i].type);
				if (num2 >= 0)
				{
					return num2;
				}
			}
		}
		return 0;
	}

	public void FruitWasKilled(Fruit fruit)
	{
		if (fruit != null && !fruit.IsActive())
		{
			PushbackSpawn();
		}
	}

	public void FruitWasThrown(Fruit fruit)
	{
		if (fruit != null && !CanSpawn())
		{
			fruit.KillFruit();
			PushbackSpawn();
		}
	}

	public virtual void PushbackSpawn()
	{
		Game.game_work.saveData.SetTotal(totalName, 3);
	}

	public virtual void NewGameStarted()
	{
	}
}
