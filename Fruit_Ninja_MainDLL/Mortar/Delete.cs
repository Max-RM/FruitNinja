namespace Mortar;

public class Delete
{
	public static void SAFE_DELETE<T>(ref T dl)
	{
		dl = default(T);
	}

	public static void SAFE_DELETE_ARRAY<T>(ref T dl)
	{
		dl = default(T);
	}
}
