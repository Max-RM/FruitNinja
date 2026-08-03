using System.Xml.Linq;
using Mortar;

namespace FruitNinja;

public class PurchaseInfo
{
	protected int m_games;

	protected int m_cost;

	protected string m_description;

	protected string m_title;

	protected Texture m_texture;

	protected Texture m_inUseTexture;

	protected Texture m_greyTexture;

	public int CurrentGames;

	public PurchaseInfo Duplicate()
	{
		PurchaseInfo purchaseInfo = new PurchaseInfo();
		purchaseInfo.m_games = m_games;
		purchaseInfo.m_cost = m_cost;
		purchaseInfo.m_description = m_description;
		purchaseInfo.m_title = m_title;
		purchaseInfo.m_texture = m_texture;
		purchaseInfo.m_inUseTexture = m_inUseTexture;
		purchaseInfo.m_greyTexture = m_greyTexture;
		return purchaseInfo;
	}

	public PurchaseInfo()
	{
		m_games = 0;
		CurrentGames = 0;
		m_cost = 0;
	}

	public string GetDescription()
	{
		return m_description;
	}

	public string GetTitle()
	{
		return m_title;
	}

	public Texture GetTexture()
	{
		return m_texture;
	}

	public Texture GetInUseTexture()
	{
		return m_inUseTexture;
	}

	public Texture GetGreyTexture()
	{
		return m_greyTexture;
	}

	public int GetCost()
	{
		return m_cost;
	}

	public int GetGames()
	{
		return m_games;
	}

	public void Parse(XElement element)
	{
		element.QueryIntAttribute("games", ref m_games);
		CurrentGames = m_games;
		element.QueryIntAttribute("cost", ref m_cost);
		string text = element.AttributeStr("title");
		if (text != null)
		{
			m_title = text;
		}
		text = element.AttributeStr("texture");
		string texture = string.Format("textureswp7/{0}.tex", (text != null) ? text : "arcade_item_01_buy");
		m_texture = TextureManager.GetInstance().Load(texture);
		text = element.AttributeStr("selectedTexture");
		texture = string.Format("textureswp7/{0}.tex", (text != null) ? text : "arcade_item_01_selected");
		m_inUseTexture = TextureManager.GetInstance().Load(texture);
		text = element.AttributeStr("usedTexture");
		texture = string.Format("textureswp7/{0}.tex", (text != null) ? text : "arcade_item_01_used");
		m_greyTexture = TextureManager.GetInstance().Load(texture);
		text = element.FirstChildElement("description")?.GetText();
		if (text != null)
		{
			m_description = text;
		}
	}
}
