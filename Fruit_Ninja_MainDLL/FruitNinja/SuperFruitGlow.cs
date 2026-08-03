using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class SuperFruitGlow : HUDControl3d
{
	private bool m_shouldFadeAway;

	private Fruit m_fruit;

	private MortarSound m_loopSound;

	private float m_fadeOutTime;

	public static Texture GlowTexture;

	public SuperFruitGlow(Fruit fruit)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		m_fruit = fruit;
		m_fadeOutTime = 0f;
		m_loopSound = null;
		m_shouldFadeAway = false;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		Fruit fruit2 = m_fruit;
		fruit2.m_fruitKilled = (Fruit.FruitEvent)Delegate.Combine(fruit2.m_fruitKilled, new Fruit.FruitEvent(FruitWasKilled));
		m_texture = GlowTexture;
		m_rotation = Utils.GetRandBetween(10f, 170f);
		m_loopSound = new MortarSound();
		SoundManager.GetInstance().SFXPlay("pome-lp", 0u, m_loopSound, 0, 0, loop: true);
		m_scale = Vector3.One * 150f;
	}

	private void FruitWasKilled(Fruit fruit)
	{
		if (fruit == m_fruit)
		{
			m_fruit = null;
		}
		m_shouldFadeAway = true;
		StopSound();
	}

	~SuperFruitGlow()
	{
		Release();
	}

	public override void Release()
	{
		m_texture = null;
		if (m_fruit != null)
		{
			Fruit fruit = m_fruit;
			fruit.m_fruitKilled = (Fruit.FruitEvent)Delegate.Remove(fruit.m_fruitKilled, new Fruit.FruitEvent(FruitWasKilled));
		}
		StopSound();
	}

	public void StopSound()
	{
		if (m_loopSound != null)
		{
			m_loopSound.Stop(0f);
			m_loopSound = null;
		}
	}

	public override void Update(float dt)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (!Game.game_work.pause)
		{
			m_rotation += dt * 60f;
			if (m_fruit != null && m_fruit.Sliced())
			{
				m_shouldFadeAway = true;
			}
			if (m_fruit == null)
			{
				m_terminate = true;
			}
			if (!m_shouldFadeAway)
			{
				m_fadeOutTime = Mortar.Math.MIN(m_fadeOutTime + dt * 2f, 1f);
			}
			else
			{
				m_fadeOutTime -= dt * 2f;
				if (m_fadeOutTime <= 0f)
				{
					StopSound();
					m_terminate = true;
				}
			}
		}
		if (m_fruit != null)
		{
			m_pos = m_fruit.m_pos;
			m_pos.Z = m_fruit.m_z - 40f;
			m_pos = Game.game_work.camera.TranslatePos(m_pos, screenToWorld: false, includeShake: true);
		}
		m_color = new Color(255f, 255f, 255f, 75f * m_fadeOutTime);
		if (m_loopSound != null)
		{
			m_loopSound.SetVolume(Game.game_work.pause ? 0f : m_fadeOutTime);
		}
	}

	public override void DrawOrder(float[] tintChannels, int order)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 scale = m_scale;
		m_scale *= m_fadeOutTime;
		Draw(tintChannels);
		m_rotation = 0f - m_rotation;
		Draw(tintChannels);
		m_rotation = 0f - m_rotation;
		m_scale = scale;
	}
}
