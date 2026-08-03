using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class PopOverControl
{
	public enum POC
	{
		IN,
		MOVING_IN,
		OUT,
		MOVING_OUT
	}

	public delegate void WhenIn();

	private static PopOverControl _instance = new PopOverControl();

	private Texture backgroundTex;

	private float m_time;

	private static POC m_state = POC.OUT;

	public static bool IsInPopup = false;

	public WhenIn whenIndel;

	public static PopOverControl Instance => _instance;

	public static POC State => m_state;

	public PopOverControl()
	{
		backgroundTex = TextureManager.GetInstance().Load("textureswp7/BG_store.tex");
		m_state = POC.OUT;
		m_time = 0f;
	}

	public static void Update(float dt)
	{
		_instance._Update(dt);
	}

	private void _Update(float dt)
	{
		switch (m_state)
		{
		case POC.MOVING_IN:
			m_time += (1f - m_time) * 0.25f;
			if (m_time > 0.999f)
			{
				m_time = 1f;
				m_state = POC.IN;
				IsInPopup = true;
				if (whenIndel != null)
				{
					whenIndel();
					whenIndel = null;
				}
			}
			break;
		case POC.MOVING_OUT:
			IsInPopup = false;
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				m_time = 0f;
				m_state = POC.OUT;
			}
			break;
		case POC.OUT:
			break;
		}
	}

	public static void DrawBack()
	{
		_instance._DrawBack();
	}

	public static void DrawFront()
	{
		_instance._DrawFront();
	}

	public void In()
	{
		if (m_state != POC.IN)
		{
			m_state = POC.MOVING_IN;
		}
	}

	public void In(WhenIn del)
	{
		whenIndel = del;
		if (m_state != POC.IN)
		{
			m_state = POC.MOVING_IN;
		}
	}

	public void Out()
	{
		if (m_state != POC.OUT)
		{
			m_state = POC.MOVING_OUT;
		}
		IsInPopup = false;
	}

	private void _DrawBack()
	{
		if (m_state == POC.IN)
		{
			_Draw();
		}
	}

	private void _DrawFront()
	{
		if (m_state == POC.MOVING_IN || m_state == POC.MOVING_OUT)
		{
			_Draw();
		}
	}

	public void _Draw()
	{
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		float num = Game.SCREEN_WIDTH / 2f - ShopScreen.SHOP_BACK_RIGHT_SIDE / 2f;
		if (m_time < 1f)
		{
			num += (1f - m_time) * ShopScreen.SHOP_BACK_RIGHT_SIDE * 1.5f;
			backgroundTex.Set();
			float num2 = ShopScreen.SHOP_LIST_POS_X - (1f - m_time) * ShopScreen.SHOP_BACK_LEFT_SIDE * 1.5f;
			Matrix val = Matrix.CreateScale(new Vector3(ShopScreen.SHOP_BACK_LEFT_SIDE + 1f, Game.SCREEN_HEIGHT + 1f, 0f));
			val *= Matrix.CreateTranslation(new Vector3(num2, 0f, 0f));
			MatrixManager.instance.Reset();
			MatrixManager.instance.SetMatrix(val);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(Color.White, (512f - Game.SCREEN_WIDTH) / 2f / 512f, (ShopScreen.SHOP_BACK_LEFT_SIDE + (512f - Game.SCREEN_WIDTH) / 2f) / 512f, (512f - Game.SCREEN_HEIGHT) / 2f / 512f, 1f - (512f - Game.SCREEN_HEIGHT) / 2f / 512f);
			Math.Scale44(new Vector3(ShopScreen.SHOP_BACK_RIGHT_SIDE + 1f, Game.SCREEN_HEIGHT + 1f, 0f), out val);
			Vector3 scl = default(Vector3);
			scl = new Vector3(num, 0f, 0f);
			Math.GlobalTranslate44(ref val, scl);
			MatrixManager.instance.Reset();
			MatrixManager.instance.SetMatrix(val);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(Color.White, (ShopScreen.SHOP_BACK_LEFT_SIDE + (512f - Game.SCREEN_WIDTH) / 2f) / 512f, 1f - (512f - Game.SCREEN_WIDTH) / 2f / 512f, (512f - Game.SCREEN_HEIGHT) / 2f / 512f, 1f - (512f - Game.SCREEN_HEIGHT) / 2f / 512f);
		}
		else
		{
			Math.Scale44(new Vector3(Game.SCREEN_WIDTH + 1f, Game.SCREEN_HEIGHT + 1f, 0f), out var mtx);
			MatrixManager.instance.Reset();
			MatrixManager.instance.SetMatrix(mtx);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			backgroundTex.Set();
			Mesh.DrawQuad(Color.White, (512f - Game.SCREEN_WIDTH) / 2f / 512f, 1f - (512f - Game.SCREEN_WIDTH) / 2f / 512f, (512f - Game.SCREEN_HEIGHT) / 2f / 512f, 1f - (512f - Game.SCREEN_HEIGHT) / 2f / 512f);
		}
	}
}
