namespace FruitNinja;

internal class FNHighscoreList
{
	private bool m_reorderRanks;

	private bool m_allowDuplicateUsers;

	private bool AddScore(string user, int score, int rank)
	{
		return false;
	}

	public void ClearScores(int mode, int type)
	{
	}

	public bool GetHighscoreForUser(string user, FNHighscore userScore, FNHighscore before, FNHighscore after)
	{
		return false;
	}

	public void PrepareForDataRetrieval()
	{
	}

	public void SetReorderRanks(bool reorderRanks)
	{
		m_reorderRanks = reorderRanks;
	}

	public bool GetReorderRanks()
	{
		return m_reorderRanks;
	}

	public void SetAllowDuplicateUsers(bool allowDuplicates)
	{
		m_allowDuplicateUsers = allowDuplicates;
	}

	public bool AllowDuplicateUsers()
	{
		return m_allowDuplicateUsers;
	}
}
