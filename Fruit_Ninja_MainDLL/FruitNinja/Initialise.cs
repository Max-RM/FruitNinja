using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class Initialise
{
	private static string[] modeNames = new string[5] { "CLASSIC", "CASINO", "ARCADE", "ZEN", "ALL" };

	private static uint[] names = new uint[4]
	{
		StringFunctions.StringHash("CLASSIC"),
		StringFunctions.StringHash("CASINO"),
		StringFunctions.StringHash("ARCADE"),
		StringFunctions.StringHash("ZEN")
	};

	public static Vector3 ScreenPos(float x, float y, Vector3 anchor)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return ScreenPos(x, y, anchor, splitscreen: false);
	}

	public static Vector3 ScreenPos(float x, float y, Vector3 anchor, bool splitscreen)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(x, y, 0f) + new Vector3(Game.SCREEN_WIDTH, Game.SCREEN_HEIGHT, 0f) * anchor;
	}

	public static Vector3 ScreenPos(Vector3 pos, Vector3 anchor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ScreenPos(pos, anchor, splitscreen: false);
	}

	public static Vector3 ScreenPos(Vector3 pos, Vector3 anchor, bool splitscreen)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return pos + new Vector3(Game.SCREEN_WIDTH, Game.SCREEN_HEIGHT, 0f) * anchor;
	}

	public static string GetModeName(Game.GAME_MODE mode)
	{
		return modeNames[(int)mode];
	}

	public static Game.GAME_MODE ParseGameMode(uint hash)
	{
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i] == hash)
			{
				return (Game.GAME_MODE)i;
			}
		}
		return Game.GAME_MODE.GM_MAX;
	}

	public static uint GetModeBitMask(Game.GAME_MODE mode)
	{
		if (mode == Game.GAME_MODE.GM_MAX)
		{
			return uint.MaxValue;
		}
		return (uint)(1 << (int)mode);
	}
}
