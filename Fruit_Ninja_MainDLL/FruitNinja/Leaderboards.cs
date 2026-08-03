using System;

namespace FruitNinja;

internal class Leaderboards
{
	public delegate void ReadFinishedEventHandler(int result);

	private enum ReadMode
	{
		None,
		Reading
	}

	private const int ENTRIES_TO_READ = 25;

	public const int LEADERBOARD_TYPE_CLASSIC = 0;

	public const int LEADERBOARD_TYPE_ZEN = 1;

	public const int LEADERBOARD_TYPE_ARCADE = 2;

	public const int LEADERBOARD_TYPE_TOTAL_SCORES = 3;

	public const int LEADERBOARD_TYPE_TOTAL_FRUIT = 4;

	public const int LEADERBOARD_RESULT_NO_PLAYER = 0;

	public const int LEADERBOARD_RESULT_TIMED_OUT = 1;

	public const int LEADERBOARD_RESULT_SUCCESS = 2;

	private static int gameMode;

	private static ReadFinishedEventHandler notifier;

	private static ReadMode mode;

	static Leaderboards()
	{
		mode = ReadMode.None;
	}

	public static bool StartRead(int leaderboard, ReadFinishedEventHandler callback)
	{
		return false;
	}

	public static void Write(int mode, long value)
	{
	}

	private static void LeaderboardReadCallback(IAsyncResult result)
	{
	}

	public static void PageUp()
	{
	}

	public static void PageDown()
	{
	}
}
