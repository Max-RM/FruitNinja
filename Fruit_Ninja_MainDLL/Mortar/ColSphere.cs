using System;
using Microsoft.Xna.Framework;

namespace Mortar;

public class ColSphere : Col
{
	public float Radius;

	public ColSphere()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		centre = Vector3.Zero;
		Radius = 1f;
		ClearCollideFlag();
	}

	public ColSphere(Vector3 pos, float rad)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		centre = pos;
		Radius = rad;
		ClearCollideFlag();
	}

	public override COLISIONOBJECT GetType()
	{
		return COLISIONOBJECT.COL_SPHERE;
	}

	public override bool Collide(Col obj2, out Vector3 proj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		proj = default(Vector3);
		return false;
	}

	public override void DrawDebug()
	{
		throw new MissingMethodException();
	}

	public static bool ColSphereSphere(ColSphere obj1, ColSphere obj2, out Vector3 proj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		proj = default(Vector3);
		return false;
	}

	public static bool ColSphereLine(ColSphere obj1, ColLine obj2, out Vector3 proj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		proj = default(Vector3);
		Vector3 result = Vector3.Zero;
		Math.ClosestPointOnLine(ref obj2.centre, ref obj2.Direction, ref obj1.centre, ref result);
		Vector3 val = result - obj1.centre;
		if (val.LengthSquared() < obj1.Radius * obj1.Radius)
		{
			float num = val.Length();
			val.Normalize();
			val = (proj = val * Math.ABS(obj1.Radius - num));
			return true;
		}
		return false;
	}
}
