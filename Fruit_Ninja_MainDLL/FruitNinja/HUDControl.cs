using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class HUDControl
{
	public delegate void HUDControlDeletedCallback(HUDControl bob);

	public const int NUM_TEXT_ALIGN_RIGHT = 1;

	public const int NUM_TEXT_ALIGN_CENTER = 2;

	public const int NUM_TEXT_DISABLE_MTX_RESET = 4;

	public const int TINT_CHANNELS = 3;

	public Vector3 m_pos;

	public Vector3 m_scale;

	public Vector3 m_posAnchor;

	public float m_rotation;

	public bool m_update;

	public bool m_forceDraw;

	public bool m_selfCleanUp;

	public bool m_terminate;

	public bool partOfPopup;

	public HUD.HUD_ORDER m_drawOrder;

	public HUDControlDeletedCallback m_deleteCall;

	public Color m_color;

	public bool m_canBeTinted;

	public Vector2[] m_uvs = new Vector2[2];

	public static Color TintColor(Color col, float[] tints)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		col.R = (byte)Math.CLAMP((float)(int)col.R * tints[0], 0f, 255f);
		col.G = (byte)Math.CLAMP((float)(int)col.G * tints[1], 0f, 255f);
		col.B = (byte)Math.CLAMP((float)(int)col.B * tints[2], 0f, 255f);
		return col;
	}

	public static Color TintWhite(float[] tints)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		return new Color((int)Math.CLAMP(255f * tints[0], 0f, 255f), (int)Math.CLAMP(255f * tints[1], 0f, 255f), (int)Math.CLAMP(255f * tints[2], 0f, 255f), 255);
	}

	public static void DefaultDeleteCallback(HUDControl control)
	{
	}

	public HUDControl()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
		m_update = true;
		m_deleteCall = DefaultDeleteCallback;
		m_color = Color.White;
		m_canBeTinted = true;
		ref Vector2 reference = ref m_uvs[0];
		reference = Vector2.Zero;
		ref Vector2 reference2 = ref m_uvs[1];
		reference2 = Vector2.One;
	}

	public virtual void Init()
	{
	}

	public virtual void Release()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void PreDraw(float[] tintChannels)
	{
	}

	public virtual void Draw(float[] tintChannels)
	{
	}

	public virtual void Update(float dt)
	{
	}

	public bool ForceDraw()
	{
		return m_forceDraw;
	}

	public virtual void DrawOrder(float[] tintChannels, int order)
	{
		Draw(tintChannels);
	}

	public void SetActive(bool state)
	{
		m_update = state;
	}

	public bool GetActive()
	{
		return m_update;
	}

	public virtual bool Terminate()
	{
		return m_terminate;
	}

	public new virtual HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_2D;
	}

	public virtual void Skip()
	{
	}

	public virtual void Save()
	{
	}
}
