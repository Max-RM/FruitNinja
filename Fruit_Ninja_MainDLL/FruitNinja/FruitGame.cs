namespace FruitNinja;

public class FruitGame
{
	public void Init(uint instance, string startUpCommandLine)
	{
		Game.GameInitialise(instance);
	}

	public void End()
	{
		Game.GameTaskExit();
		Game.GameDestroy();
	}

	public void Update(float timeSinceLastUpdate)
	{
		Game.GameTaskUpdate(timeSinceLastUpdate);
	}

	public void Draw(float timeSinceLastUpdate)
	{
		Game.GameTaskDraw(timeSinceLastUpdate);
	}

	public void Paused()
	{
		GameTask.SkipToPause(atStart: false);
	}

	public void UnPaused()
	{
		if (Game.game_work.gameOverTransition != 0f)
		{
			GameTask.UnpauseGame();
		}
	}

	public void LoadContent()
	{
	}

	public string SelfVersion()
	{
		return "1.21";
	}
}
