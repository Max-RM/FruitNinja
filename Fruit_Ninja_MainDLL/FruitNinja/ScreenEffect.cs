using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ScreenEffect
{
	private List<Emmiter> m_emmiters = new List<Emmiter>();

	private List<EffectImage> m_effectImages = new List<EffectImage>();

	private List<ScreenTint> m_screenTints = new List<ScreenTint>();

	private List<SoundEffect> m_sounds = new List<SoundEffect>();

	private string m_name;

	private uint m_nameHash;

	public PowerUp m_parent;

	public float m_time;

	public float m_totalTime;

	public void Duplicate(ScreenEffect dest)
	{
		foreach (Emmiter emmiter in m_emmiters)
		{
			dest.m_emmiters.Add(emmiter.Duplicate());
		}
		foreach (EffectImage effectImage in m_effectImages)
		{
			dest.m_effectImages.Add(effectImage.Duplicate());
		}
		foreach (ScreenTint screenTint in m_screenTints)
		{
			dest.m_screenTints.Add(screenTint.Duplicate());
		}
		foreach (SoundEffect sound in m_sounds)
		{
			dest.m_sounds.Add(sound.Duplicate());
		}
		dest.m_name = m_name;
		dest.m_nameHash = m_nameHash;
		dest.m_parent = m_parent;
		dest.m_time = m_time;
		dest.m_totalTime = m_totalTime;
	}

	public ScreenEffect()
	{
		m_parent = null;
		m_time = 0f;
		m_totalTime = 0f;
	}

	public void Deactivate()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		foreach (Emmiter emmiter in m_emmiters)
		{
			if (emmiter.emmiter != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(emmiter.emmiter);
				emmiter.emmiter = null;
			}
		}
		foreach (EffectImage effectImage in m_effectImages)
		{
			if (effectImage.control == null)
			{
				continue;
			}
			if (effectImage.deferer)
			{
				if (m_parent.GetCurrentTimeProgress() <= 0.01f)
				{
					((ScoreMultiplyerBoard)effectImage.control).m_finalScore = m_parent.GetDeferedPoints() * 2;
				}
				else
				{
					((ScoreMultiplyerBoard)effectImage.control).m_finalScore = 0;
				}
				((ScoreMultiplyerBoard)effectImage.control).m_moveFrom = ((ScoreMultiplyerBoard)effectImage.control).m_pos;
				((ScoreMultiplyerBoard)effectImage.control).m_power = null;
			}
			else
			{
				effectImage.control.m_terminate = true;
			}
		}
		m_effectImages.Clear();
		m_screenTints.Clear();
		m_emmiters.Clear();
		m_sounds.Clear();
	}

	public void Update(float dt, float timeLeft, float totalTime)
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		if (m_time > 0f)
		{
			timeLeft = m_time;
			m_time -= dt;
			totalTime = m_totalTime;
		}
		foreach (EffectImage effectImage in m_effectImages)
		{
			float num = effectImage.timeStart * totalTime;
			float num2 = effectImage.timeEnd * totalTime;
			if (Game.game_work.hud != null && !effectImage.added)
			{
				effectImage.added = true;
				Game.game_work.hud.AddControl(effectImage.control);
			}
			bool flag = true;
			if (effectImage.transitionTime > 0f)
			{
				if (timeLeft <= num)
				{
					if (timeLeft > num2 + effectImage.transitionTime)
					{
						effectImage.transitionAmount = Math.MIN(1f, effectImage.transitionAmount + dt / effectImage.transitionTime);
					}
					else
					{
						flag = false;
						effectImage.transitionAmount = Math.CLAMP((timeLeft - num2) / effectImage.transitionTime, 0f, 1f);
					}
				}
				else
				{
					effectImage.transitionAmount = 0f;
				}
			}
			else
			{
				effectImage.transitionAmount = (Math.BETWEEN(timeLeft, num2, num) ? 1 : 0);
			}
			float num3 = 1f - effectImage.transitionAmount;
			num3 *= num3;
			effectImage.control.m_pos = effectImage.pos + (flag ? effectImage.transitionMoveIn : effectImage.transitionMoveOut) * num3;
			if ((effectImage.transitionMask & 1) != 0)
			{
				effectImage.control.m_scale = effectImage.scale * (1f - num3);
			}
			else
			{
				effectImage.control.m_scale = effectImage.scale;
			}
			if ((effectImage.transitionMask & 2) != 0)
			{
				effectImage.control.m_color.A = (byte)((float)(int)effectImage.colour.A * effectImage.transitionAmount);
			}
			effectImage.pulse += (ushort)((float)(int)Math.DEGREE_TO_IDX(180f) * dt * effectImage.pulseSpeed);
			float num4 = Math.SinIdx(effectImage.pulse);
			num4 = ((!(num4 > 0f)) ? (num4 * effectImage.pulseScaleNegative) : (num4 * effectImage.pulseScalePositive));
			HUDControl3d control = effectImage.control;
			control.m_scale *= num4 + 1f;
		}
		foreach (ScreenTint screenTint in m_screenTints)
		{
			float num5 = screenTint.timeStart * totalTime;
			float num6 = screenTint.timeEnd * totalTime;
			if (screenTint.transitionTime > 0f)
			{
				if (timeLeft <= num5)
				{
					if (timeLeft > num6 + screenTint.transitionTime)
					{
						screenTint.transitionAmount = Math.MIN(1f, screenTint.transitionAmount + dt / screenTint.transitionTime);
					}
					else
					{
						screenTint.transitionAmount = Math.CLAMP((timeLeft - num6) / screenTint.transitionTime, 0f, 1f);
					}
				}
				else
				{
					screenTint.transitionAmount = 0f;
				}
			}
			else
			{
				screenTint.transitionAmount = 1f;
			}
			if (Game.game_work.hud != null)
			{
				for (int i = 0; i < 3; i++)
				{
					Game.game_work.hud.m_backTint[i] *= Math.CLAMP(1f + (screenTint.backTint[i] - 1f) * screenTint.transitionAmount, 0f, 1f);
					Game.game_work.hud.m_backTint[i] *= Math.CLAMP(1f + (screenTint.backTint[i] - 1f) * screenTint.transitionAmount, 0f, 1f);
					Game.game_work.hud.m_tint[i] *= Math.CLAMP(1f + (screenTint.hudTint[i] - 1f) * screenTint.transitionAmount, 0f, 1f);
					Game.game_work.hud.m_tint[i] *= Math.CLAMP(1f + (screenTint.hudTint[i] - 1f) * screenTint.transitionAmount, 0f, 1f);
				}
			}
		}
		foreach (Emmiter emmiter in m_emmiters)
		{
			if (emmiter.emmiter != null && timeLeft < 0.8f)
			{
				emmiter.emmiter.rateScale = 0f;
			}
		}
		int num7 = 0;
		while (num7 < m_sounds.Count)
		{
			SoundEffect soundEffect = m_sounds[num7];
			float num8 = soundEffect.timeStart * totalTime;
			float num9 = soundEffect.timeEnd * totalTime;
			if (timeLeft <= num8)
			{
				if (!(num9 >= 0f))
				{
					SoundManager.GetInstance().SFXPlay(soundEffect.file);
					m_sounds.RemoveAt(num7);
					continue;
				}
				soundEffect.timeStart = 100f;
				if (soundEffect.sfx == null)
				{
					soundEffect.sfx = SoundManager.GetInstance().SFXPlay(soundEffect.file);
				}
				if (timeLeft < num9)
				{
					m_sounds.RemoveAt(num7);
					continue;
				}
			}
			num7++;
		}
	}

	public void Activate()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		foreach (Emmiter emmiter in m_emmiters)
		{
			if (!PSPParticleManager.GetInstance().EmitterExists(emmiter.hash))
			{
				continue;
			}
			emmiter.emmiter = PSPParticleManager.GetInstance().AddEmitter(emmiter.hash, null);
			if (emmiter.emmiter != null)
			{
				emmiter.emmiter.pos = emmiter.pos;
				if (emmiter.emmiter.tmplt.Ends())
				{
					emmiter.emmiter = null;
				}
			}
		}
		foreach (EffectImage effectImage in m_effectImages)
		{
			if (effectImage.deferer)
			{
				ScoreMultiplyerBoard scoreMultiplyerBoard = new ScoreMultiplyerBoard();
				scoreMultiplyerBoard.m_power = m_parent;
				effectImage.control = scoreMultiplyerBoard;
			}
			else
			{
				effectImage.control = new HUDControl3d();
			}
			effectImage.control.m_texture = effectImage.texture;
			effectImage.control.m_pos = effectImage.pos + effectImage.transitionMoveIn;
			effectImage.control.m_color = effectImage.colour;
			effectImage.control.m_drawOrder = effectImage.drawOrder;
			if ((effectImage.transitionMask & 1) != 0)
			{
				effectImage.control.m_scale = Vector3.Zero;
			}
			else
			{
				effectImage.control.m_scale = effectImage.scale;
			}
			if ((effectImage.transitionMask & 2) != 0)
			{
				effectImage.control.m_color.A = 0;
			}
			if (Game.game_work.hud != null)
			{
				Game.game_work.hud.AddControl(effectImage.control);
			}
			else
			{
				effectImage.added = false;
			}
		}
	}

	public void Parse(XElement parent)
	{
		string text = parent.AttributeStr("name");
		if (text == null)
		{
			text = "void";
		}
		m_name = text;
		m_nameHash = StringFunctions.StringHash(m_name);
		parent.QueryFloatAttribute("length", ref m_totalTime);
		if (m_totalTime > 0f)
		{
			m_time = m_totalTime;
		}
		for (XElement xElement = parent.FirstChildElement(); xElement != null; xElement = xElement.NextSiblingElement())
		{
			string text2 = xElement.AttributeStr("hardware");
			if ((text2 == null || !(text2 == "fast") || Game.IsFastHardware()) && (text2 == null || !(text2 == "slow") || !Game.IsFastHardware()))
			{
				if (xElement.Name == "image")
				{
					EffectImage effectImage = new EffectImage();
					effectImage.Parse(xElement);
					m_effectImages.Add(effectImage);
				}
				else if (xElement.Name == "emmiter")
				{
					Emmiter emmiter = new Emmiter();
					emmiter.Parse(xElement);
					m_emmiters.Add(emmiter);
				}
				else if (xElement.Name == "tint")
				{
					ScreenTint screenTint = new ScreenTint();
					screenTint.Parse(xElement);
					m_screenTints.Add(screenTint);
				}
				else if (xElement.Name == "sound")
				{
					SoundEffect soundEffect = new SoundEffect();
					soundEffect.Parse(xElement);
					m_sounds.Add(soundEffect);
				}
			}
		}
	}

	public uint GetNameHash()
	{
		return m_nameHash;
	}

	public string GetName()
	{
		return m_name;
	}
}
