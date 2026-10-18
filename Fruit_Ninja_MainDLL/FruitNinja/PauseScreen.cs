using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Mortar;

namespace FruitNinja;

public class PauseScreen : HUDControl3d
{
	public enum PS
	{
		PS_DISABLED,
		PS_TO_MENU,
		PS_IN,
		PS_PAUSED,
		PS_UNPAUSE,
		PS_RETRY,
		PS_QUIT,
		PS_GAME
	}

	protected float m_time;

	protected Vector3 m_originalScale;

	protected Vector3 m_pauseButtonScale;

	protected MenuButton m_pauseButton;

	protected MenuButton m_quitButton;

	protected MenuButton m_resumeButton;

	protected MenuButton m_retryButton;

	protected MenuButton m_leaderboardsButton;

	protected MenuButton m_achivementsButton;

	protected float m_enabled;

	protected Texture m_pauseTexture;

	protected Texture m_playTexture;

	protected float m_repauseTime;

	private PS m_state;

	private static Texture s_flashTexture;

	public static float BEGINNING_WAIT => 0.15f;

	public static int PAUSE_TITLE_Y => 30;

	public static int RESUME_TITLE_Y => 145;

	public static int RETRY_TITLE_Y => 190;

	public static int QUIT_TITLE_Y => 255;

	public static int TOP_BUTTONS_Y => 120;

