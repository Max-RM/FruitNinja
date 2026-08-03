using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class TimeControl : HUDControl3d
{
	private float m_time;

	private string m_text;

	private float m_countingDownFrom;

	private float m_timeImageFrame;

	private string m_stopTimeText;

	private float[] zen_times = new float[5] { 180f, 120f, 90f, 60f, 30f };

	private static float normalX;

	private static bool beenInUpdateBefore = false;

	private static bool tickTock = true;

	public static int MAX_MULTIPLYERS => 8;

	public TimeControl()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		m_texture = null;
		m_countingDownFrom = -1f;
		m_scale = new Vector3(0f, 18f, 0f);
		m_pos = new Vector3((Game.SCREEN_WIDTH - m_scale.X) / 2f - 5f, (Game.SCREEN_HEIGHT + m_scale.Y) / 2f - 5f, 0f);
		m_selfCleanUp = false;
		m_stopTimeText = "";
		Reset();
	}

	public override void Reset()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		m_time = Math.MAX(0f, m_countingDownFrom);
		m_stopTimeText = "";
		if (Game.IsMultiplayer())
		{
			m_time = zen_times[(Game.game_work.saveData.GetTotal(StringFunctions.StringHash("vs_option_zen")) - 1) % zen_times.Length] + 0.9f;
		}
		else if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			m_time = 60.9f;
			if (Game.game_work.saveData.timer == 0f && Game.game_work.gameOverTransition <= 0f)
			{
				Game.game_work.saveData.timer = m_time;
			}
		}
		m_timeImageFrame = 0f;
		m_color = Color.White;
	}

	public float GetCountDown()
	{
		if (Game.game_work.gameMode != Game.GAME_MODE.GM_ARCADE)
		{
			return m_countingDownFrom;
		}
		return 60.9f;
	}

	public void AddTime(float time)
	{
		m_time += time;
	}

	public void SetTime(float time)
	{
		m_time = time;
	}

	public float GetTime()
	{
		return m_time;
	}

	public override void Release()
	{
		m_texture = null;
	}

	public override void Init()
	{
		Reset();
	}

	public override void Update(float dt)
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		if (!beenInUpdateBefore)
		{
			beenInUpdateBefore = true;
			normalX = m_pos.X;
		}
		if (Game.game_work.gameMode == Game.GAME_MODE.GM_ZEN || Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
			m_pos.X = normalX;
			if (!Game.game_work.pause && !Game.game_work.gameOver)
			{
				if (m_countingDownFrom > 0f)
				{
					int b = m_color.B;
					float num = 0f;
					if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
					{
						num = PowerUpManager.GetInstance().GetStopClockAmount();
					}
					if (num > 0f)
					{
						m_color = new Color(255, 100, 100, 255);
						m_stopTimeText = $"+{(int)num + 1}";
					}
					else
					{
						if (SuperFruitControl.IsInSuperFruitState())
						{
							dt = 0f;
						}
						m_stopTimeText = "";
						if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
						{
							m_time -= dt * PowerUpManager.GetInstance().SlowClock();
						}
						else
						{
							m_time -= dt;
						}
						if (m_time < 0.5f)
						{
							Game.GameOver();
							Fruit.s_consecutiveCount = 0;
							Fruit.s_consecutiveType = -1;
							m_time = 0f;
							m_color = new Color(255, 100, 100, 255);
							SoundManager.GetInstance().SFXPlay(SoundDef.SND_TIME_UP);
						}
						else if (m_time < 3f)
						{
							m_color = (Color)(((int)(m_time * 8f) % 2 != 0) ? new Color(255, 100, 100, 255) : Color.White);
						}
						else if (m_time < 6f)
						{
							m_color = (Color)(((int)(m_time * 4f) % 2 != 0) ? new Color(255, 100, 100, 255) : Color.White);
						}
						else if (m_time < 11f)
						{
							m_color = (Color)(((int)(m_time * 2f) % 2 != 0) ? new Color(255, 100, 100, 255) : Color.White);
						}
						if (m_time > 0f && m_time < 11f && b != m_color.B)
						{
							tickTock = !tickTock;
							SoundManager.GetInstance().SFXPlay(tickTock ? SoundDef.SND_TIME_TICK : SoundDef.SND_TIME_TOCK);
						}
						m_timeImageFrame = (float)((int)(m_countingDownFrom - m_time) % 6) + 0.5f;
					}
				}
				else
				{
					m_time += dt;
					m_timeImageFrame = (float)((int)m_time % 6) + 0.5f;
				}
			}
			Game.game_work.saveData.timer = m_time;
			int num2 = (int)m_time % 60;
			int num3 = (int)(m_time / 60f);
			m_text = string.Format("{0}:{2}{1}", num3, num2, (num2 < 10) ? "0" : "");
			m_pos.Y = (Game.SCREEN_HEIGHT + 2f * m_scale.Y) / 2f - 2f * m_scale.Y * (1f - Math.Abs(Game.game_work.gameOverTransition));
			if (Game.IsMultiplayer())
			{
				ref Vector3 pos = ref m_pos;
				pos.Y *= -1f;
				ref Vector3 pos2 = ref m_pos;
				pos2.Y += 5f;
			}
		}
		else
		{
			m_drawOrder = (HUD.HUD_ORDER)(-1);
			Game.game_work.saveData.timer = -1f;
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if ((Math.Abs(Game.game_work.gameOverTransition) < 1f && Game.game_work.gameMode == Game.GAME_MODE.GM_ZEN) || Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			Game.game_work.pNumberFont.DrawString(m_text, m_pos.X - m_scale.X * 0.6f, m_pos.Y, 0f, HUDControl.TintColor(m_color, tintChannels), 32f, 0f, 0f, (ALIGNMENT_TYPE)14);
		}
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_TIMER;
	}

	public override void Skip()
	{
		m_time = Game.game_work.saveData.timer;
		m_timeImageFrame = 0f;
	}

	public void CountDown(float time)
	{
		m_countingDownFrom = time;
	}
}
