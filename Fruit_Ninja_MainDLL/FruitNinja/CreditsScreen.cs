using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mortar;

namespace FruitNinja;

public class CreditsScreen : HUDControl3d
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

	protected static Texture m_creditsTexture;

	protected static Texture m_senseiTexture;

	public DojoScreen m_dojoScreen;

	private int m_state;

	private static float sx = 38f;

	private static float sy = 80f;

	public static int SENSEI_CENTRE_X => 424;

	public static int SENSEI_CENTRE_Y => 104;

	public static int CREDITS_CENTRE_X => 180;

	public static int CREDITS_CENTRE_Y => 160;

	public static int ABOUT_CENTRE_X => 190;

	public static int ABOUT_CENTRE_Y => 97;

	public static float ABOUT_SCREEN_HEIGHT => 320f;

	public static string VERSION_TITLE => "VERSION:";

	public CreditsScreen(DojoScreen dojo)
	{
		m_dojoScreen = dojo;
		m_texture = s_boardTexture;
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
		m_creditsTexture = TextureManager.GetInstance().Load("credits_screen.tex", localise: true);
		m_senseiTexture = TextureManager.GetInstance().Load("textureswp7/sensei.tex");
	}

	public static void UnLoadContent()
	{
		s_boardTexture = null;
		m_creditsTexture = null;
		m_senseiTexture = null;
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
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Invalid comparison between Unknown and I4
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		switch (m_state)
		{
		case 0:
			m_time += (1f - m_time) * 0.125f;
			if (m_time > 0.999f)
			{
				m_time = 1f;
				m_quitButton = new MenuButton("back_icon.tex", new Vector3(425f - Game.SCREEN_WIDTH / 2f, ABOUT_SCREEN_HEIGHT / 2f - 266f, 0f), QuitGameCallback, Fruit.MAX_FRUIT_TYPES, Vector3.Zero, localise: true);
				m_quitButton.Init();
				m_quitButton.m_triggerOnBackPress = true;
				Game.game_work.hud.AddControl(m_quitButton);
				Game.game_work.tutorialControl.ResetTutePos(m_quitButton);
				MenuButton quitButton = m_quitButton;
				quitButton.m_originalScale *= 0.825f;
				Entity entity = m_quitButton.m_entity;
				entity.m_cur_scale *= 0.825f;
				m_state = 1;
			}
			break;
		case 1:
		{
			GamePadState state = GamePad.GetState((PlayerIndex)0);
			GamePadButtons buttons = state.Buttons;
			if ((int)buttons.Back == 1)
			{
				QuitGameCallback();
			}
			break;
		}
		case 2:
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				m_dojoScreen.Reset();
				m_terminate = true;
			}
			break;
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			m_texture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)m_texture.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)m_texture.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 1f));
			float num = ABOUT_SCREEN_HEIGHT / 2f + (float)m_texture.GetHeight() * 0.5f;
			float num2 = ABOUT_SCREEN_HEIGHT / 2f - (float)ABOUT_CENTRE_Y - 60f;
			float num3 = num - (num - num2) * m_time;
			MatrixManager.GetInstance().Translate(new Vector3((float)ABOUT_CENTRE_X - Game.SCREEN_WIDTH / 2f, num3, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_texture.UnSet();
		}
		if (m_creditsTexture != null)
		{
			m_creditsTexture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3(((float)m_creditsTexture.GetWidth() - sx) * Game.GAME_MODE_SCALE_FIX + 1f, ((float)m_creditsTexture.GetHeight() - sy) * Game.GAME_MODE_SCALE_FIX + 1f, 1f));
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
