using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Mortar;

namespace FruitNinja;

public static class GameTask
{
	public struct SliceEffect
	{
		public float time;

		public float mag;

		public float angle;

		public Vector3 pos;

		public bool isBlue;

		public int model;

		public Fruit waitForFruit;

		public float speed;
	}

	private const float START_DELAY = 1.5f;

	public static float BACKGROUND_UVS_U0 = 0.126f;

	public static float BACKGROUND_UVS_U1 = 0.874f;

	public static float BACKGROUND_UVS_V0 = 0.21876f;

	public static float BACKGROUND_UVS_V1 = 0.78124f;

	public static Font hud_font;

	public static Font challenge_font;

	public static bool unpause_game = false;

	public static float unpauseDelay = 0f;

	public static float repauseDelay = 0f;

	public static bool challengeOver = false;

	public static bool debugMenu = false;

	public static bool clearInput = false;

	public static bool initialised = false;

	public static bool movieNotLoaded = false;

	public static Texture backgroundTexture = null;

	public static MortarSound s_bombSound = null;

	public static float s_startFadeInTime = 1.5f;

	public static Texture s_HBlogo = null;

	public static float s_musicFade = 0f;

	public static int s_songPlaying = 0;

	public static int MAX_SLASHES = 4;

	public static SlashEntity[] inputEnts = ArrayInit.CreateFilledArray<SlashEntity>(MAX_SLASHES);

	public static Model[] s_sliceModel;

	public static Vector3[] fruitSliceFrameScales;

	public static LinkedList<SliceEffect> s_slices;

	public static float[,] missPositions;

	public static MainScreen s_mainScreen;

	public static PauseScreen s_pauseScreen;

	public static float END_SCREEN_DELAY;

	public static float end_screen_delay;

	public static Vector3 s_bombHitPos;

	public static Vector3 s_critHitPos;

	private static bool showdlg;

	public static bool s_menuBombHit;

	private static float slowTime;

	private static float slowTimeTime;

	private static float slowTimeSpeed;

	private static float paticlesDt;

	public static float quickener;

	private static int s_flash;

	private static int s_flashAlpha;

	private static Texture s_flashTex;

	private static float[] startTint;

	public static Texture s_flashTexture;

	public static Color s_flashColor;

	public static bool __isSaving;

	public static float GetBombZPositionz;

	public static float GetFruitZPositionz;

	private static GameVertex[] verts;

	public static float UNPAUSE_DELAY_TIME => 0.25f;

	public static float REPAUSE_DELAY_TIME => 0.4f;

	public static float CROSS_SPACING => 4f;

	public static float SLICE_FRAMES => 6f;

	public static float TOTAL_SLICE_LENGTH => 0.15f;

	public static float SLICE_SPEED => 1f / (TOTAL_SLICE_LENGTH / SLICE_FRAMES);

	public static float HIT_BOMB_WAIT => 3.2f;

	public static float BOMB_FLASH_START => 2f;

	public static float BOMB_FLASH_FULL => 1.55f;

	public static float BOMB_FLASH_START_FADE => 1f;

	public static float BOMB_FLASH_START_GAME_OVER => 1.5f;

	public static float RETRY_TIME_DELAY => 0.1f;

	public static void SaveCurrentData()
	{
		SaveCurrentData(isClosingSave: false);
	}

	public static void AddSlice(Vector3 pos, float angle, float mag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		AddSlice(pos, angle, mag, isBlue: false);
	}

	public static void SkipToPause()
	{
		SkipToPause(atStart: true);
	}

	private static void AllowMusicCallback(IAsyncResult result)
	{
		bool flag = false;
		int? num = Guide.EndShowMessageBox(result);
		if (num.HasValue)
		{
			Guide.EndShowMessageBox(result);
			if (num.HasValue && num.Value == 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			SoundManager.GetInstance().AllowMusic();
			SoundManager.GetInstance().SongPlay("Music-menu");
		}
		else
		{
			SoundManager.GetInstance().CustomMusic = true;
		}
	}

	private static void AllowMusicCallback2(IAsyncResult result)
	{
		bool flag = false;
		int? num = Guide.EndShowMessageBox(result);
		if (num.HasValue)
		{
			Guide.EndShowMessageBox(result);
			if (num.HasValue && num.Value == 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			SoundManager.GetInstance().AllowMusic();
			SoundManager.GetInstance().SongPlay("background");
		}
		else
		{
			SoundManager.GetInstance().CustomMusic = true;
		}
	}

	private static void ShowDialog()
	{
		if (SoundManager.GetInstance().UserPlayingMusic())
		{
			string[] array = new string[2]
			{
				TheGame.instance.stringTable.GetString(926),
				TheGame.instance.stringTable.GetString(931)
			};
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			string text = TheGame.instance.stringTable.GetString(1188);
			string text2 = TheGame.instance.stringTable.GetString(1189);
			Guide.BeginShowMessageBox(text, text2, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)AllowMusicCallback, (object)null);
		}
		else
		{
			SoundManager.GetInstance().AllowMusic();
			SoundManager.GetInstance().SongPlay("Music-menu");
		}
	}

	private static void ShowDialog2()
	{
		if (SoundManager.GetInstance().UserPlayingMusic())
		{
			string[] array = new string[2]
			{
				TheGame.instance.stringTable.GetString(926),
				TheGame.instance.stringTable.GetString(931)
			};
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			string text = TheGame.instance.stringTable.GetString(1188);
			string text2 = TheGame.instance.stringTable.GetString(1189);
			Guide.BeginShowMessageBox(text, text2, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)AllowMusicCallback2, (object)null);
		}
		else
		{
			SoundManager.GetInstance().AllowMusic();
			SoundManager.GetInstance().SongPlay("background");
		}
	}

	public static void UpdateMusic(float dt)
	{
		if (!Game.game_work.musicEnabled)
		{
			return;
		}
		if (Game.game_work.gameOverTransition < 0f)
		{
			if (s_songPlaying != -1)
			{
				s_songPlaying = -1;
				if (showdlg)
				{
					showdlg = false;
					ShowDialog();
				}
				else
				{
					SoundManager.GetInstance().SongPlay("Music-menu");
				}
			}
		}
		else if (s_songPlaying != 1)
		{
			s_songPlaying = 1;
			if (showdlg)
			{
				showdlg = false;
				ShowDialog2();
			}
			else
			{
				SoundManager.GetInstance().SongPlay("background");
			}
		}
	}

