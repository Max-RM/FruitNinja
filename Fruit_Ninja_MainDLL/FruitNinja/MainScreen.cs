using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Mortar;

namespace FruitNinja;

public class MainScreen : HUDControl3d
{
	public enum MS
	{
		MS_IN,
		MS_WAIT,
		MS_OUT,
		MS_ABOUT,
		MS_DOJO,
		MS_RETURN,
		MS_OPENFEINT,
		MS_OPENFEINT_GAMES,
		MS_GAME_MODE,
		MS_GAME_MODE_MP,
		MS_GAME,
		MS_ACHIEVEMENTS
	}

	protected float m_time;

	protected Vector3 m_originalScale;

	protected Mortar.Texture s_newGameTex;

	protected Mortar.Texture s_dojoTex;

	protected Mortar.Texture s_helpAndSettingsTex;

	protected Mortar.Texture s_moreGamesTex;

	protected Mortar.Texture s_leaderboardsTex;

	protected MenuButton m_achivementsButton;

	protected MenuButton m_gamerProfileButton;

	protected MenuButton m_helpOptionsButton;

	protected MenuButton m_leaderboardsButton;

	protected MenuButton m_marketplaceButton;

	protected MenuButton m_purchaseButton;

	protected Texture2D m_live;

	protected MenuButton m_newGame;

	protected MenuButton m_helpAndSettingsFruit;

	protected MenuButton m_dojoButton;

	protected MenuButton m_moreGames;

	protected MenuButton m_multiplayerGame;

	protected MenuButton m_sound;

	protected MenuButton m_music;

	protected Mortar.Texture m_leaderBoardComingSoonTexture;

	protected Vector3 m_comingSoonPos;

	protected Mortar.Texture[] m_musicTextures = new Mortar.Texture[2];

	protected Mortar.Texture[] m_soundTextures = new Mortar.Texture[2];

	protected Mortar.Texture m_fruitTex;

	protected Mortar.Texture m_ninjaTex;

	protected Mortar.Model m_backing;

	protected Mortar.Texture m_tuteTex;

	protected Vector3 m_tutePos;

	protected float m_tuteTime;

	protected Vector3 m_fruitPos;

	protected Vector3 m_ninjaPos;

	protected float m_ninjaGrav;

	public MS m_state;

	public float m_transitionWait;

	private static float tute = 1f;

	private static float liveX;

	private static float liveY;

	private static bool madeTris = false;

	private static VertexPositionColorTexture[] top_tri = new VertexPositionColorTexture[3];

	public static float BEGINNING_WAIT => 0.15f;

	public static float BEGINNING_SCALE => 2f;

	public static float NORMAL_SCALE => 1f;

	public static float SLIDE_IN_TIME => 0.2f;

	public static float POP_SINE => (int)Mortar.Math.DEGREE_TO_IDX(110f);

	public static float TOP_BUTTONS_Y => Game.SCREEN_HEIGHT / 2f - 24.5f;

	public static Vector3 TUTE_DIRECTION => new Vector3(-120f, -17f, 0f);

