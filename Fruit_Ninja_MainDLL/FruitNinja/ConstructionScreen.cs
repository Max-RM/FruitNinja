using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ConstructionScreen : HUDControl3d
{
	public enum AS
	{
		AS_IN,
		AS_WAIT,
		AS_OUT
	}

	protected float m_time;

	protected Vector3 m_originalScale;

	protected MenuButton m_quitButton;

	protected static Texture s_boardTexture;

	protected static Texture s_boardTexture2;

	protected static Texture m_creditsTexture;

	protected static Texture m_senseiTexture;

	public DojoScreen m_dojoScreen;

	private int m_state;

	private int m_mode;

	public static int SENSEI_CENTRE_X => 395;

	public static int SENSEI_CENTRE_Y => 104;

	public static int CREDITS_CENTRE_X => 190;

	public static int CREDITS_CENTRE_Y => 256;

	public static int ABOUT_CENTRE_X => 190;

	public static int ABOUT_CENTRE_Y => 97;

	public static float ABOUT_SCREEN_HEIGHT => 320f;

	public static string VERSION_TITLE => "VERSION:";

	public ConstructionScreen(DojoScreen dojo, int mode)
	{
		m_dojoScreen = dojo;
		m_mode = mode;
		m_texture = ((mode == 0) ? s_boardTexture : s_boardTexture2);
		m_selfCleanUp = false;
		m_quitButton = null;
		m_state = 0;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		m_time = 0f;
	}

	public void QuitGameCallback()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_MENU_BOMB);
		m_state = 2;
		((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
		m_quitButton.m_entity.m_vel = new Vector3(Math.g_random.RandF(5f) + 5f, 0f - Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
	}

	public static void LoadContent()
	{
		s_boardTexture = TextureManager.GetInstance().Load("textureswp7/swag_soon.tex");
		s_boardTexture2 = TextureManager.GetInstance().Load("textureswp7/arcade_soon.tex");
	}

	public static void UnLoadContent()
	{
		s_boardTexture = null;
		m_creditsTexture = null;
		m_senseiTexture = null;
		s_boardTexture2 = null;
	}

	public override void Reset()
	{
	}

	public override void Release()
	{
		m_texture = null;
		m_time = 0f;
	}

	public override void Init()
	{
		Reset();
	}

	public override void Update(float dt)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (Game.isWP7TrialMode())
		{
			Game.ShowBuyMessageBox();
			if (m_mode == 0)
			{
				m_dojoScreen.Reset();
			}
			else
			{
				GameModeScreen gameModeScreen = new GameModeScreen(isMultiplayer: false);
				gameModeScreen.Init();
				Game.game_work.hud.AddControl(gameModeScreen);
			}
			m_terminate = true;
			return;
		}
		switch (m_state)
		{
		case 0:
			m_time += (1f - m_time) * 0.125f;
			if (m_time > 0.999f)
			{
				m_time = 1f;
				m_quitButton = new MenuButton("back_icon.tex", new Vector3(425f - Game.SCREEN_WIDTH / 2f, ABOUT_SCREEN_HEIGHT / 2f - 266f, 0f), QuitGameCallback, Fruit.MAX_FRUIT_TYPES, Vector3.Zero, localise: true);
				m_quitButton.Init();
				Game.game_work.hud.AddControl(m_quitButton);
				Game.game_work.tutorialControl.ResetTutePos(m_quitButton);
				MenuButton quitButton = m_quitButton;
				quitButton.m_originalScale *= 0.825f;
				Entity entity = m_quitButton.m_entity;
				entity.m_cur_scale *= 0.825f;
				m_state = 1;
			}
			break;
		case 2:
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				if (m_mode == 0)
				{
					m_dojoScreen.Reset();
				}
				else
				{
					GameModeScreen gameModeScreen2 = new GameModeScreen(isMultiplayer: false);
					gameModeScreen2.Init();
					Game.game_work.hud.AddControl(gameModeScreen2);
				}
				m_terminate = true;
			}
			break;
		case 1:
			break;
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			m_texture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3(480f, 320f, 1f));
			float num = ABOUT_SCREEN_HEIGHT / 2f + (float)m_texture.GetHeight() * 0.5f + 40f;
			float num2 = 0f;
			float num3 = num - (num - num2) * m_time;
			MatrixManager.GetInstance().Translate(new Vector3(0f, num3, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_texture.UnSet();
		}
		if (m_creditsTexture != null)
		{
			m_creditsTexture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)m_creditsTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)m_creditsTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 1f));
			float num4 = 0f - Game.SCREEN_HEIGHT / 2f - (float)m_creditsTexture.GetHeight() * 0.5f * Game.GAME_MODE_SCALE_FIX;
			float num5 = 0f - Game.SCREEN_HEIGHT / 2f + (float)(320 - CREDITS_CENTRE_Y);
			float num6 = num4 - (num4 - num5) * m_time;
			MatrixManager.GetInstance().Translate(new Vector3((float)CREDITS_CENTRE_X - Game.SCREEN_WIDTH / 2f, num6, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_creditsTexture.UnSet();
		}
		if (m_senseiTexture != null)
		{
			m_senseiTexture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)m_senseiTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)m_senseiTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 1f));
			float num7 = (float)m_senseiTexture.GetWidth() * 0.5f + Game.SCREEN_WIDTH / 2f;
			float num8 = (float)SENSEI_CENTRE_X - Game.SCREEN_WIDTH / 2f;
			float num9 = num7 - (num7 - num8) * m_time;
			MatrixManager.GetInstance().Translate(new Vector3(num9, ABOUT_SCREEN_HEIGHT / 2f - (float)SENSEI_CENTRE_Y, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White);
			m_senseiTexture.UnSet();
		}
	}
}
