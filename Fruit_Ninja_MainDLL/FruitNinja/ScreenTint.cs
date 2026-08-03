using System;
using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class ScreenTint
{
	public float transitionAmount;

	public float transitionTime;

	public float timeStart;

	public float timeEnd;

	public float[] backTint = new float[3];

	public float[] hudTint = new float[3];

	public ScreenTint Duplicate()
	{
		ScreenTint screenTint = new ScreenTint();
		Array.Copy(backTint, screenTint.backTint, backTint.Length);
		Array.Copy(hudTint, screenTint.hudTint, hudTint.Length);
		screenTint.transitionAmount = transitionAmount;
		screenTint.transitionTime = transitionTime;
		screenTint.timeStart = timeStart;
		screenTint.timeEnd = timeEnd;
		return screenTint;
	}

	public ScreenTint()
	{
		for (int i = 0; i < 3; i++)
		{
			backTint[i] = 0f;
			hudTint[i] = 0f;
		}
		transitionAmount = 0f;
		transitionTime = 0f;
		timeStart = 1f;
		timeEnd = 0f;
	}

	public void Parse(XElement parent)
	{
		parent.QueryFloatAttribute("timeStart", ref timeStart);
		parent.QueryFloatAttribute("timeEnd", ref timeEnd);
		StringFunctions.ParseFloats(parent.AttributeStr("tint"), backTint, 3);
		for (int i = 0; i < 3; i++)
		{
			hudTint[i] = backTint[i];
		}
		StringFunctions.ParseFloats(parent.AttributeStr("backTint"), backTint, 3);
		StringFunctions.ParseFloats(parent.AttributeStr("hudTint"), hudTint, 3);
		parent.QueryFloatAttribute("transitionTime", ref transitionTime);
	}
}
