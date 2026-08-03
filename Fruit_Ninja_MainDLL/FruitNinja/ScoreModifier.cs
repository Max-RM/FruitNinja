using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class ScoreModifier : GameModifier
{
	protected int m_gainAdd;

	protected int m_gainMultiply;

	protected int m_lossAdd;

	protected int m_lossMultiply;

	protected int m_count;

	protected bool m_deferPoints;

	protected int m_deferedPoints;

	private void Duplicate(ScoreModifier dest)
	{
		Duplicate((GameModifier)dest);
		dest.m_gainAdd = m_gainAdd;
		dest.m_gainMultiply = m_gainMultiply;
		dest.m_lossAdd = m_lossAdd;
		dest.m_lossMultiply = m_lossMultiply;
		dest.m_count = m_count;
		dest.m_deferPoints = m_deferPoints;
		dest.m_deferedPoints = m_deferedPoints;
	}

	public ScoreModifier()
	{
		m_gainAdd = 0;
		m_gainMultiply = 1;
		m_lossAdd = 0;
		m_lossMultiply = 1;
		m_count = 0;
		m_deferPoints = false;
		m_deferedPoints = 0;
	}

	private int AddScoreNomal(int score)
	{
		return score;
	}

	public override void ApplyModifier(bool fromSave, float? length)
	{
		base.ApplyModifier(fromSave, length);
		if (m_deferPoints)
		{
			m_parent.AddDeferedPoints(0);
			Game.SetScoreDelegate(DeferPoints);
		}
		m_count++;
	}

	public override void RemoveModifier()
	{
		if (m_deferPoints)
		{
			Game.SetScoreDelegate();
		}
	}

	public override bool UpdateSpecific(float dt)
	{
		if (!m_deferPoints)
		{
			PowerUpManager.GetInstance().AddToScoreGainAdd(m_gainAdd * m_count);
			PowerUpManager.GetInstance().AddToScoreLossAdd(m_lossAdd * m_count);
			for (int i = 0; i < m_count; i++)
			{
				PowerUpManager.GetInstance().AddToScoreGainMultiply(m_gainMultiply);
				PowerUpManager.GetInstance().AddToScoreLossMultiply(m_lossMultiply);
			}
		}
		return false;
	}

	public override void ResetSpecific()
	{
		m_gainAdd = 0;
		m_gainMultiply = 1;
		m_lossAdd = 0;
		m_lossMultiply = 1;
	}

	public override void ParseSpecific(XElement parent)
	{
		XElement xElement = parent.FirstChildElement("multiplier");
		ResetSpecific();
		if (xElement != null)
		{
			xElement.QueryIntAttribute("gainAdd", ref m_gainAdd);
			xElement.QueryIntAttribute("gainMultiply", ref m_gainMultiply);
			xElement.QueryIntAttribute("lossAdd", ref m_lossAdd);
			xElement.QueryIntAttribute("lossMultiply", ref m_lossMultiply);
			m_deferPoints = StringFunctions.CompareWords(xElement.AttributeStr("deferPoints"), "true");
		}
	}

	public override int GetType()
	{
		return 2;
	}

	public override GameModifier Clone()
	{
		ScoreModifier scoreModifier = new ScoreModifier();
		Duplicate(scoreModifier);
		scoreModifier.isOriginal = false;
		return scoreModifier;
	}

	public int DeferPoints(int points)
	{
		m_parent.AddDeferedPoints(points);
		m_deferedPoints += points;
		return 0;
	}

	public bool DoesDeferPoint()
	{
		return m_deferPoints;
	}
}
