using Microsoft.Xna.Framework;

namespace Mortar;

public struct ColRay(Vector3 v1, Vector3 v2)
{
	private Vector3 o = v1;

	private Vector3 d = v2;
}
