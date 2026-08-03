using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class WaveManager
{
	public const float COMBOS_REQUIRED_FOR_NEW_BLITZ = 2.5f;

	public const float MAX_COMBO_BONUS = 6f;

	public bool paused;

	public static float MAX_SPEEDIE_GONZALES = 14f;

	public Mortar.Math.Random w_random = new Mortar.Math.Random();

	private Mortar.Math.Random m_random = new Mortar.Math.Random();

	public static int MAX_PROBABILITY_OVERIDE_TYPES = 20;

	public static float FRUIT_SPAWN_MIN_X = -150f;

	public static float FRUIT_SPAWN_MAX_X = 150f;

	public static float FRUIT_ANGLE_RANGE = 20f;

	public static float FRUIT_MAX_VEL_X = 15f;

	public static float FRUIT_MIN_VEL_Y = 9f;

	public static float FRUIT_MAX_VEL_Y = 11f;

	public static float FRUIT_MIN_VEL = 9.5f;

	public static float FRUIT_MAX_VEL = 11f;

	public static float FRUIT_SPAWN_Z_OFFSET = 32f;

	private string[] gameModeWaveListXMLs = new string[4] { "xml/originalWaveList.xml", "xml/comboWaveList.xml", "xml/arcadeWaveList.xml", "xml/zenWaveList.xml" };

	private static SPAWNER_INFO[] spinfos = ArrayInit.CreateFilledArray<SPAWNER_INFO>(3);

	private static bool made_spawners = false;

	private static bool still_spawning;

	private int[] m_numberOfWavesSpawned = new int[Game.MAX_PLAYERS];

	private List<GlobalProbabilityOveride> m_globalProbabilityOverides = new List<GlobalProbabilityOveride>();

	private float m_absoluteGlobalDtMod;

	private static uint[] stringHashesddd = new uint[5]
	{
		StringFunctions.StringHash("BOTTOM"),
		StringFunctions.StringHash("TOP"),
		StringFunctions.StringHash("LEFT"),
		StringFunctions.StringHash("RIGHT"),
		StringFunctions.StringHash("LEFT_RIGHT")
	};

	private static WaveManager instance = new WaveManager();

	private SpeedControl[] m_speedControl = new SpeedControl[Game.MAX_PLAYERS];

	private float[] m_speedLossTime = new float[Game.MAX_PLAYERS];

	private float[] m_speed = new float[Game.MAX_PLAYERS];

	private float[] m_desiredSpeed = new float[Game.MAX_PLAYERS];

	private int[] m_comboBonus = new int[Game.MAX_PLAYERS];

	private float[] m_nextComboBonus = new float[Game.MAX_PLAYERS];

	private float m_bombScale;

	private float m_bombMultiplyer;

	private float m_fruitMultiplyer;

	private float m_criticalChanceMod;

	private float m_currentGlobalDt;

	private float m_currentGlobalDtMod;

	private float[] m_globalDtInc = new float[4];

	private float[] m_globalDtStart = new float[4];

	private float[] m_globalDtMax = new float[4];

	private List<WAVE_INFO>[,] m_lists = ArrayInit.CreateFilledArray<List<WAVE_INFO>>(Game.MAX_PLAYERS, 4);

	private DEFAULT_WAVE_INFO[] m_defaultInfo = ArrayInit.CreateFilledArray<DEFAULT_WAVE_INFO>(4);

	private COIN_CHANCEINATOR[] m_coinChanceLists = ArrayInit.CreateFilledArray<COIN_CHANCEINATOR>(4);

	private List<PROBABILITY_OVERIDE>[,] m_probabilityOverides = ArrayInit.CreateFilledArray<List<PROBABILITY_OVERIDE>>(Game.MAX_PLAYERS, 4);

	private WAVE_INFO[] m_currentWave = ArrayInit.CreateFilledArray<WAVE_INFO>(Game.MAX_PLAYERS);

	private int[] m_waveCount = new int[Game.MAX_PLAYERS];

	private float[] m_waveDelay = new float[Game.MAX_PLAYERS];

	private float[] m_waveWait = new float[Game.MAX_PLAYERS];

	private bool[] processingWave = new bool[Game.MAX_PLAYERS];

	private byte m_blitzSpawnedThisGame;

	private byte m_blitzForceSpawnedCounter;

	private float m_blitzSpawnTime;

	private int[,] m_fruitTypesToPickFrom = new int[Game.MAX_PLAYERS, 32];

	private int[] m_fruitTypesToPickFromWave = new int[Game.MAX_PLAYERS];

	private int[] m_numFruitTypesToPickFrom = new int[Game.MAX_PLAYERS];

	public static float FRUIT_SPAWN_RANGE => FRUIT_SPAWN_MAX_X - FRUIT_SPAWN_MIN_X;

	public static float FRUIT_SPAWN_Y => (0f - Game.SCREEN_HEIGHT) / 2f;

	public static float SPEED_START => 2.9f;

	public static float MAX_DELTA_TIME_SCALE => 100f;

	public float GetAbsoluteDtMod()
	{
		return m_absoluteGlobalDtMod;
	}

	public void SetAbsoluteDtMod()
	{
		m_absoluteGlobalDtMod = 1f;
	}

	public void SetAbsoluteDtMod(float mod)
	{
		m_absoluteGlobalDtMod = mod;
	}

	private bool ParseCoinChanceinator(COIN_CHANCEINATOR coinChanceinater, XElement pParent)
	{
		return true;
	}

	private SPAWN_PLACEMENTS ParsePlacement(string text)
	{
		uint num = StringFunctions.StringHash(text);
		for (int i = 0; i < 5; i++)
		{
			if (num == stringHashesddd[i])
			{
				return (SPAWN_PLACEMENTS)i;
			}
		}
		return SPAWN_PLACEMENTS.SPAWNER_BOTTOM;
	}

	public static WaveManager GetInstance()
	{
		return instance;
	}

	public void Init()
	{
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		m_absoluteGlobalDtMod = 1f;
		Fruit.LoadInfo();
		for (int i = 0; i < 4; i++)
		{
			XDocument xDocument = MortarXml.Load(gameModeWaveListXMLs[i]);
			if (xDocument != null)
			{
				XElement element = xDocument.FirstChildElement("waveManagerFile");
				element = element.FirstChildElement();
				m_defaultInfo[i].Reset();
				int num = 0;
				while (element != null)
				{
					switch (element.Name.LocalName)
					{
					case "WaveInfo":
					{
						WAVE_INFO wAVE_INFO = new WAVE_INFO(m_defaultInfo[i]);
						wAVE_INFO.idx = num;
						num++;
						XElement xElement = element;
						int value = -1;
						xElement.QueryIntAttribute("waveNo", ref value);
						if (value >= 0 || i == 2)
						{
							wAVE_INFO.waveNo = value;
						}
						xElement.QueryIntAttribute("overideProbabiltyPool", ref wAVE_INFO.overideProbabilty);
						string text = null;
						if (xElement.Attribute("until") != null)
						{
							text = xElement.Attribute("until").Value;
							if (text == "forever")
							{
								wAVE_INFO.waveNoRange = -2;
							}
							else
							{
								value = MParser.ParseInt(text);
								if (value >= 0 && value >= wAVE_INFO.waveNo)
								{
									wAVE_INFO.waveNoRange = value;
								}
							}
						}
						value = -1;
						xElement.QueryIntAttribute("chance", ref value);
						if (value >= 0)
						{
							wAVE_INFO.chance = value;
						}
						xElement.QueryFloatAttribute("criticalChance", ref wAVE_INFO.criticalChance);
						xElement.QueryFloatAttribute("chanceRegrowth", ref wAVE_INFO.chanceRegrowth);
						xElement.QueryIntAttribute("games", ref wAVE_INFO.gamesMin);
						xElement.QueryIntAttribute("gamesMin", ref wAVE_INFO.gamesMin);
						xElement.QueryIntAttribute("gamesMax", ref wAVE_INFO.gamesMax);
						if (wAVE_INFO.gamesMin < 0)
						{
							wAVE_INFO.gamesMin = wAVE_INFO.gamesMax;
						}
						if (wAVE_INFO.gamesMax < 0)
						{
							wAVE_INFO.gamesMax = wAVE_INFO.gamesMin;
						}
						XElement xElement2 = xElement.FirstChildElement("coin_chances");
						if (xElement2 != null)
						{
							wAVE_INFO.coinChanceinator = new COIN_CHANCEINATOR();
							ParseCoinChanceinator(wAVE_INFO.coinChanceinator, xElement2);
						}
						xElement2 = xElement.FirstChildElement("Spawn");
						uint num2 = StringFunctions.StringHash("Spawn");
						while (xElement2 != null)
						{
							if (StringFunctions.StringHash(xElement2.Name.LocalName) == num2)
							{
								wAVE_INFO.spawnerCount++;
							}
							xElement2 = xElement2.NextSiblingElement();
						}
						if (wAVE_INFO.spawnerCount > 0)
						{
							wAVE_INFO.spawners = ArrayInit.CreateFilledArray<SPAWNER_INFO>(wAVE_INFO.spawnerCount);
						}
						xElement2 = xElement.FirstChildElement("Spawn");
						text = null;
						int num3 = 0;
						SPAWNER_INFO sPAWNER_INFO = null;
						if (wAVE_INFO.spawners != null)
						{
							sPAWNER_INFO = wAVE_INFO.spawners[num3];
						}
						while (xElement2 != null)
						{
							sPAWNER_INFO = wAVE_INFO.spawners[num3];
							if (StringFunctions.StringHash(xElement2.Name.LocalName) == num2)
							{
								text = xElement2.Attribute("type").Value;
								sPAWNER_INFO.typeCount = StringFunctions.SplitWords(text, ref sPAWNER_INFO.types);
								if (sPAWNER_INFO.typeCount > 0)
								{
									sPAWNER_INFO.randomTypes = new int[sPAWNER_INFO.typeCount];
									for (int k = 0; k < sPAWNER_INFO.typeCount; k++)
									{
										sPAWNER_INFO.randomTypes[k] = -1;
									}
								}
								xElement2.QueryFloatAttribute("min", ref sPAWNER_INFO.minSpawn);
								xElement2.QueryFloatAttribute("max", ref sPAWNER_INFO.maxSpawn);
								xElement2.QueryFloatAttribute("mininc", ref sPAWNER_INFO.maxInc);
								xElement2.QueryFloatAttribute("maxinc", ref sPAWNER_INFO.maxInc);
								xElement2.QueryFloatAttribute("delay", ref sPAWNER_INFO.delay);
								xElement2.QueryFloatAttribute("delayinc", ref sPAWNER_INFO.delayInc);
								xElement2.QueryFloatAttribute("horizmin", ref sPAWNER_INFO.horizontalMin);
								xElement2.QueryFloatAttribute("horizmax", ref sPAWNER_INFO.horizontalMax);
								xElement2.QueryFloatAttribute("velscale", ref sPAWNER_INFO.velXscale);
								sPAWNER_INFO.velYscale = sPAWNER_INFO.velXscale;
								xElement2.QueryFloatAttribute("velXscale", ref sPAWNER_INFO.velXscale);
								xElement2.QueryFloatAttribute("velYscale", ref sPAWNER_INFO.velYscale);
								if (xElement2.Attribute("gravity") != null)
								{
									text = xElement2.Attribute("gravity").Value;
									sPAWNER_INFO.gravity = Save.ParseVector(text);
								}
								if (xElement2.Attribute("placement") != null)
								{
									text = xElement2.Attribute("placement").Value;
									sPAWNER_INFO.placement = ParsePlacement(text);
								}
								else
								{
									sPAWNER_INFO.placement = SPAWN_PLACEMENTS.SPAWNER_BOTTOM;
								}
								text = xElement2.AttributeStr("mirror");
								sPAWNER_INFO.mirror = text != null && "true" == text;
							}
							xElement2 = xElement2.NextSiblingElement("Spawn");
							num3++;
						}
						xElement2 = xElement.FirstChildElement("Wave_dt");
						if (xElement2 != null)
						{
							xElement2.QueryFloatAttribute("dt", ref wAVE_INFO.deltaT);
							xElement2.QueryFloatAttribute("inc", ref wAVE_INFO.deltaTinc);
							xElement2.QueryFloatAttribute("spinc", ref wAVE_INFO.deltaSpinc);
						}
						xElement2 = xElement.FirstChildElement("NextWaveDelay");
						if (xElement2 != null)
						{
							xElement2.QueryFloatAttribute("wait", ref wAVE_INFO.nextDelay);
							xElement2.QueryFloatAttribute("waitSpinc", ref wAVE_INFO.nextDelaySpInc);
							xElement2.QueryFloatAttribute("speedLoss", ref wAVE_INFO.speedLoss);
							if (wAVE_INFO.nextDelay > 0f)
							{
								wAVE_INFO.beforeDelay = 0f;
								wAVE_INFO.beforeDelayInc = 0f;
							}
							xElement2.QueryFloatAttribute("delay", ref wAVE_INFO.beforeDelay);
							xElement2.QueryFloatAttribute("inc", ref wAVE_INFO.beforeDelayInc);
							string text2 = xElement2.AttributeStr("waitForEntities");
							wAVE_INFO.waitForEntities = text2 == null || !("false" == text2);
						}
						xElement2 = xElement.FirstChildElement("ChooseFrom");
						wAVE_INFO.typesToPickFrom.Clear();
						wAVE_INFO.typesToPickFromCount = 0;
						if (xElement2 != null)
						{
							text = xElement2.AttributeStr("types");
							StringFunctions.SplitWords(text, ref wAVE_INFO.typesToPickFrom);
						}
						m_lists[m_defaultInfo[i].players[0], i].Add(wAVE_INFO);
						int num4 = 1;
						while (num4 < Game.MAX_PLAYERS && m_defaultInfo[i].players[num4] >= 0)
						{
							WAVE_INFO item = wAVE_INFO.Clone();
							m_lists[m_defaultInfo[i].players[num4], i].Add(item);
							num4++;
							num4++;
						}
						break;
					}
					case "defaults":
					{
						m_defaultInfo[i].Reset();
						element.QueryIntAttribute("waveChance", ref m_defaultInfo[i].waveChance);
						element.QueryFloatAttribute("waveChanceRegrowth", ref m_defaultInfo[i].waveChanceRegrowth);
						element.QueryFloatAttribute("criticalChance", ref m_defaultInfo[i].criticalChance);
						element.QueryFloatAttribute("dt", ref m_defaultInfo[i].dt);
						element.QueryFloatAttribute("dtInc", ref m_defaultInfo[i].dtInc);
						element.QueryFloatAttribute("dtSpInc", ref m_defaultInfo[i].dtSpInc);
						element.QueryFloatAttribute("beforeDelay", ref m_defaultInfo[i].beforeDelay);
						element.QueryFloatAttribute("beforeDelayInc", ref m_defaultInfo[i].beforeDelayInc);
						element.QueryFloatAttribute("nextDelay", ref m_defaultInfo[i].nextDelay);
						element.QueryFloatAttribute("nextDelayInc", ref m_defaultInfo[i].nextDelayInc);
						element.QueryFloatAttribute("nextDelaySpInc", ref m_defaultInfo[i].nextDelaySpInc);
						element.QueryFloatAttribute("speedLoss", ref m_defaultInfo[i].speedLoss);
						element.QueryIntAttribute("overideProbabiltyPool", ref m_defaultInfo[i].overideProbabilty);
						string text3 = element.AttributeStr("waitForEntities");
						m_defaultInfo[i].waitForEntities = text3 == null || !("false" == text3);
						if (element.Attribute("players") != null && element.Attribute("players").Value == "1,2")
						{
							m_defaultInfo[i].players[0] = 1;
							m_defaultInfo[i].players[1] = 2;
							m_defaultInfo[i].players[2] = -1;
						}
						element.QueryFloatAttribute("globalDtInc", ref m_globalDtInc[i]);
						element.QueryFloatAttribute("globalDtStart", ref m_globalDtStart[i]);
						element.QueryFloatAttribute("globalDtMax", ref m_globalDtMax[i]);
						break;
					}
					case "coin_chances":
						ParseCoinChanceinator(m_coinChanceLists[i], element);
						break;
					case "OverideProbability":
					{
						PROBABILITY_OVERIDE pROBABILITY_OVERIDE = new PROBABILITY_OVERIDE();
						pROBABILITY_OVERIDE.Parse(element);
						m_probabilityOverides[m_defaultInfo[i].players[0], i].Add(pROBABILITY_OVERIDE);
						for (int j = 1; j < Game.MAX_PLAYERS && m_defaultInfo[i].players[j] >= 0; j++)
						{
							m_probabilityOverides[m_defaultInfo[i].players[j], i].Add(pROBABILITY_OVERIDE);
						}
						break;
					}
					}
					element = element.NextSiblingElement();
				}
				for (int l = 0; l < Game.MAX_PLAYERS; l++)
				{
					foreach (WAVE_INFO item2 in m_lists[l, i])
					{
						if (item2.waveNoRange != -1)
						{
							continue;
						}
						int num5 = 1000000;
						item2.waveNoRange = -2;
						foreach (WAVE_INFO item3 in m_lists[l, i])
						{
							if (item3.waveNo > item2.waveNo && item3.waveNo - 1 < num5)
							{
								num5 = item3.waveNo - 1;
							}
						}
						if (num5 < 1000000)
						{
							item2.waveNoRange = num5;
						}
					}
				}
			}
			xDocument = null;
		}
		ParseGlobalProbabilityOverides("xml/globalprobabilities.xml");
		for (int m = 0; m < Game.MAX_PLAYERS; m++)
		{
			m_speedControl[m] = null;
		}
	}

	public WAVE_INFO GetCurrentWave()
	{
		return GetCurrentWave(0);
	}

	public WAVE_INFO GetCurrentWave(int player)
	{
		return m_currentWave[player];
	}

	public int GetNumberOfWavesSpawned()
	{
		return GetNumberOfWavesSpawned(0);
	}

	public int GetNumberOfWavesSpawned(int player)
	{
		return m_numberOfWavesSpawned[player];
	}

	private void ParseGlobalProbabilityOverides(string file)
	{
		XDocument xDocument = MortarXml.Load(file);
		if (xDocument == null)
		{
			return;
		}
		string ename = "probabilityFile" + Game.game_work.saveData.GetTotal(StringFunctions.StringHash("super_fruit_probability_system"));
		XElement element = xDocument.FirstChildElement("probabilityFile");
		element = element.FirstChildElement(ename);
		if (element != null)
		{
			for (XElement xElement = element.FirstChildElement("globalProbability"); xElement != null; xElement = xElement.NextSiblingElement("globalProbability"))
			{
				GlobalProbabilityOveride globalProbabilityOveride = new GlobalProbabilityOveride();
				globalProbabilityOveride.Parse(xElement);
				m_globalProbabilityOverides.Add(globalProbabilityOveride);
			}
			for (XElement xElement2 = element.FirstChildElement("globalProbabilityPointBased"); xElement2 != null; xElement2 = xElement2.NextSiblingElement("globalProbabilityPointBased"))
			{
				GlobalProbabilityOveridePointBased globalProbabilityOveridePointBased = new GlobalProbabilityOveridePointBased();
				globalProbabilityOveridePointBased.Parse(xElement2);
				m_globalProbabilityOverides.Add(globalProbabilityOveridePointBased);
			}
			for (XElement xElement3 = element.FirstChildElement("globalProbabilityTimed"); xElement3 != null; xElement3 = xElement3.NextSiblingElement("globalProbabilityTimed"))
			{
				GlobalProbabilityOverideTimed globalProbabilityOverideTimed = new GlobalProbabilityOverideTimed();
				globalProbabilityOverideTimed.Parse(xElement3);
				m_globalProbabilityOverides.Add(globalProbabilityOverideTimed);
			}
		}
	}

	private GlobalProbabilityOveride CheckForGlobalProbabilityOveride(ref int type)
	{
		for (int i = 0; i < m_globalProbabilityOverides.Count; i++)
		{
			if (m_globalProbabilityOverides[i].CheckForOverride(ref type))
			{
				return m_globalProbabilityOverides[i];
			}
		}
		return null;
	}

	public void Reset()
	{
		Reset(newGame: false);
	}

	public void Reset(bool newGame)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Game.game_work.gameOver = false;
		m_blitzSpawnedThisGame = 0;
		m_blitzForceSpawnedCounter = 0;
		m_blitzSpawnTime = 10f + Mortar.Math.g_random.RandF(10f);
		Game.game_work.currentMissCount = 0;
		Game.game_work.currentScore = 0;
		Game.game_work.hasDroppedFruit = false;
		Fruit.s_consecutiveCount = 0;
		for (num = 0; num < Game.MAX_PLAYERS; num++)
		{
			m_waveCount[num] = -1;
			m_waveWait[num] = 0f;
			m_speed[num] = 0f;
			m_desiredSpeed[num] = 0f;
			m_speedLossTime[num] = 0f;
			m_nextComboBonus[num] = 0f;
			m_comboBonus[num] = 0;
			m_numberOfWavesSpawned[num] = 0;
		}
		float fOVy = Game.game_work.camera.GetFOVy();
		Vector3 zero = Vector3.Zero;
		float num2 = Mortar.Math.TanIdx(Mortar.Math.DEGREE_TO_IDX(fOVy));
		zero.Z = Game.SCREEN_HEIGHT / 2f / num2;
		Game.game_work.camera.SetLookAt(Vector3.Zero);
		Game.game_work.camera.SetPos(zero);
		Game.game_work.camera.SetUp(new Vector3(0f, 1f, 0f));
		if (Game.game_work.hud != null)
		{
			Game.game_work.hud.ResetControls();
		}
		uint num3 = 0u;
		Fruit.ClearUnspawned();
		Bomb.ClearUnspawned();
		for (Entity entity = ActorManager.GetInstance().GetEntity(EntityTypes.ENTITY_BEGIN, num3++); entity != null; entity = ActorManager.GetInstance().GetEntity(EntityTypes.ENTITY_BEGIN, num3++))
		{
			((Fruit)entity).Disable();
		}
		num3 = 0u;
		for (Entity entity = ActorManager.GetInstance().GetEntity(EntityTypes.ENTITY_BOMB, num3++); entity != null; entity = ActorManager.GetInstance().GetEntity(EntityTypes.ENTITY_BOMB, num3++))
		{
			((Bomb)entity).Disable();
		}
		ResetWaveChances();
		for (num = 0; num < Game.MAX_PLAYERS; num++)
		{
			foreach (PROBABILITY_OVERIDE item in m_probabilityOverides[num, (int)Game.game_work.gameMode])
			{
				item.SelectType();
			}
			m_fruitTypesToPickFromWave[num] = 0;
			m_numFruitTypesToPickFrom[num] = 1;
			for (int i = 0; i < 32; i++)
			{
				m_fruitTypesToPickFrom[num, i] = -1;
			}
			if (m_lists[num, (int)Game.game_work.gameMode].Count > 0)
			{
				GetNextWave(num);
				if (Game.IsMultiplayer())
				{
					m_waveDelay[num] += 1f;
				}
			}
		}
		m_currentGlobalDtMod = 1f;
		m_currentGlobalDt = m_globalDtStart[(int)Game.game_work.gameMode];
		for (int j = 0; j < m_globalProbabilityOverides.Count; j++)
		{
			m_globalProbabilityOverides[j].NewGameStarted();
		}
		if (newGame)
		{
			NewGame();
		}
	}

	public void Resume()
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		Game.game_work.currentScore = (ushort)Game.game_work.saveData.score;
		Game.game_work.currentMissCount = (byte)Game.game_work.saveData.misses;
		Fruit.s_consecutiveType = Game.game_work.saveData.consecutiveType;
		Fruit.s_consecutiveCount = Game.game_work.saveData.consecutiveCount;
		Game.game_work.hasDroppedFruit = Game.game_work.saveData.hasDropped;
		bool flag = false;
		for (int i = 0; i < Game.MAX_PLAYERS; i++)
		{
			foreach (PROBABILITY_OVERIDE item in m_probabilityOverides[i, (int)Game.game_work.gameMode])
			{
				item.SelectType();
			}
			m_fruitTypesToPickFromWave[i] = 0;
			m_numFruitTypesToPickFrom[i] = 1;
			for (int j = 0; j < 32; j++)
			{
				m_fruitTypesToPickFrom[i, j] = -1;
			}
		}
		if (Game.game_work.saveData.entities.Count > 0)
		{
			foreach (EntityState entity2 in Game.game_work.saveData.entities)
			{
				EntityState entityState = entity2;
				Entity entity = ActorManager.GetInstance().Add((entityState.type >= Fruit.MAX_FRUIT_TYPES) ? EntityTypes.ENTITY_BOMB : ((entityState.type < 0) ? EntityTypes.ENTITY_BOMB_BLAST : EntityTypes.ENTITY_BEGIN));
				Vector3 one = Vector3.One;
				entity.Init(null, entityState.type, one);
				entity.m_pos = entityState.pos;
				entity.m_vel = entityState.vel;
				if (entity.m_type == 1)
				{
					((Bomb)entity).m_gravity = entityState.grav;
					if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
					{
						((Bomb)entity).SetForPlayer(Game.MAX_PLAYERS * 2 - 1);
					}
				}
				else if (entity.m_type == 0)
				{
					((Fruit)entity).m_gravity = entityState.grav;
					if (entityState.superState != null)
					{
						SuperFruitControl control = new SuperFruitControl((Fruit)entity, entityState.superState);
						Game.game_work.hud.AddControl(control);
						entityState.superState = null;
					}
				}
				if (!(entityState.wait > 0f))
				{
					continue;
				}
				if (entity.m_type == 0)
				{
					((Fruit)entity).Chuck(entityState.wait);
				}
				else if (entity.m_type == 1)
				{
					if (entityState.hit)
					{
						((Bomb)entity).SetHit(entityState.wait);
					}
					else
					{
						((Bomb)entity).Chuck(entityState.wait);
					}
				}
				else if (entity.m_type == 4)
				{
					((BombBlast)entity).Update(entity2.wait);
				}
			}
			flag = true;
		}
		ActorManager.GetInstance().Update(0f);
		int misses = Game.game_work.saveData.misses;
		if ((Game.game_work.saveData.go_bombHitTime > 0f && Game.game_work.gameMode != Game.GAME_MODE.GM_ARCADE) || Game.game_work.saveData.go_state > -1)
		{
			int winner = -1;
			GameTask.SkipToGameOver(Game.game_work.saveData.go_state, Game.game_work.saveData.go_time, Game.game_work.saveData.go_transition, Game.game_work.saveData.go_bombHitTime, winner);
		}
		else if ((flag || Game.game_work.saveData.waves.Count > 0) && misses < 3)
		{
			GameTask.SkipToPause();
			int num = 0;
			m_numFruitTypesToPickFrom[num] = Game.game_work.saveData.numFruitTypesToPickFrom;
			for (int k = 0; k < 32; k++)
			{
				m_fruitTypesToPickFrom[num, k] = Game.game_work.saveData.fruitTypesToPickFrom[k];
			}
			processingWave[num] = true;
			m_waveCount[num] = Game.game_work.saveData.currentWave;
			m_numberOfWavesSpawned[num] = Game.game_work.saveData.numberOfWavesSpawned;
			m_waveDelay[num] = Game.game_work.saveData.currentWaveDelay;
			m_waveWait[num] = Game.game_work.saveData.currentWaveWait;
			m_currentGlobalDt = Game.game_work.saveData.globalWaveDt;
			m_blitzSpawnedThisGame = (byte)Game.game_work.saveData.m_blitzSpawnedThisGame;
			m_blitzForceSpawnedCounter = (byte)Game.game_work.saveData.m_blitzForceSpawnedCounter;
			m_blitzSpawnTime = Game.game_work.saveData.m_blitzSpawnTime;
			m_speedLossTime[num] = Game.game_work.saveData.speedLossTime;
			m_nextComboBonus[num] = Game.game_work.saveData.nextComboBonus;
			m_speed[num] = (m_desiredSpeed[num] = Game.game_work.saveData.desiredSpeed);
			foreach (WaveState wave in Game.game_work.saveData.waves)
			{
				WAVE_INFO wAVE_INFO = m_lists[num, (int)Game.game_work.gameMode][wave.index];
				wAVE_INFO.m_inc = wave.inc;
				if (wave.spawners.Count() <= 0)
				{
					continue;
				}
				m_currentWave[num] = wAVE_INFO;
				int num2 = 0;
				foreach (SpawnState spawner in wave.spawners)
				{
					m_currentWave[num].spawners[num2].toSpawnThisWave = spawner.toSpawnThisWave;
					m_currentWave[num].spawners[num2].maxToSpawnThisWave = spawner.toSpawnThisWave;
					m_currentWave[num].spawners[num2].bombsSpawnedThisWave = 0;
					m_currentWave[num].spawners[num2].delayWait = spawner.delayWait;
					m_currentWave[num].spawners[num2].SelectTypes();
					num2++;
				}
			}
			ResetWaveChances();
		}
		Game.game_work.camera.m_cameraShakeMaxTime = Game.game_work.saveData.shake_max_time;
		Game.game_work.camera.m_cameraShakeTime = Game.game_work.saveData.shake_time;
		Game.game_work.saveData.entities.Clear();
	}

	private void DeleteSpeedControl(HUDControl control)
	{
		for (int i = 0; i < Game.MAX_PLAYERS; i++)
		{
			if (m_speedControl[i] == control)
			{
				m_speedControl[i] = null;
			}
		}
	}

	private SPAWNER_INFO GetRandomPowerSpawner()
	{
		return GetRandomPowerSpawner(allowTop: true);
	}

	public Mortar.Math.Random GetRandomiser()
	{
		return m_random;
	}

	private SPAWNER_INFO GetRandomPowerSpawner(bool allowTop)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!made_spawners)
		{
			spinfos[0].placement = SPAWN_PLACEMENTS.SPAWNER_TOP;
			spinfos[0].gravity = new Vector3(0f, -1.1f, 0f);
			spinfos[0].horizontalMin = -0.5f;
			spinfos[0].horizontalMax = 0.5f;
			spinfos[0].velYscale = 0f;
			spinfos[0].velXscale = 0.666f;
			spinfos[0].delayWait = -3f;
			spinfos[0].dt = 0.75f;
			spinfos[1].placement = SPAWN_PLACEMENTS.SPAWNER_RIGHT;
			spinfos[1].horizontalMin = -1f;
			spinfos[1].horizontalMax = -0.5f;
			spinfos[1].velYscale = 0.75f;
			spinfos[1].delayWait = -3f;
			spinfos[2].placement = SPAWN_PLACEMENTS.SPAWNER_LEFT;
			spinfos[2].horizontalMin = -1f;
			spinfos[2].horizontalMax = -0.5f;
			spinfos[2].velYscale = 0.75f;
			spinfos[2].delayWait = -3f;
			made_spawners = true;
		}
		return spinfos[GetInstance().GetRandomiser().Rand32(allowTop ? 3 : 2) + ((!allowTop) ? 1 : 0)];
	}

	public void Update(float dt)
	{
		still_spawning = false;
		if (paused)
		{
			return;
		}
		m_bombScale = 1f;
		m_bombMultiplyer = 1f;
		m_fruitMultiplyer = 1f;
		m_criticalChanceMod = 1f;
		if (Game.game_work.gameOverTransition < 1f && Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			PowerUpManager.GetInstance().Update(dt);
			PowerUpManager.GetInstance().GetCurrentTimedPowerProgress();
			m_currentGlobalDtMod = PowerUpManager.GetInstance().GetDtMod();
		}
		else
		{
			PowerUpManager.GetInstance().SetDefaults();
			m_currentGlobalDtMod = 1f;
		}
		if (Game.game_work.gameOver && m_waveCount[0] > 0)
		{
			UpdateComboSpeed(dt);
			return;
		}
		m_currentGlobalDt = Mortar.Math.CLAMP(m_currentGlobalDt + m_globalDtInc[(int)Game.game_work.gameMode] * dt, m_globalDtStart[(int)Game.game_work.gameMode], m_globalDtMax[(int)Game.game_work.gameMode]);
		int num = 0;
		for (num = 0; num < Game.MAX_PLAYERS; num++)
		{
			UpdateComboSpeed(dt);
			still_spawning = false;
			if (m_lists[num, (int)Game.game_work.gameMode].Count() <= 0)
			{
				continue;
			}
			if (m_currentWave[num] != null)
			{
				if (m_waveDelay[num] <= 0f)
				{
					m_waveDelay[num] = 0f;
					for (int i = 0; i < m_currentWave[num].spawnerCount; i++)
					{
						m_currentWave[num].spawners[i].delayWait -= dt * ((m_currentGlobalDtMod > 1f) ? m_currentGlobalDtMod : 1f);
						int toSpawnThisWave = m_currentWave[num].spawners[i].toSpawnThisWave;
						int num2 = 0;
						List<PROBABILITY_OVERIDE> list = m_probabilityOverides[num, (int)Game.game_work.gameMode];
						while (m_currentWave[num].spawners[i].toSpawnThisWave > 0)
						{
							SPAWNER_INFO sPAWNER_INFO = m_currentWave[num].spawners[i];
							if (m_currentWave[num].spawners[i].typeCount > 0)
							{
								int num3 = Mortar.Math.g_random.Rand32(m_currentWave[num].spawners[i].typeCount);
								int type = m_currentWave[num].spawners[i].randomTypes[num3];
								if (m_currentWave[num].spawners[i].typeCount > 1 && num2 >= toSpawnThisWave / 2)
								{
									while (type == -2)
									{
										num3 = Mortar.Math.g_random.Rand32(m_currentWave[num].spawners[i].typeCount);
										type = m_currentWave[num].spawners[i].randomTypes[num3];
									}
								}
								bool flag = false;
								if (type == -1 || m_currentWave[num].spawners[i].types[num3] == "1fruit")
								{
									bool flag2 = false;
									if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && Game.game_work.timeControl.GetTime() <= Game.game_work.timeControl.GetCountDown() - m_blitzSpawnTime)
									{
										if (m_blitzForceSpawnedCounter == 0)
										{
											m_blitzForceSpawnedCounter++;
											if (m_blitzSpawnedThisGame == 0)
											{
												flag2 = true;
											}
											m_blitzSpawnTime = 35f + Mortar.Math.g_random.RandF(10f);
										}
										else if (m_blitzForceSpawnedCounter == 1 && m_blitzSpawnedThisGame == 1)
										{
											m_blitzForceSpawnedCounter++;
											flag2 = true;
										}
									}
									num3 = Mortar.Math.g_random.Rand32(m_currentWave[num].overideProbabilty);
									int num4 = 0;
									int j = 0;
									int blitzSpawnedThisGame = m_blitzSpawnedThisGame;
									for (; j < list.Count; j++)
									{
										bool flag3 = true;
										if (list[j].percentageChance <= 0 || !flag2)
										{
											if (list[j].spawnedThisWave >= list[j].perWave && list[j].perWave > 0)
											{
												flag3 = false;
											}
											float activeProgression = PowerUpManager.GetInstance().GetActiveProgression(Mortar.Math.MAX(0f, sPAWNER_INFO.delayWait) + 0.21f);
											flag3 = flag3 && list[j].canSpawnWithPowerup < activeProgression;
											if (m_waveCount[num] >= 0 && m_waveCount[num] < list[j].waveCount)
											{
												flag3 = false;
											}
										}
										if (!flag3)
										{
											continue;
										}
										int num5 = list[j].percentageChance;
										if (m_blitzSpawnedThisGame >= 6)
										{
											num5 >>= 1;
										}
										num4 += num5;
										if (num3 >= num4 && (list[j].percentageChance <= 0 || !flag2))
										{
											continue;
										}
										int count = list[j].powerAllowanceChances.Count;
										if (count > 0)
										{
											int index = Mortar.Math.MIN(count - 1, PowerUpManager.GetInstance().GetNumActiveTimedPowers());
											int num6 = m_random.Rand32(100);
											if (num6 >= list[j].powerAllowanceChances[index])
											{
												break;
											}
										}
										int type2 = list[j].GetType();
										FRUIT_POWERS fRUIT_POWERS = (Mortar.Math.BETWEEN(type2, 0, Fruit.MAX_FRUIT_TYPES) ? Fruit.FruitInfo(type2).powers : null);
										int num7 = 30;
										while (num7 > 0 && fRUIT_POWERS != null && fRUIT_POWERS.AnyActivePowers())
										{
											type2 = list[j].GetType();
											fRUIT_POWERS = (Mortar.Math.BETWEEN(type2, 0, Fruit.MAX_FRUIT_TYPES) ? Fruit.FruitInfo(type2).powers : null);
											num7--;
										}
										if (Mortar.Math.BETWEEN(type2, 0, Fruit.MAX_FRUIT_TYPES) && Fruit.FruitInfo(type2).superFruit)
										{
											sPAWNER_INFO = GetRandomPowerSpawner(allowTop: false);
										}
										if (fRUIT_POWERS != null)
										{
											if (fRUIT_POWERS.AnyActivePowers())
											{
												break;
											}
											sPAWNER_INFO = GetRandomPowerSpawner();
											if (!(m_fruitMultiplyer <= 0f) && Fruit.NumberOfPowerupFruits() <= 0 && (!(Game.game_work.saveData.timer < 8f) || fRUIT_POWERS.powerUps[0].powerHash == StringFunctions.StringHash("freeze")) && !fRUIT_POWERS.AnyActivePowers())
											{
												m_blitzSpawnedThisGame++;
												flag2 = false;
											}
										}
										list[j].spawnedThisWave++;
										num3 = 0;
										type = type2;
										flag = true;
										if (m_blitzSpawnedThisGame >= 6)
										{
											_ = 6;
										}
										blitzSpawnedThisGame = m_blitzSpawnedThisGame;
										break;
									}
								}
								if (type == -2)
								{
									num2++;
									if (m_bombMultiplyer > 0f)
									{
										SpawnBomb(1, sPAWNER_INFO, num);
									}
								}
								else if (m_fruitMultiplyer > 0f)
								{
									GlobalProbabilityOveride globalProbabilityOveride = null;
									if (!flag)
									{
										globalProbabilityOveride = CheckForGlobalProbabilityOveride(ref type);
									}
									Fruit fruit = SpawnFruit(1, type, sPAWNER_INFO, num);
									if (fruit != null && globalProbabilityOveride != null)
									{
										fruit.m_fruitKilled = (Fruit.FruitEvent)Delegate.Combine(fruit.m_fruitKilled, new Fruit.FruitEvent(globalProbabilityOveride.FruitWasKilled));
										fruit.m_fruitThrown = (Fruit.FruitEvent)Delegate.Combine(fruit.m_fruitThrown, new Fruit.FruitEvent(globalProbabilityOveride.FruitWasThrown));
									}
								}
								processingWave[num] = true;
								m_currentWave[num].spawners[i].toSpawnThisWave--;
							}
							else
							{
								m_currentWave[num].spawners[i].delayWait = 0f;
								m_currentWave[num].spawners[i].toSpawnThisWave = 0;
							}
							m_currentWave[num].spawners[i].delayWait += Mortar.Math.MAX(0f, m_currentWave[num].spawners[i].delay + m_currentWave[num].spawners[i].delayInc * m_currentWave[num].m_inc);
						}
						if (m_currentWave[num].spawners[i].toSpawnThisWave > 0)
						{
							still_spawning = true;
						}
					}
				}
				else
				{
					m_waveDelay[num] -= dt;
					still_spawning = true;
				}
			}
			if (IsWaveProcessing(num) || still_spawning)
			{
				continue;
			}
			bool flag4 = true;
			if (m_currentWave[num] != null && m_waveWait[num] > 0f)
			{
				m_waveWait[num] -= dt;
				if (m_waveWait[num] > 0f)
				{
					flag4 = false;
				}
			}
			if (flag4)
			{
				GetNextWave(num);
			}
		}
	}

	public void Draw()
	{
		Draw(0);
	}

	public void Draw(int player)
	{
		if (player == 0)
		{
			PowerUpManager.GetInstance().Draw();
		}
	}

	public void Destroy()
	{
		for (int i = 0; i < Game.MAX_PLAYERS; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				m_lists[i, j].Clear();
			}
		}
	}

	public void ClearUnspawned()
	{
		Fruit.ClearUnspawned();
		Bomb.ClearUnspawned();
	}

	private void UpdateComboSpeed(float dt)
	{
		int num = 0;
		float num2 = m_desiredSpeed[num];
		if (num2 < SPEED_START)
		{
			num2 = 0f;
		}
		m_speed[num] += ((num2 > m_speed[num]) ? Mortar.Math.MIN(num2 - m_speed[num], dt * 5f) : ((num2 < m_speed[num]) ? Mortar.Math.MAX(num2 - m_speed[num], (0f - dt) * 5f) : 0f));
		if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			if (m_speedControl[num] != null)
			{
				m_speedControl[num].m_pulseSpeed = m_speed[num];
				m_speedControl[num].m_lossTime = m_speedLossTime[num];
			}
			else
			{
				m_speedControl[num] = new SpeedControl();
				m_speedControl[num].m_deleteCall = DeleteSpeedControl;
				Game.game_work.hud.AddControl(m_speedControl[num]);
				m_speedControl[num].m_pulseSpeed = m_speed[num];
				m_speedControl[num].m_lossTime = m_speedLossTime[num];
			}
		}
		if (m_speedLossTime[num] > 0f && m_currentWave[num] != null && m_currentWave[num].speedLoss > 0f)
		{
			m_speedLossTime[num] -= Mortar.Math.MIN(1f, GetWavedt(num)) * dt / m_currentWave[num].speedLoss;
			if (m_speedLossTime[num] <= 0f)
			{
				ResetSpeed(num);
			}
		}
	}

	public bool IsWaveProcessing()
	{
		return IsWaveProcessing(0);
	}

	public bool IsWaveProcessing(int player)
	{
		if (processingWave[player])
		{
			if (player == 0)
			{
				if (m_currentWave[player] != null && !m_currentWave[player].waitForEntities)
				{
					if (Fruit.GetNumActiveForPlayer(-1, waitForDeath: false) > 0 || Bomb.GetNumActiveForPlayer(-1, waitForDeath: false) > 0)
					{
						return true;
					}
				}
				else if (ActorManager.GetInstance().GetNumEntities(0) != 0 || (Game.IsMultiplayer() && Bomb.GetNumActiveForPlayer(-1) > 0) || (!Game.IsMultiplayer() && ActorManager.GetInstance().GetNumEntities(1) != 0))
				{
					return true;
				}
			}
			else if (Fruit.GetNumActiveForPlayer(player) > 0 || Bomb.GetNumActiveForPlayer(player) > 0)
			{
				return true;
			}
			processingWave[player] = false;
		}
		return processingWave[player];
	}

	public float GetWavedt()
	{
		return GetWavedt(0);
	}

	public float GetWavedt(int player)
	{
		float num = 1f;
		if (m_currentWave[player] != null)
		{
			num = m_currentWave[player].deltaT + m_currentWave[player].m_inc * m_currentWave[player].deltaTinc + m_currentWave[player].deltaSpinc * m_speed[player];
		}
		return Mortar.Math.CLAMP(num * ((player != 0) ? 1f : (m_currentGlobalDt * m_currentGlobalDtMod)), 0f, MAX_DELTA_TIME_SCALE);
	}

	public bool SaveWaveInfo(FruitSaveData saveData)
	{
		int num = 0;
		saveData.speedLossTime = 0f;
		saveData.nextComboBonus = 0f;
		saveData.desiredSpeed = 0f;
		saveData.m_blitzSpawnedThisGame = m_blitzSpawnedThisGame;
		saveData.m_blitzForceSpawnedCounter = m_blitzForceSpawnedCounter;
		saveData.m_blitzSpawnTime = m_blitzSpawnTime;
		saveData.waves.Clear();
		if ((Game.game_work.gameOver && m_waveCount[num] >= 0) || m_lists[num, (int)Game.game_work.gameMode].Count <= 0)
		{
			return false;
		}
		saveData.globalWaveDt = m_currentGlobalDt;
		WAVE_INFO[] array = ArrayInit.CreateFilledArray<WAVE_INFO>(20);
		int[] array2 = new int[20];
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (WAVE_INFO item in m_lists[num, (int)Game.game_work.gameMode])
		{
			if (m_waveCount[num] >= item.waveNo && (m_waveCount[num] <= item.waveNoRange || item.waveNoRange == -2))
			{
				array[num3] = item;
				array2[num3] = num2;
				num4 += item.chance;
				num3++;
			}
			num2++;
		}
		WaveState value = default(WaveState);
		SpawnState value2 = default(SpawnState);
		for (int i = 0; i < num3; i++)
		{
			value.inc = array[i].m_inc;
			value.index = array2[i];
			value.spawners = new LinkedList<SpawnState>();
			value.spawners.Clear();
			if (array[i] == m_currentWave[num])
			{
				for (int j = 0; j < array[i].spawnerCount; j++)
				{
					value2.toSpawnThisWave = array[i].spawners[j].toSpawnThisWave;
					value2.delayWait = array[i].spawners[j].delayWait;
					value.spawners.AddLast(value2);
				}
			}
			saveData.waves.AddLast(value);
		}
		saveData.currentWave = m_waveCount[num];
		saveData.currentWaveDelay = m_waveDelay[num];
		saveData.currentWaveWait = m_waveWait[num];
		saveData.speedLossTime = m_speedLossTime[num];
		saveData.nextComboBonus = m_nextComboBonus[num];
		saveData.desiredSpeed = m_desiredSpeed[num];
		saveData.numberOfWavesSpawned = m_numberOfWavesSpawned[num];
		saveData.numFruitTypesToPickFrom = m_numFruitTypesToPickFrom[num];
		for (int k = 0; k < 32; k++)
		{
			saveData.fruitTypesToPickFrom[k] = m_fruitTypesToPickFrom[num, k];
		}
		return true;
	}

	public float GetCriticalChance()
	{
		return GetCriticalChance(0);
	}

	public bool CriticalMode()
	{
		return CriticalMode(0);
	}

	public bool CriticalMode(int player)
	{
		return GetCriticalChance(player) > (float)(Fruit.CRITICAL_CHANCE / 2);
	}

	public int RequestCoins()
	{
		int num = 0;
		int num2 = 0;
		if (m_currentWave != null && m_currentWave[num].coinChanceinator != null)
		{
			num2 = m_currentWave[num].coinChanceinator.GetCoins();
		}
		if (num2 <= 0)
		{
			num2 = m_coinChanceLists[(int)Game.game_work.gameMode].GetCoins();
		}
		return num2;
	}

	public Fruit SpawnFruit(int fruitCount)
	{
		return SpawnFruit(fruitCount, -1);
	}

	public Fruit SpawnFruit(int fruitCount, int type)
	{
		return SpawnFruit(fruitCount, type, null);
	}

	public Fruit SpawnFruit(int fruitCount, int type, SPAWNER_INFO spawner)
	{
		return SpawnFruit(fruitCount, type, spawner, 0);
	}

	public Fruit SpawnFruit(int fruitCount, int type, SPAWNER_INFO spawner, int player)
	{
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < fruitCount; i++)
		{
			float num = spawner?.horizontalMin ?? (-1f);
			float num2 = spawner?.horizontalMax ?? 1f;
			int num3 = (int)(FRUIT_SPAWN_MAX_X * num + (float)Mortar.Math.g_random.Rand32((uint)(num2 * FRUIT_SPAWN_MAX_X - num * FRUIT_SPAWN_MAX_X)));
			int num4 = (int)FRUIT_SPAWN_Y;
			float num5 = FRUIT_ANGLE_RANGE;
			if (spawner != null && spawner.placement >= SPAWN_PLACEMENTS.SPAWNER_LEFT)
			{
				num5 *= 0.6f;
			}
			int num6 = (int)((0f - (float)num3 / (FRUIT_SPAWN_RANGE / 2f)) * (num5 / 2f));
			float num7 = Mortar.Math.g_random.RandF(1f) - 0.5f;
			num7 = (0.5f - (0.5f - Mortar.Math.ABS(num7)) * (0.5f - Mortar.Math.ABS(num7)) * 2f) * (float)Mortar.Math.MATH_SIGN(num7);
			int num8 = (int)((float)num6 + num5 * num7);
			ushort idx = Mortar.Math.DEGREE_TO_IDX(num8);
			float num9 = FRUIT_MIN_VEL + Mortar.Math.g_random.RandF(FRUIT_MAX_VEL - FRUIT_MIN_VEL);
			float num10 = Mortar.Math.SinIdx(idx) * num9 * (spawner?.velXscale ?? 1f);
			float num11 = Mortar.Math.CosIdx(idx) * num9 * 1.075f * (spawner?.velYscale ?? 1f);
			int num12 = type;
			float dtMod = 1f;
			if (spawner != null)
			{
				dtMod = spawner.dt;
			}
			if (num12 == -1 && m_numFruitTypesToPickFrom[player] > 0)
			{
				num12 = m_fruitTypesToPickFrom[player, Mortar.Math.g_random.Rand32(m_numFruitTypesToPickFrom[player])];
			}
			Vector3 val = Vector3.One;
			Vector3 val2 = Vector3.UnitY * -1f;
			SPAWN_PLACEMENTS sPAWN_PLACEMENTS = spawner?.placement ?? SPAWN_PLACEMENTS.SPAWNER_BOTTOM;
			switch (sPAWN_PLACEMENTS)
			{
			case SPAWN_PLACEMENTS.SPAWNER_TOP:
				val = Vector3.One * -1f;
				num11 /= 2f;
				val2 = Vector3.UnitY;
				break;
			case SPAWN_PLACEMENTS.SPAWNER_LEFT_RIGHT:
				sPAWN_PLACEMENTS = ((Mortar.Math.g_random.Rand32(2) != 0) ? SPAWN_PLACEMENTS.SPAWNER_RIGHT : SPAWN_PLACEMENTS.SPAWNER_LEFT);
				goto case SPAWN_PLACEMENTS.SPAWNER_RIGHT;
			case SPAWN_PLACEMENTS.SPAWNER_RIGHT:
				val = new Vector3(1f, 1f, 1f);
				val2 = Vector3.UnitX;
				goto case SPAWN_PLACEMENTS.SPAWNER_LEFT;
			case SPAWN_PLACEMENTS.SPAWNER_LEFT:
			{
				float num13 = num3;
				num3 = -num4 * (int)Game.SCREEN_WIDTH / (int)Game.SCREEN_HEIGHT;
				num4 = (int)(num13 * Game.SCREEN_HEIGHT / Game.SCREEN_WIDTH);
				num13 = num10;
				num10 = -0.75f * num11;
				num11 = num13 - num9 * spawner.gravity.Y * 0.65f;
				if (sPAWN_PLACEMENTS == SPAWN_PLACEMENTS.SPAWNER_LEFT)
				{
					val = new Vector3(-1f, 1f, 1f);
					val2 = Vector3.UnitX * -1f;
				}
				break;
			}
			}
			num3 *= (int)val.X;
			num4 *= (int)val.Y;
			num10 *= val.X;
			num11 *= val.Y;
			float v = spawner?.delayWait ?? 0f;
			if (Game.IsMultiplayer())
			{
				float num14 = 0.975f * Game.SPLIT_SCREEN_SCALE;
				float x = val2.X;
				val2.X = val2.Y * num14;
				val2.Y = (0f - x) * num14;
				x = num3;
				num3 = num4 * (int)(num14 - Game.SCREEN_WIDTH / 4f);
				num4 = (int)((0f - x) * num14);
				x = num10;
				num10 = num11 * num14;
				num11 = (0f - x) * num14;
				Fruit fruit = null;
				Vector3 value = Vector3.One * num14;
				if (player == 0 || player == 1)
				{
					fruit = (Fruit)ActorManager.GetInstance().Add(EntityTypes.ENTITY_BEGIN);
					fruit.m_pos = new Vector3((float)num3, (float)num4, 0f);
					fruit.m_vel = new Vector3(num10, num11, 0f);
					fruit.Init(null, num12, value);
					Fruit fruit2 = fruit;
					fruit2.m_pos += val2 * fruit.m_cur_scale.Y * 100f;
					fruit.m_gravity.X = fruit.m_gravity.Y * num14;
					fruit.m_gravity.Y = 0f;
					fruit.Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
					fruit.SetForPlayer(1);
					num12 = fruit.GetFruitType();
				}
				if (player == 0 || player == 2)
				{
					fruit = (Fruit)ActorManager.GetInstance().Add(EntityTypes.ENTITY_BEGIN);
					fruit.m_pos = new Vector3((float)(-num3), (float)(-num4), 0f);
					fruit.m_vel = new Vector3(0f - num10, 0f - num11, 0f);
					fruit.Init(null, num12, value);
					Fruit fruit3 = fruit;
					fruit3.m_pos -= val2 * fruit.m_cur_scale.Y * 100f;
					fruit.m_gravity.X = (0f - fruit.m_gravity.Y) * num14;
					fruit.m_gravity.Y = 0f;
					fruit.Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
					fruit.SetForPlayer(2);
				}
				continue;
			}
			Fruit fruit4 = (Fruit)ActorManager.GetInstance().Add(EntityTypes.ENTITY_BEGIN);
			Vector3 one = Vector3.One;
			fruit4.m_pos = new Vector3((float)num3, (float)num4, 0f);
			fruit4.m_vel = new Vector3(num10, num11, 0f);
			fruit4.Init(null, num12, one);
			fruit4.m_dtMod = dtMod;
			if (spawner != null)
			{
				fruit4.m_gravity = spawner.gravity * (0f - fruit4.m_gravity.Y);
				switch (sPAWN_PLACEMENTS)
				{
				case SPAWN_PLACEMENTS.SPAWNER_LEFT:
				{
					ref Vector3 gravity2 = ref fruit4.m_gravity;
					gravity2.X += 0.01f;
					break;
				}
				case SPAWN_PLACEMENTS.SPAWNER_RIGHT:
				{
					ref Vector3 gravity = ref fruit4.m_gravity;
					gravity.X -= 0.01f;
					break;
				}
				}
			}
			Fruit fruit5 = fruit4;
			fruit5.m_pos += val2 * fruit4.m_cur_scale.Y * 100f;
			fruit4.Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
			if (spawner != null && spawner.mirror)
			{
				Vector3 pos = fruit4.m_pos;
				Vector3 gravity3 = fruit4.m_gravity;
				Vector3 vel = fruit4.m_vel;
				num12 = fruit4.GetFruitType();
				fruit4 = (Fruit)ActorManager.GetInstance().Add(EntityTypes.ENTITY_BEGIN);
				fruit4.Init(null, num12, one);
				fruit4.Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
				fruit4.m_pos = -pos;
				fruit4.m_vel = -vel;
				fruit4.m_gravity = -gravity3;
			}
			return fruit4;
		}
		return null;
	}

	public void SpawnBomb(int bombCount)
	{
		SpawnBomb(bombCount, null, 0);
	}

	public void SpawnBomb(int bombCount, SPAWNER_INFO spawner, int player)
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < bombCount; i++)
		{
			float num = spawner?.horizontalMin ?? (-1f);
			float num2 = spawner?.horizontalMax ?? 1f;
			int num3 = (int)(FRUIT_SPAWN_MAX_X * num + (float)Mortar.Math.g_random.Rand32((uint)(num2 * FRUIT_SPAWN_MAX_X - num * FRUIT_SPAWN_MAX_X)));
			int num4 = (int)FRUIT_SPAWN_Y;
			float num5 = FRUIT_ANGLE_RANGE;
			if (spawner != null && spawner.placement >= SPAWN_PLACEMENTS.SPAWNER_LEFT)
			{
				num5 *= 0.6f;
			}
			int num6 = (int)((0f - (float)num3 / FRUIT_SPAWN_RANGE) * (num5 / 2f));
			int num7 = num6 + (int)(num5 / 2f);
			num6 -= (int)(num5 / 2f);
			int num8 = num6 + Mortar.Math.g_random.Rand32(num7 - num6);
			ushort idx = Mortar.Math.DEGREE_TO_IDX(num8);
			float num9 = FRUIT_MIN_VEL + Mortar.Math.g_random.RandF(FRUIT_MAX_VEL - FRUIT_MIN_VEL);
			float num10 = Mortar.Math.SinIdx(idx) * num9 * (spawner?.velXscale ?? 1f);
			float num11 = Mortar.Math.CosIdx(idx) * num9 * 1.075f * (spawner?.velYscale ?? 1f);
			float v = spawner?.delayWait ?? 0f;
			Vector3 val = Vector3.One;
			Vector3 val2 = Vector3.UnitY * -1f;
			SPAWN_PLACEMENTS sPAWN_PLACEMENTS = spawner?.placement ?? SPAWN_PLACEMENTS.SPAWNER_BOTTOM;
			switch (sPAWN_PLACEMENTS)
			{
			case SPAWN_PLACEMENTS.SPAWNER_TOP:
				val = Vector3.One * -1f;
				num11 /= 2f;
				val2 = Vector3.UnitY;
				break;
			case SPAWN_PLACEMENTS.SPAWNER_LEFT_RIGHT:
				sPAWN_PLACEMENTS = ((Mortar.Math.g_random.Rand32(2) != 0) ? SPAWN_PLACEMENTS.SPAWNER_RIGHT : SPAWN_PLACEMENTS.SPAWNER_LEFT);
				goto case SPAWN_PLACEMENTS.SPAWNER_RIGHT;
			case SPAWN_PLACEMENTS.SPAWNER_RIGHT:
				val = new Vector3(1f, 1f, 1f);
				val2 = Vector3.UnitX;
				goto case SPAWN_PLACEMENTS.SPAWNER_LEFT;
			case SPAWN_PLACEMENTS.SPAWNER_LEFT:
			{
				float num12 = num3;
				num3 = -num4 * (int)(Game.SCREEN_WIDTH / Game.SCREEN_HEIGHT);
				num4 = (int)(num12 * Game.SCREEN_HEIGHT / Game.SCREEN_WIDTH);
				num12 = num10;
				num10 = -0.75f * num11;
				num11 = num12 - num9 * spawner.gravity.Y * 0.65f;
				if (sPAWN_PLACEMENTS == SPAWN_PLACEMENTS.SPAWNER_LEFT)
				{
					val = new Vector3(-1f, 1f, 1f);
					val2 = Vector3.UnitX * -1f;
				}
				break;
			}
			}
			num3 *= (int)val.X;
			num4 *= (int)val.Y;
			num10 *= val.X;
			num11 *= val.Y;
			if (Game.IsMultiplayer())
			{
				float num13 = 0.975f * Game.SPLIT_SCREEN_SCALE;
				float x = val2.X;
				val2.X = val2.Y * num13;
				val2.Y = (0f - x) * num13;
				x = num3;
				num3 = num4 * (int)(num13 - Game.SCREEN_WIDTH / 4f);
				num4 = (int)((0f - x) * num13);
				x = num10;
				num10 = num11 * num13;
				num11 = (0f - x) * num13;
				Bomb bomb = null;
				Vector3 value = Vector3.One * num13 * m_bombScale;
				if (player == 1 || player == 0)
				{
					bomb = (Bomb)ActorManager.GetInstance().Add(EntityTypes.ENTITY_BOMB);
					bomb.m_pos = new Vector3((float)num3, (float)num4, (float)(i + 1) * FRUIT_SPAWN_Z_OFFSET);
					bomb.m_vel = new Vector3(num10, num11, 0f);
					bomb.Init(null, 0, value);
					bomb.m_gravity.X = bomb.m_gravity.Y * num13;
					bomb.m_gravity.Y = 0f;
					ref Vector3 pos = ref bomb.m_pos;
					pos.X -= bomb.m_cur_scale.Y * 100f;
					bomb.SetForPlayer(1);
					bomb.Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
				}
				if (player == 2 || player == 0)
				{
					bomb = (Bomb)ActorManager.GetInstance().Add(EntityTypes.ENTITY_BOMB);
					bomb.m_pos = new Vector3((float)(-num3), (float)(-num4), (float)(i + 1) * FRUIT_SPAWN_Z_OFFSET);
					bomb.m_vel = new Vector3(0f - num10, 0f - num11, 0f);
					bomb.Init(null, 0, value);
					bomb.m_gravity.X = (0f - bomb.m_gravity.Y) * num13;
					bomb.m_gravity.Y = 0f;
					ref Vector3 pos2 = ref bomb.m_pos;
					pos2.X += bomb.m_cur_scale.Y * 100f;
					bomb.SetForPlayer(2);
					bomb.Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
				}
				if (spawner == null && bomb != null && player > 0)
				{
					bomb.MakeFat();
				}
			}
			else
			{
				Entity entity = ActorManager.GetInstance().Add(EntityTypes.ENTITY_BOMB);
				Vector3 value2 = Vector3.One * m_bombScale;
				entity.m_pos = new Vector3((float)num3, (float)num4, (float)(i + 1) * FRUIT_SPAWN_Z_OFFSET);
				entity.m_vel = new Vector3(num10, num11, 0f);
				entity.Init(null, 0, value2);
				ref Vector3 pos3 = ref entity.m_pos;
				pos3.Y -= entity.m_cur_scale.Y * 100f;
				((Bomb)entity).Chuck(Mortar.Math.MAX(0f, v) + 0.21f);
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
				{
					((Bomb)entity).SetForPlayer(Game.MAX_PLAYERS * 2 - 1);
				}
			}
		}
	}

	public void NewGame()
	{
		ResetGlobalDt();
		PowerUpManager.GetInstance().Reset(newGame: true);
	}

	private void GameOver()
	{
		ResetGlobalDt();
		PowerUpManager.GetInstance().Reset();
	}

	private void ResetGlobalDt(float to)
	{
		for (int i = 0; i < Game.MAX_PLAYERS; i++)
		{
			List<PROBABILITY_OVERIDE> list = m_probabilityOverides[i, (int)Game.game_work.gameMode];
			int num = 0;
			while (num < list.Count)
			{
				if (list[num].numWaves >= 0)
				{
					list.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
		m_currentGlobalDt = to;
	}

	private float GetCriticalChance(int player)
	{
		return ((m_currentWave[player] != null) ? m_currentWave[player].criticalChance : 1f) * m_criticalChanceMod;
	}

	protected void ResetWaveChances()
	{
		int num = 0;
		int num2 = 0;
		foreach (WAVE_INFO item in m_lists[num, (int)Game.game_work.gameMode])
		{
			item.m_inc = 1f;
			item.currentChance = item.chance;
			item.currentChanceRegrowth = item.chanceRegrowth;
			if (item.gamesMin > 0)
			{
				if (Game.game_work.saveData.waveGameCount[(int)Game.game_work.gameMode].TryGetValue(num2, out var value))
				{
					if (value > 0)
					{
						Game.game_work.saveData.waveGameCount[(int)Game.game_work.gameMode][num2] = Mortar.Math.MIN(value, item.gamesMax);
						item.currentChance = 0;
						item.currentChanceRegrowth = 0f;
					}
				}
				else
				{
					int num3 = (item.gamesMax + item.gamesMin) / 2;
					int value2 = Mortar.Math.g_random.Rand32(Mortar.Math.MAX(1, Mortar.Math.g_random.Rand32(num3 / 2))) + num3 / 4;
					Game.game_work.saveData.waveGameCount[(int)Game.game_work.gameMode][num2] = value2;
					item.currentChance = 0;
					item.currentChanceRegrowth = 0f;
				}
			}
			num2++;
		}
	}

	public void GetNextWave()
	{
		GetNextWave(0);
	}

	protected void GetNextWave(int player)
	{
		AchievementManager achievementManager = AchievementManager.GetInstance();
		Game.game_work.saveData.UnlockTotals();
		achievementManager.UnlockScoreAchievement(Game.game_work.currentScore);
		achievementManager.UnlockTotalFruitAchievement((int)Game.game_work.totalScore);
		m_waveCount[player]++;
		m_numberOfWavesSpawned[player]++;
		if (m_waveCount[player] > 1 && m_currentWave != null)
		{
			m_currentWave[player].m_inc += 1f;
		}
		m_currentWave[player] = m_lists[player, (int)Game.game_work.gameMode][0];
		WAVE_INFO[] array = ArrayInit.CreateFilledArray<WAVE_INFO>(20);
		int num = 0;
		int num2 = 0;
		foreach (WAVE_INFO item in m_lists[player, (int)Game.game_work.gameMode])
		{
			if (item.currentChanceRegrowth > 0f)
			{
				if (item.currentChance < item.chance)
				{
					item.currentChance += Mortar.Math.MAX(1, item.chance * (int)item.currentChanceRegrowth);
					if (item.currentChance > item.chance)
					{
						item.currentChance = item.chance;
					}
				}
				else
				{
					item.currentChance = item.chance;
				}
			}
			if (m_waveCount[player] >= item.waveNo && (m_waveCount[player] <= item.waveNoRange || item.waveNoRange == -2))
			{
				if (num == 0)
				{
					m_currentWave[player] = item;
				}
				array[num] = item;
				num2 += item.currentChance;
				num++;
			}
		}
		if (num > 0 && array[0].typesToPickFromCount > 0 && array[0].waveNo > m_fruitTypesToPickFromWave[player])
		{
			m_fruitTypesToPickFromWave[player] = array[0].waveNo;
			m_numFruitTypesToPickFrom[player] = Mortar.Math.MIN(32, array[0].typesToPickFromCount);
			for (int i = 0; i < m_numFruitTypesToPickFrom[player]; i++)
			{
				int num3 = Fruit.FruitType(array[0].typesToPickFrom[i]);
				if (num3 < 0)
				{
					num3 = Fruit.RandomFruit(canUseCoiners: false);
					bool flag = i < Fruit.MAX_FRUIT_TYPES - 2;
					while (flag)
					{
						flag = false;
						for (int j = 0; j < i; j++)
						{
							if (num3 == m_fruitTypesToPickFrom[player, j])
							{
								flag = true;
								num3 = Fruit.RandomFruit(canUseCoiners: false);
								break;
							}
						}
					}
				}
				m_fruitTypesToPickFrom[player, i] = num3;
			}
		}
		if (num > 1)
		{
			int num4 = Mortar.Math.g_random.Rand32(num2 * 10);
			int num5 = 0;
			for (int k = 0; k < num; k++)
			{
				num5 += array[k].currentChance * 10;
				if (num4 < num5)
				{
					m_currentWave[player] = array[k];
					break;
				}
			}
		}
		if (m_currentWave[player].gamesMin > 0)
		{
			Game.game_work.saveData.waveGameCount[(int)Game.game_work.gameMode][m_currentWave[player].idx] = ((m_currentWave[player].gamesMin == m_currentWave[player].gamesMax) ? m_currentWave[player].gamesMin : (Mortar.Math.g_random.Rand32(m_currentWave[player].gamesMax - m_currentWave[player].gamesMin) + m_currentWave[player].gamesMin));
			m_currentWave[player].currentChanceRegrowth = 0f;
		}
		m_currentWave[player].currentChance = 0;
		if (m_currentWave[player].beforeDelay <= 0f)
		{
			m_waveDelay[player] = 0f;
		}
		else
		{
			m_waveDelay[player] = Mortar.Math.MAX(0.05f, m_currentWave[player].beforeDelay + m_currentWave[player].beforeDelayInc * m_currentWave[player].m_inc);
		}
		m_waveWait[player] = m_currentWave[player].nextDelay;
		if (m_currentWave[player].nextDelaySpInc != 0f)
		{
			m_waveWait[player] = Mortar.Math.MAX(m_waveWait[player] + m_speed[player] * m_currentWave[player].nextDelaySpInc, 0.05f);
		}
		for (int l = 0; l < m_currentWave[player].spawnerCount; l++)
		{
			m_currentWave[player].spawners[l].Reset(m_currentWave[player].m_inc);
		}
		List<PROBABILITY_OVERIDE> list = m_probabilityOverides[player, (int)Game.game_work.gameMode];
		int num6 = 0;
		while (num6 < list.Count)
		{
			if (list[num6].numWaves > 0 && m_waveCount[player] != 0)
			{
				list[num6].numWaves--;
				if (list[num6].numWaves <= 0)
				{
					list.RemoveAt(num6);
					continue;
				}
			}
			list[num6].spawnedThisWave = 0;
			num6++;
		}
	}

	public void SetCurrentWave(int wave, float extraTime, int player)
	{
		ClearUnspawned();
		m_waveCount[player] = wave - 1;
		GetNextWave();
		m_waveDelay[player] = Mortar.Math.MAX(0f, m_waveDelay[player] + extraTime);
	}

	public void ResetGlobalDt()
	{
		ResetGlobalDt(1f);
	}

	public float BombScale()
	{
		return BombScale(1f);
	}

	public float BombScale(float value)
	{
		return m_bombScale *= value;
	}

	public float BombMultiplyer()
	{
		return BombMultiplyer(1f);
	}

	public float FruitMultiplyer()
	{
		return FruitMultiplyer(1f);
	}

	public float CriticalChanceMod()
	{
		return CriticalChanceMod(1f);
	}

	public float BombMultiplyer(float value)
	{
		return m_bombMultiplyer *= value;
	}

	public float FruitMultiplyer(float value)
	{
		return m_fruitMultiplyer *= value;
	}

	public float CriticalChanceMod(float value)
	{
		return m_criticalChanceMod *= value;
	}

	public List<PROBABILITY_OVERIDE> GetCurrentOverideList()
	{
		return GetCurrentOverideList(0);
	}

	public void SetCurrentWave(int wave, float extraTime)
	{
		SetCurrentWave(wave, extraTime, 0);
	}

	public float GetSpeed()
	{
		return GetSpeed(0);
	}

	public void ResetSpeed()
	{
		ResetSpeed(0);
	}

	public void AddSpeed(float val)
	{
		AddSpeed(val, 0);
	}

	public int GetComboBonus()
	{
		return GetComboBonus(0);
	}

	public int GetComboBonus(int player)
	{
		return m_comboBonus[player];
	}

	public void AddToSpeedLossTime(float val)
	{
		AddToSpeedLossTime(val, 0);
	}

	public int WaveCount()
	{
		return WaveCount(0);
	}

	public int WaveCount(int player)
	{
		return m_waveCount[player];
	}

	public List<PROBABILITY_OVERIDE> GetCurrentOverideList(int player)
	{
		return m_probabilityOverides[player, (int)Game.game_work.gameMode];
	}

	public float GetSpeed(int player)
	{
		return m_speed[player];
	}

	public void ResetSpeed(int player)
	{
		m_desiredSpeed[player] = 0f;
		m_speed[player] = 0f;
		m_speedLossTime[player] = 0f;
		uint hash = StringFunctions.StringHash("blitz_bonus");
		Game.game_work.saveData.ClearTotal(hash);
		m_comboBonus[player] = 0;
		m_nextComboBonus[player] = 0f;
		if (m_speedControl[player] != null)
		{
			m_speedControl[player].m_pulseSpeed = 0f;
			m_speedControl[player].m_lossTime = 0f;
		}
	}

	public void AddSpeed(float val, int player)
	{
		m_desiredSpeed[player] = Mortar.Math.CLAMP(val + m_desiredSpeed[player], 0f, MAX_SPEEDIE_GONZALES);
		if (!(val > 0f))
		{
			return;
		}
		uint hash = StringFunctions.StringHash("blitz_bonus");
		m_speedLossTime[player] = 1f;
		uint hash2;
		if (m_nextComboBonus[player] > 0f)
		{
			m_nextComboBonus[player] -= val;
			if (m_nextComboBonus[player] <= 0f)
			{
				m_comboBonus[player] = Game.game_work.saveData.AddToTotal("blitz_bonus", hash, 1, universal: false, achievementUpload: false);
				string s = $"blitz_{Mortar.Math.MIN(m_comboBonus[player], 6)}";
				PowerUpManager.GetInstance().ActivateScreenEffect(StringFunctions.StringHash(s));
				Game.AddToCurrentScore(5 * Mortar.Math.MIN(m_comboBonus[player], 6), player);
				m_nextComboBonus[player] = 2.5f;
				if (m_comboBonus[player] > 3)
				{
					m_nextComboBonus[player] = 2.5f;
				}
			}
		}
		else if (m_desiredSpeed[player] > SPEED_START)
		{
			m_nextComboBonus[player] = 2.5f;
			Game.game_work.saveData.ClearTotal(hash);
			m_comboBonus[player] = Game.game_work.saveData.AddToTotal("blitz_bonus", hash, 1, universal: false, achievementUpload: false);
			Game.AddToCurrentScore(5, player);
			hash2 = StringFunctions.StringHash("blitz_1");
			PowerUpManager.GetInstance().ActivateScreenEffect(hash2);
		}
		hash2 = StringFunctions.StringHash("best_blitz");
		int total = Game.game_work.saveData.GetTotal(hash2);
		Game.game_work.saveData.AddToTotal("best_blitz", hash2, Mortar.Math.MAX(0, m_comboBonus[player] - total), universal: false, achievementUpload: false);
	}

	public void AddToSpeedLossTime(float val, int player)
	{
		if (m_speedLossTime[player] > 0f)
		{
			m_speedLossTime[player] = Mortar.Math.MIN(m_speedLossTime[player] + val, 1f);
		}
	}

	public float GetComboBonusProgression(int player)
	{
		float num = (float)m_comboBonus[player] + Mortar.Math.CLAMP(1f - m_nextComboBonus[player] / 2.5f, 0f, 1f);
		return Mortar.Math.MIN(num / 6f, 1f);
	}

	public WaveManager()
	{
		m_currentGlobalDtMod = 1f;
		m_currentGlobalDt = 1f;
		m_blitzSpawnedThisGame = 0;
		m_blitzForceSpawnedCounter = 0;
		m_blitzSpawnTime = 10f + Mortar.Math.g_random.RandF(10f);
		for (int i = 0; i < 4; i++)
		{
			m_globalDtInc[i] = 0f;
			m_globalDtStart[i] = 1f;
			m_globalDtMax[i] = 1f;
		}
	}
}
