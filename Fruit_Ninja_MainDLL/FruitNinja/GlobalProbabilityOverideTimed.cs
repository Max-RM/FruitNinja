using System.Xml.Linq;

namespace FruitNinja;

internal class GlobalProbabilityOverideTimed : GlobalProbabilityOveride
{
	public override void ParseSpecific(XElement element)
	{
	}

	public override bool CheckForOverride(ref int type)
	{
		if ((Game.GetModeBitMask(Game.game_work.gameMode) & modeMask) != modeMask)
		{
			return false;
		}
		int total = Game.game_work.saveData.GetTotal(totalNameHash);
		if (Game.game_work.gameTime > (float)total && total >= 0 && CanSpawn())
		{
			type = PickFruit();
			Game.game_work.saveData.AddToTotal(totalName, totalNameHash, -maxWait * 2, universal: false, achievementUpload: false);
			return true;
		}
		return false;
	}

	public override void PushbackSpawn()
	{
		Game.game_work.saveData.AddToTotal(totalName, totalNameHash, maxWait * 2, universal: false, achievementUpload: false);
	}

	public override void NewGameStarted()
	{
		Game.game_work.saveData.SetTotal(totalName, (int)Utils.GetRandBetween(minWait, maxWait), universal: false, achievementUpload: false);
	}
}
