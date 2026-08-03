using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mortar;

namespace FruitNinja;

internal class AchievementsScreen : HUDControl3d
{
	private class MapInfo
	{
		public string key;

		public int index;

		public MapInfo(string s, int i)
		{
			key = s;
			index = i;
		}
	}

	private enum ScrollState
	{
		Idle,
		Scroll
	}

	public enum AS
	{
		AS_IN,
		AS_WAIT,
		AS_OUT
	}

	private float m_time;

	private int m_state;

	private MenuButton m_quitButton;

	private float prevTextOffset;

	private float textOffset;

	private float offset;

	private float snapY;

	private float tx;

	private float ty;

	private float initialX;

	private float initialY;

	private bool down;

	private bool prevTouch;

	private bool currTouch;

	private float renderSize = 1584f;

	private ScrollState scrollState;

	private static Texture2D m_backing;

	private static Mortar.Texture m_title;

	private static Mortar.Texture m_box;

	private static Texture2D m_live;

	private static float liveX;

	private static float liveY;

	public static int[] achIconMap = new int[20]
	{
		8, 7, 6, 2, 9, 5, 13, 4, 14, 3,
		1, 15, 10, 12, 16, 17, 11, 18, 19, 20
	};

	private static MapInfo[] mapdata = new MapInfo[20]
	{
		new MapInfo("Fruit Blitz", 4),
		new MapInfo("Year Of The Dragon", 16),
		new MapInfo("Fruit Fight", 3),
		new MapInfo("Deja Vu", 12),
		new MapInfo("Wake Up", 15),
		new MapInfo("Great Fruit Ninja", 1),
		new MapInfo("Ultimate Fruit Ninja", 2),
		new MapInfo("Fruit Frenzy", 5),
		new MapInfo("Fruit Rampage", 6),
		new MapInfo("Fruit Annihilation", 7),
		new MapInfo("Lucky Ninja", 8),
		new MapInfo("Almost A Century", 9),
		new MapInfo("Mango Magic", 10),
		new MapInfo("Combo Mambo", 13),
		new MapInfo("Moment Of Zen", 14),
		new MapInfo("The Lovely Bunch", 17),
		new MapInfo("Its All Pear Shaped", 11),
		new MapInfo("Underachiever", 18),
		new MapInfo("Overachiever", 19),
		new MapInfo("Bomb Magnet", 20)
	};

	private static Dictionary<string, Texture2D> imgMapInfo = null;

	private static float py = 0f;

	private static float sy = 0f;

	public static int ABOUT_CENTRE_X => 190;

	public static int ABOUT_CENTRE_Y => 97;

	public static float ABOUT_SCREEN_HEIGHT => 320f;

	public AchievementsScreen()
	{
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
		if (m_backing == null)
		{
			m_backing = TextureManager.GetInstance().Load("textureswp7/leaderboard_back.tex").intex;
		}
		if (m_title == null)
		{
			m_title = TextureManager.GetInstance().Load("achievements.tex", localise: true);
		}
		if (m_box == null)
		{
			m_box = TextureManager.GetInstance().Load("achievements_box.tex", localise: true);
		}
	}

	public static void UnLoadContent()
	{
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
		imgMapInfo = new Dictionary<string, Texture2D>();
		MapInfo[] array = mapdata;
		foreach (MapInfo mapInfo in array)
		{
			imgMapInfo.Add(mapInfo.key, ((Microsoft.Xna.Framework.Game)TheGame.instance).Content.Load<Texture2D>("textureswp7/achicon_" + mapInfo.index + ".tga"));
		}
	}

	private bool InTextBox()
	{
		if (Mortar.Math.BETWEEN((int)tx, 100, 582) && Mortar.Math.BETWEEN((int)ty, 172, 410))
		{
			return true;
		}
		return false;
	}

