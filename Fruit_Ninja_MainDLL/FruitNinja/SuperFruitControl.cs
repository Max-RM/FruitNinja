using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class SuperFruitControl : HUDControl3d
{
	public const float SUPER_SOUND_FADE_AMOUNT = 0.125f;

	public const float BACK_TEXT_SIZE = 50f;

	public const float POPIN_START_TIME = -2f;

	public const float POPIN_END_TIME = -1.85f;

	public const float CAM_ZOOM = 0.625f;

	public const float CAM_MOVE_MIN_DIST = 7f;

	public const float CAM_MOVE_MAX_DIST = 15f;

	public const float CAM_MOVE_MAX_RADIUS = 44f;

	public const int MIN_SLICES_BEFORE_NEXT_CAM_MOVE = 0;

	public const int MAX_SLICES_BEFORE_NEXT_CAM_MOVE = 0;

	public const float DIRECTION_CHANGES_MIN = 4f;

	public const float DIRECTION_CHANGES_MAX = 5f;

	public const float DIRECTION_CHANGE_MIN = 40f;

	public const float DIRECTION_CHANGE_MAX = 60f;

	public const float DT_MOD_DURING = 0.1f;

	public const float CAM_MOVE_SPEED = 0.175f;

	public const float SLICES_PER_RAY = 2f;

	public const float JIB_SPLAT_FREQUENCY = 20f;

	public const float SUPER_FRUIT_RAY_SPEED = 60f;

	public const float COLLISION_SCALE = 0.775f;

	public const float MAX_SLICES_PER_SEC = 17.5f;

	public Fruit m_fruit;

	public Fruit m_fruitHandle;

	public float m_timeSinceHit;

	public float m_time;

	public float m_prevTime;

	public int m_hits;

	public SuperFruitHitControl m_lastCreatedHitControl;

	public float m_sliceTime;

	public float m_jumpTime;

	public Vector3 m_shakeDir;

	public Vector3 m_cameraOffset;

	public Vector3 m_cameraOffsetPrev;

	public Vector3 m_cameraOffsetDest;

	public float m_cameraOffsetMoveTime;

	public int m_slicesTillCameraMove;

	public ushort m_cameraOffsetMoveAngle;

	public int m_cameraOffsetDirectionChangeCount;

	private string m_finalAddText;

	private string m_bakedString;

	private Color m_colour;

	private static Dictionary<Fruit, SuperFruitControl> SuperFruitControls = new Dictionary<Fruit, SuperFruitControl>();

	private static Texture ShockWaveTexture;

	private static Model JibletModel = null;

	private float m_splosionRadius;

	private float m_secondarySplosionRadius;

	private Vector3 m_splosionCenter;

	private Vector3 m_splosionCenterBounded;

	private static Color[] baseColours = new Color[3]
	{
		new Color(255, 218, 46),
		new Color(255, 119, 54),
		new Color(137, 46, 255)
	};

	private static int rayNum = 0;

	private static int[] sectorRandomness = new int[8] { 1, 3, 5, 7, 2, 4, 0, 6 };

	public float SLICE_TIME => m_sliceTime;

	public float ZOOM_OUT_SOUND_TIME => m_sliceTime - 0.1f;

	public float EXPLODE_TIME => SLICE_TIME + 0.5f;

	public float EXPLODE_FULL => EXPLODE_TIME + 0.35f;

	public float EXPLODE_FADED => EXPLODE_FULL + 0.25f;

	public float SECONDARY_EXPLODE_TIME => EXPLODE_FULL + 0.55f;

	public float SECONDARY_EXPLODE_FULL => SECONDARY_EXPLODE_TIME + 0.65f;

	public float SECONDARY_EXPLODE_FADED => SECONDARY_EXPLODE_FULL + 0.25f;

	public float DT_MOD_RAMP_UP_START => EXPLODE_FULL + 0.4f;

	public float DT_MOD_RAMP_UP_FINISH => EXPLODE_FULL + 0.6f;

	public float EXPLODE_END => SECONDARY_EXPLODE_FULL + 0.5f;

	public float FINAL_SCORE_IN_TIME => SECONDARY_EXPLODE_TIME + 0.1f;

	public float FINAL_SCORE_FULL_TIME => FINAL_SCORE_IN_TIME + 0.25f;

	public float KILL_FRUIT_TIME => SECONDARY_EXPLODE_FADED + 0.55f;

	public float POPOUT_START_TIME => SECONDARY_EXPLODE_FADED + ((Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE) ? 1.5f : 0.5f);

	public float POPOUT_END_TIME => POPOUT_START_TIME + 0.15f;

	public float SPLOSION_RADIUS_FULL => Mortar.Math.Sqrt(Game.SCREEN_WIDTH * Game.SCREEN_WIDTH + Game.SCREEN_HEIGHT * Game.SCREEN_WIDTH);

	public SuperFruitControl(Fruit fruit)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		m_fruit = fruit;
		m_fruitHandle = fruit;
		m_time = -2f;
		m_prevTime = -2f;
		m_lastCreatedHitControl = null;
		m_finalAddText = "";
		SlashEntity.ComboCanceledEvent = (SlashEntity.ComboCanceled)Delegate.Combine(SlashEntity.ComboCanceledEvent, new SlashEntity.ComboCanceled(ComboCancel));
		m_cameraOffset = (m_cameraOffsetPrev = (m_cameraOffsetDest = Vector3.Zero));
		m_cameraOffsetMoveTime = 1f;
		m_slicesTillCameraMove = (int)Utils.GetRandBetween(0f, 0f);
		m_hits = 1;
		float num = Mortar.Math.g_random.RandF(10f) + 15f;
		if (Mortar.Math.g_random.Rand32(2) > 0)
		{
			num *= -1f;
		}
		float num2 = 0.625f;
		Vector3 val = default(Vector3);
		val = new Vector3(Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(num)), Mortar.Math.CosIdx(Mortar.Math.DEGREE_TO_IDX(num)), 0f);
		Game.game_work.camera.Transition(fruit.m_pos + val * Game.SCREEN_HEIGHT * 0.15f * num2, num2, num, TransitionFin);
		fruit.m_waitingForCombo = true;
		m_fruit.m_sliceWait = 1f;
		((ColSphere)fruit.m_col_box).Radius *= 0.775f;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		fruit.m_z = 10f;
		ChangeText("SLICE!", pop: false);
		m_scale = Vector3.One * 0.5f;
		m_pos = new Vector3(fruit.m_pos.X, fruit.m_pos.Y, 0f);
		m_rotation = 0f - num;
		m_jumpTime = 1f;
		m_shakeDir = Vector3.Zero;
		m_pos += val * Game.SCREEN_HEIGHT * 0.4f * num2;
		WaveManager.GetInstance().paused = true;
		Fruit.ClearUnspawned();
		Bomb.ClearUnspawned();
		Bomb.DeactivateAll();
		SoundManager.GetInstance().SFXPlay("pome-rampdown");
		m_sliceTime = Utils.GetRandBetween(2f, 3f);
		m_timeSinceHit = 0f;
		m_colour = Color.Violet;
	}

	~SuperFruitControl()
	{
		Release();
	}

	public static void LoadContent()
	{
		Fruit.FruitThrown = (Fruit.FruitEvent)Delegate.Combine(Fruit.FruitThrown, new Fruit.FruitEvent(SuperFruitThrown));
		Fruit.FruitSliced = (Fruit.FruitSliceEvent)Delegate.Combine(Fruit.FruitSliced, new Fruit.FruitSliceEvent(SuperFruitSliced));
		SuperFruitGlow.GlowTexture = Texture.Load("textureswp7/rays.tex");
		ShockWaveTexture = Texture.Load("textureswp7/explosion_radius.tex");
		FruitRay.RayTexture = Texture.Load("textureswp7/pomegranate_rays.tex");
		JibletModel = MeshManager.GetInstance().Load("models/fruit/pomegranate_jiblet.mmd");
	}

	public static void UnLoadContent()
	{
		FruitRay.RayTexture = null;
		ShockWaveTexture = null;
		JibletModel = null;
	}

	public static void SuperFruitThrown(Fruit fruit)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (Fruit.FruitInfo(fruit.GetFruitType()).superFruit && !fruit.m_destroy)
		{
			fruit.m_z = -5f;
			if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
			{
				fruit.m_pos = new Vector3(-35f, (0f - Game.SCREEN_HEIGHT) / 2f - 100f, 0f);
				fruit.m_vel = new Vector3(0.5f, 8.5f, 0f);
				fruit.m_gravity.Y = -7.5f;
				fruit.m_gravity.X = 0f;
			}
			else
			{
				fruit.m_pos = new Vector3((0f - Game.SCREEN_WIDTH) / 2f - 100f, -100f, 0f);
				fruit.m_vel = new Vector3(5f, 5f, 0f);
				fruit.m_gravity.Y = -4.5f;
				fruit.m_gravity.X = 0.01f;
			}
			if (WaveManager.GetInstance().w_random.Rand32(100) <= 50)
			{
				ref Vector3 gravity = ref fruit.m_gravity;
				gravity.X *= -1f;
				ref Vector3 pos = ref fruit.m_pos;
				pos.X *= -1f;
				ref Vector3 vel = ref fruit.m_vel;
				vel.X *= -1f;
			}
			Game.game_work.saveData.AddToTotal("super_pomegranates_spawned", StringFunctions.StringHash("super_pomegranates_spawned"), 1, universal: false, achievementUpload: false);
			SuperFruitGlow control = new SuperFruitGlow(fruit);
			Game.game_work.hud.AddControl(control);
			TheGame.pomegranateThrown = true;
		}
	}

	public static void SuperFruitSliced(Fruit fruit, int points, Entity otherEnt)
	{
		if (!Fruit.FruitInfo(fruit.GetFruitType()).superFruit)
		{
			return;
		}
		SuperFruitControl superFruitControl = null;
		try
		{
			if (SuperFruitControls.Count > 0)
			{
				superFruitControl = SuperFruitControls[fruit];
			}
		}
		catch
		{
		}
		if (superFruitControl != null)
		{
			superFruitControl.Sliced(otherEnt);
			return;
		}
		if (Game.FailureEnabled() && Game.game_work.gameOver)
		{
			fruit.m_sliceWait = 0f;
			return;
		}
		superFruitControl = new SuperFruitControl(fruit);
		Game.game_work.hud.AddControl(superFruitControl, toFront: false);
		SuperFruitControls[fruit] = superFruitControl;
	}

	public static bool IsInSuperFruitState()
	{
		return SuperFruitControls.Count > 0;
	}

	public static void Clear()
	{
		SuperFruitControls.Clear();
	}

	private void Sliced(Entity otherEnt)
	{
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		if (m_time < 0f)
		{
			m_time = 0f;
		}
		else if (m_time >= SLICE_TIME || m_timeSinceHit > 1.5f)
		{
			return;
		}
		m_timeSinceHit += 1f;
		m_hits++;
		if (m_slicesTillCameraMove <= 0)
		{
			float num = TransitionFunctions.LerpF(7f, 15f, Mortar.Math.g_random.RandF(1f));
			ushort num2 = 0;
			float num3 = m_cameraOffsetDest.Length();
			if (num3 < 1f)
			{
				num2 = (ushort)Mortar.Math.g_random.Rand32();
			}
			else if (num3 > 44f)
			{
				num2 = (ushort)(Mortar.Math.Atan2Idx(m_cameraOffsetDest.Y, m_cameraOffsetDest.X) + Mortar.Math.DEGREE_TO_IDX(135f) + (ushort)Mortar.Math.g_random.Rand32(Mortar.Math.DEGREE_TO_IDX(90f)));
			}
			else
			{
				Vector3 val = m_cameraOffsetDest - m_cameraOffsetPrev;
				num2 = Mortar.Math.Atan2Idx(val.Y, val.X);
			}
			m_cameraOffsetDest += new Vector3(Mortar.Math.CosIdx(num2), Mortar.Math.SinIdx(num2), 0f) * num;
			m_cameraOffsetPrev = m_cameraOffset;
			m_cameraOffsetMoveTime = 0f;
			m_slicesTillCameraMove = 0;
		}
		else
		{
			m_slicesTillCameraMove--;
		}
		WaveManager.GetInstance().AddToSpeedLossTime(0.1f);
		m_fruit.RemoveTrailParticles();
		m_shakeDir = m_fruit.GetSliceDir(Mortar.Math.DEGREE_TO_IDX(0f - m_rotation)) * m_fruit.m_slicedMag * 3f;
		Vector3 val2 = otherEnt.m_col_box.centre - ((ColLine)otherEnt.m_col_box).Direction;
		Vector3 pos = m_fruit.m_pos;
		pos.Z = m_fruit.m_z - 5f;
		float randBetween = Utils.GetRandBetween(0.8f, 1.1f);
		GameTask.AddSlice(pos, (float)(int)Mortar.Math.Atan2Idx(val2.Y, val2.X) / 182f, randBetween, 0, null, 0.65f);
		GameTask.AddSlice(pos, (float)(int)Mortar.Math.Atan2Idx(val2.Y, val2.X) / 182f, randBetween, 3, m_fruit, 0.65f);
		if (otherEnt != null && otherEnt.m_type == 3)
		{
			((SlashEntity)otherEnt).m_splatsToMake = -1;
		}
		if (m_lastCreatedHitControl != null)
		{
			m_lastCreatedHitControl.m_parent = null;
			m_lastCreatedHitControl.RemoveQuickly();
		}
		uint hash = Fruit.fruitInfo[m_fruit.GetFruitType()].hash[0];
		if (PSPParticleManager.GetInstance().EmitterExists(hash))
		{
			for (int i = 0; i < 2; i++)
			{
				PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(hash, null);
				if (pSPParticleEmitter != null)
				{
					pSPParticleEmitter.sinz = 0f - Mortar.Math.SinIdx((ushort)(-m_fruit.m_slicedAngle));
					pSPParticleEmitter.cosz = Mortar.Math.CosIdx((ushort)(-m_fruit.m_slicedAngle));
					pSPParticleEmitter.pos = otherEnt.m_col_box.centre;
					pSPParticleEmitter.dtMod /= WaveManager.GetInstance().GetAbsoluteDtMod();
					pSPParticleEmitter.sizeScale *= 0.5f;
				}
			}
		}
		ChangeText($"{m_hits} HITS", pop: true);
		string filename = $"pome-slice-{(int)Utils.GetRandBetween(1f, 3f)}";
		SoundManager.GetInstance().SFXPlay(filename, 0u, null, 0, -1);
		if ((float)m_hits % 2f == 1f)
		{
			SpawnRay();
		}
	}

	private void ChangeText(string text, bool pop)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		float t = Mortar.Math.CLAMP((float)m_hits / 70f, 0f, 1f);
		m_colour = Utils.LerpColourFromArray(t, baseColours, 3);
		if (pop)
		{
			m_jumpTime = 0f;
		}
		m_bakedString = text;
		GameTask.Flash();
	}

	private void ComboCancel(SlashEntity slash)
	{
		if (m_fruit != null && m_time < SLICE_TIME)
		{
			m_fruit.m_sliceWait = -1f;
		}
	}

	private void SpawnRay()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		rayNum++;
		int num = rayNum % 8;
		num = sectorRandomness[num];
		float start = ((num < 4) ? (-35f) : 5f);
		float end = ((num < 4) ? (-5f) : 70f);
		float start2 = num % 4 * 90;
		float end2 = num % 4 * 90 + 80;
		ushort num2 = Mortar.Math.DEGREE_TO_IDX(Utils.GetRandBetween(0f, 180f));
		ushort num3 = Mortar.Math.DEGREE_TO_IDX(Utils.GetRandBetween(start, end));
		ushort num4 = Mortar.Math.DEGREE_TO_IDX(Utils.GetRandBetween(start2, end2));
		Quaternion val = Quaternion.CreateFromAxisAngle(new Vector3(0f, 0f, 1f), (float)(int)num2);
		Quaternion val2 = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), (float)(int)num3);
		Quaternion val3 = Quaternion.CreateFromAxisAngle(new Vector3(1f, 0f, 0f), (float)(int)num4);
		Quaternion offset = val * val2 * val3;
		FruitRay fruitRay = (FruitRay)ActorManager.GetInstance().Add(EntityTypes.ENTITY_FRUIT_RAY);
		fruitRay.Init(m_fruit, offset);
	}

	public override void Reset()
	{
		WaveManager.GetInstance().paused = false;
		WaveManager.GetInstance().SetAbsoluteDtMod(1f);
		PSPParticleManager.GetInstance().RepulsiveRadius = 0f;
		PSPParticleManager.GetInstance().RepulsiveStrength = 1f;
		Game.game_work.hud.m_hudAmount = 1f;
		Game.game_work.camera.TransitionOut();
		Game.game_work.camera.m_transitionFinished = null;
		GameTask.UnpauseSlices();
		LinkedListNode<Entity> iterator = null;
		for (FruitRay fruitRay = (FruitRay)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_FRUIT_RAY, ref iterator); fruitRay != null; fruitRay = (FruitRay)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_FRUIT_RAY, ref iterator))
		{
			fruitRay.m_destroy = true;
		}
		m_terminate = true;
	}

	public static bool StopAllPomegranates()
	{
		foreach (KeyValuePair<Fruit, SuperFruitControl> superFruitControl in SuperFruitControls)
		{
			superFruitControl.Value.Reset();
		}
		return true;
	}

	private void StopRays()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		LinkedListNode<Entity> iterator = null;
		for (FruitRay fruitRay = (FruitRay)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_FRUIT_RAY, ref iterator); fruitRay != null; fruitRay = (FruitRay)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_FRUIT_RAY, ref iterator))
		{
			fruitRay.m_fadeOut = true;
		}
		if (m_fruit != null)
		{
			for (int i = 0; i < 2; i++)
			{
				ref Vector3 reference = ref m_fruit.m_rotation_speed[i];
				reference = Vector3.Zero;
			}
		}
	}

	private void SpawnJibs()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		string s = $"{Fruit.FruitInfo(m_fruit.GetFruitType()).fruitName}_explode";
		if (PSPParticleManager.GetInstance().EmitterExists(StringFunctions.StringHash(s)))
		{
			PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash(s), null);
			if (pSPParticleEmitter != null)
			{
				pSPParticleEmitter.canNotBeRepulsed = true;
				pSPParticleEmitter.pos = m_splosionCenter;
				pSPParticleEmitter.cosz = Mortar.Math.CosIdx(m_fruit.m_slicedAngle);
				pSPParticleEmitter.sinz = Mortar.Math.SinIdx(m_fruit.m_slicedAngle);
			}
		}
		int num = 12;
		float randBetween = Utils.GetRandBetween(0f, 360f / (float)num);
		s = $"{Fruit.FruitInfo(m_fruit.GetFruitType()).fruitName}_jiblet";
		uint particles = StringFunctions.StringHash(s);
		for (int i = 0; i < num; i++)
		{
			Jiblet jiblet = (Jiblet)ActorManager.GetInstance().Add(EntityTypes.ENTITY_JIBLET);
			float start = ((float)i + 0.2f) * 360f / (float)num;
			float end = ((float)i + 0.8f) * 360f / (float)num;
			Vector3 val = Utils.VectorFromMagDegree(1f, randBetween + Utils.GetRandBetween(start, end));
			jiblet.Init(m_fruit.GetFruitType(), m_splosionCenter, Utils.GetRandBetween(0.8f, 1.25f), val * Utils.GetRandBetween(500f, 900f), JibletModel, particles, 20f, val * 45f);
		}
	}

	private void ExplodeSuperFruit()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		Matrix orientation = Matrix.CreateFromQuaternion(m_fruit.m_rotation_piece[0]);
		float slicedMag = m_fruit.m_slicedMag;
		int num = 14;
		for (int i = 0; i < num; i++)
		{
			ushort idx = (ushort)Mortar.Math.g_random.Rand32(Mortar.Math.DEGREE_TO_IDX(360f));
			float num2 = (Mortar.Math.g_random.RandF(0.5f) + 1f) * slicedMag * (5f + (float)i * 0.2f);
			SplatEntity free = SplatEntity.GetFree();
			int fruitType = m_fruit.GetFruitType();
			free.MakeSplat(m_fruit.m_pos, new Vector3(Mortar.Math.SinIdx(idx) * num2, Mortar.Math.CosIdx(idx) * num2, 0f), canBeLong: false, noSound: true, fruitType);
			ref Vector3 vel = ref free.m_vel;
			vel.Z *= Mortar.Math.CLAMP(1f - (float)(i - 2) / (float)num, 0.3f, 1f);
		}
		GameTask.CriticalFlash(m_fruit.m_pos, Color.White);
		SoundManager.GetInstance().SFXPlay("pome-burst");
		for (int j = 0; j < 8; j++)
		{
			Vector3 val = new Vector3
			{
				X = ((j / 2 % 2 == 0) ? 1 : (-1)),
				Y = ((j % 2 != 0) ? 1 : (-1)),
				Z = ((j / 4 == 0) ? 1 : (-1))
			};
			val.Normalize();
			Vector3 val2 = Vector3.UnitX * orientation.Translation;
			Jiblet jiblet = (Jiblet)ActorManager.GetInstance().Add(EntityTypes.ENTITY_JIBLET);
			Vector3 val3 = val2;
			val3.Z = 0f;
			val3.Normalize();
			string modelFileName = $"models/Fruit/{Fruit.FruitInfo(m_fruit.GetFruitType()).modelName}_{Fruit.FruitInfo(m_fruit.GetFruitType()).modelName[0]}_piece_{j + 1}.mmd";
			jiblet.Init(m_fruit.GetFruitType(), m_splosionCenter, 1f, val3 * Utils.GetRandBetween(600f, 1000f), MeshManager.GetInstance().Load(modelFileName), 0u, 0f, val3 * 700f);
			jiblet.m_cur_scale = m_fruit.m_cur_scale;
			jiblet.m_time = 0f;
			jiblet.m_orientation = orientation;
		}
		m_fruit.m_cur_scale = Vector3.Zero;
		TheGame.pomegranateThrown = false;
	}

	private void TransitionFin()
	{
		if (Game.game_work.camera.IsTransitionIn() && m_fruit != null)
		{
			m_fruit.m_sliceWait = -1f;
		}
	}

	public override void Release()
	{
		if (SlashEntity.ComboCanceledEvent != null)
		{
			SlashEntity.ComboCanceledEvent = (SlashEntity.ComboCanceled)Delegate.Remove(SlashEntity.ComboCanceledEvent, new SlashEntity.ComboCanceled(ComboCancel));
		}
		SuperFruitControls.Clear();
		StopRays();
		if (m_fruit != null)
		{
			m_fruit.KillFruit();
			m_fruit = null;
		}
	}

	public override void Save()
	{
	}

	public static void SaveSuperFruitState(Fruit fruit, XElement element)
	{
		if (fruit == null)
		{
			return;
		}
		foreach (KeyValuePair<Fruit, SuperFruitControl> superFruitControl in SuperFruitControls)
		{
			SuperFruitState superFruitState = new SuperFruitState();
			superFruitState.time = superFruitControl.Value.m_time;
			superFruitState.sliceTime = superFruitControl.Value.m_sliceTime;
			superFruitState.hits = superFruitControl.Value.m_hits;
			superFruitState.rotation = superFruitControl.Value.m_rotation;
			element.Add(superFruitState.WriteToElement());
		}
	}

	public SuperFruitControl(Fruit fruit, SuperFruitState state)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		m_prevTime = -2f;
		m_lastCreatedHitControl = null;
		m_fruit = (m_fruitHandle = fruit);
		SuperFruitControls[m_fruit] = this;
		m_prevTime = (m_time = state.time);
		m_sliceTime = state.sliceTime;
		m_hits = state.hits;
		m_rotation = state.rotation;
		SlashEntity.ComboCanceledEvent = (SlashEntity.ComboCanceled)Delegate.Combine(SlashEntity.ComboCanceledEvent, new SlashEntity.ComboCanceled(ComboCancel));
		m_cameraOffset = (m_cameraOffsetPrev = (m_cameraOffsetDest = Vector3.Zero));
		m_cameraOffsetMoveTime = 1f;
		WaveManager.GetInstance().paused = true;
		m_scale = Vector3.One * 0.5f;
		float angle = 0f - m_rotation;
		float num = 0.625f;
		Vector3 val = default(Vector3);
		val = new Vector3(Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(angle)), Mortar.Math.CosIdx(Mortar.Math.DEGREE_TO_IDX(angle)), 0f);
		m_pos = new Vector3(m_fruit.m_pos.X, m_fruit.m_pos.Y, 0f);
		m_pos += val * Game.SCREEN_HEIGHT * 0.25f * num;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
		((ColSphere)fruit.m_col_box).Radius *= 0.775f;
		if (m_time >= EXPLODE_TIME)
		{
			m_fruit.m_cur_scale = Vector3.Zero;
			m_finalAddText = $"+{m_hits}";
			m_splosionCenter = m_fruit.m_pos;
		}
		if (m_time < 0f)
		{
			ChangeText("SLICE!", pop: false);
		}
		else
		{
			string text = string.Format("%i HITS", m_hits);
			ChangeText(text, pop: true);
		}
		m_jumpTime = 1f;
		m_shakeDir = Vector3.Zero;
	}

	public static bool CanSpawnFinalPomegranate()
	{
		if (Fruit.NumberOfPowerupFruits() <= 0)
		{
			return PowerUpManager.GetInstance().GetActiveProgression() >= 2f;
		}
		return false;
	}

	public static bool SpawnFinalPomegranate()
	{
		WaveManager.GetInstance().SpawnFruit(1).Chuck(0.01f);
		WaveManager.GetInstance().SpawnFruit(1).Chuck(0.01f);
		Game.game_work.saveData.AddToTotal("super_pomegranates_spawned", StringFunctions.StringHash("super_pomegranates_spawned"), 1, universal: false, achievementUpload: false);
		WaveManager.GetInstance().SpawnFruit(1, Fruit.FruitType("super_pomegranate")).Chuck(0.1f);
		return true;
	}

	public static int NumPomegranatesSpawnedThisGame()
	{
		if (Game.game_work.saveData == null)
		{
			return 0;
		}
		return Game.game_work.saveData.GetTotal(StringFunctions.StringHash("super_pomegranates_spawned"));
	}

	private void StopAllFruit()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		LinkedListNode<Entity> iterator = null;
		Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator);
		Fruit.ClearUnspawned();
		Bomb.ClearUnspawned();
		while (fruit != null)
		{
			fruit.m_dtMod = 0f;
			fruit.m_gravity = Vector3.Zero;
			fruit.EnableGravity(state: false);
			if (fruit.Sliced())
			{
				Vector3 val = fruit.m_pos - m_splosionCenter;
				val.Normalize();
				val *= 5f;
				fruit.m_vel = (fruit.m_vel + val) / 2f;
				val = fruit.m_pos2 - m_splosionCenter;
				val.Normalize();
				val *= 5f;
				fruit.m_vel2 = (fruit.m_vel2 + val) / 2f;
			}
			fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator);
		}
		for (Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); bomb != null; bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			Vector3 val2 = bomb.m_pos - m_splosionCenter;
			val2.Normalize();
			val2 *= 5f;
			bomb.m_vel = (bomb.m_vel + val2) / 2f;
			bomb.m_gravity = Vector3.Zero;
			bomb.EnableGravity(state: false);
			bomb.m_dtMod = 0f;
		}
	}

	private void PushBombsAway(float dt)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = dt * WaveManager.GetInstance().GetWavedt();
		LinkedListNode<Entity> iterator = null;
		Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator);
		float num2 = 200f;
		float num3 = 100f;
		while (bomb != null)
		{
			Vector3 val = bomb.m_pos - m_fruit.m_pos;
			float num4 = val.Length();
			val.Normalize();
			if (num4 < num2)
			{
				Bomb bomb2 = bomb;
				bomb2.m_vel += val * num * num3;
			}
			bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator);
		}
	}

	private void UpdateExplosion(float dt)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		float num = SPLOSION_RADIUS_FULL * 1.2f;
		m_drawOrder |= HUD.HUD_ORDER.HUD_ORDER_AFTER_BOMB;
		float progressBetween = TransitionFunctions.GetProgressBetween(m_time, EXPLODE_TIME, EXPLODE_FULL, clamp: true);
		m_splosionRadius = progressBetween * num;
		float absoluteDtMod = TransitionFunctions.LerpF(0.1f, 1f, TransitionFunctions.GetProgressBetween(m_time, DT_MOD_RAMP_UP_START, DT_MOD_RAMP_UP_FINISH, clamp: true));
		WaveManager.GetInstance().SetAbsoluteDtMod(absoluteDtMod);
		progressBetween = TransitionFunctions.GetProgressBetween(m_time, SECONDARY_EXPLODE_TIME, SECONDARY_EXPLODE_FULL, clamp: true);
		m_secondarySplosionRadius = progressBetween * num;
		PSPParticleManager.GetInstance().RepulsivePosition = m_splosionCenter;
		PSPParticleManager.GetInstance().RepulsiveRadius = m_splosionRadius * 1.6f;
		PSPParticleManager.GetInstance().RepulsiveStrength = TransitionFunctions.GetProgressBetween(m_time, POPOUT_START_TIME, SECONDARY_EXPLODE_FULL, clamp: true);
		float num2 = dt * WaveManager.GetInstance().GetWavedt();
		if (m_time > EXPLODE_END)
		{
			return;
		}
		LinkedListNode<Entity> iterator = null;
		for (Fruit fruit = (Fruit)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator); fruit != null; fruit = (Fruit)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator))
		{
			if (fruit != m_fruit)
			{
				if (m_splosionRadius > 0f)
				{
					fruit.m_dtMod = 1f;
				}
				Vector3 proj = fruit.m_pos - m_splosionCenter;
				float num3 = proj.Length();
				proj.Normalize();
				if (fruit.Sliced())
				{
					Vector3 val = fruit.m_pos2 - m_splosionCenter;
					float num4 = val.Length();
					val.Normalize();
					if (num4 < m_secondarySplosionRadius && m_time > EXPLODE_FADED)
					{
						Fruit fruit2 = fruit;
						fruit2.m_vel2 += val * Mortar.Math.MAX(0f, m_secondarySplosionRadius - num4) * num2 * 4f;
					}
					if (num3 < m_secondarySplosionRadius)
					{
						if (fruit.m_sliceWait > 0.0001f)
						{
							fruit.m_sliceWait = 0.0001f;
						}
						if (m_time > EXPLODE_FADED)
						{
							Fruit fruit3 = fruit;
							fruit3.m_vel += proj * Mortar.Math.MAX(0f, m_secondarySplosionRadius - num3) * num2 * 4f;
						}
					}
					else if (fruit.m_sliceWait > 0f && m_time < SECONDARY_EXPLODE_FULL)
					{
						fruit.m_sliceWait = 0.5f;
					}
				}
				else if (fruit.IsActive() && num3 < m_splosionRadius && m_splosionRadius < num && m_time < EXPLODE_FADED)
				{
					proj *= 2f;
					fruit.CollisionResponse(null, 0u, 0u, ref proj);
					Game.AddToCurrentScore(1, 0);
					fruit.m_sliceWait = 1E-05f;
				}
			}
			else
			{
				m_fruit.m_sliceWait = 0.5f;
				m_fruit.m_vel = Vector3.Zero;
				m_fruit.m_vel2 = Vector3.Zero;
			}
		}
		for (Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); bomb != null; bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			Vector3 val2 = bomb.m_pos - m_splosionCenter;
			float num5 = val2.Length();
			val2.Normalize();
			if (m_splosionRadius > 0f)
			{
				bomb.m_dtMod = 1f;
			}
			if (num5 < m_secondarySplosionRadius)
			{
				Bomb bomb2 = bomb;
				bomb2.m_vel += val2 * (m_secondarySplosionRadius - num5) * num2 * 5f;
			}
		}
		for (Jiblet jiblet = (Jiblet)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_JIBLET, ref iterator); jiblet != null; jiblet = (Jiblet)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_JIBLET, ref iterator))
		{
			Vector3 val3 = jiblet.m_pos - m_splosionCenter;
			float num6 = val3.Length();
			val3.Normalize();
			if (num6 < m_secondarySplosionRadius)
			{
				Jiblet jiblet2 = jiblet;
				jiblet2.m_vel += val3 * (m_secondarySplosionRadius - num6) * num2 * 5f;
			}
		}
	}

	private void DrawExplosion()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		float progressBetween = TransitionFunctions.GetProgressBetween(m_time, EXPLODE_FADED, EXPLODE_FULL, clamp: true);
		if (progressBetween > 0f && m_splosionRadius > 0f && ShockWaveTexture != null)
		{
			Matrix val = Matrix.CreateScale(Vector3.One * m_splosionRadius * 2f / (27f / 32f));
			val *= Matrix.CreateTranslation(m_splosionCenter);
			MatrixManager.GetInstance().SetMatrix(val);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			ShockWaveTexture.Set();
			Mesh.DrawQuad(new Color(255, 150, 175, Mortar.Math.CLAMP((int)(progressBetween * 128f), 0, 255)));
		}
		progressBetween = TransitionFunctions.GetProgressBetween(m_time, SECONDARY_EXPLODE_FADED, SECONDARY_EXPLODE_FULL, clamp: true);
		if (progressBetween > 0f && m_secondarySplosionRadius > 0f && ShockWaveTexture != null)
		{
			Matrix val2 = Matrix.CreateScale(Vector3.One * m_secondarySplosionRadius * 2f / (27f / 32f));
			val2 *= Matrix.CreateTranslation(m_splosionCenter);
			MatrixManager.GetInstance().SetMatrix(val2);
			MatrixManager.GetInstance().UploadCurrentMatrices();
			ShockWaveTexture.Set();
			Mesh.DrawQuad(new Color(255, 150, 175, Mortar.Math.CLAMP((int)(progressBetween * 128f), 0, 255)));
		}
	}

	public override void Update(float dt)
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		if (m_terminate)
		{
			return;
		}
		if (!Game.game_work.pause)
		{
			m_time += dt;
			if (m_timeSinceHit > 0f)
			{
				m_timeSinceHit -= dt * 17.5f;
				if (m_timeSinceHit <= 0f)
				{
					m_timeSinceHit = 0f;
				}
			}
		}
		if (m_fruit != null && m_fruit.m_sliceWait > 0f)
		{
			m_fruit.m_sliceWait = 1f;
		}
		if (m_prevTime < 0f && m_time >= 0f && m_hits == 1)
		{
			ChangeText("1 HIT", pop: false);
		}
		if (m_time < EXPLODE_TIME)
		{
			WaveManager.GetInstance().SetAbsoluteDtMod(0.1f);
			MissControl.MakeEmAllDissappear();
		}
		if (m_time < POPOUT_START_TIME)
		{
			Bomb.DeactivateAll();
		}
		if (m_time >= ZOOM_OUT_SOUND_TIME && m_prevTime < ZOOM_OUT_SOUND_TIME)
		{
			SoundManager.GetInstance().SFXPlay("pome-rampdown");
		}
		if (m_time >= SLICE_TIME)
		{
			if (m_prevTime < SLICE_TIME)
			{
				m_splosionCenter = m_fruit.m_pos;
				Game.game_work.camera.TransitionOut();
				StopAllFruit();
				GameTask.UnpauseSlices();
				if (m_lastCreatedHitControl != null)
				{
					m_lastCreatedHitControl.m_parent = null;
				}
				m_splosionCenterBounded = new Vector3(Mortar.Math.CLAMP(m_fruit.m_pos.X, (0f - Game.SCREEN_WIDTH) * 0.425f, Game.SCREEN_WIDTH * 0.425f), Mortar.Math.CLAMP(m_fruit.m_pos.Y, (0f - Game.SCREEN_HEIGHT) * 0.4f, Game.SCREEN_HEIGHT * 0.4f), 0f);
			}
			if (m_prevTime < EXPLODE_TIME)
			{
				m_splosionCenter = m_fruit.m_pos;
				if (m_time >= EXPLODE_TIME)
				{
					Game.game_work.camera.CreateCameraShake(m_pos, 1f, 2f);
					ExplodeSuperFruit();
					SpawnJibs();
					StopRays();
					m_finalAddText = $"+{m_hits}";
				}
			}
			if (m_time >= EXPLODE_TIME)
			{
				m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
				UpdateExplosion(dt);
				if (m_prevTime < DT_MOD_RAMP_UP_START && m_time >= DT_MOD_RAMP_UP_START)
				{
					Game.game_work.camera.CreateCameraShake(m_pos, 1.6f, 2f);
				}
				Game.game_work.hud.m_hudAmount = Game.TAILY_TRANSITION_TO(Game.game_work.hud.m_hudAmount, 1f, dt, 0.75f);
			}
			if (m_prevTime < FINAL_SCORE_IN_TIME && m_time >= FINAL_SCORE_IN_TIME)
			{
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_CLASSIC)
				{
					Game.game_work.saveData.AddToTotal("super_fruit_gp_classic", StringFunctions.StringHash("super_fruit_gp_classic"), m_hits, universal: false, achievementUpload: false);
				}
				Game.AddToCurrentScore(m_hits, 0);
			}
			if (m_time > KILL_FRUIT_TIME)
			{
				m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
				if (m_fruit != null && m_prevTime <= KILL_FRUIT_TIME)
				{
					m_fruit.KillFruit();
					m_fruit = null;
				}
			}
			if (m_time > POPOUT_END_TIME)
			{
				WaveManager.GetInstance().SetAbsoluteDtMod();
				WaveManager.GetInstance().paused = false;
				WaveManager.GetInstance().GetNextWave();
				PSPParticleManager.GetInstance().RepulsiveRadius = 0f;
				PSPParticleManager.GetInstance().RepulsiveStrength = 1f;
				Game.game_work.hud.m_hudAmount = 1f;
				m_terminate = true;
				Release();
			}
		}
		else
		{
			if (m_fruit.m_gravity.X != 0f)
			{
				float num = Game.SCREEN_WIDTH * 0.3f;
				float num2 = Game.SCREEN_WIDTH * 0.45f;
				if (m_fruit.m_vel.X < 0f)
				{
					num = 0f - num;
					num2 = 0f - num2;
				}
				m_fruit.m_dtMod = TransitionFunctions.GetProgressBetween(m_fruit.m_pos.X, num2, num, clamp: true);
			}
			else if (m_fruit.m_vel.Y < 0f)
			{
				float end = (0f - Game.SCREEN_HEIGHT) * 0.3f;
				float start = (0f - Game.SCREEN_HEIGHT) * 0.4f;
				m_fruit.m_dtMod = TransitionFunctions.GetProgressBetween(m_fruit.m_pos.Y, start, end, clamp: true);
			}
			PushBombsAway(dt);
			Game.game_work.hud.m_hudAmount = Game.TAILY_TRANSITION_TO(Game.game_work.hud.m_hudAmount, -1f, dt, 0.75f);
			float num3 = 0f - m_rotation;
			float num4 = 0.625f;
			Vector3 val = default(Vector3);
			val = new Vector3(Mortar.Math.SinIdx(Mortar.Math.DEGREE_TO_IDX(num3)), Mortar.Math.CosIdx(Mortar.Math.DEGREE_TO_IDX(num3)), 0f);
			Vector3 val2 = m_fruit.m_pos;
			if (m_jumpTime < 1f)
			{
				val2 += m_shakeDir * TransitionFunctions.JumpySinPulse(Mortar.Math.CLAMP(m_jumpTime * 2f, 0f, 1f), 2f);
			}
			m_cameraOffset = TransitionFunctions.LerpF(m_cameraOffsetPrev, m_cameraOffsetDest, TransitionFunctions.SinTransition(Mortar.Math.MIN(m_cameraOffsetMoveTime, 1f), 105f));
			val2 += m_cameraOffset;
			Game.game_work.camera.Transition(val2, num4, num3, TransitionFin);
			m_pos = new Vector3(m_fruit.m_pos.X, m_fruit.m_pos.Y, 0f);
			m_pos += val * Game.SCREEN_HEIGHT * 0.25f * num4;
		}
		if (m_jumpTime < 1f)
		{
			m_jumpTime += 3f * dt;
			if (m_jumpTime >= 1f)
			{
				m_jumpTime = 1f;
			}
		}
		if (m_cameraOffsetMoveTime < 1f)
		{
			m_cameraOffsetMoveTime += dt / 0.175f;
			if (m_cameraOffsetMoveTime > 1f)
			{
				m_cameraOffsetMoveTime = 1f;
			}
		}
		m_prevTime = m_time;
	}

	public override void Draw(float[] tintChannels)
	{
		DrawOrder(tintChannels, (int)HUD.ORDER);
	}

	public override void DrawOrder(float[] tintChannels, int order)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		float num = TransitionFunctions.SinTransition(TransitionFunctions.GetProgressBetween(m_time, POPOUT_END_TIME, POPOUT_START_TIME, clamp: true), 115f) * TransitionFunctions.SinTransition(TransitionFunctions.GetProgressBetween(m_time, -2f, -1.85f, clamp: true), 115f);
		Vector3 val = Initialise.ScreenPos(m_pos, m_posAnchor);
		float num2 = m_scale.X * (1f + 0.25f * TransitionFunctions.JumpySinPulse(m_jumpTime, 3f)) * num;
		if (m_time < 0f)
		{
			num2 *= 1f + Mortar.Math.MAX(0f, TransitionFunctions.SinPulse(m_time, 8f) * 0.125f);
		}
		if (m_time > SLICE_TIME)
		{
			val += (m_splosionCenterBounded - m_splosionCenter) * (1f - Game.game_work.camera.m_transitionAmount);
		}
		val.Z = 1f;
		Vector3 val2 = Game.game_work.camera.TranslatePos(val, screenToWorld: false, includeShake: true);
		Color black = Color.Black;
		black.A = m_colour.A;
		Game.game_work.pGameFont.DrawString(m_bakedString, val2 + new Vector3(-1f, -1f, 0f), black, num2 * 100f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
		Game.game_work.pGameFont.DrawString(m_bakedString, val2 + new Vector3(1f, 1f, 0f), black, num2 * 100f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
		Game.game_work.pGameFont.DrawString(m_bakedString, val2, m_colour, num2 * 100f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
		if (m_finalAddText.Length > 0)
		{
			TransitionFunctions.SinTransition(TransitionFunctions.GetProgressBetween(m_time, FINAL_SCORE_IN_TIME, FINAL_SCORE_FULL_TIME, clamp: true), 115f);
			Vector3 val3 = TransitionFunctions.LerpF(m_splosionCenterBounded, m_splosionCenter, Game.game_work.camera.m_transitionAmount);
			Game.game_work.pGameFont.DrawString(m_finalAddText, val3 + new Vector3(-1f, -1f, 0f), black, num2 * 100f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
			Game.game_work.pGameFont.DrawString(m_finalAddText, val3 + new Vector3(1f, 1f, 0f), black, num2 * 100f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
			Game.game_work.pGameFont.DrawString(m_finalAddText, val3, m_colour, num2 * 100f, Vector2.Zero, ALIGNMENT_TYPE.ALIGN_CENTER);
		}
		if (order == 5)
		{
			DrawExplosion();
		}
	}
}
