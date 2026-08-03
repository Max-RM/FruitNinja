using System.Collections.Generic;

namespace FruitNinja;

public struct WaveState
{
	public LinkedList<SpawnState> spawners;

	public float inc;

	public int index;
}
