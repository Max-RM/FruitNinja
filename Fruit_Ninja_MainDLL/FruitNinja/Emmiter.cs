using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class Emmiter
{
	public uint hash;

	public PSPParticleEmitter emmiter;

	public Vector3 pos;

	public Emmiter Duplicate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Emmiter emmiter = new Emmiter();
		emmiter.hash = hash;
		emmiter.emmiter = this.emmiter;
		emmiter.pos = pos;
		return emmiter;
	}

	public Emmiter()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		pos = Vector3.Zero;
		hash = 0u;
		emmiter = null;
	}

	public void Parse(XElement parent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		pos = Save.ParseVector(parent.AttributeStr("pos"));
		if (parent.Attribute("particle") != null)
		{
			hash = StringFunctions.StringHash(parent.AttributeStr("particle"));
		}
	}
}
