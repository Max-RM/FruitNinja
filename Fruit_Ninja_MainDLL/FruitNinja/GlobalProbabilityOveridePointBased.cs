using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

internal class GlobalProbabilityOveridePointBased : GlobalProbabilityOveride
{
	private int everyMin;

	private int everyMax;

	private int fromMin;

	private int fromMax;

	public override bool CheckForOverride(ref int type)
	{
		if ((Game.GetModeBitMask(Game.game_work.gameMode) & modeMask) != modeMask)
		{
			return false;
		}
		int total = Game.game_work.saveData.GetTotal(totalNameHash);
		if (Game.GetCurrentScore() >= total && CanSpawn())
		{
			if (total < 0)
			{
				Game.game_work.saveData.SetTotal(totalName, -total, universal: false, achievementUpload: false);
			}
			type = PickFruit();
			Game.game_work.saveData.AddToTotal(totalName, totalNameHash, (int)Utils.GetRandBetween(everyMin, everyMax), universal: false, achievementUpload: false);
			return true;
		}
		return false;
	}

	public override void ParseSpecific(XElement element)
	{
		element.QueryIntAttribute("every", ref everyMin);
		everyMax = everyMin;
		element.QueryIntAttribute("from", ref fromMin);
		fromMax = fromMin;
		element.QueryIntAttribute("everyMin", ref everyMin);
		element.QueryIntAttribute("everyMax", ref everyMax);
		element.QueryIntAttribute("fromMin", ref fromMin);
		element.QueryIntAttribute("fromMax", ref fromMax);
	}

	public override void PushbackSpawn()
	{
		int total = Game.game_work.saveData.GetTotal(totalNameHash);
		if (total > 0)
		{
			Game.game_work.saveData.SetTotal(totalName, -total, universal: false, achievementUpload: false);
		}
	}

	public override void NewGameStarted()
	{
		Game.game_work.saveData.SetTotal(totalName, (int)Utils.GetRandBetween(fromMin, fromMax), universal: false, achievementUpload: false);
	}
}
