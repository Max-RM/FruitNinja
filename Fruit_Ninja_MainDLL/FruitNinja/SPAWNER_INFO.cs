using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class SPAWNER_INFO
{
	public int[] randomTypes;

	public List<string> types = new List<string>();

	public int typeCount;

	public float dt;

	public Vector3 gravity;

	public float velXscale;

	public float velYscale;

	public float horizontalMin;

	public float horizontalMax;

	public SPAWN_PLACEMENTS placement;

	public float minSpawn;

	public float minInc;

	public float maxSpawn;

	public float maxInc;

	public float delay;

	public float delayInc;

	private static uint[] bombHashes = new uint[2]
	{
		StringFunctions.StringHash("bomb"),
		StringFunctions.StringHash("Bomb")
	};

	private static uint one_fruit = StringFunctions.StringHash("1fruit");

	public int toSpawnThisWave;

	public int maxToSpawnThisWave;

	public int bombsSpawnedThisWave;

	public float delayWait;

	public bool mirror;

	public bool isClone;

	public void ResetDelay(float inc)
	{
		delayWait = Math.MAX(0f, delay + inc * delayInc);
	}

	public void Reset(float inc)
	{
		toSpawnThisWave = GetRandCount(inc);
		maxToSpawnThisWave = toSpawnThisWave;
		bombsSpawnedThisWave = 0;
		SelectTypes();
		ResetDelay(inc);
	}

	public void SelectTypes()
	{
		for (int i = 0; i < typeCount; i++)
		{
			randomTypes[i] = -1;
			uint num = StringFunctions.StringHash(types[i]);
			if (num == bombHashes[0] || num == bombHashes[1])
			{
				randomTypes[i] = -2;
			}
			else if (num == one_fruit)
			{
				randomTypes[i] = Fruit.RandomFruit(canUseCoiners: false);
			}
			else
			{
				randomTypes[i] = Fruit.FruitType(types[i]);
			}
		}
	}

	public int GetRandCount(float inc)
	{
		int num = Math.MAX(0, (int)(minSpawn + inc * minInc));
		int num2 = Math.MAX(0, (int)(maxSpawn + inc * maxInc));
		int num3 = num2 - num;
		return num + ((num3 > 0) ? Math.g_random.Rand32(num3) : 0);
	}

	public SPAWNER_INFO()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		dt = 1f;
		isClone = false;
		horizontalMin = -1f;
		horizontalMax = 1f;
		gravity = new Vector3(0f, -1f, 0f);
		placement = SPAWN_PLACEMENTS.SPAWNER_BOTTOM;
		delay = 0f;
		delayInc = 0f;
		typeCount = 0;
		minSpawn = (minInc = (maxSpawn = (maxInc = 0f)));
		toSpawnThisWave = 0;
		delayWait = 0f;
		randomTypes = null;
		velXscale = 1f;
		velYscale = 1f;
		mirror = false;
	}
}
