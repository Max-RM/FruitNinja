using Microsoft.Xna.Framework;

namespace FruitNinja;

public class PSPParticleSet
{
	public PSPParticleTemplate template_idx;

	public float time_start;

	public float time_end;

	public byte number_start;

	public byte number_per_second;

	public ushort pad00;

	public Vector3 vel_min;

	public Vector3 vel_max;
}
