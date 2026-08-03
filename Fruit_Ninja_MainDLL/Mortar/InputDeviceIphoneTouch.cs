namespace Mortar;

internal class InputDeviceIphoneTouch : InputDevice
{
	private int currTouchId;

	private int lx;

	private int ly;

	private uint randomTimestampThing;

	public override void Init()
	{
	}

	public override void Update(float dt)
	{
		randomTimestampThing++;
		int x;
		int y;
		if (currTouchId == 0)
		{
			currTouchId = (int)Touch.GetInstance().GetMostRecentTouch();
			if (currTouchId == 0)
			{
				currTouchId = (int)Touch.GetInstance().GetAnyTouch();
			}
			if (currTouchId != 0)
			{
				Touch.GetInstance().GetTouchPos((uint)currTouchId, out x, out y);
				int num = x;
				int num2 = y;
				AxisEvent(116, 32u, num, num - lx, randomTimestampThing);
				AxisEvent(117, 32u, num2, num2 - ly, randomTimestampThing);
				ButtonPressed(108u, 1u, 1f, randomTimestampThing);
				lx = num;
				ly = num2;
			}
			else
			{
				ButtonPressed(108u, 8u, 1f, randomTimestampThing);
			}
		}
		else
		{
			uint num3 = Touch.GetInstance().GetMostRecentTouch();
			if (num3 == 0)
			{
				num3 = Touch.GetInstance().GetAnyTouch();
			}
			bool flag = num3 != currTouchId;
			if (!Touch.GetInstance().GetTouchDelta((uint)currTouchId, out var _, out var _) || flag)
			{
				ButtonPressed(108u, 4u, 1f, randomTimestampThing);
				ButtonPressed(108u, 8u, 1f, randomTimestampThing);
				currTouchId = 0;
			}
			else
			{
				Touch.GetInstance().GetTouchPos((uint)currTouchId, out x, out y);
				int num = x;
				int num2 = y;
				AxisEvent(116, 32u, num, num - lx, randomTimestampThing);
				AxisEvent(117, 32u, num2, num2 - ly, randomTimestampThing);
				ButtonPressed(108u, 2u, 1f, randomTimestampThing);
				lx = num;
				ly = num2;
			}
		}
		Touch.GetInstance().SendIndividualTouchCallbacks(this);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void Reset()
	{
		currTouchId = 0;
		lx = 0;
		ly = 0;
		Touch.GetInstance().Clear();
	}

	public bool TestAxisDelta(int axis, float amount)
	{
		return false;
	}
}
