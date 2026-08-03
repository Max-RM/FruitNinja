using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class GameModeScreen : HUDControl3d
{
	public enum GMS
	{
		GMS_IN,
		GMS_WAIT,
		GMS_CLASSIC,
		GMS_CASINO,
		GMS_ARCADE,
		GMS_ZEN,
		GMS_OUT
	}

	protected float m_time;

	protected Vector3 m_originalScale;

	protected MenuButton m_quitButton;

	protected float m_comingSoonRespawnTime1;

	protected float m_comingSoonRespawnTime2;

	protected MenuButton m_comingSoon1;

	protected MenuButton m_comingSoon2;

	protected float m_infoBoardTime;

	protected bool m_isMultiplayer;

	protected HUD.HUD_ORDER m_backOrder;

	public GMS m_state;

	private static Texture s_classicTexture = null;

	private static Texture s_zenTexture = null;

	private static Texture s_arcadeTexture = null;

	private static Texture s_titleTexture = null;

	private static Texture s_senseiTexture = null;

	private static Texture s_modeselectTexture = null;

	private static Texture s_zenBoardTexture = null;

	private bool RESPOAWWEN;

	private static Vector3 Updatescale;

	private static bool firstRender = false;

	private static GameVertex[] top_tri = ArrayInit.CreateFilledArray<GameVertex>(3);

	private static GameVertex[] btm_tri = ArrayInit.CreateFilledArray<GameVertex>(3);

	public static Vector3 CLASSIC_MODE_POS => new Vector3(-70f, 71f, 0f);

	public static Vector3 ZEN_MODE_POS => new Vector3(88f, 48f, 0f);

	public static Vector3 INTENSE_MODE_POS => new Vector3(19f, -76f, 0f);

	public static Vector3 COMING_SOON_1_POS => new Vector3(12f, -72f, 0f);

	public static Vector3 COMING_SOON_2_POS => new Vector3(90f, -75f, 0f);

	public static Vector3 ZEN_BOARD_POS_START => new Vector3(314f, 14f, 10f);

	public static Vector3 ZEN_BOARD_POS_FINISH => new Vector3(194f, 29f, 10f);

	public static Vector3 MODE_SELECT_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 125f, (0f - Game.SCREEN_HEIGHT) / 2f + 30f, 0f);

	public static Vector3 TITLE_POS => new Vector3(Game.SCREEN_WIDTH / 2f - 58f, Game.SCREEN_HEIGHT / 2f - 23f, 0f);

	public static Vector3 SENSEI_SELECT_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 52f, (0f - Game.SCREEN_HEIGHT) / 2f + 128f, 0f);

	public GameModeScreen()
		: this(isMultiplayer: false)
	{
	}

	public static void LoadContent()
	{
		s_titleTexture = TextureManager.GetInstance().Load("textureswp7/sml_title.tex");
		s_senseiTexture = TextureManager.GetInstance().Load("textureswp7/mode_sensei.tex");
		s_modeselectTexture = TextureManager.GetInstance().Load("mode_select.tex", localise: true);
		s_classicTexture = TextureManager.GetInstance().Load("classic.tex", localise: true);
		s_zenTexture = TextureManager.GetInstance().Load("mode_2.tex", localise: true);
		s_arcadeTexture = TextureManager.GetInstance().Load("arcade_mode.tex", localise: true);
		s_zenBoardTexture = TextureManager.GetInstance().Load("zen_sign.tex", localise: true);
	}

	public static void UnLoadContent()
	{
		s_classicTexture = null;
		s_zenTexture = null;
		s_arcadeTexture = null;
		s_titleTexture = null;
		s_senseiTexture = null;
		s_modeselectTexture = null;
		s_zenBoardTexture = null;
	}

	public GameModeScreen(bool isMultiplayer)
	{
		m_isMultiplayer = isMultiplayer;
		m_texture = null;
		m_infoBoardTime = -2.5f;
		m_selfCleanUp = false;
		m_quitButton = null;
		m_comingSoon1 = null;
		m_comingSoon2 = null;
		m_state = GMS.GMS_IN;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
		m_backOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		m_time = 0f;
		RESPOAWWEN = false;
	}

	public override void Init()
	{
		Reset();
	}

	public override void Reset()
	{
	}

	public override void Release()
	{
		m_texture = null;
		m_time = 0f;
	}

	public override void Update(float dt)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		if (ActorManager.GetInstance().GetNumEntities(0) == 0 && RESPOAWWEN)
		{
			RESPOAWWEN = false;
			m_time = 0f;
			m_state = GMS.GMS_IN;
			TheGame.instance.DoUpsell(exitGame: false);
		}
		float time = m_time;
		switch (m_state)
		{
		case GMS.GMS_IN:
			m_time += (1f - m_time) * 0.15f;
			if (m_time > 0.999f)
			{
				m_state = GMS.GMS_WAIT;
				m_time = 1f;
				m_quitButton = new MenuButton("back_icon.tex", new Vector3(Game.SCREEN_WIDTH / 2f - 45f, (0f - Game.SCREEN_HEIGHT) / 2f + 50f, 0f), QuitCallback, Fruit.MAX_FRUIT_TYPES, Vector3.Zero, localise: true);
				m_quitButton.Init();
				m_quitButton.m_triggerOnBackPress = true;
				Game.game_work.hud.AddControl(m_quitButton);
				MenuButton quitButton = m_quitButton;
				quitButton.m_originalScale *= 0.75f;
				Entity entity = m_quitButton.m_entity;
				entity.m_cur_scale *= 0.75f;
				MenuButton menuButton = new MenuButton(s_classicTexture, CLASSIC_MODE_POS, ClassicModeCallback, Fruit.FruitType("watermelon"));
				Game.game_work.tutorialControl.ResetTutePos(menuButton);
				menuButton.Init();
				Game.game_work.hud.AddControl(menuButton);
				MenuButton menuButton2 = menuButton;
				menuButton2.m_originalScale *= 0.9f;
				Entity entity2 = menuButton.m_entity;
				entity2.m_cur_scale *= 0.95f;
				Updatescale = menuButton.m_originalScale * 0.85f;
				menuButton = new MenuButton(s_zenTexture, ZEN_MODE_POS, ZenModeCallback, Fruit.FruitType("apple_red"));
				menuButton.m_isTrialLockable = true;
				menuButton.Init();
				menuButton.m_originalScale = Updatescale;
				Entity entity3 = menuButton.m_entity;
				entity3.m_cur_scale *= 0.9f;
				Game.game_work.hud.AddControl(menuButton);
				menuButton = new MenuButton(s_arcadeTexture, COMING_SOON_1_POS, ArcadeCallback, Fruit.FruitType("banana"));
				menuButton.m_isTrialLockable = true;
				menuButton.Init();
				menuButton.m_originalScale = Updatescale;
				Entity entity4 = menuButton.m_entity;
				entity4.m_cur_scale *= 0.75f;
				Game.game_work.hud.AddControl(menuButton);
				m_comingSoonRespawnTime1 = -1f;
				m_comingSoonRespawnTime2 = -1f;
			}
			break;
		case GMS.GMS_WAIT:
			m_infoBoardTime += Mortar.Math.CLAMP((1f - m_infoBoardTime) * 0.25f, -0.1f, 0.1f);
			if (m_comingSoonRespawnTime1 > 0f)
			{
				m_comingSoonRespawnTime1 -= dt;
				if (m_comingSoonRespawnTime1 <= 0f)
				{
					m_comingSoonRespawnTime1 = -1f;
				}
			}
			if (m_comingSoonRespawnTime1 < 0f)
			{
				_ = m_comingSoon1;
			}
			if (m_comingSoonRespawnTime2 > 0f)
			{
				m_comingSoonRespawnTime2 -= dt;
				if (m_comingSoonRespawnTime2 <= 0f)
				{
					m_comingSoonRespawnTime2 = -1f;
				}
			}
			break;
		case GMS.GMS_OUT:
			m_time *= 0.75f;
			m_infoBoardTime = m_time;
			if (time > 0.25f && m_time <= 0.25f)
			{
				Game.game_work.mainScreen.m_state = MainScreen.MS.MS_RETURN;
			}
			if (m_time < 0.001f)
			{
				m_terminate = true;
			}
			break;
		case GMS.GMS_CLASSIC:
		case GMS.GMS_CASINO:
		case GMS.GMS_ARCADE:
		case GMS.GMS_ZEN:
			m_time *= 0.85f;
			m_infoBoardTime = m_time;
			if (Game.game_work.gameOverTransition < -0.9f)
			{
				Game.game_work.levelStartCoins = Game.game_work.coins;
				WaveManager.GetInstance().Reset(newGame: true);
				Game.game_work.gameOver = true;
			}
			Game.game_work.gameOverTransition *= 0.75f;
			if (Mortar.Math.Abs(Game.game_work.gameOverTransition) < 0.001f)
			{
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_DANANANA_SCHWING);
				Game.game_work.gameOverTransition = 0f;
				Game.game_work.gameOver = false;
				m_terminate = true;
				Game.game_work.mainScreen.m_state = MainScreen.MS.MS_GAME;
			}
			break;
		}
	}

	public void DeletedMenuButton(HUDControl control)
	{
		if (control == m_comingSoon1)
		{
			m_comingSoon1 = null;
			m_comingSoonRespawnTime1 = 0.85f;
		}
		if (control == m_comingSoon2)
		{
			m_comingSoon2 = null;
			m_comingSoonRespawnTime2 = 0.85f;
		}
	}

	public override void Draw(float[] tintchannnnel)
	{
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (!firstRender)
		{
			firstRender = true;
			Color color = default(Color);
			color = new Color(0, 0, 0, 128);
			for (int i = 0; i < 3; i++)
			{
				top_tri[i].X = (top_tri[i].Y = (top_tri[i].Z = (top_tri[i].nx = (top_tri[i].ny = 0f))));
				top_tri[i].nz = 1f;
				top_tri[i].color = color;
				top_tri[i].u = 0.5f;
				top_tri[i].v = 0.5f;
				btm_tri[i].X = (btm_tri[i].Y = (btm_tri[i].Z = (btm_tri[i].nx = (btm_tri[i].ny = 0f))));
				btm_tri[i].nz = 1f;
				btm_tri[i].color = color;
				btm_tri[i].u = 0.5f;
				btm_tri[i].v = 0.5f;
			}
			top_tri[1].Y = (top_tri[2].Y = 82f);
			top_tri[2].X = -656f;
			btm_tri[1].Y = (btm_tri[2].Y = -82f);
			btm_tri[2].X = 656f;
		}
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(new Vector3((float)(s_senseiTexture.GetWidth() + 1), (float)(s_senseiTexture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX);
		MatrixManager.GetInstance().Translate(SENSEI_SELECT_POS - Vector3.UnitX * (float)s_senseiTexture.GetWidth() * (1f - m_time));
		MatrixManager.GetInstance().UploadCurrentMatrices();
		s_senseiTexture.Set();
		Mesh.DrawQuad(Color.White);
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Translate(new Vector3(Game.SCREEN_WIDTH / 2f, Game.SCREEN_HEIGHT / 2f - 48f * m_time, 0f));
		MatrixManager.GetInstance().UploadCurrentMatrices();
		Mesh.DrawTriList(top_tri, 3, hasAlpha: true);
		s_senseiTexture.UnSet();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(new Vector3((float)(s_titleTexture.GetWidth() + 1), (float)(s_titleTexture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX);
		MatrixManager.GetInstance().Translate(TITLE_POS + Vector3.UnitY * 48f * (1f - m_time));
		MatrixManager.GetInstance().UploadCurrentMatrices();
		s_titleTexture.Set();
		Mesh.DrawQuad(Color.White);
		s_titleTexture.UnSet();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 2f, (0f - Game.SCREEN_HEIGHT) / 2f + 55f * m_time, 0f));
		MatrixManager.GetInstance().UploadCurrentMatrices();
		s_senseiTexture.Set();
		Mesh.DrawTriList(btm_tri, 3, hasAlpha: true);
		s_senseiTexture.UnSet();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(new Vector3((float)(s_modeselectTexture.GetWidth() + 1), (float)(s_modeselectTexture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX);
		MatrixManager.GetInstance().Translate(MODE_SELECT_POS - Vector3.UnitY * 55f * (1f - m_time));
		MatrixManager.GetInstance().UploadCurrentMatrices();
		s_modeselectTexture.Set();
		Mesh.DrawQuad(Color.White);
		s_modeselectTexture.UnSet();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(new Vector3((float)(s_zenBoardTexture.GetWidth() + 1), (float)(s_zenBoardTexture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX);
		MatrixManager.GetInstance().Translate(ZEN_BOARD_POS_START + (ZEN_BOARD_POS_FINISH - ZEN_BOARD_POS_START) * m_infoBoardTime);
		MatrixManager.GetInstance().UploadCurrentMatrices();
		s_zenBoardTexture.Set();
		Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
		s_zenBoardTexture.UnSet();
	}

	public void QuitCallback()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_MENU_BOMB);
		m_state = GMS.GMS_OUT;
		((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
		m_quitButton.m_entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(5f) + 5f, 0f - Mortar.Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
	}

	public void ClassicModeCallback()
	{
		m_state = GMS.GMS_CLASSIC;
		Game.game_work.gameMode = Game.GAME_MODE.GM_CLASSIC;
		TheGame.PE_GameMode("Classic");
	}

	public void CasinoModeCallback()
	{
		m_state = GMS.GMS_CASINO;
		Game.game_work.gameMode = Game.GAME_MODE.GM_CASINO;
	}

	public void ZenModeCallback()
	{
		if (Game.isWP7TrialMode())
		{
			Game.ClearMenuItems();
			RESPOAWWEN = true;
		}
		else
		{
			m_state = GMS.GMS_ZEN;
			Game.game_work.gameMode = Game.GAME_MODE.GM_ZEN;
			TheGame.PE_GameMode("Zen");
		}
	}

	public void ArcadeCallback()
	{
		if (Game.isWP7TrialMode())
		{
			Game.ClearMenuItems();
			RESPOAWWEN = true;
		}
		else
		{
			m_state = GMS.GMS_ARCADE;
			Game.game_work.gameMode = Game.GAME_MODE.GM_ARCADE;
			TheGame.PE_GameMode("Arcade");
		}
	}

	public bool IsMultiplayer()
	{
		throw new MissingMethodException();
	}
}
