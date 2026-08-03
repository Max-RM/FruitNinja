using System.Collections.Generic;
using Mortar;

namespace FruitNinja;

public class FruitSaveData
{
	public Dictionary<uint, SliceTotal> totals = new Dictionary<uint, SliceTotal>();

	public Dictionary<uint, SliceTotal> universalTotals = new Dictionary<uint, SliceTotal>();

	public bool resume;

	public bool inGame;

	public bool game_rated;

	public LinkedList<EntityState> entities = new LinkedList<EntityState>();

	public int highScore;

	public int[] highScores = new int[4];

	public int score;

	public int misses;

	public int mode;

	public bool hasDropped;

	public int consecutiveType;

	public int consecutiveCount;

	public int numFruitTypesToPickFrom;

	public int[] fruitTypesToPickFrom = new int[32];

	public float speedLossTime;

	public float desiredSpeed;

	public float nextComboBonus;

	public float timer;

	public int numberOfWavesSpawned;

	public int criticalProgression;

	public int go_state;

	public float go_time;

	public int go_body;

	public int go_head;

	public int go_fruit;

	public int go_fact;

	public bool go_showHighScore;

	public bool go_setScore;

	public float go_bombHitTime;

	public float go_transition;

	public float shake_time;

	public float shake_max_time;

	public int currentWave;

	public float currentWaveDelay;

	public float currentWaveWait;

	public float globalWaveDt;

	public LinkedList<WaveState> waves = new LinkedList<WaveState>();

	public Dictionary<uint, AchievementItem> AchievementQue = new Dictionary<uint, AchievementItem>();

	public Dictionary<uint, AchievementItem> unlockedAchievements = new Dictionary<uint, AchievementItem>();

	public int m_blitzSpawnedThisGame;

	public int m_blitzForceSpawnedCounter;

	public float m_blitzSpawnTime;

	public Dictionary<int, int>[] waveGameCount = ArrayInit.CreateFilledArray<Dictionary<int, int>>(4);

	public int saveVersion;

	private static uint[] IDS = new uint[17]
	{
		StringFunctions.StringHash("261754"),
		StringFunctions.StringHash("261464"),
		StringFunctions.StringHash("261514"),
		StringFunctions.StringHash("261524"),
		StringFunctions.StringHash("261544"),
		StringFunctions.StringHash("261554"),
		StringFunctions.StringHash("261574"),
		StringFunctions.StringHash("261584"),
		StringFunctions.StringHash("261654"),
		StringFunctions.StringHash("261794"),
		StringFunctions.StringHash("324154"),
		StringFunctions.StringHash("324144"),
		StringFunctions.StringHash("324524"),
		StringFunctions.StringHash("350264"),
		StringFunctions.StringHash("350274"),
		StringFunctions.StringHash("370914"),
		StringFunctions.StringHash("374004")
	};

	private static string[] wp7Keys = new string[18]
	{
		"Wake Up", "Great Fruit Ninja", "Ultimate Fruit Ninja", "Fruit Fight", "Fruit Blitz", "Fruit Frenzy", "Fruit Rampage", "Fruit Annihilation", "Lucky Ninja", "Almost A Century",
		"Mango Magic", "Its All Pear Shaped", "Deja Vu", "Combo Mambo", "Moment Of Zen", "Year Of The Dragon", "The Lovely Bunch", "AAA"
	};

	public int numFruitTypesInSliceCombo;

	public int[] sliceComboFruitTypes = new int[11];

	public FruitSaveData()
	{
		score = 0;
		misses = 0;
		game_rated = false;
		go_time = -1f;
		go_transition = -1f;
		go_state = -1;
		go_bombHitTime = 0f;
		go_head = -1;
		go_body = -1;
		go_fruit = -1;
		go_fact = -1;
		go_showHighScore = false;
		go_setScore = false;
		criticalProgression = 70;
		shake_max_time = 1f;
		shake_time = 0f;
		highScore = 0;
		consecutiveCount = 0;
		consecutiveType = -1;
		mode = 0;
		timer = -1f;
		numFruitTypesToPickFrom = 0;
		for (int i = 0; i < 25; i++)
		{
			fruitTypesToPickFrom[i] = -1;
		}
		for (int j = 0; j < 4; j++)
		{
			highScores[j] = 0;
			waveGameCount[j].Clear();
		}
		currentWave = 0;
		currentWaveDelay = 0f;
		currentWaveWait = 0f;
		waves.Clear();
		AchievementQue.Clear();
		unlockedAchievements.Clear();
		saveVersion = 0;
		resume = false;
		inGame = false;
		hasDropped = false;
		totals.Clear();
		entities.Clear();
		numFruitTypesInSliceCombo = 0;
		for (int k = 0; k < 11; k++)
		{
			sliceComboFruitTypes[k] = -1;
		}
		desiredSpeed = 0f;
		speedLossTime = 0f;
		nextComboBonus = 0f;
	}

