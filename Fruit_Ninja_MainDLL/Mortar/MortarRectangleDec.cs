namespace Mortar;

public struct MortarRectangleDec
{
	public float left;

	public float top;

	public float right;

	public float bottom;

	public float Width()
	{
		return right - left;
	}

	public float Height()
	{
		return bottom - top;
	}

	public PointDec Centre()
	{
		return new PointDec(left + Width() * 0.5f, top + Height() * 0.5f);
	}

	public MortarRectangleDec(float l, float t, float r, float b)
	{
		left = l;
		top = t;
		right = r;
		bottom = b;
	}
}
