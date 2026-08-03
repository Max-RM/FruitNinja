using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ScoreMultiplyerBoard : HUDControl3d
{
	public Vector3 m_moveFrom;

	public PowerUp m_power;

	public int m_score;

	public int m_finalScore;

	public float m_time;

	public float m_textScaleAmt;

	public static float MOVE_DOWN_TIME => 0.3f;

	public static float START_WAIT_TIME => 0.1f;

	public static float SCALE_DOWN_TIME => 0.2f;

	public static float SCALE_UP_TIME => 0.2f;

	public static float SCALE_WAIT_TIME => 0.3f;

	public static float MOVE_LEFT_TIME => 0.25f;

	public static Vector3 MOVE_DOWN_AMOUNT => new Vector3(0f, -70f, 0f);

	public static Vector3 MOVE_LEFT_AMOUNT => new Vector3(-150f, 0f, 0f);

	public static float MOVE_DOWN_TIME_START => 0f;

	public static float START_WAIT_TIME_START => MOVE_DOWN_TIME_START + START_WAIT_TIME;

	public static float SCALE_DOWN_TIME_START => START_WAIT_TIME_START + SCALE_DOWN_TIME;

	public static float SCALE_UP_TIME_START => SCALE_DOWN_TIME_START + SCALE_UP_TIME;

	public static float SCALE_WAIT_TIME_START => SCALE_UP_TIME_START + SCALE_WAIT_TIME;

	public static float MOVE_LEFT_TIME_START => SCALE_WAIT_TIME_START + MOVE_LEFT_TIME;

	public static float MOVE_DOWN_TIME_TOTAL => MOVE_DOWN_TIME;

	public static float START_WAIT_TIME_TOTAL => MOVE_DOWN_TIME_TOTAL + START_WAIT_TIME;

	public static float SCALE_DOWN_TIME_TOTAL => START_WAIT_TIME_TOTAL + SCALE_DOWN_TIME;

	public static float SCALE_UP_TIME_TOTAL => SCALE_DOWN_TIME_TOTAL + SCALE_UP_TIME;

	public static float SCALE_WAIT_TIME_TOTAL => SCALE_UP_TIME_TOTAL + SCALE_WAIT_TIME;

	public static float MOVE_LEFT_TIME_TOTAL => SCALE_WAIT_TIME_TOTAL + MOVE_LEFT_TIME;

	public static float ANIMATION_TIME_TOTAL => MOVE_LEFT_TIME_TOTAL;

	public static float MOVE_DOWN_PROG(float t)
	{
		return Math.CLAMP((t - MOVE_DOWN_TIME_START) / MOVE_DOWN_TIME, 0f, 1f);
	}

	public static float START_WAIT_PROG(float t)
	{
		return Math.CLAMP((t - START_WAIT_TIME_START) / START_WAIT_TIME, 0f, 1f);
	}

	public static float SCALE_DOWN_PROG(float t)
	{
		return Math.CLAMP((t - SCALE_DOWN_TIME_START) / SCALE_DOWN_TIME, 0f, 1f);
	}

	public static float SCALE_UP_PROG(float t)
	{
		return Math.CLAMP((t - SCALE_UP_TIME_START) / SCALE_UP_TIME, 0f, 1f);
	}

	public static float SCALE_WAIT_PROG(float t)
	{
		return Math.CLAMP((t - SCALE_WAIT_TIME_START) / SCALE_WAIT_TIME, 0f, 1f);
	}

	public static float MOVE_LEFT_PROG(float t)
	{
		return Math.CLAMP((t - MOVE_LEFT_TIME_START) / MOVE_LEFT_TIME, 0f, 1f);
	}

	private int AddScoreNomals(int score)
	{
		return score;
	}

	public ScoreMultiplyerBoard()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		m_score = 0;
		m_finalScore = -1;
		m_power = null;
		m_time = 0f;
		m_textScaleAmt = 1f;
		m_moveFrom = Vector3.Zero;
	}

	public override void Reset()
	{
		m_score = 0;
		m_finalScore = 0;
		m_terminate = true;
	}

	public override void Update(float dt)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (m_power != null)
		{
			m_score = m_power.GetDeferedPoints();
		}
		else
		{
			if (Game.game_work.pause)
			{
				return;
			}
			float time = m_time;
			m_time += dt;
			float num = 1f - MOVE_DOWN_PROG(m_time);
			num *= num;
			num = 1f - num;
			if (m_finalScore == 0)
			{
				m_pos = m_moveFrom - MOVE_DOWN_AMOUNT * num;
				if (num > 0.99f)
				{
					m_terminate = true;
				}
				return;
			}
			float num2 = MOVE_LEFT_PROG(m_time);
			num2 *= num2;
			m_pos = m_moveFrom + MOVE_DOWN_AMOUNT * num + MOVE_LEFT_AMOUNT * num2;
			if (m_time < SCALE_UP_TIME_START)
			{
				m_textScaleAmt = 1f - SCALE_DOWN_PROG(m_time);
				m_textScaleAmt = Math.SinIdx((ushort)(m_textScaleAmt * (float)(int)Math.DEGREE_TO_IDX(90f)));
			}
			else
			{
				if (time < SCALE_UP_TIME_START)
				{
					PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("bonus_star_impact"), null);
					if (pSPParticleEmitter != null)
					{
						pSPParticleEmitter.pos = m_pos + Vector3.UnitY * 10f;
					}
				}
				m_textScaleAmt = SCALE_UP_PROG(m_time);
				m_textScaleAmt = Math.SinIdx((ushort)(m_textScaleAmt * (float)(int)Math.DEGREE_TO_IDX(113f))) / Math.SinIdx(Math.DEGREE_TO_IDX(113f)) * 1.35f;
			}
			if (m_time > SCALE_WAIT_TIME_START && m_score > 0)
			{
				Game.SetScoreDelegate(AddScoreNomals);
				Game.AddToCurrentScore(m_finalScore);
				PowerUpManager.GetInstance().SetAppropriateScoreCallback();
				m_score = 0;
			}
			if (m_time > ANIMATION_TIME_TOTAL)
			{
				m_terminate = true;
			}
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		float[] tintChannels2 = new float[3] { 1f, 1f, 1f };
		m_color = Color.White;
		base.Draw(tintChannels2);
		Font font = Game.game_work.pNumberFont;
		string stringToDraw;
		if (m_time < SCALE_UP_TIME_START)
		{
			m_color = new Color(20, 150, 20);
			stringToDraw = $"{m_score}";
		}
		else
		{
			font = Game.game_work.pNumberFontBlue2;
			stringToDraw = $"{m_finalScore}";
		}
		font.DrawString(stringToDraw, m_pos + Vector3.UnitY * 10f, m_color, 35f * m_textScaleAmt, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
	}

	public override void Save()
	{
		if (m_power == null && m_score > 0)
		{
			Game.SetScoreDelegate(AddScoreNomals);
			Game.AddToCurrentScore(m_finalScore);
			PowerUpManager.GetInstance().SetAppropriateScoreCallback();
			m_score = 0;
		}
	}
}
