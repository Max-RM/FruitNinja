using Mortar;

namespace FruitNinja;

public class AchievementInfo
{
	public string name;

	public string id;

	public uint idHash;

	public Texture texture;

	public string description;

	public int total;

	public int score;

	public bool isGameOver;

	public AchievementUnlockType type;

	public uint modeMask;

	public SpecificOrder specificOrder;

	public AchievementInfo()
	{
		texture = null;
		specificOrder = null;
		total = 0;
		score = 0;
		type = AchievementUnlockType.UNLOCK_TYPE_MAX;
	}
}
