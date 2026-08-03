using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ItemInfo
{
	public string name;

	public uint nameHash;

	public int cost;

	public ItemType type;

	public string shopTitle;

	public string shopDescription;

	public string unlockDescription;

	public string unlockTotal;

	public int unlockCountDownFrom;

	public string textureName;

	public Color colour;

	public Color titleColor;

	public bool hasBeenSeen;

	public bool IsLocked()
	{
		return cost > 0;
	}

	public ItemInfo()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		hasBeenSeen = true;
		name = null;
		shopTitle = null;
		shopDescription = null;
		textureName = null;
		unlockDescription = null;
		cost = 0;
		type = ItemType.ITEM_NONE;
		colour = Color.White;
		unlockTotal = null;
		unlockCountDownFrom = 0;
	}

	public virtual void SetEquipped()
	{
	}

	public virtual void Parse(XElement el)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		XElement xElement = el.FirstChildElement("requirements");
		if (xElement != null)
		{
			cost = 1;
			xElement.QueryIntAttribute("coins", ref cost);
			unlockDescription = xElement.AttributeStr("description");
			if (unlockDescription == null)
			{
				unlockDescription = xElement.Value;
			}
			xElement.QueryIntAttribute("countDownFrom", ref unlockCountDownFrom);
			unlockTotal = xElement.AttributeStr("total");
		}
		name = el.AttributeStr("name");
		nameHash = StringFunctions.StringHash(name);
		shopTitle = el.AttributeStr("title");
		xElement = el.FirstChildElement("description");
		if (xElement != null)
		{
			shopDescription = xElement.Value;
		}
		textureName = el.AttributeStr("texture");
		StringFunctions.ParseColour(ref colour, el.AttributeStr("colour"));
		titleColor = colour;
		StringFunctions.ParseColour(ref titleColor, el.AttributeStr("titleolour"));
	}
}
