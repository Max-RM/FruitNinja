using System.Collections.Generic;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class PROBABILITY_OVERIDE
{
	public int percentageChance;

	public int perWave;

	public int spawnedThisWave;

	public int typeCount;

	public List<string> typeNames = new List<string>();

	public int[] types = new int[WaveManager.MAX_PROBABILITY_OVERIDE_TYPES];

	public float canSpawnWithPowerup;

	public int waveCount;

	public int numWaves;

	public List<int> powerAllowanceChances = new List<int>();

	private static uint[] bombHashes = new uint[2]
	{
		StringFunctions.StringHash("bomb"),
		StringFunctions.StringHash("Bomb")
	};

	private static uint one_fruit = StringFunctions.StringHash("1fruit");

	public PROBABILITY_OVERIDE()
	{
		waveCount = -1000000;
		numWaves = -1;
		percentageChance = 0;
		perWave = 0;
		spawnedThisWave = 0;
		typeCount = 0;
		for (int i = 0; i < WaveManager.MAX_PROBABILITY_OVERIDE_TYPES; i++)
		{
			types[i] = -1;
		}
		canSpawnWithPowerup = 0f;
	}

	public void Parse(XElement element)
	{
		element.QueryIntAttribute("percentageChance", ref percentageChance);
		element.QueryIntAttribute("waveCount", ref waveCount);
		string text = element.AttributeStr("types");
		typeCount = StringFunctions.SplitWords(text, ref typeNames);
		element.QueryIntAttribute("perWave", ref perWave);
		element.QueryFloatAttribute("disableWhenPowered", ref canSpawnWithPowerup);
		element.QueryIntAttribute("numWaves", ref numWaves);
		for (XElement xElement = element.FirstChildElement("PowerAllowance"); xElement != null; xElement = xElement.NextSiblingElement("PowerAllowance"))
		{
			int value = 0;
			xElement.QueryIntAttribute("allowPercentage", ref value);
			powerAllowanceChances.Add(value);
		}
	}

	public void SelectType()
	{
		for (int i = 0; i < typeCount; i++)
		{
			uint num = StringFunctions.StringHash(typeNames[i]);
			if (num == bombHashes[0] || num == bombHashes[1])
			{
				types[i] = -2;
			}
			else if (num == one_fruit)
			{
				types[i] = Fruit.RandomFruit(canUseCoiners: false);
			}
			else
			{
				types[i] = Fruit.FruitType(typeNames[i]);
			}
		}
	}

	public new virtual int GetType()
	{
		int num = WaveManager.GetInstance().GetRandomiser().Rand32(typeCount);
		return types[num];
	}
}
