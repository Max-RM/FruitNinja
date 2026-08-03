using System.Globalization;

namespace Mortar;

public static class MParser
{
	public static int ParseInt(string astr)
	{
		return int.Parse(astr, CultureInfo.InvariantCulture);
	}

	public static float ParseFloat(string astr)
	{
		return float.Parse(astr, CultureInfo.InvariantCulture);
	}

	public static uint ParseUInt(string astr)
	{
		return uint.Parse(astr, CultureInfo.InvariantCulture);
	}

	public static ushort ParseUShort(string astr)
	{
		return ushort.Parse(astr, CultureInfo.InvariantCulture);
	}

	public static ulong ParseULong(string astr)
	{
		return ulong.Parse(astr, CultureInfo.InvariantCulture);
	}

	public static byte ParseByte(string astr)
	{
		return byte.Parse(astr, CultureInfo.InvariantCulture);
	}
}
