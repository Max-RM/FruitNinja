using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class Jiblet : Entity
{
	private Model m_model;

	private uint m_particleHash;

	private PSPParticleEmitter m_emmitter;

	public Matrix m_orientation;

	private float m_timeTillSplat;

	private float m_splatFrequency;

	private int m_fruitType;

	private Vector3 m_acc;

	private Vector3 m_rotation_speed;

	public float m_time;

	public void Init(int fruitType, Vector3 pos, float scale, Vector3 vel, Model model, uint particles, float splatFrequency, Vector3 acc)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		m_pos = pos;
		m_model = model;
		m_vel = vel;
		m_acc = acc;
		m_cur_scale = Vector3.One * scale;
		m_time = -0.04f;
		m_orientation = Matrix.Identity;
		ushort idx = (ushort)Math.g_random.Rand32();
		m_orientation *= Matrix.CreateRotationX(Math.IDX_TO_RADIANS(idx));
		idx = (ushort)Math.g_random.Rand32();
		m_orientation *= Matrix.CreateRotationY(Math.IDX_TO_RADIANS(idx));
		idx = (ushort)Math.g_random.Rand32();
		m_orientation *= Matrix.CreateRotationZ(Math.IDX_TO_RADIANS(idx));
		base.m_dormant = false;
		base.m_destroy = false;
		m_rotation_speed = new Vector3(Utils.GetRandBetween(-100f, 100f), Utils.GetRandBetween(-100f, 100f), Utils.GetRandBetween(-100f, 100f));
		m_fruitType = fruitType;
		m_splatFrequency = splatFrequency;
		if (m_splatFrequency > 0f)
		{
			m_timeTillSplat = Utils.GetRandBetween(0f, 1f / splatFrequency);
		}
		else
		{
			m_timeTillSplat = 100f;
		}
		m_particleHash = particles;
		m_emmitter = null;
	}

	public override void Update(float dt)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		m_time += dt;
		if (m_emmitter == null && m_time > 0.05f && PSPParticleManager.GetInstance().EmitterExists(m_particleHash))
		{
			m_emmitter = PSPParticleManager.GetInstance().AddEmitter(m_particleHash, null);
			if (m_emmitter != null)
			{
				m_emmitter.canNotBeRepulsed = true;
				Vector3 vel = m_vel;
				vel.Normalize();
				m_emmitter.pos = m_pos;
				m_emmitter.cosz = 0f - vel.Y;
				m_emmitter.sinz = 0f - vel.X;
			}
		}
		if (m_time >= 0f)
		{
			m_pos += (m_vel + m_acc * 0.5f * dt) * dt;
			m_vel += m_acc * dt;
			Quaternion val = Quaternion.CreateFromAxisAngle(new Vector3(1f, 0f, 0f), (float)(int)Math.DEGREE_TO_IDX(m_rotation_speed.X * dt));
			Quaternion val2 = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)(int)Math.DEGREE_TO_IDX(m_rotation_speed.Y * dt));
			Quaternion val3 = Quaternion.CreateFromAxisAngle(new Vector3(0f, 0f, 1f), (float)(int)Math.DEGREE_TO_IDX(m_rotation_speed.Z * dt));
			Quaternion val4 = val * val2 * val3;
			m_orientation *= Matrix.CreateFromQuaternion(val4);
			m_timeTillSplat -= dt;
			while (m_timeTillSplat < 0f && m_splatFrequency > 0f)
			{
				ushort idx = (ushort)Math.g_random.Rand32();
				float randBetween = Utils.GetRandBetween(1f, 40f);
				SplatEntity free = SplatEntity.GetFree();
				free.MakeSplat(m_pos, new Vector3(Math.SinIdx(idx) * randBetween, Math.CosIdx(idx) * randBetween, 0f), canBeLong: false, noSound: true, m_fruitType);
				m_timeTillSplat += 1f / m_splatFrequency;
			}
		}
		if (m_emmitter != null)
		{
			m_emmitter.pos = m_pos;
		}
		if (!Math.BETWEEN(m_pos.X, (0f - Game.SCREEN_WIDTH) * 0.6f, Game.SCREEN_WIDTH * 0.6f) || !Math.BETWEEN(m_pos.Y, (0f - Game.SCREEN_HEIGHT) * 0.6f, Game.SCREEN_HEIGHT * 0.6f))
		{
			Kill();
		}
	}

	private void Kill()
	{
		if (m_emmitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_emmitter);
			m_emmitter = null;
		}
		m_model = null;
		base.m_destroy = true;
	}

	public override void Draw()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (m_model != null)
		{
			Matrix orientation = m_orientation;
			orientation *= Matrix.CreateScale(m_cur_scale);
			orientation *= Matrix.CreateTranslation(m_pos);
			m_model.Draw(orientation);
		}
	}

	public override void DrawUpdate(float dt)
	{
	}
}
