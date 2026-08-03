using Mortar;

namespace FruitNinja;

public class PToDraw
{
	public int depth;

	public int numPoints;

	public int offset;

	public Texture tex;

	public PToDraw(int points, int ofs, Texture t, int d)
	{
		numPoints = points;
		offset = ofs;
		tex = t;
		depth = d;
	}
}
