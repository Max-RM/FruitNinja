using System;
using Microsoft.Xna.Framework;

namespace Mortar;

public class ColAABB : Col
{
	public Vector3 extents;

	public ColAABB()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		centre = Vector3.Zero;
		extents = Vector3.One;
		ClearCollideFlag();
	}

	public ColAABB(Vector3 pos, Vector3 ext)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		centre = pos;
		extents = ext;
		ClearCollideFlag();
	}

	public override COLISIONOBJECT GetType()
	{
		return COLISIONOBJECT.COL_AABB;
	}

	public override bool Collide(Col obj2, out Vector3 proj)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		switch (obj2.GetType())
		{
		case COLISIONOBJECT.COL_AABB:
			proj = Vector3.Zero;
			throw new MissingMethodException();
		case COLISIONOBJECT.COL_SPHERE:
			proj = Vector3.Zero;
			throw new MissingMethodException();
		case COLISIONOBJECT.COL_LINE:
			proj = Vector3.Zero;
			break;
		default:
			result = obj2.Collide(this, out proj);
			break;
		}
		return result;
	}

	public override void DrawDebug()
	{
		throw new MissingMethodException();
	}
}
