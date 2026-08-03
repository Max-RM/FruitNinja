namespace Mortar;

public class AnimationManager
{
	private static AnimationManager instance;

	public static AnimationManager GetInstance()
	{
		if (instance == null)
		{
			instance = new AnimationManager();
		}
		return instance;
	}

	public void Initialise()
	{
	}

	public void Initialise(int heapsize)
	{
	}
}
