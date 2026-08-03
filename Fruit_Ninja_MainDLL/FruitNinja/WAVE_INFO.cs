using System.Collections.Generic;

namespace FruitNinja;

public class WAVE_INFO
{
	public int waveNo;

	public int waveNoRange;

	public SPAWNER_INFO[] spawners;

	public int spawnerCount;

	public float deltaT;

	public float deltaTinc;

	public float deltaSpinc;

	public float speedLoss;

	public float beforeDelay;

	public float beforeDelayInc;

	public float nextDelay;

	public float nextDelayInc;

	public float nextDelaySpInc;

	public float m_inc;

	public bool waitForEntities;

	public int chance;

	public int currentChance;

	public float chanceRegrowth;

	public float currentChanceRegrowth;

	public int gamesMin;

	public int gamesMax;

	public List<string> typesToPickFrom = new List<string>();

	public int typesToPickFromCount;

	public float criticalChance;

	public int idx;

	public COIN_CHANCEINATOR coinChanceinator;

	public int overideProbabilty;

	public WAVE_INFO Clone()
	{
		return null;
	}

	public WAVE_INFO()
	{
		m_inc = 0f;
		overideProbabilty = 100;
		gamesMin = -1;
		gamesMax = -1;
		spawnerCount = 0;
		waveNo = 0;
		waveNoRange = -1;
		deltaT = 1f;
		deltaTinc = 0f;
		beforeDelay = 2f;
		beforeDelayInc = 0f;
		nextDelay = 0f;
		nextDelayInc = 0f;
		nextDelaySpInc = 0f;
		deltaSpinc = 0f;
		speedLoss = 0f;
		chance = 10;
		chanceRegrowth = 0.25f;
		currentChanceRegrowth = 0.25f;
		spawners = null;
		criticalChance = 1f;
		coinChanceinator = null;
		idx = 0;
		waitForEntities = true;
	}

	public WAVE_INFO(DEFAULT_WAVE_INFO defaultInfo)
	{
		m_inc = 0f;
		overideProbabilty = 100;
		gamesMin = -1;
		gamesMax = -1;
		spawnerCount = 0;
		waveNo = 0;
		waveNoRange = -1;
		deltaT = 1f;
		deltaTinc = 0f;
		beforeDelay = 2f;
		beforeDelayInc = 0f;
		nextDelay = 0f;
		nextDelayInc = 0f;
		nextDelaySpInc = 0f;
		deltaSpinc = 0f;
		chance = 10;
		chanceRegrowth = 0.25f;
		currentChanceRegrowth = 0.25f;
		spawners = null;
		criticalChance = 1f;
		idx = 0;
		coinChanceinator = null;
		waitForEntities = true;
		speedLoss = 0f;
		deltaT = defaultInfo.dt;
		deltaTinc = defaultInfo.dtInc;
		deltaSpinc = defaultInfo.dtSpInc;
		beforeDelay = defaultInfo.beforeDelay;
		beforeDelayInc = defaultInfo.beforeDelayInc;
		nextDelay = defaultInfo.nextDelay;
		nextDelayInc = defaultInfo.nextDelayInc;
		nextDelaySpInc = defaultInfo.nextDelaySpInc;
		chance = defaultInfo.waveChance;
		chanceRegrowth = defaultInfo.waveChanceRegrowth;
		currentChanceRegrowth = defaultInfo.waveChanceRegrowth;
		criticalChance = defaultInfo.criticalChance;
		waitForEntities = defaultInfo.waitForEntities;
		speedLoss = defaultInfo.speedLoss;
		overideProbabilty = defaultInfo.overideProbabilty;
	}
}
