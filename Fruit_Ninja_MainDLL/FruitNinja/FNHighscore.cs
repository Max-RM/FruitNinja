namespace FruitNinja;

internal class FNHighscore
{
	private string user;

	private string displayName;

	private uint userHash;

	private uint rank;

	private int score;

	private FNHighscore()
	{
		user = null;
		displayName = null;
		userHash = 0u;
		score = 0;
		rank = 0u;
	}

	private FNHighscore(string name, uint hash, int a_score, int a_rank, string display)
	{
		user = name;
		displayName = display;
		userHash = hash;
		score = a_score;
		rank = (uint)a_rank;
	}

	private bool IsCurrentUser()
	{
		return false;
	}

	public static bool operator >(FNHighscore b1, FNHighscore b2)
	{
		return b2.score < b1.score;
	}

	public static bool operator <(FNHighscore b1, FNHighscore b2)
	{
		return b2.score > b1.score;
	}
}
