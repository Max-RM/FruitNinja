using Microsoft.Xna.Framework;

namespace Mortar;

public class EntityChunk
{
	public int size;

	public string name;

	public uint name_hash;

	public string type;

	public uint type_hash;

	public Vector3 scale;

	public Vector3 rotate;

	public Vector3 translate;

	public Vector3 volume_min;

	public Vector3 volume_max;
}
