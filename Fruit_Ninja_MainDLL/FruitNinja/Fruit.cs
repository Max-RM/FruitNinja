using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class Fruit : Entity
{
	public delegate void FruitEvent(Fruit fruit);

	public delegate void FruitSliceEvent(Fruit fruit, int points, Entity otherEnt);

	public enum FRUIT_PEICE
	{
		FRUIT_PEICE_TOP,
		FRUIT_PEICE_BOTTOM,
		MAX_FRUIT_PIECES
	}

	public const int FINAL_FREEZE_TIME = 8;

	public static FruitEvent FruitKilled;

	public static FruitEvent FruitThrown;

	public static FruitSliceEvent FruitSliced;

	public FruitSliceEvent m_fruitSliced;

	public FruitEvent m_fruitKilled;

	public FruitEvent m_fruitThrown;

	private static bool s_fruitModelsLoaded = false;

	private static bool s_sfxPlayedThisFrame = false;

	private static List<Model>[] s_fruitModels = ArrayInit.CreateFilledArray<List<Model>>(3);

	protected byte m_fruitType;

	protected bool m_disabled;

	protected byte PAD01;

	protected PSPParticleEmitter[] m_slicedEmitter = ArrayInit.CreateFilledArray<PSPParticleEmitter>(2);

	protected Vector3 m_splashEffectPos;

	protected Vector3 m_splashEffectScale;

	protected int m_splashTime;

	protected int m_splashFrameLength;

	protected int m_splashFrames;

	public float m_sliceWait;

	public ushort m_slicedAngle;

	public float m_slicedMag;

	protected bool m_gravityEnabled;

	protected float m_chuckWait;

	protected Vector3 m_separation;

	protected int m_forPlayer;

	public float m_dtMod;

	public bool m_isLockedMenuButton;

	public float m_z;

	public Vector3 m_gravity;

	public Vector3 m_orig_scale;

	public bool m_waitingForCombo;

	public static int MAX_FRUIT_TYPES = 0;

	public static int CRITICAL_SPLATS = 10;

	public static float CRITICAL_SPLAT_SCALE = 1.5f;

	public static float CRITICAL_SPLAT_SPREAD = 1.2f;

	public static float CRITICAL_DISAPPEAR_SPEED = 2f;

	public static Color CRITICAL_COLOUR = Color.Blue;

	public static int CRITICAL_SCORE = 5;

	public static int CRITICAL_CHANCE = 5;

	public static int CRITICAL_CHANCE_START_INC = 30;

	public static float CRITICAL_FLASH_TIME = 0.5f;

	public static float CRITICAL_FLASH_FULL = 0.4f;

	public static float CRITICAL_FLASH_START_FADE = 0.3f;

	public static Color CRITICAL_FLASH_COLOUR = new Color(128, 128, 255, 128);

	public static int NEW_LIFE_AT = 100;

	public static FRUIT_INFO[] fruitInfo = null;

	public static int s_consecutiveCount = 0;

	public static int s_consecutiveType = -1;

	public static int s_numActivePowerUpFruits = 0;

	public bool m_isSliced;

	public Vector3 m_pos2;

	public Vector3 m_vel2;

	public Quaternion[] m_rotation_piece = ArrayInit.CreateFilledArray<Quaternion>(2);

	public Vector3[] m_rotation_speed = ArrayInit.CreateFilledArray<Vector3>(2);

	private Vector3[,] m_rotation_axes = ArrayInit.CreateFilledArray<Vector3>(2, 3);

	public MenuButton m_hudControl;

	public bool m_isMenuItem;

	public bool m_isCritical;

	private static int[] outOfFruitTime = new int[Game.MAX_PLAYERS];

	private static bool inited = false;

	private static int banana;

	public static int LOCKED_BANANA_TYPE = -1;

	private static int totalChance = 0;

	private static int totalChanceNonCoined = 0;

	private static int totalChanceOfCritical = 0;

	private static int totalChanceOfCriticalNonCoined = 0;

	private static int redApple = FruitType("apple_red");

	private static int apple = FruitType("apple");

	public static float MAX_ROTATION_SPEED => 5.5f;

	public static float MAX_ROTATION_ANGLE => 359f;

	public static float MIN_SPEED => 4f;

	public static float MAX_SPEED => 8f;

	public static int RANDOM_ANGLE_SLICE => Mortar.Math.DEGREE_TO_IDX(120f);

	public static float SIXTY_FPS_DT => 1f / 60f;

	public static float SLICE_DELAY => 0.03f;

	public static float FRUIT_CHUCK_WAIT => 0.125f;

	public void RemoveTrailParticles()
	{
		for (int i = 0; i < 2; i++)
		{
			if (m_slicedEmitter[i] != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(m_slicedEmitter[i]);
			}
			m_slicedEmitter[i] = null;
		}
	}

	public static int NumberOfPowerupFruits()
	{
		LinkedListNode<Entity> iterator = null;
		Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator);
		int num = 0;
		while (fruit != null)
		{
			if (fruit.GetFruitInfo().powers != null)
			{
				num++;
			}
			fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator);
		}
		return num;
	}

	public FRUIT_INFO GetFruitInfo()
	{
		return fruitInfo[m_fruitType];
	}

	public Vector3 GetSliceDir()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetSliceDir(0);
	}

	public Vector3 GetSliceDir(ushort offset)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mortar.Math.SinIdx((ushort)(m_slicedAngle + offset)), Mortar.Math.CosIdx((ushort)(m_slicedAngle + offset)), 0f);
	}

	public void KillFruit()
	{
		KillFruit(dropped: false);
	}

	public void KillFruit(bool dropped)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 2; i++)
		{
			if (m_slicedEmitter[i] != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(m_slicedEmitter[i]);
				m_slicedEmitter[i] = null;
			}
		}
		if (!m_isSliced && !m_disabled && !Game.InViewer() && fruitInfo[m_fruitType].score < 5)
		{
			if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
			{
				if (dropped)
				{
					uint hash = StringFunctions.StringHash("dropped");
					Game.game_work.saveData.AddToTotal("dropped", hash, 1, universal: false, achievementUpload: false);
				}
			}
			else if (Game.FailureEnabled() && dropped && !SuperFruitControl.IsInSuperFruitState() && !GetFruitInfo().superFruit)
			{
				if (!Game.game_work.gameOver)
				{
					MissControl free = MissControl.GetFree();
					free.MakeDisappear(m_pos, m_forPlayer);
					SoundManager.GetInstance().SFXPlay(SoundDef.SND_GANK);
					Game.game_work.hasDroppedFruit = true;
				}
				Game.game_work.currentMissCount++;
				if (Game.game_work.currentMissCount >= Game.MAX_FRUIT_MISSES)
				{
					Game.GameOver();
					s_consecutiveCount = 0;
					s_consecutiveType = -1;
				}
			}
		}
		if (m_hudControl != null && m_hudControl.m_entity == this)
		{
			m_hudControl.m_entity = null;
		}
		if (!base.m_destroy && fruitInfo[m_fruitType].powers != null)
		{
			s_numActivePowerUpFruits = Mortar.Math.MAX(s_numActivePowerUpFruits - 1, 0);
		}
		if (FruitKilled != null)
		{
			FruitKilled(this);
		}
		if (m_fruitKilled != null)
		{
			m_fruitKilled(this);
		}
		base.m_destroy = true;
	}

	public int CheckFruitDropped()
	{
		bool flag = false;
		for (int i = 1; i < Game.MAX_PLAYERS; i++)
		{
			if (outOfFruitTime[i] > 0)
			{
				outOfFruitTime[i]--;
				if (outOfFruitTime[i] == 0)
				{
					flag = true;
					Game.GameOver(-1, -1f, i);
				}
			}
		}
		if (!flag)
		{
			if (outOfFruitTime[1] > 0 && outOfFruitTime[2] > 0)
			{
				flag = true;
				Game.GameOver(-1, -1f, 0);
			}
			else if (outOfFruitTime[1] > 0)
			{
				flag = true;
				Game.GameOver(-1, -1f, 1);
			}
			else if (outOfFruitTime[2] > 0)
			{
				flag = true;
				Game.GameOver(-1, -1f, 2);
			}
		}
		if (flag)
		{
			for (int j = 1; j < Game.MAX_PLAYERS; j++)
			{
				outOfFruitTime[j] = 0;
			}
		}
		if (!flag)
		{
			return 0;
		}
		return 1;
	}

	protected void UpdateBombAvoidance(float dt)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (m_isSliced)
		{
			return;
		}
		LinkedListNode<Entity> iterator = null;
		for (Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); entity != null; entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			if (((Bomb)entity).IsActive() && ((Bomb)entity).ForPlayer() == ForPlayer() && entity.m_col_box != null)
			{
				Vector3 val = entity.m_pos - m_pos;
				float num = val.LengthSquared();
				float num2 = (m_vel.X - entity.m_vel.X) * (m_vel.X - entity.m_vel.X) + (m_vel.Y - entity.m_vel.Y) * (m_vel.Y - entity.m_vel.Y);
				if (num < 4900f && num2 < 56.25f)
				{
					ref Vector3 vel = ref entity.m_vel;
					vel.X += (float)Mortar.Math.MATH_SIGN(val.Y) * dt * 12f;
				}
			}
		}
	}

	public Fruit()
	{
		m_col_box = null;
		m_hudControl = null;
		m_isMenuItem = false;
		m_waitingForCombo = false;
		for (int i = 0; i < 2; i++)
		{
			m_slicedEmitter[i] = null;
		}
	}

	public override void Init(byte[] tpl_data, int tpl_size, Vector3? size)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		m_isLockedMenuButton = false;
		m_waitingForCombo = false;
		m_dtMod = 1f;
		if (tpl_size >= 0 && tpl_size < MAX_FRUIT_TYPES)
		{
			m_fruitType = (byte)tpl_size;
		}
		else
		{
			m_fruitType = (byte)RandomFruit(canUseCoiners: true);
		}
		if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && Game.game_work.gameOverTransition < 1f)
		{
			if (!inited)
			{
				banana = FruitType("banana");
				inited = true;
			}
			while (m_fruitType == banana)
			{
				m_fruitType = (byte)RandomFruit(canUseCoiners: true);
			}
			if (fruitInfo[m_fruitType].powers != null)
			{
				uint num = StringFunctions.StringHash("freeze");
				if (s_numActivePowerUpFruits > 0 || (Game.game_work.saveData.timer < 8f && fruitInfo[m_fruitType].powers.powerUps[0].powerHash != num) || fruitInfo[m_fruitType].powers.AnyActivePowers())
				{
					base.m_destroy = true;
					return;
				}
			}
		}
		if (fruitInfo[m_fruitType].powers != null)
		{
			s_numActivePowerUpFruits++;
		}
		SetFruitType(m_fruitType, size.HasValue ? size.Value.X : 1f);
		base.m_updateAlways = true;
		base.m_destroy = false;
		m_isSliced = false;
		m_disabled = false;
		m_sliceWait = -1f;
		m_chuckWait = 0f;
		m_forPlayer = 0;
		m_separation = Vector3.Zero;
		float num2 = Mortar.Math.g_random.RandF(MAX_ROTATION_SPEED * 2f) - MAX_ROTATION_SPEED;
		float num3 = Mortar.Math.g_random.RandF(MAX_ROTATION_SPEED * 2f) - MAX_ROTATION_SPEED;
		float num4 = Mortar.Math.g_random.RandF(MAX_ROTATION_SPEED * 2f) - MAX_ROTATION_SPEED;
		Quaternion q = default(Quaternion);
		RandomStartAngle(ref q);
		if (fruitInfo[m_fruitType].superFruit)
		{
			q = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)(int)Mortar.Math.DEGREE_TO_IDX(Utils.GetRandBetween(12f, 40f)));
			num2 = 10f;
			num3 = 0f;
			num4 = Utils.GetRandBetween(3f, 5f, 0.5f);
		}
		for (int i = 0; i < 2; i++)
		{
			ref Vector3 reference = ref m_rotation_axes[i, 0];
			reference = Vector3.UnitX;
			ref Vector3 reference2 = ref m_rotation_axes[i, 1];
			reference2 = Vector3.UnitY;
			ref Vector3 reference3 = ref m_rotation_axes[i, 2];
			reference3 = Vector3.UnitZ;
			ref Vector3 reference4 = ref m_rotation_speed[i];
			reference4 = new Vector3(num2, num3, num4);
			m_rotation_piece[i] = q;
			m_slicedEmitter[i] = null;
		}
		m_splashTime = 0;
		m_splashFrameLength = 75;
		m_splashFrames = 4;
		m_gravityEnabled = true;
		m_hudControl = null;
		m_isMenuItem = false;
		m_isCritical = true;
		m_z = Game.GetFruitZPosition();
		m_gravity = new Vector3(0f, 0f - Game.GRAVITY / SIXTY_FPS_DT, 0f);
	}

	public override void Release()
	{
		for (int i = 0; i < 2; i++)
		{
			if (m_slicedEmitter[i] != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(m_slicedEmitter[i]);
				m_slicedEmitter[i] = null;
			}
		}
		if (m_hudControl != null && m_hudControl.m_entity == this)
		{
			m_hudControl.m_entity = null;
		}
		base.Release();
	}

	public override void Update(float dt)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		dt *= m_dtMod;
		float num = dt / SIXTY_FPS_DT;
		if (m_isSliced)
		{
			if (!m_isCritical)
			{
				float num2 = m_gravity.Length();
				m_gravity.Normalize();
				m_gravity *= num2 + Game.GRAVITY * num * 4.5f;
			}
			if (m_isMenuItem)
			{
				float num3 = m_gravity.Length();
				m_gravity.Normalize();
				m_gravity *= num3 + Game.GRAVITY * num * 6.5f;
			}
			m_vel += m_gravity * dt;
			m_vel2 += m_gravity * dt;
			m_pos += m_vel * num;
			m_pos2 += m_vel2 * num;
			m_splashTime += (int)(dt * 1000f);
		}
		else
		{
			if (m_chuckWait > 0f)
			{
				float chuckWait = m_chuckWait;
				if (!Game.game_work.pause && Game.game_work.hitBombTime <= 0f && ((Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && Game.game_work.gameOverTransition < 1f) || !Game.game_work.gameOver))
				{
					m_chuckWait -= Game.game_work.dt;
				}
				if (m_chuckWait <= 0.2f && chuckWait > 0.2f && !s_sfxPlayedThisFrame && !Game.game_work.gameOver)
				{
					SoundManager.GetInstance().SFXPlay(SoundDef.SND_THROW_FRUIT);
					s_sfxPlayedThisFrame = true;
				}
				if (!(m_chuckWait <= 0f))
				{
					return;
				}
				if (m_fruitThrown != null)
				{
					m_fruitThrown(this);
				}
				FruitThrown(this);
				if (PSPParticleManager.GetInstance().EmitterExists(fruitInfo[m_fruitType].hash[2]))
				{
					m_slicedEmitter[0] = PSPParticleManager.GetInstance().AddEmitter(fruitInfo[m_fruitType].hash[2], null);
					if (m_slicedEmitter[0] != null)
					{
						m_slicedEmitter[0].pos = m_pos + Vector3.UnitZ * m_z;
					}
				}
				else if (!Game.IsFastHardware() && m_slicedEmitter[0] != null)
				{
					m_slicedEmitter[0].pos = m_pos + Vector3.UnitZ * (m_z - 20f);
				}
				float num4 = WaveManager.GetInstance().FruitMultiplyer();
				int num5 = (int)num4;
				if (num4 > (float)num5 + 0.01f && (num4 - (float)num5) * 100f > (float)Mortar.Math.g_random.Rand32(100))
				{
					num5++;
				}
				if (num5 <= 0)
				{
					m_chuckWait = 0f;
					m_pos.Y = 0f - Game.SCREEN_HEIGHT;
					m_vel = new Vector3(0f, -1f, 0f);
				}
				else if (num5 > 1)
				{
					SPAWNER_INFO[] array = new SPAWNER_INFO[3];
					array[0].placement = SPAWN_PLACEMENTS.SPAWNER_TOP;
					array[0].gravity = new Vector3(0f, -0.05f, 0f);
					array[0].delayWait = -3f;
					array[1].placement = SPAWN_PLACEMENTS.SPAWNER_LEFT;
					array[1].horizontalMin = -1f;
					array[1].horizontalMax = -0.5f;
					array[1].delayWait = -3f;
					array[2].placement = SPAWN_PLACEMENTS.SPAWNER_RIGHT;
					array[2].horizontalMin = -1f;
					array[2].horizontalMax = -0.5f;
					array[2].delayWait = -3f;
					WaveManager.GetInstance().SpawnFruit(num5 - 1, -1, array[Mortar.Math.g_random.Rand32(3)]);
				}
			}
			if (m_gravityEnabled && !m_waitingForCombo)
			{
				m_pos += (m_vel * dt + m_gravity * 0.5f * (dt * dt)) * 60f;
				m_vel += m_gravity * dt;
				m_pos += m_separation * dt;
			}
			m_pos2 = m_pos;
			m_vel2 = m_vel;
			if (m_sliceWait > 0f)
			{
				if (!m_waitingForCombo)
				{
					m_sliceWait -= dt;
				}
				if (m_sliceWait <= 0f)
				{
					m_sliceWait = 0f;
					Slice();
				}
			}
			UpdateBombAvoidance(dt);
		}
		for (int i = 0; i < 2; i++)
		{
			if (m_waitingForCombo)
			{
				break;
			}
			if (GetFruitInfo().superFruit && !m_isSliced)
			{
				Matrix val = Matrix.CreateFromQuaternion(m_rotation_piece[0]);
				ref Vector3 reference = ref m_rotation_axes[i, 0];
				reference = Vector3.UnitX * val.Translation;
			}
			Vector3 val2 = m_rotation_axes[i, 0];
			Vector3 val3 = m_rotation_axes[i, 1];
			Vector3 val4 = m_rotation_axes[i, 2];
			Quaternion val5 = Quaternion.CreateFromAxisAngle(val2, MathHelper.ToRadians(m_rotation_speed[i].X * num));
			Quaternion val6 = Quaternion.CreateFromAxisAngle(val3, MathHelper.ToRadians(m_rotation_speed[i].Y * num));
			Quaternion val7 = Quaternion.CreateFromAxisAngle(val4, MathHelper.ToRadians(m_rotation_speed[i].Z * num));
			ref Quaternion reference2 = ref m_rotation_piece[i];
			reference2 = m_rotation_piece[i] * val5 * val6 * val7;
			m_rotation_piece[i].Normalize();
		}
		if (CheckHasGoneOffsceen())
		{
			KillFruit(dropped: true);
		}
		if (m_col_box != null)
		{
			m_col_box.centre = m_pos;
			m_col_box.centre.Z = 0f;
		}
		if (dt == 0f)
		{
			for (int j = 0; j < 2; j++)
			{
				if (m_slicedEmitter[j] != null)
				{
					PSPParticleManager.GetInstance().ClearEmitter(m_slicedEmitter[j]);
				}
				m_slicedEmitter[j] = null;
			}
		}
		if (m_slicedEmitter[0] != null)
		{
			m_slicedEmitter[0].pos = m_pos + Vector3.UnitZ * (m_z - 20f);
			m_slicedEmitter[0].pos.Z = -5000f;
			Matrix val8 = default(Matrix);
			Matrix.CreateFromQuaternion(ref m_rotation_piece[0], out val8);
			Vector3 unitZ = Vector3.UnitZ;
			Vector3 val9 = default(Vector3);
			Vector3.Transform(ref unitZ, ref val8, out val9);
			ushort idx = Mortar.Math.Atan2Idx(val9.X, val9.Y);
			m_slicedEmitter[0].sinz = Mortar.Math.SinIdx(idx);
			m_slicedEmitter[0].cosz = Mortar.Math.CosIdx(idx);
		}
		if (m_slicedEmitter[1] != null)
		{
			m_slicedEmitter[1].pos = m_pos2 + Vector3.UnitZ * m_z;
			Matrix val10 = default(Matrix);
			Matrix.CreateFromQuaternion(ref m_rotation_piece[0], out val10);
			Vector3 unitZ2 = Vector3.UnitZ;
			Vector3 val11 = default(Vector3);
			Vector3.Transform(ref unitZ2, ref val10, out val11);
			ushort idx2 = Mortar.Math.Atan2Idx(val11.X, val11.Y);
			m_slicedEmitter[1].sinz = Mortar.Math.SinIdx(idx2);
			m_slicedEmitter[1].cosz = Mortar.Math.CosIdx(idx2);
		}
	}

	public override void Draw()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (PopOverControl.IsInPopup && !partOfPopup)
		{
			return;
		}
		s_sfxPlayedThisFrame = false;
		if (m_chuckWait > 0f)
		{
			return;
		}
		if (LOCKED_BANANA_TYPE == -1)
		{
			LOCKED_BANANA_TYPE = FruitType("banana_locked");
		}
		int index = ((m_isLockedMenuButton && Game.isWP7TrialMode()) ? LOCKED_BANANA_TYPE : m_fruitType);
		if (!m_isSliced)
		{
			if (s_fruitModels[2][index] != null)
			{
				Matrix val = default(Matrix);
				Matrix.CreateScale(ref m_cur_scale, out val);
				val *= Matrix.CreateFromQuaternion(m_rotation_piece[0]);
				val *= Matrix.CreateTranslation(m_pos + Vector3.UnitZ * m_z);
				s_fruitModels[2][index].Draw(val);
			}
		}
		else
		{
			if (GetFruitInfo().superFruit)
			{
				return;
			}
			Matrix val2 = default(Matrix);
			for (int i = 0; i < 2; i++)
			{
				if (s_fruitModels[i][index] != null)
				{
					Matrix.CreateScale(ref m_cur_scale, out val2);
					val2 *= Matrix.CreateFromQuaternion(m_rotation_piece[i]);
					Vector3 val3 = m_pos;
					if (i != 0)
					{
						val3 = m_pos2;
					}
					val3.Z += m_z;
					val2 *= Matrix.CreateTranslation(val3);
					s_fruitModels[i][index].Draw(val2);
				}
			}
		}
	}

	public override void DrawUpdate(float dt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_separation *= 0.9f;
		if (m_isSliced || m_chuckWait > 0f)
		{
			return;
		}
		if (m_gravity.X == 0f)
		{
			if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE && (SlashEntity.ModPowerMask & 0x20) == 32)
			{
				if (m_pos.X < (0f - Game.SCREEN_WIDTH) * 0.4f)
				{
					m_pos.X = (0f - Game.SCREEN_WIDTH) * 0.4f;
					ref Vector3 vel = ref m_vel;
					vel.X *= -1f;
				}
				if (m_pos.X > Game.SCREEN_WIDTH * 0.4f)
				{
					m_pos.X = Game.SCREEN_WIDTH * 0.4f;
					ref Vector3 vel2 = ref m_vel;
					vel2.X *= -1f;
				}
				return;
			}
			if (m_pos.X < (0f - Game.SCREEN_WIDTH) * 0.4f)
			{
				ref Vector3 vel3 = ref m_vel;
				vel3.X += dt * 16f;
				ref Vector3 separation = ref m_separation;
				separation.X += 20f;
			}
			if (m_pos.X > Game.SCREEN_WIDTH * 0.4f)
			{
				ref Vector3 vel4 = ref m_vel;
				vel4.X -= dt * 16f;
				ref Vector3 separation2 = ref m_separation;
				separation2.X -= 20f;
			}
		}
		else if (m_gravity.Y == 0f)
		{
			if (m_pos.Y < (0f - Game.SCREEN_HEIGHT) * 0.4f)
			{
				ref Vector3 vel5 = ref m_vel;
				vel5.Y += dt * 16f;
				ref Vector3 separation3 = ref m_separation;
				separation3.Y += 20f;
			}
			if (m_pos.Y > Game.SCREEN_HEIGHT * 0.4f)
			{
				ref Vector3 vel6 = ref m_vel;
				vel6.Y -= dt * 16f;
				ref Vector3 separation4 = ref m_separation;
				separation4.Y -= 20f;
			}
		}
	}

	public override bool CollisionResponse(Entity p_ent2, uint col_1, uint col_2, ref Vector3 proj)
	{
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		if (PopOverControl.IsInPopup && !partOfPopup)
		{
			return false;
		}
		if (m_isSliced || m_sliceWait > -1f)
		{
			return true;
		}
		bool flag = !GetFruitInfo().superFruit;
		int currentScore = Game.game_work.currentScore;
		if (flag)
		{
			if (currentScore < 2 || !fruitInfo[m_fruitType].canBeCritical || Game.game_work.gameOver || Game.game_work.hitBombTime > 0f)
			{
				m_isCritical = false;
			}
			else
			{
				Game.game_work.criticalChance = Mortar.Math.MAX(2, Game.game_work.criticalChance - 1);
				m_isCritical = WaveManager.GetInstance().GetCriticalChance() > 0f && Mortar.Math.g_random.Rand32((uint)Mortar.Math.MAX((float)Mortar.Math.MIN(Game.game_work.criticalChance, CRITICAL_CHANCE) / WaveManager.GetInstance().GetCriticalChance(), 1f)) == 0;
				if (m_isCritical)
				{
					Game.game_work.criticalChance = CRITICAL_CHANCE + CRITICAL_CHANCE_START_INC;
				}
			}
			if (!Game.game_work.inRetrySequence || !IsOffscreen())
			{
				if (m_isCritical)
				{
					SoundManager.GetInstance().SFXPlay("Critical");
					if (m_fruitType == FruitType("mango"))
					{
						AchievementManager.AwardAchievementWP7("Mango Magic");
					}
				}
				else
				{
					for (int i = 0; i < fruitInfo[m_fruitType].numImpactSounds; i++)
					{
						SoundManager.GetInstance().SFXPlay(fruitInfo[m_fruitType].impactSounds[i].file);
					}
				}
			}
		}
		else
		{
			m_isCritical = false;
		}
		m_splashEffectPos = m_pos;
		m_sliceWait = SLICE_DELAY;
		float v = proj.Length() * 0.1f;
		float num = Mortar.Math.CLAMP(v, MIN_SPEED, MAX_SPEED);
		if (m_isCritical)
		{
			num = Mortar.Math.CLAMP(v, MAX_SPEED * 0.75f, MAX_SPEED);
			m_sliceWait *= 2.5f;
			GameTask.CriticalFlash(m_pos, CRITICAL_COLOUR);
		}
		else if ((GetFruitInfo().score == 50 || GetFruitInfo().superFruit) && !Game.game_work.gameOver && m_fruitType == 15)
		{
			num = Mortar.Math.CLAMP(v, MAX_SPEED * 0.75f, MAX_SPEED);
			m_sliceWait *= 0.5f;
			GameTask.CriticalFlash(m_pos, new Color(255, 255, 255, 128));
			MissControl free = MissControl.GetFree();
			free.MakeRare(m_pos);
		}
		m_slicedAngle = Mortar.Math.Atan2Idx(proj.X, proj.Y);
		m_slicedMag = num;
		for (int j = 0; j < 2; j++)
		{
			if (m_slicedEmitter[j] != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(m_slicedEmitter[j]);
			}
			m_slicedEmitter[j] = null;
		}
		if (Game.game_work.inRetrySequence && IsOffscreen())
		{
			return false;
		}
		if (flag && PSPParticleManager.GetInstance().RepulsiveRadius <= 0f)
		{
			GameTask.AddSlice(m_pos, (0f - (float)(int)m_slicedAngle) / 182f + 90f, num * 0.4f, m_isCritical);
		}
		uint hash = fruitInfo[m_fruitType].hash[0];
		if (m_isCritical)
		{
			hash = StringFunctions.StringHash("crit_hit");
		}
		if (flag && PSPParticleManager.GetInstance().EmitterExists(hash))
		{
			PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(hash, null);
			if (pSPParticleEmitter != null)
			{
				pSPParticleEmitter.sinz = 0f - Mortar.Math.SinIdx((ushort)(-m_slicedAngle));
				pSPParticleEmitter.cosz = Mortar.Math.CosIdx((ushort)(-m_slicedAngle));
				pSPParticleEmitter.pos = m_pos;
			}
		}
		hash = fruitInfo[m_fruitType].hash[3];
		if (m_isCritical)
		{
			hash = StringFunctions.StringHash("crit_hit_stars");
		}
		if (flag && PSPParticleManager.GetInstance().EmitterExists(hash))
		{
			m_slicedEmitter[0] = PSPParticleManager.GetInstance().AddEmitter(hash, null);
			m_slicedEmitter[1] = PSPParticleManager.GetInstance().AddEmitter(hash, null);
			if (m_slicedEmitter[0] != null)
			{
				m_slicedEmitter[0].pos = m_pos;
			}
			if (m_slicedEmitter[1] != null)
			{
				m_slicedEmitter[1].pos = m_pos2;
			}
		}
		if (Game.game_work.inRetrySequence)
		{
			return false;
		}
		AchievementManager.GetInstance().UnlockSpecificOrderAchievement(fruitInfo[m_fruitType].hash[0]);
		if (p_ent2 != null && !m_disabled && (!Game.game_work.gameOver || ((Game.game_work.gameMode == Game.GAME_MODE.GM_ZEN || Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE) && Game.game_work.gameOverTransition < 0.95f && Game.game_work.gameOverTransition > -0.1f)) && !Game.InViewer() && p_ent2 != null)
		{
			if (fruitInfo[m_fruitType].powers != null)
			{
				PowerUpManager.GetInstance().ActivatePower(fruitInfo[m_fruitType].powers.RandomPower(), m_pos);
			}
			int num2 = (m_isCritical ? (CRITICAL_SCORE + fruitInfo[m_fruitType].score) : fruitInfo[m_fruitType].score);
			if (s_consecutiveType != m_fruitType || GetFruitInfo().superFruit)
			{
				s_consecutiveType = m_fruitType;
				while (s_consecutiveCount > 1 && Game.game_work.gameMode == Game.GAME_MODE.GM_CASINO)
				{
					s_consecutiveCount--;
					num2 += 1 << s_consecutiveCount;
				}
				s_consecutiveCount = 0;
			}
			else if (s_consecutiveCount >= 1 && Game.game_work.gameMode == Game.GAME_MODE.GM_CASINO)
			{
				string filename = $"combo-{s_consecutiveCount}";
				SoundManager.GetInstance().SFXPlay(filename);
			}
			s_consecutiveCount++;
			if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
			{
				WaveManager.GetInstance().AddToSpeedLossTime(0.05f);
				uint hash2 = StringFunctions.StringHash("first_fruit");
				uint hash3 = StringFunctions.StringHash("last_fruit");
				if (Game.game_work.saveData.GetTotal(hash2) <= 0)
				{
					Game.game_work.saveData.AddToTotal("first_fruit", hash2, 1 + m_fruitType, universal: false, achievementUpload: false);
				}
				Game.game_work.saveData.AddToTotal("last_fruit", hash3, 1 + m_fruitType - Game.game_work.saveData.GetTotal(hash3), universal: false, achievementUpload: false);
			}
			Game.AddToCurrentScore(num2, m_forPlayer);
			FruitSliced(this, num2, p_ent2);
			if (m_fruitSliced != null)
			{
				m_fruitSliced(this, num2, p_ent2);
			}
			uint hash4 = StringFunctions.StringHash("crit");
			AchievementManager instance = AchievementManager.GetInstance();
			if (!Game.game_work.hasDroppedFruit)
			{
				instance.UnlockScoreUnsulliedAchievement(Game.game_work.currentScore);
			}
			instance.UnlockConsecutiveAchievement(s_consecutiveCount, fruitInfo[m_fruitType].hash[0]);
			Game.game_work.saveData.AddToTotal(fruitInfo[m_fruitType].fruitName, fruitInfo[m_fruitType].hash[0], 1, universal: false, achievementUpload: false);
			Game.game_work.saveData.AddToTotal(fruitInfo[m_fruitType].fruitNameTotal, fruitInfo[m_fruitType].hash[4], 1, universal: true, achievementUpload: false);
			int num3 = 0;
			if (fruitInfo[m_fruitType].coinsMax > 0)
			{
				num3 = (int)((fruitInfo[m_fruitType].coinsMin >= fruitInfo[m_fruitType].coinsMax) ? fruitInfo[m_fruitType].coinsMin : (Mortar.Math.g_random.Rand32((uint)(fruitInfo[m_fruitType].coinsMax - fruitInfo[m_fruitType].coinsMin)) + fruitInfo[m_fruitType].coinsMin));
			}
			if (m_isCritical)
			{
				num3 *= CRITICAL_SCORE / 2;
				Game.game_work.saveData.AddToTotal("crit", hash4, 1, universal: false, achievementUpload: false);
				string text = fruitInfo[m_fruitType].fruitName + "%scrit";
				uint hash5 = StringFunctions.StringHash(text);
				Game.game_work.saveData.AddToTotal(text, hash5, 1, universal: false, achievementUpload: false);
			}
			if (num3 > 0)
			{
				Coin.MakeCoins(num3, 1, m_pos, m_slicedAngle, Mortar.Math.DEGREE_TO_IDX(Mortar.Math.MIN(360, 45 + num3 * 45)));
			}
		}
		else if (GetFruitInfo().superFruit && p_ent2 != null)
		{
			FruitSliced(this, 0, p_ent2);
			m_fruitSliced(this, 0, p_ent2);
		}
		return false;
	}

	public void Slice()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		m_sliceWait = 0f;
		ushort num = (ushort)(Mortar.Math.MAX(Mortar.Math.g_random.Rand32((uint)RANDOM_ANGLE_SLICE), RANDOM_ANGLE_SLICE >> 1) / 5f);
		ushort num2 = (ushort)(Mortar.Math.MAX(Mortar.Math.g_random.Rand32((uint)RANDOM_ANGLE_SLICE), RANDOM_ANGLE_SLICE >> 1) / 5f);
		Matrix val = Matrix.CreateFromQuaternion(m_rotation_piece[0]);
		Vector3 val2 = new Vector3(0f, 0f, 1f);
		Vector3 val3 = default(Vector3);
		Vector3.Transform(ref val2, ref val, out val3);
		if (Game.game_work.gameOver)
		{
			m_isCritical = false;
		}
		bool flag = false;
		if (GetFruitInfo().superFruit)
		{
			flag = true;
		}
		else if (Mortar.Math.ABS(val3.Y) + Mortar.Math.ABS(val3.X) > 0f)
		{
			float to = 360f - (float)(int)(ushort)(Mortar.Math.Atan2Idx(val3.Y, val3.X) - Mortar.Math.DEGREE_TO_IDX(90f)) / 182f;
			float num3 = (float)(int)(ushort)(m_slicedAngle - Mortar.Math.DEGREE_TO_IDX(180f)) / 182f;
			if (GetSmallestDelta(to, num3) < 0f)
			{
				flag = true;
			}
		}
		float num4 = m_slicedMag;
		int num5 = Mortar.Math.g_random.Rand32(2) + 2;
		if (m_isCritical)
		{
			GameTask.AddSlice(m_pos, (0f - (float)(int)m_slicedAngle) / 182f + 60f, num4 * 0.4f * 0.7f, isBlue: true);
			GameTask.AddSlice(m_pos, (0f - (float)(int)m_slicedAngle) / 182f - 60f, num4 * 0.4f * 0.7f, isBlue: true);
			num4 *= 1.5f;
			num5 = CRITICAL_SPLATS;
			MissControl free = MissControl.GetFree();
			free.MakeCritical(m_pos, m_forPlayer);
		}
		if (fruitInfo[m_fruitType].score == 50 || GetFruitInfo().superFruit)
		{
			num4 *= 1.5f;
			num5 = CRITICAL_SPLATS;
		}
		if (Game.game_work.inRetrySequence && IsOffscreen())
		{
			num5 = 0;
		}
		num5 /= 2;
		for (int i = 0; i < num5; i++)
		{
			ushort idx = (ushort)Mortar.Math.g_random.Rand32((uint)Mortar.Math.DEGREE_TO_IDX(360f));
			float num6 = (Mortar.Math.g_random.RandF(0.5f) + 1f) * num4 * (5f + (float)i * 0.2f);
			SplatEntity free2 = SplatEntity.GetFree();
			int num7 = m_fruitType;
			if (m_isCritical)
			{
				num7 += MAX_FRUIT_TYPES;
			}
			free2.MakeSplat(m_pos, new Vector3(Mortar.Math.SinIdx(idx) * num6, Mortar.Math.CosIdx(idx) * num6, 0f), canBeLong: false, GetFruitInfo().superFruit, num7);
			ref Vector3 vel = ref free2.m_vel;
			vel.Z *= Mortar.Math.CLAMP(1f - (float)(i - 2) / (float)num5, 0.3f, 1f);
			if (i > 2)
			{
				ref Vector3 vel2 = ref free2.m_vel;
				vel2.X *= CRITICAL_SPLAT_SPREAD;
				ref Vector3 vel3 = ref free2.m_vel;
				vel3.Y *= CRITICAL_SPLAT_SPREAD;
				free2.m_cur_scale *= CRITICAL_SPLAT_SCALE;
			}
		}
		if (num5 > 0)
		{
			string filename = $"Clean-Slice-{Mortar.Math.g_random.Rand32(3) + 1}";
			SoundManager.GetInstance().SFXPlay(filename);
		}
		float hitInfluence = fruitInfo[m_fruitType].hitInfluence;
		num = (ushort)(Mortar.Math.MAX(Mortar.Math.g_random.Rand32((uint)RANDOM_ANGLE_SLICE), RANDOM_ANGLE_SLICE >> 1) * (1f - hitInfluence) * 4f);
		num2 = (ushort)(Mortar.Math.MAX(Mortar.Math.g_random.Rand32((uint)RANDOM_ANGLE_SLICE), RANDOM_ANGLE_SLICE >> 1) * (1f - hitInfluence) * 4f);
		ushort num8 = (ushort)(m_slicedAngle - num);
		ushort num9 = (ushort)(m_slicedAngle + num2);
		if (flag)
		{
			m_slicedAngle += Mortar.Math.DEGREE_TO_IDX(180f);
			num8 = (ushort)(m_slicedAngle - num2);
			num9 = (ushort)(m_slicedAngle + num);
		}
		float num10;
		float num11;
		if (flag)
		{
			num10 = Mortar.Math.SinIdx((ushort)(num8 + Mortar.Math.DEGREE_TO_IDX(180f))) * num4;
			num11 = Mortar.Math.CosIdx((ushort)(num8 + Mortar.Math.DEGREE_TO_IDX(180f))) * num4;
		}
		else
		{
			num10 = Mortar.Math.SinIdx(num9) * num4;
			num11 = Mortar.Math.CosIdx(num9) * num4;
		}
		m_vel2 = new Vector3(num10, num11, 0f) * hitInfluence + m_vel * (1f - hitInfluence);
		if (flag)
		{
			num10 = Mortar.Math.SinIdx((ushort)(num9 + Mortar.Math.DEGREE_TO_IDX(180f))) * num4;
			num11 = Mortar.Math.CosIdx((ushort)(num9 + Mortar.Math.DEGREE_TO_IDX(180f))) * num4;
		}
		else
		{
			num10 = Mortar.Math.SinIdx(num8) * num4;
			num11 = Mortar.Math.CosIdx(num8) * num4;
		}
		m_vel = new Vector3(num10, num11, 0f) * hitInfluence + m_vel * (1f - hitInfluence);
		if (m_isCritical || fruitInfo[m_fruitType].score == 50 || GetFruitInfo().superFruit)
		{
			num10 = Mortar.Math.SinIdx((ushort)(m_slicedAngle + Mortar.Math.DEGREE_TO_IDX(90f))) * num4;
			num11 = Mortar.Math.CosIdx((ushort)(m_slicedAngle + Mortar.Math.DEGREE_TO_IDX(90f))) * num4;
			m_vel2 = new Vector3(num10, num11, 0f) * 1.75f;
			num10 = Mortar.Math.SinIdx((ushort)(m_slicedAngle - Mortar.Math.DEGREE_TO_IDX(90f))) * num4;
			num11 = Mortar.Math.CosIdx((ushort)(m_slicedAngle - Mortar.Math.DEGREE_TO_IDX(90f))) * num4;
			m_vel = new Vector3(num10, num11, 0f) * 1.75f;
		}
		else
		{
			Game.MoveFruitZPositionToBack(ref m_z);
		}
		m_isSliced = true;
		SetupSliceRotations(GetFruitInfo().superFruit, flag);
	}

	private void SetupSliceRotations(bool openOutwards, bool upsideDown)
	{
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 2; i++)
		{
			float num = Mortar.Math.ABS(m_rotation_speed[i].X) + Mortar.Math.ABS(m_rotation_speed[i].Y) + Mortar.Math.ABS(m_rotation_speed[i].Z);
			num *= (m_isCritical ? 2f : 0.5f);
			if (openOutwards)
			{
				float angle = i * 180;
				ref Vector3 reference = ref m_rotation_axes[i, 0];
				reference = new Vector3(Mortar.Math.CosIdx((ushort)(-m_slicedAngle + Mortar.Math.DEGREE_TO_IDX(90f))), Mortar.Math.SinIdx((ushort)(-m_slicedAngle + Mortar.Math.DEGREE_TO_IDX(90f))), 0f);
				ref Vector3 reference2 = ref m_rotation_axes[i, 1];
				reference2 = (upsideDown ? Vector3.UnitZ : (Vector3.UnitZ * -1f));
				ref Vector3 reference3 = ref m_rotation_axes[i, 2];
				reference3 = new Vector3(Mortar.Math.CosIdx((ushort)(-m_slicedAngle + Mortar.Math.DEGREE_TO_IDX(angle))), Mortar.Math.SinIdx((ushort)(-m_slicedAngle + Mortar.Math.DEGREE_TO_IDX(angle))), 0f);
				if (i == 1)
				{
					ref Vector3 reference4 = ref m_rotation_axes[i, 0];
					reference4 *= -1f;
					ref Vector3 reference5 = ref m_rotation_axes[i, 1];
					reference5 *= -1f;
					ref Vector3 reference6 = ref m_rotation_axes[i, 2];
					reference6 *= -1f;
				}
				float num2 = num * (Mortar.Math.g_random.RandF(0.5f) + 0.75f);
				float num3 = num * (Mortar.Math.g_random.RandF(0.5f) + 0.75f);
				float num4 = (0f - num) * (Mortar.Math.g_random.RandF(0.5f) + 0.75f);
				if (Mortar.Math.g_random.Rand32(100) <= 1)
				{
					num2 = 0f - num2;
				}
				if (Mortar.Math.g_random.Rand32(3) > 0)
				{
					num3 *= 0.2f;
				}
				else
				{
					num4 *= 0.2f;
				}
				ref Vector3 reference7 = ref m_rotation_speed[i];
				reference7 = new Vector3(num2, num3, num4);
			}
			else
			{
				ref Vector3 reference8 = ref m_rotation_axes[i, 0];
				reference8 = Vector3.UnitX;
				ref Vector3 reference9 = ref m_rotation_axes[i, 1];
				reference9 = Vector3.UnitY;
				ref Vector3 reference10 = ref m_rotation_axes[i, 2];
				reference10 = Vector3.UnitZ;
				float num5 = num * (Mortar.Math.g_random.RandF(0.5f) + 0.75f);
				float num6 = num * (Mortar.Math.g_random.RandF(0.5f) + 0.75f);
				if (upsideDown)
				{
					if (Mortar.Math.g_random.Rand32(5) > 1)
					{
						num5 = 0f - num5;
					}
					num5 = ((i == 1) ? (0f - num5) : num5);
					num6 = ((i == 0) ? (0f - num6) : num6);
				}
				else
				{
					if (Mortar.Math.g_random.Rand32(5) <= 1)
					{
						num5 = 0f - num5;
					}
					num5 = ((i == 0) ? (0f - num5) : num5);
					num6 = ((i == 1) ? (0f - num6) : num6);
				}
				bool flag = Mortar.Math.g_random.Rand32(3) > 0;
				ref Vector3 reference11 = ref m_rotation_speed[i];
				reference11 = new Vector3(flag ? (num * (Mortar.Math.g_random.RandF(0.3f) - 0.1f)) : Mortar.Math.ABS(num5 * 1.5f), flag ? num5 : 0f, 0f - num6);
			}
			ref Quaternion reference12 = ref m_rotation_piece[i];
			reference12 = Quaternion.CreateFromAxisAngle(new Vector3(0f, 0f, 1f), Mortar.Math.IDX_TO_RADIANS(Mortar.Math.DEGREE_TO_IDX(90f)));
			Quaternion val = Quaternion.CreateFromAxisAngle(new Vector3(0f, 0f, 1f), Mortar.Math.IDX_TO_RADIANS(m_slicedAngle));
			ref Quaternion reference13 = ref m_rotation_piece[i];
			reference13 = val * m_rotation_piece[i];
		}
	}

	public void Disable()
	{
		m_disabled = true;
	}

	public int GetFruitType()
	{
		return m_fruitType;
	}

	public void SetFruitType(int type)
	{
		SetFruitType(type, 1f);
	}

	public void SetFruitType(int type, float scaler)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		m_fruitType = (byte)type;
		m_cur_scale = Vector3.One * fruitInfo[m_fruitType].scale * 0.01f * scaler;
		m_orig_scale = m_cur_scale;
		if (fruitInfo[m_fruitType].scale * 0.5f + fruitInfo[m_fruitType].collision > 0f)
		{
			if (m_col_box == null)
			{
				m_col_box = new ColSphere();
			}
			((ColSphere)m_col_box).centre = new Vector3(m_pos.X, m_pos.Y, 0f);
			((ColSphere)m_col_box).Radius = (fruitInfo[m_fruitType].scale * 0.5f + fruitInfo[m_fruitType].collision) * scaler;
		}
		else
		{
			m_col_box = null;
		}
	}

	public void EnableGravity(bool state)
	{
		m_gravityEnabled = state;
	}

	public bool GravityEnabled()
	{
		return m_gravityEnabled;
	}

	private float GetSmallestDelta(float to, float from)
	{
		float num = to - from;
		if (Mortar.Math.Abs(num) > 180f)
		{
			num = ((to > from) ? (num - 360f) : (to + 360f - from));
		}
		return num;
	}

	public bool CollideWithFruit()
	{
		return false;
	}

	public bool CheckHasGoneOffsceen()
	{
		bool isSliced = m_isSliced;
		if (isSliced && Mortar.Math.Abs(m_gravity.X) > 0f && (m_pos.Y <= 0f - (Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) || m_pos.Y >= Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) && (m_pos2.Y <= 0f - (Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) || m_pos2.Y >= Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f))
		{
			return true;
		}
		bool flag = false;
		if (m_gravity.Y < 0f)
		{
			if (m_pos.Y > Game.SCREEN_HEIGHT * 0.75f && isSliced)
			{
				m_pos.Y = 0f - Game.SCREEN_HEIGHT;
				m_vel.Y = -1f;
			}
			if (m_pos2.Y > Game.SCREEN_HEIGHT * 0.75f && isSliced)
			{
				m_pos2.Y = 0f - Game.SCREEN_HEIGHT;
				m_vel2.Y = -1f;
			}
			if (m_pos.Y <= 0f - (Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) && m_vel.Y < 0f)
			{
				flag = true;
			}
			if (flag && m_sliceWait <= 0f && m_pos2.Y <= 0f - (Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) && m_vel2.Y < 0f)
			{
				return true;
			}
			if (isSliced && (m_pos.X <= 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) || m_pos.X >= Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) && (m_pos2.X <= 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) || m_pos2.X >= Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f))
			{
				return true;
			}
		}
		if (m_gravity.Y > 0f)
		{
			if (m_pos.Y < (0f - Game.SCREEN_HEIGHT) * 0.75f && isSliced)
			{
				m_pos.Y = Game.SCREEN_HEIGHT;
				m_vel.Y = 1f;
			}
			if (m_pos2.Y < (0f - Game.SCREEN_HEIGHT) * 0.75f && isSliced)
			{
				m_pos2.Y = Game.SCREEN_HEIGHT;
				m_vel2.Y = 1f;
			}
			if (m_pos.Y >= Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f && m_vel.Y > 0f)
			{
				flag = true;
			}
			if (flag && m_sliceWait <= 0f && m_pos2.Y >= Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f && m_vel2.Y > 0f)
			{
				return true;
			}
			if (isSliced && (m_pos.X <= 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) || m_pos.X >= Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) && (m_pos2.X <= 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) || m_pos2.X >= Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f))
			{
				return true;
			}
		}
		if (m_gravity.X < 0f)
		{
			if (isSliced && m_pos.X > Game.SCREEN_WIDTH * 0.75f)
			{
				m_pos.X = 0f - Game.SCREEN_WIDTH;
				m_vel.X = -1f;
			}
			if (isSliced && m_pos2.X > Game.SCREEN_WIDTH * 0.75f)
			{
				m_pos2.X = 0f - Game.SCREEN_WIDTH;
				m_vel2.X = -1f;
			}
			if (m_pos.X <= 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) && m_vel.X < 0f)
			{
				flag = true;
			}
			if (flag && m_sliceWait <= 0f && m_pos2.X <= 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) && m_vel2.X < 0f)
			{
				return true;
			}
		}
		if (m_gravity.X > 0f)
		{
			if (isSliced && m_pos.X < (0f - Game.SCREEN_WIDTH) * 0.75f)
			{
				m_pos.X = Game.SCREEN_WIDTH;
				m_vel.X = 1f;
			}
			if (isSliced && m_pos2.X < (0f - Game.SCREEN_WIDTH) * 0.75f)
			{
				m_pos2.X = Game.SCREEN_WIDTH;
				m_vel2.X = 1f;
			}
			if (m_pos.X >= Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f && m_vel.X > 0f)
			{
				flag = true;
			}
			if (flag && m_sliceWait <= 0f && m_pos2.X >= Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f && m_vel2.X > 0f)
			{
				return true;
			}
		}
		return false;
	}

	public static int FruitType(string type)
	{
		return FruitType(type, returnRandIfFalse: false);
	}

	public static int FruitType(string type, bool returnRandIfFalse)
	{
		if (type != null && type.Length > 0)
		{
			uint num = StringFunctions.StringHash(type);
			for (int i = 0; i < MAX_FRUIT_TYPES; i++)
			{
				if (fruitInfo[i].hash[0] == num || fruitInfo[i].hash[1] == num)
				{
					return i;
				}
			}
		}
		if (!returnRandIfFalse)
		{
			return -1;
		}
		return (int)Mortar.Math.g_random.Rand32((uint)(MAX_FRUIT_TYPES - 1));
	}

	public static int RandomFruit(bool canUseCoiners)
	{
		if (totalChance <= 0)
		{
			totalChance = 0;
			for (int i = 0; i < MAX_FRUIT_TYPES; i++)
			{
				totalChance += fruitInfo[i].chance;
				fruitInfo[i].totalChance = totalChance;
				if (fruitInfo[i].coinsMax <= 0)
				{
					totalChanceNonCoined += fruitInfo[i].chance;
				}
				if (fruitInfo[i].canBeCritical)
				{
					totalChanceOfCritical += fruitInfo[i].chance;
					if (fruitInfo[i].coinsMax <= 0)
					{
						totalChanceOfCriticalNonCoined += fruitInfo[i].chance;
					}
				}
				fruitInfo[i].totalChanceOfCritical = totalChanceOfCritical;
			}
		}
		if (WaveManager.GetInstance().CriticalMode())
		{
			if (canUseCoiners)
			{
				int num = (int)Mortar.Math.g_random.Rand32((uint)totalChanceOfCritical);
				for (int j = 0; j < MAX_FRUIT_TYPES; j++)
				{
					if (num < fruitInfo[j].totalChanceOfCritical)
					{
						return j;
					}
				}
			}
			else
			{
				int num2 = 0;
				int num3 = Mortar.Math.g_random.Rand32(totalChanceOfCriticalNonCoined);
				for (int k = 0; k < MAX_FRUIT_TYPES; k++)
				{
					if (fruitInfo[k].coinsMax <= 0 && fruitInfo[k].canBeCritical)
					{
						num2 += fruitInfo[k].chance;
						if (num3 < num2)
						{
							return k;
						}
					}
				}
			}
		}
		else if (!canUseCoiners)
		{
			int num4 = 0;
			int num5 = Mortar.Math.g_random.Rand32(totalChanceNonCoined);
			for (int l = 0; l < MAX_FRUIT_TYPES; l++)
			{
				if (fruitInfo[l].coinsMax <= 0)
				{
					num4 += fruitInfo[l].chance;
					if (num5 < num4)
					{
						return l;
					}
				}
			}
		}
		else
		{
			int num6 = Mortar.Math.g_random.Rand32(totalChance);
			for (int m = 0; m < MAX_FRUIT_TYPES; m++)
			{
				if (num6 < fruitInfo[m].totalChance)
				{
					return m;
				}
			}
		}
		return Mortar.Math.g_random.Rand32(MAX_FRUIT_TYPES - 1);
	}

	public static uint FruitTypeHash(int type)
	{
		return fruitInfo[type].hash[0];
	}

	public static string FruitTypeName(int type)
	{
		return fruitInfo[type].fruitName;
	}

	public static Color FruitTypeColor(int type)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (MAX_FRUIT_TYPES == type)
		{
			return CRITICAL_COLOUR;
		}
		return fruitInfo[type].splatColor;
	}

	public static Color FruitFactColor(int type)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return fruitInfo[type].factColor;
	}

	public static string FruitFactTexture(int type)
	{
		return fruitInfo[type].factTextureName;
	}

	public static FRUIT_INFO FruitInfo(int type)
	{
		return fruitInfo[type];
	}

	public static bool LoadInfo()
	{
		if (fruitInfo != null)
		{
			return false;
		}
		XDocument xDocument = MortarXml.Load("xmlwp7/fruitList.xml");
		if (xDocument != null)
		{
			XElement xElement = xDocument.Element("fruitInfoFile");
			XElement xElement2 = xElement.Element("critical");
			if (xElement2 != null)
			{
				XAttribute xAttribute = xElement2.Attribute("colour");
				if (xAttribute != null && xAttribute.Value.Length > 0)
				{
					string value = xAttribute.Value;
					int[] array = new int[4] { 255, 255, 255, 255 };
					int num = 0;
					for (int i = 0; i < 4; i++)
					{
						int num2 = value.IndexOf(',', num);
						if (num2 == -1)
						{
							num2 = value.Length;
						}
						array[i] = MParser.ParseInt(value.Substring(num, num2 - num).Trim());
						if (num2 == value.Length)
						{
							break;
						}
						num = num2 + 1;
					}
					CRITICAL_COLOUR.A = (byte)array[0];
					CRITICAL_COLOUR.R = (byte)array[1];
					CRITICAL_COLOUR.G = (byte)array[2];
					CRITICAL_COLOUR.B = (byte)array[3];
				}
				xElement2.QueryIntAttribute("new_life_at", ref NEW_LIFE_AT);
				xElement2.QueryIntAttribute("score", ref CRITICAL_SCORE);
				xElement2.QueryIntAttribute("chance", ref CRITICAL_CHANCE);
				xElement2.QueryIntAttribute("chance_inc", ref CRITICAL_CHANCE_START_INC);
				xElement2.QueryIntAttribute("splats", ref CRITICAL_SPLATS);
				xElement2.QueryFloatAttribute("scale", ref CRITICAL_SPLAT_SCALE);
				xElement2.QueryFloatAttribute("spread", ref CRITICAL_SPLAT_SPREAD);
				xElement2.QueryFloatAttribute("disappear_speed", ref CRITICAL_DISAPPEAR_SPEED);
			}
			XElement xElement3 = xElement.Element("bomb");
			if (xElement3 != null)
			{
				string text = xElement3.AttributeStr("size");
				if (text != null && text.Length > 0)
				{
					Game.game_work.bombSize = MParser.ParseInt(text);
				}
				text = xElement3.AttributeStr("collision");
				if (text != null && text.Length > 0)
				{
					Game.game_work.bombCollision = MParser.ParseInt(text);
				}
			}
			xElement = xElement.Element("FruitInfo");
			MAX_FRUIT_TYPES = 0;
			while (xElement != null)
			{
				MAX_FRUIT_TYPES++;
				xElement = xElement.NextSiblingElement("FruitInfo");
			}
			fruitInfo = ArrayInit.CreateFilledArray<FRUIT_INFO>(MAX_FRUIT_TYPES);
			xElement = xDocument.Element("fruitInfoFile");
			XElement xElement4 = xElement.Element("FruitInfo");
			int num3 = 0;
			FRUIT_INFO fRUIT_INFO = fruitInfo[num3];
			fRUIT_INFO.icon = null;
			while (xElement4 != null)
			{
				fRUIT_INFO = fruitInfo[num3];
				string text2 = xElement4.AttributeStr("name");
				if (text2 != null)
				{
					fRUIT_INFO.fruitName = text2;
					fRUIT_INFO.hash[0] = StringFunctions.StringHash(fRUIT_INFO.fruitName);
					string text3 = fRUIT_INFO.fruitName.Substring(0, 1);
					string text4 = fRUIT_INFO.fruitName.Substring(1);
					fRUIT_INFO.hash[1] = StringFunctions.StringHash(text3.ToUpper() + text4);
					string s = fRUIT_INFO.fruitName + "_trail";
					fRUIT_INFO.hash[2] = StringFunctions.StringHash(s);
					s = fRUIT_INFO.fruitName + "_sliced";
					fRUIT_INFO.hash[3] = StringFunctions.StringHash(s);
					fRUIT_INFO.fruitNameTotal = fRUIT_INFO.fruitName + "_total";
					fRUIT_INFO.hash[4] = StringFunctions.StringHash(fRUIT_INFO.fruitNameTotal);
					fRUIT_INFO.fruitNamePointTotal = fRUIT_INFO.fruitName + "_point_total";
					fRUIT_INFO.hash[5] = StringFunctions.StringHash(fRUIT_INFO.fruitNamePointTotal);
					s = "textureswp7/hud_" + text2 + ".tex";
					if (TextureManager.TextureFileExists(s))
					{
						fRUIT_INFO.icon = TextureManager.GetInstance().Load(s);
					}
					s = "textureswp7/zen_" + text2 + ".tex";
					if (TextureManager.TextureFileExists(s))
					{
						fRUIT_INFO.zenIcon = TextureManager.GetInstance().Load(s);
					}
				}
				XElement xElement5 = xElement4.FirstChildElement("titles");
				if (xElement5 != null)
				{
					text2 = xElement5.AttributeStr("plural");
					if (text2 != null)
					{
						fRUIT_INFO.fruitPlural = text2;
					}
					else
					{
						fRUIT_INFO.fruitPlural = fRUIT_INFO.fruitName + "s";
					}
					text2 = xElement5.AttributeStr("singular");
					if (text2 != null)
					{
						fRUIT_INFO.fruitSingular = text2;
					}
					else
					{
						fRUIT_INFO.fruitSingular = fRUIT_INFO.fruitName;
					}
				}
				text2 = xElement4.AttributeStr("factTexture");
				if (text2 != null)
				{
					fRUIT_INFO.factTextureName = text2;
				}
				text2 = xElement4.AttributeStr("modelName");
				if (text2 != null)
				{
					fRUIT_INFO.modelName = text2;
				}
				else
				{
					fRUIT_INFO.modelName = fRUIT_INFO.fruitName;
				}
				XElement xElement6 = xElement4.FirstChildElement("colours");
				if (xElement6 != null)
				{
					text2 = xElement6.AttributeStr("colour");
					if (text2 != null)
					{
						int[] array2 = new int[4] { 255, 255, 255, 255 };
						int num4 = 0;
						for (int j = 0; j < 4; j++)
						{
							int num5 = text2.IndexOf(',', num4);
							if (num5 == -1)
							{
								num5 = text2.Length;
							}
							array2[j] = MParser.ParseInt(text2.Substring(num4, num5 - num4).Trim());
							if (num5 == text2.Length)
							{
								break;
							}
							num4 = num5 + 1;
						}
						fRUIT_INFO.splatColor.R = (byte)array2[0];
						fRUIT_INFO.splatColor.G = (byte)array2[1];
						fRUIT_INFO.splatColor.B = (byte)array2[2];
						fRUIT_INFO.splatColor.A = (byte)array2[3];
						if (fRUIT_INFO.splatColor.A == 0)
						{
							fRUIT_INFO.canBeCritical = false;
						}
					}
					text2 = xElement6.AttributeStr("factColour");
					if (text2 != null)
					{
						int[] array3 = new int[3] { 255, 255, 255 };
						int num6 = 0;
						for (int k = 0; k < 3; k++)
						{
							int num7 = text2.IndexOf(',', num6);
							if (num7 == -1)
							{
								num7 = text2.Length;
							}
							array3[k] = MParser.ParseInt(text2.Substring(num6, num7 - num6).Trim());
							if (num7 == text2.Length)
							{
								break;
							}
							num6 = num7 + 1;
						}
						fRUIT_INFO.factColor.R = (byte)array3[0];
						fRUIT_INFO.factColor.G = (byte)array3[1];
						fRUIT_INFO.factColor.B = (byte)array3[2];
						fRUIT_INFO.factColor.A = byte.MaxValue;
					}
				}
				fRUIT_INFO.scale = 25f;
				fRUIT_INFO.collision = 1f;
				fRUIT_INFO.chance = 0;
				fRUIT_INFO.hitInfluence = 0.75f;
				xElement4.QueryFloatAttribute("scale", ref fRUIT_INFO.scale);
				xElement4.QueryFloatAttribute("collision", ref fRUIT_INFO.collision);
				xElement4.QueryFloatAttribute("hitInfluence", ref fRUIT_INFO.hitInfluence);
				xElement4.QueryIntAttribute("chance", ref fRUIT_INFO.chance);
				xElement4.QueryIntAttribute("score", ref fRUIT_INFO.score);
				if (fRUIT_INFO.score == 0)
				{
					fRUIT_INFO.superFruit = true;
				}
				xElement4.QueryIntAttribute("coins", ref fRUIT_INFO.coinsMin);
				fRUIT_INFO.coinsMax = fRUIT_INFO.coinsMin;
				xElement4.QueryIntAttribute("coinsMin", ref fRUIT_INFO.coinsMin);
				xElement4.QueryIntAttribute("coinsMax", ref fRUIT_INFO.coinsMax);
				int value2 = 0;
				xElement4.QueryIntAttribute("hasSplatSeeds", ref value2);
				fRUIT_INFO.hasSplatSeeds = value2 == 1;
				if (fRUIT_INFO.score >= CRITICAL_SCORE)
				{
					fRUIT_INFO.canBeCritical = false;
				}
				text2 = xElement4.AttributeStr("onside");
				text2 = ((text2 == null) ? xElement4.AttributeStr("onSide") : text2);
				fRUIT_INFO.onSide = text2 != null && "true" == text2;
				text2 = xElement4.AttributeStr("onlySprinkle");
				text2 = ((text2 == null) ? xElement4.AttributeStr("onlySprinkle") : text2);
				fRUIT_INFO.onlySprinkle = text2 != null && "true" == text2;
				text2 = xElement4.AttributeStr("noCritical");
				fRUIT_INFO.canBeCritical = ((text2 != null) ? ("true" == text2) : fRUIT_INFO.canBeCritical);
				fRUIT_INFO.factCount = 0;
				fRUIT_INFO.facts = null;
				for (XElement xElement7 = xElement4.Element("fact"); xElement7 != null; xElement7 = xElement7.NextSiblingElement("fact"))
				{
					fRUIT_INFO.factCount++;
				}
				if (fRUIT_INFO.factCount > 0)
				{
					fRUIT_INFO.facts = ArrayInit.CreateFilledArray<FruitFact>(fRUIT_INFO.factCount);
					int num8 = 0;
					for (XElement xElement8 = xElement4.Element("fact"); xElement8 != null; xElement8 = xElement8.NextSiblingElement("fact"))
					{
						FruitFact fruitFact = fRUIT_INFO.facts[num8];
						fruitFact.text = xElement8.GetText();
						num8++;
					}
				}
				fRUIT_INFO.numImpactSounds = 0;
				for (XElement xElement9 = xElement4.Element("impact_sound"); xElement9 != null; xElement9 = xElement9.NextSiblingElement("impact_sound"))
				{
					fRUIT_INFO.numImpactSounds++;
				}
				if (fRUIT_INFO.numImpactSounds > 0)
				{
					fRUIT_INFO.impactSounds = ArrayInit.CreateFilledArray<ImpactSound>(fRUIT_INFO.numImpactSounds);
					int num9 = 0;
					ImpactSound impactSound = fRUIT_INFO.impactSounds[num9];
					int num10 = 0;
					for (XElement xElement10 = xElement4.Element("impact_sound"); xElement10 != null; xElement10 = xElement10.NextSiblingElement("impact_sound"))
					{
						impactSound = fRUIT_INFO.impactSounds[num9];
						xElement10.QueryIntAttribute("chance", ref impactSound.chance);
						num10 = (impactSound.chanceTotal = num10 + impactSound.chance);
						string text5 = xElement10.GetText();
						impactSound.file = text5;
						num9++;
					}
				}
				else
				{
					fRUIT_INFO.numImpactSounds = 1;
					fRUIT_INFO.impactSounds = ArrayInit.CreateFilledArray<ImpactSound>(1);
					fRUIT_INFO.impactSounds[0].file = "Impact-" + fRUIT_INFO.fruitName.Substring(0, 1).ToLower() + fRUIT_INFO.fruitName.Substring(1);
				}
				int num11 = 0;
				for (XElement xElement11 = xElement4.FirstChildElement("power"); xElement11 != null; xElement11 = xElement11.NextSiblingElement("power"))
				{
					num11++;
				}
				if (num11 > 0)
				{
					fRUIT_INFO.powers = new FRUIT_POWERS();
					fRUIT_INFO.powers.numPowerUpTypes = num11;
					fRUIT_INFO.powers.powerUps = ArrayInit.CreateFilledArray<FRUIT_POWER>(num11);
					int num12 = 0;
					int num13 = 0;
					for (XElement xElement12 = xElement4.FirstChildElement("power"); xElement12 != null; xElement12 = xElement12.NextSiblingElement("power"))
					{
						FRUIT_POWER fRUIT_POWER = fRUIT_INFO.powers.powerUps[num12];
						string text6 = xElement12.AttributeStr("name");
						if (text6 != null)
						{
							fRUIT_POWER.powerHash = StringFunctions.StringHash(text6);
						}
						xElement12.QueryIntAttribute("chance", ref fRUIT_POWER.chance);
						num13 = (fRUIT_POWER.totalChance = num13 + fRUIT_POWER.chance);
						num12++;
					}
				}
				num3++;
				xElement4 = xElement4.NextSiblingElement("FruitInfo");
			}
		}
		xDocument = null;
		if (fruitInfo != null)
		{
			for (int l = 0; l < 2; l++)
			{
				s_fruitModels[l] = ArrayInit.CreateInitedList<Model>(MAX_FRUIT_TYPES);
				for (int m = 0; m < MAX_FRUIT_TYPES; m++)
				{
					string modelFileName = $"models/Fruit/{fruitInfo[m].modelName}_{fruitInfo[m].modelName[0]}_piece_{l + 1}.mmd";
					s_fruitModels[l][m] = MeshManager.GetInstance().Load(modelFileName);
				}
			}
			s_fruitModels[2] = ArrayInit.CreateInitedList<Model>(MAX_FRUIT_TYPES);
			for (int n = 0; n < MAX_FRUIT_TYPES; n++)
			{
				string modelFileName = $"models/Fruit/{fruitInfo[n].modelName}_single.mmd";
				if (TextureManager.TextureFileExists(modelFileName))
				{
					s_fruitModels[2][n] = MeshManager.GetInstance().Load(modelFileName);
				}
				else
				{
					s_fruitModels[2][n] = null;
				}
			}
			s_fruitModelsLoaded = true;
		}
		return true;
	}

	public bool IsActive()
	{
		return m_chuckWait <= 0f;
	}

	public void Chuck()
	{
		Chuck(0f);
	}

	public void Chuck(float chuck)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (chuck < 0f)
		{
			chuck = FRUIT_CHUCK_WAIT;
		}
		m_pos2 = m_pos;
		m_chuckWait = chuck;
		uint num = StringFunctions.StringHash("freeze");
		if (fruitInfo[m_fruitType].powers != null && Game.game_work.saveData.timer - chuck < 8f && fruitInfo[m_fruitType].powers.powerUps[0].powerHash != num)
		{
			s_numActivePowerUpFruits--;
			base.m_destroy = true;
		}
	}

	public bool IsOffscreen()
	{
		bool result = false;
		if (Mortar.Math.Abs(m_gravity.Y) > 0f)
		{
			if (m_pos.Y < 0f - (Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) || m_pos.Y > Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f || m_pos2.Y < 0f - (Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f) || m_pos2.Y > Game.SCREEN_HEIGHT / 2f + m_cur_scale.Y * 50f)
			{
				result = true;
			}
		}
		else if (Mortar.Math.Abs(m_gravity.X) > 0f && (m_pos.X < 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) || m_pos.X > Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f || m_pos2.X < 0f - (Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f) || m_pos2.X > Game.SCREEN_WIDTH / 2f + m_cur_scale.Y * 50f))
		{
			result = true;
		}
		return result;
	}

	public float GetWait()
	{
		return m_chuckWait;
	}

	public bool Sliced()
	{
		if (!m_isSliced)
		{
			return m_sliceWait > -1f;
		}
		return true;
	}

	public static string GetFact(Action<int> getType, Action<int> getFact)
	{
		return GetFact(getType, getFact, -1);
	}

	public static string GetFact(Action<int> getType, Action<int> getFact, int type)
	{
		return GetFact(getType, getFact, type, -1);
	}

	public static string GetFact(Action<int> getType, Action<int> getFact, int type, int fact)
	{
		if (type < 0)
		{
			type = Mortar.Math.g_random.Rand32(MAX_FRUIT_TYPES);
		}
		type = Mortar.Math.CLAMP(type, 0, MAX_FRUIT_TYPES - 2);
		if (type == redApple)
		{
			type = apple;
		}
		getType?.Invoke(type);
		if (fact < 0)
		{
			if (fruitInfo[type].factCount <= 0)
			{
				return GetFact(getType, getFact);
			}
			string text = fruitInfo[type].fruitName + "_facts";
			fact = Game.game_work.saveData.AddToTotal(text, StringFunctions.StringHash(text), 1, universal: true, achievementUpload: true) - 1;
			fact %= fruitInfo[type].factCount;
		}
		fact = Mortar.Math.CLAMP(fact, 0, fruitInfo[type].factCount - 1);
		getFact?.Invoke(fact);
		string text2 = fruitInfo[type].facts[fact].text;
		return TheGame.instance.stringTable.GetString(text2);
	}

	public int ForPlayer()
	{
		return m_forPlayer;
	}

	public void SetForPlayer(int forPlayer)
	{
		m_forPlayer = forPlayer;
	}

	public static int GetNumActiveForPlayer(int player)
	{
		return GetNumActiveForPlayer(player, waitForDeath: true);
	}

	public static int GetNumActiveForPlayer(int player, bool waitForDeath)
	{
		LinkedListNode<Entity> iterator = null;
		Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator);
		int num = 0;
		if (waitForDeath)
		{
			while (fruit != null)
			{
				if (player == fruit.ForPlayer())
				{
					num++;
				}
				fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator);
			}
		}
		else
		{
			while (fruit != null)
			{
				if (!fruit.IsActive())
				{
					num++;
				}
				fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator);
			}
		}
		return num;
	}

	public static void ClearUnspawned()
	{
		LinkedListNode<Entity> iterator = null;
		for (Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); fruit != null; fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			if (fruit.GetWait() > 0f)
			{
				fruit.KillFruit();
			}
		}
	}

	public static void CleanupFruit()
	{
		if (s_fruitModelsLoaded)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < MAX_FRUIT_TYPES; j++)
				{
					s_fruitModels[i][j] = null;
				}
				s_fruitModels[i] = null;
			}
		}
		fruitInfo = null;
	}

	public static void RandomStartAngle(ref Quaternion q)
	{
		RandomStartAngle(ref q, faceUp: false);
	}

	public static void RandomStartAngle(ref Quaternion q, bool faceUp)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (faceUp)
		{
			q = Quaternion.CreateFromAxisAngle(new Vector3(-1f, 0f, 0f), (float)(int)Mortar.Math.DEGREE_TO_IDX(290f));
			return;
		}
		Vector3 val = default(Vector3);
		val = new Vector3(Mortar.Math.g_random.RandF(2f) - 1f, Mortar.Math.g_random.RandF(2f) - 1f, Mortar.Math.g_random.RandF(2f) - 1f);
		ushort num = (ushort)Mortar.Math.g_random.Rand32(Mortar.Math.DEGREE_TO_IDX(359f));
		val.Normalize();
		q = Quaternion.CreateFromAxisAngle(new Vector3(val.X, val.Y, val.Z), (float)(int)num);
	}
}