	public static bool PointerMoveCallback(InputEvent e)
	{
		if (TheGame.exceptionThrown)
		{
			return true;
		}
		MortarRectangle windowSize = DisplayManager.instance.GetWindowSize();
		Vector2 val = default(Vector2);
		val = new Vector2((float)(windowSize.right - windowSize.left), (float)(windowSize.bottom - windowSize.top));
		if (e.axis.axis == 116)
		{
			Game.game_work.mainPointer.X = (e.axis.absolutePos - val.X / 2f) * (Game.SCREEN_WIDTH / Game.SCREEN_SIZE_X);
		}
		if (e.axis.axis == 117)
		{
			Game.game_work.mainPointer.Y = (0f - (e.axis.absolutePos - val.Y / 2f)) * (Game.SCREEN_HEIGHT / Game.SCREEN_SIZE_Y);
		}
		if (e.axis.axis >= 153 && e.axis.axis < 169)
		{
			inputEnts[e.axis.axis - 153].TouchMoveX(e);
			Game.game_work.touchPositions[e.axis.axis - 153].X = (e.axis.absolutePos - val.X / 2f) * (Game.SCREEN_WIDTH / Game.SCREEN_SIZE_X);
		}
		if (e.axis.axis >= 169 && e.axis.axis < 185)
		{
			inputEnts[e.axis.axis - 169].TouchMoveY(e);
			Game.game_work.touchPositions[e.axis.axis - 169].Y = (0f - (e.axis.absolutePos - val.Y / 2f)) * (Game.SCREEN_HEIGHT / Game.SCREEN_SIZE_Y);
		}
		return true;
	}

	public static bool TouchDownCallback(InputEvent e)
	{
		if (TheGame.exceptionThrown)
		{
			return true;
		}
		if (e.button.key >= 137 && e.button.key < 153)
		{
			inputEnts[e.button.key - 137].TouchDown(e);
			if (Game.game_work.touchPositions[e.button.key - 137].Z == -1f)
			{
				Game.game_work.touchPositions[e.button.key - 137].Z = 2f;
			}
			else
			{
				Game.game_work.touchPositions[e.button.key - 137].Z = 1f;
			}
		}
		return true;
	}

	public static bool PointerDownCallback(InputEvent e)
	{
		Game.game_work.pointerDown = true;
		Game.game_work.pointerPressed = true;
		return true;
	}

	public static bool PointerUpCallback(InputEvent e)
	{
		Game.game_work.pointerDown = false;
		Game.game_work.pointerReleased = true;
		return true;
	}

	public static bool GameReset(uint flags)
	{
		bool result = false;
		try
		{
			if (s_bombSound != null)
			{
				s_bombSound.SetVolume(0f);
				s_bombSound.Stop(0f);
				SoundManager.GetInstance().Release(s_bombSound);
				Delete.SAFE_DELETE(ref s_bombSound);
			}
			challengeOver = false;
			unpause_game = false;
			hud_font = null;
			challenge_font = null;
			s_sliceModel[0] = null;
			s_sliceModel[1] = null;
			for (int i = 0; i < MAX_SLASHES; i++)
			{
				inputEnts[i] = null;
			}
			s_slices.Clear();
			Delete.SAFE_DELETE(ref s_slices);
			s_flashTexture = null;
			PSPParticleManager.GetInstance().ClearEmitters();
			InputManager.GetInstance().ClearActions(0u);
			Game.game_work.hud.Release();
			MissControl.CleanPool();
			ActorManager.GetInstance().Clear();
			ActorManager.GetInstance().ClearAllListeners();
			Game.game_work.pause = false;
			initialised = false;
			backgroundTexture = null;
			GC.Collect();
			Game.game_work.hud = null;
			GameInit(1u);
			result = true;
		}
		catch
		{
		}
		return result;
	}

	public static void GameInit(uint flags)
	{
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (initialised)
		{
			return;
		}
		debugMenu = false;
		challengeOver = false;
		if (Game.game_work.hud == null)
		{
			Game.game_work.hud = new HUD();
		}
		Game.game_work.hud.Release();
		int num = 0;
		float num2 = ((num == 0) ? Game.HUD_SCALE : 1f);
		for (int i = 0; i < Game.MAX_FRUIT_MISSES; i++)
		{
			MissControl missControl = new MissControl();
			missControl.SetActive(state: true);
			if (num == 0)
			{
				missControl.m_pos = new Vector3(Game.SCREEN_WIDTH / 2f - missPositions[i, 0] * num2, Game.SCREEN_HEIGHT / 2f - missPositions[i, 1] * num2, 50f);
			}
			else
			{
				missControl.m_pos = new Vector3((0f - (missPositions[i, 0] - missPositions[1, 0] + 5f)) * num2, Game.SCREEN_HEIGHT / 2f - (missPositions[i, 1] + 15f) * num2, 50f);
			}
			missControl.m_rotation = 0f - missPositions[i, 2];
			missControl.m_scale = new Vector3(32f, 32f, 32f) * missPositions[i, 3] * num2;
			missControl.m_id = i;
			missControl.m_drawOrder = (HUD.HUD_ORDER)num;
			Game.game_work.hud.AddControl(missControl);
		}
		MissControl.CreatePool(8, Game.game_work.hud);
		ScoreControl scoreControl = new ScoreControl();
		scoreControl.m_texture = TextureManager.GetInstance().Load("textureswp7/hud_fruit.tex");
		scoreControl.m_scroreTexture = TextureManager.GetInstance().Load("score.tex", localise: true);
		scoreControl.m_highScoreTexture = TextureManager.GetInstance().Load("new_best_score.tex", localise: true);
		scoreControl.m_scale = Vector3.One * 64f;
		scoreControl.m_pos = new Vector3(0f - Game.PIXEL_PLATFORMX(Game.SCREEN_WIDTH / 2f - Game.GAME_FONT_SIZE) + scoreControl.m_scale.X * 0.35f, Game.PIXEL_PLATFORMY(Game.SCREEN_HEIGHT / 2f + Game.GAME_FONT_SIZE * 4f) - scoreControl.m_scale.Y * 0.35f, 0f);
		Game.game_work.hud.AddControl(scoreControl);
		Game.game_work.timeControl = new TimeControl();
		Game.game_work.timeControl.Init();
		Game.game_work.timeControl.CountDown(90.9f);
		Game.game_work.hud.AddControl(Game.game_work.timeControl);
		if (backgroundTexture == null)
		{
			backgroundTexture = (Game.IsFastHardware() ? TextureManager.GetInstance().Load("textureswp7/gb_game_sml.tex") : TextureManager.GetInstance().Load("textureswp7/gb_game_sml.tex"));
		}
		s_sliceModel[0] = MeshManager.GetInstance().Load("models/fruit/slice_fx.mmd");
		s_sliceModel[1] = MeshManager.GetInstance().Load("models/fruit/slice_fx_crit.mmd");
		s_slices = new LinkedList<SliceEffect>();
		s_slices.Clear();
		hud_font = Game.game_work.pGameFont;
		Game.game_work.pause = false;
		unpause_game = false;
		clearInput = false;
		initialised = true;
		s_mainScreen = new MainScreen();
		s_mainScreen.Init();
		Game.game_work.mainScreen = s_mainScreen;
		s_pauseScreen = new PauseScreen();
		s_pauseScreen.Init();
		Game.game_work.tutorialControl = new TutorialControl();
		Game.game_work.tutorialControl.Init();
		Game.game_work.gameOver = true;
		Game.game_work.gameOverTransition = -1f;
		Game.game_work.hud.AddControl(s_mainScreen);
		Game.game_work.hud.AddControl(s_pauseScreen);
		Game.game_work.hud.AddControl(Game.game_work.tutorialControl);
		Entity.HeapCreate(131072u);
		ActorManager.GetInstance().Initialise(7);
		ActorManager.GetInstance().RegisterFactory(EntityFactory.CreateEntity);
		ActorManager.GetInstance().RegisterHashConverter(EntityFactory.HashTypeConvert);
		WaveManager.GetInstance().Init();
		GameTaskInitInput();
		for (int j = 0; j < 30; j++)
		{
			Entity entity = ActorManager.GetInstance().Add(EntityTypes.ENTITY_BEGIN);
			entity.m_dormant = true;
			entity.m_destroy = true;
			entity = ActorManager.GetInstance().Add(EntityTypes.ENTITY_BOMB);
			entity.m_dormant = true;
			entity.m_destroy = true;
			entity = ActorManager.GetInstance().Add(EntityTypes.ENTITY_BOMB_BLAST);
			entity.m_dormant = true;
			entity.m_destroy = true;
		}
		SplatEntity.CreatePool(32);
		WaveManager.GetInstance().Resume();
		BombFlash.CreatePool(16);
		SoundManager.GetInstance().Initialise("Sound/Win32Project/Win/FruitNinja", Game.HEAP_SIZE_SOUND);
		SoundManager.GetInstance().SetSFXVolume(Game.game_work.soundEnabled ? SoundDef.DEFAULT_SFX_VOL : 0f);
		if (flags == 1 && TheGame.exceptionThrown)
		{
			TheGame.exceptionThrown = false;
		}
	}

