namespace Mortar;

public class NetworkManager
{
	private static NetworkManager instance;

	public static NetworkManager GetInstance()
	{
		if (instance == null)
		{
			instance = new NetworkManager();
		}
		return instance;
	}

	public bool UserHasEnabledNetwork()
	{
		return true;
	}

	public bool UnlockAchievement(string id)
	{
		return true;
	}

	public bool IsOnline()
	{
		return false;
	}
}
