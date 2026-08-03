using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Mortar;

namespace FruitNinja;

public class AboutScreen : HUDControl3d
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

	protected static MenuButton m_englishTexture;

	protected static MenuButton m_frenchTexture;

	protected static MenuButton m_italianTexture;

	protected static MenuButton m_germanTexture;

	protected static MenuButton m_spanishTexture;

	protected static MenuButton m_chineseTexture;

	private static bool init = false;

	private int m_state;

	public static float AboutScreenTime = -1f;

	private float timer;

	private int show;

	public static int SENSEI_CENTRE_X => 400;

	public static int SENSEI_CENTRE_Y => 104;

	public static int CREDITS_CENTRE_X => 192;

	public static int CREDITS_CENTRE_Y => 265;

	public static int ABOUT_CENTRE_X => 190;

	public static int ABOUT_CENTRE_Y => 97;

	public static float ABOUT_SCREEN_HEIGHT => 320f;

	public static string VERSION_TITLE => "VERSION:";

	public AboutScreen(MainScreen dojo)
	{
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
		m_quitButton.m_entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(5f) + 5f, 0f - Mortar.Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
	}

	public static void LoadContent()
	{
		s_boardTexture = TextureManager.GetInstance().Load("haikus.tex", localise: true);
		m_creditsTexture = TextureManager.GetInstance().Load("options.tex", localise: true);
		m_senseiTexture = TextureManager.GetInstance().Load("textureswp7/sensei.tex");
		AboutScreenTime = 0f - Game.SCREEN_HEIGHT / 2f - (float)m_creditsTexture.GetHeight() * 0.5f * Game.GAME_MODE_SCALE_FIX;
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
		timer = 0f;
		show = 0;
	}

	private static void SwitchToLanguageCallback(IAsyncResult result)
	{
		bool flag = false;
		int? num = Guide.EndShowMessageBox(result);
		if (num.HasValue)
		{
			Guide.EndShowMessageBox(result);
			if (num.HasValue && num.Value == 1)
			{
				flag = true;
			}
		}
		if (flag)
		{
			TheGame.switchLanguage = true;
		}
	}

	private void ShowSwitchToLanguageDialog(StringTableUtils.Language language)
	{
		TheGame.switchToLanguage = language;
		string[] array = new string[2]
		{
			TheGame.instance.stringTable.GetString(926),
			TheGame.instance.stringTable.GetString(931)
		};
		try
		{
			if (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			string text = TheGame.instance.stringTable.GetString(1183);
			string text2 = TheGame.instance.stringTable.GetString(1184);
			Guide.BeginShowMessageBox(text, text2, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)SwitchToLanguageCallback, (object)null);
		}
		catch
		{
		}
	}

	private void Callback_English()
	{
		if (Game.game_work.language != StringTableUtils.Language.LANGUAGE_ENGLISH_UK && Game.game_work.language != StringTableUtils.Language.LANGUAGE_ENGLISH)
		{
			ShowSwitchToLanguageDialog(StringTableUtils.Language.LANGUAGE_ENGLISH);
		}
	}

	private void Callback_French()
	{
		if (Game.game_work.language != StringTableUtils.Language.LANGUAGE_FRENCH)
		{
			ShowSwitchToLanguageDialog(StringTableUtils.Language.LANGUAGE_FRENCH);
		}
	}

	private void Callback_Italian()
	{
		if (Game.game_work.language != StringTableUtils.Language.LANGUAGE_ITALIAN)
		{
			ShowSwitchToLanguageDialog(StringTableUtils.Language.LANGUAGE_ITALIAN);
		}
	}

	private void Callback_German()
	{
		if (Game.game_work.language != StringTableUtils.Language.LANGUAGE_GERMAN)
		{
			ShowSwitchToLanguageDialog(StringTableUtils.Language.LANGUAGE_GERMAN);
		}
	}

	private void Callback_Spanish()
	{
		if (Game.game_work.language != StringTableUtils.Language.LANGUAGE_SPANISH)
		{
			ShowSwitchToLanguageDialog(StringTableUtils.Language.LANGUAGE_SPANISH);
		}
	}

	private void Callback_Chinese()
	{
		if (Game.game_work.language != StringTableUtils.Language.LANGUAGE_CHINESE_SIMPLIFIED)
		{
			ShowSwitchToLanguageDialog(StringTableUtils.Language.LANGUAGE_CHINESE_SIMPLIFIED);
		}
	}

	public override void Update(float dt)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		timer += dt;
		if (timer > 3f)
		{
			timer = 0f;
			show ^= 1;
		}
		if (!init)
		{
			init = true;
			Vector3 scale = default(Vector3);
			scale = new Vector3(32f, 32f, 1f);
			if (m_englishTexture == null)
			{
				m_englishTexture = new MenuButton("english.tex", new Vector3(0f, 0f, 0f), Callback_English, -1, scale, localise: false);
				m_englishTexture.Init();
				Game.game_work.hud.AddControl(m_englishTexture);
				m_englishTexture.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			}
			if (m_frenchTexture == null)
			{
				m_frenchTexture = new MenuButton("francais.tex", new Vector3(0f, 0f, 0f), Callback_French, -1, scale, localise: false);
				m_frenchTexture.Init();
				Game.game_work.hud.AddControl(m_frenchTexture);
				m_frenchTexture.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			}
			if (m_italianTexture == null)
			{
				m_italianTexture = new MenuButton("italiano.tex", new Vector3(0f, 0f, 0f), Callback_Italian, -1, scale, localise: false);
				m_italianTexture.Init();
				Game.game_work.hud.AddControl(m_italianTexture);
				m_italianTexture.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			}
			if (m_germanTexture == null)
			{
				m_germanTexture = new MenuButton("deusch.tex", new Vector3(0f, 0f, 0f), Callback_German, -1, scale, localise: false);
				m_germanTexture.Init();
				Game.game_work.hud.AddControl(m_germanTexture);
				m_germanTexture.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			}
			if (m_spanishTexture == null)
			{
				m_spanishTexture = new MenuButton("espanol.tex", new Vector3(0f, 0f, 0f), Callback_Spanish, -1, scale, localise: false);
				m_spanishTexture.Init();
				Game.game_work.hud.AddControl(m_spanishTexture);
				m_spanishTexture.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			}
			if (m_chineseTexture == null)
			{
				m_chineseTexture = new MenuButton("chinese.tex", new Vector3(0f, 0f, 0f), Callback_Chinese, -1, scale, localise: false);
				m_chineseTexture.Init();
				Game.game_work.hud.AddControl(m_chineseTexture);
				m_chineseTexture.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			}
		}
		float num = -80f;
		if (GameTask.GetPauseAmount() == 0f)
		{
			float num2 = 33f;
			float num3 = 0f - Game.SCREEN_HEIGHT / 2f - (float)m_creditsTexture.GetHeight() * 0.5f * Game.GAME_MODE_SCALE_FIX;
			float num4 = 0f - Game.SCREEN_HEIGHT / 2f + (float)(320 - (CREDITS_CENTRE_Y + 10));
			float num5 = num3 - (num3 - num4) * m_time;
			if (m_englishTexture != null)
			{
				m_englishTexture.m_pos.Y = num5;
				m_englishTexture.m_pos.X = num;
				num += num2;
			}
			if (m_frenchTexture != null)
			{
				m_frenchTexture.m_pos.Y = num5;
				m_frenchTexture.m_pos.X = num;
				num += num2;
			}
			if (m_italianTexture != null)
			{
				m_italianTexture.m_pos.Y = num5;
				m_italianTexture.m_pos.X = num;
				num += num2;
			}
			if (m_germanTexture != null)
			{
				m_germanTexture.m_pos.Y = num5;
				m_germanTexture.m_pos.X = num;
				num += num2;
			}
			if (m_spanishTexture != null)
			{
				m_spanishTexture.m_pos.Y = num5;
				m_spanishTexture.m_pos.X = num;
				num += num2;
			}
			if (m_chineseTexture != null)
			{
				m_chineseTexture.m_pos.Y = num5;
				m_chineseTexture.m_pos.X = num;
				num += num2;
			}
			AboutScreenTime = num5;
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
				Game.game_work.mainScreen.m_state = MainScreen.MS.MS_RETURN;
				m_terminate = true;
			}
			break;
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (m_texture != null)
			{
				m_texture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3(400f * Game.GAME_MODE_SCALE_FIX, 320f * Game.GAME_MODE_SCALE_FIX, 1f));
				float num = ABOUT_SCREEN_HEIGHT / 2f + 110f;
				float num2 = ABOUT_SCREEN_HEIGHT / 2f - (float)ABOUT_CENTRE_Y - 16f;
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
				MatrixManager.GetInstance().Scale(new Vector3(390f * Game.GAME_MODE_SCALE_FIX + 1f, 160f * Game.GAME_MODE_SCALE_FIX + 1f, 1f));
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
				MatrixManager.GetInstance().Scale(new Vector3((float)m_senseiTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX, (float)m_senseiTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX, 1f));
				float num7 = (float)m_senseiTexture.GetWidth() * 0.5f + Game.SCREEN_WIDTH / 2f;
				float num8 = (float)SENSEI_CENTRE_X - Game.SCREEN_WIDTH / 2f;
				float num9 = num7 - (num7 - num8) * m_time;
				MatrixManager.GetInstance().Translate(new Vector3(num9, ABOUT_SCREEN_HEIGHT / 2f - (float)SENSEI_CENTRE_Y, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(Color.White);
				m_senseiTexture.UnSet();
			}
			if (m_state == 1)
			{
				string text = FNConstants.VersionString + "  ";
				text = Game.game_work.language switch
				{
					StringTableUtils.Language.LANGUAGE_FRENCH => text + "Contactez-nous:", 
					StringTableUtils.Language.LANGUAGE_ITALIAN => text + "Contatti:", 
					StringTableUtils.Language.LANGUAGE_GERMAN => text + "Kontakt:", 
					StringTableUtils.Language.LANGUAGE_SPANISH => text + "Contacte con nosotros:", 
					_ => text + "Contact us:", 
				} + " support@halfbrick.com";
				TheGame.instance.spriteBatch.Begin();
				TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text, new Vector2(8f, 8f), Color.Black);
				TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text, new Vector2(7f, 7f), Color.BlanchedAlmond);
				TheGame.instance.spriteBatch.End();
			}
		}
		catch
		{
		}
	}
}
