using System;
using System.Collections.Generic;
using System.Threading;
using FNWP72.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Mortar;

namespace FruitNinja;

public class Game
{
	public delegate void GameInitFunction(uint flags);

	public delegate void GameFunction(float dt, bool update);

	public delegate void GameFunctionDraw(float dt, bool draw);

	public delegate void GameExitFunction();

	public enum TASK
	{
		TASK_SPLASHSCREEN,
		TASK_FRONTEND,
		TASK_GAME_UPDATE,
		TASK_PARTICLES_UPDATE
	}

	public enum Task
	{
		TASK_SPLASHSCREEN,
		TASK_FRONTEND,
		TASK_GAME_UPDATE,
		TASK_PARTICLES_UPDATE
	}

	public enum GAME_MODE
	{
		GM_CLASSIC,
		GM_CASINO,
		GM_ARCADE,
		GM_ZEN,
		GM_MAX
	}

	public class GameWork
	{
		public bool inBonusScreen;

		public Task routine_0;

		public Task routine_1;

		public bool pause;

		public StringTableUtils.Language language;

		public StringTableUtils.FontFamily fontFamily;

		public GAME_MODE gameMode;

		public bool gameOver;

		public bool inRetrySequence;

		public float retryTimer;

		public float gameOverTransition;

		public float hitBombTime;

		public byte currentMissCount;

		public int currentScore;

		public int scoreBeforeBonuses;

		public bool hasDroppedFruit;

		public int coins;

		public int coinsTotal;

		public int levelStartCoins;

		public float critHitTime;

		public int criticalChance;

		public bool canFastForward;

		public float dt;

		public HUD hud;

		public HUD menu;

		public bool soundEnabled;

		public bool musicEnabled;

		public FruitCamera camera;

		public FruitSaveData saveData;

		public Font pDebugFont;

		public Font pGameFont;

		public Font pNumberFont;

		public Font pNumberFontSilver;

		public Font pNumberFontGreen;

		public Font pNumberFontLeaderboard;

		public Font pNumberFontBlue2;

		public bool fullAmbience;

		public bool loadedSaveState;

		public float bombSize;

		public float bombCollision;

		public Vector3 mainPointer;

		public bool pointerPressed;

		public bool pointerReleased;

		public bool pointerDown;

		public Vector3[] touchPositions = new Vector3[16];

		public MainScreen mainScreen;

		public GameOverScreen gameOverScreen;

		public TutorialControl tutorialControl;

		public uint totalScore;

		public Texture backTexture;

		public TimeControl timeControl;

		public uint gameSeedValue;

		public float gameTime;

		public void SetLanguage(StringTableUtils.Language language)
		{
			this.language = language;
			fontFamily = StringTableUtils.GetFontFamily(this.language);
		}
	}

	public delegate int ScoreDelegate(int gdfh);

	public const int HEAP_SIZE_MESH = 158720;

	public const int HEAP_SIZE_ANIMATION = 512000;

	public const int HEAP_SIZE_TEXTURE = 51200;

	public const int SAVE_SIZE = 160;

	public const int DEFAULT_MUSIC_VOLUME = 60;

	public const int DEFAULT_SFX_VOLUME = 80;

	public static int MAX_SYSTEM_ACHIEVEMENTS = 30;

	private static uint[] names = new uint[4]
	{
		StringFunctions.StringHash("CLASSIC"),
		StringFunctions.StringHash("CASINO"),
		StringFunctions.StringHash("ARCADE"),
		StringFunctions.StringHash("ZEN")
	};

	private static bool s_trialModeState = false;

	private static int version_major = 1;

	private static int version_minor = 0;

	private static int version_patch = 0;

	public static readonly float SCREEN_WIDTH = 480f;

	public static readonly float SCREEN_HEIGHT = 320f;

	public static readonly float SCREEN_SIZE_X = 800f;

	public static readonly float SCREEN_SIZE_Y = 480f;