	public static Vector3 TUTE_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 65f, Game.SCREEN_HEIGHT / 2f - 134f, 0f);

	public static float BOMB_FLASH_FULL => 1.55f;

	public static float OVERALL_BUTTON_SCALE => 1f;

	public static Vector3 HELP_AND_SETTINGS_POS => new Vector3(403f - Game.SCREEN_WIDTH / 2f, -45f, 0f);

	public static Vector3 NEW_GAME_POS => new Vector3(244f - Game.SCREEN_WIDTH / 2f, -66f, 0f);

	public static Vector3 DOJO_POS => new Vector3(77f - Game.SCREEN_WIDTH / 2f, -87f, 0f);

	public static Vector3 LEADERBOARD_BUTTON_POS => new Vector3(388f - Game.SCREEN_WIDTH / 2f, 7f, 0f);

	public static Vector3 MORE_GAMES_POS => new Vector3(422f - Game.SCREEN_WIDTH / 2f, -106f, 0f);

	private string YES => TheGame.instance.stringTable.GetString(931);

	private string NO => TheGame.instance.stringTable.GetString(926);

	public MainScreen()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		m_fruitTex = TextureManager.GetInstance().Load("textureswp7/fruit_text.tex");
		m_ninjaTex = TextureManager.GetInstance().Load("textureswp7/ninja_text.tex");
		m_tuteTex = TextureManager.GetInstance().Load("slice_fruit.tex", localise: true);
		m_backing = null;
		s_newGameTex = TextureManager.GetInstance().Load("newgame.tex", localise: true);
		s_helpAndSettingsTex = TextureManager.GetInstance().Load("menu_help_ring.tex", localise: true);
		s_dojoTex = TextureManager.GetInstance().Load("textureswp7/dojo_icon.tex");
		m_moreGames = null;
		m_originalScale = (m_scale = new Vector3(480f, 138f, 1f));
		m_state = MS.MS_IN;
		m_selfCleanUp = false;
		m_time = 0f;
		m_sound = null;
		m_music = null;
		m_newGame = null;
		m_multiplayerGame = null;
		m_dojoButton = null;
		m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT - m_scale.Y) * 0.5f, 0f);
		m_tuteTime = 1f;
		m_ninjaPos.Y = Game.SCREEN_HEIGHT / 2f + (float)(m_ninjaTex.GetHeight() / 2);
		m_ninjaGrav = 0f;
		m_transitionWait = 0f;
		m_terminate = false;
		for (int i = 0; i < 2; i++)
		{
			m_soundTextures[i] = null;
			m_musicTextures[i] = null;
		}
	}

	public void NewGameCallback()
	{
		m_state = MS.MS_OUT;
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_DANANANA_SCHWING);
		if (m_newGame != null)
		{
			Mortar.Math.g_random.Seed((uint)(-2.1474836E+09f * m_newGame.m_rotation));
		}
	}

	public void LeaderboardsCallback()
	{
	}

	public void MoreGamesCallback()
	{
		m_state = MS.MS_IN;
	}

	public void AboutCallback()
	{
		m_state = MS.MS_DOJO;
		m_time = 1f;
		Game.game_work.tutorialControl.ResetTutePos();
	}

	private void NotConnected(IAsyncResult result)
	{
	}

	public void MarketplaceCallback()
	{
	}

	public void ShowCardCallback()
	{
	}

	public void AchievementCallback()
	{
	}

	public void BlankCallback()
	{
	}

	public void GameModeCallback()
	{
		m_state = MS.MS_GAME_MODE;
		m_time = 1f;
		Game.game_work.tutorialControl.ResetTutePos();
		if (m_newGame != null)
		{
			Mortar.Math.g_random.Seed((uint)(-2.1474836E+09f * m_newGame.m_rotation));
		}
	}

	public void HelpAndOptionsCallback()
	{
		m_state = MS.MS_ABOUT;
		m_time = 1f;
		Game.game_work.tutorialControl.ResetTutePos();
	}

	private static void MuteCallback(IAsyncResult result)
	{
		bool flag = false;
		if (true)
		{
			Game.game_work.musicEnabled = !Game.game_work.musicEnabled;
			try
			{
				MediaPlayer.IsMuted = !Game.game_work.musicEnabled;
			}
			catch
			{
			}
		}
	}

	public void MusicCallback()
	{
		if (Game.game_work.musicEnabled && SoundManager.GetInstance().CustomMusic)
		{
			TheGame.instance.stringTable.GetString(926);
			TheGame.instance.stringTable.GetString(931);
			try
			{
				MuteCallback(null);
				return;
			}
			catch
			{
				return;
			}
		}
		Game.game_work.musicEnabled = !Game.game_work.musicEnabled;
		try
		{
			MediaPlayer.IsMuted = !Game.game_work.musicEnabled;
		}
		catch
		{
		}
	}

	public void SoundCallback()
	{
		Game.game_work.soundEnabled = !Game.game_work.soundEnabled;
		SoundManager.GetInstance().SetSFXVolume(Game.game_work.soundEnabled ? SoundDef.DEFAULT_SFX_VOL : 0f);
	}

	public void MultiplayerGameModeCallback()
	{
	}

	public void ClickedCallback()
	{
		throw new MissingMethodException();
	}

	public override void Reset()
	{
	}

	public override void Release()
	{
		if (m_dojoButton != null)
		{
			m_dojoButton.m_deleteCall = HUDControl.DefaultDeleteCallback;
		}
		m_texture = null;
		m_backing = null;
		for (int i = 0; i < 2; i++)
		{
			m_soundTextures[i] = null;
			m_musicTextures[i] = null;
		}
		s_newGameTex = null;
		s_dojoTex = null;
		s_helpAndSettingsTex = null;
		s_moreGamesTex = null;
		s_leaderboardsTex = null;
	}

	public override void Init()
	{
		Reset();
	}

	private void ExitGameCallback(IAsyncResult result)
	{
		bool flag = false;
		int? num = Guide.EndShowMessageBox(result);
		if (num.HasValue && num.HasValue && num.Value == 1)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (Game.isWP7TrialMode())
		{
			TheGame.instance.DoUpsell(exitGame: true);
			return;
		}
		try
		{
			MediaPlayer.IsMuted = false;
		}
		catch
		{
		}
		TheGame.exitingGame = true;
		((Microsoft.Xna.Framework.Game)TheGame.instance).Exit();
	}

	public override void Update(float dt)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0912: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f - Game.game_work.gameOverTransition;
		Vector3 val = default(Vector3);
		val = new Vector3(128f, 40f, 1f);
		_ = m_gamerProfileButton;
		Game.isWP7TrialMode();
		if (m_sound == null)
		{
			m_sound = new MenuButton("sound.tex", new Vector3(Game.SCREEN_WIDTH / 2f - 24f, TOP_BUTTONS_Y, 0f), SoundCallback, -1, new Vector3(32f, 32f, 1f), localise: false);
			m_sound.Init();
			Game.game_work.hud.AddControl(m_sound);
			m_soundTextures[0] = m_sound.m_texture;
			m_soundTextures[1] = TextureManager.GetInstance().Load("textureswp7/sound_cross.tex");
			m_sound.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		if (m_music == null)
		{
			m_music = new MenuButton("music.tex", new Vector3(Game.SCREEN_WIDTH / 2f - 24f - 40f, TOP_BUTTONS_Y, 0f), MusicCallback, -1, new Vector3(32f, 32f, 1f), localise: false);
			m_music.Init();
			Game.game_work.hud.AddControl(m_music);
			m_musicTextures[0] = m_music.m_texture;
			m_musicTextures[1] = TextureManager.GetInstance().Load("textureswp7/music_cross.tex");
			m_music.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		m_music.m_texture = m_musicTextures[(!Game.game_work.musicEnabled) ? 1u : 0u];
		m_sound.m_texture = m_soundTextures[(!Game.game_work.soundEnabled) ? 1u : 0u];
		switch (m_state)
		{
		case MS.MS_IN:
			Game.game_work.gameMode = Game.GAME_MODE.GM_CLASSIC;
			TheGame.pomegranateThrown = false;
			if (m_achivementsButton != null)
			{
				m_achivementsButton.SetActive(state: true);
			}
			if (m_gamerProfileButton != null)
			{
				m_gamerProfileButton.SetActive(state: true);
			}
			if (m_helpOptionsButton != null)
			{
				m_helpOptionsButton.SetActive(state: true);
			}
			if (m_leaderboardsButton != null)
			{
				m_leaderboardsButton.SetActive(state: true);
			}
			if (m_marketplaceButton != null)
			{
				m_marketplaceButton.SetActive(state: true);
			}
			if (m_purchaseButton != null)
			{
				m_purchaseButton.SetActive(state: true);
			}
			m_moreGames = null;
			if (m_transitionWait > 0f || Game.game_work.hitBombTime > BOMB_FLASH_FULL - 0.1f)
			{
				m_transitionWait -= dt;
				Game.game_work.gameOverTransition += (-1f - Game.game_work.gameOverTransition) * 0.125f;
				if (Game.game_work.gameOverTransition < 0f)
				{
					Game.game_work.gameOverTransition = 0f;
				}
			}
			else
			{
				m_time += dt;
				if (Game.game_work.gameOverTransition < -0.999f)
				{
					Game.game_work.gameOverTransition = -1f;
				}
				else
				{
					Game.game_work.gameOverTransition += (-1f - Game.game_work.gameOverTransition) * 0.125f;
				}
			}
			if (m_time > BEGINNING_WAIT && Game.game_work.gameOverTransition < 0f)
			{
				m_state = MS.MS_WAIT;
				m_helpAndSettingsFruit = new MenuButton(s_helpAndSettingsTex, HELP_AND_SETTINGS_POS, HelpAndOptionsCallback, Fruit.FruitType("pear", returnRandIfFalse: true));
				m_helpAndSettingsFruit.Init();
				Game.game_work.hud.AddControl(m_helpAndSettingsFruit);
				m_helpAndSettingsFruit.m_originalScale = new Vector3((float)(m_helpAndSettingsFruit.m_texture.GetWidth() + 1), (float)(m_helpAndSettingsFruit.m_texture.GetHeight() + 1), 1f) * Game.GAME_MODE_SCALE_FIX * OVERALL_BUTTON_SCALE;
				Entity entity = m_helpAndSettingsFruit.m_entity;
				entity.m_cur_scale *= OVERALL_BUTTON_SCALE;
				m_helpAndSettingsFruit.m_overallScratchScale = 0.5f;
				m_helpAndSettingsFruit.m_texture.texture_filename = "menu_help_ring";
				m_newGame = new MenuButton(s_newGameTex, NEW_GAME_POS, GameModeCallback, 3);
				m_newGame.Init();
				Game.game_work.hud.AddControl(m_newGame);
				m_newGame.m_originalScale = new Vector3((float)(m_newGame.m_texture.GetWidth() + 1), (float)(m_newGame.m_texture.GetHeight() + 1), 1f) * Game.GAME_MODE_SCALE_FIX * OVERALL_BUTTON_SCALE;
				Entity entity2 = m_newGame.m_entity;
				entity2.m_cur_scale *= OVERALL_BUTTON_SCALE;
				m_newGame.m_overallScratchScale = 0.5f;
				m_newGame.m_texture.texture_filename = "newgame";
				Game.game_work.tutorialControl.ResetTutePos(m_newGame);
				m_dojoButton = new MenuButton(s_dojoTex, DOJO_POS, AboutCallback, Fruit.FruitType("mango"));
				m_dojoButton.Init();
				m_dojoButton.SetNewSymbol(ItemManager.GetInstance().AreNewItems());
				m_dojoButton.m_deleteCall = ButtonDeleted;
				if (m_dojoButton.m_entity != null)
				{
					Entity entity3 = m_dojoButton.m_entity;
					entity3.m_cur_scale *= 0.9f * OVERALL_BUTTON_SCALE;
				}
				MenuButton dojoButton = m_dojoButton;
				dojoButton.m_originalScale *= 1.05f * OVERALL_BUTTON_SCALE;
				m_dojoButton.m_clearOthers = true;
				m_dojoButton.m_texture.texture_filename = "dojo";
				Game.game_work.hud.AddControl(m_dojoButton);
			}
			break;
		case MS.MS_OUT:
			if (num > 0.999f)
			{
				Game.game_work.levelStartCoins = Game.game_work.coins;
				WaveManager.GetInstance().Reset();
				Game.game_work.gameOver = true;
			}
			Game.game_work.gameOverTransition *= 0.75f;
			if (Mortar.Math.Abs(Game.game_work.gameOverTransition) < 0.001f)
			{
				Game.game_work.gameOverTransition = 0f;
				Game.game_work.gameOver = false;
				m_state = MS.MS_GAME;
			}
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			break;
		case MS.MS_WAIT:
			if (!Guide.IsVisible && (TheGame.instance.BackButtonWasPressed || TheGame.instance.EscapeButtonWasPressed))
			{
				string[] array = new string[2] { NO, YES };
				Guide.BeginShowMessageBox(TheGame.instance.stringTable.GetString(929), TheGame.instance.stringTable.GetString(1009), (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)ExitGameCallback, (object)null);
				return;
			}
			if (m_dojoButton != null)
			{
				m_dojoButton.SetNewSymbol(ItemManager.GetInstance().AreNewItems());
			}
			_ = m_moreGames;
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			if (Game.game_work.gameOverTransition < -0.999f)
			{
				Game.game_work.gameOverTransition = -1f;
			}
			else
			{
				Game.game_work.gameOverTransition += (-1f - Game.game_work.gameOverTransition) * 0.125f;
			}
			break;
		case MS.MS_ABOUT:
		case MS.MS_DOJO:
			ActorManager.GetInstance().GetNumEntities(0);
			m_time *= 0.75f;
			if (m_time != 0f && m_time < 0.001f)
			{
				m_time = 0f;
				HUDControl hUDControl = null;
				hUDControl = ((m_state != MS.MS_ABOUT) ? ((HUDControl3d)new DojoScreen()) : ((HUDControl3d)new AboutScreen(this)));
				hUDControl.Init();
				Game.game_work.hud.AddControl(hUDControl);
			}
			num = m_time;
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			break;
		case MS.MS_GAME_MODE:
		case MS.MS_GAME_MODE_MP:
		{
			float time = m_time;
			m_time *= 0.85f;
			if (time > 0.25f && m_time <= 0.25f)
			{
				m_time = 0f;
				bool isMultiplayer = false;
				GameModeScreen gameModeScreen = new GameModeScreen(isMultiplayer);
				gameModeScreen.Init();
				Game.game_work.hud.AddControl(gameModeScreen);
				if (m_achivementsButton != null)
				{
					m_achivementsButton.SetActive(state: false);
				}
				if (m_gamerProfileButton != null)
				{
					m_gamerProfileButton.SetActive(state: false);
				}
				if (m_helpOptionsButton != null)
				{
					m_helpOptionsButton.SetActive(state: false);
				}
				if (m_leaderboardsButton != null)
				{
					m_leaderboardsButton.SetActive(state: false);
				}
				if (m_marketplaceButton != null)
				{
					m_marketplaceButton.SetActive(state: false);
				}
				if (m_purchaseButton != null)
				{
					m_purchaseButton.SetActive(state: false);
				}
				GC.Collect();
			}
			num = m_time;
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			break;
		}
		case MS.MS_RETURN:
			if (m_time > 0.999f)
			{
				num = 1f;
				m_time += dt;
				if (m_time > 1.5f)
				{
					m_time = BEGINNING_WAIT;
					m_transitionWait = 0f;
					m_state = MS.MS_IN;
				}
			}
			else
			{
				m_time += (1f - m_time) * 0.125f;
				num = m_time;
			}
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			break;
		case MS.MS_OPENFEINT:
		case MS.MS_OPENFEINT_GAMES:
			ActorManager.GetInstance().GetNumEntities(0);
			m_time *= 0.75f;
			if (m_time != 0f && m_time < 0.001f)
			{
				m_time = 0f;
				LeaderboardsScreen leaderboardsScreen = new LeaderboardsScreen();
				leaderboardsScreen.Init();
				Game.game_work.hud.AddControl(leaderboardsScreen);
			}
			num = m_time;
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			break;
		case MS.MS_ACHIEVEMENTS:
			ActorManager.GetInstance().GetNumEntities(0);
			m_time *= 0.75f;
			if (m_time != 0f && m_time < 0.001f)
			{
				m_time = 0f;
				AchievementsScreen achievementsScreen = new AchievementsScreen();
				achievementsScreen.Init();
				Game.game_work.hud.AddControl(achievementsScreen);
			}
			num = m_time;
			m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT + m_scale.Y - m_scale.Y * num * 2f) * 0.5f, 0f);
			break;
		}
		if (m_state != MS.MS_WAIT && m_state != MS.MS_IN && m_newGame != null && m_newGame.m_scale.X < 45f * OVERALL_BUTTON_SCALE)
		{
			m_newGame = null;
		}
		if (m_state != MS.MS_WAIT || (m_moreGames != null && m_moreGames.m_entity == null && m_moreGames.m_scale.X < 45f))
		{
			m_moreGames = null;
		}
		dt = Mortar.Math.MIN(dt, 0.04f);
		m_fruitPos.X = (0f - Game.SCREEN_WIDTH) / 4f;
		m_fruitPos.Y = m_pos.Y + 18f;
		m_fruitPos.Z = (m_ninjaPos.Z = 0f);
		m_ninjaPos.X = m_fruitPos.X + 180f;
		m_ninjaGrav -= dt * 55f;
		ref Vector3 ninjaPos = ref m_ninjaPos;
		ninjaPos.Y += m_ninjaGrav * dt * 15f;
		m_tutePos = m_fruitPos;
		if (dt > 0f)
		{
			tute = 1f;
		}
		if (m_ninjaPos.Y < m_fruitPos.Y - 15f)
		{
			m_ninjaPos.Y = m_fruitPos.Y - 15f;
			m_ninjaGrav *= -0.25f;
			if (Mortar.Math.Abs(m_ninjaGrav) < 5f && num > 0.99f && dt > 0f)
			{
				tute = 0f;
			}
		}
		m_tuteTime += (tute - m_tuteTime) * 0.25f;
		m_tutePos = TUTE_POS + TUTE_DIRECTION * m_tuteTime * 2f;
		if (m_sound != null && m_music != null)
		{
			m_sound.m_pos.X = -180f;
			m_music.m_pos.X = -140f;
			m_sound.SetActive(state: true);
			m_music.SetActive(state: true);
			m_sound.m_pos.Y = AboutScreen.AboutScreenTime;
			m_music.m_pos.Y = AboutScreen.AboutScreenTime;
		}
		float num2 = 340f + num * -170f;
		float num3 = 30f;
		float num4 = -50f;
		if (GameTask.GetPauseAmount() != 0f)
		{
			return;
		}
		_ = m_gamerProfileButton;
		if (m_live != null)
		{
			liveY = 12f;
			liveX = 130f + num2 * 2f;
		}
		if (m_achivementsButton != null)
		{
			if (Game.isWP7TrialMode())
			{
				m_achivementsButton.m_pos.Y = -12333f;
				m_achivementsButton.m_pos.X = -12333f;
			}
			else
			{
				m_achivementsButton.m_pos.Y = num3;
				m_achivementsButton.m_pos.X = num2;
				num3 += num4;
			}
		}
		if (m_leaderboardsButton != null)
		{
			if (Game.isWP7TrialMode())
			{
				m_leaderboardsButton.m_pos.Y = -12333f;
				m_leaderboardsButton.m_pos.X = -12333f;
			}
			else
			{
				m_leaderboardsButton.m_pos.Y = num3;
				m_leaderboardsButton.m_pos.X = num2;
				num3 += num4;
			}
		}
		if (m_helpOptionsButton != null)
		{
			m_helpOptionsButton.m_pos.Y = num3;
			m_helpOptionsButton.m_pos.X = num2;
			num3 += num4;
		}
		if (!Game.isWP7TrialMode() && m_marketplaceButton != null)
		{
			m_marketplaceButton.m_pos.Y = num3;
			m_marketplaceButton.m_pos.X = num2;
			num3 += num4;
		}
		if (m_purchaseButton != null)
		{
			if (!Game.isWP7TrialMode())
			{
				m_purchaseButton.m_pos.X = -12333f;
			}
			else
			{
				m_purchaseButton.m_pos.Y = num3;
				m_purchaseButton.m_pos.X = num2;
			}
			num3 += num4;
		}
	}

	private void ReadFinished(int result)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		if (m_state == MS.MS_GAME || ((m_state == MS.MS_ABOUT || m_state == MS.MS_DOJO) && m_time == 0f))
		{
			return;
		}
		if (!madeTris)
		{
			madeTris = true;
			Color color = default(Color);
			color = new Color(0, 0, 0, 128);
			for (int i = 0; i < 3; i++)
			{
				top_tri[i].Position.X = (top_tri[i].Position.Y = (top_tri[i].Position.Z = 0f));
				top_tri[i].Color = color;
				top_tri[i].TextureCoordinate.X = 0.5f;
				top_tri[i].TextureCoordinate.Y = 0.5f;
			}
			top_tri[0].Position.X = -0.5f;
			top_tri[0].Position.Y = -0.5f;
			top_tri[1].Position.X = 3.5f;
			top_tri[1].Position.Y = 1f;
			top_tri[2].Position.X = -0.5f;
			top_tri[2].Position.Y = 1f;
		}
		if (m_fruitTex != null)
		{
			m_fruitTex.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(m_scale);
			MatrixManager.GetInstance().Translate(m_pos);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawTriListEx(top_tri, 3, hasAlpha: true, 0);
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)m_fruitTex.GetWidth(), (float)m_fruitTex.GetHeight(), 0f) * Game.GAME_MODE_SCALE_FIX * 0.85f);
			MatrixManager.GetInstance().Translate(m_fruitPos);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_fruitTex.UnSet();
		}
		if (m_ninjaTex != null)
		{
			m_ninjaTex.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)m_ninjaTex.GetWidth(), (float)m_ninjaTex.GetHeight(), 0f) * Game.GAME_MODE_SCALE_FIX);
			MatrixManager.GetInstance().Translate(m_ninjaPos);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_ninjaTex.UnSet();
		}
		if (m_tuteTex != null)
		{
			m_tuteTex.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)m_tuteTex.GetWidth(), (float)m_tuteTex.GetHeight(), 0f) * Game.GAME_MODE_SCALE_FIX);
			MatrixManager.GetInstance().Translate(m_tutePos);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_tuteTex.UnSet();
		}
		if (m_leaderBoardComingSoonTexture != null && m_newGame != null)
		{
			m_leaderBoardComingSoonTexture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3(0.5f, 0.5f * ((float)m_leaderBoardComingSoonTexture.GetHeight() / (float)m_leaderBoardComingSoonTexture.GetWidth()), 1f) * m_newGame.m_scale.X);
			MatrixManager.GetInstance().Translate(LEADERBOARD_BUTTON_POS);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_leaderBoardComingSoonTexture.UnSet();
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public void Hide()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		m_pos = new Vector3(-1000f, 1000f, -1000f);
		m_state = MS.MS_GAME;
	}

	public void ButtonDeleted(HUDControl control)
	{
		if (control == m_dojoButton)
		{
			m_dojoButton = null;
		}
	}
}
