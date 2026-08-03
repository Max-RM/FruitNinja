using System.Collections.Generic;

namespace FruitNinja;

public class HUD
{
	public enum HUD_ORDER
	{
		HUD_ORDER_NORMAL,
		HUD_ORDER_POST,
		HUD_ORDER_BEFORE_SPLAT,
		HUD_ORDER_AFTER_SPLAT,
		HUD_ORDER_BEFORE_BOMB,
		HUD_ORDER_AFTER_BOMB
	}

	public float m_hudAmount;

	public float[] m_tint = new float[3];

	public float[] m_backTint = new float[3];

	protected LinkedList<HUDControl> m_controls = new LinkedList<HUDControl>();

	protected List<HUDControl> m_notifications = new List<HUDControl>();

	private static float[] tint = new float[3] { 1f, 1f, 1f };

	public static HUD_ORDER ORDER = HUD_ORDER.HUD_ORDER_AFTER_BOMB;

	public HUD()
	{
		for (int i = 0; i < 3; i++)
		{
			m_backTint[i] = 1f;
			m_tint[i] = 1f;
		}
		m_hudAmount = 1f;
	}

	public void Init()
	{
		m_controls.Clear();
	}

	public void Release()
	{
		foreach (HUDControl control in m_controls)
		{
			control.m_deleteCall(control);
		}
		m_controls.Clear();
	}

	public void AddNotification(HUDControl control)
	{
		m_notifications.Add(control);
	}

	public void AddControl(HUDControl control)
	{
		AddControl(control, toFront: false);
	}

	public void AddControl(HUDControl control, bool toFront)
	{
		if (toFront)
		{
			m_controls.AddFirst(control);
		}
		else
		{
			m_controls.AddLast(control);
		}
	}

	public void RemoveControl(HUDControl control)
	{
		if (control != null)
		{
			control.m_deleteCall(control);
			m_controls.Remove(control);
		}
	}

	public void ResetControls()
	{
		foreach (HUDControl control in m_controls)
		{
			control.Reset();
		}
	}

	public void Update(float dt)
	{
		MissControl.PreUpdate(dt);
		LinkedListNode<HUDControl> linkedListNode = m_controls.First;
		while (linkedListNode != null)
		{
			if (linkedListNode.Value.GetActive())
			{
				linkedListNode.Value.Update(dt);
			}
			if (linkedListNode.Value.Terminate())
			{
				linkedListNode.Value.m_deleteCall(linkedListNode.Value);
				LinkedListNode<HUDControl> next = linkedListNode.Next;
				m_controls.Remove(linkedListNode);
				linkedListNode = next;
			}
			else
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		if (m_notifications.Count > 0)
		{
			HUDControl hUDControl = m_notifications[0];
			hUDControl.Update(dt);
			if (hUDControl.Terminate())
			{
				m_notifications.Remove(hUDControl);
			}
		}
	}

	public void Save()
	{
		foreach (HUDControl control in m_controls)
		{
			control.Save();
		}
	}

	public void Draw()
	{
		Draw(HUD_ORDER.HUD_ORDER_NORMAL);
	}

	public void Draw(HUD_ORDER order)
	{
		ORDER = order;
		foreach (HUDControl control in m_controls)
		{
			if (control.m_drawOrder == order && control.m_update && (!PopOverControl.IsInPopup || control.partOfPopup))
			{
				if (control.m_canBeTinted)
				{
					control.PreDraw(m_tint);
					control.Draw(m_tint);
				}
				else
				{
					control.PreDraw(tint);
					control.Draw(tint);
				}
			}
		}
		if (m_notifications.Count <= 0)
		{
			return;
		}
		HUDControl hUDControl = m_notifications[0];
		if (hUDControl.m_drawOrder == order && hUDControl.GetActive() && (!PopOverControl.IsInPopup || hUDControl.partOfPopup))
		{
			if (hUDControl.m_canBeTinted)
			{
				hUDControl.PreDraw(m_tint);
				hUDControl.Draw(m_tint);
			}
			else
			{
				hUDControl.PreDraw(tint);
				hUDControl.Draw(tint);
			}
		}
	}

	public void Skip()
	{
		foreach (HUDControl control in m_controls)
		{
			control.Skip();
		}
	}

	public void OnPause()
	{
		foreach (HUDControl control in m_controls)
		{
			if (control.GetType() == HUD_TYPE.HUD_TYPE_SCROLLING_LIST)
			{
				((ScrollingMenu)control).ClearTouch();
			}
		}
	}
}
