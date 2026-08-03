using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class PowerUpManager
{
	private const int BAR_WIDTH = 110;

	private Dictionary<uint, PowerUp> m_powerups = new Dictionary<uint, PowerUp>();

	private List<PowerUp> m_activePowerUps = new List<PowerUp>();

	private Dictionary<uint, PowerUp> m_activeSinglePowerUps = new Dictionary<uint, PowerUp>();

	private Dictionary<uint, ScreenEffect> m_screenEffects = new Dictionary<uint, ScreenEffect>();

	private List<ScreenEffect> m_activeScreenEffects = new List<ScreenEffect>();

	private List<PowerUp> m_purchasable = new List<PowerUp>();

	private PowerUp m_currentTimedPower;

	private float m_dtMod;

	private float m_totalStopClockTime;

	private float m_clockSpeed;

	private float m_powerUpDtMod;

	private float m_previousPowerUpDtMod;

	private int m_scoreGainMultiply;

	private int m_scoreGainAdd;

	private int m_scoreLossMultiply;

	private int m_scoreLossAdd;

	private float m_currentTimedPowerProgress;

	private static PowerUpManager instance = new PowerUpManager();

	private static Texture s_bar = null;

	private void ActivatePurchase(PowerUp power)
	{
		power.Deactivate(clearAll: true);
		if (m_powerups.TryGetValue(power.GetHash(), out var value))
		{
			foreach (GameModifier item in value.GetFirstMod())
			{
				GameModifier mod = item.Clone();
				power.AddModifier(mod);
			}
			foreach (GameModifier item2 in value.GetFirstMod())
			{
				if (!item2.IsWaiting())
				{
					item2.ApplyModifier(fromSave: false, null);
				}
			}
		}
		power.GetPurchasableInfo().CurrentGames--;
	}

	public PowerUpManager()
	{
		m_powerUpDtMod = 1f;
		m_previousPowerUpDtMod = m_powerUpDtMod;
	}

	public static PowerUpManager GetInstance()
	{
		return instance;
	}

	public void SetDefaults()
	{
		m_powerUpDtMod = 1f;
		m_dtMod = 1f;
		m_totalStopClockTime = 0f;
		m_clockSpeed = 1f;
		SlashEntity.ModPowerMask = 0u;
		m_currentTimedPowerProgress = 0f;
		m_currentTimedPower = null;
		ClearScoreMultipliers();
		for (int i = 0; i < 3; i++)
		{
			Game.game_work.hud.m_backTint[i] = 1f;
			Game.game_work.hud.m_tint[i] = 1f;
		}
	}

	public void Load()
	{
		s_bar = TextureManager.GetInstance().Load("textureswp7/hud_power_meter.tex");
		XDocument xDocument = MortarXml.Load("xmlwp7/powerUpList.xml");
		m_powerups.Clear();
		m_purchasable.Clear();
		if (xDocument != null)
		{
			if (1 == 0)
			{
				return;
			}
			XElement element = xDocument.FirstChildElement("powerInfoFile");
			for (XElement xElement = element.FirstChildElement("power"); xElement != null; xElement = xElement.NextSiblingElement("power"))
			{
				PowerUp powerUp = new PowerUp();
				powerUp.Parse(xElement);
				m_powerups[powerUp.GetHash()] = powerUp;
				if (powerUp.Purchaseable() != 0)
				{
					m_purchasable.Add(powerUp);
				}
			}
			for (XElement xElement2 = element.FirstChildElement("effect"); xElement2 != null; xElement2 = xElement2.NextSiblingElement("effect"))
			{
				if (xElement2.Attribute("name") != null)
				{
					ScreenEffect screenEffect = new ScreenEffect();
					screenEffect.Parse(xElement2);
					m_screenEffects[screenEffect.GetNameHash()] = screenEffect;
				}
			}
			return;
		}
		throw new Exception("No powerup data");
	}

	public void Reset()
	{
		Reset(newGame: false);
	}

	public void Reset(bool newGame)
	{
		m_powerUpDtMod = 1f;
		m_previousPowerUpDtMod = m_powerUpDtMod;
		m_dtMod = 1f;
		m_totalStopClockTime = 0f;
		m_clockSpeed = 1f;
		SlashEntity.ModPowerMask = 0u;
		m_currentTimedPowerProgress = 0f;
		m_currentTimedPower = null;
		ClearScoreMultipliers();
		for (int i = 0; i < 3; i++)
		{
			Game.game_work.hud.m_backTint[i] = 1f;
			Game.game_work.hud.m_tint[i] = 1f;
		}
		if (newGame)
		{
			Game.game_work.timeControl.Reset();
		}
		int num = 0;
		while (num < m_activePowerUps.Count)
		{
			PowerUp powerUp = m_activePowerUps[num];
			if (powerUp.Purchaseable() != 0)
			{
				powerUp.Deactivate(clearAll: true);
				if (newGame)
				{
					ActivatePurchase(powerUp);
					num++;
					continue;
				}
				if (powerUp.GetPurchasableInfo().CurrentGames > 0)
				{
					num++;
					continue;
				}
			}
			m_activeSinglePowerUps.Remove(powerUp.GetHash());
			powerUp.Deactivate();
			powerUp.Release();
			m_activePowerUps.RemoveAt(num);
		}
		foreach (KeyValuePair<uint, PowerUp> powerup in m_powerups)
		{
			if (powerup.Value.IsAuto())
			{
				ActivatePower(powerup.Key);
			}
		}
	}

	public void ClearTimedPowers()
	{
		m_currentTimedPowerProgress = 0f;
		m_currentTimedPower = null;
		int num = 0;
		while (num < m_activePowerUps.Count)
		{
			PowerUp powerUp = m_activePowerUps[num];
			if (powerUp.Purchaseable() != 0 || !powerUp.IsTimed())
			{
				num++;
				continue;
			}
			m_activeSinglePowerUps.Remove(powerUp.GetHash());
			powerUp.Deactivate();
			powerUp.Release();
			m_activePowerUps.RemoveAt(num);
		}
	}

	public void Update(float dt)
	{
		float num = dt * m_previousPowerUpDtMod;
		m_powerUpDtMod = 1f;
		m_dtMod = 1f;
		m_totalStopClockTime = 0f;
		m_clockSpeed = 1f;
		SlashEntity.ModPowerMask = 0u;
		m_currentTimedPowerProgress = 0f;
		m_currentTimedPower = null;
		ClearScoreMultipliers();
		for (int i = 0; i < 3; i++)
		{
			Game.game_work.hud.m_backTint[i] = 1f;
			Game.game_work.hud.m_tint[i] = 1f;
		}
		int num2 = 0;
		int num3 = 0;
		while (num3 < m_activePowerUps.Count)
		{
			PowerUp powerUp = m_activePowerUps[num3];
			if (powerUp.Update((powerUp.Purchaseable() == 0) ? num : dt))
			{
				m_activeSinglePowerUps.Remove(powerUp.GetHash());
				powerUp.Deactivate();
				powerUp.Release();
				m_activePowerUps.RemoveAt(num3);
				continue;
			}
			float currentTimeProgress = powerUp.GetCurrentTimeProgress();
			if (currentTimeProgress > m_currentTimedPowerProgress)
			{
				if (powerUp.Purchaseable() != 0)
				{
					if (m_currentTimedPowerProgress < 0.001f)
					{
						m_currentTimedPowerProgress = 0.001f;
					}
				}
				else
				{
					m_currentTimedPower = powerUp;
					m_currentTimedPowerProgress = currentTimeProgress;
				}
			}
			float num4 = -55f * (float)(GetNumActiveTimedPowers() - 1) + (float)(110 * num2);
			powerUp.m_barX += (num4 - powerUp.m_barX) * 0.2f;
			if (powerUp.IsSpecial())
			{
				num2++;
			}
			num3++;
		}
		int num5 = 0;
		while (num5 < m_activeScreenEffects.Count)
		{
			ScreenEffect screenEffect = m_activeScreenEffects[num5];
			screenEffect.Update(dt, 0f, 0f);
			if (screenEffect.m_time <= 0f)
			{
				screenEffect.Deactivate();
				m_activeScreenEffects.RemoveAt(num5);
			}
			else
			{
				num5++;
			}
		}
		m_previousPowerUpDtMod = m_powerUpDtMod;
	}

	public void Draw()
	{
		foreach (PowerUp activePowerUp in m_activePowerUps)
		{
			activePowerUp.DrawBar();
		}
	}

	public PowerUp ActivatePower(uint hash)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ActivatePower(hash, Vector3.Zero);
	}

	public PowerUp ActivatePower(uint hash, Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ActivatePower(hash, pos, null);
	}

	public PowerUp ActivatePower(uint hash, Vector3 pos, float? fromSave)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (m_powerups.TryGetValue(hash, out var value))
		{
			PowerUp powerUp = value;
			if (m_activeSinglePowerUps.TryGetValue(hash, out value))
			{
				float longestMod = value.GetLongestMod();
				value.Activate(isNew: false, fromSave.HasValue, pos, longestMod);
				return value;
			}
			PowerUp powerUp2 = powerUp.Clone();
			m_activePowerUps.Add(powerUp2);
			if (GetNumActiveTimedPowers() != 0 && !powerUp2.IsAuto() && powerUp2.Purchaseable() == 0 && !fromSave.HasValue)
			{
				float num = powerUp2.GetLongestMod();
				foreach (PowerUp activePowerUp in m_activePowerUps)
				{
					if (activePowerUp.IsSpecial())
					{
						float longestMod2 = activePowerUp.GetLongestMod();
						if (longestMod2 < num)
						{
							num = longestMod2;
						}
					}
				}
				foreach (PowerUp activePowerUp2 in m_activePowerUps)
				{
					if (activePowerUp2.IsSpecial() && activePowerUp2 != powerUp2)
					{
						activePowerUp2.Activate(isNew: false, fromSave: false, Vector3.Zero, num);
					}
				}
				powerUp2.Activate(isNew: true, fromSave: false, pos, num);
			}
			else
			{
				powerUp2.Activate(isNew: true, fromSave.HasValue, pos, fromSave);
			}
			powerUp2.m_barX = 55f * (float)GetNumActiveTimedPowers();
			if (powerUp2.IsSingle())
			{
				m_activeSinglePowerUps[hash] = powerUp2;
			}
			return powerUp2;
		}
		return null;
	}

	public float GetDtMod()
	{
		return m_dtMod;
	}

	public void StopClock(float time)
	{
		m_totalStopClockTime += time;
	}

	public float SlowClock()
	{
		return SlowClock(1f);
	}

	public float SlowClock(float val)
	{
		return m_clockSpeed *= val;
	}

	public float GetStopClockAmount()
	{
		return m_totalStopClockTime;
	}

	public float GetCurrentTimedPowerProgress()
	{
		return m_currentTimedPowerProgress;
	}

	public void ApplyDtMod(float mod)
	{
		m_dtMod *= mod;
	}

	public int GetScoreGainMultiplier()
	{
		return m_scoreGainMultiply * m_scoreGainAdd;
	}

	public int GetScoreLossMultiplier()
	{
		return m_scoreLossMultiply * m_scoreLossAdd;
	}

	public void AddToScoreGainAdd(int multiplier)
	{
		m_scoreGainAdd += multiplier;
	}

	public void AddToScoreLossAdd(int multiplier)
	{
		m_scoreLossAdd += multiplier;
	}

	public void AddToScoreGainMultiply(int multiplier)
	{
		m_scoreGainMultiply *= multiplier;
	}

	public void AddToScoreLossMultiply(int multiplier)
	{
		m_scoreLossMultiply *= multiplier;
	}

	public void ClearScoreMultipliers()
	{
		m_scoreGainAdd = 1;
		m_scoreLossAdd = 1;
		m_scoreGainMultiply = 1;
		m_scoreLossMultiply = 1;
	}

	public void Release()
	{
		s_bar = null;
		foreach (PowerUp activePowerUp in m_activePowerUps)
		{
			activePowerUp.Release();
		}
		foreach (KeyValuePair<uint, PowerUp> powerup in m_powerups)
		{
			powerup.Value.Release();
		}
		foreach (KeyValuePair<uint, ScreenEffect> screenEffect in m_screenEffects)
		{
			screenEffect.Value.Deactivate();
		}
		m_screenEffects.Clear();
		m_activeScreenEffects.Clear();
	}

	public float PrevPowerupDtModMultiply()
	{
		return m_previousPowerUpDtMod;
	}

	public void PowerupDtModMultiply(float value)
	{
		m_powerUpDtMod *= value;
	}

	public PowerUp GetActiveSingle(uint hash)
	{
		PowerUp value = null;
		if (m_activeSinglePowerUps.TryGetValue(hash, out value))
		{
			return value;
		}
		return null;
	}

	public void SaveActivePowerUps(XElement root)
	{
		foreach (PowerUp activePowerUp in m_activePowerUps)
		{
			PowerUp powerUp = activePowerUp;
			XElement xElement = new XElement("power");
			xElement.SetAttributeValue("name", powerUp.GetName());
			xElement.SetAttributeValue("time", powerUp.GetCurrentTime());
			xElement.SetAttributeValue("totalTime", powerUp.GetTotalTime());
			if (powerUp.GetDeferedPoints() >= 0)
			{
				xElement.SetAttributeValue("deferedPoints", powerUp.GetDeferedPoints());
			}
			root.Add(xElement);
		}
	}

	public void LoadActivePowerUps(XElement root, int gameMode)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		for (XElement xElement = root.FirstChildElement("power"); xElement != null; xElement = xElement.NextSiblingElement("power"))
		{
			float value = 0f;
			xElement.QueryFloatAttribute("time", ref value);
			uint num = StringFunctions.StringHash(xElement.AttributeStr("name"));
			if (m_powerups.TryGetValue(num, out var value2))
			{
				PowerUp powerUp = value2;
				if ((!powerUp.IsSpecial() && !powerUp.IsAuto()) || gameMode == 2)
				{
					powerUp = ActivatePower(num, Vector3.Zero, value);
					powerUp.SetCurrentTime(value);
					xElement.QueryFloatAttribute("totalTime", ref value);
					powerUp.SetTotalTime(value);
					int value3 = -1;
					xElement.QueryIntAttribute("deferedPoints", ref value3);
					if (value3 >= 0)
					{
						Game.AddToCurrentScore(value3);
					}
				}
			}
		}
	}

	public int GetNumActiveTimedPowers()
	{
		int num = 0;
		foreach (PowerUp activePowerUp in m_activePowerUps)
		{
			PowerUp powerUp = activePowerUp;
			if (powerUp.IsSpecial())
			{
				num++;
			}
		}
		return num;
	}

	public float GetActiveProgression()
	{
		return GetActiveProgression(0f);
	}

	public float GetActiveProgression(float plusTime)
	{
		PowerUp powerUp = null;
		foreach (PowerUp activePowerUp in m_activePowerUps)
		{
			if (activePowerUp.IsSpecial())
			{
				powerUp = activePowerUp;
			}
		}
		if (powerUp != null)
		{
			if (plusTime > 0f && powerUp.GetTotalTime() > 0f)
			{
				return (powerUp.GetCurrentTime() - plusTime) / powerUp.GetTotalTime();
			}
			return powerUp.GetCurrentTimeProgress();
		}
		return 2f;
	}

	public bool ActivateScreenEffect(uint hash)
	{
		if (m_screenEffects.TryGetValue(hash, out var value))
		{
			ScreenEffect screenEffect = new ScreenEffect();
			value.Duplicate(screenEffect);
			screenEffect.Activate();
			m_activeScreenEffects.Add(screenEffect);
			return true;
		}
		return false;
	}

	public bool SetAppropriateScoreCallback()
	{
		foreach (PowerUp activePowerUp in m_activePowerUps)
		{
			PowerUp powerUp = activePowerUp;
			foreach (GameModifier item in powerUp.GetFirstMod())
			{
				if (item.GetType() == 2)
				{
					ScoreModifier scoreModifier = (ScoreModifier)item;
					if (scoreModifier.DoesDeferPoint())
					{
						Game.SetScoreDelegate(scoreModifier.DeferPoints);
						return true;
					}
				}
			}
		}
		Game.SetScoreDelegate();
		return false;
	}
}
