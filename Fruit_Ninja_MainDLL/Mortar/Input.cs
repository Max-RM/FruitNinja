namespace Mortar;

public static class Input
{
	public const uint ACTION_MASK = 65535u;

	public const uint EVENT_MASK = 4294901760u;

	public const uint EVENTTYPE_PRESSED = 1u;

	public const uint EVENTTYPE_DOWN = 2u;

	public const uint EVENTTYPE_RELEASED = 4u;

	public const uint EVENTTYPE_UP = 8u;

	public const uint EVENTTYPE_ACTIVE = 16u;

	public const uint EVENTTYPE_MOVED = 32u;

	public const uint EVENTTYPE_DEAD = 64u;

	public const uint EVENTTYPE_BUTTON = 65536u;

	public const uint EVENTTYPE_AXIS = 131072u;

	public const uint EVENTTYPE_AXIS2D = 262144u;
}