	public static readonly float HUD_SCALE = 1f;

	public static readonly float GAME_MODE_SCALE_FIX = 1f;

	public static float GRAVITY = 0.2f;

	private static GameInitFunction[] task_init = new GameInitFunction[3]
	{
		SplashInit,
		FrontendInit,
		GameTask.GameInit
	};

	private static GameFunction[] task_main = new GameFunction[3]
	{
		SplashUpdate,
		FrontendUpdate,
		GameTask.GameUpdate
	};

	private static GameFunctionDraw[] task_draw = new GameFunctionDraw[3]
	{
		SplashDraw,
		FrontendDraw,
		GameTask.GameDraw
	};

	private static GameExitFunction[] task_exit = new GameExitFunction[3]
	{
		SplashExit,
		FrontendExit,
		GameTask.GameExit
	};

	private static Task old_routine = Task.TASK_SPLASHSCREEN;

	private static bool task_initialised = false;

	private static bool updated = false;

	private static float drawDt = 0f;

	public static bool trialModeEnded = false;

	private static bool FIRST = true;

	private static ScoreDelegate s_scoreDelagate = DefaultScoreDelegate;

	private static float GetFruitZPositionz = -500f;

	private static float GetBombZPositionz = -10f;

	public static GameWork game_work = new GameWork();

	private static Color DefaultBackgroundColor = Color.Black;

	public static float PLATFORM_IPHONEOS_RES_SCALE => 1f;

	public static float PSP_SCREEN_WIDTH => 480f;

	public static float PSP_SCREEN_HEIGHT => 272f;

	public static float PC_SCREEN_WIDTH => SCREEN_WIDTH;

	public static float PC_SCREEN_HEIGHT => SCREEN_HEIGHT;

	public static bool USE_ARCADE_GO_SCREEN => false;

	public static bool USE_ZEN_GO_SCREEN
	{
		get
		{
			if (game_work.gameMode != GAME_MODE.GM_ZEN)
			{
				return game_work.gameMode == GAME_MODE.GM_ARCADE;
			}
			return true;
		}
	}

	public static float SCREEN_SCALE_X => SCREEN_WIDTH / SCREEN_SIZE_X;

	public static float SCREEN_SCALE_Y => SCREEN_HEIGHT / SCREEN_SIZE_Y;

	public static float SPLIT_SCREEN_SCALE => SCREEN_WIDTH * 0.5f / SCREEN_HEIGHT;

	public static float SPLIT_SCREEN_SCALE_INV => SCREEN_HEIGHT / (SCREEN_WIDTH * 0.5f);

	public static float SPLIT_SCREEN_WIDTH => SCREEN_HEIGHT / SPLIT_SCREEN_SCALE;

	public static float SPLIT_SCREEN_HIEGHT => SCREEN_HEIGHT;

	public static float SCREEN_LEFT => SCREEN_TO_PLATFORMX(-0.5f);

	public static float SCREEN_RIGHT => SCREEN_TO_PLATFORMX(0.5f);

	public static float SCREEN_TOP => SCREEN_TO_PLATFORMY(0.5f);

	public static float SCREEN_BOTTOM => SCREEN_TO_PLATFORMY(-0.5f);

	public static float SCREEN_TOP_LEFTX => SCREEN_TO_PLATFORMX(-0.5f);

	public static float SCREEN_TOP_LEFTY => SCREEN_TO_PLATFORMY(0.5f);

	public static float SCREEN_TOP_RIGHTX => SCREEN_TO_PLATFORMX(0.5f);

	public static float SCREEN_TOP_RIGHTY => SCREEN_TO_PLATFORMY(0.5f);

	public static float SCREEN_BOTTOM_LEFTX => SCREEN_TO_PLATFORMX(0f);

	public static float SCREEN_BOTTOM_LEFTY => SCREEN_TO_PLATFORMY(-0.5f);

