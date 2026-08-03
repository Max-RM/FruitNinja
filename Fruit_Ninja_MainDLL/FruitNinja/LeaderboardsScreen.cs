using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mortar;

namespace FruitNinja;

internal class LeaderboardsScreen : HUDControl3d
{
	private enum AS
	{
		AS_IN,
		AS_WAIT,
		AS_OUT,
		AS_START_LB_READ,
		AS_WAITING_LB_READ,
		AS_READ_LB_SUCCESS,
		AS_READ_LB_FAILED,
		AS_READ_LB_DISPLAY,
		AS_READ_LB_PAGE_UP,
		AS_READ_LB_PAGE_DOWN,
		AS_READ_LB_WAIT_PAGE,
		AS_MODE_SELECT
	}

	private enum ScrollState
	{
		Idle,
		Scroll
	}

	public class LeaderboardData
	{
		public string score;

		public bool isPlayer;

		public bool gotWeekly;

		public string gamerTag;

		public int scoreAsInt;

		public LeaderboardData()
		{
			score = "110";
			isPlayer = false;
			gotWeekly = false;
			gamerTag = null;
			scoreAsInt = 0;
		}
	}

	public const int GAME_MODE_WEEKLY = -2;

	public const int GAME_MODE_TOTAL_FRUIT = -1;

	protected float m_time;

	private static int MAX_PLAYERS = 25;

	private static int TIMEOUT_DELAY = 30000;

	private static int TABLE_Y = 138;

	private static int ADJUST = 50;

	private static int TAB1 = 118;

	private static int TAB3 = 510;

	private MenuButton m_modeClass;

	private MenuButton m_modeZen;

	private MenuButton m_modeArcade;

	private MenuButton m_modeWeekly;

	private MenuButton m_modeTotalFruit;

	private static Texture2D m_backing;

	private static Texture2D m_title;

	private static Mortar.Texture m_boxTexture;

	private static Mortar.Texture[] m_caption;

	private static Texture2D[] m_gamerPics;

	private static Texture2D m_live;

	private static Mortar.Texture m_noScoreThisWeek;

	private static float liveX;

	private static float liveY;

	private bool slide_on;

	private bool slide_off;

	private float slide_transition;

	private int gameMode;

	private int caption;

	private static int startWith = 0;

	private float prevTextOffset;

	private float textOffset;

	private float offset;

	private float snapY;

	private float tx;

	private float ty;

	private float initialX;

	private float initialY;

	private bool down;

	private float renderSize;

	private int m_timeout;

	private float panelY;

	private ScrollState scrollState;

	private int m_state;

	private MenuButton m_quitButton;

	private List<LeaderboardData> entries;

	private bool prevTouch;

	private bool currTouch;

	private float py;

	private float ssy;

	public static int bcx = 188;

	public static int bcy = 176;

	public static int sx = 628;

	public static int sy = 344;

	public static int ABOUT_CENTRE_X => 190;

	public static int ABOUT_CENTRE_Y => 97;

	public static float ABOUT_SCREEN_HEIGHT => 320f;

	public static int BOX_CENTRE_X => bcx;

	public static int BOX_CENTRE_Y => bcy;

	public LeaderboardsScreen()
	{
		m_selfCleanUp = false;
		m_quitButton = null;
		m_state = 0;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		m_time = 0f;
		entries = new List<LeaderboardData>();
	}

	public static void LoadContent()
	{
		if (m_backing == null)
		{
			m_backing = TextureManager.GetInstance().Load("textureswp7/leaderboard_back.tex").intex;
		}
		if (m_title == null)
		{
			m_title = TextureManager.GetInstance().Load("textureswp7/leaderboard.tex").intex;
		}
		m_boxTexture = TextureManager.GetInstance().Load("leaderboard_box.tex", localise: true);
		m_caption = new Mortar.Texture[5];
		m_caption[0] = TextureManager.GetInstance().Load("classic_title.tex", localise: true);
		m_caption[1] = TextureManager.GetInstance().Load("zen_title.tex", localise: true);
		m_caption[2] = TextureManager.GetInstance().Load("arcade_total_title.tex", localise: true);
		m_caption[3] = TextureManager.GetInstance().Load("arcade_weekly_title.tex", localise: true);
		m_caption[4] = TextureManager.GetInstance().Load("total_fruit_title.tex", localise: true);
		m_gamerPics = new Texture2D[MAX_PLAYERS];
		m_noScoreThisWeek = TextureManager.GetInstance().Load("no_score_this_week.tex", localise: true);
	}

	public static void UnLoadContent()
	{
	}

	public override void Reset()
	{
		entries.Clear();
	}

