namespace Mortar;

public struct MortarRectangle
{
	public int left;

	public int top;

	public int right;

	public int bottom;

	public int Width()
	{
		return right - left;
	}

	public int Height()
	{
		return bottom - top;
	}

	public Point Centre()
	{
		return new Point(left + (Width() >> 1), top + (Height() >> 1));
	}
}
