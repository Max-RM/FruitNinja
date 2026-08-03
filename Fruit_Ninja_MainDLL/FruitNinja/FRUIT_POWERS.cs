using Mortar;

namespace FruitNinja;

public class FRUIT_POWERS
{
	public FRUIT_POWER[] powerUps;

	public int numPowerUpTypes;

	public uint RandomPower()
	{
		int num = Math.g_random.Rand32(powerUps[numPowerUpTypes - 1].totalChance);
		for (int i = 0; i < numPowerUpTypes; i++)
		{
			if (num < powerUps[i].totalChance)
			{
				return powerUps[i].powerHash;
			}
		}
		return powerUps[0].powerHash;
	}

	public bool AnyActivePowers()
	{
		for (int i = 0; i < numPowerUpTypes; i++)
		{
			if (PowerUpManager.GetInstance().GetActiveSingle(powerUps[i].powerHash) != null)
			{
				return true;
			}
		}
		return false;
	}

	public FRUIT_POWERS()
	{
		numPowerUpTypes = 0;
	}
}
