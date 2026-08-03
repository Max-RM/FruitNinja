using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class TimeModifier : GameModifier
{
	protected float m_dt;

	protected float m_dtTransitionTime;

	protected float m_currentDt;

	protected bool m_stopClock;

	protected float m_clockSpeed;

	protected float m_addToClock;

	protected int m_addClockWait;

	public override void ResetSpecific()
	{
		m_currentDt = 0f;
	}

	public TimeModifier()
	{
		m_addToClock = 0f;
		m_addClockWait = 0;
		m_dt = 1f;
		m_stopClock = false;
		m_currentDt = 1f;
		m_dt = 1f;
		m_dtTransitionTime = 0f;
		m_clockSpeed = 1f;
	}

	public override bool UpdateSpecific(float dt)
	{
		if (m_addClockWait > 0)
		{
			m_addClockWait--;
			if (m_addClockWait <= 0)
			{
				Game.game_work.timeControl.AddTime(m_addToClock);
				return true;
			}
		}
		if (m_stopClock)
		{
			PowerUpManager.GetInstance().StopClock(m_currentTime);
		}
		if (m_clockSpeed != 1f)
		{
			PowerUpManager.GetInstance().SlowClock(m_clockSpeed);
		}
		if (m_dtTransitionTime > 0f)
		{
			if (m_currentTime <= m_dtTransitionTime && m_length > 0f)
			{
				if (m_dt > 1f)
				{
					m_currentDt = Math.MIN(m_currentDt, 1f + (m_dt - 1f) * (m_currentTime / m_dtTransitionTime));
				}
				else
				{
					m_currentDt = Math.MAX(m_currentDt, 1f + (m_dt - 1f) * (m_currentTime / m_dtTransitionTime));
				}
			}
			else if (m_currentDt < m_dt)
			{
				m_currentDt += dt / m_dtTransitionTime;
				if (m_currentDt > m_dt)
				{
					m_currentDt = m_dt;
				}
			}
			else if (m_currentDt > m_dt)
			{
				m_currentDt -= dt / m_dtTransitionTime;
				if (m_currentDt < m_dt)
				{
					m_currentDt = m_dt;
				}
			}
		}
		else
		{
			m_currentDt = m_dt;
		}
		PowerUpManager.GetInstance().ApplyDtMod(m_currentDt);
		return false;
	}

	public override int GetType()
	{
		return 0;
	}

	public override void ParseSpecific(XElement parent)
	{
		string text = parent.AttributeStr("stopClock");
		m_stopClock = StringFunctions.CompareWords(text, "true");
		parent.QueryFloatAttribute("slowClock", ref m_clockSpeed);
		parent.QueryFloatAttribute("addClock", ref m_addToClock);
		if (m_addToClock != 0f)
		{
			m_addClockWait = 1;
		}
		m_currentDt = 1f;
		m_dt = 1f;
		m_dtTransitionTime = 0f;
		XElement xElement = parent.FirstChildElement("dt_speed");
		if (xElement != null)
		{
			xElement.QueryFloatAttribute("transitionTime", ref m_dtTransitionTime);
			xElement.QueryFloatAttribute("dt", ref m_dt);
		}
	}

	private void Duplicate(TimeModifier dest)
	{
		Duplicate((GameModifier)dest);
		dest.m_dt = m_dt;
		dest.m_dtTransitionTime = m_dtTransitionTime;
		dest.m_currentDt = m_currentDt;
		dest.m_stopClock = m_stopClock;
		dest.m_clockSpeed = m_clockSpeed;
		dest.m_addToClock = m_addToClock;
		dest.m_addClockWait = m_addClockWait;
	}

	public override GameModifier Clone()
	{
		TimeModifier timeModifier = new TimeModifier();
		Duplicate(timeModifier);
		timeModifier.isOriginal = false;
		return timeModifier;
	}

	public bool DoesStopClock()
	{
		if (!m_stopClock && m_addClockWait == 0)
		{
			return m_clockSpeed == 0f;
		}
		return true;
	}
}
