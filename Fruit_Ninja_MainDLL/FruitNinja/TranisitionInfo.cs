using Mortar;

namespace FruitNinja;

internal class TranisitionInfo
{
	public delegate float transitionFunc(float amt, float parameter);

	private transitionFunc func;

	private float paramter;

	private float start;

	private float end;

	public float empty;

	public float full;

	public TranisitionInfo(transitionFunc f, float p)
	{
		func = f;
		paramter = p;
		start = 0f;
		end = 1f;
		empty = 0f;
		full = 1f;
	}

	public TranisitionInfo()
	{
		func = null;
		paramter = 0f;
		start = 0f;
		end = 1f;
		empty = 0f;
		full = 1f;
	}

	public float GetAmt(float amt)
	{
		amt = Math.CLAMP((amt - start) / (end - start), 0f, 1f);
		return empty + ((func != null) ? func(amt, paramter) : amt) * (full - empty);
	}
}
