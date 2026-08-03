using System;

namespace FruitNinja;

internal class PulseInfo
{
	private float start;

	private float end;

	private float frequency;

	private float def;

	private TranisitionInfo transition;

	private float GetPulseAmt(float time)
	{
		if (time > start && frequency > 0f)
		{
			if (time > end && end > start)
			{
				return def;
			}
			return transition.GetAmt((float)Math.IEEERemainder(time - start, frequency) / frequency);
		}
		return def;
	}

	internal PulseInfo(float s, float f, float e, TranisitionInfo t)
	{
		def = 1f;
		start = s;
		frequency = f;
		end = e;
		transition = t;
		transition.empty = 1f;
		transition.full = 1.25f;
	}

	public PulseInfo()
		: this(0f, 0f, 0f, new TranisitionInfo(TransitionFunctions.FullTransition, 0f))
	{
	}

	public PulseInfo(float s, float f, float e)
		: this(s, f, e, new TranisitionInfo(TransitionFunctions.FullTransition, 0f))
	{
	}

	public PulseInfo(TranisitionInfo t, float s, float f, float e)
		: this(s, f, e, t)
	{
		Init(s, f, e);
		transition = t;
	}

	private void Init(float s, float f, float e)
	{
		def = 1f;
		start = s;
		frequency = f;
		end = e;
		transition = new TranisitionInfo(TransitionFunctions.FullTransition, 0f);
		transition.empty = 1f;
		transition.full = 1.25f;
	}
}
