using System.Collections.Generic;

namespace Mortar;

public abstract class InputDevice
{
	protected LinkedList<InputActionMapper> m_actions = new LinkedList<InputActionMapper>();

	public InputDevice()
	{
	}

	public abstract void Init();

	public abstract void Update(float dt);

	public virtual void Destroy()
	{
		m_actions.Clear();
	}

	public virtual void Reset()
	{
	}

	public void ButtonPressed(uint button, uint flags, float pressure, uint timestamp)
	{
		InputEvent inputEvent = new InputEvent();
		inputEvent.eventType = 0x10000 | flags;
		inputEvent.button.key = button;
		inputEvent.button.pressure = pressure;
		inputEvent.timeStamp = timestamp;
		CheckActions(inputEvent);
	}

	public void AxisEvent(int axis, uint flags, float absolute, float relative, uint timestamp)
	{
		InputEvent inputEvent = new InputEvent();
		inputEvent.eventType = 0x20000 | flags;
		inputEvent.axis.axis = axis;
		inputEvent.axis.absolutePos = absolute;
		inputEvent.axis.relativePos = relative;
		inputEvent.timeStamp = timestamp;
		CheckActions(inputEvent);
	}

	public virtual void AddAction(InputActionMapper action)
	{
		m_actions.AddLast(action);
	}

	public void CheckActions(InputEvent e)
	{
		foreach (InputActionMapper action in m_actions)
		{
			action.ProcessEvent(e);
		}
	}

	public void ClearActions(uint ParentHash, bool del)
	{
		for (LinkedListNode<InputActionMapper> linkedListNode = m_actions.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value.GetParentHash() == ParentHash || ParentHash == 0)
			{
				InputActionMapper dl = linkedListNode.Value;
				m_actions.Remove(linkedListNode);
				if (del)
				{
					Delete.SAFE_DELETE(ref dl);
				}
			}
		}
	}

	public void RegisterInputCallback(uint hash, string str, InputActionMapper.InputCallback callback)
	{
		bool flag = false;
		foreach (InputActionMapper action in m_actions)
		{
			if (action.GetHash() == hash)
			{
				flag = true;
				action.SetCallback(callback);
			}
		}
	}
}
