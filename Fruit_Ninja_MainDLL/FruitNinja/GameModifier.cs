using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public abstract class GameModifier
{
	protected float m_length;

	protected int m_fruitLength;

	protected float m_currentTime;

	protected bool isOriginal;

	protected float m_waitUntilTime;

	protected bool m_isWaiting;

	public PowerUp m_parent;

	public void Duplicate(GameModifier dest)
	{
		dest.m_length = m_length;
		dest.m_fruitLength = m_fruitLength;
		dest.m_currentTime = m_currentTime;
		dest.isOriginal = isOriginal;
		dest.m_waitUntilTime = m_waitUntilTime;
		dest.m_isWaiting = m_isWaiting;
		dest.m_parent = m_parent;
	}

	public GameModifier()
	{
		isOriginal = false;
		m_length = 0f;
		m_currentTime = 0f;
		m_waitUntilTime = -1f;
		m_isWaiting = false;
		m_parent = null;
	}

	public void Reset()
	{
		m_currentTime = 0f;
		ResetSpecific();
	}

	public abstract void ResetSpecific();

	public virtual bool Update(float dt)
	{
		if (m_isWaiting)
		{
			if (!(Game.game_work.saveData.timer <= m_waitUntilTime))
			{
				return false;
			}
			ApplyModifier(fromSave: false, null);
			m_isWaiting = false;
		}
		if (m_currentTime > 0f)
		{
			m_currentTime -= dt;
			if (m_currentTime <= 0f)
			{
				return true;
			}
		}
		return UpdateSpecific(dt);
	}

	public abstract bool UpdateSpecific(float dt);

	public virtual void ApplyModifier(bool fromSave, float? length)
	{
		if (m_currentTime > 0f)
		{
			m_currentTime += m_length;
		}
		else
		{
			m_currentTime = m_length;
		}
		if (length.HasValue)
		{
			m_currentTime = length.Value;
		}
		uint hash = StringFunctions.StringHash("overtime");
		uint num = StringFunctions.StringHash("freeze");
		float num2 = Game.game_work.saveData.timer + ((PowerUpManager.GetInstance().GetActiveSingle(hash) == null) ? 0f : 5f) + (((m_parent != null && m_parent.GetHash() == num) || PowerUpManager.GetInstance().GetActiveSingle(num) != null) ? 50f : 0f);
		if (m_currentTime / PowerUpManager.GetInstance().PrevPowerupDtModMultiply() > num2)
		{
			m_currentTime = Math.MAX(num2 * PowerUpManager.GetInstance().PrevPowerupDtModMultiply() - 0.333f, 0.1f);
		}
	}

	public virtual void RemoveModifier()
	{
	}

	public new virtual int GetType()
	{
		return -1;
	}

	public void Parse(XElement parent)
	{
		isOriginal = true;
		parent.QueryFloatAttribute("length", ref m_length);
		parent.QueryFloatAttribute("waitUntilTime", ref m_waitUntilTime);
		if (m_waitUntilTime > -1f)
		{
			m_isWaiting = true;
		}
		ParseSpecific(parent);
	}

	public abstract void ParseSpecific(XElement parent);

	public abstract GameModifier Clone();

	public float GetCurrentTime()
	{
		return m_currentTime;
	}

	public float GetTotalTime()
	{
		return m_length;
	}

	public bool IsWaiting()
	{
		return m_isWaiting;
	}
}
