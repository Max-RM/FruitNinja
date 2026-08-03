using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ScrollingMenuItem
{
	public delegate void ClickedMenuItemCallback(ScrollingMenuItem fdsdf);

	protected Vector3 m_pos;

	protected ScrollingMenu m_parentList;

	protected Color m_colour;

	protected Vector3 m_textOffset;

	protected float m_height;

	protected float m_width;

	protected bool m_highlighted;

	protected bool m_isOnScreen;

	protected ClickedMenuItemCallback m_clickedCallback;

	public string m_text;

	public static void DefaultClickedMenuItemCallback(ScrollingMenuItem item)
	{
	}

	public void CallClickedMenuItemCallback()
	{
		m_clickedCallback(this);
	}

	public void SetClickedFocusedCallback()
	{
		SetClickedFocusedCallback(DefaultClickedMenuItemCallback);
	}

	public void SetClickedFocusedCallback(ClickedMenuItemCallback call)
	{
		m_clickedCallback = call;
	}

	public ScrollingMenuItem()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		m_textOffset = Vector3.Zero;
		m_text = null;
		m_colour = Color.White;
		m_height = 25f;
		m_width = 0f;
		m_clickedCallback = DefaultClickedMenuItemCallback;
	}

	public ScrollingMenuItem(float height, float width, string text)
		: this(height, width, text, DefaultClickedMenuItemCallback)
	{
	}

	public ScrollingMenuItem(float height, float width, string text, ClickedMenuItemCallback call)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		m_clickedCallback = call;
		m_textOffset = Vector3.Zero;
		m_text = null;
		SetText(text);
		m_colour = Color.White;
		m_height = height;
		m_width = width;
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
	}

	public virtual void Move(Vector3 toPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_pos = toPos;
	}

	public virtual void Remove()
	{
		Delete.SAFE_DELETE_ARRAY(ref m_text);
	}

	public void Highlight(bool highlight)
	{
		m_highlighted = highlight;
	}

	public virtual void SetParent(ScrollingMenu parent)
	{
		m_parentList = parent;
	}

	public virtual void SetOnscreen(bool onscreen)
	{
		m_isOnScreen = onscreen;
	}

	public virtual void SetText(string text)
	{
		m_text = text;
	}

	public virtual void Draw()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (m_text != null)
		{
			Vector3 pos = m_pos + m_textOffset;
			MortarRectangleDec? rect = null;
			if (m_parentList != null)
			{
				MortarRectangleDec value = default(MortarRectangleDec);
				value.top = m_parentList.m_pos.Y + m_parentList.GetHeight() / 2f;
				value.bottom = m_parentList.m_pos.Y - m_parentList.GetHeight() / 2f;
				value.left = m_parentList.m_pos.X - m_parentList.GetWidth() / 2f;
				value.right = m_parentList.m_pos.X + m_parentList.GetWidth() / 2f;
				rect = value;
			}
			Game.game_work.pGameFont.DrawString(m_text, pos, m_colour, 30f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER, 1f, rect);
		}
	}
}
