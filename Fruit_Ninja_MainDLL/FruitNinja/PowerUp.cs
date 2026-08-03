using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class PowerUp
{
	protected List<GameModifier> m_modifiers = new List<GameModifier>();

	protected uint m_nameHash;

	protected string m_name;

	protected string m_nameUpperCase;

	protected bool m_isSingle;

	protected bool m_auto;

	protected PurchaseInfo m_purchaseInfo;

	protected bool m_isClone;

	protected float m_currentTime;

	protected float m_totalTime;

	protected Color m_colour;

	protected float m_onscreenAmt;

	protected Texture m_barTexture;

	protected Texture m_popupTexture;

	protected ScreenEffect m_screenEffect;

	protected Vector3 m_pos;

	protected int m_deferedPoints;

	public float m_barX;

	public int GetDeferedPoints()
	{
		return m_deferedPoints;
	}

	public int AddDeferedPoints(int points)
	{
		if (m_deferedPoints < 0)
		{
			m_deferedPoints = 0;
		}
		m_deferedPoints += points;
		return 0;
	}

	public void SetDeferedPoints(int points)
	{
		foreach (GameModifier modifier in m_modifiers)
		{
			if (modifier.GetType() == 2)
			{
				ScoreModifier scoreModifier = (ScoreModifier)modifier;
				scoreModifier.DeferPoints(points);
			}
		}
	}

	public void DrawBar()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (m_onscreenAmt > 0f && m_barTexture != null)
		{
			float num = PowerUpManager.GetInstance().PrevPowerupDtModMultiply();
			float num2 = m_barX + (float)m_barTexture.GetWidth() * 0f;
			float num3 = 1f - m_onscreenAmt;
			num3 *= num3;
			MatrixManager.GetInstance().SetMatrix(Matrix.Identity);
			MatrixManager.GetInstance().Scale(new Vector3((float)m_barTexture.GetWidth(), (float)m_barTexture.GetHeight(), 0f));
			MatrixManager.GetInstance().Translate(new Vector3(num2, Game.SCREEN_HEIGHT / 2f + (float)m_barTexture.GetHeight() * (-0.5f + num3) + 1f, 0f));
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			m_barTexture.Set();
			Mesh.DrawQuad(Color.White, 0f, 1f, 0f, 1f);
		}
	}

	public Color GetColour()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_colour;
	}

	public bool IsClone()
	{
		return m_isClone;
	}

	public bool IsSingle()
	{
		return m_isSingle;
	}

	public bool IsTimed()
	{
		return m_totalTime > 0f;
	}

	public bool IsAuto()
	{
		return m_auto;
	}

	public float GetCurrentTimeProgress()
	{
		if (!(m_totalTime > 0f))
		{
			return 0f;
		}
		return m_currentTime / m_totalTime;
	}

	public float GetCurrentTime()
	{
		return m_currentTime;
	}

	public float GetTotalTime()
	{
		return m_totalTime;
	}

	public void SetCurrentTime(float val)
	{
		m_currentTime = val;
	}

	public void SetTotalTime(float val)
	{
		m_totalTime = val;
		m_onscreenAmt = Math.MIN((m_totalTime - m_currentTime) * 4f, 1f);
		if (m_screenEffect != null)
		{
			m_screenEffect.Update(m_totalTime - m_currentTime, m_currentTime, m_totalTime);
		}
	}

	public void AddModifier(GameModifier mod)
	{
		mod.m_parent = this;
		m_modifiers.Add(mod);
	}

	public void Activate(bool isNew, bool fromSave)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Activate(isNew, fromSave, Vector3.Zero);
	}

	public void Activate(bool isNew, bool fromSave, Vector3 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Activate(isNew, fromSave, pos, null);
	}

	public void Activate(bool isNew, bool fromSave, Vector3 pos, float? length)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!fromSave && m_popupTexture != null)
		{
			MissControl free = MissControl.GetFree();
			free.MakeDisappear(pos, 0, m_popupTexture);
			free.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_POST;
		}
		if (!fromSave && m_purchaseInfo != null)
		{
			Game.AddCoins(-m_purchaseInfo.GetCost());
		}
		foreach (GameModifier modifier in m_modifiers)
		{
			if (!modifier.IsWaiting())
			{
				modifier.ApplyModifier(fromSave, length);
			}
		}
		if (m_screenEffect != null && isNew)
		{
			m_screenEffect.Activate();
		}
	}

	public void Deactivate()
	{
		Deactivate(clearAll: false);
	}

	public void Deactivate(bool clearAll)
	{
		foreach (GameModifier modifier in m_modifiers)
		{
			if (!modifier.IsWaiting() || clearAll)
			{
				modifier.RemoveModifier();
			}
		}
		m_modifiers.Clear();
		if (m_screenEffect != null)
		{
			m_screenEffect.Deactivate();
			Delete.SAFE_DELETE(ref m_screenEffect);
		}
	}

	public uint GetHash()
	{
		return m_nameHash;
	}

	public string GetName()
	{
		return m_name;
	}

	public string GetNameUpperCase()
	{
		return m_nameUpperCase;
	}

	public void Parse(XElement parent)
	{
		m_name = parent.AttributeStr("name");
		m_nameHash = StringFunctions.StringHash(m_name);
		m_nameUpperCase = m_name.ToUpper();
		m_isSingle = StringFunctions.CompareWords(parent.AttributeStr("single"), "true");
		m_auto = StringFunctions.CompareWords(parent.AttributeStr("automatic"), "true");
		StringFunctions.ParseColour(ref m_colour, parent.AttributeStr("colour"));
		string text = parent.AttributeStr("localise");
		if (text == null)
		{
			m_barTexture = StringFunctions.LoadTexture(parent.AttributeStr("bar"));
		}
		else
		{
			string texture = parent.AttributeStr("bar") + ".tex";
			m_barTexture = TextureManager.GetInstance().Load(texture, localise: true);
		}
		string text2 = parent.AttributeStr("popup");
		if (text2 != null)
		{
			m_popupTexture = TextureManager.GetInstance().Load(text2 + ".tex", localise: true);
		}
		else
		{
			m_popupTexture = null;
		}
		m_totalTime = 0f;
		for (XElement xElement = parent.FirstChildElement(); xElement != null; xElement = xElement.NextSiblingElement())
		{
			if ("purchase_info" == xElement.Name)
			{
				m_purchaseInfo = new PurchaseInfo();
				m_purchaseInfo.Parse(xElement);
				m_isSingle = true;
			}
			else if ("effect" == xElement.Name)
			{
				if (m_screenEffect == null)
				{
					m_screenEffect = new ScreenEffect();
					m_screenEffect.m_parent = this;
					m_screenEffect.Parse(xElement);
				}
			}
			else
			{
				GameModifier gameModifier = null;
				if ("wave_mod" == xElement.Name)
				{
					gameModifier = new WaveModifier();
				}
				if ("slash_mod" == xElement.Name)
				{
					gameModifier = new SlashModifier();
				}
				if ("time_mod" == xElement.Name)
				{
					gameModifier = new TimeModifier();
				}
				if ("score_mod" == xElement.Name)
				{
					gameModifier = new ScoreModifier();
				}
				if ("explody_mod" == xElement.Name)
				{
					gameModifier = new ExplodyFruitModifier();
				}
				if (gameModifier != null)
				{
					gameModifier.Parse(xElement);
					if (gameModifier.GetTotalTime() > m_totalTime)
					{
						m_totalTime = gameModifier.GetTotalTime();
					}
					m_modifiers.Add(gameModifier);
				}
			}
		}
	}

	public PowerUp()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		m_screenEffect = null;
		m_isSingle = false;
		m_purchaseInfo = null;
		m_isClone = false;
		m_currentTime = 0f;
		m_totalTime = 0f;
		m_colour = Color.White;
		m_onscreenAmt = 0f;
		m_barTexture = null;
		m_popupTexture = null;
		m_deferedPoints = -1;
		m_barX = 0f;
		m_auto = false;
	}

	public PowerUp(PowerUp copyFrom)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		m_barX = 0f;
		m_nameHash = copyFrom.GetHash();
		m_name = copyFrom.GetName();
		m_nameUpperCase = copyFrom.GetNameUpperCase();
		m_purchaseInfo = null;
		m_colour = copyFrom.GetColour();
		m_isSingle = copyFrom.IsSingle();
		m_auto = copyFrom.IsAuto();
		m_isClone = true;
		m_currentTime = 0f;
		m_totalTime = copyFrom.GetTotalTime();
		m_onscreenAmt = 0f;
		m_deferedPoints = -1;
		m_screenEffect = null;
		if (copyFrom.m_screenEffect != null)
		{
			m_screenEffect = new ScreenEffect();
			copyFrom.m_screenEffect.Duplicate(m_screenEffect);
			m_screenEffect.m_parent = this;
		}
		m_barTexture = copyFrom.m_barTexture;
		m_popupTexture = copyFrom.m_popupTexture;
	}

	public int Purchaseable()
	{
		if (m_purchaseInfo == null)
		{
			return 0;
		}
		return m_purchaseInfo.GetCost();
	}

	public PurchaseInfo GetPurchasableInfo()
	{
		return m_purchaseInfo;
	}

	public void Release()
	{
		m_modifiers.Clear();
		Delete.SAFE_DELETE(ref m_purchaseInfo);
		Delete.SAFE_DELETE(ref m_screenEffect);
	}

	public virtual bool Update(float dt)
	{
		int num = 0;
		m_currentTime = 0f;
		int num2 = 0;
		while (num2 < m_modifiers.Count)
		{
			GameModifier gameModifier = m_modifiers[num2];
			if (gameModifier.Update(dt))
			{
				gameModifier.RemoveModifier();
				m_modifiers.RemoveAt(num2);
				continue;
			}
			if (gameModifier.GetCurrentTime() > m_currentTime)
			{
				m_currentTime = gameModifier.GetCurrentTime();
				if (m_currentTime > m_totalTime)
				{
					m_totalTime = m_currentTime;
				}
			}
			num++;
			num2++;
		}
		if (m_screenEffect != null)
		{
			m_screenEffect.Update(dt, m_currentTime, m_totalTime);
		}
		if (m_currentTime > 0f)
		{
			m_onscreenAmt = Math.MIN(m_onscreenAmt + dt * 4f, 1f);
		}
		if (m_purchaseInfo != null)
		{
			if (m_purchaseInfo.CurrentGames < 0)
			{
				return num == 0;
			}
			return false;
		}
		if (num == 0)
		{
			if (!(m_onscreenAmt > 0f))
			{
				return true;
			}
			m_onscreenAmt = Math.MAX(m_onscreenAmt - dt * 12f, 0f);
		}
		return false;
	}

	public PowerUp Clone()
	{
		PowerUp powerUp = new PowerUp(this);
		foreach (GameModifier modifier in m_modifiers)
		{
			GameModifier mod = modifier.Clone();
			powerUp.AddModifier(mod);
		}
		if (m_purchaseInfo != null)
		{
			powerUp.m_purchaseInfo = m_purchaseInfo.Duplicate();
		}
		return powerUp;
	}

	public List<GameModifier> GetFirstMod()
	{
		return m_modifiers;
	}

	public bool IsSpecial()
	{
		if (IsTimed() && !IsAuto())
		{
			return Purchaseable() == 0;
		}
		return false;
	}

	public float GetLongestMod()
	{
		float num = -1f;
		foreach (GameModifier modifier in m_modifiers)
		{
			if (modifier.GetTotalTime() > num)
			{
				num = modifier.GetTotalTime();
			}
		}
		return num;
	}
}