	public PauseScreen()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		m_texture = null;
		m_texture = TextureManager.GetInstance().Load("pause_title.tex", localise: true);
		m_repauseTime = 0f;
		m_originalScale = (m_scale = new Vector3((float)m_texture.GetWidth(), (float)m_texture.GetHeight(), 1f));
		m_state = PS.PS_DISABLED;
		m_selfCleanUp = false;
		m_time = 0f;
		m_pos = new Vector3(0f, (Game.SCREEN_HEIGHT - m_scale.Y) * 0.5f, 0f);
		m_terminate = false;
		m_resumeButton = null;
		m_quitButton = null;
		m_leaderboardsButton = null;
		m_achivementsButton = null;
		m_pauseButton = null;
		m_time = 0f;
		m_enabled = 1f;
		m_pauseButtonScale = Vector3.One;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
	}

	public void ContinueGameCallback()
	{
		if (m_state == PS.PS_PAUSED)
		{
			m_state = PS.PS_UNPAUSE;
		}
	}

	public void PauseGameCallback()
	{
		if (m_enabled == 0f)
		{
			if (m_state == PS.PS_DISABLED)
			{
				m_state = PS.PS_IN;
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_PAUSE);
				GameTask.PauseGame();
			}
			else if (m_state == PS.PS_PAUSED)
			{
				m_repauseTime = 2f;
				m_pauseButton.m_enabled = false;
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_UNPAUSE);
				m_state = PS.PS_UNPAUSE;
			}
		}
	}

	private void ExitGameCallback(IAsyncResult result)
	{
		bool flag = false;
		int? num = Guide.EndShowMessageBox(result);
		if (num.HasValue && num.HasValue && num.Value == 1)
		{
			flag = true;
		}
		if (flag)
		{
			Game.game_work.saveData.ClearTotals();
			Game.game_work.saveData.ClearCombo();
			m_state = PS.PS_QUIT;
			Game.game_work.loadedSaveState = false;
		}
	}

	public void QuitGameCallback()
	{
		try
		{
			if (m_state == PS.PS_PAUSED)
			{
				string text = TheGame.instance.stringTable.GetString(931);
				string text2 = TheGame.instance.stringTable.GetString(926);
				string text3 = TheGame.instance.stringTable.GetString(929);
				string text4 = TheGame.instance.stringTable.GetString(1009);
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
		}
		catch
		{
			if (m_state == PS.PS_PAUSED)
			{
				Game.game_work.saveData.ClearTotals();
				Game.game_work.saveData.ClearCombo();
				m_state = PS.PS_QUIT;
				Game.game_work.loadedSaveState = false;
			}
		}
	}

	public void QuitGameCallbackForLeaderboards()
	{
		PopOverControl.Instance.In(delegate
		{
			LeaderboardsScreen leaderboardsScreen = new LeaderboardsScreen();
			leaderboardsScreen.Init();
			Game.game_work.hud.AddControl(leaderboardsScreen);
		});
	}

	public void PlayerReview()
	{
	}

	public void RetryGameCallback()
	{
		if (m_state == PS.PS_PAUSED)
		{
			Game.game_work.saveData.ClearTotals();
			Game.game_work.saveData.ClearCombo();
			m_state = PS.PS_RETRY;
			Game.game_work.loadedSaveState = false;
		}
	}

	public override void Draw(float[] tintChannels)
	{
		if (m_time > 0f)
		{
			base.Draw(tintChannels);
		}
	}

	public override void Reset()
	{
	}

	public override void Release()
	{
		m_texture = null;
		m_playTexture = null;
		m_pauseTexture = null;
	}

	public override void Init()
	{
		Reset();
	}

	public override void Update(float dt)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Invalid comparison between Unknown and I4
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0937: Unknown result type (might be due to invalid IL or missing references)
		if (m_pauseButton == null)
		{
			m_pauseButton = new MenuButton("pause_button.tex", new Vector3(Game.SCREEN_WIDTH / 2f, (0f - Game.SCREEN_HEIGHT) / 2f, 0f), PauseGameCallback);
			m_pauseButton.Init();
			Game.game_work.hud.AddControl(m_pauseButton);
			m_pauseButton.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			m_pauseButtonScale = (m_pauseButton.m_originalScale = Vector3.One * 64f * Game.HUD_SCALE);
			m_pauseButton.m_outerBound = 500f;
			m_pauseButton.m_triggerOnBackPress = true;
			m_pauseButton.m_triggerOnEscape = false;
			m_pauseTexture = m_pauseButton.m_texture;
			m_playTexture = TextureManager.GetInstance().Load("textureswp7/play_button.tex");
		}
		if (m_quitButton == null)
		{
			m_quitButton = new MenuButton("quit_title.tex", new Vector3(0f, Game.SCREEN_HEIGHT, 0f), QuitGameCallback, -1, Vector3.Zero, localise: true);
			m_quitButton.Init();
			m_quitButton.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			MenuButton quitButton = m_quitButton;
			quitButton.m_originalScale *= Game.GAME_MODE_SCALE_FIX;
			Game.game_work.hud.AddControl(m_quitButton);
		}
		if (m_retryButton == null)
		{
			m_retryButton = new MenuButton("retry_button.tex", new Vector3(0f, Game.SCREEN_HEIGHT, 0f), RetryGameCallback);
			m_retryButton.Init();
			m_retryButton.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			MenuButton retryButton = m_retryButton;
			retryButton.m_originalScale *= Game.GAME_MODE_SCALE_FIX;
			Game.game_work.hud.AddControl(m_retryButton);
		}
		switch (m_state)
		{
		case PS.PS_TO_MENU:
			m_enabled = 0f;
			m_time = 1f;
			if (Game.BombFlashFull())
			{
				m_time = 0f;
				m_enabled = 1f;
				PowerUpManager.GetInstance().Reset();
				m_state = PS.PS_DISABLED;
			}
			break;
		case PS.PS_DISABLED:
			m_time *= 0.75f;
			if (m_time < 0.01f)
			{
				m_time = 0f;
			}
			if (m_repauseTime > 0f)
			{
				m_repauseTime -= dt;
				if (m_repauseTime <= 0f)
				{
					m_pauseButton.m_enabled = true;
				}
			}
			else
			{
				m_pauseButton.m_enabled = true;
			}
			if (TheGame.instance.EscapeButtonWasPressed && IsEnabled())
			{
				PauseGameCallback();
			}
			break;
		case PS.PS_IN:
			m_time += (1f - m_time) * 0.25f;
			Game.game_work.pause = true;
			if (m_time > 0.999f)
			{
				m_time = 1f;
				m_state = PS.PS_PAUSED;
			}
			break;
		case PS.PS_PAUSED:
		{
			m_pauseButton.m_enabled = true;
			Game.game_work.pause = true;
			GamePadState state = GamePad.GetState((PlayerIndex)0);
			GamePadButtons buttons = state.Buttons;
			if ((int)buttons.Back == 1)
			{
				ContinueGameCallback();
			}
			if (TheGame.instance.EscapeButtonWasPressed)
			{
				QuitGameCallback();
			}
			break;
		}
		case PS.PS_QUIT:
			m_time *= 0.5f;
			goto case PS.PS_UNPAUSE;
		case PS.PS_UNPAUSE:
		case PS.PS_RETRY:
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				m_time = 0f;
				if (m_state == PS.PS_QUIT)
				{
					Game.QuitToMenu();
					Game.HitMenuBomb(m_quitButton.m_pos);
					m_enabled = 0f;
					m_time = 1f;
					m_state = PS.PS_TO_MENU;
					GameTask.SaveCurrentData();
				}
				else if (m_state == PS.PS_RETRY)
				{
					GameTask.SaveCurrentData();
					m_enabled = 0f;
					m_time = 0f;
					m_state = PS.PS_DISABLED;
					m_repauseTime = 2f;
					GameTask.RetryLevel();
				}
				else
				{
					m_repauseTime = 2f;
					m_state = PS.PS_DISABLED;
				}
				GameTask.UnpauseGame();
			}
			else
			{
				Game.game_work.pause = true;
			}
			break;
		}
		if (IsEnabled())
		{
			m_enabled *= 0.75f;
			if (m_enabled < 0.001f)
			{
				m_enabled = 0f;
			}
		}
		else
		{
			m_enabled += (1f - m_enabled) * 0.25f;
		}
		float time = m_time;
		float enabled = m_enabled;
		if (m_state == PS.PS_QUIT)
		{
			m_enabled = 0f;
			m_time = 1f;
		}
		if (m_time > 0.5f)
		{
			m_pauseButton.m_texture = m_playTexture;
			m_pauseButton.m_rotation = 0f;
		}
		else
		{
			m_pauseButton.m_texture = m_pauseTexture;
			m_pauseButton.m_rotation = (Game.IsMultiplayer() ? 90 : 0);
		}
		m_pos.Y = Game.SCREEN_HEIGHT * 0.5f + m_scale.Y - (Game.SCREEN_HEIGHT * 0.5f - (float)PAUSE_TITLE_Y) * m_time;
		if (m_resumeButton != null)
		{
			m_resumeButton.m_pos.Y = Game.SCREEN_HEIGHT * 1.5f - (float)RESUME_TITLE_Y - Game.SCREEN_HEIGHT * m_time;
		}
		if (m_retryButton != null)
		{
			m_retryButton.m_pos.Y = -20f;
			m_retryButton.m_pos.X = 300f + -250f * m_time;
		}
		if (m_quitButton != null)
		{
			m_quitButton.m_pos.Y = 0f - (Game.SCREEN_HEIGHT / 2f - m_quitButton.m_originalScale.Y * 0.5f - 5f + (1f - m_time) * (m_quitButton.m_originalScale.Y + 10f));
			m_quitButton.m_pos.X = Game.SCREEN_WIDTH / 2f - m_quitButton.m_originalScale.X * 0.5f;
			m_quitButton.SetActive(m_time > 0.01f);
		}
		if (m_leaderboardsButton != null)
		{
			m_leaderboardsButton.m_pos.Y = 0f - (Game.SCREEN_HEIGHT / 2f - m_quitButton.m_originalScale.Y * 0.5f - 5f + (1f - m_time) * (m_quitButton.m_originalScale.Y + 10f));
			m_leaderboardsButton.m_pos.X = (0f - m_quitButton.m_originalScale.X) * 0.5f - 20f;
			m_leaderboardsButton.SetActive(m_time > 0.01f);
		}
		if (m_achivementsButton != null)
		{
			m_achivementsButton.m_pos.Y = 0f - (Game.SCREEN_HEIGHT / 2f - m_quitButton.m_originalScale.Y * 0.5f - 5f + (1f - m_time) * (m_quitButton.m_originalScale.Y + 10f));
			m_achivementsButton.m_pos.X = (0f - m_quitButton.m_originalScale.X) * 0.5f + 80f;
			m_achivementsButton.SetActive(m_time > 0.01f);
		}
		m_pauseButton.m_pos.Y = (0f - Game.SCREEN_HEIGHT) / 2f + m_pauseButton.m_originalScale.Y * 0.5f - 5f;
		if (Game.IsMultiplayer())
		{
			m_pauseButton.m_pos.X = 0f;
			ref Vector3 pos = ref m_pauseButton.m_pos;
			pos.Y -= Mortar.Math.Abs(m_enabled) * (m_pauseButton.m_originalScale.X + 10f);
		}
		else
		{
			m_pauseButton.m_pos.X = 0f - (Game.SCREEN_WIDTH / 2f - m_pauseButton.m_originalScale.X * 0.5f + 4f + Mortar.Math.Abs(m_enabled) * (m_pauseButton.m_originalScale.X + 10f));
		}
		MenuButton pauseButton = m_pauseButton;
		pauseButton.m_pos += (new Vector3(-50f, -20f, 0f) - m_pauseButton.m_pos) * m_time;
		m_pauseButton.m_originalScale = m_pauseButtonScale * (0.75f + m_time * 1.25f);
		m_pauseButton.SetActive(Mortar.Math.Abs(Game.game_work.gameOverTransition) < 0.99f);
		m_time = time;
		m_enabled = enabled;
	}

	public override void PreDraw(float[] tintChannels)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (m_time > 0f)
		{
			if (s_flashTexture == null)
			{
				s_flashTexture = TextureManager.GetInstance().Load("textureswp7/flash.tex");
			}
			float num = m_time * 10000f;
			s_flashTexture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3(num, num, 1f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(new Color(0f, 0f, 0f, Mortar.Math.CLAMP(m_time * 1000f, 0f, 128f) / 256f));
			s_flashTexture.UnSet();
		}
	}

	public void SkipTo()
	{
		m_state = PS.PS_PAUSED;
		m_time = 1f;
	}

	public bool IsEnabled()
	{
		if (Mortar.Math.Abs(Game.game_work.gameOverTransition) < 0.001f && Game.game_work.hitBombTime <= 0f)
		{
			return !Game.game_work.gameOver;
		}
		return false;
	}

	public float GetTime()
	{
		if (m_state != PS.PS_QUIT)
		{
			return m_time;
		}
		return 1f;
	}
}
