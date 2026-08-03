using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class Utils
{
	public static float GetRandBetween(float start, float end)
	{
		return GetRandBetween(start, end, 0f);
	}

	public static float GetRandBetween(float start, float end, float negatePercentage)
	{
		float num = start;
		if (start != end)
		{
			num += (end - start) * Mortar.Math.g_random.RandF(1f);
		}
		if (negatePercentage > 0f && Mortar.Math.g_random.RandF(1f) <= negatePercentage)
		{
			num = 0f - num;
		}
		return num;
	}

	public static Vector3 VectorFromMagAngle(float magnitude, ushort angle)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mortar.Math.CosIdx(angle) * magnitude, Mortar.Math.SinIdx(angle) * magnitude, 0f);
	}

	public static Vector3 VectorFromMagDegree(float magnitude, float angle)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mortar.Math.CosIdx(Mortar.Math.DEGREE_TO_IDX(angle)) * magnitude, Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(angle)) * magnitude, 0f);
	}

	public static Color LerpColourFromArray(float t, Color[] colours, int count)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (t >= 1f)
		{
			return colours[count - 1];
		}
		if (t <= 0f || count == 1)
		{
			return colours[0];
		}
		int num = (int)(t * (float)(count - 1));
		float num2 = (float)System.Math.IEEERemainder(t * (float)(count - 1), 1.0);
		return Color.Lerp(colours[num + 1], colours[num], num2);
	}
}
