using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ShopListItem : ScrollingMenuItem
{
	protected ShopScreen m_shop;

	protected string m_description;

	protected float m_newFade;

	protected float m_selectedFade;

	public float m_shakeTime;

	public Vector3 m_texturePos;

	public Texture m_texture;

	public ItemInfo m_info;

	public bool m_backing;

	public static bool s_isContentLoaded = false;

	public static Texture blankTex = null;

	public static Texture descriptionBox = null;

	public static Texture backGround = null;

	public static Texture dividerTex = null;

	public static Texture buyNowTexture = null;

	public static Texture equipNowTexture = null;

	public static Texture unequipNowTexture = null;

	public static Texture swipe_to_scroll = null;

	public static Texture coinsBuyTexture = null;

	public static Texture coinsSpendTexture = null;

	public static Texture selectedTexture = null;

	public static Texture selectedSmallTexture = null;

	public static Texture lockedStrokeTexture = null;

	public static Texture newStrokeTexture = null;

	public static Texture newItemTexture = null;

	public static float newItemBob = 0f;

	public static ushort newItemBobTime = 0;

	public bool isFirst;

	public bool isLast;

	public float m_fadeTime;

	private static int lastDrawnType;

	private static float[] typeNameLengths;

	public static float BOB_TIME => 0.5f;

	public static float BOB_AMT => 6f;

	public ShopListItem()
	{
		m_shakeTime = 0f;
		m_info = null;
		m_texture = null;
		m_backing = true;
		isFirst = false;
		isLast = false;
		m_fadeTime = 0f;
		m_newFade = 0f;
		m_selectedFade = 0f;
	}

	~ShopListItem()
	{
		m_text = null;
		m_texture = null;
		m_backing = false;
	}

	public void Create(ItemInfo info, ShopScreen shop)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		m_fadeTime = 0f;
		m_text = "";
		m_info = info;
		m_height = ShopScreen.SHOP_ITEM_HEIGHT;
		m_width = ShopScreen.SHOP_BACK_LEFT_SIDE;
		m_shop = shop;
		m_textOffset = new Vector3(60f, 13f, 0f);
		if (info.textureName != null)
		{
			string text = null;
			text = ((info.type != ItemType.ITEM_BACKGROUND) ? $"textureswp7/{info.textureName}.tex" : $"textureswp7/item_{info.textureName}.tex");
			m_texture = TextureManager.GetInstance().Load(text);
		}
		if (m_info.IsLocked())
		{
			if (m_info.unlockTotal != null)
			{
				int num = Game.game_work.saveData.GetTotal(StringFunctions.StringHash(m_info.unlockTotal));
				if (m_info.unlockCountDownFrom > 0)
				{
					num = Math.MAX(0, m_info.unlockCountDownFrom - num);
				}
				m_description = TheGame.instance.stringTable.GetString(m_info.unlockDescription);
				m_description = string.Format(m_description, num);
			}
			else
			{
				m_description = TheGame.instance.stringTable.GetString(m_info.unlockDescription);
			}
		}
		else
		{
			m_description = TheGame.instance.stringTable.GetString(m_info.shopDescription);
		}
		if (ItemManager.GetInstance().IsEquipped(m_info))
		{
			m_selectedFade = 1f;
		}
		if (!m_info.hasBeenSeen)
		{
			m_newFade = 1f;
		}
	}

	public override void Move(Vector3 toPos)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (isFirst)
		{
			newItemBob = Math.ABS(Math.SinIdx(newItemBobTime) * BOB_AMT);
			newItemBobTime += (ushort)(Game.game_work.dt * ((float)(int)Math.DEGREE_TO_IDX(180f) / BOB_TIME) / 2f);
		}
		m_pos = toPos;
		if (m_texture != null)
		{
			m_texturePos = m_pos;
			ref Vector3 texturePos = ref m_texturePos;
			texturePos.X += m_textOffset.X + 35.2f;
			if (m_shakeTime > 0f)
			{
				m_shakeTime -= Game.game_work.dt;
				m_texturePos += new Vector3(Math.g_random.RandF(ShopScreen.SHAKE_RANGE) - ShopScreen.SHAKE_RANGE / 2f, Math.g_random.RandF(ShopScreen.SHAKE_RANGE) - ShopScreen.SHAKE_RANGE / 2f, 0f);
			}
		}
		if (m_shop != null && m_shop.GetSelected() == this)
		{
			m_fadeTime = Math.MIN(1f, m_fadeTime + Game.game_work.dt * 5f);
		}
		else
		{
			m_fadeTime = Math.MAX(0f, m_fadeTime - Game.game_work.dt * 5f);
		}
		m_newFade = Math.CLAMP(m_newFade + Game.game_work.dt * (m_info.hasBeenSeen ? (-5f) : 5f), 0f, 1f);
		m_selectedFade = Math.CLAMP(m_selectedFade + Game.game_work.dt * (ItemManager.GetInstance().IsEquipped(m_info) ? 5f : (-5f)), 0f, 1f);
	}

	public void ButtonClicked()
	{
		if (m_shop != null)
		{
			m_shop.SetSelected(this);
		}
	}

	public override void Draw()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		if (isFirst)
		{
			lastDrawnType = -1;
		}
		if (m_isOnScreen)
		{
			string[] array = new string[2]
			{
				TheGame.instance.stringTable.GetString(233),
				TheGame.instance.stringTable.GetString(232)
			};
			Font pGameFont = Game.game_work.pGameFont;
			Color white = Color.White;
			if (m_info.IsLocked())
			{
				white = new Color(200, 200, 200, 255);
			}
			Vector3 val = m_pos + m_textOffset;
			float num = pGameFont.MeasureString(m_info.shopTitle) * 25f * ShopScreen.SHOP_TEXT_SCALE;
			pGameFont.DrawString(TheGame.instance.stringTable.GetString(m_info.shopTitle), val + new Vector3(4f, -4f, 0f), new Color(0, 0, 0, 64), 25f * ShopScreen.SHOP_TEXT_SCALE, Vector2.Zero, (ALIGNMENT_TYPE)14);
			pGameFont.DrawString(TheGame.instance.stringTable.GetString(m_info.shopTitle), val, white, 25f * ShopScreen.SHOP_TEXT_SCALE, Vector2.Zero, (ALIGNMENT_TYPE)14);
			float num2 = 20f * ShopScreen.SHOP_TEXT_SCALE;
			val.Y -= 26f;
			for (int i = 0; i < array.Length; i++)
			{
				typeNameLengths[i] = pGameFont.MeasureString(array[i]) * num2;
			}
			string text = array[(int)m_info.type];
			if (text != null)
			{
				pGameFont.DrawString(text, val + new Vector3(4f, -4f, 0f), new Color(0, 0, 0, 64), num2, Vector2.Zero, (ALIGNMENT_TYPE)14);
				pGameFont.DrawString(text, val, white, num2, Vector2.Zero, (ALIGNMENT_TYPE)14);
			}
			Matrix val2;
			if (m_newFade > 0f)
			{
				val2 = Matrix.CreateScale(new Vector3(65f, 33f, 0f) * m_newFade * m_newFade);
				val2 *= Matrix.CreateTranslation(new Vector3((float)(int)(val.X - num - 4f), (float)(int)(val.Y + 34f + newItemBob), 0f));
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().SetMatrix(val2);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				newItemTexture.Set();
				Mesh.DrawQuad(Color.White);
				newItemTexture.UnSet();
			}
			if (m_selectedFade > 0f)
			{
				val2 = Matrix.CreateScale(new Vector3(65f, 33f, 0f) * m_selectedFade * m_selectedFade);
				val2 *= Matrix.CreateTranslation(new Vector3((float)(int)(val.X - typeNameLengths[(int)m_info.type] - 32f), val.Y, 0f));
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().SetMatrix(val2);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				selectedSmallTexture.Set();
				Mesh.DrawQuad(Color.White);
				selectedSmallTexture.UnSet();
			}
			if (m_texture != null)
			{
				val2 = Matrix.CreateScale(new Vector3(64f, 64f, 0f));
				val2 *= Matrix.CreateTranslation(m_texturePos);
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().SetMatrix(val2);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				if (m_info.IsLocked())
				{
					lockedStrokeTexture.Set();
					Mesh.DrawQuad(Color.White);
					lockedStrokeTexture.UnSet();
				}
				else
				{
					m_texture.Set();
					Mesh.DrawQuad(Color.White);
					m_texture.UnSet();
				}
			}
			val2 = Matrix.CreateScale(new Vector3(257f, 17f, 0f));
			val2 *= Matrix.CreateTranslation(m_pos + Vector3.UnitY * m_height / 2f);
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().SetMatrix(val2);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			dividerTex.Set();
			if (lastDrawnType != (int)m_info.type)
			{
				lastDrawnType = (int)m_info.type;
				Mesh.DrawQuad(new Color(128, 128, 128, 255));
			}
			else
			{
				Mesh.DrawQuad(new Color(255, 255, 255, 200));
			}
			dividerTex.UnSet();
			if (isLast)
			{
				val2 = Matrix.CreateScale(new Vector3(257f, 17f, 0f));
				val2 *= Matrix.CreateTranslation(m_pos - Vector3.UnitY * m_height / 2f);
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().SetMatrix(val2);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				dividerTex.Set();
				Mesh.DrawQuad(new Color(128, 128, 128, 255));
				dividerTex.UnSet();
			}
		}
		float v = m_pos.Y;
		if (isFirst)
		{
			v = Math.MAX(v, 0f);
		}
		if (isLast)
		{
			v = Math.MIN(v, 0f);
		}
		int num3 = Math.CLAMP((int)(255f * m_fadeTime), 0, 255);
		if (m_shop != null && m_info != null && num3 > 0)
		{
			Game.game_work.pGameFont.DrawString(m_description, m_shop.GetDescriptionTextXPos(), 0f, 0f, m_info.IsLocked() ? new Color(255, 255, 255, num3) : new Color(116, 93, 59, num3), 18f * ShopScreen.SHOP_TEXT_SCALE, ShopScreen.DESCRIPTION_BOX_WIDTH, 0f, ALIGNMENT_TYPE.ALIGN_CENTER);
		}
		if (isLast)
		{
			blankTex.Set();
			Matrix val3 = Matrix.CreateScale(new Vector3(ShopScreen.SHOP_BACK_LEFT_SIDE, ShopScreen.DARKNESS_HEIGHT, 0f));
			val3 *= Matrix.CreateTranslation(new Vector3(m_parentList.m_pos.X - 2f, Game.SCREEN_HEIGHT / 2f - ShopScreen.DARKNESS_HEIGHT / 2f + 5f, 0f));
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().SetMatrix(val3);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(new Color(0, 0, 0, 128));
			val3 = Matrix.CreateScale(new Vector3(ShopScreen.SHOP_BACK_LEFT_SIDE, ShopScreen.DARKNESS_HEIGHT, 0f));
			val3 *= Matrix.CreateTranslation(new Vector3(m_parentList.m_pos.X - 2f, (0f - Game.SCREEN_HEIGHT) / 2f + ShopScreen.DARKNESS_HEIGHT / 2f - 5f, 0f));
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().SetMatrix(val3);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(new Color(0, 0, 0, 128));
			blankTex.UnSet();
		}
	}

	static ShopListItem()
	{
		float[] array = new float[2];
		typeNameLengths = array;
	}
}
