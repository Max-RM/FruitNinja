using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mortar;

namespace FruitNinja;

public class SpeedControl : HUDControl3d
{
	private static string[] backingDrumNames = new string[2] { "Combo-Blitz-Backing-Light", "Combo-Blitz-Backing" };

	public ushort m_pulseTime;

	public float m_pulseSpeed;

	public float m_onScreenTime;

	public Vector3 m_originalScale;

	public float m_lossTime;

	public float m_alpha;

	public float m_drumVol;

	private MortarSound m_backing;

	private int m_usingBackingSound;

	public PSPParticleEmitter m_emitter;

	private static bool first = true;

	private static bool firstFrame = true;

	private static VertexPositionColorTexture[] verts = new VertexPositionColorTexture[6];

	public static float MIN_SCALE => 40f * Game.HUD_SCALE;

	public static float MAX_SCALE => 50f * Game.HUD_SCALE;

	public static float GAME_SCREEN_X_IN => 2f * Game.HUD_SCALE;

	public static float GAME_SCREEN_Y_IN => 2f * Game.HUD_SCALE;

	public static Vector3 IN_GAME_POS => new Vector3((0f - Game.SCREEN_WIDTH) / 2f + MIN_SCALE / 2f + GAME_SCREEN_X_IN, Game.SCREEN_HEIGHT / 2f - MIN_SCALE / 2f - GAME_SCREEN_Y_IN, 0f);

	private float LINEAR_TRANSITION_TO(float val, float valTo, float movement)
	{
		if (!(val > valTo))
		{
			if (!(val < valTo))
			{
				return valTo;
			}
			return Math.MIN(valTo, val + movement);
		}
		return Math.MAX(valTo, val - movement);
	}

	public SpeedControl()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		m_texture = TextureManager.GetInstance().Load("extra/white");
		m_pulseTime = 0;
		m_pulseSpeed = 0f;
		m_originalScale = (m_scale = new Vector3((float)m_texture.GetWidth(), (float)m_texture.GetHeight() * 0.125f, 0f) * Game.GAME_MODE_SCALE_FIX);
		m_onScreenTime = 0f;
		m_canBeTinted = false;
		m_lossTime = 1f;
		m_alpha = 0f;
		m_emitter = null;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_BEFORE_SPLAT;
		m_drumVol = 0f;
		for (int i = 0; i < 6; i++)
		{
			verts[i].TextureCoordinate = new Vector2(0.5f, 0.5f);
		}
	}

	public override void Reset()
	{
	}

	public override void Init()
	{
	}

	public override void Update(float dt)
	{
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Invalid comparison between Unknown and I4
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		if (firstFrame)
		{
			if (Game.game_work.pause && m_pulseSpeed > 0f)
			{
				m_onScreenTime = 1f;
				m_alpha = Math.CLAMP(m_lossTime * 1.333f, 0f, 1f);
			}
			firstFrame = false;
		}
		if (Game.game_work.pause)
		{
			return;
		}
		float num = 0f;
		float valTo = 1f;
		if (m_pulseSpeed != 0f)
		{
			if (Game.game_work.gameMode != Game.GAME_MODE.GM_ARCADE || Game.game_work.gameOver)
			{
				m_pulseSpeed = 0f;
			}
			else
			{
				num = Math.CLAMP((WaveManager.GetInstance().GetComboBonusProgression(0) - 0.25f) / 0.75f, 0f, 1f);
				valTo = 1f - num * 0.2f;
				num *= Math.CLAMP(m_alpha * 2f, 0f, 1f);
			}
			m_pulseTime += (ushort)((float)(int)Math.DEGREE_TO_IDX(180f) * dt * Math.CLAMP(m_pulseSpeed, 3f, 20f) * 0.25f);
		}
		else
		{
			if (m_emitter != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(m_emitter);
				m_emitter = null;
			}
			m_lossTime = 0f;
		}
		m_alpha += (Math.CLAMP(m_lossTime * 1.333f, 0f, 1f) - m_alpha) * 0.1f;
		m_alpha += (Math.CLAMP(m_lossTime * 1.333f, 0f, 1f) - m_alpha) * 0.1f;
		if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
		{
			SoundManager.GetInstance().soundFadeOut = LINEAR_TRANSITION_TO(SoundManager.GetInstance().soundFadeOut, valTo, dt);
			if (first)
			{
				m_drumVol = num;
				first = false;
			}
			m_drumVol = LINEAR_TRANSITION_TO(m_drumVol, num, dt / 2f);
		}
		else
		{
			m_drumVol = 0f;
			SoundManager.GetInstance().soundFadeOut = 1f;
		}
		if (m_drumVol > 0f)
		{
			if (m_backing == null)
			{
				m_usingBackingSound = 0;
				m_backing = new MortarSound();
				SoundManager.GetInstance().SFXPlay(backingDrumNames[m_usingBackingSound], 0u, m_backing);
			}
			if (m_backing != null)
			{
				if ((int)m_backing.inst.State == 2)
				{
					m_backing = new MortarSound();
					m_usingBackingSound ^= 1;
					SoundManager.GetInstance().SFXPlay(backingDrumNames[m_usingBackingSound], 0u, m_backing);
				}
				m_backing.SetVolume(m_drumVol * 10f);
			}
		}
		else if (m_backing != null)
		{
			SoundManager.GetInstance().Release(m_backing);
			m_backing = null;
		}
		if (!(m_alpha > 0.01f))
		{
			return;
		}
		m_color = new Color(0.082f, 0.076f, 0f, Math.CLAMP(m_alpha, 0f, 1f) / 16f);
		for (int i = 0; i < 6; i++)
		{
			verts[i].Color = m_color;
			verts[i].Position = Vector3.Zero;
			if (i / 2 != 1)
			{
				verts[i].Position.X = (float)((i > 2) ? 1 : (-1)) * Game.SCREEN_WIDTH / 2f;
				verts[i].Position.Y = (0f - Game.SCREEN_WIDTH) / 2f;
			}
			if (i % 2 == 1)
			{
				ref Vector3 position = ref verts[i].Position;
				position.Y -= Game.SCREEN_WIDTH / 6f;
			}
		}
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_SCORE;
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (m_alpha > 0.01f)
		{
			m_texture.Set();
			float num = (float)(int)m_pulseTime / 65536f;
			num *= Game.SCREEN_WIDTH / 3f;
			num -= Game.SCREEN_HEIGHT / 2f;
			for (int i = 0; i < 4; i++)
			{
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Translate(new Vector3(0f, num, -5550f));
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Mesh.DrawPrimitives2Ex((PrimitiveType)1, verts, 6, hashAlpah: true, 4, i);
				num += Game.SCREEN_WIDTH / 3f;
			}
		}
	}

	public override void Skip()
	{
	}
}
