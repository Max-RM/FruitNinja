using Mortar;

namespace FruitNinja;

public class MetricData
{
	public int highScore;

	public int totalTotals;

	public SliceTotal[] totals = ArrayInit.CreateFilledArray<SliceTotal>(5);

	public int AchievementsUnlocked;

	public uint[] Achievements = new uint[20];
}
