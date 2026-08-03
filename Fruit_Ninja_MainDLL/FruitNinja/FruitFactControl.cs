using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class FruitFactControl : HUDControl3d
{
	public enum FFL
	{
		FFL_NO_FRIENDS,
		FFL_OFFLINE,
		FFL_DOWNLOADING,
		FFL_LEADERBOARD,
		FFL_DOWNLOAD_FAILED
	}

	public enum DownloadResult
	{
		Pending,
		Success,
		Failed
	}

	private class FriendLeaderboardItem : LeaderboardItem
	{
		public bool isPlayer;

		private Vector3 o;

		public FriendLeaderboardItem()
		{
			isPlayer = false;
		}

		public FriendLeaderboardItem(string name, int rank, int score)
			: base(name, rank, score)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			isPlayer = false;
			m_rank = rank;
			m_score = score;
			m_height = LEADERBOARD_Y_DIFF;
			m_colour = new Color(116, 93, 59);
			m_textOffset = Vector3.UnitX * -68f;
			m_text = name.ToUpper();
		}

		public float AdjustScale()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = m_pos + m_textOffset;
			float num = 23f;
			Vector3 val2 = val + Vector3.UnitX * 115f;
			o = Vector3.Zero;
			float num2 = num;
			if (val2.Y > 114f)
			{
				float num3 = val2.Y - 114f;
				num2 -= num3;
				num2 = Mortar.Math.MAX(0f, num2);
				o.Y = 0f - num3;
			}
			else if (val2.Y < -10f)
			{
				float num4 = -10f - val2.Y;
				num2 -= num4;
				num2 = Mortar.Math.MAX(0f, num2);
				o.Y = num4;
			}
			return num2;
		}

		private Color TintColour(Color col, float[] tints)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			col.R = (byte)Mortar.Math.CLAMP((float)(int)col.R * tints[0], 0f, 255f);
			col.G = (byte)Mortar.Math.CLAMP((float)(int)col.G * tints[1], 0f, 255f);
			col.B = (byte)Mortar.Math.CLAMP((float)(int)col.B * tints[2], 0f, 255f);
			return col;
		}

		public override void Draw()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = m_pos + m_textOffset;
			if (m_rank < 0 || !m_isOnScreen)
			{
				return;
			}
			float num = 23f;
			float num2 = Game.game_work.pGameFont.MeasureString(m_text);
			if (num2 > 145f)
			{
				num *= 145f / num2;
			}
			MortarRectangleDec? rect = null;
			MortarRectangleDec mortarRectangleDec = default(MortarRectangleDec);
			mortarRectangleDec.left = 0f;
			mortarRectangleDec.right = 0f;
			mortarRectangleDec.top = 0f;
			mortarRectangleDec.bottom = 0f;
			if (m_parentList != null)
			{
				mortarRectangleDec.top = 105f;
				mortarRectangleDec.bottom = -30f;
				mortarRectangleDec.left = 0f - Game.SCREEN_WIDTH;
				mortarRectangleDec.right = Game.SCREEN_WIDTH;
				rect = mortarRectangleDec;
			}
			Color white = default(Color);
			white = new Color(64, 34, 23);
			Font font = Game.game_work.pGameFont;
			if (isPlayer)
			{
				num *= 1.2f;
				font = Game.game_work.pNumberFontLeaderboard;
				white = Color.White;
				GameVertex[] array = new GameVertex[4];
				if (SetupQuad(array, val, s_youTexture.GetWidth(), s_youTexture.GetHeight(), mortarRectangleDec, Color.White))
				{
					s_youTexture.Set();
					MatrixManager.GetInstance().Reset();
					MatrixManager.GetInstance().UploadCurrentMatrices();
					Mesh.DrawTriStrip(array, 4);
					s_youTexture.UnSet();
				}
			}
			else
			{
				float num3 = AdjustScale();
				Game.game_work.pGameFont.DrawString(m_text, val + o, white, new Vector2(num, num3), Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER, 1f, rect);
			}
			if (m_score > 0)
			{
				float num4 = AdjustScale();
				font.DrawString(m_score.ToString(), val + Vector3.UnitX * 115f + o, white, new Vector2(num, num4), Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER, 1f, rect);
				float[] tints = new float[3] { 5f, 5f, 5f };
				Game.game_work.pNumberFontLeaderboard.DrawString(m_rank.ToString(), val + Vector3.UnitX * 175f + o, TintColour(white, tints), new Vector2(num, num4), Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER, 1f, rect);
				return;
			}
			mortarRectangleDec.top -= 16f;
			mortarRectangleDec.bottom -= 16f;
			GameVertex[] array2 = new GameVertex[4];
			if (SetupQuad(array2, val + Vector3.UnitX * 290f / 2f + new Vector3(-4f, 0f, 0f), s_noScoreTexture.GetWidth() / 2, s_noScoreTexture.GetHeight() / 2, mortarRectangleDec, Color.White))
			{
				s_noScoreTexture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawTriStrip(array2, 4);
				s_noScoreTexture.UnSet();
			}
		}
	}

	public string m_text;

	public int m_fruitType;

	public int m_fact;

	public Texture m_fruitTexture;

	public Vector3 m_fruitOffset;

	public Color m_fruitColor;

	public static Texture s_boardTexture;

	public static Texture s_bigBoardTexture;

	public static Texture s_noComboTexture;

	public static Texture s_senseiHead;

	public static Texture s_loadingTexture;

	public bool m_showCombo;

	public int[] m_comboFruits = new int[11];

	public int m_numComboFruits;

	public float m_fruitComboTime;

	public Texture m_stickerTexture;

	public COMBO_TYPE m_comboStickerType;

	public int m_mode;

	private int displayMode;

	private float m_loadingSymbolTime;

	public static LeaderboardList m_leaderboardList;

	public MenuButton m_leaderboardButton;

	public FFL m_leaderboardState;

	private bool start_download;

	private bool download_complete;

	private DownloadResult download_result;

	public MenuButton[] m_buttons = new MenuButton[2];

	public static Texture s_youTexture = null;

	public static Texture s_noScoreTexture = null;

	private static Texture s_buttonTexture = null;

	private static Texture s_bigLeaderBoardTexture = null;

	private static Texture s_bigLeaderBoardDialogTexture = null;

	private static Texture s_bigBonusBoardTexture = null;

	private static Texture s_xboxLive = null;

	private static bool contentLoaded = false;

	private static string zenModeTitle;

	private static GameVertex[] symbo_tris = new GameVertex[48];

	public static bool USE_ZEN_FACT_SCREEN => Game.game_work.gameMode == Game.GAME_MODE.GM_ZEN;

	public static int NUM_ARCADE_MODE_MODES => 1;

	public static float LEADERBOARD_Y_DIFF => 47f;

	public static bool SetupQuad(GameVertex[] verts, Vector3 pos, float width, float height, MortarRectangleDec rectangle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return SetupQuad(verts, pos, width, height, rectangle, Color.White);
	}

	private static bool SetupQuad(GameVertex[] verts, Vector3 pos, float width, float height, MortarRectangleDec rectangle, Color colour)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		rectangle.bottom += height * 0.5f;
		rectangle.top += height * 0.5f;
		float x = pos.X - width * 0.5f;
		float x2 = pos.X + width * 0.5f;
		float v = 1f;
		float v2 = 0f;
		float num = pos.Y - height * 0.5f;
		float num2 = pos.Y + height * 0.5f;
		if (num < rectangle.bottom)
		{
			num = rectangle.bottom;
			v = 0.5f - (num - pos.Y) / height;
		}
		if (num2 > rectangle.top)
		{
			num2 = rectangle.top;
			v2 = 0.5f - (num2 - pos.Y) / height;
		}
		if (num2 > rectangle.bottom && num < rectangle.top)
		{
			for (int i = 0; i < 4; i++)
			{
				verts[i].color = colour;
				verts[i].X = (verts[i].Y = (verts[i].Z = (verts[i].nx = (verts[i].ny = 0f))));
				verts[i].nz = 1f;
				if (i % 2 == 0)
				{
					verts[i].X = x;
					verts[i].u = 0f;
				}
				else
				{
					verts[i].X = x2;
					verts[i].u = 1f;
				}
				if (i < 2)
				{
					verts[i].Y = num;
					verts[i].v = v;
				}
				else
				{
					verts[i].Y = num2;
					verts[i].v = v2;
				}
			}
			return true;
		}
		return false;
	}

	public FruitFactControl()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		LoadContent();
		m_rotation = 0f;
		m_selfCleanUp = true;
		m_fruitColor = new Color(116, 93, 59);
		m_text = null;
		m_stickerTexture = null;
		m_fruitComboTime = 0f;
		m_numComboFruits = 0;
		m_fruitType = -1;
		m_fact = -1;
		m_mode = 0;
		m_loadingSymbolTime = 0f;
		m_leaderboardList = null;
		m_buttons[0] = null;
		m_buttons[1] = null;
		m_leaderboardButton = null;
		m_leaderboardState = FFL.FFL_OFFLINE;
		start_download = false;
		download_complete = false;
		download_result = DownloadResult.Pending;
		Reset();
	}

	~FruitFactControl()
	{
		Release();
	}

	public override void Init()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		m_fruitComboTime = -0.5f;
		m_stickerTexture = null;
		m_texture = ((Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE) ? s_bigLeaderBoardDialogTexture : (USE_ZEN_FACT_SCREEN ? s_bigBoardTexture : s_boardTexture));
		displayMode = ((Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE) ? 1 : (USE_ZEN_FACT_SCREEN ? 2 : 3));
		if (USE_ZEN_FACT_SCREEN)
		{
			m_scale = new Vector3((float)(m_texture.GetWidth() + 1), (float)(m_texture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX;
		}
		else
		{
			m_scale = new Vector3((float)(m_texture.GetWidth() + 1), (float)(m_texture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX;
		}
		m_comboStickerType = COMBO_TYPE.CT_NONE;
		m_showCombo = USE_ZEN_FACT_SCREEN && Game.game_work.saveData.numFruitTypesInSliceCombo > 2;
		m_fruitOffset = new Vector3(-69f, 53f, 0f);
		if (m_showCombo)
		{
			string text = TheGame.instance.stringTable.GetString(202);
			text = text.Replace("%i", "{0}");
			zenModeTitle = string.Format(text, Game.game_work.saveData.numFruitTypesInSliceCombo);
			m_numComboFruits = Game.game_work.saveData.numFruitTypesInSliceCombo;
			for (int i = 0; i < m_numComboFruits; i++)
			{
				m_comboFruits[i] = Game.game_work.saveData.sliceComboFruitTypes[i];
			}
			int mostOf = 0;
			m_comboStickerType = ComboChecker.CheckCombo(m_comboFruits, m_numComboFruits, out mostOf);
			m_stickerTexture = ComboChecker.GetComboStarTexture(m_comboStickerType);
			if (m_fruitType != mostOf)
			{
				m_fruitType = mostOf;
			}
			m_fruitOffset = new Vector3(140f, -72f, 0f);
		}
		else
		{
			zenModeTitle = TheGame.instance.stringTable.GetString(227);
		}
		int num = 0;
		int num3;
		do
		{
			try
			{
				m_text = Fruit.GetFact(delegate(int v)
				{
					m_fruitType = v;
				}, delegate(int v)
				{
					m_fact = v;
				}, m_fruitType, m_fact);
				num++;
			}
			catch
			{
				num = 100;
			}
			if (num == 100)
			{
				m_fruitType = -1;
				m_fact = -1;
				m_text = "";
				num = 0;
			}
			float num2 = 16f;
			num3 = 0;
			int num4 = 0;
			int num5 = -1;
			for (int num6 = 0; num6 < m_text.Length; num6++)
			{
				if (m_text[num6] == ' ')
				{
					num5 = num6;
				}
				num4++;
				if ((float)num4 == num2)
				{
					if (!((float)(num6 - num5) >= num2) && num6 != num5)
					{
						num6 = num5 + 1;
					}
					num4 = 0;
					num3++;
				}
			}
			if (num4 != 0)
			{
				num3++;
			}
		}
		while (num3 > 6 || m_text.Trim().Length == 0);
		m_fruitColor = Fruit.FruitFactColor(m_fruitType);
		string text2 = $"textureswp7/{Fruit.FruitFactTexture(m_fruitType)}.tex";
		if (TextureManager.TextureFileExists(text2))
		{
			m_fruitTexture = TextureManager.GetInstance().Load(text2);
		}
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		Reset();
	}

	public override void Release()
	{
		m_texture = null;
		m_fruitTexture = null;
		m_stickerTexture = null;
		if (m_leaderboardList != null)
		{
			Game.game_work.hud.RemoveControl(m_leaderboardList);
			Delete.SAFE_DELETE(ref m_leaderboardList);
		}
		for (int i = 0; i < 2; i++)
		{
			if (m_buttons[i] != null)
			{
				Game.game_work.hud.RemoveControl(m_buttons[i]);
			}
			Delete.SAFE_DELETE(ref m_buttons[i]);
		}
		if (m_leaderboardButton != null)
		{
			Game.game_work.hud.RemoveControl(m_leaderboardButton);
			Delete.SAFE_DELETE(ref m_leaderboardButton);
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (m_drawOrder == HUD.HUD_ORDER.HUD_ORDER_POST)
		{
			s_senseiHead.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3((float)s_senseiHead.GetWidth() * Game.GAME_MODE_SCALE_FIX, (float)s_senseiHead.GetHeight() * Game.GAME_MODE_SCALE_FIX, 0f));
			if (Game.game_work.language == StringTableUtils.Language.LANGUAGE_SPANISH || Game.game_work.language == StringTableUtils.Language.LANGUAGE_GERMAN)
			{
				MatrixManager.GetInstance().Translate(m_pos + new Vector3(156f, 68f, 0f));
			}
			else
			{
				MatrixManager.GetInstance().Translate(m_pos + new Vector3(140f, 68f, 0f));
			}
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			s_senseiHead.UnSet();
			return;
		}
		if (USE_ZEN_FACT_SCREEN)
		{
			if (m_texture != null)
			{
				m_texture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(m_scale);
				MatrixManager.GetInstance().Translate(m_pos - new Vector3(8f, -8f, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
				m_texture.UnSet();
			}
			flag = true;
			Game.game_work.pGameFont.DrawString(TheGame.instance.stringTable.GetString(205), m_pos.X - 8f, m_pos.Y, 0f, m_fruitColor, 16f, 0f, 0f, ALIGNMENT_TYPE.ALIGN_HCENTER);
			Game.game_work.pGameFont.DrawString(m_text, m_pos.X - 127f - 8f, m_pos.Y - 40f, 0f, new Color(116, 93, 59), 16f, 255f, 0f, ALIGNMENT_TYPE.ALIGN_CENTER);
			Game.game_work.pGameFont.DrawString(zenModeTitle, m_pos.X - 8f, m_pos.Y + 89f, 0f, m_fruitColor, 20f, 0f, 0f, ALIGNMENT_TYPE.ALIGN_HCENTER);
			if (m_showCombo)
			{
				float num = 40f;
				float num2 = (float)(m_numComboFruits - 1) * num;
				if (num2 > 220f)
				{
					num2 = 220f;
					num = num2 / (float)(m_numComboFruits - 1);
				}
				for (int i = 0; i < m_numComboFruits; i++)
				{
					Texture zenIcon = Fruit.FruitInfo(m_comboFruits[i]).zenIcon;
					if (zenIcon != null)
					{
						float num3 = 0f;
						if ((int)(m_fruitComboTime + 2f) == i + 2)
						{
							float num4 = Mortar.Math.CLAMP((m_fruitComboTime - (float)i - 0.5f) * 2f, 0f, 1f);
							num3 = 65f * Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(135f * num4)) / 0.70710677f;
						}
						else
						{
							if (!(m_fruitComboTime > (float)i))
							{
								break;
							}
							num3 = 65f;
						}
						zenIcon.Set();
						MatrixManager.GetInstance().Reset();
						MatrixManager.GetInstance().Scale(Vector3.One * num3);
						MatrixManager.GetInstance().Translate(m_pos + new Vector3(-8f - num2 / 2f + (float)i * num, 37f, 0f));
						MatrixManager.GetInstance().UploadCurrentMatrices();
						Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
						zenIcon.UnSet();
					}
					zenIcon = null;
				}
				if (m_stickerTexture != null && m_fruitComboTime > (float)m_numComboFruits)
				{
					float num5 = Mortar.Math.CLAMP((m_fruitComboTime - (float)m_numComboFruits - 0.5f) * 2f, 0f, 1f);
					float num6 = Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(135f * num5)) / 0.70710677f;
					m_stickerTexture.Set();
					MatrixManager.GetInstance().Reset();
					MatrixManager.GetInstance().Scale(new Vector3((float)(m_stickerTexture.GetWidth() + 1), (float)(m_stickerTexture.GetHeight() + 1), 0f) * num6 * Game.GAME_MODE_SCALE_FIX);
					MatrixManager.GetInstance().Translate(m_pos + new Vector3(-8f + num2 / 2f, 37f + (float)m_stickerTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX / 2f, 0f));
					MatrixManager.GetInstance().UploadCurrentMatrices();
					Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
					m_stickerTexture.UnSet();
				}
			}
			else
			{
				float num7 = Mortar.Math.CLAMP((m_fruitComboTime - (float)m_numComboFruits - 0.5f) * 2f, 0f, 1f);
				float num8 = Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(135f * num7)) / 0.70710677f;
				s_noComboTexture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3((float)s_noComboTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)s_noComboTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 0f) * num8);
				MatrixManager.GetInstance().Translate(m_pos + new Vector3(-8f, 37f, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
				s_noComboTexture.UnSet();
			}
		}
		else if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && m_mode == 1)
		{
			flag = true;
			DrawLeaderboard();
		}
		else if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && m_mode == 0)
		{
			flag = true;
			if (s_bigBonusBoardTexture != null)
			{
				s_bigBonusBoardTexture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(m_scale);
				MatrixManager.GetInstance().Translate(m_pos - new Vector3(8f, -8f, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
				s_bigBonusBoardTexture.UnSet();
			}
			LinkedListNode<Bonus> it = null;
			Bonus bonus = BonusManager.GetInstance().GetFirstBestBonus(ref it);
			Color[] array = new Color[3]
			{
				new Color(173, 126, 0),
				new Color(160, 5, 5),
				new Color(1, 92, 149)
			};
			float num9 = 20f;
			Vector3 val = m_pos + new Vector3(-114f, 58f, 0f);
			int num10 = 0;
			while (bonus != null)
			{
				Game.game_work.pGameFont.DrawString(bonus.GetText(), val, array[num10], 16f, Vector2.Zero, (ALIGNMENT_TYPE)13);
				string stringToDraw = $"{bonus.GetPoints()}";
				Game.game_work.pGameFont.DrawString(stringToDraw, val + Vector3.UnitX * 209f, array[num10], 16f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
				Mortar.Math.Scale44((float)bonus.texture.GetWidth() * 0.5f * Game.GAME_MODE_SCALE_FIX, (float)bonus.texture.GetHeight() * 0.5f * Game.GAME_MODE_SCALE_FIX, 0f, out var mtx);
				Mortar.Math.GlobalTranslate44(ref mtx, val - Vector3.UnitX * 20f);
				MatrixManager.GetInstance().SetMatrix(mtx);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				bonus.texture.Set();
				Mesh.DrawQuad(array[num10], 0f, 1f, 0f, 1f);
				bonus.texture.UnSet();
				bonus = BonusManager.GetInstance().GetNextBestBonus(ref it);
				val.Y -= num9;
				num10 = Mortar.Math.MIN(num10 + 1, array.Length);
			}
			Game.game_work.pGameFont.DrawString(TheGame.instance.stringTable.GetString(205), m_pos.X - 8f, m_pos.Y - (float)((Game.game_work.language != StringTableUtils.Language.LANGUAGE_ENGLISH && Game.game_work.language != StringTableUtils.Language.LANGUAGE_ENGLISH_UK) ? 4 : 0), 0f, m_fruitColor, 16f, 0f, 0f, ALIGNMENT_TYPE.ALIGN_HCENTER);
			Game.game_work.pGameFont.DrawString(m_text, m_pos.X - 127f - 8f, m_pos.Y - 40f - (float)((Game.game_work.language != StringTableUtils.Language.LANGUAGE_ENGLISH && Game.game_work.language != StringTableUtils.Language.LANGUAGE_ENGLISH_UK) ? 4 : 0), 0f, new Color(116, 93, 59), 16f, 255f, 0f, ALIGNMENT_TYPE.ALIGN_CENTER);
		}
		else
		{
			if (m_texture != null)
			{
				m_texture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(m_scale);
				MatrixManager.GetInstance().Translate(m_pos - new Vector3(8f, -8f, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
				m_texture.UnSet();
			}
			Game.game_work.pGameFont.DrawString(m_text, m_pos.X - 64f - 5f, m_pos.Y - 14f, 0f, new Color(116, 93, 59), 16f, 128f, 0f, ALIGNMENT_TYPE.ALIGN_CENTER);
			if (m_fruitTexture != null)
			{
				m_fruitTexture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3((float)m_fruitTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX * Game.SCREEN_SCALE_X * 1.5f, (float)m_fruitTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX * Game.SCREEN_SCALE_X * 1.5f, 0f));
				MatrixManager.GetInstance().Translate(m_pos + m_fruitOffset + new Vector3(-12f, 24f, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
				m_fruitTexture.UnSet();
			}
		}
		if (s_senseiHead != null && flag)
		{
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
	}

	private Vector3 ConvertPos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.Zero;
		pos.X -= Game.game_work.camera.m_cameraShake.X;
		pos.Y -= Game.game_work.camera.m_cameraShake.Y;
		zero = pos;
		zero.X += 240f;
		zero.Y += 160f;
		zero.Y = 320f - zero.Y;
		return zero * 1.6f;
	}

	private void LeftButton()
	{
		m_mode--;
		if (m_mode < 0)
		{
			m_mode = NUM_ARCADE_MODE_MODES - 1;
		}
	}

	private void RightButton()
	{
		m_mode++;
		if (m_mode >= NUM_ARCADE_MODE_MODES)
		{
			m_mode = 0;
		}
	}

	public override void Update(float dt)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		m_loadingSymbolTime += dt * 8f;
		if (m_loadingSymbolTime >= 8f)
		{
			m_loadingSymbolTime = 0f;
		}
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			if (NUM_ARCADE_MODE_MODES > 1)
			{
				if (m_buttons[0] == null)
				{
					m_buttons[0] = new MenuButton(s_buttonTexture, m_pos, LeftButton);
					m_buttons[0].m_selfCleanUp = true;
					m_buttons[0].Init();
					m_buttons[0].m_uvs[0].X = 1f;
					m_buttons[0].m_uvs[1].X = 0f;
					Game.game_work.hud.AddControl(m_buttons[0]);
				}
				if (m_buttons[1] == null)
				{
					m_buttons[1] = new MenuButton(s_buttonTexture, m_pos, RightButton);
					m_buttons[1].Init();
					m_buttons[1].m_selfCleanUp = true;
					Game.game_work.hud.AddControl(m_buttons[1]);
				}
				m_buttons[0].m_pos = m_pos + new Vector3(-158f, 8f, 0f);
				m_buttons[1].m_pos = m_pos + new Vector3(142f, 8f, 0f);
			}
			_ = m_leaderboardList;
			if (m_leaderboardList != null)
			{
				m_leaderboardList.m_update = false;
			}
			if (m_leaderboardButton != null)
			{
				m_leaderboardButton.m_update = false;
			}
		}
		if (USE_ZEN_FACT_SCREEN)
		{
			if (Game.game_work.gameOverTransition > 0.75f && m_fruitComboTime < (float)m_numComboFruits)
			{
				float num = m_fruitComboTime - (float)(int)m_fruitComboTime;
				m_fruitComboTime += dt * 4f;
				if (m_fruitComboTime > 0f && num < 0.5f && m_fruitComboTime - (float)(int)m_fruitComboTime >= 0.5f)
				{
					string filename = $"popup-{Mortar.Math.MIN((int)m_fruitComboTime + 1, 8)}";
					SoundManager.GetInstance().SFXPlay(filename);
				}
				if (!(m_fruitComboTime >= (float)m_numComboFruits))
				{
				}
			}
			else if (m_fruitComboTime >= (float)m_numComboFruits && m_fruitComboTime < (float)(m_numComboFruits + 1))
			{
				float num2 = m_fruitComboTime - (float)(int)m_fruitComboTime;
				m_fruitComboTime += dt * 2f;
				if (m_stickerTexture != null && m_fruitComboTime > 0f && num2 < 0.5f && m_fruitComboTime - (float)(int)m_fruitComboTime >= 0.5f)
				{
					SoundManager.GetInstance().SFXPlay("achievement");
				}
			}
		}
		else if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && m_mode == 1)
		{
			UpdateLeaderboard();
		}
		else if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && m_mode == 0)
		{
			LinkedListNode<Bonus> it = null;
			if (BonusManager.GetInstance().GetFirstBestBonus(ref it) == null)
			{
				BonusManager.GetInstance().SetUpBonusScreen(null);
			}
		}
	}

	private void UpdateLeaderboard()
	{
	}

	private void ReadFinished(int result)
	{
	}

	private void DrawXboxLive()
	{
	}

	private void DrawLeaderboard()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			m_texture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(m_scale);
			MatrixManager.GetInstance().Translate(m_pos - new Vector3(8f, -8f, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
			m_texture.UnSet();
		}
		string text = null;
		switch (m_leaderboardState)
		{
		case FFL.FFL_DOWNLOAD_FAILED:
			text = TheGame.instance.stringTable.GetString(1165);
			DrawXboxLive();
			break;
		case FFL.FFL_NO_FRIENDS:
			text = TheGame.instance.stringTable.GetString(226);
			DrawXboxLive();
			break;
		case FFL.FFL_OFFLINE:
			text = TheGame.instance.stringTable.GetString(1165);
			DrawXboxLive();
			break;
		case FFL.FFL_DOWNLOADING:
			if (m_loadingSymbolTime >= 0f && s_loadingTexture != null)
			{
				DrawDownloadIcon();
				string stringToDraw = TheGame.instance.stringTable.GetString(129);
				Game.game_work.pGameFont.DrawString(stringToDraw, m_pos + new Vector3(-125f, 27f, 0f), new Color(72, 44, 21), 18f, new Vector2(234f, 0f), ALIGNMENT_TYPE.ALIGN_CENTER);
			}
			break;
		}
		float num = 294f;
		if (text == null)
		{
			return;
		}
		Color val = default(Color);
		val = new Color(79, 159, 0);
		Vector2 val2 = TheGame.instance.font3.MeasureString(" ");
		string[] array = text.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		string text2 = "";
		string text3 = "";
		string text4 = "";
		float num2 = 0f;
		int num3 = 0;
		string[] array2 = array;
		foreach (string text5 in array2)
		{
			string text6 = text5;
			text6 = text6.Trim();
			Vector2 val3 = TheGame.instance.font3.MeasureString(text6);
			if (num2 + val3.X + val2.X <= num)
			{
				num2 += val3.X + val2.X;
			}
			else
			{
				num2 = val3.X + val2.X;
				num3++;
			}
			switch (num3)
			{
			case 0:
				text2 = text2 + text5 + " ";
				break;
			case 1:
				text3 = text3 + text5 + " ";
				break;
			default:
				text4 = text4 + text5 + " ";
				break;
			}
		}
		TheGame.instance.spriteBatch.Begin();
		float num4 = m_pos.X + 440f;
		float num5 = 140f;
		float num6 = 24f;
		if (text2.Length > 0)
		{
			string text7 = text2;
			text7 = text7.Trim();
			Vector2 val4 = TheGame.instance.font3.MeasureString(text7);
			TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text7, new Vector2(num4 - val4.X / 2f, num5), val);
			num5 += num6;
		}
		if (text3.Length > 0)
		{
			string text8 = text3;
			text8 = text8.Trim();
			Vector2 val5 = TheGame.instance.font3.MeasureString(text8);
			TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text8, new Vector2(num4 - val5.X / 2f, num5), val);
			num5 += num6;
		}
		if (text4.Length > 0)
		{
			string text9 = text4;
			text9 = text9.Trim();
			Vector2 val6 = TheGame.instance.font3.MeasureString(text9);
			TheGame.instance.spriteBatch.DrawString(TheGame.instance.font3, text9, new Vector2(num4 - val6.X / 2f, num5), val);
			num5 += num6;
		}
		TheGame.instance.spriteBatch.End();
	}

	private static Texture GetLeaderboardTitleTexture()
	{
		return s_xboxLive;
	}

	private void DrawDownloadIcon()
	{
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)m_loadingSymbolTime;
		num %= 8;
		num = 7 - num;
		if (0 == 0)
		{
			bool flag = true;
			float num2 = 0.6f;
			float num3 = 0.075f;
			Vector3 val = default(Vector3);
			Vector3 val2 = default(Vector3);
			for (int i = 0; i < 8; i++)
			{
				val = new Vector3(Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(i * 45)) * 0.5f, Mortar.Math.CosIdx(Mortar.Math.DEGREE_TO_IDX(i * 45)) * 0.5f, 0f);
				val2 = new Vector3(Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(i * 45 + 90)) * num3, Mortar.Math.CosIdx(Mortar.Math.DEGREE_TO_IDX(i * 45 + 90)) * num3, 0f);
				symbo_tris[i * 6].X = val.X - val2.X;
				symbo_tris[i * 6].Y = val.Y - val2.Y;
				symbo_tris[i * 6].u = 0f;
				symbo_tris[i * 6].v = 0f;
				symbo_tris[i * 6 + 1].X = val.X + val2.X;
				symbo_tris[i * 6 + 1].Y = val.Y + val2.Y;
				symbo_tris[i * 6 + 1].u = 1f;
				symbo_tris[i * 6 + 1].v = 0f;
				symbo_tris[i * 6 + 2].X = val.X * num2 - val2.X;
				symbo_tris[i * 6 + 2].Y = val.Y * num2 - val2.Y;
				symbo_tris[i * 6 + 2].u = 0f;
				symbo_tris[i * 6 + 2].v = 1f;
				symbo_tris[i * 6 + 3].X = val.X * num2 - val2.X;
				symbo_tris[i * 6 + 3].Y = val.Y * num2 - val2.Y;
				symbo_tris[i * 6 + 3].u = 0f;
				symbo_tris[i * 6 + 3].v = 1f;
				symbo_tris[i * 6 + 4].X = val.X + val2.X;
				symbo_tris[i * 6 + 4].Y = val.Y + val2.Y;
				symbo_tris[i * 6 + 4].u = 1f;
				symbo_tris[i * 6 + 4].v = 0f;
				symbo_tris[i * 6 + 5].X = val.X * num2 + val2.X;
				symbo_tris[i * 6 + 5].Y = val.Y * num2 + val2.Y;
				symbo_tris[i * 6 + 5].u = 1f;
				symbo_tris[i * 6 + 5].v = 1f;
				for (int j = 0; j < 6; j++)
				{
					symbo_tris[i * 6 + j].nz = 1f;
					symbo_tris[i * 6 + j].Z = 0f;
				}
			}
		}
		Color color = default(Color);
		for (int k = 0; k < 8; k++)
		{
			int num4 = Mortar.Math.CLAMP((num + k) % 8 * 32, 64, 255);
			color = new Color(num4, num4, num4, 200);
			for (int l = 0; l < 6; l++)
			{
				symbo_tris[k * 6 + l].color = color;
			}
		}
		Vector3 val3 = default(Vector3);
		val3 = new Vector3(-7f, -23f, 0f);
		s_loadingTexture.Set();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(new Vector3(32f, 32f, 0f));
		MatrixManager.GetInstance().Translate(m_pos + val3);
		MatrixManager.GetInstance().UploadCurrentMatrices();
		Mesh.DrawTriList(symbo_tris, 48, hasAlpha: false);
		s_loadingTexture.UnSet();
	}

	public override void Reset()
	{
	}

	public static void LoadContent()
	{
		if (!contentLoaded)
		{
			s_boardTexture = TextureManager.GetInstance().Load("fact_board.tex", localise: true);
			s_bigBoardTexture = TextureManager.GetInstance().Load("diolog_box_big.tex", localise: true);
			s_noComboTexture = TextureManager.GetInstance().Load("combo_description.tex", localise: true);
			s_senseiHead = TextureManager.GetInstance().Load("textureswp7/sensei_head.tex");
			contentLoaded = true;
			s_buttonTexture = TextureManager.GetInstance().Load("textureswp7/arcade_results_arrow.tex");
			s_bigLeaderBoardTexture = TextureManager.GetInstance().Load("arcade_results_score_box.tex", localise: true);
			s_bigLeaderBoardDialogTexture = TextureManager.GetInstance().Load("arcade_results_diolog_box.tex", localise: true);
			s_bigBonusBoardTexture = TextureManager.GetInstance().Load("arcade_results_bonus_box.tex", localise: true);
			s_loadingTexture = TextureManager.GetInstance().Load("textureswp7/blurry_backing.tex");
			s_youTexture = TextureManager.GetInstance().Load("score_you.tex", localise: true);
			s_noScoreTexture = TextureManager.GetInstance().Load("no_score_this_week.tex", localise: true);
		}
	}

	public static void UnLoadContent()
	{
		s_boardTexture = null;
		s_bigBoardTexture = null;
		s_noComboTexture = null;
		s_senseiHead = null;
		s_buttonTexture = null;
		s_bigLeaderBoardTexture = null;
		s_bigLeaderBoardDialogTexture = null;
		s_bigBonusBoardTexture = null;
		s_youTexture = null;
		s_noScoreTexture = null;
	}

	public bool LeftPressed(InputEvent evnt)
	{
		m_fact--;
		if (m_fact < 0)
		{
			m_fact = Fruit.FruitInfo(m_fruitType).factCount - 1;
		}
		m_text = Fruit.GetFact(null, null, m_fruitType, m_fact);
		return true;
	}

	public bool RightPressed(InputEvent evnt)
	{
		m_fact++;
		if (m_fact >= Fruit.FruitInfo(m_fruitType).factCount)
		{
			m_fact = 0;
		}
		m_text = Fruit.GetFact(null, null, m_fruitType, m_fact);
		return true;
	}

	public bool UpPressed(InputEvent evnt)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		do
		{
			m_fruitType++;
			if (m_fruitType >= Fruit.MAX_FRUIT_TYPES)
			{
				m_fruitType = 0;
			}
		}
		while (Fruit.FruitInfo(m_fruitType).factCount <= 0);
		m_text = Fruit.GetFact(null, delegate(int v)
		{
			m_fact = v;
		}, m_fruitType);
		m_fruitColor = Fruit.FruitFactColor(m_fruitType);
		string texture = $"textureswp7/{Fruit.FruitFactTexture(m_fruitType)}.tex";
		m_fruitTexture = TextureManager.GetInstance().Load(texture);
		return true;
	}

	public bool DownPressed(InputEvent evnt)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		do
		{
			m_fruitType--;
			if (m_fruitType < 0)
			{
				m_fruitType = Fruit.MAX_FRUIT_TYPES - 1;
			}
		}
		while (Fruit.FruitInfo(m_fruitType).factCount <= 0);
		m_text = Fruit.GetFact(null, delegate(int v)
		{
			m_fact = v;
		}, m_fruitType);
		m_fruitColor = Fruit.FruitFactColor(m_fruitType);
		string texture = $"textureswp7/{Fruit.FruitFactTexture(m_fruitType)}.tex";
		m_fruitTexture = TextureManager.GetInstance().Load(texture);
		return true;
	}
}
