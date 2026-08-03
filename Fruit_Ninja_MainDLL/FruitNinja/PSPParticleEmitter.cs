using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class PSPParticleEmitter
{
	public float time;

	public ushort enabled;

	public Vector3 pos;

	public Vector3 _vel;

	public float rateScale;

	public float lifeScale;

	public float sizeScale;

	public float dtMod;

	public float cosz;

	public float sinz;

	public float scale;

	public bool canNotBeRepulsed;

	public PSPEmitterTemplate tmplt;

	public Action<PSPParticleEmitter> zeroer;

	public bool updateEvenIfPaused;

	public void Update(float dt)
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		PSPParticleManager instance = PSPParticleManager.GetInstance();
		for (int i = 0; i < tmplt.particle_set_num; i++)
		{
			PSPParticleSet pSPParticleSet = tmplt.sets[i];
			if (time >= pSPParticleSet.time_start && (pSPParticleSet.time_end == 0f || time <= pSPParticleSet.time_end))
			{
				int num = (int)((float)(int)pSPParticleSet.number_per_second * (time - pSPParticleSet.time_start));
				int num2 = (int)((float)(int)pSPParticleSet.number_per_second * (time + dt * rateScale - pSPParticleSet.time_start));
				int num3 = num2 - num;
				for (int j = 0; j < num3; j++)
				{
					AddParticle(pSPParticleSet, instance);
				}
			}
			if (time == pSPParticleSet.time_start)
			{
				for (int k = 0; k < pSPParticleSet.number_start; k++)
				{
					AddParticle(pSPParticleSet, instance);
				}
				if (rateScale == 0f)
				{
					time += dt;
				}
			}
		}
		time += dt * rateScale;
		pos += _vel;
	}

	public void AddParticle(PSPParticleSet t, PSPParticleManager pm)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		if (t.template_idx == null || pm.particleFreeList == 0)
		{
			return;
		}
		pm.m_activeParticles++;
		PSPParticleTemplate template_idx = t.template_idx;
		ushort particleFreeList = pm.particleFreeList;
		PSPParticle pSPParticle = pm.particles[particleFreeList];
		pm.particleFreeList = (ushort)pSPParticle.next_particle;
		pSPParticle.next_particle = template_idx.first_particle;
		template_idx.first_particle = particleFreeList;
		if (pSPParticle != null)
		{
			pSPParticle.time = template_idx.life;
			pSPParticle.lifeScale = template_idx.life - lifeScale * template_idx.life;
			if (template_idx.coord_type == 0)
			{
				pSPParticle.pos = pos;
			}
			else
			{
				pSPParticle.pos = Vector3.Zero;
			}
			pSPParticle.owner = this;
			pSPParticle.gravity = template_idx.gravity_min + (template_idx.gravity_max - template_idx.gravity_min) * Mortar.Math.g_random.RandF(1f);
			if (Game.IsMultiplayer() && !updateEvenIfPaused)
			{
				float x = pSPParticle.gravity.X;
				pSPParticle.gravity.X = pSPParticle.gravity.Y;
				pSPParticle.gravity.Y = x;
				ref Vector3 gravity = ref pSPParticle.gravity;
				gravity.X *= (float)(-Mortar.Math.MATH_SIGN(pSPParticle.pos.X));
				pSPParticle.gravity *= scale;
			}
			Vector3 val = default(Vector3);
			val = new Vector3(t.vel_min.X + (t.vel_max.X - t.vel_min.X) * Mortar.Math.g_random.RandF(1f), t.vel_min.Y + (t.vel_max.Y - t.vel_min.Y) * Mortar.Math.g_random.RandF(1f), t.vel_min.Z + (t.vel_max.Z - t.vel_min.Z) * Mortar.Math.g_random.RandF(1f));
			val *= scale;
			float x2 = val.X;
			float y = val.Y;
			val.X = x2 * cosz + y * sinz;
			val.Y = x2 * (0f - sinz) + y * cosz;
			val /= 2f;
			pSPParticle.vel.X = val.X;
			pSPParticle.vel.Y = val.Y;
			pSPParticle.vel.Z = val.Z;
			pSPParticle.current_spin = Mortar.Math.DEGREE_TO_IDX(Mortar.Math.floatLERP(template_idx.angleMin, template_idx.angleMax, Mortar.Math.g_random.RandF(1f)));
			if (template_idx.particle_type == 1)
			{
				pSPParticle.pos -= pSPParticle.vel * template_idx.life;
			}
			float num = Mortar.Math.LERP(template_idx.size_start_min, template_idx.size_start_max, Mortar.Math.g_random.Rand32(4095) & 0xFFF);
			float num2 = Mortar.Math.LERP(template_idx.size_mid_min, template_idx.size_mid_max, Mortar.Math.g_random.Rand32(4095) & 0xFFF);
			float num3 = Mortar.Math.LERP(template_idx.size_end_min, template_idx.size_end_max, Mortar.Math.g_random.Rand32(4095) & 0xFFF);
			pSPParticle.sizeS = (ushort)(num * sizeScale);
			pSPParticle.sizeInc = (short)((num2 - num) * sizeScale);
			pSPParticle.sizeInc2 = (short)((num3 - num2) * sizeScale);
			for (int i = 0; i < 4; i++)
			{
				long num4 = template_idx.color_start_min[i];
				long num5 = template_idx.color_start_max[i];
				float num6 = Mortar.Math.g_random.RandF(1f);
				int num7 = (int)((float)num4 + (float)(num5 - num4) * num6);
				num4 = template_idx.color_mid_min[i];
				num5 = template_idx.color_mid_max[i];
				scale = Mortar.Math.g_random.RandF(1f);
				int num8 = (int)((float)num4 + (float)(num5 - num4) * num6);
				num4 = template_idx.color_end_min[i];
				num5 = template_idx.color_end_max[i];
				scale = Mortar.Math.g_random.RandF(1f);
				int num9 = (int)((float)num4 + (float)(num5 - num4) * num6);
				pSPParticle.Cs[i] = (byte)num7;
				pSPParticle.Ci[i] = (short)(num8 - num7);
				pSPParticle.Ci2[i] = (short)(num9 - num8);
			}
			if (template_idx.particle_type == 2)
			{
				pSPParticle.current_spin += Mortar.Math.Atan2Idx(pSPParticle.vel.X, pSPParticle.vel.Y);
			}
			float t2 = Mortar.Math.g_random.RandF(1f);
			pSPParticle.spin_speed_start = Mortar.Math.floatLERP(template_idx.spin_start_min, template_idx.spin_start_max, t2);
			pSPParticle.spin_speed_end = Mortar.Math.floatLERP(template_idx.spin_end_min, template_idx.spin_end_max, t2);
			t2 = Mortar.Math.g_random.RandF(1f);
			pSPParticle.cycleX_speed_start = Mortar.Math.floatLERP(template_idx.cycleX_start_min, template_idx.cycleX_start_max, t2);
			pSPParticle.cycleX_speed_end = Mortar.Math.floatLERP(template_idx.cycleX_end_min, template_idx.cycleX_end_max, t2);
			if (pSPParticle.cycleX_speed_end != 0f || pSPParticle.cycleX_speed_start != 0f)
			{
				pSPParticle.current_x_scale_cycle = (ushort)Mortar.Math.g_random.Rand32();
			}
			else
			{
				pSPParticle.current_x_scale_cycle = 0;
			}
			t2 = Mortar.Math.g_random.RandF(1f);
			pSPParticle.cycleY_speed_start = Mortar.Math.floatLERP(template_idx.cycleY_start_min, template_idx.cycleY_start_max, t2);
			pSPParticle.cycleY_speed_end = Mortar.Math.floatLERP(template_idx.cycleY_end_min, template_idx.cycleY_end_max, t2);
			if (pSPParticle.cycleY_speed_end != 0f || pSPParticle.cycleY_speed_start != 0f)
			{
				pSPParticle.current_y_scale_cycle = (ushort)Mortar.Math.g_random.Rand32();
			}
			else
			{
				pSPParticle.current_y_scale_cycle = 0;
			}
			if (pSPParticle.current_spin == 0)
			{
				pSPParticle.sinz = -1f;
				pSPParticle.cosz = 1f;
				pSPParticle.vecX = new Vector2(1f, 0f);
				pSPParticle.vecY = new Vector2(0f, 1f);
			}
			else
			{
				pSPParticle.vecX = new Vector2(Mortar.Math.SinIdx((ushort)(pSPParticle.current_spin + 16384)), Mortar.Math.CosIdx((ushort)(pSPParticle.current_spin + 16384)));
				pSPParticle.vecY = new Vector2(Mortar.Math.SinIdx(pSPParticle.current_spin), Mortar.Math.CosIdx(pSPParticle.current_spin));
				ushort idx = (ushort)((pSPParticle.current_spin + 57330) % 65520);
				pSPParticle.sinz = Mortar.Math.SinIdx(idx) * 1.41f;
				pSPParticle.cosz = Mortar.Math.CosIdx(idx) * 1.41f;
			}
		}
	}
}