	public override void Release()
	{
		m_texture = null;
		m_time = 0f;
	}

	public override void Init()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		partOfPopup = true;
		Reset();
		scrollState = ScrollState.Idle;
		Vector3 scale = default(Vector3);
		scale = new Vector3(102.4f, 36f, 1f);
		if (m_modeClass == null)
		{
			m_modeClass = new MenuButton("classic_button.tex", new Vector3(0f, 0f, 0f), Classic_Callback, -1, scale, localise: true);
			m_modeClass.partOfPopup = true;
			m_modeClass.Init();
			Game.game_work.hud.AddControl(m_modeClass);
			m_modeClass.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		if (m_modeZen == null)
		{
			m_modeZen = new MenuButton("zen.tex", new Vector3(0f, 0f, 0f), Zen_Callback, -1, scale, localise: true);
			m_modeZen.partOfPopup = true;
			m_modeZen.Init();
			Game.game_work.hud.AddControl(m_modeZen);
			m_modeZen.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		if (m_modeArcade == null)
		{
			m_modeArcade = new MenuButton("arcade_total.tex", new Vector3(0f, 0f, 0f), Arcade_Callback, -1, scale, localise: true);
			m_modeArcade.partOfPopup = true;
			m_modeArcade.Init();
			Game.game_work.hud.AddControl(m_modeArcade);
			m_modeArcade.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		if (m_modeWeekly == null)
		{
			m_modeWeekly = new MenuButton("arcade_weekly.tex", new Vector3(0f, 0f, 0f), Arcade_Weekly, -1, scale, localise: true);
			m_modeWeekly.partOfPopup = true;
			m_modeWeekly.Init();
			Game.game_work.hud.AddControl(m_modeWeekly);
			m_modeWeekly.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		if (m_modeTotalFruit == null)
		{
			m_modeTotalFruit = new MenuButton("total_fruit.tex", new Vector3(0f, 0f, 0f), Arcade_TotalFruit, -1, scale, localise: true);
			m_modeTotalFruit.partOfPopup = true;
			m_modeTotalFruit.Init();
			Game.game_work.hud.AddControl(m_modeTotalFruit);
			m_modeTotalFruit.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		slide_transition = 0f;
	}

	public void Classic_Callback()
	{
		if ((m_state == 11 || m_state == 7) && !slide_on && !slide_off)
		{
			entries.Clear();
			m_state = 4;
			slide_off = true;
			gameMode = 0;
			caption = 0;
			textOffset = 0f;
			offset = 0f;
			m_timeout = TIMEOUT_DELAY;
			Leaderboards.StartRead(0, ReadFinished);
		}
	}

	public void Zen_Callback()
	{
		if ((m_state == 11 || m_state == 7) && !slide_on && !slide_off)
		{
			entries.Clear();
			m_state = 4;
			gameMode = 3;
			slide_off = true;
			caption = 1;
			textOffset = 0f;
			offset = 0f;
			m_timeout = TIMEOUT_DELAY;
			Leaderboards.StartRead(1, ReadFinished);
		}
	}

	public void Arcade_Callback()
	{
		if ((m_state == 11 || m_state == 7) && !slide_on && !slide_off)
		{
			entries.Clear();
			m_state = 4;
			slide_off = true;
			gameMode = 2;
			caption = 2;
			textOffset = 0f;
			offset = 0f;
			m_timeout = TIMEOUT_DELAY;
			Leaderboards.StartRead(2, ReadFinished);
		}
	}

	public void Arcade_Weekly()
	{
		if ((m_state == 11 || m_state == 7) && !slide_on && !slide_off)
		{
			gameMode = -2;
			entries.Clear();
			m_state = 4;
			slide_off = true;
			caption = 3;
			textOffset = 0f;
			offset = 0f;
			m_timeout = TIMEOUT_DELAY;
			Leaderboards.StartRead(3, ReadFinished);
		}
	}

	public void Arcade_TotalFruit()
	{
		if ((m_state == 11 || m_state == 7) && !slide_on && !slide_off)
		{
			gameMode = -1;
			entries.Clear();
			m_state = 4;
			slide_off = true;
			caption = 4;
			textOffset = 0f;
			offset = 0f;
			m_timeout = TIMEOUT_DELAY;
			Leaderboards.StartRead(4, ReadFinished);
		}
	}

	public void Callback_PageUp()
	{
	}

	public void Callback_PageDown()
	{
	}

	private bool InTextBox()
	{
		if (Mortar.Math.BETWEEN((int)tx, 132, 560) && Mortar.Math.BETWEEN((int)ty, 232, 448))
		{
			return true;
		}
		return false;
	}

	public override void Update(float dt)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		if (m_state == 7 && !slide_on)
		{
			int touchCount = Touch.GetInstance().GetTouchCount();
			if (touchCount == 1)
			{
				Vector2 position = Touch.GetInstance().GetPosition();
				tx = position.X;
				ty = position.Y;
				currTouch = true;
			}
			else
			{
				currTouch = false;
			}
			if (prevTouch != currTouch)
			{
				prevTouch = currTouch;
				if (currTouch)
				{
					down = true;
					if (InTextBox())
					{
						initialX = tx;
						initialY = ty;
						textOffset = 0f;
						prevTextOffset = 0f;
						scrollState = ScrollState.Scroll;
					}
				}
				else
				{
					down = false;
				}
			}
			if (down && scrollState == ScrollState.Scroll && InTextBox() && renderSize > 200f)
			{
				float t = ty - py;
				if (Mortar.Math.Abs(t) > 1f && Mortar.Math.Abs(t) < 32f)
				{
					ssy = t;
				}
				prevTextOffset = textOffset;
				textOffset = ty - initialY;
				if (offset + textOffset > 0f)
				{
					snapY = textOffset - prevTextOffset;
					textOffset = prevTextOffset;
				}
				if (offset + textOffset < 0f - (renderSize - 200f))
				{
					snapY = textOffset - prevTextOffset;
					textOffset = prevTextOffset;
				}
				py = ty;
			}
			else
			{
				float num = 200f;
				if (Mortar.Math.Abs(ssy) > 1f)
				{
					prevTextOffset = textOffset;
					textOffset += ssy;
					ssy -= ssy / 8f;
					if (offset + textOffset > 0f)
					{
						snapY = textOffset - prevTextOffset;
						textOffset = prevTextOffset;
						ssy = 0f;
					}
					if (offset + textOffset < 0f - (renderSize - num))
					{
						snapY = textOffset - prevTextOffset;
						textOffset = prevTextOffset;
						ssy = 0f;
					}
				}
				else
				{
					if (scrollState == ScrollState.Scroll)
					{
						scrollState = ScrollState.Idle;
						offset += textOffset;
						textOffset = 0f;
					}
					snapY -= snapY / 8f;
				}
			}
		}
		else
		{
			scrollState = ScrollState.Idle;
			initialX = (tx = -1f);
			initialY = (ty = -1f);
			textOffset = 0f;
		}
		Game.game_work.tutorialControl.ResetTutePos();
		switch (m_state)
		{
		case 0:
			m_time += (1f - m_time) * 0.125f;
			if (m_time > 0.999f)
			{
				m_time = 1f;
				m_quitButton = new MenuButton("back_icon.tex", new Vector3(425f - Game.SCREEN_WIDTH / 2f, ABOUT_SCREEN_HEIGHT / 2f - 266f, 0f), QuitGameCallback, Fruit.MAX_FRUIT_TYPES, Vector3.Zero, localise: true);
				m_quitButton.partOfPopup = true;
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
			m_state = 3;
			break;
		case 3:
			entries.Clear();
			m_state = 4;
			textOffset = 0f;
			offset = 0f;
			m_timeout = TIMEOUT_DELAY;
			switch (startWith)
			{
			case 0:
				gameMode = 0;
				caption = 0;
				Leaderboards.StartRead(0, ReadFinished);
				break;
			case 1:
				gameMode = 3;
				caption = 1;
				Leaderboards.StartRead(1, ReadFinished);
				break;
			case 2:
				gameMode = 2;
				caption = 2;
				Leaderboards.StartRead(2, ReadFinished);
				break;
			default:
				gameMode = 0;
				caption = 0;
				Leaderboards.StartRead(0, ReadFinished);
				break;
			}
			startWith = 0;
			break;
		case 4:
			m_timeout -= (int)(dt * 1000f);
			if (m_timeout < 0)
			{
				m_state = 6;
			}
			break;
		case 5:
			if (!slide_off)
			{
				renderSize = 0f;
				slide_on = true;
				m_state = 7;
				renderSize = 50 * entries.Count;
			}
			break;
		case 2:
			m_time *= 0.75f;
			if (m_time < 0.001f && !slide_off)
			{
				if (PopOverControl.IsInPopup)
				{
					PopOverControl.Instance.Out();
				}
				else
				{
					Game.game_work.mainScreen.m_state = MainScreen.MS.MS_RETURN;
				}
				m_terminate = true;
			}
			break;
		}
		if (slide_on)
		{
			if (slide_transition < -0.999f)
			{
				slide_transition = -1f;
				slide_on = false;
			}
			else
			{
				slide_transition += (-1f - slide_transition) * 0.125f;
			}
		}
		if (slide_off)
		{
			if (Mortar.Math.Abs(slide_transition) < 0.001f)
			{
				slide_transition = 0f;
				slide_off = false;
			}
			else
			{
				slide_transition += (0f - slide_transition) * 0.125f;
			}
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_099a: Unknown result type (might be due to invalid IL or missing references)
		//IL_099f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			m_texture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3(440f * Game.GAME_MODE_SCALE_FIX, 220f * Game.GAME_MODE_SCALE_FIX, 1f));
			float num = ABOUT_SCREEN_HEIGHT / 2f + 110f;
			float num2 = ABOUT_SCREEN_HEIGHT / 2f - (float)ABOUT_CENTRE_Y - 20f;
			float num3 = num - (num - num2) * m_time;
			MatrixManager.GetInstance().Translate(new Vector3((float)ABOUT_CENTRE_X - Game.SCREEN_WIDTH / 2f, num3, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_texture.UnSet();
		}
		float num4 = 0f;
		float num5 = 0f;
		if (m_backing != null)
		{
			TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			TheGame.instance.spriteBatch.Draw(m_backing, new Vector2(0f, 0f), Color.White);
			TheGame.instance.spriteBatch.End();
		}
		if (m_boxTexture != null && m_state == 7)
		{
			float num6 = 480f;
			float num7 = 16f;
			float num8 = -512f;
			float num9 = 60f;
			num4 = num6 - (num6 - num7) * Mortar.Math.Abs(slide_transition);
			num5 = num8 - (num8 - num9) * Mortar.Math.Abs(slide_transition);
			TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			TheGame.instance.spriteBatch.Draw(m_boxTexture.intex, new Vector2(-196f, num4), Color.White);
			TheGame.instance.spriteBatch.Draw(m_caption[caption].intex, new Vector2(num5, 0f), Color.White);
			if (m_live != null)
			{
				liveY = 64f;
				liveX = 320f + num5 * 2f;
				TheGame.instance.spriteBatch.Draw(m_live, new Vector2(liveX, liveY), Color.White);
			}
			panelY = num4;
			TheGame.instance.spriteBatch.End();
		}
		if (m_state == 7)
		{
			TheGame.instance.spriteBatch.Begin();
			float num10 = TABLE_Y + 72;
			int num11 = 0;
			bool flag = false;
			foreach (LeaderboardData entry in entries)
			{
				flag = true;
				float num12 = num10 + num4 + textOffset + offset + snapY;
				if (!slide_on)
				{
					if (num12 <= 188f)
					{
						flag = false;
					}
					else if (num12 >= 432f)
					{
						flag = false;
					}
				}
				if (flag)
				{
					if (gameMode == -2)
					{
						if (entry.gotWeekly || entry.isPlayer)
						{
							if (!slide_on)
							{
								if (num12 < 424f)
								{
									TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.gamerTag, new Vector2((float)TAB1, num12), (Color)((entry.isPlayer && !slide_on) ? new Color(200, 0, 0) : Color.Black));
								}
							}
							else
							{
								TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.gamerTag, new Vector2((float)TAB1, num12), (Color)((entry.isPlayer && !slide_on) ? new Color(200, 0, 0) : Color.Black));
							}
							Vector2 val = TheGame.instance.font1.MeasureString(entry.score);
							TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.score, new Vector2((float)TAB3 - val.X / 2f, num12), Color.Black);
						}
						else
						{
							TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.gamerTag, new Vector2((float)TAB1, num12), (Color)((entry.isPlayer && !slide_on) ? new Color(200, 0, 0) : Color.Black));
							int num13 = (int)(panelY - 16f);
							int num14 = 440 + num13 - (int)(m_noScoreThisWeek.h / 2);
							if (num12 > (float)num14)
							{
								int num15 = (int)(num12 - (float)num14);
								int w = (int)m_noScoreThisWeek.w;
								int h = (int)m_noScoreThisWeek.h;
								TheGame.instance.spriteBatch.End();
								TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
								TheGame.instance.spriteBatch.Draw(m_noScoreThisWeek.intex, new Rectangle(TAB3 - 68, (int)num12 - 16, w >> 1, (h >> 1) - num15), (Rectangle?)new Rectangle(0, 0, w, h - (num15 << 1)), Color.White);
							}
							else
							{
								TheGame.instance.spriteBatch.End();
								TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
								TheGame.instance.spriteBatch.Draw(m_noScoreThisWeek.intex, new Rectangle(TAB3 - 68, (int)num12 - 16, (int)(m_noScoreThisWeek.w / 2), (int)(m_noScoreThisWeek.h / 2)), Color.White);
							}
						}
					}
					else
					{
						if (!slide_on)
						{
							if (num12 < 424f)
							{
								TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.gamerTag, new Vector2((float)TAB1, num12), (Color)((entry.isPlayer && !slide_on) ? new Color(200, 0, 0) : Color.Black));
							}
						}
						else
						{
							TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.gamerTag, new Vector2((float)TAB1, num12), (Color)((entry.isPlayer && !slide_on) ? new Color(200, 0, 0) : Color.Black));
						}
						Vector2 val2 = TheGame.instance.font1.MeasureString(entry.score);
						TheGame.instance.spriteBatch.DrawString(TheGame.instance.font2, entry.score, new Vector2((float)TAB3 - val2.X / 2f, num12), Color.Black);
					}
				}
				num10 += (float)ADJUST;
				num11++;
			}
			TheGame.instance.spriteBatch.End();
			TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			TheGame.instance.spriteBatch.Draw(m_boxTexture.intex, new Rectangle(-196, (int)num4, 1024, 204), (Rectangle?)new Rectangle(0, 0, 1024, 204), Color.White);
			TheGame.instance.spriteBatch.Draw(m_boxTexture.intex, new Rectangle(-196, (int)num4 + 408, 1024, 48), (Rectangle?)new Rectangle(0, 408, 1024, 48), Color.White);
			TheGame.instance.spriteBatch.End();
			TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			TheGame.instance.spriteBatch.End();
		}
		else if (m_state == 3 || m_state == 4 || m_state == 5)
		{
			if (!slide_off && !slide_on)
			{
				string text = TheGame.instance.stringTable.GetString(1175);
				float num16 = 400f;
				float num17 = 220f;
				string[] array = text.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
				TheGame.instance.spriteBatch.Begin();
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					string text3 = text2;
					text3 = text3.Trim();
					Vector2 val3 = TheGame.instance.font1.MeasureString(text3);
					TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text3, new Vector2(num16 - val3.X / 2f, num17), Color.Bisque);
					num17 += 30f;
				}
				TheGame.instance.spriteBatch.End();
			}
		}
		else if (m_state == 6 && !slide_off && !slide_on)
		{
			string text4 = TheGame.instance.stringTable.GetString(1176);
			float num18 = 400f;
			float num19 = 220f;
			string[] array3 = text4.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
			TheGame.instance.spriteBatch.Begin();
			string[] array4 = array3;
			foreach (string text5 in array4)
			{
				string text6 = text5;
				text6 = text6.Trim();
				Vector2 val4 = TheGame.instance.font1.MeasureString(text6);
				TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text6, new Vector2(num18 - val4.X / 2f, num19), Color.Bisque);
				num19 += 30f;
			}
			TheGame.instance.spriteBatch.End();
		}
		float x = 350f + (0f - slide_transition) * -170f;
		float num20 = 125f;
		float num21 = -40f;
		if (m_modeClass != null)
		{
			m_modeClass.m_pos.Y = num20;
			m_modeClass.m_pos.X = x;
			num20 += num21;
		}
		if (m_modeZen != null)
		{
			m_modeZen.m_pos.Y = num20;
			m_modeZen.m_pos.X = x;
			num20 += num21;
		}
		if (m_modeArcade != null)
		{
			m_modeArcade.m_pos.Y = num20;
			m_modeArcade.m_pos.X = x;
			num20 += num21;
		}
		if (m_modeWeekly != null)
		{
			m_modeWeekly.m_pos.Y = num20;
			m_modeWeekly.m_pos.X = x;
			num20 += num21;
		}
		if (m_modeTotalFruit != null)
		{
			m_modeTotalFruit.m_pos.Y = num20;
			m_modeTotalFruit.m_pos.X = x;
			num20 += num21;
		}
	}

	private void QuitGameCallback()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_MENU_BOMB);
		m_state = 2;
		slide_on = false;
		slide_off = true;
		((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
		m_quitButton.m_entity.m_vel = new Vector3(Mortar.Math.g_random.RandF(5f) + 5f, 0f - Mortar.Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
		Game.ClearMenuItems();
	}

	private void ReadFinished(int result)
	{
		if (m_state == 4)
		{
			if (result == 2)
			{
				renderSize = 0f;
				m_state = 5;
			}
			else
			{
				m_state = 6;
			}
		}
	}

	public static void SetStartLeaderboard(int start)
	{
		startWith = start;
	}

	private static void PlayerGetProfileCallback(IAsyncResult result)
	{
	}
}
