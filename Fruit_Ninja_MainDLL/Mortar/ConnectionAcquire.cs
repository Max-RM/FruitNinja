using System.Net.NetworkInformation;

namespace Mortar;

internal class ConnectionAcquire
{
	private bool complete;

	private string info;

	public ConnectionAcquire()
	{
		complete = false;
	}

	public bool IsComplete()
	{
		return complete;
	}

	public void DoWork()
	{
		try
		{
			info = NetworkInterface.GetIsNetworkAvailable().ToString();
		}
		catch
		{
			info = "Unknown";
		}
		complete = true;
	}

	public string GetInterfaceType()
	{
		return info;
	}
}