	public static void SlowTime(float factor, float length)
	{
		slowTimeTime = 1f;
		slowTime = factor;
		slowTimeSpeed = 1f / length;
	}

	public static void GameUpdate(float dt, bool update)
	{
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Invalid comparison between Unknown and I4
		Game.game_work.canFastForward = false;
		Game.game_work.pointerReleased = false;
		Game.game_work.pointerPressed = false;
		PopOverControl.Update(dt);
		for (int i = 0; i < MAX_SLASHES; i++)
		{
			if (Game.game_work.touchPositions[i].Z > 0f)
			{
				Game.game_work.touchPositions[i].Z = 0f;
			}
			else if (Game.game_work.touchPositions[i].Z == 0f)
			{
				Game.game_work.touchPositions[i].Z = -1f;
			}
		}
		float num = dt;
		if (s_startFadeInTime <= 0f)
		{
			InputManager.GetInstance().Update(dt);
		}
		else
		{
			if (s_HBlogo == null)
			{
				s_HBlogo = TextureManager.GetInstance().Load("textureswp7/HB_logo.tex");
			}
			if (s_startFadeInTime > 0f)
			{
				var touchState = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();
				var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
				if (touchState.Count > 0 || mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				{
					s_startFadeInTime = 0f;
				}
			}
			s_startFadeInTime = Mortar.Math.MAX(s_startFadeInTime - dt * 2f, 0f);
			dt = 0f;
			num = 0f;
			Game.game_work.dt = 0f;
			if (s_startFadeInTime <= 0f)
			{
				s_HBlogo = null;
			}
		}
		SoundManager.GetInstance().Update(dt);
		UpdateMusic(dt);
		_ = end_screen_delay;
		float num2 = dt;
		if (update)
		{
			Game.game_work.saveData.inGame = false;
			float dt2 = num;
			if (Game.game_work.critHitTime > 0f)
			{
				Game.game_work.critHitTime -= dt;
			}
			float num3 = 1f;
			if (slowTimeTime > 0f)
			{
				slowTimeTime -= dt * slowTimeSpeed;
				num3 += (slowTime - num3) * slowTimeTime;
			}
			quickener = Mortar.Math.MAX(quickener - dt * (Game.game_work.canFastForward ? 2.5f : 5f), 1f);
			dt *= quickener * num3;
			num *= quickener * num3;
			Game.game_work.dt *= quickener * num3;
			num2 *= quickener * num3;
			SlashEntity.PreUpdate(dt2);
			SplatEntity.UpdateActiveSplats(dt2);
			if (Game.game_work.hitBombTime > 0f)
			{
				if (!s_menuBombHit)
				{
					Game.game_work.canFastForward = true;
				}
				float hitBombTime = Game.game_work.hitBombTime;
				Game.game_work.hitBombTime -= dt;
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && Game.game_work.gameOverTransition < 1f)
				{
					Game.game_work.hitBombTime -= dt;
				}
				if (Game.game_work.hitBombTime > BOMB_FLASH_FULL)
				{
					num2 = 0f - num2;
				}
				else
				{
					UpdateBombHit(hitBombTime);
					num2 += num2;
				}
				if (Game.game_work.hitBombTime <= BOMB_FLASH_START_GAME_OVER && hitBombTime > BOMB_FLASH_START_GAME_OVER && !Game.game_work.gameOver && !s_menuBombHit)
				{
					Game.GameOver();
				}
				if (Game.game_work.hitBombTime < 0f)
				{
					Game.game_work.hitBombTime = 0f;
				}
				dt2 = 0f;
			}
			else
			{
				Game.game_work.saveData.inGame = true;
				WaveManager.GetInstance().Update(dt);
				dt2 = num * WaveManager.GetInstance().GetWavedt();
			}
			if (dt2 == 0f && Game.game_work.gameOverTransition < 0f)
			{
				dt2 = num;
			}
			BombFlash.UpdateActiveFlashes(dt2);
			ActorManager.GetInstance().Update(dt2);
		}
		else
		{
			if (Mortar.Math.ABS(Game.game_work.gameOverTransition) > 0.999f)
			{
				Game.game_work.pause = false;
			}
			if (Game.game_work.pause)
			{
				Game.game_work.saveData.inGame = true;
			}
			SlashEntity.PreUpdate(0f);
			for (int j = 0; j < MAX_SLASHES; j++)
			{
				inputEnts[j].Update(num);
				inputEnts[j].DrawUpdate(num);
			}
			WaveManager.GetInstance().Update(0f);
		}
		paticlesDt = 1f;
		if (!Game.game_work.pause)
		{
			paticlesDt = Mortar.Math.MAX(1f, 1f / WaveManager.GetInstance().GetWavedt());
		}
		PSPParticleManager.GetInstance().Update(dt * paticlesDt, Game.game_work.pause);
		Game.game_work.camera.UpdateCamera(num2);
		Game.game_work.hud.Update(num);
		if (Game.game_work.canFastForward)
		{
			quickener = Mortar.Math.MIN(5f, quickener * (Game.game_work.pointerPressed ? 1.75f : 1f));
		}
		float num4 = (Game.game_work.gameOver ? 0f : Bomb.GetHeighestBomb());
		if (num4 > 0f && !Game.game_work.pause)
		{
			if (s_bombSound == null || (int)s_bombSound.inst.State == 2)
			{
				s_bombSound = SoundManager.CreateNewSound();
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_BOMB_FUSE, 0u, s_bombSound);
			}
			s_bombSound.SetVolume(Mortar.Math.CLAMP(num4 / 100f, 0f, 1f));
		}
		else if (s_bombSound != null)
		{
			s_bombSound.SetVolume(0f);
		}
		if (Game.game_work.inRetrySequence)
		{
			if (Game.game_work.retryTimer <= 0f)
			{
				EndRetryLevel();
				Game.game_work.retryTimer = 0f;
			}
			else
			{
				RetryUpdate(dt);
				Game.game_work.retryTimer -= dt;
			}
		}
	}

	public static void RetryUpdate(float dt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.Zero;
		LinkedListNode<Entity> iterator = null;
		float num = (RETRY_TIME_DELAY - Game.game_work.retryTimer) / RETRY_TIME_DELAY;
		num *= num;
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			Vector3 orig_scale = ((Bomb)entity).m_orig_scale;
			entity.m_cur_scale = orig_scale - (orig_scale - zero) * num;
			entity.m_vel = new Vector3(0f, 0f, 0f);
		}
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			Vector3 orig_scale = ((Fruit)entity).m_orig_scale;
			entity.m_cur_scale = orig_scale - (orig_scale - zero) * num;
			entity.m_vel = new Vector3(0f, 0f, 0f);
			((Fruit)entity).m_vel2 = new Vector3(0f, 0f, 0f);
		}
	}

	public static void Flash()
	{
		s_flash = 3;
		s_flashAlpha = 48;
		if (s_flashTex == null)
		{
			s_flashTex = TextureManager.GetInstance().Load("extra/white");
		}
	}

	public static void GameDraw(float dt, bool draw)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (draw)
		{
			if (backgroundTexture == null)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				startTint[i] = Game.game_work.hud.m_tint[i];
			}
			DisplayManager.instance.SetDepthBufferWrite(en: false);
			DisplayManager.instance.SetDepthBuffer(en: false);
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_BACKGROUND, changed: true);
			DrawBackground();
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			if (s_flash > 0)
			{
				s_flash--;
				if (s_flashTex != null)
				{
					s_flashTex.Set();
					MatrixManager.GetInstance().Reset();
					Matrix matrix = Matrix.CreateScale(Game.SCREEN_WIDTH + 1f, Game.SCREEN_HEIGHT + 1f, 0f);
					MatrixManager.GetInstance().SetMatrix(matrix);
					MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
					Mesh.DrawQuad(new Color(255, 255, 255, s_flashAlpha), 0f, 1f, 0f, 1f);
					s_flashAlpha >>= 1;
				}
			}
			PopOverControl.DrawBack();
			Game.game_work.hud.Draw(HUD.HUD_ORDER.HUD_ORDER_BEFORE_SPLAT);
			DisplayManager.instance.SetDepthBufferWrite(en: true);
			DisplayManager.instance.SetDepthBuffer(en: true);
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL, changed: true);
			FruitRay.DrawRays();
			ActorManager.GetInstance().Draw();
			DisplayManager.instance.SetDepthBuffer(en: true);
			DisplayManager.instance.SetDepthBufferWrite(en: false);
			if (!PopOverControl.IsInPopup)
			{
				SplatEntity.DrawActiveSplats();
			}
			BombBlast.DrawActiveBlasts();
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			Game.game_work.hud.Draw(HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT);
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL, changed: true);
			PSPParticleManager.GetInstance().Draw(dt, Game.game_work.pause, -1);
			DisplayManager.instance.SetDepthBuffer(en: false);
			if (!Game.game_work.inBonusScreen)
			{
				PSPParticleManager.GetInstance().Draw(dt, Game.game_work.pause, 0);
			}
			DrawSlices(dt);
			if (Game.game_work.hitBombTime <= 0f)
			{
				Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			}
			for (int j = 0; j < MAX_SLASHES; j++)
			{
				inputEnts[j].DrawSlice();
			}
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			Game.game_work.hud.Draw(HUD.HUD_ORDER.HUD_ORDER_NORMAL);
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL, changed: true);
			PSPParticleManager.GetInstance().Draw(dt, Game.game_work.pause, 1);
			if (Game.game_work.inBonusScreen)
			{
				PSPParticleManager.GetInstance().Draw(dt, Game.game_work.pause, 0);
			}
			for (int k = 0; k < 3; k++)
			{
				Game.game_work.hud.m_tint[k] = 1f;
			}
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			WaveManager.GetInstance().Draw();
			Game.game_work.hud.Draw(HUD.HUD_ORDER.HUD_ORDER_POST);
			if (Game.game_work.critHitTime > 0f)
			{
				Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL, changed: true);
				DrawCritHit();
			}
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			Game.game_work.hud.Draw(HUD.HUD_ORDER.HUD_ORDER_BEFORE_BOMB);
			if (Game.game_work.hitBombTime > 0f)
			{
				DrawBombHit();
			}
			Game.game_work.hud.Draw(HUD.HUD_ORDER.HUD_ORDER_AFTER_BOMB);
			for (int l = 0; l < 3; l++)
			{
				Game.game_work.hud.m_tint[l] = startTint[l];
			}
			if (s_startFadeInTime > 0f)
			{
				DrawStartFade();
			}
			PopOverControl.DrawFront();
		}
		if (unpause_game && Game.game_work.pause)
		{
			unpause_game = false;
			InputManager.GetInstance().ClearActions(StringFunctions.StringHash("Input/PauseMenu.txt"));
			debugMenu = false;
			Game.game_work.pause = !Game.game_work.pause;
		}
	}

	public static void ResetGameEntities()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < MAX_SLASHES; i++)
		{
			inputEnts[i].Reset();
		}
		LinkedListNode<Entity> iterator = null;
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			entity.m_pos.Y = (0f - Game.SCREEN_HEIGHT) * 1.5f;
			entity.m_vel.Y = -1.5f;
			((Bomb)entity).Chuck();
			entity.Update(0f);
		}
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			((Fruit)entity).Chuck();
			if (Game.game_work.inRetrySequence)
			{
				((Fruit)entity).m_isSliced = true;
			}
			if (!((Fruit)entity).m_isSliced)
			{
				Vector3 proj = s_bombHitPos - entity.m_pos;
				if (proj.LengthSquared() > 400f)
				{
					proj.Normalize();
					proj *= 20f;
				}
				entity.CollisionResponse(null, 0u, 0u, ref proj);
				((Fruit)entity).Slice();
			}
			entity.m_pos.Y = (0f - Game.SCREEN_HEIGHT) * 1.5f;
			entity.m_vel.Y = -1.5f;
			((Fruit)entity).m_pos2.Y = (0f - Game.SCREEN_HEIGHT) * 1.5f;
			((Fruit)entity).m_vel2.Y = -1.5f;
			entity.Update(0f);
		}
	}

	public static void RemoveFlashEntities()
	{
		LinkedListNode<Entity> iterator = null;
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB_BLAST, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB_BLAST, ref iterator))
		{
			entity.m_destroy = true;
			entity.m_dormant = true;
		}
	}

	public static void RetryLevel()
	{
		SuperFruitControl.StopAllPomegranates();
		Game.game_work.inRetrySequence = true;
		Game.game_work.retryTimer = RETRY_TIME_DELAY;
		WaveManager.GetInstance().ResetGlobalDt();
		Game.game_work.gameOver = true;
		for (int i = 0; i < SplatEntity.poolCount; i++)
		{
			SplatEntity splatEntity = SplatEntity.pool[i];
			splatEntity.m_fadeTime = Mortar.Math.MIN(RETRY_TIME_DELAY * 1.5f, splatEntity.m_fadeTime);
			splatEntity.m_fadeSpeed = 0.25f;
		}
		if (s_bombSound != null)
		{
			s_bombSound.SetVolume(0f);
		}
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_DANANANA_SCHWING);
		TheGame.pomegranateThrown = false;
		if (SuperFruitControl.IsInSuperFruitState())
		{
			LinkedListNode<Entity> iterator = null;
			for (Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); fruit != null; fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
			{
				fruit.m_dormant = true;
				fruit.m_destroy = true;
			}
		}
		SuperFruitControl.Clear();
	}

	public static void EndRetryLevel()
	{
		s_mainScreen.m_state = MainScreen.MS.MS_IN;
		s_mainScreen.m_transitionWait = 0.5f;
		Game.game_work.currentScore = 0;
		Game.game_work.saveData.go_head = (Game.game_work.saveData.go_body = (Game.game_work.saveData.go_fruit = (Game.game_work.saveData.go_fact = -1)));
		Game.game_work.levelStartCoins = Game.game_work.coins;
		ResetGameEntities();
		RemoveFlashEntities();
		WaveManager.GetInstance().Reset(newGame: true);
		Game.game_work.gameOverTransition = 0f;
		Game.game_work.gameOver = false;
		Game.game_work.inRetrySequence = false;
		Game.game_work.mainScreen.m_state = MainScreen.MS.MS_GAME;
	}

	public static void QuitToMenu()
	{
		SuperFruitControl.StopAllPomegranates();
		SuperFruitControl.Clear();
		TheGame.pomegranateThrown = false;
		WaveManager.GetInstance().ResetGlobalDt();
		Game.game_work.gameOver = true;
		s_mainScreen.m_state = MainScreen.MS.MS_IN;
		s_mainScreen.m_transitionWait = 0.5f;
		Game.game_work.currentScore = 0;
	}

	public static void HitBomb(Vector3 pos)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!Game.game_work.gameOver)
		{
			uint hash = StringFunctions.StringHash("bomb");
			Game.game_work.saveData.AddToTotal("bomb", hash, 1);
			Game.game_work.hitBombTime = HIT_BOMB_WAIT;
			Game.game_work.camera.CreateCameraShake(pos, HIT_BOMB_WAIT * 0.5f, 2f);
			s_bombHitPos = pos;
			s_menuBombHit = false;
			SoundManager.GetInstance().SFXPlay(SoundDef.SND_BOMB_EXPLODE);
		}
	}

	public static void HitMenuBomb(Vector3 pos)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_MENU_BOMB);
		Game.game_work.hitBombTime = BOMB_FLASH_START;
		s_bombHitPos = pos;
		s_menuBombHit = true;
	}

	public static bool BombFlashFull()
	{
		if (Game.game_work.hitBombTime < BOMB_FLASH_FULL)
		{
			return Game.game_work.hitBombTime < BOMB_FLASH_START_FADE;
		}
		return false;
	}

	public static void UpdateBombHit(float prev)
	{
		if (prev > BOMB_FLASH_FULL - 0.05f && Game.game_work.hitBombTime <= BOMB_FLASH_FULL - 0.05f)
		{
			ResetGameEntities();
		}
		if (Game.game_work.hitBombTime > 0f && Game.game_work.hitBombTime < BOMB_FLASH_FULL)
		{
			RemoveFlashEntities();
		}
	}

	public static void DrawStartFade()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (s_startFadeInTime > 0f)
		{
			Game.game_work.camera.SetupPerspective(FruitCamera.PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE, changed: true);
			float num = 1f;
			float num2 = 1f;
			if (s_startFadeInTime > 0.5f)
			{
				num2 = s_startFadeInTime - 0.5f;
				num2 *= 2f;
				num += (1f - Mortar.Math.CLAMP(num2, 0f, 1f)) * (1f - Mortar.Math.CLAMP(num2, 0f, 1f));
			}
			else
			{
				num2 = 0f;
				_ = s_startFadeInTime;
			}
			s_HBlogo.Set();
			MatrixManager.instance.Reset();
			MatrixManager.instance.Scale(new Vector3(Game.SCREEN_WIDTH + 1f, Game.SCREEN_HEIGHT + 1f, 1f) * num);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mortar.Math.CLAMP((int)(num2 * 255f), 0, 255);
		}
	}

	public static void DrawBombHit()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (s_flashTexture == null)
		{
			s_flashTexture = TextureManager.GetInstance().Load("textureswp7/flash.tex");
		}
		if (Game.game_work.hitBombTime < BOMB_FLASH_START)
		{
			float num = Mortar.Math.CLAMP(1f - (Game.game_work.hitBombTime - BOMB_FLASH_FULL) / (BOMB_FLASH_START - BOMB_FLASH_FULL), 0f, 1f) * 20000f;
			s_flashTexture.Set();
			MatrixManager.instance.Reset();
			MatrixManager.instance.Scale(new Vector3(num, num, 1f));
			MatrixManager.instance.Translate(s_bombHitPos);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(new Color(255, 255, 255, Mortar.Math.CLAMP((int)(Game.game_work.hitBombTime * 255f / BOMB_FLASH_START_FADE), 0, 255)));
		}
	}

	public static void CriticalFlash(Vector3 pos, Color col)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		s_critHitPos = pos;
		s_flashColor = col;
		Game.game_work.critHitTime = Fruit.CRITICAL_FLASH_TIME;
	}

	public static void DrawCritHit()
	{
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (s_flashTexture == null)
		{
			s_flashTexture = TextureManager.GetInstance().Load("textureswp7/flash.tex");
		}
		if (!(Game.game_work.critHitTime < Fruit.CRITICAL_FLASH_TIME))
		{
			return;
		}
		float v = Mortar.Math.CLAMP(1f - (Game.game_work.critHitTime - Fruit.CRITICAL_FLASH_FULL) / (Fruit.CRITICAL_FLASH_TIME - Fruit.CRITICAL_FLASH_FULL), 0f, 1f) * 15000f;
		s_flashTexture.Set();
		MatrixManager.instance.Reset();
		if (Game.IsMultiplayer())
		{
			MatrixManager.instance.Scale(new Vector3(Mortar.Math.MIN(v, Game.SCREEN_WIDTH / 2f), Mortar.Math.MIN(v, Game.SCREEN_HEIGHT), 1f));
			if (s_critHitPos.X < 0f)
			{
				MatrixManager.instance.Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 4f, 0f, 0f));
			}
			else
			{
				MatrixManager.instance.Translate(new Vector3(Game.SCREEN_WIDTH / 4f, 0f, 0f));
			}
		}
		else
		{
			MatrixManager.instance.Scale(new Vector3(Mortar.Math.MIN(v, Game.SCREEN_WIDTH), Mortar.Math.MIN(v, Game.SCREEN_HEIGHT), 1f) / 2f);
			MatrixManager.instance.Translate(Vector3.Zero);
		}
		MatrixManager.instance.UploadCurrentMatrices(quick: true);
		Mesh.DrawQuad(new Color((int)s_flashColor.R, (int)s_flashColor.G, (int)s_flashColor.B, Mortar.Math.CLAMP((int)(Game.game_work.critHitTime * (float)(int)s_flashColor.A / BOMB_FLASH_START_FADE), 0, s_flashColor.A)));
	}

	public static void ClearMenuItems()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		LinkedListNode<Entity> iterator = null;
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			if (!((Fruit)entity).m_isSliced)
			{
				((Fruit)entity).m_isSliced = true;
				((Fruit)entity).m_vel = new Vector3(Mortar.Math.g_random.RandF(10f) - 5f, Mortar.Math.g_random.RandF(5f), 0f);
				entity.m_vel.X = System.Math.Abs(entity.m_vel.X) * (float)Mortar.Math.MATH_SIGN(entity.m_pos.X);
				((Fruit)entity).m_vel2 = entity.m_vel;
			}
		}
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			if (((Bomb)entity).Enabled())
			{
				((Bomb)entity).Disable();
				entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(10f) - 5f, Mortar.Math.g_random.RandF(5f), 0f);
			}
			((Bomb)entity).EnableGravity(state: true);
		}
	}

	public static void SkipToPause(bool atStart)
	{
		if (atStart || (s_pauseScreen != null && s_pauseScreen.IsEnabled()))
		{
			Game.game_work.gameOverTransition = 0f;
			s_pauseScreen.SkipTo();
			Game.game_work.pause = true;
			Game.game_work.gameOver = false;
			s_mainScreen.Hide();
			Game.game_work.hud.Skip();
		}
	}

	public static void SkipToGameOver(int state, float time, float transition, float bombtime, int winner)
	{
		s_mainScreen.Hide();
		Game.game_work.hitBombTime = bombtime;
		Game.game_work.gameOverTransition = transition;
		Game.game_work.saveData.timer = 0f;
		if (Game.game_work.timeControl != null)
		{
			Game.game_work.timeControl.SetTime(0f);
		}
		Game.game_work.gameOver = false;
		if (state > -1 && time > -1f)
		{
			Game.GameOver(state, time, winner);
		}
		s_menuBombHit = false;
		Game.game_work.hud.Skip();
	}

	public static bool GetIsSavingBool()
	{
		return __isSaving;
	}

	public static void SetIsSavingBool(bool s)
	{
		__isSaving = s;
	}

	public static void SaveCurrentData(bool isClosingSave)
	{
		if (Game.game_work.saveData == null)
		{
			return;
		}
		SetIsSavingBool(s: true);
		ItemManager.GetInstance().SaveItemInfo();
		FruitSaveData fruitSaveData = Game.game_work.saveData.CLOAN();
		fruitSaveData.hasDropped = Game.game_work.hasDroppedFruit;
		fruitSaveData.score = Game.game_work.currentScore;
		fruitSaveData.misses = Game.game_work.currentMissCount;
		fruitSaveData.criticalProgression = Game.game_work.criticalChance;
		fruitSaveData.consecutiveType = Fruit.s_consecutiveType;
		fruitSaveData.consecutiveCount = Fruit.s_consecutiveCount;
		fruitSaveData.mode = (int)Game.game_work.gameMode;
		bool num;
		if (Game.game_work.gameMode != Game.GAME_MODE.GM_ARCADE)
		{
			if (!s_menuBombHit)
			{
				num = Game.game_work.hitBombTime > 0f;
				goto IL_00f0;
			}
		}
		else if (Game.game_work.gameOverTransition < 0.01f && Game.game_work.hitBombTime > 0f)
		{
			num = s_mainScreen.m_state == MainScreen.MS.MS_GAME;
			goto IL_00f0;
		}
		goto IL_0104;
		IL_010f:
		if (Game.game_work.gameOverScreen != null && !Game.game_work.inRetrySequence && (Game.game_work.gameOverScreen.GetState() == 0 || (Game.game_work.gameOverScreen.GetState() == 1 && Game.game_work.gameOverTransition < 1f)))
		{
			fruitSaveData.go_state = Game.game_work.gameOverScreen.GetState();
			fruitSaveData.go_time = Game.game_work.gameOverScreen.GetTime();
			fruitSaveData.go_transition = Game.game_work.gameOverTransition;
			fruitSaveData.go_head = Game.game_work.gameOverScreen.m_head;
			fruitSaveData.go_body = Game.game_work.gameOverScreen.m_body;
			fruitSaveData.go_fruit = Game.game_work.gameOverScreen.m_fruit;
			fruitSaveData.go_fact = Game.game_work.gameOverScreen.m_fact;
			fruitSaveData.go_setScore = Game.game_work.gameOverScreen.m_hasSetScore;
		}
		else
		{
			if (Game.game_work.gameOverScreen != null)
			{
				fruitSaveData.inGame = false;
				fruitSaveData.score = Game.game_work.currentScore;
			}
			else if (Game.game_work.inRetrySequence)
			{
				fruitSaveData.inGame = false;
			}
			fruitSaveData.go_state = -1;
			fruitSaveData.go_time = -1f;
			fruitSaveData.go_head = -1;
			fruitSaveData.go_body = -1;
			fruitSaveData.go_fruit = -1;
			fruitSaveData.go_fact = -1;
			fruitSaveData.go_showHighScore = false;
			fruitSaveData.go_setScore = false;
			fruitSaveData.go_transition = 0f;
		}
		fruitSaveData.shake_max_time = Game.game_work.camera.m_cameraShakeMaxTime;
		fruitSaveData.shake_time = Game.game_work.camera.m_cameraShakeTime;
		if (isClosingSave)
		{
			WaveManager.GetInstance().SaveWaveInfo(fruitSaveData);
		}
		else
		{
			fruitSaveData.inGame = false;
		}
		if (!Game.game_work.soundEnabled)
		{
			fruitSaveData.AddToTotal("soundOff", StringFunctions.StringHash("soundOff"), 1, universal: false);
		}
		if (!Game.game_work.musicEnabled)
		{
			fruitSaveData.AddToTotal("musicOff", StringFunctions.StringHash("musicOff"), 1, universal: false);
		}
		Save.SaveGame(fruitSaveData);
		SetIsSavingBool(s: false);
		return;
		IL_00f0:
		if (!num)
		{
			goto IL_0104;
		}
		fruitSaveData.go_bombHitTime = Game.game_work.hitBombTime;
		goto IL_010f;
		IL_0104:
		fruitSaveData.go_bombHitTime = 0f;
		goto IL_010f;
	}

	public static void GameExit()
	{
		Coin.ClearCoins(callDelegate: true);
		if (!GetIsSavingBool())
		{
			Game.game_work.hud.Save();
			SaveCurrentData(isClosingSave: true);
		}
		if (s_bombSound != null)
		{
			s_bombSound.SetVolume(0f);
			s_bombSound.Stop(0f);
			SoundManager.GetInstance().Release(s_bombSound);
			Delete.SAFE_DELETE(ref s_bombSound);
		}
		challengeOver = false;
		unpause_game = false;
		hud_font = null;
		challenge_font = null;
		s_sliceModel[0] = null;
		s_sliceModel[1] = null;
		for (int i = 0; i < MAX_SLASHES; i++)
		{
			inputEnts[i] = null;
		}
		s_slices.Clear();
		Delete.SAFE_DELETE(ref s_slices);
		s_flashTexture = null;
		PSPParticleManager.GetInstance().ClearEmitters();
		InputManager.GetInstance().ClearActions(0u);
		Game.game_work.hud.Release();
		Delete.SAFE_DELETE(ref Game.game_work.hud);
		Game.game_work.hud = null;
		MissControl.CleanPool();
		ActorManager.GetInstance().Clear();
		ActorManager.GetInstance().ClearAllListeners();
		Game.game_work.pause = false;
		initialised = false;
		backgroundTexture = null;
		s_HBlogo = null;
	}

	public static bool PauseGame(InputEvent e)
	{
		if (e != null)
		{
			if (!Game.game_work.pause)
			{
				PauseGame();
			}
			else
			{
				UnpauseGame();
			}
			return true;
		}
		return true;
	}

	public static void AddSlice(Vector3 pos, float angle, float mag, int model, Fruit waitFor, float speed)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		SliceEffect value = default(SliceEffect);
		value.time = 0f;
		value.mag = mag;
		value.angle = angle;
		value.pos = pos;
		value.isBlue = false;
		value.model = model;
		value.waitForFruit = waitFor;
		value.speed = speed;
		if (mag > 2.5f && Mortar.Math.g_random.Rand32(3) == 0)
		{
			SoundManager.GetInstance().SFXPlay(SoundDef.SND_VISCERAL_IMPACT);
		}
		int num = 0;
		for (LinkedListNode<SliceEffect> linkedListNode = s_slices.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SliceEffect value2 = linkedListNode.Value;
			if (waitFor != null && value2.waitForFruit == waitFor)
			{
				num++;
				if (num >= 2)
				{
					value2.waitForFruit = null;
					value2.time = SLICE_FRAMES;
				}
			}
		}
		s_slices.AddFirst(value);
	}

	public static void AddSlice(Vector3 pos, float angle, float mag, bool isBlue)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		mag *= 0.6f;
		SliceEffect value = default(SliceEffect);
		value.time = 0f;
		value.mag = mag;
		value.angle = angle;
		value.pos = pos;
		value.isBlue = isBlue;
		value.model = 0;
		value.waitForFruit = null;
		value.speed = 0f;
		if (mag > 2.5f && Mortar.Math.g_random.Rand32(3) == 0)
		{
			SoundManager.GetInstance().SFXPlay(SoundDef.SND_VISCERAL_IMPACT);
		}
		s_slices.AddFirst(value);
	}

	public static void DrawSlices(float dt)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		Matrix val2 = default(Matrix);
		for (LinkedListNode<SliceEffect> linkedListNode = s_slices.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SliceEffect value = linkedListNode.Value;
			value.time += dt * SLICE_SPEED * (value.isBlue ? 0.75f : 1f);
			if (value.time < SLICE_FRAMES)
			{
				if (value.time > 0f)
				{
					Vector3 val = (fruitSliceFrameScales[(int)value.time] + (fruitSliceFrameScales[(int)value.time + 1] - fruitSliceFrameScales[(int)value.time]) * (value.time - (float)(int)value.time)) * value.mag;
					Matrix.CreateScale(ref val, out val2);
					val2 *= Matrix.CreateRotationZ(MathHelper.ToRadians(value.angle));
					val2 *= Matrix.CreateTranslation(value.pos);
					s_sliceModel[value.isBlue ? 1 : 0].Draw(val2);
				}
				linkedListNode.Value = value;
			}
			else
			{
				s_slices.Remove(linkedListNode);
			}
		}
	}

	public static void UnpauseSlices()
	{
		for (LinkedListNode<SliceEffect> linkedListNode = s_slices.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			_ = linkedListNode.Value;
		}
	}

	public static void PauseGame()
	{
		unpauseDelay = UNPAUSE_DELAY_TIME;
		Game.game_work.pause = true;
		unpause_game = false;
	}

	public static void UnpauseGame()
	{
		repauseDelay = REPAUSE_DELAY_TIME;
		unpause_game = true;
	}

	public static float GetPauseAmount()
	{
		if (s_pauseScreen == null)
		{
			return 0f;
		}
		return Mortar.Math.CLAMP(s_pauseScreen.GetTime(), 0f, 1f);
	}

	public static int GetPausedBy()
	{
		return 0;
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

	public static float GetFruitZPosition()
	{
		GetFruitZPositionz -= 100f;
		if (GetFruitZPositionz < -2499f)
		{
			GetFruitZPositionz = -500f;
		}
		return GetFruitZPositionz;
	}

	public static void MoveFruitZPositionToBack(ref float z)
	{
		z += 500f;
		z /= 2f;
		z -= 2600f;
	}

	public static void ChangeBackground(string texture)
	{
		string texture2 = string.Format("textureswp7/{0}.tex", (texture != null) ? texture : "gb_game", "");
		backgroundTexture = TextureManager.GetInstance().Load(texture2);
	}

	public static void ChangeBackground(Texture texture)
	{
		backgroundTexture = texture;
	}

	public static Texture GetCurrentBackground()
	{
		return backgroundTexture;
	}

	public static int IsTouchDown(int touch)
	{
		if (touch >= 0 && touch < MAX_SLASHES)
		{
			if (Game.game_work.touchPositions[touch].Z == 1f)
			{
				return 1;
			}
			if (Game.game_work.touchPositions[touch].Z == 2f)
			{
				return 2;
			}
		}
		return 0;
	}

	public static int TouchInRegion(float xMin, float xMax, float yMin, float yMax, int touch)
	{
		if (touch >= 0 && touch < MAX_SLASHES && Game.game_work.touchPositions[touch].Z > 0f && Game.game_work.touchPositions[touch].X >= xMin && Game.game_work.touchPositions[touch].X <= xMax && Game.game_work.touchPositions[touch].Y >= yMin && Game.game_work.touchPositions[touch].Y <= yMax)
		{
			return touch;
		}
		for (int i = 0; i < MAX_SLASHES; i++)
		{
			if (Game.game_work.touchPositions[i].Z > 0f && Game.game_work.touchPositions[i].X >= xMin && Game.game_work.touchPositions[i].X <= xMax && Game.game_work.touchPositions[i].Y >= yMin && Game.game_work.touchPositions[i].Y <= yMax)
			{
				return i;
			}
		}
		return -1;
	}

	public static void GameTaskInitInput()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		InputManager.GetInstance().LoadConfigFile("Input/Input.txt");
		for (int i = 0; i < MAX_SLASHES; i++)
		{
			ref Vector3 reference = ref Game.game_work.touchPositions[i];
			reference = Vector3.Zero;
			inputEnts[i] = (SlashEntity)ActorManager.GetInstance().Add(EntityTypes.ENTITY_SLASH);
			Vector3 one = Vector3.One;
			inputEnts[i].Init(null, 0, one);
			string text = $"TouchMove_X{i}";
			string hash = $"TouchDown_{i}";
			InputManager.GetInstance().RegisterInputCallback(text, PointerMoveCallback);
			text = text.Replace('X', 'Y');
			InputManager.GetInstance().RegisterInputCallback(text, PointerMoveCallback);
			InputManager.GetInstance().RegisterInputCallback(hash, TouchDownCallback);
		}
	}

	private static void DrawBackground()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		backgroundTexture.Set();
		float bACKGROUND_UVS_U = BACKGROUND_UVS_U0;
		float bACKGROUND_UVS_U2 = BACKGROUND_UVS_U1;
		float bACKGROUND_UVS_V = BACKGROUND_UVS_V0;
		float bACKGROUND_UVS_V2 = BACKGROUND_UVS_V1;
		MatrixManager.GetInstance().Reset();
		if (Game.game_work.camera.ViewIsNormal())
		{
			Matrix matrix = Matrix.CreateScale(Game.SCREEN_WIDTH + 1f, Game.SCREEN_HEIGHT + 1f, 0f);
			MatrixManager.GetInstance().SetMatrix(matrix);
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(HUDControl.TintWhite(Game.game_work.hud.m_backTint), bACKGROUND_UVS_U, bACKGROUND_UVS_U2, bACKGROUND_UVS_V, bACKGROUND_UVS_V2);
			return;
		}
		MatrixManager.instance.Scale(new Vector3(Game.SCREEN_WIDTH + 1f, Game.SCREEN_HEIGHT + 1f, 0f));
		MatrixManager.instance.Translate(new Vector3(0f, 0f, -5999f));
		MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
		Color val = HUDControl.TintWhite(Game.game_work.hud.m_backTint);
		Color black = Color.Black;
		float num = 0.125f;
		bool flag = false;
		if (num <= 0f)
		{
			flag = true;
			num = 0.5f;
		}
		float num2 = num;
		float num3 = num * 1.5f;
		bool flag2 = flag;
		bool flag3 = flag2;
		int num4 = 0;
		for (int i = 0; i < 3; i++)
		{
			float num5 = ((i != 1 && flag2) ? bACKGROUND_UVS_U2 : bACKGROUND_UVS_U);
			float num6 = ((i != 1 && flag2) ? bACKGROUND_UVS_U : bACKGROUND_UVS_U2);
			float num7 = (float)i - 1.5f;
			float num8 = (float)i - 0.5f;
			switch (i)
			{
			case 0:
				num7 = num8 - num2;
				num5 = TransitionFunctions.LerpF(num6, num5, flag ? 0f : num2);
				break;
			case 2:
				num8 = num7 + num2;
				num6 = TransitionFunctions.LerpF(num5, num6, flag ? 0f : num2);
				break;
			}
			for (int j = 0; j < 3; j++)
			{
				float num9 = 1.5f - (float)j;
				float num10 = 0.5f - (float)j;
				float num11 = ((j != 1 && flag3) ? bACKGROUND_UVS_V2 : bACKGROUND_UVS_V);
				float num12 = ((j != 1 && flag3) ? bACKGROUND_UVS_V : bACKGROUND_UVS_V2);
				if (j == 0)
				{
					num9 = num10 + num3;
					num11 = TransitionFunctions.LerpF(num12, num11, flag ? 0f : num3);
				}
				if (j == 2)
				{
					num10 = num9 - num3;
					num12 = TransitionFunctions.LerpF(num11, num12, flag ? 0f : num3);
				}
				for (int k = 0; k < 4; k++)
				{
					verts[num4].X = (verts[num4].Y = (verts[num4].Z = (verts[num4].nx = (verts[num4].ny = 0f))));
					verts[num4].nz = 1f;
					bool flag4 = (i == 0 && k % 2 == 0) || (i == 2 && k % 2 == 1) || (j == 0 && k < 2) || (j == 2 && k >= 2);
					verts[num4].u = ((k % 2 == 0) ? num5 : num6);
					verts[num4].v = ((k < 2) ? num11 : num12);
					verts[num4].X = ((k % 2 == 0) ? num7 : num8);
					verts[num4].Y = ((k < 2) ? num9 : num10);
					Color color = (flag4 ? black : val);
					verts[num4].color = color;
					if (k == 1 || k == 2)
					{
						ref GameVertex reference = ref verts[num4 + 2];
						reference = verts[num4];
						if (k == 2)
						{
							num4++;
							num4++;
						}
					}
					num4++;
				}
			}
		}
		Mesh.DrawTriList(verts, 54, hasAlpha: false, 0);
	}

	static GameTask()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		Model[] array = new Model[2];
		s_sliceModel = array;
		fruitSliceFrameScales = new Vector3[7]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(1.7f, 0.3f, 1f),
			new Vector3(8f, 0.1f, 1f),
			new Vector3(20f, 0.1f, 1f),
			new Vector3(4f, 0.1f, 1f),
			new Vector3(0.1f, 0.1f, 0.1f),
			new Vector3(0.1f, 0.1f, 0.1f)
		};
		s_slices = null;
		missPositions = new float[3, 4]
		{
			{ 79f, 10f, -5f, 0.75f },
			{ 52f, 13f, 5f, 1f },
			{ 20f, 18f, 10f, 1.2f }
		};
		s_mainScreen = null;
		s_pauseScreen = null;
		END_SCREEN_DELAY = 0.33f;
		end_screen_delay = END_SCREEN_DELAY;
		s_bombHitPos = Vector3.Zero;
		s_critHitPos = Vector3.Zero;
		showdlg = true;
		s_menuBombHit = false;
		slowTime = 1f;
		slowTimeTime = 0f;
		slowTimeSpeed = 1f;
		paticlesDt = 1f;
		quickener = 1f;
		s_flash = 0;
		s_flashAlpha = 0;
		s_flashTex = null;
		startTint = new float[3];
		s_flashTexture = null;
		s_flashColor = Color.White;
		__isSaving = false;
		GetBombZPositionz = -10f;
		GetFruitZPositionz = -500f;
		verts = new GameVertex[54];
	}
}
