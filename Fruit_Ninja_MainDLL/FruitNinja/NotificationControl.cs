using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class NotificationControl : HUDControl3d
{
	public enum NotificationType
	{
		NETWORK_NOTIFICATION,
		ACHIEVEMENT_UNLOCK,
		ITEM_UNLOCK
	}

	public static Texture s_banner;

	public static Texture s_unlockBanner;

	public float m_textScale;

	public float m_time;

	public int m_score;

	public string m_text;

	public string m_numberText;

	public NotificationType m_type;

	public static float NOTIFICATION_TIME => 1f;

	public static float NOTIFICATION_IN_TIME => 0.2f;

	public static float NOTIFICATION_WAIT_TIME => 2.5f;

	public static float NOTIFICATION_OUT_TIME => 0.2f;

	public static float NOTIFICATION_IN_TIME_TOTAL => NOTIFICATION_IN_TIME;

	public static float NOTIFICATION_WAIT_TIME_TOTAL => NOTIFICATION_IN_TIME_TOTAL + NOTIFICATION_WAIT_TIME;

	public static float NOTIFICATION_OUT_TIME_TOTAL => NOTIFICATION_WAIT_TIME_TOTAL + NOTIFICATION_OUT_TIME;

	public static Vector3 NOTIFICATION_START_POS(NotificationType type)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (type == NotificationType.ITEM_UNLOCK)
		{
			return new Vector3(-95f, Game.SCREEN_HEIGHT / 2f + 32f, 0f);
		}
		return new Vector3(-95f, Game.SCREEN_HEIGHT / 2f + 24f, 0f);
	}

	public static Vector3 NOTIFICATION_DEST_POS(NotificationType type)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (type == NotificationType.ITEM_UNLOCK)
		{
			return new Vector3(-95f, Game.SCREEN_HEIGHT / 2f - 32f, 0f);
		}
		return new Vector3(-95f, Game.SCREEN_HEIGHT / 2f - 13f, 0f);
	}

	public static Vector3 NOTIFICATION_POS_DELTA(NotificationType type)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (type == NotificationType.ITEM_UNLOCK)
		{
			return new Vector3(0f, -64f, 0f);
		}
		return new Vector3(0f, -37f, 0f);
	}

	public static float NOTIFICATION_IN_TIME_PROGRESS(float t)
	{
		return t / NOTIFICATION_IN_TIME;
	}

	public static float NOTIFICATION_WAIT_TIME_PROGRESS(float t)
	{
		return (t - NOTIFICATION_IN_TIME_TOTAL) / NOTIFICATION_WAIT_TIME;
	}

	public static float NOTIFICATION_OUT_TIME_PROGRESS(float t)
	{
		return (t - NOTIFICATION_WAIT_TIME_TOTAL) / NOTIFICATION_OUT_TIME;
	}

	public NotificationControl(string text, int score)
		: this(text, score, null)
	{
	}

	public NotificationControl(string text, int score, Texture texture)
		: this(text, score, texture, NotificationType.ACHIEVEMENT_UNLOCK)
	{
	}

	public NotificationControl(string text, int score, Texture texture, NotificationType type)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		m_type = type;
		m_texture = texture;
		m_selfCleanUp = false;
		m_text = text.ToUpper();
		float num = 170f;
		if (score >= 0 && type == NotificationType.ACHIEVEMENT_UNLOCK)
		{
			m_numberText = string.Concat(score);
		}
		else
		{
			num = 195f;
			m_numberText = "";
		}
		m_textScale = 16f;
		m_type = type;
		if (type == NotificationType.ITEM_UNLOCK)
		{
			SoundManager.GetInstance().SFXPlay(SoundDef.SND_FAN_FARE);
		}
		float num2 = Game.game_work.pGameFont.MeasureString(m_text) * m_textScale;
		if (num2 > num)
		{
			m_textScale *= num / num2;
		}
		m_time = 0f;
		m_score = score;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		m_pos = new Vector3(-996f, -996f, 0f);
	}

	public override void Reset()
	{
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		float time = m_time;
		m_time += dt;
		if (m_time > NOTIFICATION_WAIT_TIME_TOTAL)
		{
			if (m_time > NOTIFICATION_OUT_TIME_TOTAL)
			{
				m_time = NOTIFICATION_OUT_TIME_TOTAL;
				m_terminate = true;
			}
			float num = 1f - NOTIFICATION_OUT_TIME_PROGRESS(m_time);
			num *= num;
			m_pos = NOTIFICATION_START_POS(m_type) + NOTIFICATION_POS_DELTA(m_type) * num;
		}
		else if (m_time > NOTIFICATION_IN_TIME_TOTAL)
		{
			m_pos = NOTIFICATION_DEST_POS(m_type);
		}
		else
		{
			float num2 = NOTIFICATION_IN_TIME_PROGRESS(m_time);
			num2 *= num2;
			m_pos = NOTIFICATION_START_POS(m_type) + NOTIFICATION_POS_DELTA(m_type) * num2;
		}
		if (m_type == NotificationType.ITEM_UNLOCK && (int)(time * 8f) != (int)(m_time * 8f) && (int)(m_time * 8f) < 4)
		{
			uint hash = StringFunctions.StringHash("confetti");
			PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(hash, null);
			if (pSPParticleEmitter != null)
			{
				pSPParticleEmitter.pos = new Vector3(Math.g_random.RandF(20f) - 10f - 100f + (float)(100 * (int)(time * 8f)), Game.SCREEN_HEIGHT / 2f - (float)Math.g_random.Rand32(5) - 25f, 0f);
				pSPParticleEmitter.updateEvenIfPaused = true;
				pSPParticleEmitter.sizeScale = 1f;
			}
			Math.g_random.Rand32(Math.DEGREE_TO_IDX(20f));
			Math.DEGREE_TO_IDX(10f);
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		switch (m_type)
		{
		case NotificationType.NETWORK_NOTIFICATION:
		case NotificationType.ACHIEVEMENT_UNLOCK:
		{
			if (s_banner != null)
			{
				s_banner.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3(257f, 65f, 1f));
				MatrixManager.GetInstance().Translate(new Vector3(0f, m_pos.Y - 16f, 1f));
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Mesh.DrawQuad(Color.White, 0f, 1f, (m_numberText.Length > 0) ? 0f : 0.5f, (m_numberText.Length > 0) ? (15f / 32f) : (31f / 32f));
			}
			if (m_texture != null)
			{
				m_texture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3(30f, 30f, 30f));
				MatrixManager.GetInstance().Translate(m_pos);
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Mesh.DrawQuad(Color.White);
			}
			float num = 18f;
			float num2 = 186f;
			Game.game_work.pGameFont.DrawString(m_text, m_pos.X + num, m_pos.Y, 0f, new Color(50, 50, 50, 255), m_textScale, 0f, 0f, ALIGNMENT_TYPE.ALIGN_VCENTER);
			if (m_numberText.Length > 0)
			{
				Game.game_work.pGameFont.DrawString(m_numberText, m_pos.X + num2, m_pos.Y, 0f, new Color(50, 50, 50, 255), m_textScale, 0f, 0f, ALIGNMENT_TYPE.ALIGN_VCENTER);
			}
			break;
		}
		case NotificationType.ITEM_UNLOCK:
			if (s_unlockBanner != null)
			{
				s_unlockBanner.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3(257f, 65f, 1f));
				MatrixManager.GetInstance().Translate(new Vector3(0f, m_pos.Y, 1f));
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 63f / 64f);
			}
			if (m_texture != null)
			{
				m_texture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3(32f, 32f, 32f));
				MatrixManager.GetInstance().Translate(m_pos + new Vector3(0f, 16f, 0f));
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Mesh.DrawQuad(Color.White);
			}
			Game.game_work.pGameFont.DrawString(m_text, m_pos.X + 18f, m_pos.Y + 16f, 0f, new Color(50, 50, 50, 255), m_textScale, 0f, 0f, ALIGNMENT_TYPE.ALIGN_VCENTER);
			break;
		}
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_NOTIFICATION;
	}
}
