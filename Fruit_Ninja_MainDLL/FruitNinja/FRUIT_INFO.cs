using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class FRUIT_INFO
{
	public string fruitName;

	public string fruitSingular;

	public string fruitPlural;

	public string fruitNameTotal;

	public string fruitNamePointTotal;

	public string modelName;

	public Color splatColor;

	public float scale;

	public float collision;

	public float hitInfluence;

	public uint[] hash = new uint[6];

	public bool onSide;

	public int factCount;

	public FruitFact[] facts;

	public string factTextureName;

	public Color factColor;

	public bool onlySprinkle;

	public Texture icon;

	public Texture zenIcon;

	public int chance;

	public int totalChance;

	public int totalChanceOfCritical;

	public int score;

	public bool canBeCritical;

	public bool hasSplatSeeds;

	public ImpactSound[] impactSounds;

	public int numImpactSounds;

	public int coinsMin;

	public int coinsMax;

	public FRUIT_POWERS powers;

	public bool superFruit;

	public FRUIT_INFO()
	{
		superFruit = false;
		powers = null;
		hasSplatSeeds = false;
		numImpactSounds = 0;
		impactSounds = null;
		hitInfluence = 0.75f;
		coinsMin = 0;
		coinsMax = 0;
		score = 1;
		chance = 0;
		totalChance = 0;
		totalChanceOfCritical = 0;
		icon = null;
		zenIcon = null;
		onlySprinkle = false;
		onSide = false;
		factCount = 0;
		facts = null;
		canBeCritical = true;
	}
}
