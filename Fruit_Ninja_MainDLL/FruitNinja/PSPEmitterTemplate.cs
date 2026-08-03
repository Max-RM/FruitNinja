using System.Collections.Generic;

namespace FruitNinja;

public class PSPEmitterTemplate
{
	public string name;

	public uint hash;

	public float life;

	public byte size_start;

	public byte size_end;

	public byte pad00;

	public byte particle_set_num;

	public List<PSPParticleSet> sets = new List<PSPParticleSet>();

	public bool Ends()
	{
		for (int i = 0; i < particle_set_num; i++)
		{
			if (sets[i].time_end <= 0f && sets[i].number_per_second > 0)
			{
				return false;
			}
		}
		return true;
	}
}
