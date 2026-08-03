namespace Mortar;

public class InputActionMapper
{
	public delegate bool InputCallback(InputEvent bob);

	protected bool m_active;

	protected uint m_hash;

	protected uint m_creationHash;

	protected InputEvent m_event;

	protected InputCallback m_callback;

	public InputActionMapper(InputEvent e, InputCallback call)
		: this(e, call, 0u)
	{
	}

	public InputActionMapper(InputEvent e, InputCallback call, uint hash)
		: this(e, call, hash, 0u)
	{
	}

	public InputActionMapper(InputEvent e, InputCallback call, uint hash, uint creationHash)
	{
		m_hash = hash;
		m_active = true;
		m_event = new InputEvent();
		m_event.axis = e.axis;
		m_event.button = e.button;
		m_event.chr = e.chr;
		m_event.eventType = e.eventType;
		m_event.timeStamp = e.timeStamp;
		m_callback = call;
		m_creationHash = creationHash;
	}

	public uint GetHash()
	{
		return m_hash;
	}

	public uint GetParentHash()
	{
		return m_creationHash;
	}

	public void SetCallback(InputCallback callback)
	{
		m_callback = callback;
	}

	public bool ProcessEvent(InputEvent e)
	{
		uint num = e.eventType & 0xFFFF0000u;
		uint num2 = e.eventType & 0xFFFF;
		if ((num & m_event.eventType) != 0 && (num2 & m_event.eventType) != 0)
		{
			bool flag = false;
			switch (num)
			{
			case 65536u:
				if (e.button.key == m_event.button.key)
				{
					flag |= m_callback(e);
				}
				break;
			case 131072u:
				if (m_event.axis.axis >= 137)
				{
					if (e.axis.axis == m_event.axis.axis)
					{
						return flag |= m_callback(e);
					}
				}
				else if ((e.axis.axis & m_event.axis.axis) != 0)
				{
					return flag |= m_callback(e);
				}
				break;
			}
		}
		return false;
	}

	public InputEvent GetEvent()
	{
		return m_event;
	}
}
