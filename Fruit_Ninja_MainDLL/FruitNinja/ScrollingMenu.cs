using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ScrollingMenu : HUDControl
{
	public const float LIST_FRICTION = 0.9f;

	protected int m_touchInRegion;

	protected Vector3 m_touchStartPos;

	protected Vector3 m_touchStartOffset;

	protected Vector3 m_vel;

	protected float m_itemHeight;

	protected float m_height;

	protected float m_width;

	protected float m_totalHeight;

	protected float m_totalWidth;

	protected List<ScrollingMenuItem> m_list = new List<ScrollingMenuItem>();

	protected int m_itemClosestToZero;

	protected int m_overideLockTo;

	protected float m_velocityFix;

	protected bool m_hasMovedList;

	protected bool m_isLockedOn;

	protected bool m_cullItemsOutOfBounds;

	public bool m_disabled;

	public Vector3 m_offset;

	public MortarRectangleDec m_initialTouchRegion;

	public MortarRectangleDec m_scrollTouchRegion;

	protected void DestroyList()
	{
		SlashEntity.ModPowerMask &= 4294967231u;
		m_list.Clear();
	}

	public ScrollingMenu()
	{
		m_totalWidth = 0f;
		m_totalHeight = 0f;
		m_itemClosestToZero = 0;
		m_width = Game.SCREEN_WIDTH / 2f;
		m_height = Game.SCREEN_HEIGHT;
		m_itemHeight = Game.SCREEN_HEIGHT;
		m_velocityFix = 1f;
		m_hasMovedList = false;
		m_overideLockTo = -1;
		m_isLockedOn = false;
		m_touchInRegion = -1;
		m_disabled = false;
		m_initialTouchRegion = new MortarRectangleDec((0f - m_width) / 2f, Game.SCREEN_HEIGHT, m_width / 2f, 0f - Game.SCREEN_HEIGHT);
		m_scrollTouchRegion = new MortarRectangleDec((0f - m_width) / 2f, Game.SCREEN_HEIGHT, m_width / 2f, 0f - Game.SCREEN_HEIGHT);
		m_deleteCall = delegate(HUDControl l)
		{
			((ScrollingMenu)l).Release();
		};
	}

	~ScrollingMenu()
	{
		Release();
	}

	public override void Reset()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		m_selfCleanUp = true;
		m_touchInRegion = -1;
		m_offset = Vector3.Zero;
		m_touchStartPos = Vector3.Zero;
		m_touchStartOffset = Vector3.Zero;
		m_vel = Vector3.Zero;
		m_velocityFix = 1f;
		m_overideLockTo = -1;
		m_cullItemsOutOfBounds = false;
	}

	public override void Release()
	{
		DestroyList();
	}

	public override void Init()
	{
		Reset();
	}

	public override void Update(float dt)
	{
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		m_isLockedOn = false;
		if (m_touchInRegion == -1 && !m_disabled)
		{
			m_touchInRegion = Game.TouchInRegion(m_pos.X + m_initialTouchRegion.left, m_pos.X + m_initialTouchRegion.right, m_pos.X + m_initialTouchRegion.bottom, m_pos.X + m_initialTouchRegion.top);
			if (Game.IsTouchDown(m_touchInRegion) != 2)
			{
				m_touchInRegion = -1;
			}
			else
			{
				SlashEntity.ModPowerMask |= 64u;
				m_overideLockTo = -1;
				m_touchStartPos = Game.game_work.touchPositions[m_touchInRegion];
				m_touchStartOffset = m_offset;
			}
		}
		int overideLockTo = m_overideLockTo;
		if (m_touchInRegion != -1 && Game.TouchInRegion(m_pos.X + m_scrollTouchRegion.left, m_pos.X + m_scrollTouchRegion.right, m_pos.X + m_scrollTouchRegion.bottom, m_pos.X + m_scrollTouchRegion.top, m_touchInRegion) != m_touchInRegion)
		{
			if (!m_hasMovedList)
			{
				float num = m_pos.Y - m_offset.Y;
				int num2 = 0;
				m_overideLockTo = -1;
				float num3 = 1000000f;
				foreach (ScrollingMenuItem item in m_list)
				{
					if (Math.ABS(num - m_pos.Y - m_touchStartPos.Y) < num3)
					{
						num3 = Math.ABS(num - m_pos.Y - m_touchStartPos.Y);
						m_overideLockTo = num2;
					}
					num -= item.GetHeight();
					num2++;
				}
				m_vel = Vector3.Zero;
			}
			else
			{
				float num4 = 0f;
				float num5 = m_vel.Y;
				while (Math.ABS(num5) > 0.05f)
				{
					num5 *= 0.9f;
					num4 += num5;
				}
				if (Math.ABS(num4) > 0.01f && m_offset.Y + num4 <= 0.01f && m_offset.Y + num4 >= 0f - m_totalHeight + m_itemHeight + 0.01f)
				{
					float num6 = 10000f;
					float num7 = 0f;
					float num8 = m_pos.Y - m_offset.Y - num4;
					float num9 = 0f;
					foreach (ScrollingMenuItem item2 in m_list)
					{
						if (Math.ABS(num8 - m_pos.Y) < num6)
						{
							num6 = Math.ABS(num8 - m_pos.Y);
							num7 = num9;
						}
						num9 -= item2.GetHeight();
						num8 -= item2.GetHeight();
					}
					m_velocityFix = Math.ABS((num7 - m_offset.Y) / num4);
				}
			}
			m_hasMovedList = false;
			m_touchInRegion = -1;
			SlashEntity.ModPowerMask &= 4294967231u;
		}
		if (m_touchInRegion != -1)
		{
			m_velocityFix = 1f;
			m_vel.Y = (0f - (m_offset.Y - (m_touchStartOffset.Y - (Game.game_work.touchPositions[m_touchInRegion].Y - m_touchStartPos.Y)))) * 0.5f;
			if (Math.ABS(Game.game_work.touchPositions[m_touchInRegion].Y - m_touchStartPos.Y) > 0.001f)
			{
				m_hasMovedList = true;
			}
		}
		m_vel *= 0.9f;
		m_offset += m_vel * m_velocityFix;
		Vector3 toPos = m_pos - m_offset;
		int num10 = 0;
		float num11 = 0f;
		ScrollingMenuItem scrollingMenuItem = null;
		float num12 = 10000f;
		float num13 = 0f;
		m_itemClosestToZero = 0;
		ScrollingMenuItem scrollingMenuItem2 = null;
		float num14 = Game.SCREEN_HEIGHT / 2f;
		float num15 = (0f - Game.SCREEN_HEIGHT) / 2f;
		if (m_cullItemsOutOfBounds)
		{
			num14 = m_pos.Y;
			num15 = m_pos.Y - m_height;
		}
		foreach (ScrollingMenuItem item3 in m_list)
		{
			scrollingMenuItem = item3;
			num11 = scrollingMenuItem.GetHeight() / 2f;
			if (m_overideLockTo >= 0)
			{
				if (m_overideLockTo == num10)
				{
					num12 = Math.ABS(toPos.Y - m_pos.Y);
					num13 = toPos.Y - (m_pos.Y - m_offset.Y);
					m_itemClosestToZero = num10;
					scrollingMenuItem2 = scrollingMenuItem;
				}
			}
			else if (Math.ABS(toPos.Y - m_pos.Y) < num12)
			{
				num12 = Math.ABS(toPos.Y - m_pos.Y);
				num13 = toPos.Y - (m_pos.Y - m_offset.Y);
				m_itemClosestToZero = num10;
			}
			toPos.Y -= num11;
			if (toPos.Y - num11 > num14 || toPos.Y + num11 < num15)
			{
				scrollingMenuItem.SetOnscreen(onscreen: false);
			}
			else
			{
				scrollingMenuItem.SetOnscreen(onscreen: true);
			}
			scrollingMenuItem.Move(toPos);
			toPos.Y -= num11;
			num10++;
		}
		float num16 = num13 - m_offset.Y;
		if (!m_hasMovedList && Math.ABS(num16) < 2f && Math.ABS(m_vel.Y) < 0.5f)
		{
			m_isLockedOn = true;
			if (overideLockTo == -1)
			{
				scrollingMenuItem2?.CallClickedMenuItemCallback();
			}
		}
		if (m_offset.Y > 0f && m_overideLockTo < 0)
		{
			ref Vector3 offset = ref m_offset;
			offset.Y *= 0.75f;
			m_vel *= 0.9f;
		}
		else if (m_offset.Y < 0f - m_totalHeight + m_height && m_overideLockTo < 0)
		{
			ref Vector3 offset2 = ref m_offset;
			offset2.Y += (0f - m_totalHeight + m_height - m_offset.Y) * 0.25f;
			m_vel *= 0.9f;
		}
		else if (m_touchInRegion == -1 && Math.ABS(m_vel.Y) <= 0.1f)
		{
			ref Vector3 offset3 = ref m_offset;
			offset3.Y += num16 * 0.1f;
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		foreach (ScrollingMenuItem item in m_list)
		{
			item.Draw();
		}
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_SCROLLING_LIST;
	}

	public override void Skip()
	{
	}

	public void ClearTouch()
	{
		m_touchInRegion = -1;
		m_hasMovedList = false;
	}

	public void AddItem(ScrollingMenuItem item)
	{
		m_totalWidth += item.GetWidth();
		m_totalHeight += item.GetHeight();
		item.SetParent(this);
		m_list.Add(item);
	}

	public virtual ScrollingMenuItem GetItemClosestToZero()
	{
		return m_list[m_itemClosestToZero];
	}

	public virtual int GetItemClosestToZeroIdx()
	{
		return m_itemClosestToZero;
	}

	public virtual float GetHeight()
	{
		return m_height;
	}

	public virtual float GetWidth()
	{
		return m_width;
	}

	public virtual void SetHeight(float v)
	{
		m_height = v;
	}

	public virtual void SetWidth(float v)
	{
		m_width = v;
		m_scrollTouchRegion.left = 1.25f * (m_initialTouchRegion.left = (0f - v) / 2f);
		m_scrollTouchRegion.right = 1.25f * (m_initialTouchRegion.right = v / 2f);
	}

	public virtual void SetItemHeight(float v)
	{
		m_itemHeight = v;
	}

	public virtual float GetItemHeight()
	{
		return m_itemHeight;
	}

	public int GetNumItems()
	{
		return m_list.Count;
	}

	public bool IsLockedIn()
	{
		return m_isLockedOn;
	}

	public void SetCullItems(bool val)
	{
		m_cullItemsOutOfBounds = val;
	}

	public bool GetCullItems()
	{
		return m_cullItemsOutOfBounds;
	}
}
