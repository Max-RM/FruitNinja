namespace FruitNinja;

internal class LeaderboardManager
{
	public enum LeaderboardType
	{
		LEADERBOARDTYPE_FRIENDS,
		LEADERBOARDTYPE_GLOBAL,
		LEADERBOARDTYPE_LOCAL,
		LEADERBOARDTYPE_WEEKLY,
		LEADERBOARDTYPE_MAX
	}

	protected static LeaderboardManager m_instance;

	private FNHighscoreList[,] m_leaderboards;

	private LeaderboardManager()
	{
		m_leaderboards = new FNHighscoreList[4, 4];
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				m_leaderboards[i, j] = null;
			}
		}
	}

	~LeaderboardManager()
	{
	}

	public static LeaderboardManager GetInstance()
	{
		if (m_instance == null)
		{
			m_instance = new LeaderboardManager();
		}
		return m_instance;
	}

	public void Init()
	{
	}

	public void Destroy()
	{
	}

	public FNHighscoreList GetLeaderboard(int mode, int type)
	{
		return m_leaderboards[mode, type];
	}

	public FNHighscoreList RefreshLeaderboard(int mode, int type)
	{
		return null;
	}

	public FNHighscoreList NextPage(int mode, int type)
	{
		return null;
	}

	public FNHighscoreList PreviousPage(int mode, int type)
	{
		return null;
	}

	public void ClearScores(int mode, int type)
	{
	}
}
