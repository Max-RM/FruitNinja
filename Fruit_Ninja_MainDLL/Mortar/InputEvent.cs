namespace Mortar;

public class InputEvent
{
	public enum Propagation
	{
		Continue = 0,
		Stop = 255
	}

	public uint eventType;

	public ushort chr;

	public ButtonEvent button;

	public axisEvent axis;

	public uint timeStamp;
}