	public static float SCREEN_BOTTOM_RIGHTX => SCREEN_TO_PLATFORMX(0.5f);

	public static float SCREEN_BOTTOM_RIGHTY => SCREEN_TO_PLATFORMY(-0.5f);

	public static float SCREEN_CENTREX => SCREEN_TO_PLATFORMX(0f);

	public static float SCREEN_CENTREY => SCREEN_TO_PLATFORMY(0f);

	public static float GAME_FONT_SIZE => 16f;

	public static float GAME_FONT_PIXEL_SIZE => 14f;

	public static float PETITA_BIG_SIZE => 32f;

	public static float PETITA_SMALL_SIZE => 22f;

	public static float DEFAULT_HUD_SPACING => 8f;

	public static int MAX_PLAYERS => 1;

	public static byte MAX_FRUIT_MISSES => 3;

	public static int HEAP_SIZE_SOUND => 524288;

	public static float HIT_BOMB_WAIT => 3.2f;

	public static Color TintColour(Color col, float[] tints)
	{
		col.R = (byte)Mortar.Math.CLAMP((float)(int)col.R * tints[0], 0f, 255f);
		col.G = (byte)Mortar.Math.CLAMP((float)(int)col.G * tints[1], 0f, 255f);
		col.B = (byte)Mortar.Math.CLAMP((float)(int)col.B * tints[2], 0f, 255f);
		return col;
	}

	public static Color TintWhite(float[] tints)
	{
		return new Color((int)(byte)Mortar.Math.CLAMP(255f * tints[0], 0f, 255f), (int)(byte)Mortar.Math.CLAMP(255f * tints[1], 0f, 255f), (int)(byte)Mortar.Math.CLAMP(255f * tints[2], 0f, 255f), 255);
	}

	public static uint GetModeBitMask(GAME_MODE mode)
	{
		if (mode == GAME_MODE.GM_MAX)
		{
			return uint.MaxValue;
		}
		return (uint)(1 << (int)mode);
	}

