using Microsoft.Xna.Framework;

namespace Mortar;

public static class Fps
{
	private const float OneSecond = 1000f;

	private static long time;

	private static int ticks;

	private static int rate;

	private static int min;

	private static int max;

	public static int FrameRate => rate;

	public static int FrameRateMin => min;

	public static int FrameRateMax => max;

	static Fps()
	{
		Reset();
	}

	public static void Reset()
	{
		time = 0L;
		ticks = 0;
		rate = 0;
		min = 1000000;
		max = -1000000;
	}

	public static void Update(GameTime gameTime)
	{
		time += gameTime.ElapsedGameTime.Ticks;
		if (time > 10000000)
		{
			rate = ticks;
			time = 0L;
			ticks = 0;
			if (rate < min)
			{
				min = rate;
			}
			if (rate > max)
			{
				max = rate;
			}
		}
		else
		{
			ticks++;
		}
	}
}
