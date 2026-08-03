using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class GameOverScreen : HUDControl3d
{
	public enum GOS
	{
		GOS_IN,
		GOS_BONUS_SCREEN,
		GOS_WAIT,
		GOS_OUT,
		GOS_RESTARTING,
		GOS_WAIT_TO_DIE,
		GOS_LEADERBOARDS,
		GOS_DIE,
		GOS_POSTING,
		GOS_CHANGE_MODE
	}

	protected float m_alertTime;

	protected int m_state;

	protected float m_time;

	protected Vector3 m_originalScale;

	protected MenuButton m_retry;

	protected MenuButton m_leaderboardButton;

	protected MenuButton m_quitButton;

	protected static Texture[] m_senseiHeads = new Texture[3];

	protected static Texture[] m_senseiBody = new Texture[3];

	protected static Texture s_gameOverTexture;

	protected static Texture s_timeUpTexture;

	protected static Texture m_highScoreTexture;

	protected static Texture s_retryTexture;

	protected static Texture s_leaderboardTexture;

	protected static Texture s_quitTexture;

	protected int m_highScoreFrame;

	protected Vector3 m_senseiPos;

	protected FruitFactControl m_fruitFact;

	protected BonusScreen m_bonusScreen;

	protected bool m_alreadyPosted;

	protected bool m_waitingForPost;

	protected string m_coinsEarnedText;

	protected Texture m_oldHighScoreTexture;

	protected int m_mostOfFruit;

	protected int m_mostOfFruitSliced;

	public bool m_hasSetScore;

	public bool m_hasSpawnedFinalPomegranate;

	public int m_head;

	public int m_body;

	public int m_fruit;

	public int m_fact;

	public float m_leaderbordPressedTime;

	private static string[] adjectives = new string[6] { "delicious", "tasty", "juicy", "luscious", "succulent", "lush" };

	private static bool s_isContentLoaded = false;

	private static bool buttonCalled = false;

	private static Texture s_timeUpGreenTexture = null;

	private static Texture s_leaderboardButtonTexture = null;

	private static Texture s_quitButtonTexture = null;

	private static Texture s_coinsBundle = null;

	public static float BEGINNING_WAIT => 1.9f;

	public static float BEGINNING_Y => 0f;

	public static float BEGINNING_SCALE => 2f;

	public static float NORMAL_SCALE => 1f;

	public static float NORMAL_Y => Game.SCREEN_HEIGHT * 0.7f;

	public static float POP_IN_TIME => 0.2f;

	public static float POP_SINE => (int)Mortar.Math.DEGREE_TO_IDX(110f);

	public static float SIDE_BUTTONS_X => 150f;

	public static float SIDE_BUTTONS_Y => Game.USE_ARCADE_GO_SCREEN ? (-130) : 5;

	public static float TWITTER_X => (0f - Game.SCREEN_WIDTH) / 2f + 50f;

	public static float FACEBOOK_X => (0f - Game.SCREEN_WIDTH) / 2f + (float)(Game.USE_ARCADE_GO_SCREEN ? 20 : 80);

	public static float SENSEI_CENTER_X => (0f - Game.SCREEN_WIDTH) / 2f + 222f;

	public static float SENSEI_CENTER_Y => 55f;

	public static float SENSEI_OFFSCREEN_X => Game.SCREEN_WIDTH / 2f + 128f;

	public static Vector3 SENSEI_HEAD_OFFSET => new Vector3(9f, 40f, 0f);

	public static Vector3 HIGH_SCORE_OFFSET => new Vector3(-80f, 70f, 0f);

	public static Vector3 SENSEI_FACT_OFFSET_X => new Vector3(183f, 12f, 0f);

	public static float NEW_HIGHSCORE_FLASH => 1000f;

	public static float ZEN_BOARD_X => (0f - Game.SCREEN_WIDTH) / 2f + 315f;

	public static float ZEN_BOARD_Y => 53f;

	public static Vector3 QUIT_POS => new Vector3(163f, 0f - Game.SCREEN_HEIGHT / 2f + 64f, 0f);

	public static Vector3 RETRY_POS => new Vector3(0f, 0f - Game.SCREEN_HEIGHT / 2f + 64f, 0f);

	public static Vector3 LEADERBOARD_POS => new Vector3(-163f, 0f - Game.SCREEN_HEIGHT / 2f + 64f, 0f);

	public static Vector3 LEADERBOARD_BUTTON_POS => new Vector3(190f, -50f, 0f);

	public static Vector3 QUIT_BUTTON_POS => new Vector3(190f, -125f, 0f);

	public GameOverScreen()
		: this(null)
	{
	}

	public GameOverScreen(string texture)
		: this(texture, -1)
	{
	}

	public GameOverScreen(string texture, int state)
		: this(texture, state, -1f)
	{
	}

	public GameOverScreen(string texture, int state, float time)
		: this(texture, state, time, -1)
	{
	}

	public GameOverScreen(string texture, int state, float time, int head)
		: this(texture, state, time, head, -1)
	{
	}

	public GameOverScreen(string texture, int state, float time, int head, int body)
		: this(texture, state, time, head, body, -1)
	{
	}

	public GameOverScreen(string texture, int state, float time, int head, int body, int fruit)
		: this(texture, state, time, head, body, fruit, -1)
	{
	}

	public GameOverScreen(string texture, int state, float time, int head, int body, int fruit, int fact)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		m_deleteCall = delegate(HUDControl l)
		{
			((GameOverScreen)l).Release();
		};
		if (!s_isContentLoaded)
		{
			LoadContent();
		}
		m_mostOfFruit = 0;
		m_mostOfFruitSliced = 0;
		m_alertTime = 0f;
		m_fruit = fruit;
		m_fact = fact;
		m_texture = ((!Game.USE_ZEN_GO_SCREEN && !Game.USE_ARCADE_GO_SCREEN) ? s_gameOverTexture : ((Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE) ? s_timeUpGreenTexture : s_timeUpTexture));
		m_originalScale = new Vector3((float)m_texture.GetWidth(), (float)m_texture.GetHeight(), 0f);
		m_state = 0;
		m_selfCleanUp = false;
		m_time = 0f;
		m_oldHighScoreTexture = null;
		m_highScoreTexture = null;
		m_highScoreFrame = 0;
		m_retry = null;
		m_hasSetScore = false;
		m_bonusScreen = null;
		m_head = head;
		m_body = body;
		m_leaderboardButton = null;
		m_quitButton = null;
		if (m_head < 1)
		{
			m_head = 1;
			m_mostOfFruitSliced = 0;
			m_mostOfFruit = -1;
			for (int num = 0; num < Fruit.MAX_FRUIT_TYPES - 1; num++)
			{
				int total = Game.game_work.saveData.GetTotal(Fruit.FruitTypeHash(num));
				if (total > m_mostOfFruitSliced)
				{
					m_mostOfFruitSliced = total;
					m_mostOfFruit = num;
				}
			}
			if (Game.game_work.currentScore > Game.GetCurrentModeHighscore() / 2)
			{
				m_head = 2 + Mortar.Math.g_random.Rand32(2);
			}
		}
		if (m_body < 1)
		{
			m_body = Mortar.Math.g_random.Rand32(3) + 1;
		}
		m_pos = new Vector3(0f, BEGINNING_Y, 0f);
		m_senseiPos = new Vector3(SENSEI_OFFSCREEN_X, SENSEI_CENTER_Y, 0f);
		m_fruitFact = null;
		m_alreadyPosted = false;
		m_waitingForPost = false;
		m_coinsEarnedText = $"YOU JUST EARNT {Game.game_work.coins - Game.game_work.levelStartCoins} COINS";
		m_hasSpawnedFinalPomegranate = true;
		if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			m_hasSpawnedFinalPomegranate = SuperFruitControl.NumPomegranatesSpawnedThisGame() != 0;
			if (m_hasSpawnedFinalPomegranate)
			{
				m_pos = new Vector3(0f, 0f - Game.SCREEN_HEIGHT, 0f);
				if (time <= 0f && state == 1)
				{
					m_state = 1;
					m_time = (time = -0.333f);
					m_pos = new Vector3(0f, 0f - Game.SCREEN_HEIGHT, 0f);
				}
			}
		}
		if (!(time >= 0f) || state < 0)
		{
			return;
		}
		m_mostOfFruitSliced = 0;
		m_mostOfFruit = -1;
		for (int num2 = 0; num2 < Fruit.MAX_FRUIT_TYPES - 1; num2++)
		{
			int total2 = Game.game_work.saveData.GetTotal(Fruit.FruitTypeHash(num2));
			if (total2 > m_mostOfFruitSliced)
			{
				m_mostOfFruitSliced = total2;
				m_mostOfFruit = num2;
			}
		}
		if (state != 0 && Game.game_work.gameOverTransition > 0.999f)
		{
			Game.game_work.gameOverTransition = 0.998f;
			m_state = 2;
			m_hasSetScore = true;
			Update(0f);
		}
		m_time = time;
		m_state = state;
		m_texture = null;
	}

	~GameOverScreen()
	{
		Release();
	}

	public void RetryCallback()
	{
		if (m_state == 0 || (m_state == 2 && Game.game_work.gameOverTransition > 0.999f))
		{
			Mortar.Math.g_random.Seed(Game.game_work.gameSeedValue);
			Game.game_work.saveData.ClearCombo();
			m_state = 3;
			SoundManager.GetInstance().SFXPlay(SoundDef.SND_DANANANA_SCHWING);
		}
	}

	public void LeaderboardsCallback()
	{
		if (m_state == 0 || (m_state == 2 && Game.game_work.gameOverTransition > 0.999f))
		{
			m_time = 0f;
			m_state = 6;
			m_leaderbordPressedTime = 0f;
		}
	}

	public void QuitCallback()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (m_state == 0 || m_state == 2)
		{
			Game.game_work.saveData.ClearCombo();
			m_state = 5;
			Game.HitMenuBomb(new Vector3(403f - Game.SCREEN_WIDTH / 2f, Game.SCREEN_HEIGHT / 2f - 256f, 0f));
		}
	}

	public void FacebookCallback()
	{
		throw new MissingMemberException();
	}

	public void TwitterCallback()
	{
		throw new MissingMemberException();
	}

	public void PostCallback(int state)
	{
		throw new MissingMemberException();
	}

	public int GetState()
	{
		return m_state;
	}

	public float GetTime()
	{
		return m_time;
	}

	public override void Reset()
	{
	}

	public override void Release()
	{
		m_texture = null;
		if (Game.game_work.gameOverScreen == this)
		{
			Game.game_work.saveData.go_head = (Game.game_work.saveData.go_body = (Game.game_work.saveData.go_fruit = (Game.game_work.saveData.go_fact = -1)));
			Game.game_work.saveData.go_showHighScore = false;
			Game.game_work.gameOverScreen = null;
		}
		if (m_fruitFact != null)
		{
			m_fruitFact.Release();
			Game.game_work.hud.RemoveControl(m_fruitFact);
		}
		if (m_leaderboardButton != null)
		{
			Game.game_work.hud.RemoveControl(m_leaderboardButton);
		}
		if (m_quitButton != null)
		{
			Game.game_work.hud.RemoveControl(m_quitButton);
		}
		Delete.SAFE_DELETE(ref m_fruitFact);
		Delete.SAFE_DELETE(ref m_leaderboardButton);
		Delete.SAFE_DELETE(ref m_quitButton);
	}

	public override void Init()
	{
		Reset();
	}

	public override void Update(float dt)
	{
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Expected O, but got Unknown
		m_highScoreFrame += (int)(dt * 1000f);
		if ((float)m_highScoreFrame >= NEW_HIGHSCORE_FLASH)
		{
			m_highScoreFrame -= (int)NEW_HIGHSCORE_FLASH;
		}
		if (buttonCalled)
		{
			buttonCalled = false;
		}
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		switch ((GOS)m_state)
		{
		case GOS.GOS_IN:
			if (!m_hasSpawnedFinalPomegranate)
			{
				if (SuperFruitControl.CanSpawnFinalPomegranate())
				{
					m_hasSpawnedFinalPomegranate = SuperFruitControl.SpawnFinalPomegranate();
					m_state = 1;
					m_time = -0.333f;
					m_pos = new Vector3(0f, 0f - Game.SCREEN_HEIGHT, 0f);
				}
				break;
			}
			TheGame.pomegranateThrown = false;
			if (!m_hasSetScore)
			{
				Game.game_work.canFastForward = true;
			}
			m_time += dt;
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
			if (m_time < POP_IN_TIME)
			{
				ushort idx = (ushort)(Mortar.Math.MIN(POP_SINE, POP_SINE) * (m_time / POP_IN_TIME));
				m_scale = m_originalScale * Mortar.Math.SinIdx(idx) / Mortar.Math.SinIdx((ushort)POP_SINE) * BEGINNING_SCALE;
			}
			else
			{
				m_scale = m_originalScale * BEGINNING_SCALE;
			}
			if (m_time > BEGINNING_WAIT)
			{
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
				{
					m_state = 1;
					m_time = -0.333f;
				}
				else
				{
					SetStateWait();
				}
			}
			m_pos = new Vector3(0f, BEGINNING_Y, 0f);
			break;
		case GOS.GOS_BONUS_SCREEN:
			if (ActorManager.GetInstance().GetNumEntities(0) == 0 && ActorManager.GetInstance().GetNumEntities(1) == 0)
			{
				if (m_bonusScreen == null)
				{
					m_bonusScreen = new BonusScreen();
					m_bonusScreen.m_pos = new Vector3(0f, -20f, 0f);
					m_bonusScreen.m_deleteCall = DeletedBonusScreen;
					Game.game_work.hud.AddControl(m_bonusScreen);
					BonusManager.GetInstance().SetUpBonusScreen(m_bonusScreen);
					TheGame.pomegranateThrown = false;
				}
				else
				{
					m_pos.Y = Mortar.Math.MAX(m_pos.Y, m_bonusScreen.m_pos.Y + m_bonusScreen.m_offset.Y + 135f);
					m_scale = m_originalScale * BEGINNING_SCALE * (1f - m_pos.Y / (Game.SCREEN_HEIGHT * 0.7f));
				}
				m_time += dt;
				m_bonusScreen.m_time = m_time;
			}
			break;
		case GOS.GOS_WAIT_TO_DIE:
			Game.QuitToMenu();
			m_state = 7;
			break;
		case GOS.GOS_RESTARTING:
			Game.game_work.gameOverTransition *= 0.75f;
			if (Game.game_work.gameOverTransition < 0.001f)
			{
				WaveManager.GetInstance().Reset();
				Game.game_work.gameOverTransition = 0f;
				Game.game_work.gameOver = false;
				WaveManager.GetInstance().NewGame();
				m_terminate = true;
			}
			if (m_pos.Y < BEGINNING_Y * 0.95f)
			{
				m_pos = new Vector3(0f, NORMAL_Y + (NORMAL_Y - BEGINNING_Y) * (1f - Game.game_work.gameOverTransition), 0f);
			}
			break;
		case GOS.GOS_OUT:
			if (ActorManager.GetInstance().GetNumEntities(0) == 0 || m_leaderboardButton != null)
			{
				Game.game_work.levelStartCoins = Game.game_work.coins;
				WaveManager.GetInstance().Reset();
				Game.game_work.gameOver = true;
				m_state = 4;
				break;
			}
			Game.game_work.gameOverTransition = 1f;
			goto case GOS.GOS_WAIT;
		case GOS.GOS_WAIT:
			if (Game.USE_ARCADE_GO_SCREEN)
			{
				if (m_leaderboardButton == null)
				{
					_ = s_leaderboardButtonTexture;
				}
				if (m_quitButton == null && s_quitButtonTexture != null)
				{
					m_quitButton = new MenuButton(s_quitButtonTexture, QUIT_BUTTON_POS, QuitCallback);
					m_quitButton.Init();
					m_quitButton.m_triggerOnBackPress = true;
					m_quitButton.SetInnerBound(-30f);
					Game.game_work.hud.AddControl(m_quitButton);
				}
			}
			else if (m_fruitFact == null)
			{
				m_fruitFact = new FruitFactControl();
				m_fruitFact.m_pos = m_senseiPos + SENSEI_FACT_OFFSET_X;
				m_fruitFact.m_fruitType = m_fruit;
				m_fruitFact.m_fact = m_fact;
				m_fruitFact.Init();
				Game.game_work.hud.AddControl(m_fruitFact);
			}
			if (Game.game_work.gameOverTransition < 0.999f)
			{
				_ = Game.game_work.gameOverTransition;
				Game.game_work.gameOverTransition += (1f - Game.game_work.gameOverTransition) * 0.125f;
				if (Game.game_work.gameOverTransition >= 0.999f)
				{
					if (!m_hasSetScore)
					{
						int currentScore = Game.game_work.currentScore;
						m_hasSetScore = true;
						Game.game_work.saveData.go_setScore = false;
						uint hash = StringFunctions.StringHash("games");
						uint hash2 = StringFunctions.StringHash("totalscore");
						Game.game_work.saveData.AddToTotal("games", hash, 1);
						if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
						{
							Game.game_work.saveData.AddToTotal("totalscore", hash2, Game.game_work.scoreBeforeBonuses);
							Game.game_work.totalScore -= (uint)(currentScore - Game.game_work.scoreBeforeBonuses);
						}
						else
						{
							Game.game_work.saveData.AddToTotal("totalscore", hash2, currentScore);
						}
						Game.game_work.saveData.UnlockTotals();
						AchievementManager.GetInstance().UnlockScoreAchievement(currentScore);
						AchievementManager.GetInstance().UnlockTotalFruitAchievement((int)Game.game_work.totalScore);
						AchievementManager.GetInstance().UnlockEndScoreAchievement(currentScore, Game.GetCurrentModeHighscore());
						if (m_fruitFact != null && Game.USE_ZEN_GO_SCREEN && m_fruitFact.m_comboStickerType > COMBO_TYPE.CT_NONE && m_fruitFact.m_comboStickerType < COMBO_TYPE.CT_MAX)
						{
							AchievementManager.GetInstance().UnlockComboStarAchievement(m_fruitFact.m_numComboFruits, StringFunctions.StringHash(ComboChecker.GetComboName(m_fruitFact.m_comboStickerType)));
						}
						Game.game_work.saveData.go_showHighScore = false;
						if (currentScore > Game.GetCurrentModeHighscore() / 2)
						{
							Game.game_work.saveData.go_showHighScore = Game.SetCurrentModeHighscore(currentScore);
						}
						Game.game_work.saveData.FinishedGame();
						Game.game_work.saveData.ClearTotals();
						GameTask.SaveCurrentData();
						if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
						{
							TheGame.settings.tf += Game.game_work.scoreBeforeBonuses;
						}
						else
						{
							TheGame.settings.tf += currentScore;
						}
						Leaderboards.Write(4, TheGame.settings.tf);
						if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
						{
							int num = DateTime.Now.DayOfYear / 7;
							int year = DateTime.Now.Year;
							year -= 2000;
							if (year >= 10)
							{
								if (num != TheGame.settings.week)
								{
									TheGame.settings.bestThisWeek = 0;
									TheGame.settings.week = num;
								}
								if (TheGame.settings.bestThisWeek < currentScore)
								{
									TheGame.settings.bestThisWeek = currentScore;
									int num2 = (year << 24) | (num << 16) | currentScore;
									Leaderboards.Write(3, num2);
								}
							}
						}
						TheGame.SaveConfig();
						switch (Game.game_work.gameMode)
						{
						case Game.GAME_MODE.GM_CLASSIC:
							LeaderboardsScreen.SetStartLeaderboard(0);
							break;
						case Game.GAME_MODE.GM_ZEN:
							LeaderboardsScreen.SetStartLeaderboard(1);
							break;
						case Game.GAME_MODE.GM_ARCADE:
							LeaderboardsScreen.SetStartLeaderboard(2);
							break;
						}
						try
						{
						}
						catch (Exception)
						{
						}
					}
					Game.game_work.gameOverTransition = 1f;
					m_state = 2;
					m_retry = new MenuButton(s_retryTexture, RETRY_POS, RetryCallback, 0);
					m_retry.Init();
					Game.game_work.hud.AddControl(m_retry);
					if (Game.USE_ARCADE_GO_SCREEN)
					{
						m_retry.m_clearOthers = false;
						m_retry.m_pos.Y = Game.SCREEN_HEIGHT / 2f - 240f;
						m_retry.m_pos.X = (0f - Game.SCREEN_WIDTH) / 2f + 321f;
						Game.game_work.tutorialControl.ResetTutePos(m_retry);
					}
					else
					{
						MenuButton menuButton = null;
						menuButton = new MenuButton(s_quitTexture, QUIT_POS, QuitCallback, Fruit.MAX_FRUIT_TYPES);
						menuButton.m_originalScale = m_retry.m_originalScale;
						menuButton.Init();
						menuButton.m_triggerOnBackPress = true;
						Game.game_work.hud.AddControl(menuButton);
						Game.game_work.tutorialControl.ResetTutePos(m_retry);
					}
					MenuButton menuButton2 = null;
					_ = Game.USE_ARCADE_GO_SCREEN;
				}
			}
			if (m_pos.Y < NORMAL_Y * 0.95f)
			{
				m_scale = m_originalScale * (BEGINNING_SCALE + (NORMAL_SCALE - BEGINNING_SCALE) * Game.game_work.gameOverTransition);
				m_pos = new Vector3(0f, BEGINNING_Y + (NORMAL_Y - BEGINNING_Y) * Game.game_work.gameOverTransition, 0f);
			}
			break;
		case GOS.GOS_DIE:
			if (Game.game_work.gameOverTransition < 0f)
			{
				m_terminate = true;
			}
			break;
		case GOS.GOS_LEADERBOARDS:
			if (m_leaderbordPressedTime < 1f)
			{
				m_leaderbordPressedTime += dt;
				if (!(m_leaderbordPressedTime >= 1f))
				{
					break;
				}
				if (Game.isWP7TrialMode())
				{
					TheGame.instance.DoUpsell(exitGame: false);
					break;
				}
				PopOverControl.Instance.In(delegate
				{
					LeaderboardsScreen leaderboardsScreen = new LeaderboardsScreen();
					leaderboardsScreen.Init();
					Game.game_work.hud.AddControl(leaderboardsScreen);
				});
			}
			else
			{
				if ((ActorManager.GetInstance().GetNumEntities(0) + ActorManager.GetInstance().GetNumEntities(1) != 0 && m_leaderboardButton == null) || (PopOverControl.State != PopOverControl.POC.OUT && PopOverControl.State != PopOverControl.POC.MOVING_OUT))
				{
					break;
				}
				if (m_time <= 0f)
				{
					m_time = 0f;
				}
				m_time += dt;
				if (m_time > 0.2f)
				{
					m_state = 2;
					if (m_leaderboardButton == null)
					{
						Game.game_work.gameOverTransition = 0.998f;
					}
					m_time = BEGINNING_WAIT + 0.1f;
				}
			}
			break;
		}
		if (m_state != 2 && m_state != 0 && m_retry != null && m_retry.m_scale.X < 50.5f)
		{
			m_retry = null;
		}
		if (Game.USE_ZEN_GO_SCREEN && m_fruitFact != null)
		{
			m_fruitFact.m_pos = new Vector3(ZEN_BOARD_X + 480f * (1f - Game.game_work.gameOverTransition), ZEN_BOARD_Y, 0f);
			m_senseiPos = m_fruitFact.m_pos + Vector3.UnitX * 200f;
		}
		else
		{
			m_senseiPos = new Vector3(SENSEI_OFFSCREEN_X + (SENSEI_CENTER_X - SENSEI_OFFSCREEN_X) * Game.game_work.gameOverTransition, SENSEI_CENTER_Y, 0f);
			if (m_fruitFact != null)
			{
				m_fruitFact.m_pos = m_senseiPos + SENSEI_FACT_OFFSET_X;
			}
		}
		if (m_leaderboardButton != null)
		{
			m_leaderboardButton.m_pos = LEADERBOARD_BUTTON_POS + Vector3.UnitX * (1f - Game.game_work.gameOverTransition) * 120f;
		}
		if (m_quitButton != null)
		{
			m_quitButton.m_pos = QUIT_BUTTON_POS + Vector3.UnitX * (1f - Game.game_work.gameOverTransition) * 120f;
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		if (m_drawOrder == HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT)
		{
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
			if (Game.USE_ARCADE_GO_SCREEN)
			{
				Vector3 val = default(Vector3);
				val = new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 201f, Game.SCREEN_HEIGHT / 2f - 250f - (1f - Game.game_work.gameOverTransition) * Game.SCREEN_WIDTH / 2f, 0f);
				if (s_coinsBundle != null)
				{
					Mortar.Math.Scale44(s_coinsBundle.GetWidth(), s_coinsBundle.GetHeight(), 0f, out var mtx);
					Mortar.Math.GlobalTranslate44(ref mtx, val);
					MatrixManager.GetInstance().SetMatrix(mtx);
					MatrixManager.GetInstance().UploadCurrentMatrices();
					s_coinsBundle.Set();
					Mesh.DrawQuad(Color.White);
					s_coinsBundle.UnSet();
				}
				val.Y += 45f;
				Game.game_work.pNumberFontSilver.DrawString(m_coinsEarnedText, val, Color.White, 20f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
				val.Y -= 83f;
				string stringToDraw = $"{Game.game_work.coins}";
				Game.game_work.pNumberFontSilver.DrawString(stringToDraw, val, Color.White, 34f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
			}
			else if (!Game.USE_ZEN_GO_SCREEN)
			{
				if (m_body > 0)
				{
					m_senseiBody[m_body - 1].Set();
					MatrixManager.GetInstance().Reset();
					MatrixManager.GetInstance().Scale(new Vector3(257f, 257f, 0f));
					MatrixManager.GetInstance().Translate(m_senseiPos);
					MatrixManager.GetInstance().UploadCurrentMatrices();
					Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
					m_senseiBody[m_body - 1].UnSet();
				}
				if (m_head > 0)
				{
					m_senseiHeads[m_head - 1].Set();
					MatrixManager.GetInstance().Reset();
					MatrixManager.GetInstance().Scale(new Vector3(129f, 129f, 0f));
					MatrixManager.GetInstance().Translate(m_senseiPos + SENSEI_HEAD_OFFSET);
					MatrixManager.GetInstance().UploadCurrentMatrices();
					Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
					m_senseiHeads[m_head - 1].UnSet();
				}
			}
			if (m_retry != null && Game.game_work.saveData.highScore > 0)
			{
				string stringToDraw2 = $"{Game.game_work.saveData.highScore}";
				Game.game_work.pNumberFont.DrawString(stringToDraw2, new Vector3(77f - Game.SCREEN_WIDTH / 2f, 0f - Game.SCREEN_HEIGHT / 2f + 64f, 0f), Color.White, m_retry.m_scale.X * 0.5f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
				if (m_oldHighScoreTexture != null && m_retry.m_scale.X > 0f && m_retry.m_scale.X < 600f)
				{
					m_oldHighScoreTexture.Set();
					MatrixManager.GetInstance().Reset();
					MatrixManager.GetInstance().Scale(m_retry.m_scale);
					MatrixManager.GetInstance().Translate(new Vector3(77f - Game.SCREEN_WIDTH / 2f, 0f - Game.SCREEN_HEIGHT / 2f + 64f, 0f));
					MatrixManager.GetInstance().UploadCurrentMatrices();
					Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
					m_oldHighScoreTexture.UnSet();
				}
			}
		}
		else if (m_drawOrder == HUD.HUD_ORDER.HUD_ORDER_NORMAL)
		{
			base.Draw(tintChannels);
		}
	}

	public override void Draw(float[] tintChannels)
	{
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_GAME_OVER_SCREEN;
	}

	public static void LoadContent()
	{
		if (!s_isContentLoaded)
		{
			s_gameOverTexture = TextureManager.GetInstance().Load("gameover.tex", localise: true);
			s_timeUpTexture = TextureManager.GetInstance().Load("time_up.tex", localise: true);
			s_timeUpGreenTexture = TextureManager.GetInstance().Load("arcade_time_up.tex", localise: true);
			s_retryTexture = TextureManager.GetInstance().Load("retry.tex", localise: true);
			s_quitTexture = TextureManager.GetInstance().Load("quit.tex", localise: true);
			for (int i = 0; i < 3; i++)
			{
				string texture = $"textureswp7/sensei_head_0{i + 1}.tex";
				m_senseiHeads[i] = TextureManager.GetInstance().Load(texture);
				texture = $"textureswp7/sensei_body_0{i + 1}.tex";
				m_senseiBody[i] = TextureManager.GetInstance().Load(texture);
			}
			s_isContentLoaded = true;
		}
	}

	public static void UnLoadContent()
	{
		s_gameOverTexture = null;
		s_timeUpTexture = null;
		s_retryTexture = null;
		s_leaderboardTexture = null;
		s_quitTexture = null;
		s_quitButtonTexture = null;
		s_timeUpGreenTexture = null;
		s_leaderboardButtonTexture = null;
		s_coinsBundle = null;
		for (int i = 0; i < 3; i++)
		{
			m_senseiHeads[i] = null;
			m_senseiBody[i] = null;
		}
	}

	public FruitFactControl GetFruitFactControl()
	{
		return m_fruitFact;
	}

	public void SetStateWait()
	{
		int currentScore = Game.game_work.currentScore;
		int num = Game.game_work.saveData.AddToTotal("unrated_games", StringFunctions.StringHash("unrated_games"), 1);
		if (!Game.game_work.saveData.game_rated && num >= 6 && currentScore > 50 && currentScore > Game.GetCurrentModeHighscore() - 10)
		{
			Game.game_work.saveData.game_rated = true;
		}
		else
		{
			m_state = 2;
		}
	}

	public void DeletedBonusScreen(HUDControl control)
	{
		if (control == m_bonusScreen)
		{
			m_bonusScreen = null;
			m_state = 2;
		}
	}

	public void SetWeeklyLeaderboardScore(string leaderboardID, int currentScore)
	{
	}
}
