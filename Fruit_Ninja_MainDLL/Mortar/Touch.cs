using Microsoft.Xna.Framework;

namespace Mortar;

public class Touch
{
	private struct State
	{
		public int lx;

		public int ly;

		public int x;

		public int y;

		public uint internalId;

		public uint externalId;

		public int touchState;

		public void Update()
		{
			if (touchState == 1)
			{
				internalId = 0u;
				externalId = 0u;
				return;
			}
			lx = x;
			ly = y;
			if (touchState == -1)
			{
				touchState = 0;
			}
		}
	}

	public struct TEvnt
	{
		public uint internalId;

		public bool dn;

		public float x;

		public float y;

		public float timeStampSeconds;
	}

	private const int MAX_TOUCH_POINTS = 4;

	public const int PLATFORM_IPHONEOS_RES_X = 480;

	private State[] currentState = ArrayInit.CreateFilledArray<State>(4);

	private State[] nextState = ArrayInit.CreateFilledArray<State>(4);

	private static int loop = 0;

	public RingBufferT<TEvnt> eventBuffer = new RingBufferT<TEvnt>();

	public uint externalIDGenerator;

	public static Touch instance = new Touch();

	private Touch()
	{
		externalIDGenerator = 1u;
		eventBuffer.Init(10);
		for (int i = 0; i < 4; i++)
		{
			currentState[i].touchState = 1;
			nextState[i].touchState = 1;
		}
	}

	public void ___UpdateInternal(uint internalId, bool dn, float x, float y)
	{
		int num = -1;
		for (int i = 0; i < 4; i++)
		{
			int num2 = i + loop;
			num2 %= 4;
			if (nextState[num2].internalId == internalId)
			{
				if (dn)
				{
					nextState[num2].x = (int)x;
					nextState[num2].y = (int)y;
				}
				else
				{
					nextState[num2].touchState = 1;
				}
				return;
			}
			if (nextState[num2].internalId == 0)
			{
				num = num2;
			}
		}
		if (dn && num != -1)
		{
			loop++;
			if (loop >= 4)
			{
				loop = 0;
			}
			nextState[num].externalId = externalIDGenerator++;
			if (externalIDGenerator == 0)
			{
				externalIDGenerator = 1u;
			}
			nextState[num].internalId = internalId;
			nextState[num].touchState = -1;
			nextState[num].x = (int)x;
			nextState[num].y = (int)y;
			nextState[num].lx = (int)x;
			nextState[num].ly = (int)y;
		}
	}

	public void __UpdateInternal(uint internalId, bool dn, float x, float y, float timeStampSeconds)
	{
		TEvnt v = default(TEvnt);
		v.internalId = internalId;
		v.dn = dn;
		v.x = x;
		v.y = y;
		v.timeStampSeconds = timeStampSeconds;
		if (!eventBuffer.Push(ref v))
		{
			Update();
			bool flag = eventBuffer.Push(ref v);
		}
	}

	public int FindTouch(uint exId)
	{
		for (int i = 0; i < 4; i++)
		{
			if (currentState[i].externalId == exId)
			{
				return i;
			}
		}
		return -1;
	}

	public void _Update()
	{
		for (int i = 0; i < 4; i++)
		{
			ref State reference = ref currentState[i];
			reference = nextState[i];
			nextState[i].Update();
		}
	}

	public int GetTouchCount()
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (nextState[i].touchState != 1)
			{
				num++;
			}
		}
		return num;
	}

	public Vector2 GetPosition()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			if (nextState[i].touchState != 1)
			{
				return new Vector2((float)nextState[i].x, (float)nextState[i].y);
			}
		}
		return default(Vector2);
	}

	public void Clear()
	{
		for (int i = 0; i < 4; i++)
		{
			nextState[i].externalId = 0u;
			nextState[i].internalId = 0u;
			nextState[i].touchState = 1;
			nextState[i].x = 0;
			nextState[i].y = 0;
			nextState[i].lx = 0;
			nextState[i].ly = 0;
		}
	}

	public static Touch GetInstance()
	{
		return instance;
	}

	public void Update()
	{
		Update(0f);
	}

	public void Update(float gameTime)
	{
		while (true)
		{
			TEvnt ans = default(TEvnt);
			if (!eventBuffer.Peek(ref ans) || (ans.timeStampSeconds > gameTime && gameTime != 0f))
			{
				break;
			}
			TEvnt ans2 = default(TEvnt);
			eventBuffer.Pop(ref ans2);
			___UpdateInternal(ans2.internalId, ans2.dn, ans2.x, ans2.y);
		}
		_Update();
	}

	public bool GetTouchPos(uint uid, out int x, out int y)
	{
		x = 0;
		y = 0;
		int num = FindTouch(uid);
		if (num == -1)
		{
			return false;
		}
		x = currentState[num].x;
		y = currentState[num].y;
		if (currentState[num].touchState == 1)
		{
			return false;
		}
		return true;
	}

	public bool GetTouchDelta(uint uid, out int x, out int y)
	{
		x = 0;
		y = 0;
		int num = FindTouch(uid);
		if (num == -1)
		{
			return false;
		}
		if (currentState[num].touchState >= 0)
		{
			x = currentState[num].x - currentState[num].lx;
			y = currentState[num].y - currentState[num].ly;
		}
		if (currentState[num].touchState == 1)
		{
			return false;
		}
		return true;
	}

	public uint GetTouchInReigion(int x, int y, int w, int h)
	{
		for (int i = 0; i < 4; i++)
		{
			if (currentState[i].touchState < 1)
			{
				int x2 = currentState[i].x;
				int y2 = currentState[i].y;
				if (x2 >= x && x2 <= x + w && y2 >= y && y2 <= y + h)
				{
					return currentState[i].externalId;
				}
			}
		}
		return 0u;
	}

	public uint GetMostRecentTouch()
	{
		if (FindTouch(externalIDGenerator - 1) != -1)
		{
			return externalIDGenerator - 1;
		}
		return 0u;
	}

	public uint GetAnyTouch()
	{
		for (int i = 0; i < 4; i++)
		{
			if (currentState[i].touchState < 1)
			{
				return currentState[i].externalId;
			}
		}
		return 0u;
	}

	public void SendIndividualTouchCallbacks(InputDevice device)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = i;
			if (currentState[i].touchState < 1)
			{
				int x = currentState[i].x;
				int y = currentState[i].y;
				int lx = currentState[i].lx;
				int ly = currentState[i].ly;
				device.AxisEvent(153 + num, 32u, x, x - lx, 0u);
				device.AxisEvent(169 + num, 32u, y, y - ly, 0u);
				if (currentState[i].touchState == -1)
				{
					device.ButtonPressed((uint)(137 + num), 1u, 1f, 0u);
				}
				device.ButtonPressed((uint)(137 + num), 2u, 1f, 0u);
			}
			else
			{
				if (currentState[i].internalId != 0 && currentState[i].externalId != 0)
				{
					device.ButtonPressed((uint)(137 + num), 4u, 1f, 0u);
				}
				device.ButtonPressed((uint)(137 + num), 8u, 1f, 0u);
			}
		}
	}
}
