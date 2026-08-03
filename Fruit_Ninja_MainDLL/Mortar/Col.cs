using Microsoft.Xna.Framework;

namespace Mortar;

public abstract class Col
{
	public Vector3 centre;

	public static Color normalColor;

	public static Color colideColor;

	protected bool m_collided;

	public new abstract COLISIONOBJECT GetType();

	public abstract bool Collide(Col obj2, out Vector3 proj);

	public abstract void DrawDebug();

	public void AddCollision()
	{
		m_collided = true;
	}

	public void ClearCollideFlag()
	{
		m_collided = false;
	}
}
