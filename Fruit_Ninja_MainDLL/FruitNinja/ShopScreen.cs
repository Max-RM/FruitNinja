using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ShopScreen : HUDControl3d
{
	public enum SS
	{
		SS_IN,
		SS_WAIT,
		SS_OUT,
		SS_CONFIRM_IN,
		SS_CONFIRM,
		SS_CONFIRM_OUT,
		SS_CONFIRM_CANCELLED
	}

	protected float m_time;

	protected int m_backDrawOrder;

	protected MenuButton m_quitButton;

	protected float m_buyButtonWait;

	protected MenuButton m_buyButton;

	protected DojoScreen m_dojoScreen;

	protected ScrollingMenu m_scrollingMenu;

	protected ShopListItem m_selectedItem;

	protected ShopListItem[] m_equipedListItems = new ShopListItem[2];

	protected float coins;

	protected ItemInfo m_previousItem;

	protected int m_selectedScaleInOut;

	public static float ScrollOffset;

	public int m_state;

	private static bool hackedOpen = false;

	private static int c = 0;

	private static float m_fadeTime = 0f;

	public static float SHOP_SCREEN_HEIGHT => 320f;

	public static float SHOP_BACK_LEFT_SIDE => 290f;

	public static float SHOP_BACK_RIGHT_SIDE => Game.SCREEN_WIDTH - 290f;

	public static float SHOP_LIST_FROM_LEFT => SHOP_BACK_LEFT_SIDE / 2f;

	public static float SHOP_LIST_POS_X => (0f - Game.SCREEN_WIDTH) / 2f + SHOP_LIST_FROM_LEFT;

	public static float QUIT_Y => 0f - Game.SCREEN_HEIGHT / 2f + 55f;

	public static float WATERMELON_SELECT_SCALE => 0.75f;

	public static float DESCRIPTION_BOX_WIDTH => 160f;

	public static float POP_IN_OUT_TIME => 0.15f;

	public static float POP_SINE => (int)Math.DEGREE_TO_IDX(90f);

	public static float SHAKE_TIME => 0.25f;

	public static float SHAKE_RANGE => 5f;

	public static float BUY_NOW_X => 50f;

	public static float BUY_NOW_Y => Game.SCREEN_HEIGHT / 2f - 56f;

	public static float DARKNESS_HEIGHT => 120f;

	public static float SHOP_TEXT_SCALE => 0.75f;

	public static float SHOP_ITEM_HEIGHT => 80f;

	public static float ITEM_ICON_SCALE => 1f;

	public ShopScreen(DojoScreen dojoScreen)
	{
		if (!ShopListItem.s_isContentLoaded)
		{
			LoadContent();
		}
		m_buyButton = null;
		m_deleteCall = delegate(HUDControl l)
		{
			((ShopScreen)l).Release();
		};
		m_dojoScreen = dojoScreen;
		m_texture = null;
		m_selectedItem = null;
		m_scrollingMenu = null;
		m_selfCleanUp = false;
		m_quitButton = null;
		m_state = 0;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		m_time = 0f;
		m_buyButtonWait = 0f;
		m_selectedScaleInOut = 0;
		coins = (float)Game.game_work.coins + 0.5f;
	}

	public void QuitShopCallback()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		SoundManager.GetInstance().SFXPlay(SoundDef.SND_MENU_BOMB);
		m_state = 2;
		((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
		m_quitButton.m_entity.m_vel = new Vector3(Math.g_random.RandF(5f) + 5f, 0f - Math.g_random.RandF(5f), 0f);
		Game.game_work.tutorialControl.ResetTutePos();
	}

	public void BuyButtonCallback()
	{
		if (m_selectedItem == null || m_selectedItem.m_info == null)
		{
			return;
		}
		ItemType type = m_selectedItem.m_info.type;
		if (m_selectedItem.m_info.IsLocked())
		{
			ItemManager.GetInstance().BuyItem(m_selectedItem.m_info.nameHash);
			m_selectedItem.m_text = "BOUGHT";
			return;
		}
		if (ItemManager.GetInstance().IsEquipped(m_selectedItem.m_info))
		{
			ItemManager.GetInstance().SetEquippedItem(type, null);
			if (m_selectedItem.m_info.cost == 0)
			{
				m_selectedItem.m_text = "FREE";
			}
			else
			{
				m_selectedItem.m_text = "BOUGHT";
			}
			m_equipedListItems[(int)type] = null;
			return;
		}
		if (m_equipedListItems[(int)type] != null)
		{
			if (m_equipedListItems[(int)type].m_info.cost == 0)
			{
				m_equipedListItems[(int)type].m_text = "FREE";
			}
			else
			{
				m_equipedListItems[(int)type].m_text = "BOUGHT";
			}
		}
		m_equipedListItems[(int)type] = m_selectedItem;
		ItemManager.GetInstance().SetEquippedItem(type, m_selectedItem.m_info);
		m_selectedItem.m_text = "EQUIPPED";
	}

	public void EquipCallback()
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (m_buyButton != null && !hackedOpen)
		{
			m_buyButtonWait = 0.25f;
			if (m_selectedItem != null && m_selectedItem.m_info != null)
			{
				ItemManager.GetInstance().SetEquippedItem(m_selectedItem.m_info.type, m_selectedItem.m_info);
				if (m_selectedItem.m_info.type == ItemType.ITEM_SLASH_MODIFIER)
				{
					SoundManager.GetInstance().SFXPlay(SoundDef.SND_EQUIP_SLASH);
				}
				else if (m_selectedItem.m_info.type == ItemType.ITEM_BACKGROUND)
				{
					SoundManager.GetInstance().SFXPlay(SoundDef.SND_EQUIP_BACKGROUND);
				}
			}
		}
		else if (m_buyButton != null && m_buyButton.m_entity != null)
		{
			((Fruit)m_buyButton.m_entity).m_pos2 = ((Fruit)m_buyButton.m_entity).m_pos;
			((Fruit)m_buyButton.m_entity).m_vel = Vector3.Zero;
			((Fruit)m_buyButton.m_entity).m_vel2 = Vector3.Zero;
			((Fruit)m_buyButton.m_entity).m_gravity = Vector3.Zero;
		}
	}

	public void ConfirmCallback()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (m_selectedItem != null && m_selectedItem.m_info != null)
		{
			m_equipedListItems[(int)m_selectedItem.m_info.type] = m_selectedItem;
		}
		m_state = 5;
		if (m_quitButton != null && m_quitButton.m_entity != null)
		{
			((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
			m_quitButton.m_entity.m_vel = new Vector3(Math.g_random.RandF(5f) + 5f, 0f - Math.g_random.RandF(5f), 0f);
			Game.game_work.tutorialControl.ResetTutePos();
		}
	}

	public void CancelCallback()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_state = 6;
		if (m_quitButton != null && m_quitButton.m_entity != null)
		{
			((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
			m_quitButton.m_entity.m_vel = new Vector3(Math.g_random.RandF(5f) + 5f, 0f - Math.g_random.RandF(5f), 0f);
			Game.game_work.tutorialControl.ResetTutePos();
		}
	}

	public static void LoadContent()
	{
		ShopListItem.blankTex = TextureManager.GetInstance().Load("textureswp7/loading.tex");
		ShopListItem.dividerTex = TextureManager.GetInstance().Load("textureswp7/scratch_deviders.tex");
		ShopListItem.descriptionBox = TextureManager.GetInstance().Load("textureswp7/dialog_box_shop.tex");
		ShopListItem.buyNowTexture = TextureManager.GetInstance().Load("locked.tex", localise: true);
		ShopListItem.equipNowTexture = TextureManager.GetInstance().Load("select_item.tex", localise: true);
		ShopListItem.selectedTexture = TextureManager.GetInstance().Load("selected.tex", localise: true);
		ShopListItem.selectedSmallTexture = TextureManager.GetInstance().Load("selected_sml.tex", localise: true);
		ShopListItem.lockedStrokeTexture = TextureManager.GetInstance().Load("textureswp7/locked_stroke.tex");
		ShopListItem.newItemTexture = TextureManager.GetInstance().Load("new_item_sml.tex", localise: true);
		ShopListItem.backGround = TextureManager.GetInstance().Load("textureswp7/BG_store.tex");
		ShopListItem.s_isContentLoaded = true;
	}

	public static void UnLoadContent()
	{
		ShopListItem.blankTex = null;
		ShopListItem.dividerTex = null;
		ShopListItem.descriptionBox = null;
		ShopListItem.backGround = null;
		ShopListItem.buyNowTexture = null;
		ShopListItem.equipNowTexture = null;
		ShopListItem.unequipNowTexture = null;
		ShopListItem.coinsBuyTexture = null;
		ShopListItem.coinsSpendTexture = null;
		ShopListItem.swipe_to_scroll = null;
		ShopListItem.selectedTexture = null;
		ShopListItem.selectedSmallTexture = null;
		ShopListItem.lockedStrokeTexture = null;
		ShopListItem.newStrokeTexture = null;
		ShopListItem.newItemTexture = null;
	}

	public void DeletedMenuItem(HUDControl control)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (m_buyButton == control)
		{
			if (hackedOpen && m_buyButton.m_entity != null)
			{
				m_buyButton.m_entity.m_pos.Y = 0f - Game.SCREEN_WIDTH;
				((Fruit)m_buyButton.m_entity).m_pos2.Y = 0f - Game.SCREEN_WIDTH;
				((Fruit)m_buyButton.m_entity).m_gravity = -Vector3.UnitY;
				((Fruit)m_buyButton.m_entity).m_vel.Y = -10f;
				((Fruit)m_buyButton.m_entity).m_vel2.Y = -10f;
			}
			m_buyButtonWait += 0.05f;
			m_buyButton = null;
		}
		if (m_quitButton == control)
		{
			m_quitButton = null;
		}
	}

	public void SetSelected(ShopListItem item)
	{
		m_selectedItem = item;
		if (!m_scrollingMenu.IsLockedIn() || m_buyButton == null || m_selectedItem.m_info == null || m_state != 1)
		{
			return;
		}
		int fruitType = Fruit.FruitType("pineapple");
		int fruitType2 = Fruit.FruitType("black_pineapple");
		if (m_selectedItem.m_info.IsLocked())
		{
			m_buyButton.m_texture = ShopListItem.buyNowTexture;
			if (m_buyButton.m_entity != null)
			{
				((Fruit)m_buyButton.m_entity).SetFruitType(fruitType2);
			}
		}
		else
		{
			m_buyButton.m_texture = ShopListItem.equipNowTexture;
			if (m_buyButton.m_entity != null)
			{
				((Fruit)m_buyButton.m_entity).SetFruitType(fruitType);
			}
		}
	}

	public ShopListItem GetSelected(ItemType type)
	{
		return m_equipedListItems[(int)type];
	}

	public ShopListItem GetSelected()
	{
		return m_selectedItem;
	}

	public override void Reset()
	{
	}

	public override void Release()
	{
		if (m_scrollingMenu != null)
		{
			Game.game_work.hud.RemoveControl(m_scrollingMenu);
			Delete.SAFE_DELETE(ref m_scrollingMenu);
		}
		m_texture = null;
		m_time = 0f;
	}

	public override void Init()
	{
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		m_selectedItem = null;
		for (int i = 0; i < 2; i++)
		{
			m_equipedListItems[i] = null;
		}
		m_backDrawOrder = 3;
		m_scrollingMenu = new ScrollingMenu();
		m_scrollingMenu.SetWidth(290f);
		m_scrollingMenu.SetHeight(SHOP_ITEM_HEIGHT);
		m_scrollingMenu.SetItemHeight(SHOP_ITEM_HEIGHT);
		Game.game_work.hud.AddControl(m_scrollingMenu);
		int it = 0;
		ItemInfo itemInfo = ItemManager.GetInstance().GetFirst(ref it);
		bool flag = true;
		bool flag2 = true;
		ShopListItem shopListItem = null;
		float num = 0f;
		float num2 = 0f;
		while (itemInfo != null)
		{
			ShopListItem shopListItem2 = new ShopListItem();
			shopListItem2.Create(itemInfo, this);
			shopListItem2.m_backing = flag;
			flag = !flag;
			m_scrollingMenu.AddItem(shopListItem2);
			shopListItem2.SetClickedFocusedCallback(ClickedOnShopItem);
			if (ItemManager.GetInstance().IsEquipped(itemInfo))
			{
				m_equipedListItems[(int)itemInfo.type] = shopListItem2;
			}
			else if (!itemInfo.hasBeenSeen && num == 0f)
			{
				num = num2;
			}
			if (flag2)
			{
				SetSelected(shopListItem2);
				shopListItem2.isFirst = true;
			}
			flag2 = false;
			num2 += shopListItem2.GetHeight();
			shopListItem = shopListItem2;
			itemInfo = ItemManager.GetInstance().GetNext(ref it);
		}
		if (ScrollOffset > 0f)
		{
			ScrollOffset = 0f - num;
		}
		shopListItem.isLast = true;
		m_scrollingMenu.m_pos = new Vector3(SHOP_LIST_POS_X - SHOP_BACK_LEFT_SIDE * 1.5f, SHOP_ITEM_HEIGHT / 2f, 0f);
		m_scrollingMenu.Init();
		m_scrollingMenu.m_offset.Y = ScrollOffset;
		m_scrollingMenu.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		m_scrollingMenu.Update(0f);
	}

	public override void Update(float dt)
	{
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		float time = m_time;
		if (SplatEntity.NumActiveSplats() == 0)
		{
			m_backDrawOrder = 2;
		}
		if (m_scrollingMenu != null && m_selectedItem != m_scrollingMenu.GetItemClosestToZero() && c == 0)
		{
			SetSelected((ShopListItem)m_scrollingMenu.GetItemClosestToZero());
		}
		c = (c + 1) % 10;
		m_drawOrder = (HUD.HUD_ORDER)m_backDrawOrder;
		switch ((SS)m_state)
		{
		case SS.SS_IN:
			m_time += (1f - m_time) * 0.125f;
			if (m_time > 0.999f)
			{
				SplatEntity.RemoveAllSplats();
				m_buyButtonWait = 0f;
				m_time = 1f;
				m_state = 1;
				if (m_quitButton == null)
				{
					m_quitButton = new MenuButton(Game.game_work.backTexture, new Vector3(Game.SCREEN_WIDTH / 2f - 55f, QUIT_Y, 0f), QuitShopCallback, Fruit.MAX_FRUIT_TYPES);
					m_quitButton.Init();
					m_quitButton.m_triggerOnBackPress = true;
					Game.game_work.hud.AddControl(m_quitButton);
					m_quitButton.m_deleteCall = DeletedMenuItem;
					Game.game_work.tutorialControl.ResetTutePos(m_quitButton);
					MenuButton quitButton = m_quitButton;
					quitButton.m_originalScale *= 0.825f;
					Entity entity2 = m_quitButton.m_entity;
					entity2.m_cur_scale *= 0.825f;
				}
			}
			break;
		case SS.SS_WAIT:
		{
			float num = 0f - dt;
			if (m_buyButtonWait > 0f)
			{
				m_buyButtonWait -= dt;
			}
			else if (m_scrollingMenu.IsLockedIn())
			{
				if (m_selectedItem != null && m_selectedItem.m_info != null && (ItemManager.GetInstance().IsEquipped(m_selectedItem.m_info) || m_selectedItem.m_info.IsLocked()))
				{
					Game.game_work.tutorialControl.ResetTutePos();
				}
				if (m_selectedItem != null && ItemManager.GetInstance().IsEquipped(m_selectedItem.m_info))
				{
					num = dt;
				}
				else if (m_buyButton == null)
				{
					m_buyButton = new MenuButton(ShopListItem.buyNowTexture, new Vector3(Game.SCREEN_WIDTH / 2f - SHOP_BACK_RIGHT_SIDE / 2f, BUY_NOW_Y, 0f), EquipCallback, Fruit.FruitType("watermelon"));
					SetSelected(m_selectedItem);
					m_buyButton.Init();
					Game.game_work.hud.AddControl(m_buyButton);
					m_buyButton.m_clearOthers = false;
					hackedOpen = false;
					m_buyButton.m_deleteCall = DeletedMenuItem;
					Game.game_work.tutorialControl.ResetTutePos(m_buyButton);
					MenuButton buyButton = m_buyButton;
					buyButton.m_originalScale *= WATERMELON_SELECT_SCALE;
					Entity entity = m_buyButton.m_entity;
					entity.m_cur_scale *= WATERMELON_SELECT_SCALE;
					float num2 = (Math.g_random.RandF(2f) + 2f) * (float)((Math.g_random.Rand32(2) == 0) ? 1 : (-1));
					for (int i = 0; i < 2; i++)
					{
						Fruit.RandomStartAngle(ref ((Fruit)m_buyButton.m_entity).m_rotation_piece[i], faceUp: true);
						ref Vector3 reference = ref ((Fruit)m_buyButton.m_entity).m_rotation_speed[i];
						reference = new Vector3(0f, 0f, num2);
					}
				}
			}
			else if (m_buyButton != null && m_buyButton.m_entity != null && !((Fruit)m_buyButton.m_entity).Sliced() && m_buyButton != null)
			{
				((Fruit)m_buyButton.m_entity).m_isSliced = true;
				hackedOpen = true;
				m_buyButton.m_clearOthers = false;
				((Fruit)m_buyButton.m_entity).m_vel2 = Vector3.One;
			}
			m_selectedScaleInOut = (int)Math.CLAMP((float)m_selectedScaleInOut + num * (POP_SINE / POP_IN_OUT_TIME), 0f, POP_SINE);
			break;
		}
		case SS.SS_OUT:
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				m_dojoScreen.Reset();
				m_terminate = true;
			}
			break;
		case SS.SS_CONFIRM_IN:
			m_time *= 0.75f;
			if (m_time < 0.001f)
			{
				m_state = 4;
				m_time = 0f;
				m_quitButton = new MenuButton(Game.game_work.backTexture, new Vector3(Game.SCREEN_WIDTH / 2f - 55f, QUIT_Y, 0f), ConfirmCallback, Fruit.MAX_FRUIT_TYPES);
				m_quitButton.Init();
				m_quitButton.m_triggerOnBackPress = true;
				m_quitButton.m_deleteCall = DeletedMenuItem;
				Game.game_work.hud.AddControl(m_quitButton);
				MenuButton quitButton2 = m_quitButton;
				quitButton2.m_originalScale *= 0.825f;
				Entity entity3 = m_quitButton.m_entity;
				entity3.m_cur_scale *= 0.825f;
			}
			else if (m_quitButton != null && m_quitButton.m_entity != null)
			{
				((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
				m_quitButton.m_entity.m_vel = new Vector3(Math.g_random.RandF(5f) + 5f, 0f - Math.g_random.RandF(5f), 0f);
				Game.game_work.tutorialControl.ResetTutePos();
				m_quitButton = null;
			}
			break;
		case SS.SS_CONFIRM:
			m_backDrawOrder = 3;
			break;
		case SS.SS_CONFIRM_OUT:
		case SS.SS_CONFIRM_CANCELLED:
			if (m_quitButton != null && m_quitButton.m_entity != null)
			{
				((Bomb)m_quitButton.m_entity).EnableGravity(state: true);
				m_quitButton.m_entity.m_vel = new Vector3(Math.g_random.RandF(5f) + 5f, 0f - Math.g_random.RandF(5f), 0f);
				Game.game_work.tutorialControl.ResetTutePos();
				m_quitButton = null;
			}
			if (ActorManager.GetInstance().GetNumEntities(1) == 0 && ActorManager.GetInstance().GetNumEntities(0) == 0)
			{
				ItemType itemType = ((m_selectedItem != null && m_selectedItem.m_info != null) ? m_selectedItem.m_info.type : ItemType.ITEM_TYPE_MAX);
				if (itemType < ItemType.ITEM_TYPE_MAX && m_equipedListItems[(int)m_selectedItem.m_info.type] != m_selectedItem)
				{
					ItemManager.GetInstance().SetEquippedItem(itemType, (m_equipedListItems[(int)itemType] != null) ? m_equipedListItems[(int)itemType].m_info : null);
				}
				m_state = 0;
			}
			break;
		}
		if (m_scrollingMenu != null)
		{
			m_scrollingMenu.m_pos = new Vector3(SHOP_LIST_POS_X - (1f - m_time) * SHOP_BACK_LEFT_SIDE * 1.5f, SHOP_ITEM_HEIGHT / 2f, 0f);
			ScrollOffset = m_scrollingMenu.m_offset.Y;
		}
		if (!(m_time < time))
		{
			return;
		}
		float num3 = (1f - m_time - (1f - time)) * SHOP_BACK_RIGHT_SIDE * 1.5f;
		float num4 = (1f - m_time - (1f - time)) * SHOP_BACK_LEFT_SIDE * 1.5f;
		for (int j = 0; j < SplatEntity.poolCount; j++)
		{
			SplatEntity splatEntity = SplatEntity.pool[j];
			if (splatEntity.m_update && splatEntity.m_onWall >= 0)
			{
				if (splatEntity.m_pos.X > Game.SCREEN_WIDTH / 2f - SHOP_BACK_RIGHT_SIDE)
				{
					ref Vector3 pos = ref splatEntity.m_pos;
					pos.X += num3;
				}
				else
				{
					ref Vector3 pos2 = ref splatEntity.m_pos;
					pos2.X -= num4;
				}
			}
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		float num = Game.SCREEN_WIDTH / 2f - SHOP_BACK_RIGHT_SIDE / 2f;
		if (m_drawOrder != (HUD.HUD_ORDER)m_backDrawOrder)
		{
			if (m_selectedScaleInOut > 0)
			{
				if (m_time < 1f)
				{
					num += (1f - m_time) * SHOP_BACK_RIGHT_SIDE * 1.5f;
				}
				Vector3 val = new Vector3((float)(ShopListItem.selectedTexture.GetWidth() + 1), (float)(ShopListItem.selectedTexture.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX;
				Vector3 val2 = val;
				if ((float)m_selectedScaleInOut < POP_SINE)
				{
					val2 *= Math.SinIdx((ushort)m_selectedScaleInOut) / Math.SinIdx((ushort)POP_SINE);
				}
				Matrix val3 = Matrix.CreateScale(val2);
				val3 *= Matrix.CreateTranslation(new Vector3(num, BUY_NOW_Y, 0f));
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().SetMatrix(val3);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				ShopListItem.selectedTexture.Set();
				Mesh.DrawQuad(Color.White);
				ShopListItem.selectedTexture.UnSet();
			}
			return;
		}
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
		if (m_time < 1f)
		{
			num += (1f - m_time) * SHOP_BACK_RIGHT_SIDE * 1.5f;
			ShopListItem.backGround.Set();
			Matrix val4 = Matrix.CreateScale(new Vector3(SHOP_BACK_LEFT_SIDE + 1f, Game.SCREEN_HEIGHT + 1f, 0f));
			val4 *= Matrix.CreateTranslation(new Vector3(m_scrollingMenu.m_pos.X, 0f, 0f));
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().SetMatrix(val4);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, (512f - Game.SCREEN_WIDTH) / 2f / 512f, (SHOP_BACK_LEFT_SIDE + (512f - Game.SCREEN_WIDTH) / 2f) / 512f, (512f - Game.SCREEN_HEIGHT) / 2f / 512f, 1f - (512f - Game.SCREEN_HEIGHT) / 2f / 512f);
			Math.Scale44(new Vector3(SHOP_BACK_RIGHT_SIDE + 1f, Game.SCREEN_HEIGHT + 1f, 0f), out val4);
			Vector3 scl = default(Vector3);
			scl = new Vector3(num, 0f, 0f);
			Math.GlobalTranslate44(ref val4, scl);
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().SetMatrix(val4);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			Mesh.DrawQuad(Color.White, (SHOP_BACK_LEFT_SIDE + (512f - Game.SCREEN_WIDTH) / 2f) / 512f, 1f - (512f - Game.SCREEN_WIDTH) / 2f / 512f, (512f - Game.SCREEN_HEIGHT) / 2f / 512f, 1f - (512f - Game.SCREEN_HEIGHT) / 2f / 512f);
			ShopListItem.backGround.UnSet();
		}
		else
		{
			Math.Scale44(new Vector3(Game.SCREEN_WIDTH + 1f, Game.SCREEN_HEIGHT + 1f, 0f), out var mtx);
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().SetMatrix(mtx);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			ShopListItem.backGround.Set();
			Mesh.DrawQuad(Color.White, (512f - Game.SCREEN_WIDTH) / 2f / 512f, 1f - (512f - Game.SCREEN_WIDTH) / 2f / 512f, (512f - Game.SCREEN_HEIGHT) / 2f / 512f, 1f - (512f - Game.SCREEN_HEIGHT) / 2f / 512f);
			ShopListItem.backGround.UnSet();
		}
		Math.Scale44(new Vector3((float)(ShopListItem.descriptionBox.GetWidth() + 1), (float)(ShopListItem.descriptionBox.GetHeight() + 1), 0f) * Game.GAME_MODE_SCALE_FIX, out var mtx2);
		Math.GlobalTranslate44(ref mtx2, num - 4f, -3f, 0f);
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().SetMatrix(mtx2);
		MatrixManager.GetInstance().UploadCurrentMatrices();
		byte b = 0;
		if (m_selectedItem.m_info.IsLocked())
		{
			m_fadeTime = Math.MIN(1f, m_fadeTime + Game.game_work.dt * 5f);
			b = (byte)(255f - m_fadeTime * 120f);
		}
		else
		{
			m_fadeTime = Math.MAX(0f, m_fadeTime - Game.game_work.dt * 5f);
			b = (byte)(255f - m_fadeTime * 120f);
		}
		Color col = default(Color);
		col = new Color((int)b, (int)b, (int)b, 255);
		ShopListItem.descriptionBox.Set();
		Mesh.DrawQuad(col);
		ShopListItem.descriptionBox.UnSet();
	}

	public float GetDescriptionTextXPos()
	{
		float num = Game.SCREEN_WIDTH / 2f - SHOP_BACK_RIGHT_SIDE / 2f;
		if (m_time < 1f)
		{
			num += (1f - m_time) * SHOP_BACK_RIGHT_SIDE * 1.5f;
		}
		return num - DESCRIPTION_BOX_WIDTH / 2f;
	}

	public void ClickedOnShopItem(ScrollingMenuItem item)
	{
		if (((ShopListItem)item).m_info != null && !((ShopListItem)item).m_info.IsLocked())
		{
			if (m_buyButton != null)
			{
				Game.game_work.tutorialControl.ButtonPressedAtPos(m_buyButton);
			}
		}
		else
		{
			SoundManager.GetInstance().SFXPlay("equip-locked");
			((ShopListItem)item).m_shakeTime = SHAKE_TIME;
		}
	}

	public static void NewItem()
	{
		ScrollOffset = 1f;
	}
}