	public override void Update(float dt)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Invalid comparison between Unknown and I4
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		if (m_state == 1)
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
			float num = 268f;
			if (prevTouch != currTouch)
			{
				prevTouch = currTouch;
				if (currTouch)
				{
					if (Mortar.Math.Abs(sy) < 4f)
					{
						down = true;
						sy = 0f;
						if (InTextBox())
						{
							initialX = tx;
							initialY = ty;
							textOffset = 0f;
							prevTextOffset = 0f;
							scrollState = ScrollState.Scroll;
						}
					}
				}
				else
				{
					down = false;
				}
			}
			if (down && scrollState == ScrollState.Scroll && InTextBox())
			{
				prevTextOffset = textOffset;
				textOffset = ty - initialY;
				float t = ty - py;
				if (Mortar.Math.Abs(t) > 1f && Mortar.Math.Abs(t) < 64f)
				{
					sy = t;
				}
				if (offset + textOffset > 0f)
				{
					snapY = textOffset - prevTextOffset;
					textOffset = prevTextOffset;
				}
				if (offset + textOffset < 0f - (renderSize - num))
				{
					snapY = textOffset - prevTextOffset;
					textOffset = prevTextOffset;
				}
				py = ty;
			}
			else if (Mortar.Math.Abs(sy) > 1f)
			{
				prevTextOffset = textOffset;
				textOffset += sy;
				sy -= sy / 4f;
				if (offset + textOffset > 0f)
				{
					snapY = textOffset - prevTextOffset;
					textOffset = prevTextOffset;
					sy = 0f;
				}
				if (offset + textOffset < 0f - (renderSize - num))
				{
					snapY = textOffset - prevTextOffset;
					textOffset = prevTextOffset;
					sy = 0f;
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
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		if (m_backing != null)
		{
			TheGame.instance.spriteBatch.Begin();
			TheGame.instance.spriteBatch.Draw(m_backing, new Vector2(0f, 0f), Color.White);
			TheGame.instance.spriteBatch.End();
		}
		if (m_title != null)
		{
			TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			TheGame.instance.spriteBatch.Draw(m_title.intex, new Vector2(-80f, 0f), Color.White);
			TheGame.instance.spriteBatch.End();
		}
		float num = 0f;
		if (m_box != null)
		{
			float num2 = 480f;
			float num3 = 16f;
			num = num2 - (num2 - num3) * Mortar.Math.Abs(m_time);
			TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			TheGame.instance.spriteBatch.Draw(m_box.intex, new Rectangle(-170, (int)num, m_box.intex.Width - m_box.intex.Width / 16, m_box.intex.Height), Color.White);
			TheGame.instance.spriteBatch.End();
		}
		TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		if (m_live != null)
		{
			liveY = 64f;
			liveX = 400f + num * 2f;
			TheGame.instance.spriteBatch.Draw(m_live, new Vector2(liveX, liveY), Color.White);
		}
		float num4 = num + 138f;
		float num5 = 272f;
		bool flag = false;
		Vector2 val = TheGame.instance.font3.MeasureString(" ");
		for (int i = 0; i < 20; i++)
		{
			Color val2 = (Color)(AchievementManager.Unlocked(i) ? Color.Black : new Color(0, 0, 0, 96));
			flag = false;
			float num6 = num + 138f + (float)(i * 80) + textOffset + offset + snapY;
			float num7 = num6 + 64f;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			float num11 = num6;
			string[] array = AchievementManager.GetDescription(i).Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string text = "";
			string text2 = "";
			string text3 = "";
			float num12 = 0f;
			int num13 = 0;
			string[] array2 = array;
			foreach (string text4 in array2)
			{
				string text5 = text4;
				text5 = text5.Trim();
				Vector2 val3 = TheGame.instance.font3.MeasureString(text5);
				if (num12 + val3.X + val.X <= 380f)
				{
					num12 += val3.X + val.X;
				}
				else
				{
					num12 = val3.X + val.X;
					num13++;
				}
				switch (num13)
				{
				case 0:
					text = text + text4 + " ";
					break;
				case 1:
					text2 = text2 + text4 + " ";
					break;
				default:
					text3 = text3 + text4 + " ";
					break;
				}
			}
			if (num11 + 16f < num4 - 20f)
			{
				flag = true;
			}
			if (num11 + 16f > num4 + num5 - 12f)
			{
				flag = true;
			}
			if (!flag)
			{
				TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text, new Vector2(100f, num11 + 16f), val2);
				TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, AchievementManager.GetGamerScore(i).ToString(), new Vector2(510f, num11 + 16f), val2);
			}
			flag = false;
			if (num11 + 36f < num4 - 20f)
			{
				flag = true;
			}
			if (num11 + 36f > num4 + num5 - 12f)
			{
				flag = true;
			}
			if (!flag && text2.Length > 0)
			{
				TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text2, new Vector2(100f, num11 + 36f), val2);
			}
			Texture2D val4 = imgMapInfo[AchievementManager.GetKey(i)];
			flag = false;
			if (num7 > num4 + num5)
			{
				float num14 = num7 - (num4 + num5);
				if (num14 > 64f)
				{
					flag = true;
				}
				else
				{
					num8 = (int)(0f - num14);
					num9 = num8 / 2;
					if (val4.Height == 64)
					{
						num9 = num8;
					}
				}
			}
			if (num6 < num4)
			{
				float num15 = num4 - num6;
				if (num15 > 64f)
				{
					flag = true;
				}
				else
				{
					num6 = num4;
					num10 = (int)num15;
					num9 = -num10;
					num8 = num9;
					if (val4.Height == 32)
					{
						num10 /= 2;
						num9 /= 2;
					}
				}
			}
			if (!flag)
			{
				TheGame.instance.spriteBatch.Draw(val4, new Rectangle(26, (int)num6, 64, 64 + num8), (Rectangle?)new Rectangle(0, num10, val4.Width, val4.Height + num9), Color.White);
			}
		}
		TheGame.instance.spriteBatch.End();
		TheGame.instance.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		TheGame.instance.spriteBatch.Draw(m_box.intex, new Rectangle(-170, (int)num, m_box.intex.Width - m_box.intex.Width / 16, 137), (Rectangle?)new Rectangle(0, 0, m_box.intex.Width, 137), Color.White);
		TheGame.instance.spriteBatch.Draw(m_box.intex, new Rectangle(-170, (int)num + 400, m_box.intex.Width - m_box.intex.Width / 16, 48), (Rectangle?)new Rectangle(0, 400, m_box.intex.Width, 48), Color.White);
		TheGame.instance.spriteBatch.End();
	}
}
