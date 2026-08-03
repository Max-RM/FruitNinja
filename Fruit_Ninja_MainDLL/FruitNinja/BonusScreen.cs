using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class BonusScreen : HUDControl3d
{
	private int m_lastAwardSound;

	private int m_awardSoundCount;

	private static Vector3 TRANSITION_MOVE = new Vector3(0f, Game.SCREEN_HEIGHT * 0.75f, 0f);

	private static float TRANSITION_IN_TIME = 0.333f;

	private static float TRANSITION_OUT_TIME = 0.25f;

	private static float FIRST_AWARD = 0.666f;

	private static float TOTAL_TIME = 7f;

	private static float TIME_PER_AWARD = 0.8f;

	private static float WORDS_IN_TIME = 0f;

	private static float WORDS_END_TIME = 0.1f;

	private static float NUMBER_IN_TIME = 0.2f;

	private static float NUMBER_END_TIME = 0.3f;

	private static Vector3 TOTAL_POS = new Vector3(40f, -60f, 0f);

	private static Vector3 PARTICLE_POS_RIGHT = new Vector3(Game.SCREEN_WIDTH * 0.45f, 0f, 0f);

	private static Vector3 PARTICLE_POS_LEFT = new Vector3((0f - Game.SCREEN_WIDTH) * 0.45f, 0f, 0f);

	private static float AWARD_Y_DIF = -42f;

	private static float POINTS_OFFSET_X = 250f;

	private static Vector3 FIRST_NAME_OFFSET = new Vector3(-105f, 40f, 0f);

	protected int m_currentTotal;

	protected int m_total;

	protected List<BonusAwardHud> m_awards = new List<BonusAwardHud>();

	protected float m_shakeRadius;

	protected float m_shakeTime;

	protected float m_shakeTimeTotal;

	protected ushort m_shakeDir;

	protected Vector3 m_shake;

	protected float m_totalScale;

	protected bool m_hasPostedScore;

	public float m_time;

	public MortarSound m_drumRollSfx;

	public Vector3 m_offset;

	private static int oneInThree = 0;

	public static bool bomb_magnet = false;

	public void Shake(float time, float radius)
	{
		m_shakeTimeTotal = (m_shakeTime = time);
		m_shakeRadius = radius;
		m_shakeDir = (ushort)Math.g_random.Rand32(Math.DEGREE_TO_IDX(359f));
	}

	public static void LoadContent()
	{
	}

	public static void UnLoadContent()
	{
	}

	private static void AddToScoreOnArrival(Coin coin)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (oneInThree < 3)
		{
			oneInThree = 3;
		}
		oneInThree++;
		if (oneInThree == 3 || oneInThree == 6 || oneInThree >= 9)
		{
			Vector3 val = default(Vector3);
			val = new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 20f, Game.SCREEN_HEIGHT / 2f - 20f, 1f);
			Game.game_work.camera.CreateCameraShake(val, (WORDS_END_TIME - WORDS_IN_TIME) * 1.5f, 0.75f);
			SoundManager.GetInstance().SFXPlay("Bonus-Firework-Explode");
			PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("bonus_mode_fx_red"), null);
			if (pSPParticleEmitter != null)
			{
				pSPParticleEmitter.pos = val;
			}
			pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("arcade_confetti"), null);
			if (pSPParticleEmitter != null)
			{
				pSPParticleEmitter.pos = new Vector3(Math.g_random.RandF(Game.SCREEN_WIDTH * 0.05f) - Game.SCREEN_WIDTH * 0.025f + Game.SCREEN_WIDTH * 0.12f * ((oneInThree == 3) ? 0f : ((oneInThree == 6) ? 1f : (-1f))), Game.SCREEN_HEIGHT / 2f + Math.g_random.RandF(10f) + 3f, 0f);
			}
			if (oneInThree >= 9)
			{
				oneInThree = 0;
			}
		}
		SoundManager.GetInstance().SFXPlay("Bonus-Point-Get");
		Game.AddToCurrentScore(coin.GetWorth());
	}

	public BonusScreen()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		m_texture = TextureManager.GetInstance().Load("arcade_diolog_box.tex", localise: true);
		m_scale = new Vector3((float)m_texture.GetWidth(), (float)m_texture.GetHeight(), 0f) * Game.GAME_MODE_SCALE_FIX;
		m_pos = Vector3.Zero;
		m_offset = Vector3.Zero;
		m_time = 0f - TRANSITION_IN_TIME;
		m_total = 0;
		m_currentTotal = 0;
		m_awards.Clear();
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
		m_shakeTime = 0f;
		m_shakeTimeTotal = 1f;
		m_shakeRadius = 0f;
		m_shakeDir = 0;
		m_shake = Vector3.Zero;
		m_totalScale = 1f;
		m_hasPostedScore = false;
		m_lastAwardSound = 0;
		m_awardSoundCount = 0;
	}

	public override void Reset()
	{
	}

	private void SET_DEFINES()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		TRANSITION_MOVE = new Vector3(0f, Game.SCREEN_HEIGHT * 0.75f, 0f);
		TRANSITION_IN_TIME = 0.333f;
		TRANSITION_OUT_TIME = 0.25f;
		FIRST_AWARD = 0.666f;
		TIME_PER_AWARD = 1.2f;
		TOTAL_TIME = 7f;
		AWARD_Y_DIF = -42f;
		TOTAL_POS = new Vector3(50f, -88f, 0f);
		PARTICLE_POS_RIGHT = new Vector3(Game.SCREEN_WIDTH * 0.35f, 0f, 0f);
		PARTICLE_POS_LEFT = new Vector3((0f - Game.SCREEN_WIDTH) * 0.35f, 0f, 0f);
	}

	public override void Update(float dt)
	{
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		SET_DEFINES();
		Game.game_work.canFastForward = true;
		float num = m_time - dt;
		float num2 = FIRST_AWARD + TIME_PER_AWARD * ((float)m_awards.Count + 0.25f);
		if (m_drumRollSfx == null && m_time > 0f && m_time < num2)
		{
			m_drumRollSfx = SoundManager.CreateNewSound();
			if (Game.game_work.soundEnabled)
			{
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_BONUS_DRUM_ROLL, 0u, m_drumRollSfx);
				if (m_drumRollSfx != null)
				{
					m_drumRollSfx.SetVolume(0f);
				}
			}
		}
		if (m_time > TOTAL_TIME + TRANSITION_OUT_TIME)
		{
			m_terminate = true;
			Game.game_work.inBonusScreen = false;
		}
		float num3 = 0f;
		if (m_time > TOTAL_TIME)
		{
			num3 = m_time - TOTAL_TIME;
			num3 /= TRANSITION_OUT_TIME;
			for (int i = 0; i < 3; i++)
			{
				Game.game_work.hud.m_backTint[i] += (0.5f - Game.game_work.hud.m_backTint[i]) * (1f - num3);
			}
			num3 *= num3;
			m_offset = TRANSITION_MOVE * num3;
		}
		else if (m_time < 0f)
		{
			num3 = m_time;
			num3 /= TRANSITION_IN_TIME;
			num3 = 1f - Math.ABS(num3);
			for (int j = 0; j < 3; j++)
			{
				Game.game_work.hud.m_backTint[j] += (0.5f - Game.game_work.hud.m_backTint[j]) * num3;
			}
			m_offset = -TRANSITION_MOVE * (1f - Math.SinIdx(Math.DEGREE_TO_IDX(100f * num3)) / Math.SinIdx(Math.DEGREE_TO_IDX(100f)));
		}
		else
		{
			if (m_time >= num2 && m_drumRollSfx != null)
			{
				SoundManager.GetInstance().Release(m_drumRollSfx);
				m_drumRollSfx = null;
			}
			m_offset = Vector3.Zero;
			for (int k = 0; k < 3; k++)
			{
				Game.game_work.hud.m_backTint[k] = 0.5f;
			}
		}
		m_currentTotal = 0;
		Vector3 pos = m_pos + m_offset + m_shake + FIRST_NAME_OFFSET + Vector3.UnitX * POINTS_OFFSET_X;
		float num4 = FIRST_AWARD + 2f * TIME_PER_AWARD;
		float volume = 0.5f + Math.CLAMP(m_time / num4, 0f, 1f) * 0.666f;
		for (int l = 0; l < m_awards.Count; l++)
		{
			BonusAwardHud bonusAwardHud = m_awards[l];
			if (m_time - FIRST_AWARD < (float)l * TIME_PER_AWARD)
			{
				bonusAwardHud.colour.A = 0;
				bonusAwardHud.visiblePoints = 0;
				bonusAwardHud.numberScale = 0f;
			}
			else if (m_time - FIRST_AWARD < (float)l * TIME_PER_AWARD + TIME_PER_AWARD)
			{
				float num5 = (m_time - FIRST_AWARD) % TIME_PER_AWARD;
				float num6 = num5 - dt;
				num3 = Math.CLAMP((num5 - WORDS_IN_TIME) / (WORDS_END_TIME - WORDS_IN_TIME), 0f, 1f);
				bonusAwardHud.colour.A = (byte)(num3 * 255f);
				if (num6 - WORDS_IN_TIME <= 0f)
				{
					_ = 0f;
				}
				num3 = Math.CLAMP((num5 - NUMBER_IN_TIME) / (NUMBER_END_TIME - NUMBER_IN_TIME), 0f, 1f);
				if (num6 - NUMBER_IN_TIME <= 0f && num3 > 0f)
				{
					PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("bonus_mode_fx_red"), null);
					if (pSPParticleEmitter != null)
					{
						pSPParticleEmitter.pos = new Vector3((0f - Game.SCREEN_WIDTH) * 0.31f * ((l % 2 != 0) ? 1f : (-1f)), pos.Y, 0f);
					}
					pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("bonus_mode_fx_blue"), null);
					if (pSPParticleEmitter != null)
					{
						pSPParticleEmitter.pos = new Vector3(Game.SCREEN_WIDTH * 0.31f * ((l % 2 != 0) ? 1f : (-1f)), pos.Y, 0f);
					}
					pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("impact_fx"), null);
					if (pSPParticleEmitter != null)
					{
						pSPParticleEmitter.pos = pos;
					}
					Shake(NUMBER_END_TIME - NUMBER_IN_TIME, 10f);
					int num7 = ((bonusAwardHud.points <= 15) ? 1 : ((bonusAwardHud.points > 25) ? 5 : 3));
					if (num7 == m_lastAwardSound)
					{
						m_awardSoundCount++;
						m_lastAwardSound = num7;
						num7 += m_awardSoundCount;
					}
					else
					{
						m_awardSoundCount = 0;
						m_lastAwardSound = num7;
					}
					num7 = l * 2 + 1;
					string filename = $"Bonus-Explosion-{num7}";
					SoundManager.GetInstance().SFXPlay(filename);
				}
				if (num3 > 0f)
				{
					bonusAwardHud.visiblePoints = (int)((float)(bonusAwardHud.points * bonusAwardHud.multiplyer) * (num3 * 0.5f + 0.5f));
				}
				else
				{
					bonusAwardHud.visiblePoints = 0;
				}
				bonusAwardHud.numberScale = Math.SinIdx(Math.DEGREE_TO_IDX(120f * num3)) / Math.SinIdx(Math.DEGREE_TO_IDX(120f));
			}
			else
			{
				bonusAwardHud.colour.A = byte.MaxValue;
				bonusAwardHud.visiblePoints = bonusAwardHud.points * bonusAwardHud.multiplyer;
				bonusAwardHud.numberScale = 1f;
			}
			pos.Y += AWARD_Y_DIF;
			m_currentTotal += bonusAwardHud.visiblePoints;
		}
		if (m_drumRollSfx != null && Game.game_work.soundEnabled)
		{
			m_drumRollSfx.Repeat();
			m_drumRollSfx.SetVolume(volume);
		}
		if (m_time > num2)
		{
			if (num <= num2)
			{
				Vector3 value = default(Vector3);
				value = new Vector3((0f - Game.SCREEN_WIDTH) / 2f + 20f, Game.SCREEN_HEIGHT / 2f - 20f, 0f);
				if (m_total >= 5)
				{
					Coin.MakeCoins(5, m_total, m_pos + m_offset + m_shake + TOTAL_POS, 0, Math.DEGREE_TO_IDX(359f), value, -0.05f, -0.5f, "bonus_star_trail", "bonus_star_impact", AddToScoreOnArrival, useModel: false);
					Coin.MakeCoins(m_total - 5, 5, m_pos + m_offset + m_shake + TOTAL_POS, 0, Math.DEGREE_TO_IDX(359f), value, -0.05f, -0.3f, "bonus_star_trail", null, AddToScoreOnArrival, useModel: false);
				}
				else
				{
					Coin.MakeCoins(m_total, 5, m_pos + m_offset + m_shake + TOTAL_POS, 0, Math.DEGREE_TO_IDX(359f), value, -0.05f, -0.3f, "bonus_star_trail", "bonus_star_impact", AddToScoreOnArrival, useModel: false);
				}
				Game.game_work.camera.CreateCameraShake(m_pos + m_offset + m_shake + TOTAL_POS, (WORDS_END_TIME - WORDS_IN_TIME) * 2f);
				PSPParticleEmitter pSPParticleEmitter2 = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash("impact_fx"), null);
				if (pSPParticleEmitter2 != null)
				{
					pSPParticleEmitter2.pos = m_pos + m_offset + m_shake + TOTAL_POS;
				}
				int num8 = Game.game_work.currentScore + m_total;
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_FAN_FARE);
				oneInThree = 3;
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && num8 < 20)
				{
					AchievementManager.AwardAchievementWP7("Underachiever");
				}
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && num8 > 400)
				{
					AchievementManager.AwardAchievementWP7("Overachiever");
				}
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && num8 > 250 && bomb_magnet)
				{
					AchievementManager.AwardAchievementWP7("Bomb Magnet");
				}
			}
			num3 = Math.CLAMP((m_time - num2) / 0.2f, 0f, 1f);
			m_currentTotal = (int)((float)m_total * (num3 * 0.5f + 0.5f));
			m_totalScale = Math.SinIdx(Math.DEGREE_TO_IDX(num3 * 115f)) / Math.SinIdx(Math.DEGREE_TO_IDX(115f));
		}
		else
		{
			m_currentTotal = 0;
			m_totalScale = 0f;
		}
		if (m_shakeTime > 0f)
		{
			m_shakeTime -= dt;
			float num9 = m_shakeRadius * m_shakeTime / m_shakeTimeTotal;
			Vector3 val = default(Vector3);
			val = new Vector3(Math.SinIdx(m_shakeDir) * num9, Math.CosIdx(m_shakeDir) * num9, 0f);
			Vector3 val2 = val - m_shake;
			if (val2.LengthSquared() < 25f)
			{
				m_shakeDir += Math.DEGREE_TO_IDX(150f + 60f * Math.g_random.RandF(1f));
			}
			m_shake += val2 * 0.2f;
		}
		else
		{
			m_shake = Vector3.Zero;
		}
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = m_pos;
		m_pos += m_offset + m_shake;
		base.Draw(tintChannels);
		string stringToDraw = $"{m_currentTotal}";
		Game.game_work.pNumberFontBlue2.DrawString(stringToDraw, m_pos + TOTAL_POS, Color.White, (26f + 14f * ((m_currentTotal > 0) ? ((float)m_currentTotal / (float)m_total) : 0f)) * m_totalScale, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
		m_pos += FIRST_NAME_OFFSET;
		for (int i = 0; i < m_awards.Count; i++)
		{
			BonusAwardHud bonusAwardHud = m_awards[i];
			if (m_time - FIRST_AWARD < (float)i * TIME_PER_AWARD)
			{
				break;
			}
			if (bonusAwardHud.texture != null)
			{
				bonusAwardHud.texture.Set();
				MatrixManager.GetInstance().SetMatrix(Matrix.Identity);
				MatrixManager.GetInstance().Scale(new Vector3((float)bonusAwardHud.texture.GetWidth() * Game.GAME_MODE_SCALE_FIX + 1f, (float)bonusAwardHud.texture.GetHeight() * Game.GAME_MODE_SCALE_FIX + 1f, 0f));
				MatrixManager.GetInstance().Translate(m_pos - Vector3.UnitX * 35f);
				MatrixManager.GetInstance().UploadCurrentMatrices();
				Mesh.DrawQuad(bonusAwardHud.colour);
				bonusAwardHud.texture.UnSet();
			}
			Game.game_work.pGameFont.DrawString(bonusAwardHud.text, m_pos, bonusAwardHud.colour, 16f, Vector2.Zero, (ALIGNMENT_TYPE)13);
			stringToDraw = $"{bonusAwardHud.visiblePoints}";
			Game.game_work.pGameFont.DrawString(stringToDraw, m_pos + Vector3.UnitX * POINTS_OFFSET_X, bonusAwardHud.colour, 24f * bonusAwardHud.numberScale, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
			ref Vector3 pos2 = ref m_pos;
			pos2.Y += AWARD_Y_DIF;
		}
		m_pos = pos;
	}

	private Vector3 ConvertPos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.Zero;
		pos.X -= Game.game_work.camera.m_cameraShake.X;
		pos.Y -= Game.game_work.camera.m_cameraShake.Y;
		zero = pos;
		zero.X += 240f;
		zero.Y += 160f;
		zero.Y = 320f - zero.Y;
		return zero * 1.6f;
	}

	public void AddAward(Color colour, Texture texture, string text, int points)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		string strA = TheGame.instance.stringTable.GetString(835);
		if (string.Compare(strA, text) == 0)
		{
			bomb_magnet = true;
		}
		BonusAwardHud bonusAwardHud = new BonusAwardHud();
		if (text != null)
		{
			bonusAwardHud.text = text;
		}
		bonusAwardHud.texture = texture;
		bonusAwardHud.points = points;
		m_total += points;
		bonusAwardHud.visiblePoints = 0;
		bonusAwardHud.colour = colour;
		bonusAwardHud.numberColour = colour;
		bonusAwardHud.numberScale = 0f;
		m_awards.Add(bonusAwardHud);
	}
}
