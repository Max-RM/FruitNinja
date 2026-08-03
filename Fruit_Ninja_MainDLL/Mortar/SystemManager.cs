namespace Mortar;

public class SystemManager
{
	private static SystemManager instance;

	public static SystemManager GetInstance()
	{
		if (instance == null)
		{
			instance = new SystemManager();
		}
		return instance;
	}

	public void Init()
	{
	}

	public bool Update(ref float dt)
	{
		return true;
	}
}
