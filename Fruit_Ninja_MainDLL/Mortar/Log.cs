using System.Diagnostics;

namespace Mortar;

public class Log
{
	private static string prev;

	[Conditional("DEBUG")]
	public static void WriteLine(string message)
	{
		if (!string.IsNullOrEmpty(message))
		{
			_ = prev;
			prev = message;
		}
	}
}
