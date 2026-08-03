using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class FruitSplosion : HUDControl3d
{
	private static FruitSplosion controlThatMadeMe;

	private Fruit fruit;

	private float time;

	private float m_maxRadius;

	private float m_growTime;

	private float m_waitTime;

	private float m_fadeTime;

	private int m_comboType;

	private FruitSplosion m_lastCreatedChild;

	private FruitSplosion m_root;

	private int m_comboCount;

	public FruitSplosion(Fruit f, float rad, float growTime, float waitTime, float fadeTime, int comboType)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		fruit = f;
		time = 0f;
		m_root = null;
		m_lastCreatedChild = null;
		m_maxRadius = rad;
		m_growTime = growTime;
		m_waitTime = waitTime;
		m_fadeTime = fadeTime;
		m_comboType = comboType;
		m_pos = fruit.m_pos;
		Fruit obj = fruit;
		obj.m_fruitKilled = (Fruit.FruitEvent)Delegate.Combine(obj.m_fruitKilled, new Fruit.FruitEvent(FruitWasKilled));
		m_pos.Z = -4999f;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		if (controlThatMadeMe != null)
		{
			m_comboCount = 0;
			m_root = ((controlThatMadeMe.m_root != null) ? controlThatMadeMe.m_root : controlThatMadeMe);
			m_root.m_lastCreatedChild = this;
			m_root.m_comboCount++;
			if (m_root.m_comboCount >= 3 && m_comboType == 1)
			{
				MissControl free = MissControl.GetFree();
				free.MakeCombo(m_pos, m_root.m_comboCount);
				Game.AddToCurrentScore(m_root.m_comboCount, 0, addToTotal: false, sendNotification: false);
			}
			m_deleteCall = ADingoAteMyBaby;
		}
		else
		{
			m_comboCount = 1;
		}
		m_texture = Texture.Load("textureswp7/explosion_radius.tex");
	}

	~FruitSplosion()
	{
		if (fruit != null)
		{
			Fruit obj = fruit;
			obj.m_fruitKilled = (Fruit.FruitEvent)Delegate.Remove(obj.m_fruitKilled, new Fruit.FruitEvent(FruitWasKilled));
		}
	}

	private void ADingoAteMyBaby(HUDControl control)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (control == m_lastCreatedChild)
		{
			if (m_comboCount >= 3 && m_comboType == 2)
			{
				MissControl free = MissControl.GetFree();
				free.MakeCombo(m_pos, m_comboCount);
				Game.AddToCurrentScore(m_comboCount, 0, addToTotal: false, sendNotification: false);
			}
			m_terminate = true;
		}
	}

	private void FruitWasKilled(Fruit _fruit)
	{
		if (fruit == _fruit)
		{
			fruit = null;
		}
	}

	public override void Update(float dt)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (Game.game_work.pause)
		{
			return;
		}
		dt *= WaveManager.GetInstance().GetWavedt();
		time += dt;
		if (time > m_waitTime)
		{
			if (time >= m_fadeTime && m_lastCreatedChild == null)
			{
				m_terminate = true;
			}
			return;
		}
		LinkedListNode<Entity> iterator = null;
		Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator);
		float num = m_maxRadius * TransitionFunctions.SinTransition(TransitionFunctions.GetProgressBetween(time, 0f, m_growTime, clamp: true), 90f);
		while (fruit != null)
		{
			if (fruit != this.fruit && fruit.IsActive() && !fruit.Sliced())
			{
				Vector3 proj = fruit.m_pos - m_pos;
				proj.Z = 0f;
				if (proj.LengthSquared() < num * num)
				{
					proj.Normalize();
					proj *= 10f;
					controlThatMadeMe = this;
					fruit.CollisionResponse(this.fruit, 0u, 0u, ref proj);
					controlThatMadeMe = null;
				}
			}
			fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator);
		}
		m_scale = Vector3.One * num * 2f * (27f / 32f);
	}

	public override void DrawOrder(float[] tintChannels, int order)
	{
		base.DrawOrder(tintChannels, order);
	}
}
