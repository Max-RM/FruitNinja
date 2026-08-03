using System.Collections.Generic;
using System.Diagnostics;

namespace Mortar;

internal class ProfileManager
{
	private class ProfileEntry
	{
		public string name;

		public long timeThisFrame;

		public long count;

		public long total;

		public long calls;

		public long callsMax;

		private Stopwatch sw;

		public ProfileEntry(string _name)
		{
			name = _name;
			sw = new Stopwatch();
			Init();
		}

		public void Begin()
		{
			if (sw.IsRunning)
			{
				sw.Stop();
			}
			sw.Reset();
			sw.Start();
			count++;
			calls++;
		}

		public void End()
		{
			if (sw.IsRunning)
			{
				sw.Stop();
				timeThisFrame += sw.Elapsed.Ticks;
				total += sw.Elapsed.Ticks;
			}
		}

		public void Update()
		{
		}

		public void Write()
		{
		}

		public void Reset()
		{
			if (calls > callsMax)
			{
				callsMax = calls;
			}
			timeThisFrame = 0L;
			calls = 0L;
		}

		public void Init()
		{
			timeThisFrame = 0L;
			count = 1L;
			total = 0L;
			calls = 0L;
			callsMax = 0L;
		}
	}

	private static LinkedList<ProfileEntry> entries = new LinkedList<ProfileEntry>();

	[Conditional("PROFILE")]
	public static void Begin(string name)
	{
		ProfileEntry profileEntry = null;
		foreach (ProfileEntry entry in entries)
		{
			if (string.Compare(name, entry.name) == 0)
			{
				profileEntry = entry;
				break;
			}
		}
		if (profileEntry == null)
		{
			profileEntry = new ProfileEntry(name);
			entries.AddLast(profileEntry);
		}
		profileEntry.Begin();
	}

	[Conditional("PROFILE")]
	public static void End(string name)
	{
		ProfileEntry profileEntry = null;
		foreach (ProfileEntry entry in entries)
		{
			if (string.Compare(name, entry.name) == 0)
			{
				profileEntry = entry;
				break;
			}
		}
		profileEntry?.End();
	}

	[Conditional("PROFILE")]
	public static void FrameEnd()
	{
		foreach (ProfileEntry entry in entries)
		{
			entry.Update();
			entry.Reset();
		}
	}

	[Conditional("PROFILE")]
	public static void ShowData()
	{
		foreach (ProfileEntry entry in entries)
		{
			entry.Write();
		}
	}

	[Conditional("PROFILE")]
	public static void Reset()
	{
		foreach (ProfileEntry entry in entries)
		{
			entry.Init();
		}
	}
}