	public FruitSaveData CLOAN()
	{
		FruitSaveData fruitSaveData = new FruitSaveData();
		if (waveGameCount != null)
		{
			fruitSaveData.waveGameCount = new Dictionary<int, int>[waveGameCount.Length];
			for (int i = 0; i < waveGameCount.Length; i++)
			{
				if (waveGameCount[i] != null)
				{
					fruitSaveData.waveGameCount[i] = waveGameCount[i].Duplicate();
				}
			}
		}
		fruitSaveData.totals = totals.Duplicate();
		fruitSaveData.universalTotals = universalTotals.Duplicate();
		fruitSaveData.entities = entities.Duplicate();
		fruitSaveData.waves = waves.Duplicate();
		fruitSaveData.highScores = (int[])highScores.Clone();
		fruitSaveData.fruitTypesToPickFrom = (int[])fruitTypesToPickFrom.Clone();
		fruitSaveData.resume = resume;
		fruitSaveData.inGame = inGame;
		fruitSaveData.game_rated = game_rated;
		fruitSaveData.highScore = highScore;
		fruitSaveData.score = score;
		fruitSaveData.misses = misses;
		fruitSaveData.mode = mode;
		fruitSaveData.hasDropped = hasDropped;
		fruitSaveData.consecutiveType = consecutiveType;
		fruitSaveData.consecutiveCount = consecutiveCount;
		fruitSaveData.numFruitTypesToPickFrom = numFruitTypesToPickFrom;
		fruitSaveData.timer = timer;
		fruitSaveData.criticalProgression = criticalProgression;
		fruitSaveData.go_state = go_state;
		fruitSaveData.go_time = go_time;
		fruitSaveData.go_body = go_body;
		fruitSaveData.go_head = go_head;
		fruitSaveData.go_fruit = go_fruit;
		fruitSaveData.go_fact = go_fact;
		fruitSaveData.go_showHighScore = go_showHighScore;
		fruitSaveData.go_setScore = go_setScore;
		fruitSaveData.go_bombHitTime = go_bombHitTime;
		fruitSaveData.go_transition = go_transition;
		fruitSaveData.shake_time = shake_time;
		fruitSaveData.shake_max_time = shake_max_time;
		fruitSaveData.currentWave = currentWave;
		fruitSaveData.currentWaveDelay = currentWaveDelay;
		fruitSaveData.saveVersion = saveVersion;
		fruitSaveData.m_blitzSpawnedThisGame = m_blitzSpawnedThisGame;
		fruitSaveData.m_blitzForceSpawnedCounter = m_blitzForceSpawnedCounter;
		fruitSaveData.m_blitzSpawnTime = m_blitzSpawnTime;
		fruitSaveData.AchievementQue = AchievementQue.Duplicate();
		fruitSaveData.unlockedAchievements = unlockedAchievements.Duplicate();
		return fruitSaveData;
	}

	public void SaveGameState()
	{
	}

	public int SetTotal(string name, int amount)
	{
		return SetTotal(name, amount, universal: true, achievementUpload: true);
	}

	public int SetTotal(string name, int amount, bool universal, bool achievementUpload)
	{
		uint hash = StringFunctions.StringHash(name);
		int total = GetTotal(hash);
		AddToTotal(name, hash, amount - total, universal, achievementUpload);
		return total;
	}

	public int AddToTotal(string name, uint hash, int amount)
	{
		return AddToTotal(name, hash, amount, universal: true);
	}

	public int AddToTotal(string name, uint hash, int amount, bool universal)
	{
		return AddToTotal(name, hash, amount, universal, achievementUpload: true);
	}

