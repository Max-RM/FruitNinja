namespace FruitNinja;

public class DEFAULT_WAVE_INFO
{
	public int waveChance;

	public float waveChanceRegrowth;

	public float criticalChance;

	public float dt;

	public float dtInc;

	public float dtSpInc;

	public float beforeDelay;

	public float beforeDelayInc;

	public float nextDelay;

	public float nextDelayInc;

	public float nextDelaySpInc;

	public bool waitForEntities;

	public float speedLoss;

	public int overideProbabilty;

	public int[] players = new int[Game.MAX_PLAYERS + 1];

	public void Reset()
	{
		speedLoss = 0f;
		players[0] = 0;
		players[1] = -1;
		waveChance = 10;
		waveChanceRegrowth = 0.25f;
		criticalChance = 1f;
		dt = 1f;
		dtInc = 0f;
		dtSpInc = 0f;
		nextDelay = 0f;
		nextDelayInc = 0f;
		nextDelaySpInc = 0f;
		beforeDelay = 2f;
		beforeDelayInc = 0f;
		overideProbabilty = 100;
		waitForEntities = true;
	}

	public DEFAULT_WAVE_INFO()
	{
		Reset();
	}
}
