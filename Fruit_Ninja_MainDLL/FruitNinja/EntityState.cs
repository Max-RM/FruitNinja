using Microsoft.Xna.Framework;

namespace FruitNinja;

public struct EntityState
{
	public Vector3 vel;

	public Vector3 pos;

	public Vector3 grav;

	public bool hit;

	public int type;

	public float wait;

	public SuperFruitState superState;
}
