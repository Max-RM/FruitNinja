using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using FruitNinja.Port;
using Mortar;

namespace FruitNinja;

internal class ItemManager
{
	private static ItemManager instance;

	private ItemInfo[] equipedItems = new ItemInfo[2];

	private List<ItemInfo> itemInfoOrderedList = new List<ItemInfo>();

	private Dictionary<uint, ItemInfo> itemInfoList = new Dictionary<uint, ItemInfo>();

	private Dictionary<uint, ItemInfo>[] itemInfoTypeList = ArrayInit.CreateFilledArray<Dictionary<uint, ItemInfo>>(2);

	private static uint[] itemTypeHashes = new uint[2]
	{
		StringFunctions.StringHash("SLASH_MODIFIER"),
		StringFunctions.StringHash("BACKGROUND")
	};

	private static string SAVE_DIRECTORY = "FruitNinja";

	private static string SAVE_FILENAME = SAVE_DIRECTORY + "/itemsinfo.xml";

	private static int funcCalls = 2;

	public static ItemManager GetInstance()
	{
		if (instance == null)
		{
			instance = new ItemManager();
		}
		return instance;
	}

	private ItemManager()
	{
		for (int i = 0; i < 2; i++)
		{
			equipedItems[i] = null;
		}
	}

	public int GetNumNewItems()
	{
		int num = 0;
		foreach (ItemInfo itemInfoOrdered in itemInfoOrderedList)
		{
			if (!itemInfoOrdered.hasBeenSeen)
			{
				num++;
			}
		}
		return num;
	}

	public bool AreNewItems()
	{
		foreach (ItemInfo itemInfoOrdered in itemInfoOrderedList)
		{
			if (!itemInfoOrdered.hasBeenSeen)
			{
				return true;
			}
		}
		return false;
	}

	public static ItemType ParseItemType(string text)
	{
		if (text != null)
		{
			uint num = StringFunctions.StringHash(text);
			for (int i = 0; i < 2; i++)
			{
				if (num == itemTypeHashes[i])
				{
					return (ItemType)i;
				}
			}
		}
		return ItemType.ITEM_NONE;
	}

