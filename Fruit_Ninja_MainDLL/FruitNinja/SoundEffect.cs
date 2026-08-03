using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class SoundEffect
{
	public string file;

	public float timeStart;

	public float timeEnd;

	public MortarSound sfx;

	public SoundEffect Duplicate()
	{
		SoundEffect soundEffect = new SoundEffect();
		soundEffect.file = file;
		soundEffect.timeStart = timeStart;
		soundEffect.timeEnd = timeEnd;
		soundEffect.sfx = sfx;
		return soundEffect;
	}

	public SoundEffect()
	{
		sfx = null;
		file = "";
		timeStart = 1f;
		timeEnd = -1f;
	}

	~SoundEffect()
	{
		if (sfx != null)
		{
			SoundManager.GetInstance().Release(sfx);
			Delete.SAFE_DELETE(ref sfx);
		}
	}

	public void Parse(XElement parent)
	{
		string text = parent.AttributeStr("name");
		if (text != null)
		{
			file = text;
		}
		parent.QueryFloatAttribute("timeStart", ref timeStart);
		parent.QueryFloatAttribute("timeEnd", ref timeEnd);
	}
}
