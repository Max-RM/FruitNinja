using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using FruitNinja.Port;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class Save
{
	public const int MAX_FRUIT_TYPES_TO_PICK_FROM = 32;

	public const int MAX_SLICE_COMBO = 11;

	public const int MAX_METRIC_TOTALS = 5;

	public const int MIN_BYTES_FREE = 8192;

	private static string SAVE_DIRECTORY = "FruitNinja";

	private static string SAVE_FILENAME = SAVE_DIRECTORY + "/savegame.xml";

	public static bool force_save = false;

	private static uint[] universalHashes = new uint[7]
	{
		StringFunctions.StringHash("all"),
		StringFunctions.StringHash("games"),
		StringFunctions.StringHash("totalscore"),
		StringFunctions.StringHash("sessions"),
		StringFunctions.StringHash("bomb"),
		StringFunctions.StringHash("coming_soon"),
		StringFunctions.StringHash("unrated_games")
	};

	private static void QueryIntAttribute(XmlReader reader, string name, ref int val)
	{
		string attribute = reader.GetAttribute(name);
		if (attribute != null)
		{
			val = MParser.ParseInt(attribute);
		}
		else
		{
			val = 0;
		}
	}

	private static void QueryFloatAttribute(XmlReader reader, string name, ref float val)
	{
		string attribute = reader.GetAttribute(name);
		if (attribute != null)
		{
			val = MParser.ParseFloat(attribute);
		}
		else
		{
			val = 0f;
		}
	}

	private static void ParseAchievements(XDocument doc, FruitSaveData saveData, bool queList)
	{
		XmlReader xmlReader = doc.CreateReader();
		while (xmlReader.Read())
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			XmlNodeType xmlNodeType = nodeType;
			string name;
			if (xmlNodeType != XmlNodeType.Element || (name = xmlReader.Name) == null || !(name == "ach"))
			{
				continue;
			}
			AchievementItem value = new AchievementItem
			{
				time = 0f
			};
			string attribute = xmlReader.GetAttribute("name");
			if (attribute != null)
			{
				uint key = StringFunctions.StringHash(attribute);
				value.name = attribute;
				attribute = xmlReader.GetAttribute("time");
				if (attribute != null)
				{
					value.time = MParser.ParseFloat(attribute);
				}
				if (queList)
				{
					saveData.AchievementQue[key] = value;
				}
				else
				{
					saveData.unlockedAchievements[key] = value;
				}
			}
		}
	}

	private static bool ParseSuperFruit(XDocument doc, EntityState state)
	{
		XmlReader xmlReader = doc.CreateReader();
		bool result = false;
		while (xmlReader.Read())
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			XmlNodeType xmlNodeType = nodeType;
			string name;
			if (xmlNodeType == XmlNodeType.Element && (name = xmlReader.Name) != null && name == "superFruitState")
			{
				state.superState = new SuperFruitState();
				string attribute = xmlReader.GetAttribute("time");
				if (attribute != null)
				{
					state.superState.time = MParser.ParseFloat(attribute);
				}
				attribute = xmlReader.GetAttribute("sliceTime");
				if (attribute != null)
				{
					state.superState.sliceTime = MParser.ParseFloat(attribute);
				}
				attribute = xmlReader.GetAttribute("rot");
				if (attribute != null)
				{
					state.superState.rotation = MParser.ParseFloat(attribute);
				}
				attribute = xmlReader.GetAttribute("hits");
				if (attribute != null)
				{
					state.superState.hits = MParser.ParseInt(attribute);
				}
				result = true;
			}
		}
		return result;
	}

	private static void ParseWaveInfo(XDocument doc, FruitSaveData saveData)
	{
		XmlReader xmlReader = doc.CreateReader();
		saveData.waves.Clear();
		WaveState value = new WaveState
		{
			spawners = new LinkedList<SpawnState>()
		};
		while (xmlReader.Read())
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			XmlNodeType xmlNodeType = nodeType;
			if (xmlNodeType != XmlNodeType.Element)
			{
				continue;
			}
			switch (xmlReader.Name)
			{
			case "wave":
				value = new WaveState
				{
					spawners = new LinkedList<SpawnState>()
				};
				QueryFloatAttribute(xmlReader, "inc", ref value.inc);
				QueryIntAttribute(xmlReader, "index", ref value.index);
				saveData.waves.AddLast(value);
				break;
			case "spawner":
			{
				SpawnState value2 = default(SpawnState);
				QueryFloatAttribute(xmlReader, "delay", ref value2.delayWait);
				QueryIntAttribute(xmlReader, "toSpawn", ref value2.toSpawnThisWave);
				if (value.spawners != null)
				{
					value.spawners.AddLast(value2);
				}
				break;
			}
			}
		}
	}

	private static void ParseSaveFile(XDocument doc, FruitSaveData saveData)
	{
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		XmlReader xmlReader = doc.CreateReader();
		while (xmlReader.Read())
		{
			switch (xmlReader.NodeType)
			{
			case XmlNodeType.Element:
			{
				if (string.Compare(xmlReader.Name, "save_file") == 0)
				{
					XElement xElement = doc.Element("save_file");
					xElement.QueryIntAttribute("highscore", ref saveData.highScore);
					xElement.QueryIntAttribute("critical_chance", ref saveData.criticalProgression);
					int value = 0;
					int value2 = 0;
					int value3 = 0;
					xElement.QueryIntAttribute("major", ref value);
					xElement.QueryIntAttribute("minor", ref value2);
					xElement.QueryIntAttribute("patch", ref value3);
					XAttribute xAttribute = xElement.Attribute("rated");
					saveData.game_rated = xAttribute != null && string.Compare(xAttribute.Value, "true") == 0;
					saveData.highScores[0] = saveData.highScore;
					for (int i = 0; i < 4; i++)
					{
						string attributeName = $"{Initialise.GetModeName((Game.GAME_MODE)i)}highscore";
						xElement.QueryIntAttribute(attributeName, ref saveData.highScores[i]);
					}
					saveData.saveVersion = value * 10000 + value2 * 100 + value3;
					break;
				}
				if (string.Compare(xmlReader.Name, "que") == 0)
				{
					ParseAchievements(doc, saveData, queList: true);
					break;
				}
				if (string.Compare(xmlReader.Name, "unlocked") == 0)
				{
					ParseAchievements(doc, saveData, queList: false);
					break;
				}
				if (string.Compare(xmlReader.Name, "total") == 0)
				{
					int num = 0;
					string attribute = xmlReader.GetAttribute("score");
					if (attribute != null)
					{
						num = MParser.ParseInt(attribute);
					}
					bool flag = false;
					attribute = xmlReader.GetAttribute("u");
					if (attribute != null && string.Compare(attribute, "true") == 0)
					{
						flag = true;
					}
					attribute = xmlReader.GetAttribute("type");
					if (attribute == null || num <= 0)
					{
						break;
					}
					uint num2 = StringFunctions.StringHash(attribute);
					if (!flag)
					{
						for (int j = 0; j < universalHashes.Length; j++)
						{
							if (universalHashes[j] == num2)
							{
								flag = true;
								break;
							}
						}
					}
					saveData.AddToTotal(attribute, num2, num, flag, achievementUpload: false);
					break;
				}
				if (string.Compare(xmlReader.Name, "ent") == 0)
				{
					if (saveData.mode < 4 && saveData.saveVersion == Game.GetVersionTotal())
					{
						EntityState entityState = default(EntityState);
						string attribute2 = xmlReader.GetAttribute("pos");
						entityState.pos = ParseVector(attribute2);
						attribute2 = xmlReader.GetAttribute("vel");
						entityState.vel = ParseVector(attribute2);
						attribute2 = xmlReader.GetAttribute("grav");
						entityState.grav = ParseVector(attribute2);
						attribute2 = xmlReader.GetAttribute("wait");
						if (attribute2 != null)
						{
							entityState.wait = MParser.ParseFloat(attribute2);
						}
						attribute2 = xmlReader.GetAttribute("hit");
						entityState.hit = attribute2 != null && string.Compare(attribute2, "true") == 0;
						attribute2 = xmlReader.GetAttribute("type");
						if (attribute2 != null)
						{
							entityState.type = MParser.ParseInt(attribute2);
						}
						if (!ParseSuperFruit(doc, entityState))
						{
							entityState.superState = null;
						}
						saveData.entities.AddLast(entityState);
					}
					break;
				}
				if (string.Compare(xmlReader.Name, "powers") == 0)
				{
					PowerUpManager.GetInstance().LoadActivePowerUps(doc.Element("save_file").Element("powers"), saveData.mode);
					break;
				}
				if (string.Compare(xmlReader.Name, "state") == 0)
				{
					saveData.entities.Clear();
					string attribute3 = xmlReader.GetAttribute("mode");
					if (attribute3 != null)
					{
						saveData.mode = (int)Initialise.ParseGameMode(StringFunctions.StringHash(attribute3));
					}
					if (saveData.mode < 4 && saveData.saveVersion == Game.GetVersionTotal())
					{
						QueryIntAttribute(xmlReader, "score", ref saveData.score);
						QueryIntAttribute(xmlReader, "misses", ref saveData.misses);
						attribute3 = xmlReader.GetAttribute("hasDropped");
						saveData.hasDropped = attribute3 != null && string.Compare(attribute3, "true") == 0;
						QueryIntAttribute(xmlReader, "consecutiveCount", ref saveData.consecutiveCount);
						QueryIntAttribute(xmlReader, "consecutiveType", ref saveData.consecutiveType);
						QueryFloatAttribute(xmlReader, "timer", ref saveData.timer);
						QueryIntAttribute(xmlReader, "go_head", ref saveData.go_head);
						QueryIntAttribute(xmlReader, "go_body", ref saveData.go_body);
						QueryIntAttribute(xmlReader, "go_fruit", ref saveData.go_fruit);
						QueryIntAttribute(xmlReader, "go_fact", ref saveData.go_fact);
						attribute3 = xmlReader.GetAttribute("go_showHighScore");
						saveData.go_showHighScore = attribute3 != null && string.Compare(attribute3, "true") == 0;
						attribute3 = xmlReader.GetAttribute("go_setScore");
						saveData.go_setScore = attribute3 != null && string.Compare(attribute3, "true") == 0;
						attribute3 = xmlReader.GetAttribute("randSeed");
						if (attribute3 != null)
						{
							Mortar.Math.g_random.x = MParser.ParseULong(attribute3);
						}
						attribute3 = xmlReader.GetAttribute("randMul");
						if (attribute3 != null)
						{
							Mortar.Math.g_random.mul = MParser.ParseULong(attribute3);
						}
						attribute3 = xmlReader.GetAttribute("randAdd");
						if (attribute3 != null)
						{
							Mortar.Math.g_random.add = MParser.ParseULong(attribute3);
						}
						QueryIntAttribute(xmlReader, "go_state", ref saveData.go_state);
						QueryFloatAttribute(xmlReader, "go_time", ref saveData.go_time);
						QueryFloatAttribute(xmlReader, "go_bombHitTime", ref saveData.go_bombHitTime);
						QueryFloatAttribute(xmlReader, "go_transition", ref saveData.go_transition);
						QueryFloatAttribute(xmlReader, "shake_time", ref saveData.shake_time);
						QueryFloatAttribute(xmlReader, "shake_max_time", ref saveData.shake_max_time);
						QueryFloatAttribute(xmlReader, "speedLossTime", ref saveData.speedLossTime);
						QueryFloatAttribute(xmlReader, "desiredSpeed", ref saveData.desiredSpeed);
						QueryFloatAttribute(xmlReader, "nextComboBonus", ref saveData.nextComboBonus);
					}
					break;
				}
				if (string.Compare(xmlReader.Name, "wave_info") == 0)
				{
					if (saveData.mode < 4 && saveData.saveVersion == Game.GetVersionTotal())
					{
						QueryIntAttribute(xmlReader, "waveCount", ref saveData.currentWave);
						QueryFloatAttribute(xmlReader, "waveDelay", ref saveData.currentWaveDelay);
						QueryIntAttribute(xmlReader, "numberOfWavesSpawned", ref saveData.numberOfWavesSpawned);
						QueryFloatAttribute(xmlReader, "waveWait", ref saveData.currentWaveWait);
						QueryIntAttribute(xmlReader, "blitzSpawnedThisGame", ref saveData.m_blitzSpawnedThisGame);
						QueryIntAttribute(xmlReader, "blitzForceSpawnedCounter", ref saveData.m_blitzForceSpawnedCounter);
						QueryFloatAttribute(xmlReader, "blitzSpawnTime", ref saveData.m_blitzSpawnTime);
						ParseWaveInfo(doc, saveData);
					}
					break;
				}
				bool flag2 = false;
				for (int k = 0; k < 4; k++)
				{
					string text = "wave_counts_" + Initialise.GetModeName((Game.GAME_MODE)k);
					if (text == xmlReader.Name)
					{
						flag2 = true;
						XElement element = doc.Element("save_file").Element(text);
						for (XElement xElement2 = element.FirstChildElement("game_count"); xElement2 != null; xElement2 = xElement2.NextSiblingElement("game_count"))
						{
							int value4 = -1;
							int value5 = -1;
							xElement2.QueryIntAttribute("waveIdx", ref value4);
							xElement2.QueryIntAttribute("games", ref value5);
							saveData.waveGameCount[k][value4] = value5;
						}
					}
				}
				break;
			}
			}
		}
	}

	public static bool LoadGame(FruitSaveData saveData)
	{
		try
		{
			if (DesktopStorage.FileExists(SAVE_FILENAME))
			{
				byte[] array = DesktopStorage.ReadAllBytes(SAVE_FILENAME);
				if (array != null && array.Length > 0)
				{
					string text = Encoding.UTF8.GetString(array, 0, array.Length);
					XDocument doc = XDocument.Parse(text);
					ParseSaveFile(doc, saveData);
					if (saveData.mode >= 4)
					{
						saveData.mode = 0;
					}
					if (saveData.saveVersion != Game.GetVersionTotal())
					{
						saveData.ClearTotal(StringFunctions.StringHash("unrated_games"));
						for (int i = 0; i < 4; i++)
						{
							saveData.waveGameCount[i].Clear();
						}
						saveData.game_rated = false;
					}
				}
			}
			else
			{
				if (!DesktopStorage.DirectoryExists(SAVE_DIRECTORY))
				{
					DesktopStorage.CreateDirectory(SAVE_DIRECTORY);
				}
				using (DesktopStorage.OpenFile(SAVE_FILENAME, FileMode.CreateNew)) { }
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public static bool SaveGame(FruitSaveData saveData)
	{
		bool result = false;
		try
		{
			if (!DesktopStorage.DirectoryExists("FruitNinja"))
			{
				DesktopStorage.CreateDirectory("FruitNinja");
			}
			XDocument xDocument = new XDocument();
			XDeclaration declaration = new XDeclaration("1.0", "utf-8", "yes");
			xDocument.Declaration = declaration;
			XElement xElement = new XElement("save_file");
			xDocument.Add(xElement);
			for (int i = 0; i < 4; i++)
			{
				string text = $"{Initialise.GetModeName((Game.GAME_MODE)i)}highscore";
				xElement.Add(new XAttribute(text, saveData.highScores[i]));
			}
			xElement.Add(new XAttribute("highscore", saveData.highScore));
			xElement.Add(new XAttribute("critical_chance", saveData.criticalProgression));
			xElement.Add(new XAttribute("rated", saveData.game_rated ? "true" : "false"));
			xElement.Add(new XAttribute("major", Game.GetVersionMajor()));
			xElement.Add(new XAttribute("minor", Game.GetVersionMinor()));
			xElement.Add(new XAttribute("batch", Game.GetVersionPatch()));
			foreach (KeyValuePair<uint, SliceTotal> total in saveData.totals)
			{
				XElement xElement2 = new XElement("total");
				xElement2.Add(new XAttribute("type", total.Value.name));
				xElement2.Add(new XAttribute("score", total.Value.total));
				xElement.Add(xElement2);
			}
			foreach (KeyValuePair<uint, SliceTotal> universalTotal in saveData.universalTotals)
			{
				XElement xElement3 = new XElement("total");
				xElement3.Add(new XAttribute("u", true));
				xElement3.Add(new XAttribute("type", universalTotal.Value.name));
				xElement3.Add(new XAttribute("score", universalTotal.Value.total));
				xElement.Add(xElement3);
			}
			if (saveData.AchievementQue.Count > 0)
			{
				XElement xElement4 = new XElement("que");
				foreach (KeyValuePair<uint, AchievementItem> item in saveData.AchievementQue)
				{
					XElement xElement5 = new XElement("ach");
					xElement5.Add(new XAttribute("name", item.Value.name));
					xElement5.Add(new XAttribute("time", item.Value.time));
					xElement4.Add(xElement5);
				}
				xElement.Add(xElement4);
			}
			if (saveData.unlockedAchievements.Count > 0)
			{
				XElement xElement6 = new XElement("unlocked");
				foreach (KeyValuePair<uint, AchievementItem> unlockedAchievement in saveData.unlockedAchievements)
				{
					XElement xElement7 = new XElement("ach");
					xElement7.Add(new XAttribute("name", unlockedAchievement.Value.name));
					xElement6.Add(xElement7);
				}
				xElement.Add(xElement6);
			}
			if (saveData.inGame || saveData.go_bombHitTime > 0f || force_save)
			{
				XElement xElement8 = new XElement("state");
				xElement8.Add(new XAttribute("hasDropped", saveData.hasDropped ? "true" : "false"));
				xElement8.Add(new XAttribute("score", saveData.score));
				xElement8.Add(new XAttribute("misses", saveData.misses));
				xElement8.Add(new XAttribute("mode", Initialise.GetModeName((Game.GAME_MODE)saveData.mode)));
				xElement8.Add(new XAttribute("consecutiveCount", saveData.consecutiveCount));
				xElement8.Add(new XAttribute("consecutiveType", saveData.consecutiveType));
				xElement8.Add(new XAttribute("timer", saveData.timer));
				xElement8.Add(new XAttribute("globalWaveDt", saveData.globalWaveDt));
				string text2 = null;
				for (int j = 0; j < saveData.numFruitTypesToPickFrom; j++)
				{
					text2 = ((j != 0) ? $"{text2},{saveData.fruitTypesToPickFrom[j]}" : $"{saveData.fruitTypesToPickFrom[j]}");
				}
				if (text2 != null)
				{
					xElement8.Add(new XAttribute("typesToPickFrom", text2));
				}
				for (int k = 0; k < saveData.numFruitTypesInSliceCombo; k++)
				{
					text2 = ((k != 0) ? $"{text2},{saveData.sliceComboFruitTypes[k]}" : $"{saveData.sliceComboFruitTypes[k]}");
				}
				if (text2 != null)
				{
					xElement8.Add(new XAttribute("best_combo", text2));
				}
				xElement8.Add(new XAttribute("go_state", saveData.go_state));
				xElement8.Add(new XAttribute("go_time", saveData.go_time));
				xElement8.Add(new XAttribute("go_bombHitTime", saveData.go_bombHitTime));
				xElement8.Add(new XAttribute("go_transition", saveData.go_transition));
				xElement8.Add(new XAttribute("go_body", saveData.go_body));
				xElement8.Add(new XAttribute("go_head", saveData.go_head));
				xElement8.Add(new XAttribute("go_fruit", saveData.go_fruit));
				xElement8.Add(new XAttribute("go_fact", saveData.go_fact));
				xElement8.Add(new XAttribute("go_showHighScore", saveData.go_showHighScore ? "true" : "false"));
				xElement8.Add(new XAttribute("go_setScore", saveData.go_setScore ? "true" : "false"));
				xElement8.Add(new XAttribute("randSeed", Mortar.Math.g_random.x));
				xElement8.Add(new XAttribute("randMul", Mortar.Math.g_random.mul));
				xElement8.Add(new XAttribute("randAdd", Mortar.Math.g_random.add));
				xElement8.Add(new XAttribute("shake_time", saveData.shake_time));
				xElement8.Add(new XAttribute("shake_max_time", saveData.shake_time));
				if (saveData.desiredSpeed > 0f)
				{
					xElement8.Add(new XAttribute("speedLossTime", saveData.speedLossTime));
					xElement8.Add(new XAttribute("desiredSpeed", saveData.desiredSpeed));
					xElement8.Add(new XAttribute("nextComboBonus", saveData.nextComboBonus));
				}
				if (saveData.waves.Count > 0)
				{
					XElement xElement9 = new XElement("wave_info");
					xElement9.Add(new XAttribute("waveCount", saveData.currentWave));
					xElement9.Add(new XAttribute("waveDelay", saveData.currentWaveDelay));
					xElement9.Add(new XAttribute("numberOfWavesSpawned", saveData.numberOfWavesSpawned));
					xElement9.Add(new XAttribute("waveWait", saveData.currentWaveWait));
					xElement9.Add(new XAttribute("blitzSpawnedThisGame", saveData.m_blitzSpawnedThisGame));
					xElement9.Add(new XAttribute("blitzForceSpawnedCounter", saveData.m_blitzForceSpawnedCounter));
					xElement9.Add(new XAttribute("blitzSpawnTime", saveData.m_blitzSpawnTime));
					foreach (WaveState wave in saveData.waves)
					{
						XElement xElement10 = new XElement("wave");
						xElement10.Add(new XAttribute("inc", wave.inc + 0.01f));
						xElement10.Add(new XAttribute("index", wave.index));
						foreach (SpawnState spawner in wave.spawners)
						{
							XElement xElement11 = new XElement("spawner");
							xElement11.Add(new XAttribute("delay", spawner.delayWait));
							xElement11.Add(new XAttribute("toSpawn", spawner.toSpawnThisWave));
							xElement10.Add(xElement11);
						}
						xElement9.Add(xElement10);
					}
					xElement8.Add(xElement9);
				}
				LinkedListNode<Entity> iterator = null;
				for (Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); fruit != null; fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
				{
					if (!fruit.m_isSliced && !fruit.m_isMenuItem)
					{
						XElement xElement12 = new XElement("ent");
						text2 = $"{fruit.m_pos.X},{fruit.m_pos.Y},{fruit.m_pos.Z}";
						xElement12.Add(new XAttribute("pos", text2));
						text2 = $"{fruit.m_vel.X},{fruit.m_vel.Y},{fruit.m_vel.Z}";
						xElement12.Add(new XAttribute("vel", text2));
						text2 = $"{fruit.m_gravity.X},{fruit.m_gravity.Y},{fruit.m_gravity.Z}";
						xElement12.Add(new XAttribute("grav", text2));
						xElement12.Add(new XAttribute("type", fruit.GetFruitType()));
						xElement12.Add(new XAttribute("wait", fruit.GetWait()));
						if (Fruit.FruitInfo(fruit.GetFruitType()).superFruit)
						{
							SuperFruitControl.SaveSuperFruitState(fruit, xElement12);
						}
						xElement8.Add(xElement12);
					}
				}
				for (Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); bomb != null; bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
				{
					if (!bomb.m_isMenuItem)
					{
						XElement xElement13 = new XElement("ent");
						text2 = $"{bomb.m_pos.X},{bomb.m_pos.Y},{bomb.m_pos.Z}";
						xElement13.Add(new XAttribute("pos", text2));
						text2 = $"{bomb.m_vel.X},{bomb.m_vel.Y},{bomb.m_vel.Z}";
						xElement13.Add(new XAttribute("vel", text2));
						text2 = $"{bomb.m_gravity.X},{bomb.m_gravity.Y},{bomb.m_gravity.Z}";
						xElement13.Add(new XAttribute("grav", text2));
						xElement13.Add(new XAttribute("type", Fruit.MAX_FRUIT_TYPES));
						xElement13.Add(new XAttribute("hit", bomb.m_hit ? "true" : "false"));
						xElement13.Add(new XAttribute("wait", bomb.GetWait()));
						xElement8.Add(xElement13);
					}
				}
				for (BombBlast bombBlast = (BombBlast)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB_BLAST, ref iterator); bombBlast != null; bombBlast = (BombBlast)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB_BLAST, ref iterator))
				{
					XElement xElement14 = new XElement("ent");
					text2 = $"{bombBlast.m_pos.X},{bombBlast.m_pos.Y},{bombBlast.m_pos.Z}";
					xElement14.Add(new XAttribute("pos", text2));
					xElement14.Add(new XAttribute("wait", bombBlast.m_time));
					xElement14.Add(new XAttribute("type", -1));
					xElement8.Add(xElement14);
				}
				xElement.Add(xElement8);
			}
			for (int l = 0; l < 4; l++)
			{
				string text3 = null;
				text3 = $"wave_counts_{Initialise.GetModeName((Game.GAME_MODE)l)}";
				if (saveData.waveGameCount[l].Count <= 0)
				{
					continue;
				}
				XElement xElement15 = new XElement(text3);
				foreach (KeyValuePair<int, int> item2 in saveData.waveGameCount[l])
				{
					XElement xElement16 = new XElement("game_count");
					xElement16.Add(new XAttribute("waveIdx", item2.Key));
					xElement16.Add(new XAttribute("games", item2.Value));
					xElement15.Add(xElement16);
				}
				xElement.Add(xElement15);
			}
			XElement xElement17 = new XElement("powers");
			PowerUpManager.GetInstance().SaveActivePowerUps(xElement17);
			xElement.Add(xElement17);
			MemoryStream memoryStream = new MemoryStream();
			xDocument.Save(memoryStream);
			byte[] array = memoryStream.ToArray();
			Encoding.UTF8.GetString(array, 0, array.Length);
			using (var stream = DesktopStorage.OpenFile(SAVE_FILENAME, FileMode.Create))
			{
				stream.Write(array, 3, array.Length - 3);
			}
			result = true;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static Vector3 ParseVector(string text)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (text == null)
		{
			return Vector3.Zero;
		}
		int num = text.IndexOf(',');
		Vector3 result = default(Vector3);
		if (num == -1)
		{
			result.X = MParser.ParseFloat(text);
		}
		else
		{
			result.X = MParser.ParseFloat(text.Substring(0, num));
			int num2 = text.IndexOf(',', num + 1);
			if (num2 == -1)
			{
				result.Y = MParser.ParseFloat(text.Substring(num + 1, text.Length - num - 1));
			}
			else
			{
				result.Y = MParser.ParseFloat(text.Substring(num + 1, num2 - num - 1));
				result.Z = MParser.ParseFloat(text.Substring(num2 + 1, text.Length - num2 - 1));
			}
		}
		return result;
	}

	public static bool ParseVector(string text, ref Vector3 vec)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (text != null && text.Length > 0)
		{
			vec = ParseVector(text);
			return true;
		}
		return false;
	}
}