	public int AddToTotal(string name, uint hash, int amount, bool universal, bool achievementUpload)
	{
		Dictionary<uint, SliceTotal> dictionary = (universal ? universalTotals : totals);
		if (dictionary.TryGetValue(hash, out var value))
		{
			value.total += amount;
			dictionary[hash] = value;
			if (achievementUpload)
			{
				AchievementManager.GetInstance().UnlockSpecificFruitAchievement(value.total, hash);
			}
			return value.total;
		}
		SliceTotal value2 = default(SliceTotal);
		value2.hash = hash;
		value2.name = name;
		value2.total = amount;
		dictionary[hash] = value2;
		AchievementManager.GetInstance().UnlockSpecificFruitAchievement(value2.total, hash);
		return value2.total;
	}

	public void UnlockTotals()
	{
		foreach (KeyValuePair<uint, SliceTotal> universalTotal in universalTotals)
		{
			AchievementManager.GetInstance().UnlockSpecificFruitAchievement(universalTotal.Value.total, universalTotal.Key);
		}
		foreach (KeyValuePair<uint, SliceTotal> total in totals)
		{
			AchievementManager.GetInstance().UnlockSpecificFruitAchievement(total.Value.total, total.Key);
		}
	}

	public int GetTotal(uint hash)
	{
		if (totals.TryGetValue(hash, out var value))
		{
			return value.total;
		}
		if (universalTotals.TryGetValue(hash, out value))
		{
			return value.total;
		}
		return 0;
	}

	public bool IsAchievementUnlocked(uint nameHash)
	{
		foreach (KeyValuePair<uint, AchievementItem> item in AchievementQue)
		{
			if (item.Key == nameHash)
			{
				return true;
			}
		}
		foreach (KeyValuePair<uint, AchievementItem> unlockedAchievement in unlockedAchievements)
		{
			if (unlockedAchievement.Key == nameHash)
			{
				return true;
			}
		}
		return false;
	}

	public bool AddToQue(string name, uint nameHash)
	{
		if (IsAchievementUnlocked(nameHash))
		{
			return false;
		}
		AchievementItem value = new AchievementItem
		{
			name = name,
			time = 2.9f
		};
		if (AchievementQue.Count > 0)
		{
			value.time += 0.1f;
		}
		AchievementQue[nameHash] = value;
		if (!TheGame.emulator)
		{
			uint num = StringFunctions.StringHash(name);
			for (int i = 0; i < IDS.Length; i++)
			{
				if (IDS[i] == num)
				{
					AchievementManager.AwardAchievementWP7(wp7Keys[i]);
				}
			}
		}
		return true;
	}

	public void Update(float dt, HUD hud)
	{
		float num = 100f;
		bool flag = false;
		KeyValuePair<uint, AchievementItem> keyValuePair = default(KeyValuePair<uint, AchievementItem>);
		foreach (KeyValuePair<uint, AchievementItem> item in AchievementQue)
		{
			if (item.Value.time < num)
			{
				num = item.Value.time;
				keyValuePair = item;
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		float time = keyValuePair.Value.time;
		AchievementItem value = keyValuePair.Value;
		value.time -= dt;
		AchievementQue[keyValuePair.Key] = value;
		if (time >= 2.89f && value.time < 2.89f)
		{
			if (Math.BETWEEN(keyValuePair.Value.name[0], 48, 57))
			{
				NetworkManager.GetInstance().UnlockAchievement(keyValuePair.Value.name);
			}
			else
			{
				ItemManager.GetInstance().UnlockItem(keyValuePair.Key);
			}
			AchievementManager.GetInstance().UnlockedAchievement(keyValuePair.Key, hud);
		}
		if (keyValuePair.Value.time <= 0f)
		{
			unlockedAchievements[keyValuePair.Key] = keyValuePair.Value;
			AchievementQue.Remove(keyValuePair.Key);
		}
	}

	public void ClearTotals()
	{
		totals.Clear();
	}

	public void ClearTotal(uint hash)
	{
		totals.Remove(hash);
		universalTotals.Remove(hash);
	}

	public void ClearCombo()
	{
		numFruitTypesInSliceCombo = 0;
		for (int i = 0; i < 11; i++)
		{
			sliceComboFruitTypes[i] = -1;
		}
	}

	public void FinishedGame()
	{
		for (uint num = 0u; num < 4; num++)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, int> item in waveGameCount[num])
			{
				if (item.Value > -1)
				{
					list.Add(item.Key);
				}
			}
			foreach (int item2 in list)
			{
				int num2 = waveGameCount[num][item2];
				num2--;
				waveGameCount[num][item2] = num2;
			}
		}
	}
}
