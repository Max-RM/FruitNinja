using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class ScoreControl : HUDControl3d
{
	private bool firstRun;

	public ushort m_pulseTimer;

	public float m_scoreWait;

	public int m_currentScore;

	public int m_highScore;

	public float m_gameOverTransition;

	public float m_overAllScale;

	public Vector3 m_textPos;

	public Texture m_scroreTexture;

	public Texture m_highScoreTexture;

	public float m_highscoreTime;

	public ushort m_highscorePulse;

	public int m_multiplyer;

	public int m_multiplyerType;

	public float[] m_multiplyerScales = new float[MAX_MULTIPLYERS];

	public Texture m_iconTexture;

	private static float disappearWait;

	private static int cycle;

	public static int MAX_MULTIPLYERS => 16;

	public static float SCALE_UP_TIME => 0.05f;

	public static float SCALE_DOWN_TIME => 2.55f;

	public static float FULL_PULSE => 32768f;

	public static float HALF_PULSE => 16384f;

	public static float SCALE_UP_RATE => HALF_PULSE / SCALE_UP_TIME;

	public static float SCALE_DOWN_RATE => HALF_PULSE / SCALE_DOWN_TIME;

	public static float MIN_SCALE => 40f * Game.HUD_SCALE;

	public static float MAX_SCALE => 50f * Game.HUD_SCALE;

	public static float GAME_SCREEN_X_IN => 2f * Game.HUD_SCALE;

	public static float GAME_SCREEN_Y_IN => 2f * Game.HUD_SCALE;

	public static Vector3 IN_GAME_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + MIN_SCALE / 2f + GAME_SCREEN_X_IN, Game.SCREEN_HEIGHT / 2f - MIN_SCALE / 2f - GAME_SCREEN_Y_IN, 0f);

	public static float GAMEOVER_SCREEN_X_IN => 80f;

	public static float GAMEOVER_SCREEN_Y_IN
	{
		get
		{
			if (!Game.USE_ARCADE_GO_SCREEN)
			{
				return 80f;
			}
			return 240f;
		}
	}

	public static float FONT_SIZE => 48f;

	public static float GAME_OVER_SCALE => (Game.USE_ARCADE_GO_SCREEN ? 1.5f : 2f) * Game.HUD_SCALE;

	public static float NEW_HIGHSCORE_FLASH => 1000f;

	public static float COMBO_SCALE_UP_RATE => 6f;

	public static float COMBO_SCALE_DOWN_RATE => 16f;

	public static Vector3 GAMEOVER_GAME_POS(float len)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((0f - Game.SCREEN_WIDTH) / 2f + GAMEOVER_SCREEN_X_IN - len, 160f - GAMEOVER_SCREEN_Y_IN, 0f);
	}

	public ScoreControl()
	{
		m_rotation = 0f;
		m_gameOverTransition = -1f;
		m_overAllScale = 1f;
		m_highscoreTime = -2f;
		m_scoreWait = 0f;
		m_highScore = 0;
		m_iconTexture = TextureManager.GetInstance().Load("textureswp7/hud_fruit.tex");
		Reset();
	}

	public override void Reset()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_texture = m_iconTexture;
		firstRun = true;
		m_pulseTimer = 0;
		m_scale = Vector3.One * MIN_SCALE;
		for (int i = 0; i < MAX_MULTIPLYERS; i++)
		{
			m_multiplyerScales[i] = 0f;
		}
		m_multiplyerType = 0;
		m_multiplyer = 0;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
	}

	public override void Release()
	{
		m_iconTexture = null;
		m_texture = null;
		m_scroreTexture = null;
		m_highScoreTexture = null;
	}

	public override void Init()
	{
		Reset();
	}

	public override void Update(float dt)
	{
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		int currentScore = Game.game_work.currentScore;
		m_multiplyer = Math.MIN(Fruit.s_consecutiveCount - 1, MAX_MULTIPLYERS - 1);
		if (m_multiplyer > 0 && Game.game_work.gameMode == Game.GAME_MODE.GM_CASINO)
		{
			if (m_multiplyerType != Fruit.s_consecutiveType)
			{
				for (int i = 0; i < MAX_MULTIPLYERS; i++)
				{
					if (m_multiplyerScales[i] > 0f)
					{
						m_multiplyerScales[i] -= dt * COMBO_SCALE_DOWN_RATE;
						if (m_multiplyerScales[i] < 0f)
						{
							m_multiplyerScales[i] = 0f;
						}
						continue;
					}
					m_multiplyerScales[i] = 0f;
					if (i == 0)
					{
						m_multiplyerType = Fruit.s_consecutiveType;
					}
					break;
				}
			}
			else
			{
				for (int j = 0; j < m_multiplyer; j++)
				{
					if (m_multiplyerScales[j] < 1f)
					{
						m_multiplyerScales[j] += dt * COMBO_SCALE_UP_RATE;
						if (m_multiplyerScales[j] > 1f)
						{
							m_multiplyerScales[j] = 1f;
						}
					}
				}
				disappearWait = 0f;
			}
		}
		else if (disappearWait < 0.25f)
		{
			disappearWait += dt;
			for (int k = 0; k < MAX_MULTIPLYERS; k++)
			{
				if (m_multiplyerScales[k] > 1f && m_multiplyerScales[k] < 1f)
				{
					m_multiplyerScales[k] += dt * COMBO_SCALE_UP_RATE;
					if (m_multiplyerScales[k] > 1f)
					{
						m_multiplyerScales[k] = 1f;
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < MAX_MULTIPLYERS; l++)
			{
				if (m_multiplyerScales[l] > 0f)
				{
					m_multiplyerScales[l] -= dt * COMBO_SCALE_DOWN_RATE;
					if (m_multiplyerScales[l] < 0f)
					{
						m_multiplyerScales[l] = 0f;
					}
					continue;
				}
				m_multiplyerScales[l] = 0f;
				break;
			}
		}
		if (firstRun)
		{
			m_scoreWait = currentScore;
			m_currentScore = currentScore;
			firstRun = false;
		}
		int currentScore2 = m_currentScore;
		m_scoreWait += Math.MIN(((float)currentScore + 0.6f - m_scoreWait) * 0.1f, 0.3f * (float)Game.GetScoreMultiplyer() * ((Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE) ? 10f : 1f));
		m_currentScore = (int)m_scoreWait;
		if (m_currentScore > currentScore2)
		{
			m_pulseTimer = (ushort)FULL_PULSE;
		}
		if ((float)(int)m_pulseTimer > HALF_PULSE)
		{
			m_pulseTimer -= (ushort)(dt * SCALE_UP_RATE);
		}
		else if (m_pulseTimer > 0)
		{
			m_pulseTimer -= (ushort)(dt * SCALE_UP_RATE);
			if ((float)(int)m_pulseTimer > FULL_PULSE)
			{
				m_pulseTimer = 0;
			}
		}
		float num = MIN_SCALE + (MAX_SCALE - MIN_SCALE) * Math.SinIdx(m_pulseTimer);
		m_overAllScale = Math.CLAMP(Game.game_work.gameOverTransition, 0f, 1f) * (GAME_OVER_SCALE - Game.HUD_SCALE) + Game.HUD_SCALE;
		if (!Game.game_work.gameOver || currentScore == 0)
		{
			if (Game.GetCurrentModeHighscore() == 0)
			{
				m_highScore = 0;
			}
			else
			{
				m_highScore = Math.MAX(Game.GetCurrentModeHighscore(), m_currentScore);
			}
		}
		m_pos = IN_GAME_POS - new Vector3(MAX_SCALE * 2f, 0f, 0f) * (Math.Abs(Game.game_work.gameOverTransition) + (1f - Game.game_work.hud.m_hudAmount));
		if (Game.game_work.gameOverTransition > 0f)
		{
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
			m_textPos = IN_GAME_POS + new Vector3(MIN_SCALE * 0.6f, 0f, 0f);
			string stringToDraw = $"{currentScore}";
			float len = Game.game_work.pNumberFont.MeasureString(stringToDraw) * m_overAllScale * FONT_SIZE * 0.5f;
			m_textPos += (GAMEOVER_GAME_POS(len) - m_textPos) * Game.game_work.gameOverTransition;
		}
		else
		{
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
			m_textPos = m_pos + new Vector3(MIN_SCALE * 0.6f, 0f, 0f);
		}
		if (Game.game_work.gameOverTransition > 0.999f && Game.game_work.saveData.go_showHighScore)
		{
			float highscoreTime = m_highscoreTime;
			m_highscoreTime = Math.MIN(1f, m_highscoreTime + dt * 5f);
			if (m_highscoreTime == 1f)
			{
				m_highscorePulse += (ushort)(dt * (float)(int)Math.DEGREE_TO_IDX(270f));
			}
			else
			{
				m_highscorePulse = 0;
			}
			if (m_highscoreTime > 0f && highscoreTime <= 0f && !PopOverControl.IsInPopup)
			{
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_NEW_BEST);
			}
		}
		else
		{
			m_highscoreTime = Math.MAX(-1.5f, m_highscoreTime - dt * 20f);
		}
		m_scale.X = num;
		m_scale.Y = num;
	}

	public override void PreDraw(float[] tintChannels)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		string stringToDraw = "";
		if (Game.game_work.gameOverTransition > -1f)
		{
			stringToDraw = $"{m_currentScore}";
			Game.game_work.pNumberFont.DrawString(stringToDraw, m_textPos.X, m_textPos.Y, 0f, HUDControl.TintWhite(tintChannels), m_overAllScale * FONT_SIZE, 0f, 0f, (ALIGNMENT_TYPE)13);
		}
		switch (Game.game_work.gameMode)
		{
		case Game.GAME_MODE.GM_CASINO:
		{
			m_texture = Fruit.FruitInfo(Math.CLAMP(Fruit.s_consecutiveType, 0, Fruit.MAX_FRUIT_TYPES - 1)).icon;
			Color factColor = Fruit.FruitInfo(Math.CLAMP(Fruit.s_consecutiveType, 0, Fruit.MAX_FRUIT_TYPES - 1)).factColor;
			float num = Game.game_work.pNumberFont.MeasureString(stringToDraw) * m_overAllScale * FONT_SIZE + 5f;
			for (int i = 0; i < MAX_MULTIPLYERS; i++)
			{
				if (m_multiplyerScales[i] > 0f)
				{
					stringToDraw = $"{1 << i + 1}";
					float num2 = Math.SinIdx(Math.DEGREE_TO_IDX(m_multiplyerScales[i] * 135f)) * ((float)i * 6f + 45f);
					Game.game_work.pGameFont.DrawString(stringToDraw, m_textPos.X + num, Game.SCREEN_HEIGHT / 2f - 5f, 0f, factColor, num2, 0f, 0f, ALIGNMENT_TYPE.ALIGN_LEFT);
					num += Game.game_work.pGameFont.MeasureString(stringToDraw) * num2 + 5f;
				}
			}
			break;
		}
		case Game.GAME_MODE.GM_ARCADE:
			if (PowerUpManager.GetInstance().GetScoreGainMultiplier() > 1 && !Game.game_work.gameOver)
			{
				Game.game_work.pNumberFont.MeasureString(stringToDraw);
				_ = FONT_SIZE;
				stringToDraw = $"x{PowerUpManager.GetInstance().GetScoreGainMultiplier()}";
				Game.game_work.pNumberFontBlue2.DrawString(stringToDraw, m_pos.X - MIN_SCALE * 0.45f, m_pos.Y - MIN_SCALE * 1.3f, 0f, HUDControl.TintWhite(tintChannels), m_overAllScale * FONT_SIZE * 0.75f, 0f, 0f, (ALIGNMENT_TYPE)13);
			}
			break;
		}
		if (Math.Abs(Game.game_work.gameOverTransition) < 1f && m_highScore > 0)
		{
			stringToDraw = $"{m_highScore}";
			Color val = default(Color);
			val = new Color(180, 128, 5, 200);
			if (m_highScore == m_currentScore)
			{
				cycle += ((!Game.game_work.pause) ? 6 : 0);
				if (cycle >= 180)
				{
					cycle = 180;
				}
				float num3 = Math.CosIdx(Math.DEGREE_TO_IDX(cycle));
				num3 *= -0.5f;
				num3 += 0.5f;
				Color val2 = default(Color);
				val2 = new Color(100, 150, 25, 200);
				val.R = (byte)Math.CLAMP((float)(int)val.R + (float)(val2.R - val.R) * num3, 0f, 254.99f);
				val.G = (byte)Math.CLAMP((float)(int)val.G + (float)(val2.G - val.G) * num3, 0f, 254.99f);
				val.B = (byte)Math.CLAMP((float)(int)val.B + (float)(val2.B - val.B) * num3, 0f, 254.99f);
				val.A = (byte)Math.CLAMP((float)(int)val.A + (float)(val2.A - val.A) * num3, 0f, 254.99f);
			}
			val = HUDControl.TintColor(val, tintChannels);
			string stringToDraw2 = TheGame.instance.stringTable.GetString(231) + " " + stringToDraw;
			float num4 = 20f * Game.HUD_SCALE;
			float num5 = Game.game_work.pGameFont.MeasureString(stringToDraw2) * num4 - 48f;
			Game.game_work.pGameFont.DrawString(stringToDraw2, new Vector3(m_pos.X + num5 + MIN_SCALE * 0.7f, m_pos.Y - Game.HUD_SCALE * FONT_SIZE * 0.6f, 0f), val, 20f * Game.HUD_SCALE, Vector2.Zero, (ALIGNMENT_TYPE)14, 0.9f);
		}
		else
		{
			cycle = 0;
		}
		if (!(Game.game_work.gameOverTransition > 0f))
		{
			return;
		}
		if (Game.USE_ARCADE_GO_SCREEN)
		{
			Color colour = default(Color);
			colour = new Color(180, 128, 5, 200);
			stringToDraw = TheGame.instance.stringTable.GetString(231) + " " + m_highScore;
			Game.game_work.pGameFont.DrawString(stringToDraw, (0f - Game.SCREEN_WIDTH) / 2f + GAMEOVER_SCREEN_X_IN, 160f - GAMEOVER_SCREEN_Y_IN - 50f - (1f - Game.game_work.gameOverTransition) * 200f, 0f, colour, 20f * Game.HUD_SCALE, 0f, 0f, ALIGNMENT_TYPE.ALIGN_CENTER);
		}
		if (m_scroreTexture != null)
		{
			m_scroreTexture.Set();
			MatrixManager.GetInstance().Reset();
			if (Game.USE_ARCADE_GO_SCREEN)
			{
				MatrixManager.GetInstance().Scale(new Vector3((float)m_scroreTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX * 0.75f, (float)m_scroreTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX * 0.75f, 0f));
				MatrixManager.GetInstance().Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 2f + GAMEOVER_SCREEN_X_IN, m_textPos.Y + FONT_SIZE * 0.85f, 0f));
			}
			else
			{
				MatrixManager.GetInstance().Scale(new Vector3((float)m_scroreTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX, (float)m_scroreTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX, 0f));
				if (Game.IsMultiplayer())
				{
					MatrixManager.GetInstance().Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 2f - GAMEOVER_SCREEN_X_IN + GAMEOVER_SCREEN_X_IN * 2f * Game.game_work.gameOverTransition, m_textPos.Y + FONT_SIZE + 5f, 0f));
				}
				else
				{
					MatrixManager.GetInstance().Translate(new Vector3((0f - Game.SCREEN_WIDTH) / 2f + GAMEOVER_SCREEN_X_IN, m_textPos.Y + FONT_SIZE + 5f, 0f));
				}
			}
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
		}
		if (m_highscoreTime > 0f && m_highScoreTexture != null)
		{
			m_highScoreTexture.Set();
			MatrixManager.GetInstance().Reset();
			Vector3 scl = new Vector3((float)m_highScoreTexture.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)m_highScoreTexture.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 0f) * Math.SinIdx((ushort)((float)(int)Math.DEGREE_TO_IDX(120f) * m_highscoreTime)) / Math.SinIdx(Math.DEGREE_TO_IDX(120f)) * (1f + Math.SinIdx(m_highscorePulse) * 0.15f);
			Math.Scale44(scl, out var mtx);
			Math.RotZ44(ref mtx, Math.DEGREE_TO_IDX(20f));
			Math.GlobalTranslate44(ref mtx, new Vector3((0f - Game.SCREEN_WIDTH) / 2f + GAMEOVER_SCREEN_X_IN + (float)m_scroreTexture.GetWidth() * 0.5f * Game.GAME_MODE_SCALE_FIX, m_textPos.Y + FONT_SIZE * 0.6f, 0f));
			MatrixManager.GetInstance().SetMatrix(mtx);
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
		}
	}

	public override void Draw(float[] tintChannels)
	{
		if (Game.game_work.gameOverTransition > -1f)
		{
			base.Draw(tintChannels);
		}
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_SCORE;
	}

	public void AddMultipliyer(int score)
	{
	}

	public override void Skip()
	{
		int currentScore = Game.game_work.currentScore;
		m_currentScore = currentScore;
		if (Game.game_work.saveData.go_showHighScore)
		{
			m_highscoreTime = 1f;
		}
	}
}