	public void LoadItemData()
	{
		try
		{
			XDocument dl = MortarXml.Load("xmlwp7/itemList.xml");
			itemInfoList.Clear();
			itemInfoOrderedList.Clear();
			for (int i = 0; i < 2; i++)
			{
				equipedItems[i] = null;
			}
			if (dl != null)
			{
				if (true)
				{
					XElement element = dl.FirstChildElement("itemManagerFile");
					for (XElement xElement = element.FirstChildElement("item"); xElement != null; xElement = xElement.NextSiblingElement("item"))
					{
						ItemInfo itemInfo = null;
						ItemType itemType = ParseItemType(xElement.AttributeStr("type"));
						if (itemType == ItemType.ITEM_SLASH_MODIFIER)
						{
							itemInfo = new SlashModInfo();
							itemInfo.type = itemType;
						}
						else
						{
							itemInfo = new ItemInfo();
							itemInfo.type = itemType;
						}
						itemInfo.Parse(xElement);
						if (Game.game_work.saveData.IsAchievementUnlocked(itemInfo.nameHash))
						{
							itemInfo.cost = -1;
						}
						else if (!AchievementManager.GetInstance().AchievementExists(itemInfo.nameHash) && itemInfo.cost > 0)
						{
							itemInfo.cost = -1;
							itemInfo.hasBeenSeen = false;
							ShopScreen.NewItem();
						}
						itemInfoOrderedList.Add(itemInfo);
						itemInfoList[itemInfo.nameHash] = itemInfo;
						itemInfoTypeList[(int)itemInfo.type][itemInfo.nameHash] = itemInfo;
						if (equipedItems[(int)itemInfo.type] == null)
						{
							equipedItems[(int)itemInfo.type] = itemInfo;
						}
					}
				}
				Delete.SAFE_DELETE(ref dl);
			}
		}
		catch
		{
		}
		try
		{
			if (DesktopStorage.FileExists(SAVE_FILENAME))
			{
				byte[] array = DesktopStorage.ReadAllBytes(SAVE_FILENAME);
				if (array != null && array.Length > 0)
				{
					string text = Encoding.UTF8.GetString(array, 0, array.Length);
					XDocument dl2 = XDocument.Parse(text);
					if (true)
					{
						XElement element2 = dl2.FirstChildElement("item_save_file");
						element2.QueryIntAttribute("coins", ref Game.game_work.coins);
						element2.QueryIntAttribute("coinsTotal", ref Game.game_work.coinsTotal);
						element2.QueryIntAttribute("levelStartCoins", ref Game.game_work.levelStartCoins);
						XElement xElement2 = element2.FirstChildElement("boughtItems");
						if (xElement2 != null)
						{
							for (XElement xElement3 = xElement2.FirstChildElement("item"); xElement3 != null; xElement3 = xElement3.NextSiblingElement("item"))
							{
								string text2 = xElement3.AttributeStr("name");
								if (text2 != null && itemInfoList.TryGetValue(StringFunctions.StringHash(text2), out var value))
								{
									value.cost = -1;
									string text3 = xElement3.AttributeStr("seen");
									value.hasBeenSeen = text3 != null && text3 == "true";
								}
							}
						}
						XElement xElement4 = element2.FirstChildElement("equippedItems");
						if (xElement4 != null)
						{
							for (XElement xElement5 = xElement4.FirstChildElement("item"); xElement5 != null; xElement5 = xElement5.NextSiblingElement("item"))
							{
								string text4 = xElement5.AttributeStr("name");
								if (text4 != null)
								{
									ItemInfo item = GetItem(StringFunctions.StringHash(text4));
									if (item != null)
									{
										equipedItems[(int)item.type] = item;
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
		for (int j = 0; j < 2; j++)
		{
			SetEquippedItem((ItemType)j, equipedItems[j]);
		}
	}

	public void UnLoadItemData()
	{
		itemInfoList.Clear();
	}

	public void SaveItemInfo()
	{
		try
		{
			if (!DesktopStorage.DirectoryExists("FruitNinja"))
			{
				DesktopStorage.CreateDirectory("FruitNinja");
			}
			XDocument xDocument = new XDocument();
			XDeclaration declaration = new XDeclaration("1.0", "utf-8", "yes");
			xDocument.Declaration = declaration;
			XElement xElement = new XElement("item_save_file");
			xDocument.Add(xElement);
			xElement.Add(new XAttribute("coins", Game.game_work.coins));
			xElement.Add(new XAttribute("coinsTotal", Game.game_work.coinsTotal));
			xElement.Add(new XAttribute("levelStartCoins", Game.game_work.levelStartCoins));
			if (itemInfoOrderedList.Count > 0)
			{
				XElement xElement2 = new XElement("boughtItems");
				foreach (ItemInfo itemInfoOrdered in itemInfoOrderedList)
				{
					if (itemInfoOrdered.cost < 0)
					{
						XElement xElement3 = new XElement("item");
						xElement3.SetAttribute("name", itemInfoOrdered.name);
						xElement3.SetAttribute("seen", itemInfoOrdered.hasBeenSeen ? "true" : "false");
						xElement2.LinkEndChild(xElement3);
					}
				}
				xElement.LinkEndChild(xElement2);
			}
			XElement xElement4 = new XElement("equippedItems");
			for (int i = 0; i < 2; i++)
			{
				ItemInfo itemInfo = equipedItems[i];
				if (itemInfo != null && itemInfo.cost <= 0)
				{
					XElement xElement5 = new XElement("item");
					xElement5.SetAttribute("name", itemInfo.name);
					xElement4.LinkEndChild(xElement5);
				}
			}
			xElement.LinkEndChild(xElement4);
			MemoryStream memoryStream = new MemoryStream();
			xDocument.Save(memoryStream);
			byte[] array = memoryStream.ToArray();
			Encoding.UTF8.GetString(array, 0, array.Length);
			using (var stream = DesktopStorage.OpenFile(SAVE_FILENAME, FileMode.Create))
			{
				stream.Write(array, 3, array.Length - 3);
			}
		}
		catch (Exception)
		{
		}
	}

	public bool IsEquipped(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		return equipedItems[(int)item.type] == item;
	}

	public ItemInfo GetEquippedItem(ItemType type)
	{
		return equipedItems[(int)type];
	}

	public void SetEquippedItem(ItemType type, ItemInfo item)
	{
		switch (type)
		{
		case ItemType.ITEM_SLASH_MODIFIER:
			if (item != null)
			{
				item.SetEquipped();
			}
			else
			{
				SlashEntity.InitModColors();
			}
			break;
		case ItemType.ITEM_BACKGROUND:
			if (funcCalls <= 0 || (funcCalls > 0 && GameTask.GetCurrentBackground() == null))
			{
				GameTask.ChangeBackground(item?.textureName);
			}
			break;
		}
		if (funcCalls > 0)
		{
			funcCalls--;
		}
		equipedItems[(int)type] = item;
		if (item != null)
		{
			item.hasBeenSeen = true;
		}
	}

	public ItemInfo GetItem(uint hash)
	{
		if (itemInfoList.TryGetValue(hash, out var value))
		{
			return value;
		}
		return null;
	}

	public bool UnlockItem(uint hash)
	{
		if (itemInfoList.TryGetValue(hash, out var value))
		{
			ItemInfo itemInfo = value;
			itemInfo.cost = -1;
			itemInfo.hasBeenSeen = false;
			return true;
		}
		return false;
	}

	public bool BuyItem(uint hash)
	{
		if (itemInfoList.TryGetValue(hash, out var value))
		{
			ItemInfo itemInfo = value;
			if (itemInfo.cost >= 0 && Game.game_work.coins >= itemInfo.cost)
			{
				Game.AddCoins(-itemInfo.cost);
				itemInfo.cost = -1;
				return true;
			}
		}
		return false;
	}

	public bool EquipItem(uint hash)
	{
		if (itemInfoList.TryGetValue(hash, out var value))
		{
			ItemInfo itemInfo = value;
			if (itemInfo.cost < 0)
			{
				SetEquippedItem(itemInfo.type, itemInfo);
				return true;
			}
		}
		return false;
	}

	public bool UnequipItem(uint hash)
	{
		if (itemInfoList.TryGetValue(hash, out var value))
		{
			SetEquippedItem(value.type, null);
			return true;
		}
		return false;
	}

	public void UnequipItemType(ItemType type)
	{
		equipedItems[(int)type] = null;
	}

	public ItemInfo GetFirst(ref int it)
	{
		it = 0;
		if (it == itemInfoOrderedList.Count)
		{
			return null;
		}
		return itemInfoOrderedList[it];
	}

	public ItemInfo GetNext(ref int it)
	{
		it++;
		if (it == itemInfoOrderedList.Count)
		{
			return null;
		}
		return itemInfoOrderedList[it];
	}
}
