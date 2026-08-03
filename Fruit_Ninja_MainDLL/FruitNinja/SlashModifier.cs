using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class SlashModifier : GameModifier
{
	protected Color[] colours;

	protected int numColours;

	protected int slashType;

	protected float speed;

	protected string particles;

	protected string slashTexture;

	protected uint m_activePowersMask;

	protected bool m_hasBeenApplied;

	private static int referenced_slashMods;

	public SlashModifier()
	{
		m_activePowersMask = 0u;
		colours = null;
		numColours = 0;
		slashType = 0;
		speed = 1f;
		particles = null;
		slashTexture = null;
		m_hasBeenApplied = false;
	}

	public override void ResetSpecific()
	{
	}

	public override bool UpdateSpecific(float dt)
	{
		SlashEntity.ModPowerMask |= m_activePowersMask;
		return false;
	}

	public override void ApplyModifier(bool fromSave, float? length)
	{
		base.ApplyModifier(fromSave, length);
		if (colours != null && !m_hasBeenApplied)
		{
			m_hasBeenApplied = true;
			referenced_slashMods++;
			SlashEntity.SetModColors(colours, numColours, slashType, speed, particles, slashTexture);
		}
	}

	public override void RemoveModifier()
	{
		if (m_hasBeenApplied)
		{
			referenced_slashMods--;
			if (referenced_slashMods <= 0)
			{
				ItemManager.GetInstance().SetEquippedItem(ItemType.ITEM_SLASH_MODIFIER, ItemManager.GetInstance().GetEquippedItem(ItemType.ITEM_SLASH_MODIFIER));
			}
		}
	}

	public override int GetType()
	{
		return 1;
	}

	public override void ParseSpecific(XElement parent)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (parent == null)
		{
			return;
		}
		parent.QueryFloatAttribute("speed", ref speed);
		slashType = SlashEntity.ParseSlashModColorType(parent.AttributeStr("type"));
		particles = parent.AttributeStr("particles");
		string text = parent.AttributeStr("texture");
		if (text != null)
		{
			slashTexture = $"textureswp7/{text}.tex";
		}
		for (XElement xElement = parent.FirstChildElement("slash_power"); xElement != null; xElement = xElement.NextSiblingElement("slash_power"))
		{
			m_activePowersMask |= SlashEntity.ParseSlashPowerMask(xElement.AttributeStr("type"));
		}
		for (XElement xElement2 = parent.FirstChildElement("colour"); xElement2 != null; xElement2 = xElement2.NextSiblingElement("colour"))
		{
			numColours++;
		}
		if (numColours > 0)
		{
			colours = new Color[numColours];
			int num = 0;
			for (XElement xElement3 = parent.FirstChildElement("colour"); xElement3 != null; xElement3 = xElement3.NextSiblingElement("colour"))
			{
				StringFunctions.ParseColour(ref colours[num], xElement3.GetText());
				num++;
			}
		}
		else if (slashTexture != null || particles != null)
		{
			numColours = 1;
			colours = new Color[1];
			ref Color reference = ref colours[0];
			reference = Color.White;
		}
	}

	private void Duplicate(SlashModifier dest)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Duplicate((GameModifier)dest);
		dest.colours = new Color[colours.Length];
		for (int i = 0; i < colours.Length; i++)
		{
			ref Color reference = ref dest.colours[i];
			reference = colours[i];
		}
		dest.numColours = numColours;
		dest.slashType = slashType;
		dest.speed = speed;
		dest.particles = particles;
		dest.slashTexture = slashTexture;
		dest.m_activePowersMask = m_activePowersMask;
		dest.m_hasBeenApplied = m_hasBeenApplied;
	}

	public override GameModifier Clone()
	{
		SlashModifier slashModifier = new SlashModifier();
		slashModifier.isOriginal = false;
		return slashModifier;
	}
}
