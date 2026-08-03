using System;
using System.Diagnostics;

namespace Mortar;

public class Assert
{
	[Conditional("DEBUG")]
	public static void ASSERT(bool f)
	{
		if (!f)
		{
			throw new MissingMethodException("From assert");
		}
	}

	[Conditional("DEBUG")]
	public static void PANIC(string sdf)
	{
		throw new MissingMethodException(sdf);
	}
}
