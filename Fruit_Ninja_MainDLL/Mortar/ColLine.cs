using System;
using Microsoft.Xna.Framework;

namespace Mortar;

public class ColLine : Col
{
	public Vector3 Direction;

	public ColLine()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		centre = Vector3.Zero;
		Direction = new Vector3(1f, 0f, 0f);
		ClearCollideFlag();
	}

	public ColLine(Vector3 pos, Vector3 dir)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		centre = pos;
		Direction = dir;
		ClearCollideFlag();
	}

	public override COLISIONOBJECT GetType()
	{
		return COLISIONOBJECT.COL_LINE;
	}

	public override bool Collide(Col obj2, out Vector3 proj)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		proj = default(Vector3);
		switch (obj2.GetType())
		{
		case COLISIONOBJECT.COL_AABB:
			proj = -proj;
			if (flag)
			{
				AddCollision();
				obj2.AddCollision();
			}
			break;
		case COLISIONOBJECT.COL_SPHERE:
			flag = ColSphere.ColSphereLine((ColSphere)obj2, this, out proj);
			if (flag)
			{
				AddCollision();
				obj2.AddCollision();
			}
			break;
		case COLISIONOBJECT.COL_LINE:
			flag = ColLineLine(this, (ColLine)obj2, out proj);
			if (flag)
			{
				AddCollision();
				obj2.AddCollision();
			}
			break;
		default:
			flag = obj2.Collide(this, out proj);
			break;
		}
		return flag;
	}

	public override void DrawDebug()
	{
		throw new MissingMethodException();
	}

	public static bool ColLineLine(ColLine obj1, ColLine obj2, out Vector3 proj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		proj = default(Vector3);
		Vector3 val = obj1.Direction - obj1.centre;
		Vector3 val2 = obj2.Direction - obj2.centre;
		Vector3 val3 = obj1.centre - obj2.centre;
		float num = Vector3.Dot(val, val);
		float num2 = Vector3.Dot(val, val2);
		float num3 = Vector3.Dot(val2, val2);
		float num4 = Vector3.Dot(val, val3);
		float num5 = Vector3.Dot(val2, val3);
		float num6 = num * num3 - num2 * num2;
		float num7;
		float num8;
		if (num6 < 1E-06f)
		{
			num7 = 0f;
			num8 = ((num2 > num3) ? (num4 / num2) : (num5 / num3));
		}
		else
		{
			num7 = (num2 * num5 - num3 * num4) / num6;
			num8 = (num * num5 - num2 * num4) / num6;
		}
		Vector3 val4 = val3 + val * num7 - val2 * num8;
		if (val4.LengthSquared() < 1E-05f && num7 >= 0f && num7 <= 1f && num8 >= 0f && num8 <= 1f)
		{
			if (num7 >= 0.5f)
			{
				proj = -val * (1f - num7);
			}
			else
			{
				proj = val * num7;
			}
			return true;
		}
		return false;
	}
}
