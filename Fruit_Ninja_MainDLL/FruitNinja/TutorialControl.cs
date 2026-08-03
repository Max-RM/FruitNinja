using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class TutorialControl : HUDControl3d
{
	protected float m_fingerTime;

	protected Vector3 m_fingerPos;

	protected Texture m_ringTex;

	protected Color m_fingerColor;

	protected int m_fingerFrame;

	protected float m_fingerScale;

	public static float FINGER_TOUCH_ADD => 9.5f;

	public static float FINGER_WAIT_TIME => 10f;

	public static float FINGER_IN_TIME => 0.35f;

	public static float FINGER_START_WAIT_TIME => 0.25f;

	public static float FINGER_ANIMATE_TIME => 0.4f;

	public static float FINGER_SWIPE_TIME => 0.5f;

	public static float FINGER_END_WAIT_TIME => 0.75f;

	public static float FINGER_OUT_TIME => 0.5f;

	public static float FINGER_WAIT_TIME_TOTAL => 0f - FINGER_WAIT_TIME;

	public static float FINGER_IN_TIME_TOTAL => FINGER_IN_TIME;

	public static float FINGER_START_WAIT_TIME_TOTAL => FINGER_IN_TIME_TOTAL + FINGER_START_WAIT_TIME;

	public static float FINGER_ANIMATE_TIME_TOTAL => FINGER_START_WAIT_TIME_TOTAL + FINGER_ANIMATE_TIME;

	public static float FINGER_SWIPE_TIME_TOTAL => FINGER_ANIMATE_TIME_TOTAL + FINGER_SWIPE_TIME;

	public static float FINGER_END_WAIT_TIME_TOTAL => FINGER_SWIPE_TIME_TOTAL + FINGER_END_WAIT_TIME;

	public static float FINGER_OUT_TIME_TOTAL => FINGER_END_WAIT_TIME_TOTAL + FINGER_OUT_TIME;

	public static float FINGER_TIME_TOTAL => FINGER_OUT_TIME_TOTAL;

	public static float FINGER_DOWN_MOVE => 20f;

	public static Vector3 FINGER_START_POS => new Vector3(-0.5f, -0.075f, 0f);

	public static Vector3 FINGER_END_POS => new Vector3(0.5f, 0.075f, 0f);

	public static Vector3 FINGER_POS_DIFF => new Vector3(1f, 0.15f, 0f);

	public static float FINGER_WAIT_TIME_PROGRESS(float t)
	{
		return t / FINGER_WAIT_TIME;
	}

	public static float FINGER_IN_TIME_PROGRESS(float t)
	{
		return t / FINGER_IN_TIME;
	}

	public static float FINGER_START_WAIT_TIME_PROGRESS(float t)
	{
		return (t - FINGER_IN_TIME_TOTAL) / FINGER_START_WAIT_TIME;
	}

	public static float FINGER_ANIMATE_TIME_PROGRESS(float t)
	{
		return (t - FINGER_START_WAIT_TIME_TOTAL) / FINGER_ANIMATE_TIME;
	}

	public static float FINGER_SWIPE_TIME_PROGRESS(float t)
	{
		return (t - FINGER_ANIMATE_TIME_TOTAL) / FINGER_SWIPE_TIME;
	}

	public static float FINGER_END_WAIT_TIME_PROGRESS(float t)
	{
		return (t - FINGER_SWIPE_TIME_TOTAL) / FINGER_END_WAIT_TIME;
	}

	public static float FINGER_OUT_TIME_PROGRESS(float t)
	{
		return (t - FINGER_END_WAIT_TIME_TOTAL) / FINGER_OUT_TIME;
	}

	public static Vector3 FINGER_POS(float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return FINGER_START_POS + FINGER_POS_DIFF * Math.CLAMP(FINGER_SWIPE_TIME_PROGRESS(t), 0f, 1f);
	}

	public TutorialControl()
	{
		m_texture = TextureManager.GetInstance().Load("textureswp7/swipe_fruit_begin.tex");
		m_ringTex = TextureManager.GetInstance().Load("textureswp7/press_indicate.tex");
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
	}

	public override void Init()
	{
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		Reset();
	}

	public override void Release()
	{
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		if (!(m_fingerTime > 0f))
		{
			return;
		}
		if (m_fingerTime > FINGER_START_WAIT_TIME_TOTAL && m_fingerTime < FINGER_END_WAIT_TIME_TOTAL)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = (int)(m_fingerTime * 2000f) % 1000;
				float num2 = (float)i + (float)num / 1000f;
				int num3 = (int)Math.CLAMP(255f - 255f * (num2 - 3f), 0f, 255f);
				if (m_fingerTime < FINGER_START_WAIT_TIME_TOTAL + 0.25f)
				{
					num3 *= (int)((m_fingerTime - FINGER_START_WAIT_TIME_TOTAL) / 0.25f);
				}
				else if (m_fingerTime > FINGER_END_WAIT_TIME_TOTAL - 0.25f)
				{
					num3 *= (int)(1f - (m_fingerTime - (FINGER_END_WAIT_TIME_TOTAL - 0.25f)) / 0.25f);
				}
				num2 *= 2f;
				num2 *= num2;
				m_ringTex.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(Vector3.One * num2);
				MatrixManager.GetInstance().Translate(m_fingerPos);
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Mesh.DrawQuad(new Color(255, 255, 255, num3), 0f, 1f, 0f, 1f);
			}
		}
		float num4 = ((!(m_pos.X > 0f)) ? 1 : (-1));
		m_texture.Set();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(new Vector3(96f * num4, 96f, 1f));
		MatrixManager.GetInstance().Translate(m_fingerPos - new Vector3(-0.125f * num4, -13f / 32f, 0f) * 96f);
		MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
		Mesh.DrawQuad(m_fingerColor, (float)m_fingerFrame * 0.5f, (float)m_fingerFrame * 0.5f + 0.5f, 0f, 1f);
	}

	public override void Update(float dt)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		m_fingerPos = new Vector3(-1000f, -1000f, -1000f);
		m_fingerColor = new Color(0, 0, 0, 0);
		if (CanShowTute())
		{
			if (m_fingerTime < FINGER_TIME_TOTAL)
			{
				m_fingerTime += dt;
				m_fingerPos = FINGER_POS(m_fingerTime);
				m_fingerPos *= m_fingerScale;
				if (m_pos.X > 0f)
				{
					ref Vector3 fingerPos = ref m_fingerPos;
					fingerPos.X *= -1f;
				}
				m_fingerFrame = 1;
				if (m_fingerTime > 0f)
				{
					m_fingerColor = Color.White;
					if (m_fingerTime < FINGER_IN_TIME_TOTAL)
					{
						m_fingerColor.A = (byte)(255f * FINGER_IN_TIME_PROGRESS(m_fingerTime));
						m_fingerFrame = 0;
						ref Vector3 fingerPos2 = ref m_fingerPos;
						fingerPos2.Y += FINGER_DOWN_MOVE;
					}
					else if (m_fingerTime < FINGER_START_WAIT_TIME_TOTAL)
					{
						m_fingerFrame = 0;
						ref Vector3 fingerPos3 = ref m_fingerPos;
						fingerPos3.Y += FINGER_DOWN_MOVE - FINGER_START_WAIT_TIME_PROGRESS(m_fingerTime) * FINGER_DOWN_MOVE;
					}
					else if (!(m_fingerTime < FINGER_ANIMATE_TIME_TOTAL) && !(m_fingerTime < FINGER_SWIPE_TIME_TOTAL) && !(m_fingerTime < FINGER_END_WAIT_TIME_TOTAL))
					{
						if (m_fingerTime < FINGER_OUT_TIME_TOTAL)
						{
							m_fingerColor.A = (byte)(255f - 254f * FINGER_OUT_TIME_PROGRESS(m_fingerTime));
							m_fingerFrame = 0;
							ref Vector3 fingerPos4 = ref m_fingerPos;
							fingerPos4.Y += FINGER_DOWN_MOVE;
						}
						else
						{
							m_fingerFrame = 0;
							ref Vector3 fingerPos5 = ref m_fingerPos;
							fingerPos5.Y += FINGER_DOWN_MOVE;
							m_fingerTime = FINGER_WAIT_TIME_TOTAL;
						}
					}
				}
			}
		}
		else
		{
			m_fingerTime = FINGER_WAIT_TIME_TOTAL;
		}
		m_fingerPos += m_pos;
	}

	public override void Reset()
	{
		m_fingerTime = FINGER_WAIT_TIME_TOTAL;
	}

	public void ButtonPressedAtPos(MenuButton button)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!(m_fingerTime < 0f))
		{
			return;
		}
		if (button != null)
		{
			m_pos = button.m_pos;
			m_fingerScale = button.m_originalScale.X;
			if (m_fingerScale > 256f)
			{
				m_fingerScale *= 0.5f;
			}
		}
		m_fingerTime += FINGER_TOUCH_ADD;
		if (m_fingerTime > 0f)
		{
			m_fingerTime = 0f;
		}
	}

	public void ResetTutePos()
	{
		ResetTutePos(null);
	}

	public void ResetTutePos(MenuButton button)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (button != null)
		{
			m_pos = button.m_pos;
			m_fingerScale = button.m_originalScale.X;
			if (m_fingerScale > 256f)
			{
				m_fingerScale *= 0.5f;
			}
		}
		m_fingerTime = FINGER_WAIT_TIME_TOTAL;
	}

	public bool CanShowTute()
	{
		return Math.Abs(Game.game_work.gameOverTransition) > 0.99f;
	}
}
