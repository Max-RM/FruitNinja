using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class DojoScreen : HUDControl3d
{
	public enum DS
	{
		DS_IN,
		DS_WAIT,
		DS_SHOP,
		DS_ABOUT,
		DS_TRAINING,
		DS_OUT
	}

	protected float m_time;

	protected MenuButton m_quitButton;

	protected MenuButton m_shopButton;

	protected MenuButton m_aboutButton;

	protected MenuButton m_ringtoneButton;

	private int m_state;

	private static Texture blankTex = null;

	private static Texture dojoTex = null;

	private static Texture titleTexture = null;

	private static Texture s_senseiTexture = null;

	private int omode;

	private int skip;

	private static bool firstRender = false;

	private static GameVertex[] top_tri = new GameVertex[3];

	private static GameVertex[] btm_tri = new GameVertex[3];

	public static float DOJO_SCREEN_HEIGHT => 320f;

	public static Vector3 MODE_SELECT_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 125f, (0f - Game.SCREEN_HEIGHT) / 2f + 30f, 0f);

	public static Vector3 TITLE_POS => new Vector3(Game.SCREEN_WIDTH / 2f - 58f, Game.SCREEN_HEIGHT / 2f - 23f, 0f);

	public static Vector3 SENSEI_SELECT_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 60f, (0f - Game.SCREEN_HEIGHT) / 2f + 113f, 0f);

	public DojoScreen()
	{
		m_texture = null;
		m_selfCleanUp = false;
		m_quitButton = null;
		m_shopButton = null;
		m_aboutButton = null;
		m_ringtoneButton = null;
		m_state = 0;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		m_time = 0f;
	}

	public void QuitCallback()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_MENU_BOMB);
		m_state = 5;
		((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
		m_quitButton.m_entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(5f) + 5f, 0f - Mortar.Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
	}

	public void ShopCallback()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!Game.isWP7TrialMode())
		{
			m_state = 2;
			((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
			m_quitButton.m_entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(5f) + 5f, 0f - Mortar.Math.g_random.RandF(5f), 0f);
			Game.game_work.tutorialControl.ResetTutePos();
		}
	}

	public void AboutCallback()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		m_state = 3;
		((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
		m_quitButton.m_entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(5f) + 5f, 0f - Mortar.Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
	}

	public void RingtoneCallback()
	{
		if (Game.isWP7TrialMode())
		{
			return;
		}
		try
		{
			TheGame.srtt.Source = new Uri("appdata:/myTone.wma");
			TheGame.srtt.DisplayName = "Fruit Ninja";
			TheGame.srtt.Show();
		}
		catch (Exception)
		{
		}
	}

	public static void LoadContent()
	{
		titleTexture = TextureManager.GetInstance().Load("textureswp7/sml_title.tex");
		blankTex = TextureManager.GetInstance().Load("textureswp7/loading.tex");
		dojoTex = TextureManager.GetInstance().Load("textureswp7/dojo.tex");
		s_senseiTexture = TextureManager.GetInstance().Load("textureswp7/dojo_sensei.tex");
	}

	public static void UnLoadContent()
	{
		titleTexture = null;
		blankTex = null;
		dojoTex = null;
		s_senseiTexture = null;
	}

	public override void Reset()
	{
		m_state = 0;
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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		switch (m_state)
		{
		case 0:
			m_time += (1f - m_time) * 0.25f;
			if (m_time > 0.95f)
			{
				if (m_quitButton == null)
				{
					m_quitButton = new MenuButton("back_icon.tex", new Vector3(425f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 266f, 0f), QuitCallback, Fruit.MAX_FRUIT_TYPES, Vector3.Zero, localise: true);
					m_quitButton.m_triggerOnBackPress = true;
					m_quitButton.Init();
					Game.game_work.hud.AddControl(m_quitButton);
					Game.game_work.tutorialControl.ResetTutePos(m_quitButton);
					MenuButton quitButton = m_quitButton;
					quitButton.m_originalScale *= 0.825f;
					Entity entity = m_quitButton.m_entity;
					entity.m_cur_scale *= 0.825f;
				}
				if (m_shopButton == null)
				{
					m_shopButton = new MenuButton("senseis_swag.tex", new Vector3(240f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 210f, 0f), ShopCallback, Fruit.FruitType("pineapple"), Vector3.Zero, localise: true);
					m_shopButton.m_isTrialLockable = true;
					m_shopButton.m_originalScale = new Vector3((float)(m_shopButton.m_texture.GetWidth() + 1), (float)(m_shopButton.m_texture.GetHeight() + 1), 1f) * Game.GAME_MODE_SCALE_FIX;
					m_shopButton.m_overallScratchScale = 0.5f;
					MenuButton shopButton3 = m_shopButton;
					shopButton3.m_newSymbolOffset *= 0.575f;
					m_shopButton.m_deleteCall = delegate(HUDControl control)
					{
						if (control == m_shopButton)
						{
							m_shopButton = null;
						}
					};
					m_shopButton.Init();
					Game.game_work.hud.AddControl(m_shopButton);
					Game.game_work.tutorialControl.ResetTutePos(m_shopButton);
					if (!Game.isWP7TrialMode())
					{
						m_shopButton.SetNewSymbol(ItemManager.GetInstance().AreNewItems());
					}
					else
					{
						m_shopButton.m_clearOthers = false;
					}
					if (Game.isWP7TrialMode())
					{
						omode = 1;
					}
					else
					{
						omode = 2;
					}
				}
				if (m_aboutButton == null)
				{
					m_aboutButton = new MenuButton("credits.tex", new Vector3(385f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 118f, 0f), AboutCallback, Fruit.FruitType("plum"), Vector3.Zero, localise: true);
					m_aboutButton.Init();
					Game.game_work.hud.AddControl(m_aboutButton);
				}
				if (m_ringtoneButton == null)
				{
					if (Game.isWP7TrialMode())
					{
						m_ringtoneButton = new MenuButton("ringtone.tex", new Vector3(150f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 70f, 0f), RingtoneCallback, Fruit.FruitType("banana_locked"), Vector3.Zero, localise: true);
					}
					else
					{
						m_ringtoneButton = new MenuButton("ringtone.tex", new Vector3(150f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 70f, 0f), RingtoneCallback, Fruit.FruitType("strawberry"), Vector3.Zero, localise: true);
					}
					m_ringtoneButton.Init();
					m_ringtoneButton.m_clearOthers = false;
					m_ringtoneButton.m_isTrialLockable = true;
					m_ringtoneButton.m_deleteCall = delegate(HUDControl control)
					{
						if (control == m_ringtoneButton)
						{
							m_ringtoneButton = null;
						}
					};
					Game.game_work.hud.AddControl(m_ringtoneButton);
				}
			}
			if (m_time > 0.999f)
			{
				m_time = 1f;
				m_state = 1;
			}
			break;
		case 1:
			if (m_shopButton == null)
			{
				if (skip == 0)
				{
					skip = 1;
					TheGame.instance.DoUpsell(exitGame: false);
					break;
				}
				if (TheGame.instance.UpsellComplete())
				{
					skip = 0;
					m_shopButton = new MenuButton("senseis_swag.tex", new Vector3(240f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 210f, 0f), ShopCallback, Fruit.FruitType("pineapple"), Vector3.Zero, localise: true);
					m_shopButton.m_isTrialLockable = true;
					m_shopButton.m_originalScale = new Vector3((float)(m_shopButton.m_texture.GetWidth() + 1), (float)(m_shopButton.m_texture.GetHeight() + 1), 1f) * Game.GAME_MODE_SCALE_FIX;
					m_shopButton.m_overallScratchScale = 0.5f;
					MenuButton shopButton = m_shopButton;
					shopButton.m_newSymbolOffset *= 0.575f;
					m_shopButton.m_deleteCall = delegate(HUDControl control)
					{
						if (control == m_shopButton)
						{
							m_shopButton = null;
						}
					};
					m_shopButton.Init();
					Game.game_work.hud.AddControl(m_shopButton);
					Game.game_work.tutorialControl.ResetTutePos(m_shopButton);
					if (!Game.isWP7TrialMode())
					{
						m_shopButton.SetNewSymbol(ItemManager.GetInstance().AreNewItems());
					}
					else
					{
						m_shopButton.m_clearOthers = false;
					}
				}
			}
			else if (!Game.isWP7TrialMode() && omode == 1)
			{
				omode = 2;
				m_shopButton.RemoveNoShow();
				Game.game_work.hud.RemoveControl(m_shopButton);
				m_shopButton = null;
				GC.Collect();
				m_shopButton = new MenuButton("senseis_swag.tex", new Vector3(240f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 210f, 0f), ShopCallback, Fruit.FruitType("pineapple"), Vector3.Zero, localise: true);
				m_shopButton.m_isTrialLockable = true;
				m_shopButton.m_originalScale = new Vector3((float)(m_shopButton.m_texture.GetWidth() + 1), (float)(m_shopButton.m_texture.GetHeight() + 1), 1f) * Game.GAME_MODE_SCALE_FIX;
				m_shopButton.m_overallScratchScale = 0.5f;
				MenuButton shopButton2 = m_shopButton;
				shopButton2.m_newSymbolOffset *= 0.575f;
				m_shopButton.m_deleteCall = delegate(HUDControl control)
				{
					if (control == m_shopButton)
					{
						m_shopButton = null;
					}
				};
				m_shopButton.Init();
				Game.game_work.hud.AddControl(m_shopButton);
				Game.game_work.tutorialControl.ResetTutePos(m_shopButton);
				if (!Game.isWP7TrialMode())
				{
					m_shopButton.SetNewSymbol(ItemManager.GetInstance().AreNewItems());
				}
				else
				{
					m_shopButton.m_clearOthers = false;
				}
			}
			else if (m_shopButton != null)
			{
				m_shopButton.SetNewSymbol(ItemManager.GetInstance().AreNewItems());
			}
			if (m_ringtoneButton != null)
			{
				break;
			}
			if (skip == 0)
			{
				skip = 1;
				TheGame.instance.DoUpsell(exitGame: false);
			}
			else
			{
				if (!TheGame.instance.UpsellComplete())
				{
					break;
				}
				skip = 0;
				if (Game.isWP7TrialMode())
				{
					m_ringtoneButton = new MenuButton("ringtone.tex", new Vector3(150f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 70f, 0f), RingtoneCallback, Fruit.FruitType("banana_locked"), Vector3.Zero, localise: true);
				}
				else
				{
					m_ringtoneButton = new MenuButton("ringtone.tex", new Vector3(150f - Game.SCREEN_WIDTH / 2f, DOJO_SCREEN_HEIGHT / 2f - 70f, 0f), RingtoneCallback, Fruit.FruitType("strawberry"), Vector3.Zero, localise: true);
				}
				m_ringtoneButton.Init();
				m_ringtoneButton.m_clearOthers = false;
				m_ringtoneButton.m_isTrialLockable = true;
				m_ringtoneButton.m_deleteCall = delegate(HUDControl control)
				{
					if (control == m_ringtoneButton)
					{
						m_ringtoneButton = null;
					}
				};
				Game.game_work.hud.AddControl(m_ringtoneButton);
			}
			break;
		case 2:
		case 3:
			m_time *= 0.75f;
			if (m_time > 0f && m_time <= 0.001f)
			{
				m_quitButton = null;
				m_shopButton = null;
				m_aboutButton = null;
				m_ringtoneButton = null;
				if (m_state == 3)
				{
					CreditsScreen creditsScreen = new CreditsScreen(this);
					creditsScreen.Init();
					Game.game_work.hud.AddControl(creditsScreen);
				}
				else if (m_state == 2)
				{
					ShopScreen shopScreen = new ShopScreen(this);
					shopScreen.Init();
					Game.game_work.hud.AddControl(shopScreen);
				}
				m_time = 0f;
			}
			break;
		case 5:
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				Game.game_work.mainScreen.m_state = MainScreen.MS.MS_RETURN;
				m_terminate = true;
			}
			break;
		case 4:
			break;
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
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
		if (m_time > 0f)
		{
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)(s_senseiTexture.GetWidth() + 1), (float)(s_senseiTexture.GetHeight() + 1), 0f));
			MatrixManager.GetInstance().Translate(SENSEI_SELECT_POS - Vector3.UnitX * (float)s_senseiTexture.GetWidth() * (1f - m_time));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			s_senseiTexture.Set();
			Mesh.DrawQuad(Color.White);
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Translate(new Vector3(Game.SCREEN_WIDTH / 2f, Game.SCREEN_HEIGHT / 2f - 48f * m_time, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawTriList(top_tri, 3, hasAlpha: true);
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 2f, (0f - Game.SCREEN_HEIGHT) / 2f + 60f * m_time, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawTriList(btm_tri, 3, hasAlpha: true);
			s_senseiTexture.UnSet();
			dojoTex.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)dojoTex.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)dojoTex.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 0f));
			MatrixManager.GetInstance().Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 56f, (0f - Game.SCREEN_HEIGHT) / 2f + 24f - (1f - m_time) * 60f, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White);
			dojoTex.UnSet();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)(titleTexture.GetWidth() + 1), (float)(titleTexture.GetHeight() + 1), 0f));
			MatrixManager.GetInstance().Translate(TITLE_POS + Vector3.UnitY * 48f * (1f - m_time));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			titleTexture.Set();
			Mesh.DrawQuad(Color.White);
			titleTexture.UnSet();
		}
	}

	public void ButtonDeleted(HUDControl control)
	{
		if (control == m_shopButton)
		{
			m_shopButton = null;
		}
	}
}
