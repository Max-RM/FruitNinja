using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class BonusAwardHud
{
	public string text;

	public int points;

	public int multiplyer;

	public int count;

	public int visiblePoints;

	public Color colour;

	public float numberScale;

	public Color numberColour;

	public Texture texture;

	public BonusAwardHud()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		points = 0;
		multiplyer = 1;
		text = "";
		colour = Color.White;
		visiblePoints = 0;
		texture = null;
	}
}
