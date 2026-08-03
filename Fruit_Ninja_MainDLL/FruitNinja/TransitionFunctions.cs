using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class TransitionFunctions
{
	public static float SinTransition(float amt, float full)
	{
		return Math.SinIdx((ushort)(amt * (float)(int)Math.DEGREE_TO_IDX(full))) / Math.SinIdx(Math.DEGREE_TO_IDX(full));
	}

	public static float SquareTransition(float amt, float parameter)
	{
		return amt * amt;
	}

	public static float InverseSquareTransition(float amt)
	{
		return InverseSquareTransition(amt, 0f);
	}

	public static float InverseSquareTransition(float amt, float full)
	{
		amt = 1f - amt;
		amt *= amt;
		return 1f - amt;
	}

	public static float FullTransition(float amt, float parameter)
	{
		return 1f;
	}

	public static float StraightTransition(float amt, float parameter)
	{
		return Math.CLAMP(amt * parameter, 0f, 1f);
	}

	public static float SinPulse(float amt, float parameter)
	{
		return Math.SinIdx((ushort)(amt * 32768f * parameter));
	}

	public static float InAndOut(float amt, float parameter)
	{
		if (amt < parameter)
		{
			return amt / parameter;
		}
		if (amt > 1f - parameter)
		{
			amt = 1f - amt;
			return amt / parameter;
		}
		return 1f;
	}

	public static float SinInAndOut(float amt, float parameter)
	{
		return SinTransition(InAndOut(amt, parameter), 115f);
	}

	public static float JumpyPulse(float amt, float parameter)
	{
		if (amt <= parameter)
		{
			return Math.SinIdx((ushort)(amt / parameter * 32768f));
		}
		return (0f - Math.SinIdx((ushort)((amt - parameter) / (1f - parameter) * 32768f))) * 0.2f;
	}

	public static float JumpySinPulse(float amt, float parameter)
	{
		float num = parameter * 0.5f * parameter * ((amt + 1f) * (amt + 1f) * (amt + 1f) - 1f) / 7f;
		return Math.SinIdx((ushort)(num * 32768f)) * (1f - amt);
	}

	public static float GetProgressBetween(float time, float start, float end)
	{
		return GetProgressBetween(time, start, end, clamp: false);
	}

	public static float GetProgressBetween(float time, float start, float end, bool clamp)
	{
		if (start == end)
		{
			if (time >= start)
			{
				return 1f;
			}
			return 0f;
		}
		if (!clamp)
		{
			return (time - start) / (end - time);
		}
		return Math.CLAMP((time - start) / (end - start), 0f, 1f);
	}

	public static Vector3 LerpF(Vector3 start, Vector3 end, float amt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return start + (end - start) * amt;
	}

	public static float LerpF(float start, float end, float amt)
	{
		return start + (end - start) * amt;
	}
}
