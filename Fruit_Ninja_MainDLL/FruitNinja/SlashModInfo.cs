using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class SlashModInfo : ItemInfo
{
	private Color[] colours;

	private int numColors;

	private int slashType;

	private float speed;

	private string particles;

	private string slashTexture;

	public override void SetEquipped()
	{
		SlashEntity.SetModColors(colours, numColors, slashType, speed, particles, slashTexture);
	}

	public override void Parse(XElement el)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.Parse(el);
		numColors = 0;
		colours = null;
		slashType = 0;
		XElement slashModInfo = el.FirstChildElement("slashModInfo");
		ParseSlashModInfo(slashModInfo);
		if (colours == null)
		{
			numColors = 1;
			colours = new Color[numColors];
			ref Color reference = ref colours[0];
			reference = colour;
		}
	}

	public void ParseSlashModInfo(XElement slashModInfo)
	{
		if (slashModInfo == null)
		{
			return;
		}
		slashModInfo.QueryFloatAttribute("speed", ref speed);
		slashType = SlashEntity.ParseSlashModColorType(slashModInfo.AttributeStr("type"));
		particles = slashModInfo.AttributeStr("particles");
		string text = slashModInfo.AttributeStr("texture");
		if (text != null)
		{
			slashTexture = $"textureswp7/{text}.tex";
		}
		for (XElement xElement = slashModInfo.FirstChildElement("colour"); xElement != null; xElement = xElement.NextSiblingElement("colour"))
		{
			numColors++;
		}
		if (numColors > 0)
		{
			colours = new Color[numColors];
			int num = 0;
			for (XElement xElement2 = slashModInfo.FirstChildElement("colour"); xElement2 != null; xElement2 = xElement2.NextSiblingElement("colour"))
			{
				StringFunctions.ParseColour(ref colours[num], xElement2.Value);
				num++;
			}
		}
	}

	public SlashModInfo()
	{
		colours = null;
		numColors = 0;
		slashType = 0;
		speed = 1f;
		particles = null;
		slashTexture = null;
	}
}
