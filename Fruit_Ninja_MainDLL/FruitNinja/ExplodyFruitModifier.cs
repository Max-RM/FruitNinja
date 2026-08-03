using System;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

internal class ExplodyFruitModifier : GameModifier
{
	public enum ComboType
	{
		COMBOTYPE_NONE,
		COMBOTYPE_CUMULATIVE,
		COMBOTYPE_TOTAL
	}

	private float m_radius;

	private float m_growTime;

	private float m_waitTime;

	private float m_fadeTime;

	private int m_comboType;

	private static uint[] hashes = new uint[3]
	{
		StringFunctions.StringHash("none"),
		StringFunctions.StringHash("cumulative"),
		StringFunctions.StringHash("total")
	};

	public ExplodyFruitModifier()
	{
		m_radius = 100f;
		m_waitTime = 0f;
		m_growTime = 0.25f;
		m_fadeTime = 0.2f;
		m_comboType = 0;
	}

	public override void ParseSpecific(XElement parent)
	{
		parent.QueryFloatAttribute("radius", ref m_radius);
		parent.QueryFloatAttribute("growTime", ref m_growTime);
		parent.QueryFloatAttribute("waitTime", ref m_waitTime);
		parent.QueryFloatAttribute("fadeTime", ref m_fadeTime);
		m_comboType = StringFunctions.FindIndex(parent.Attribute("comboType").Value, hashes, 3);
		m_waitTime += m_growTime;
		m_fadeTime += m_waitTime;
	}

	public override GameModifier Clone()
	{
		ExplodyFruitModifier explodyFruitModifier = new ExplodyFruitModifier();
		return this;
	}

	public override void ApplyModifier(bool fromSave, float? length)
	{
		if (m_currentTime <= 0f)
		{
			Fruit.FruitSliced = (Fruit.FruitSliceEvent)Delegate.Combine(Fruit.FruitSliced, new Fruit.FruitSliceEvent(FruitWasSliced));
		}
		base.ApplyModifier(fromSave, length);
	}

	public override void RemoveModifier()
	{
		Fruit.FruitSliced = (Fruit.FruitSliceEvent)Delegate.Remove(Fruit.FruitSliced, new Fruit.FruitSliceEvent(FruitWasSliced));
	}

	public override bool UpdateSpecific(float dt)
	{
		return false;
	}

	public override void ResetSpecific()
	{
	}

	private void FruitWasSliced(Fruit fruit, int score, Entity otherEnt)
	{
		if (!Fruit.FruitInfo(fruit.GetFruitType()).superFruit)
		{
			FruitSplosion control = new FruitSplosion(fruit, m_radius, m_growTime, m_waitTime, m_fadeTime, m_comboType);
			Game.game_work.hud.AddControl(control);
		}
	}

	public override int GetType()
	{
		return 5;
	}
}
