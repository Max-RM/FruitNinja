using System.Collections.Generic;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class WaveModifier : GameModifier
{
	protected List<PROBABILITY_OVERIDE> m_overides = new List<PROBABILITY_OVERIDE>();

	protected float m_bombMultiplyer;

	protected float m_bombScale;

	protected float m_fruitMultiplyer;

	protected float m_powerUpDtMod;

	protected int m_overideWave;

	protected float m_criticalChanceMod;

	public static int MAGIC_MAXIE => 10000;

	public WaveModifier()
	{
		m_fruitMultiplyer = 1f;
		m_bombMultiplyer = 1f;
		m_powerUpDtMod = 1f;
		m_bombScale = 1f;
		m_criticalChanceMod = 1f;
		m_overideWave = MAGIC_MAXIE;
	}

	public override bool UpdateSpecific(float dt)
	{
		WaveManager.GetInstance().FruitMultiplyer(m_fruitMultiplyer);
		WaveManager.GetInstance().BombMultiplyer(m_bombMultiplyer);
		WaveManager.GetInstance().BombScale(m_bombScale);
		WaveManager.GetInstance().CriticalChanceMod(m_criticalChanceMod);
		PowerUpManager.GetInstance().PowerupDtModMultiply(m_powerUpDtMod);
		return false;
	}

	public override void ResetSpecific()
	{
	}

	public override void ParseSpecific(XElement parent)
	{
		parent.QueryFloatAttribute("fruitMultiplyer", ref m_fruitMultiplyer);
		parent.QueryFloatAttribute("bombMultiplyer", ref m_bombMultiplyer);
		parent.QueryFloatAttribute("bombScale", ref m_bombScale);
		parent.QueryFloatAttribute("criticalChance", ref m_criticalChanceMod);
		parent.QueryFloatAttribute("powerUpDtMod", ref m_powerUpDtMod);
		parent.QueryIntAttribute("waveOveride", ref m_overideWave);
		for (XElement xElement = parent.FirstChildElement("OverideProbability"); xElement != null; xElement = parent.NextSiblingElement("OverideProbability"))
		{
			PROBABILITY_OVERIDE pROBABILITY_OVERIDE = new PROBABILITY_OVERIDE();
			pROBABILITY_OVERIDE.numWaves = 0;
			pROBABILITY_OVERIDE.Parse(xElement);
			m_overides.Add(pROBABILITY_OVERIDE);
		}
	}

	public override int GetType()
	{
		return 3;
	}

	private void Duplicate(WaveModifier dest)
	{
		Duplicate((GameModifier)dest);
		dest.m_overides = m_overides;
		dest.m_bombMultiplyer = m_bombMultiplyer;
		dest.m_bombScale = m_bombScale;
		dest.m_fruitMultiplyer = m_fruitMultiplyer;
		dest.m_powerUpDtMod = m_powerUpDtMod;
		dest.m_overideWave = m_overideWave;
		dest.m_criticalChanceMod = m_criticalChanceMod;
	}

	public override GameModifier Clone()
	{
		WaveModifier waveModifier = new WaveModifier();
		Duplicate(waveModifier);
		waveModifier.isOriginal = false;
		return waveModifier;
	}

	public override void ApplyModifier(bool fromSave, float? length)
	{
		base.ApplyModifier(fromSave, length);
		if (m_overideWave < MAGIC_MAXIE && !fromSave && WaveManager.GetInstance().WaveCount() > m_overideWave)
		{
			WaveManager.GetInstance().SetCurrentWave(m_overideWave, -1f);
		}
		List<PROBABILITY_OVERIDE> currentOverideList = WaveManager.GetInstance().GetCurrentOverideList();
		foreach (PROBABILITY_OVERIDE item in currentOverideList)
		{
			item.SelectType();
		}
		currentOverideList.InsertRange(0, m_overides);
		m_overides.Clear();
	}

	public override void RemoveModifier()
	{
		if (WaveManager.GetInstance().WaveCount() < 0 && WaveManager.GetInstance().WaveCount() >= m_overideWave)
		{
			WaveManager.GetInstance().SetCurrentWave(5, 1f);
		}
	}
}
