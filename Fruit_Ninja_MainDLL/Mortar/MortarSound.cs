using Microsoft.Xna.Framework.Audio;

namespace Mortar;

public class MortarSound
{
	public SoundEffectInstance inst;

	public void SetVolume(float v)
	{
		if (inst != null)
		{
			inst.Volume = Math.CLAMP(v, 0f, 1f);
		}
	}

	public void Stop(float v)
	{
		if (inst != null)
		{
			inst.Stop();
		}
	}

	public void Repeat()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (inst != null && (int)inst.State == 2)
		{
			inst.Play();
		}
	}
}