	public static GAME_MODE ParseGameMode(uint hash)
	{
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i] == hash)
			{
				return (GAME_MODE)i;
			}
		}
		return GAME_MODE.GM_MAX;
	}

	public static uint ParseModeMask(string text)
	{
		uint num = 0u;
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = "";
			int num2 = 0;
			while (num2 < text.Length)
			{
				char c = text[num2++];
				if (c == ',')
				{
					GAME_MODE mode = ParseGameMode(StringFunctions.StringHash(text2));
					num |= GetModeBitMask(mode);
					text2 = "";
				}
				else
				{
					text2 += c;
				}
			}
			if (text2.Length > 0)
			{
				GAME_MODE mode2 = ParseGameMode(StringFunctions.StringHash(text2));
				num |= GetModeBitMask(mode2);
			}
		}
		else
		{
			num = GetModeBitMask(GAME_MODE.GM_MAX);
		}
		return num;
	}

	public static int GetCurrentScore()
	{
		return GetCurrentScore(0);
	}

	public static int GetCurrentScore(int player)
	{
		return game_work.currentScore;
	}

	public static void SetTrialModeState()
	{
		try
		{
			s_trialModeState = false;
		}
		catch
		{
			s_trialModeState = true;
		}
	}

	public static bool isWP7TrialMode()
	{
		return s_trialModeState;
	}

	public static void ShowBuyMessageBox()
	{
		try
		{
			string text = TheGame.instance.stringTable.GetString(931);
			string text2 = TheGame.instance.stringTable.GetString(926);
			string text3 = TheGame.instance.stringTable.GetString(1144);
			string text4 = TheGame.instance.stringTable.GetString(1145);
			string[] array = new string[2] { text2, text };
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			if (!Guide.IsVisible)
			{
				Guide.BeginShowMessageBox(text3, text4, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)ExitGameCallback, (object)null);
			}
		}
		catch
		{
		}
	}

	private static void ExitGameCallback(IAsyncResult result)
	{
	}

	public static float PIXEL_SCREEN_X(float x)
	{
		return SCREEN_SCALE_X * x;
	}

	public static float PIXEL_SCREEN_Y(float y)
	{
		return SCREEN_SCALE_Y * y;
	}

	public static float SCREEN_TO_PLATFORMX(float x)
	{
		return x * (float)DisplayManager.GetInstance().GetWindowSize().right;
	}

	public static float SCREEN_TO_PLATFORMY(float y)
	{
		return y * (float)DisplayManager.GetInstance().GetWindowSize().bottom;
	}

	public static float PSP_PIXEL_SCREENX(float x)
	{
		return x / PSP_SCREEN_WIDTH;
	}

	public static float PSP_PIXEL_SCREENY(float x)
	{
		return x / PSP_SCREEN_HEIGHT;
	}

	public static float PC_PIXEL_SCREENX(float x)
	{
		return x / PC_SCREEN_WIDTH;
	}

	public static float PC_PIXEL_SCREENY(float x)
	{
		return x / PC_SCREEN_HEIGHT;
	}

	public static float PIXEL_SCREENX(float x)
	{
		return x / SCREEN_WIDTH;
	}

	public static float PIXEL_SCREENY(float y)
	{
		return y / SCREEN_HEIGHT;
	}

	public static float PIXEL_PLATFORMX(float x)
	{
		return SCREEN_TO_PLATFORMX(PIXEL_SCREENX(x));
	}

	public static float PIXEL_PLATFORMY(float y)
	{
		return SCREEN_TO_PLATFORMY(PIXEL_SCREENX(y));
	}

	public static float PSP_PIXEL_PLATFORMX(float x)
	{
		return SCREEN_TO_PLATFORMX(PSP_PIXEL_SCREENX(x));
	}

	public static float PSP_PIXEL_PLATFORMY(float y)
	{
		return SCREEN_TO_PLATFORMY(PSP_PIXEL_SCREENY(y));
	}

	public static float PC_PIXEL_PLATFORMX(float x)
	{
		return SCREEN_TO_PLATFORMX(PC_PIXEL_SCREENX(x));
	}

	public static float PC_PIXEL_PLATFORMY(float y)
	{
		return SCREEN_TO_PLATFORMY(PC_PIXEL_SCREENY(y));
	}

	public static void OnActivate()
	{
		if (game_work.gameOverTransition == 0f)
		{
			if (game_work.hud != null)
			{
				game_work.hud.OnPause();
			}
			GameTask.SkipToPause(atStart: false);
		}
	}

	public static void ParticleGameInit(uint flags)
	{
	}

	public static void ParticleGameUpdate(float dt, bool update)
	{
	}

	public static void ParticleGameDraw(float dt, bool draw)
	{
	}

	public static void ParticleGameExit()
	{
	}

	public static void SplashInit(uint flags)
	{
	}

	public static void SplashUpdate(float dt, bool update)
	{
	}

	public static void SplashDraw(float dt, bool draw)
	{
	}

	public static void SplashExit()
	{
	}

	public static void FrontendInit(uint flags)
	{
	}

	public static void FrontendUpdate(float dt, bool update)
	{
	}

	public static void FrontendDraw(float dt, bool draw)
	{
	}

	public static void FrontendExit()
	{
	}

	public static void SetLanguage()
	{
	}

	public static int GetCurrentModeHighscore()
	{
		if (game_work.gameMode >= GAME_MODE.GM_CLASSIC && game_work.gameMode < GAME_MODE.GM_MAX && game_work.saveData != null)
		{
			return game_work.saveData.highScores[(int)game_work.gameMode];
		}
		return 0;
	}

	public static int GetCurrentModeHighscore(int mode)
	{
		if (mode >= 0 && mode < 4 && game_work.saveData != null)
		{
			return game_work.saveData.highScores[mode];
		}
		return 0;
	}

	public static bool SetCurrentModeHighscore(int score)
	{
		if (game_work.gameMode >= GAME_MODE.GM_CLASSIC && game_work.gameMode < GAME_MODE.GM_MAX && game_work.saveData != null && score > game_work.saveData.highScores[(int)game_work.gameMode])
		{
			game_work.saveData.highScores[(int)game_work.gameMode] = score;
			return true;
		}
		return false;
	}

	public static bool CombosEnabled()
	{
		return false;
	}

	public static void AddToCurrentScore(int score, int player, bool addToTotal, bool sendNotification)
	{
		AddToCurrentScore(score, 0);
	}

	public static void AddToCurrentScore(int score)
	{
		AddToCurrentScore(score, 0);
	}

	public static void AddToCurrentScore(int score, int player)
	{
		int currentScore = game_work.currentScore;
		if (isWP7TrialMode() && currentScore + score >= 150)
		{
			trialModeEnded = true;
			GameOver();
			score = 150 - currentScore;
			TheGame.TriggerShowBuyMessageBox = true;
		}
		int num = (game_work.currentScore += s_scoreDelagate(score * GetScoreMultiplyer()));
		if (num < 0)
		{
			num = (game_work.currentScore = 0);
		}
		if (currentScore / Fruit.NEW_LIFE_AT < num / Fruit.NEW_LIFE_AT && game_work.currentMissCount > 0)
		{
			game_work.currentMissCount--;
			SoundManager.GetInstance().SFXPlay(SoundDef.SND_EXTRA_LIFE);
		}
		uint hash = StringFunctions.StringHash("all");
		if (score > 0)
		{
			game_work.totalScore = (uint)game_work.saveData.AddToTotal("all", hash, score, universal: true, achievementUpload: false);
		}
	}

	public static void AddCoins(int coins)
	{
	}

	public static void HitBomb(Vector3 pos)
	{
		GameTask.HitBomb(pos);
	}

	public static void HitMenuBomb(Vector3 pos)
	{
		GameTask.HitMenuBomb(pos);
	}

	public static bool BombFlashFull()
	{
		if (game_work.hitBombTime < GameTask.BOMB_FLASH_FULL)
		{
			return game_work.hitBombTime < GameTask.BOMB_FLASH_START_FADE;
		}
		return false;
	}

	public static void GameOver()
	{
		GameOver(-1, -1f, -1);
	}

	public static void GameOver(int state, float time, int player)
	{
		if (!game_work.gameOver)
		{
			game_work.gameOver = true;
			WaveManager.GetInstance().ClearUnspawned();
			game_work.gameOverScreen = new GameOverScreen(null, state, time, game_work.saveData.go_head, game_work.saveData.go_body, game_work.saveData.go_fruit, game_work.saveData.go_fact);
			game_work.saveData.go_head = (game_work.saveData.go_body = (game_work.saveData.go_fruit = (game_work.saveData.go_fact = -1)));
			game_work.gameOverScreen.Init();
			game_work.hud.AddControl(game_work.gameOverScreen);
		}
	}

	public static void QuitToMenu()
	{
		game_work.gameOver = true;
		GameTask.s_mainScreen.m_state = MainScreen.MS.MS_IN;
		GameTask.s_mainScreen.m_transitionWait = 0.5f;
		game_work.currentScore = 0;
		SuperFruitControl.StopAllPomegranates();
		SuperFruitControl.Clear();
		TheGame.pomegranateThrown = false;
		WaveManager.GetInstance().SetAbsoluteDtMod(1f);
		WaveManager.GetInstance().paused = false;
		LinkedListNode<Entity> iterator = null;
		for (Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); fruit != null; fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			fruit.m_dormant = true;
			fruit.m_destroy = true;
		}
	}

	public static void ClearMenuItems()
	{
		LinkedListNode<Entity> iterator = null;
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			if ((((Fruit)entity).partOfPopup || !PopOverControl.IsInPopup) && !((Fruit)entity).m_isSliced)
			{
				((Fruit)entity).m_isSliced = true;
				((Fruit)entity).m_vel = new Vector3(Mortar.Math.g_random.RandF(10f) - 5f, Mortar.Math.g_random.RandF(5f), 0f);
				entity.m_vel.X = System.Math.Abs(entity.m_vel.X) * (float)Mortar.Math.MATH_SIGN(entity.m_pos.X);
				((Fruit)entity).m_vel2 = entity.m_vel;
			}
		}
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			if (((Bomb)entity).partOfPopup || !PopOverControl.IsInPopup)
			{
				if (((Bomb)entity).Enabled())
				{
					((Bomb)entity).Disable();
					entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(10f) - 5f, Mortar.Math.g_random.RandF(5f), 0f);
				}
				((Bomb)entity).EnableGravity(state: true);
			}
		}
	}

	public static void SetScoreDelegate(ScoreDelegate del)
	{
		s_scoreDelagate = del;
	}

	public static int DefaultScoreDelegate(int score)
	{
		if (game_work.gameMode == GAME_MODE.GM_ARCADE)
		{
			score = ((score <= 0) ? (score * PowerUpManager.GetInstance().GetScoreLossMultiplier()) : (score * PowerUpManager.GetInstance().GetScoreGainMultiplier()));
		}
		return score;
	}

	public static void GameTaskUpdate(float dt)
	{
		drawDt += dt;
		game_work.dt = dt;
		game_work.gameSeedValue += (uint)(dt * 1000f);
		if (task_initialised)
		{
			if (game_work.routine_0 != old_routine)
			{
				Task routine_ = game_work.routine_0;
				game_work.routine_0 = routine_;
				old_routine = game_work.routine_0;
				return;
			}
			bool update = !game_work.pause;
			updated = true;
			old_routine = game_work.routine_0;
			if (game_work.hitBombTime <= 0f)
			{
				game_work.saveData.Update(dt, game_work.hud);
			}
			task_main[(int)game_work.routine_0](dt, update);
		}
		else
		{
			old_routine = game_work.routine_0;
			task_init[(int)game_work.routine_0](0u);
			task_initialised = true;
			if (FIRST)
			{
				FIRST = false;
				GameTaskUpdate(dt);
			}
		}
	}

	public static void GameTaskDraw(float dt)
	{
		game_work.dt = drawDt;
		if (game_work.routine_0 == old_routine && updated)
		{
			task_draw[(int)game_work.routine_0](drawDt, draw: true);
		}
		drawDt = 0f;
	}

	public static void GameTaskExit()
	{
	}

	public static bool PointerMoveCallback(InputEvent e)
	{
		return false;
	}

	public static bool PointerDownCallback(InputEvent e)
	{
		return false;
	}

	public static bool PointerUpCallback(InputEvent e)
	{
		return false;
	}

	public static int TouchInRegion(float xMin, float xMax, float yMin, float yMax)
	{
		return TouchInRegion(xMin, xMax, yMin, yMax, -1);
	}

	public static int TouchInRegion(float xMin, float xMax, float yMin, float yMax, int touch)
	{
		if (touch >= 0 && touch < GameTask.MAX_SLASHES && game_work.touchPositions[touch].Z > 0f && game_work.touchPositions[touch].X >= xMin && game_work.touchPositions[touch].X <= xMax && game_work.touchPositions[touch].Y >= yMin && game_work.touchPositions[touch].Y <= yMax)
		{
			return touch;
		}
		for (int i = 0; i < GameTask.MAX_SLASHES; i++)
		{
			if (game_work.touchPositions[i].Z > 0f && game_work.touchPositions[i].X >= xMin && game_work.touchPositions[i].X <= xMax && game_work.touchPositions[i].Y >= yMin && game_work.touchPositions[i].Y <= yMax)
			{
				return i;
			}
		}
		return -1;
	}

	public static int IsTouchDown(int touch)
	{
		if (touch >= 0 && touch < GameTask.MAX_SLASHES)
		{
			if (game_work.touchPositions[touch].Z == 1f)
			{
				return 1;
			}
			if (game_work.touchPositions[touch].Z == 2f)
			{
				return 2;
			}
		}
		return 0;
	}

	public static string GetFormattedVersionString()
	{
		return null;
	}

	public static string GetVersionString()
	{
		return version_major.ToString() + "." + version_minor + "." + version_patch;
	}

	public static int GetVersionTotal()
	{
		return version_major * 10000 + version_minor * 100 + version_patch;
	}

	public static int GetVersionMajor()
	{
		return version_major;
	}

	public static int GetVersionMinor()
	{
		return version_minor;
	}

	public static int GetVersionPatch()
	{
		return version_patch;
	}

	public static bool IsFastHardware()
	{
		return true;
	}

	public static void MoveFruitZPositionToBack(ref float z)
	{
		z += 500f;
		z /= 2f;
		z -= 2600f;
	}

	public static float GetFruitZPosition()
	{
		GetFruitZPositionz -= 100f;
		if (GetFruitZPositionz < -2499f)
		{
			GetFruitZPositionz = -500f;
		}
		return GetFruitZPositionz;
	}

	public static float GetBombZPosition()
	{
		GetBombZPositionz -= 50f;
		if (GetBombZPositionz < -400f)
		{
			GetBombZPositionz = -10f;
		}
		return GetBombZPositionz;
	}

	public static bool InViewer()
	{
		return false;
	}

	public static void GameDestroy()
	{
	}

	private static void InitialiseData()
	{
		StringTableUtils.StringTableUtilInit();
		StringTableUtils.StringTableUtilLoadStrings();
		game_work.loadedSaveState = false;
		game_work.gameMode = GAME_MODE.GM_CLASSIC;
		game_work.saveData = new FruitSaveData();
		Save.LoadGame(game_work.saveData);
		game_work.gameMode = (GAME_MODE)game_work.saveData.mode;
		game_work.saveData.AddToTotal("sessions", StringFunctions.StringHash("sessions"), 1, universal: true, achievementUpload: true);
		game_work.routine_0 = Task.TASK_GAME_UPDATE;
		game_work.bombSize = 50f;
		game_work.hitBombTime = 0f;
		game_work.critHitTime = 0f;
		game_work.criticalChance = game_work.saveData.criticalProgression;
		game_work.hasDroppedFruit = false;
		game_work.inRetrySequence = false;
		game_work.gameOverScreen = null;
		game_work.soundEnabled = game_work.saveData.GetTotal(StringFunctions.StringHash("soundOff")) == 0;
		SoundManager.GetInstance().SetSFXVolume(game_work.soundEnabled ? SoundDef.DEFAULT_SFX_VOL : 0f);
		game_work.musicEnabled = game_work.saveData.GetTotal(StringFunctions.StringHash("musicOff")) == 0;
		game_work.saveData.AddToTotal("soundOff", StringFunctions.StringHash("soundOff"), -game_work.saveData.GetTotal(StringFunctions.StringHash("soundOff")), universal: false, achievementUpload: true);
		game_work.saveData.AddToTotal("musicOff", StringFunctions.StringHash("musicOff"), -game_work.saveData.GetTotal(StringFunctions.StringHash("musicOff")), universal: false, achievementUpload: true);
		SlashEntity.InitModColors();
		AchievementManager.GetInstance().LoadAchievementInfo();
		game_work.coins = 0;
		game_work.coinsTotal = 0;
		game_work.levelStartCoins = 0;
		ItemManager.GetInstance().LoadItemData();
		BonusManager.GetInstance().Init();
	}

	public static void GameInitialise(uint flags)
	{
		SystemManager.GetInstance().Init();
		MatrixManager.GetInstance().Init();
		DisplayManager.GetInstance().SetWindowSize(0, (int)SCREEN_SIZE_Y, 0, (int)SCREEN_SIZE_X);
		DisplayManager.GetInstance().Init("Rocket Racing");
		DisplayManager.GetInstance().SetClearColor(DefaultBackgroundColor);
		DisplayManager.GetInstance().SetLightDirection(new Vector3(0f, -10f, -5f));
		TextureManager.GetInstance().Initialise();
		TextureManager.GetInstance().Initialise(51200);
		MeshManager.GetInstance().Initialise(158720);
		AnimationManager.GetInstance().Initialise(512000);
		InputManager.GetInstance().Init();
		PSPParticleManager.GetInstance().LoadFile("particles", "particles/particles_slow.xml");
		PowerUpManager.GetInstance().Load();
		InitialiseData();
		game_work.camera = new FruitCamera();
		game_work.pointerReleased = false;
		game_work.pointerPressed = false;
		game_work.pointerDown = false;
		game_work.mainPointer = Vector3.Zero;
		game_work.timeControl = null;
		game_work.camera.Init(1f, 10000f, 16.95f, 11.3f);
		if (!MTLocalisation.FontsLoaded)
		{
			MTLocalisation.LoadFonts();
		}
		if (game_work.pGameFont == null)
		{
			game_work.pGameFont = MTLocalisation.GetCachedFont(game_work.language);
		}
		if (game_work.pNumberFont == null)
		{
			game_work.pNumberFont = new Font();
			game_work.pNumberFont.Load("fonts/fruit_ninja_numbers.fnt");
		}
		if (game_work.pNumberFontLeaderboard == null)
		{
			game_work.pNumberFontLeaderboard = new Font();
			game_work.pNumberFontLeaderboard.Load("fonts/arcade_results_numbers.fnt");
		}
		if (game_work.pNumberFontBlue2 == null)
		{
			game_work.pNumberFontBlue2 = new Font();
			game_work.pNumberFontBlue2.Load("fonts/fruit_ninja_numbers_blue2.fnt");
		}
		if (game_work.pNumberFontGreen == null)
		{
			game_work.pNumberFontGreen = new Font();
			game_work.pNumberFontGreen.Load("fonts/fruit_ninja_numbers_green.fnt");
		}
		game_work.backTexture = TextureManager.GetInstance().Load("back_icon.tex", localise: true);
		MenuButton.LoadContent();
		Fruit.LoadInfo();
		SplatEntity.LoadContent();
		SlashEntity.LoadContent();
		Bomb.LoadContent();
		GameOverScreen.LoadContent();
		DojoScreen.LoadContent();
		FruitFactControl.LoadContent();
		AboutScreen.LoadContent();
		GameModeScreen.LoadContent();
		Coin.LoadContent();
		CreditsScreen.LoadContent();
		LeaderboardsScreen.LoadContent();
		AchievementsScreen.LoadContent();
		SuperFruitControl.LoadContent();
	}

	public static void SetScoreDelegate()
	{
		SetScoreDelegate(DefaultScoreDelegate);
	}

	public static bool FailureEnabled()
	{
		if (game_work.gameMode != GAME_MODE.GM_ZEN)
		{
			return game_work.gameMode != GAME_MODE.GM_ARCADE;
		}
		return false;
	}

	public static bool CoinsEnabled()
	{
		return false;
	}

	public static bool PowersEnabled()
	{
		return game_work.gameMode == GAME_MODE.GM_ARCADE;
	}

	public static int GetScoreMultiplyer()
	{
		return GetScoreMultiplyer(0);
	}

	public static int GetScoreMultiplyer(int player)
	{
		return 1;
	}

	public static float TAILY_TRANSITION_TO(float val, float valTo, float dt, float friction)
	{
		return valTo + (val - valTo) * (float)System.Math.Pow(friction, dt * 60f);
	}

	public static bool IsMultiplayer()
	{
		return false;
	}
}
