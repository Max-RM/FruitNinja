namespace FruitNinja;

public class SpecificOrder(string text)
{
	private uint[,] orderList = new uint[AchievementManager.MAX_ORDER_LIST_TYPES, AchievementManager.MAX_ORDER_LIST_TYPES + 1];

	public bool Check(uint hash)
	{
		return false;
	}

	public uint GetFirstFruitTypeHash()
	{
		return 0u;
	}
}
