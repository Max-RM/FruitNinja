using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class SuperFruitHitControl : HUDControl
{
	public const float HIT_POP_SCALE_TIME = 0.2f;

	public const float HIT_TOTAL_TIME = 1f;

	public float m_time;

	public SuperFruitControl m_parent;

	public SuperFruitHitControl(int hits, Vector3 pos, SuperFruitControl parent)
	{
		m_time = 0f;
		m_parent = parent;
	}

	~SuperFruitHitControl()
	{
	}

	public override void Update(float dt)
	{
		if (!Game.game_work.pause)
		{
			m_time += dt;
		}
		if (m_time > 1f)
		{
			if (m_parent != null)
			{
				m_parent.m_lastCreatedHitControl = null;
			}
			m_terminate = true;
		}
	}

	public float GetScale()
	{
		if (m_time < 0.2f)
		{
			return TransitionFunctions.SinTransition(TransitionFunctions.GetProgressBetween(m_time, 0f, 0.2f, clamp: true), 115f);
		}
		if (m_time > 0.8f)
		{
			return TransitionFunctions.SinTransition(TransitionFunctions.GetProgressBetween(m_time, 1f, 0.8f, clamp: true), 115f);
		}
		return 1f;
	}

	public void RemoveQuickly()
	{
		m_time = Math.MAX(m_time, 0.8f);
	}

	public override void DrawOrder(float[] tintChannels, int order)
	{
	}
}
