using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class EffectImage
{
	public Texture texture;

	public HUDControl3d control;

	public bool added;

	public bool deferer;

	public Vector3 pos;

	public HUD.HUD_ORDER drawOrder;

	public ushort pulse;

	public float pulseSpeed;

	public float pulseScalePositive;

	public float pulseScaleNegative;

	public float transitionAmount;

	public float transitionTime;

	public Vector3 transitionMoveIn;

	public Vector3 transitionMoveOut;

	public float timeStart;

	public float timeEnd;

	public Vector3 scale;

	public Color colour;

	public uint transitionMask;

	private static uint[] hashes = new uint[2]
	{
		StringFunctions.StringHash("scale"),
		StringFunctions.StringHash("fade")
	};

	private static uint[] drawderhashes = new uint[5]
	{
		StringFunctions.StringHash("normal"),
		StringFunctions.StringHash("post"),
		StringFunctions.StringHash("before_splats"),
		StringFunctions.StringHash("after_splats"),
		StringFunctions.StringHash("after_bomb")
	};

	public EffectImage Duplicate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		EffectImage effectImage = new EffectImage();
		effectImage.texture = texture;
		effectImage.control = control;
		effectImage.added = added;
		effectImage.deferer = deferer;
		effectImage.pos = pos;
		effectImage.drawOrder = drawOrder;
		effectImage.pulse = pulse;
		effectImage.pulseSpeed = pulseSpeed;
		effectImage.pulseScalePositive = pulseScalePositive;
		effectImage.pulseScaleNegative = pulseScaleNegative;
		effectImage.transitionAmount = transitionAmount;
		effectImage.transitionTime = transitionTime;
		effectImage.transitionMoveIn = transitionMoveIn;
		effectImage.transitionMoveOut = transitionMoveOut;
		effectImage.timeStart = timeStart;
		effectImage.timeEnd = timeEnd;
		effectImage.scale = scale;
		effectImage.colour = colour;
		effectImage.transitionMask = transitionMask;
		return effectImage;
	}

	public EffectImage()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		deferer = false;
		drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
		pulse = 0;
		colour = Color.White;
		scale = Vector3.Zero;
		pulseScalePositive = 0f;
		pulseScaleNegative = 0f;
		pulseSpeed = 0f;
		timeStart = 1f;
		timeEnd = 0f;
		added = true;
		transitionAmount = 0f;
		transitionTime = 0f;
		transitionMoveOut = (transitionMoveIn = Vector3.Zero);
		pos = Vector3.Zero;
		texture = null;
		control = null;
	}

	public void Parse(XElement parent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		pos = Save.ParseVector(parent.AttributeStr("pos"));
		transitionMoveOut = (transitionMoveIn = Save.ParseVector(parent.AttributeStr("transitionMove")));
		Save.ParseVector(parent.AttributeStr("transitionMoveIn"), ref transitionMoveIn);
		Save.ParseVector(parent.AttributeStr("transitionMoveOut"), ref transitionMoveOut);
		string text = parent.AttributeStr("localise");
		bool flag = false;
		if (text == null)
		{
			texture = StringFunctions.LoadTexture(parent.AttributeStr("texture"));
		}
		else
		{
			string strA = parent.AttributeStr("texture") + ".tex";
			texture = TextureManager.GetInstance().Load(strA, localise: true);
			if (string.Compare(strA, "arcade_go.tex") == 0)
			{
				flag = true;
			}
			if (string.Compare(strA, "arcade_60seconds.tex") == 0)
			{
				flag = true;
			}
		}
		if (texture != null)
		{
			scale = new Vector3((float)texture.GetWidth(), (float)texture.GetHeight(), 0f);
			if (flag)
			{
				scale *= new Vector3(0.8f, 0.8f, 0.8f);
			}
		}
		Save.ParseVector(parent.AttributeStr("scale"), ref scale);
		scale *= Game.GAME_MODE_SCALE_FIX;
		float value = 1f;
		parent.QueryFloatAttribute("slowHardwareScale", ref value);
		scale *= value;
		parent.QueryFloatAttribute("pulseSpeed", ref pulseSpeed);
		parent.QueryFloatAttribute("pulseScale", ref pulseScalePositive);
		pulseScaleNegative = pulseScalePositive;
		parent.QueryFloatAttribute("pulseScalePositive", ref pulseScalePositive);
		parent.QueryFloatAttribute("pulseScaleNegative", ref pulseScaleNegative);
		parent.QueryFloatAttribute("transitionTime", ref transitionTime);
		parent.QueryFloatAttribute("timeStart", ref timeStart);
		parent.QueryFloatAttribute("timeEnd", ref timeEnd);
		StringFunctions.ParseColour(ref colour, parent.AttributeStr("colour"));
		transitionMask = StringFunctions.ParseMaskWords(parent.AttributeStr("transition"), hashes, hashes.Length);
		drawOrder = (HUD.HUD_ORDER)StringFunctions.FindIndex(parent.AttributeStr("drawOrder"), drawderhashes, drawderhashes.Length);
		deferer = StringFunctions.CompareWords(parent.AttributeStr("deferPoints"), "true");
	}
}
