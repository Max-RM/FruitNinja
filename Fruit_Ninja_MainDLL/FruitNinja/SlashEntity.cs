using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mortar;

namespace FruitNinja;

public class SlashEntity : Entity
{
	public delegate void ComboCanceled(SlashEntity e);

	public enum SLASH_POWER
	{
		SLASH_POWER_PUSH_FRUIT = 1,
		SLASH_POWER_PULL_FRUIT = 2,
		SLASH_POWER_PUSH_BOMB = 4,
		SLASH_POWER_PULL_BOMB = 8,
		SLASH_POWER_BOMB_HIT = 16,
		SLASH_POWER_FRUIT_BOUNCE = 32,
		SLASH_POWER_NO_COLLISION = 64,
		SLASH_NUM_POWERS = 7
	}

	public enum SlashColorChangeType
	{
		SCCT_NONE,
		SCCT_LERP,
		SCCT_PER_SLASH,
		SCCT_CONTINUOUS,
		SCCT_MAX
	}

	private const int MAX_POINTS_REMEMBERED = 6;

	private const int MAX_MOD_COLOURS = 16;

	public static float START_THICKNESS = 9f;

	public static float MAX_POINTS = 110f;

	public static float ALPHA_OUT_PERCENT = 0.99f;

	public static float SWIPE_SFX_WAIT = 0.5f;

	public static float COMBO_TIME = 0.1f;

	public static float NEW_WHOA_TIME = 6f;

	public static float FRUIT_HIT_SHOUT_COUNT = 1f;

	public static float CRIT_SHOUT_COUNT = 3f;

	public static float SWIPE_SHOUT_COUNT = 0.5f;

	public static float SHOUT_VARIANTS = 15f;

	protected PSPParticleEmitter m_emitter;

	protected float m_criticalTime;

	protected Color m_slashColor;

	protected Color m_slashModColor;

	protected bool m_hitBomb;

	protected byte PAD00;

	protected short PAD01;

	protected int m_pointsCount;

	protected int m_currentIndex;

	protected int m_drawCount;

	protected VertexPositionColorTexture[] m_points;

	protected VertexPositionColorTexture[] m_pointPoints = ArrayInit.CreateFilledArray<VertexPositionColorTexture>(4);

	protected Vector3 m_lastDir;

	protected Vector3 m_lastPos;

	protected float m_col_length;

	protected float m_slashLength;

	public int m_splatsToMake;

	protected float m_timeTillNextSplat;

	protected float m_splatInterval;

	protected Vector3 m_lastSlashDir;

	protected Vector3 m_lastSlashPos;

	protected int m_lastSlashFruitType;

	protected float m_swipeWait;

	protected Vector3[] m_pointTrail = ArrayInit.CreateFilledArray<Vector3>(6);

	protected int m_currentPoint;

	protected int m_pointsRemembered;

	protected Vector3 m_averageDir;

	protected float m_timeSinceLastHit;

	protected int m_comboLength;

	protected int m_comboPlayer;

	protected MissControl missControl;

	public byte m_touchDown;

	public float m_shoutTime;

	public int[] m_lastShouts = new int[2];

	public int[] m_comboFruitTypes = new int[11];

	public static float ModColorTime = 0f;

	public static float ModColorChangeSpeed = 1f;

	public static Color[] ModColors = ArrayInit.CreateFilledArray<Color>(16);

	public static int NumModColors = 1;

	public static int ModColorType = 0;

	public static bool ModHasPartilces = false;

	public static uint ModPartilcesHash = 0u;

	private static Mortar.Texture s_slashTexture = null;

	private static Mortar.Texture s_slashAltTexture = null;

	private static bool loaded = false;

	private static bool updated = false;

	private static bool updated_last_frame = false;

	private static int slashes = 0;

	private static Color colourOut = Color.White;

	public static ComboCanceled ComboCanceledEvent;

	private Vector3[] m_oldPositions = new Vector3[3];

	public static uint ModPowerMask;

	private static float[] pointTotals = new float[100];

	private static bool STOP = false;

	private static int STOP_COUNTER = 0;

	private static COMBO_TYPE s_combo = COMBO_TYPE.CT_NONE;

	private static uint[] hashes = new uint[6]
	{
		StringFunctions.StringHash("PUSH_FRUIT"),
		StringFunctions.StringHash("PULL_FRUIT"),
		StringFunctions.StringHash("PUSH_BOMB"),
		StringFunctions.StringHash("PULL_BOMB"),
		StringFunctions.StringHash("BOMB_HIT"),
		StringFunctions.StringHash("FRUIT_BOUNCE")
	};

	private static uint[] ParseSlashModColorTypehashes = new uint[4]
	{
		StringFunctions.StringHash("NONE"),
		StringFunctions.StringHash("LERP"),
		StringFunctions.StringHash("PER_SLASH"),
		StringFunctions.StringHash("CONTINUOUS")
	};

	public static float START_MP_THICKNESS => START_THICKNESS * Game.SPLIT_SCREEN_SCALE;

	public static float FADE_OUT_TIME => 0.2f;

	public static float FADE_FRAME_AMOUNT => START_THICKNESS / FADE_OUT_TIME;

	public static float FADE_MP_FRAME_AMOUNT => START_MP_THICKNESS / FADE_OUT_TIME;

	public static float START_CHANGED_DIST => 50f * Game.GAME_MODE_SCALE_FIX;

	public static float CHANGED_DIST => 5f * Game.GAME_MODE_SCALE_FIX;

	protected void InitPoints(int numPoints)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		Color color = white;
		m_lastDir = Vector3.Zero;
		m_drawCount = 0;
		m_pointsCount = numPoints;
		m_points = new VertexPositionColorTexture[numPoints];
		m_lastPos = new Vector3(-65535f, -65535f, -65535f);
		for (int i = 0; i < numPoints; i++)
		{
			m_points[i].Position.X = 0f;
			m_points[i].Position.Y = 0f;
			m_points[i].Position.Z = 0f;
			m_points[i].Color = color;
			m_points[i].TextureCoordinate.X = 0f;
			m_points[i].TextureCoordinate.Y = 0f;
		}
	}

	protected void AddPoint(Vector3 pos, Vector3 dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		if (!(dir != Vector3.Zero) && !(m_lastDir != Vector3.Zero))
		{
			return;
		}
		if (dir == Vector3.Zero)
		{
			dir = m_lastDir;
			ref Vector3 reference = ref m_pointTrail[m_currentPoint];
			reference = Vector3.Zero;
		}
		else if (m_pointsRemembered == 0)
		{
			ref Vector3 reference2 = ref m_pointTrail[m_currentPoint];
			reference2 = Vector3.Zero;
		}
		else
		{
			m_pointTrail[m_currentPoint] = dir;
			m_pointTrail[m_currentPoint].Normalize();
		}
		m_lastDir = dir;
		m_averageDir = Vector3.Zero;
		if (m_pointsRemembered >= 2)
		{
			for (int i = 1; i < m_pointsRemembered; i++)
			{
				m_averageDir += m_pointTrail[(m_currentPoint - i + 18) % 6];
			}
			m_averageDir /= (float)(m_pointsRemembered - 1);
			Vector3 val = m_averageDir - m_pointTrail[m_currentPoint];
			if (val.LengthSquared() > 1.6899998f)
			{
				m_timeSinceLastHit = COMBO_TIME - 0.005f;
			}
		}
		m_pointsRemembered++;
		m_pointsRemembered = Mortar.Math.MIN(m_pointsRemembered, 6);
		m_currentPoint = (m_currentPoint + 1) % 6;
		Vector3 val2 = Vector3.Cross(dir, new Vector3(0f, 0f, 1f));
		val2.Normalize();
		if (m_drawCount >= m_pointsCount)
		{
			for (int j = 2; j < m_pointsCount; j += 2)
			{
				m_points[j - 2].Position = m_points[j].Position;
				m_points[j - 1].Position = m_points[j + 1].Position;
			}
			m_drawCount = m_pointsCount - 2;
		}
		if (m_slashLength < 1f)
		{
			m_slashLength += 45f * Game.game_work.dt;
			if (m_slashLength > 1f)
			{
				m_slashLength = 1f;
			}
		}
		val2 = ((!Game.IsMultiplayer()) ? (val2 * (START_THICKNESS * m_slashLength)) : (val2 * (START_MP_THICKNESS * m_slashLength)));
		Vector3 position = pos - val2;
		ValFloat(position.X);
		ValFloat(position.Y);
		m_points[m_drawCount].Position = position;
		m_points[m_drawCount].TextureCoordinate = new Vector2(0.5f, 0f);
		m_points[m_drawCount++].Color = m_slashColor;
		position = pos + val2;
		ValFloat(position.X);
		ValFloat(position.Y);
		m_points[m_drawCount].Position = position;
		m_points[m_drawCount].TextureCoordinate = new Vector2(0.5f, 1f);
		m_points[m_drawCount++].Color = m_slashColor;
	}

	private void ValFloat(float f)
	{
		if (float.IsNaN(f))
		{
			throw new Exception();
		}
	}

	private Vector3 GetV3(ref GameVertex vertex)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(vertex.X, vertex.Y, vertex.Z);
	}

	protected void UpdatePoints(float dt)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		if (dt == 0f && Game.game_work.hitBombTime > 0f)
		{
			if (m_hitBomb)
			{
				Color red = Color.Red;
				for (int i = 0; i < m_drawCount; i++)
				{
					m_points[i].Color = red;
				}
				m_pointPoints[0].Color = red;
				m_pointPoints[1].Color = red;
				m_pointPoints[2].Color = red;
			}
			m_col_length = -1f;
			return;
		}
		if (m_drawCount >= 4 && m_touchDown != 0)
		{
			Vector3 val = Game.game_work.camera.TranslatePos(m_oldPositions[1], screenToWorld: true, includeShake: false);
			Vector3 val2 = Game.game_work.camera.TranslatePos(m_oldPositions[0], screenToWorld: true, includeShake: false);
			val = (val + val2) * 0.5f;
			((ColLine)m_col_box).centre = val;
			((ColLine)m_col_box).Direction = val2;
			m_col_length = (val.X - val2.X) * (val.X - val2.X) + (val.Y - val2.Y) * (val.Y - val2.Y);
		}
		else
		{
			m_col_length = -1f;
		}
		if (m_drawCount < 4)
		{
			m_lastDir = Vector3.Zero;
		}
		if (ModColorType == 0)
		{
			ModColorTime = 0f;
		}
		pointTotals[0] = 0f;
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < m_drawCount; j += 2)
		{
			Vector3 val3 = m_points[j].Position + m_points[j + 1].Position;
			Vector3 val4 = val3 / 2f;
			val3 = val4 - m_points[j].Position;
			float num3 = val3.Length();
			num3 = ((!Game.IsMultiplayer()) ? (num3 - FADE_FRAME_AMOUNT * dt) : (num3 - FADE_MP_FRAME_AMOUNT * dt));
			if (num3 <= 0f)
			{
				num2 += 2;
			}
			else if (m_drawCount > 2 && j + 3 < m_drawCount)
			{
				num++;
				Vector3 val5 = m_points[j + 2].Position + m_points[j + 3].Position;
				Vector3 val6 = val5 / 2f;
				Vector3 val7 = val6 - val4;
				float num4 = val7.Length();
				val7.Normalize();
				Vector3 val8 = Vector3.Cross(val7, new Vector3(0f, 0f, 1f));
				val8 *= num3;
				if (num > 0)
				{
					pointTotals[num] = pointTotals[num - 1] + num4;
				}
				Vector3 position = val4 - val8;
				ValFloat(position.X);
				ValFloat(position.Y);
				m_points[j - num2].Position = position;
				position = val4 + val8;
				ValFloat(position.X);
				ValFloat(position.Y);
				m_points[j - num2 + 1].Position = position;
				float x = (float)j / (float)m_drawCount * 0.98f;
				m_points[j - num2].TextureCoordinate.X = x;
				m_points[j - num2 + 1].TextureCoordinate.X = x;
				if (ModColorType == 0)
				{
					UpdateModColor(ref m_slashModColor, -2f / (float)m_drawCount);
					if (m_criticalTime > 0f)
					{
						float num5 = 1f - m_criticalTime;
						m_slashColor.R = (byte)((float)(int)Fruit.CRITICAL_COLOUR.R + ((float)(int)m_slashModColor.R - (float)(int)Fruit.CRITICAL_COLOUR.R) * num5);
						m_slashColor.G = (byte)((float)(int)Fruit.CRITICAL_COLOUR.G + ((float)(int)m_slashModColor.G - (float)(int)Fruit.CRITICAL_COLOUR.G) * num5);
						m_slashColor.B = (byte)((float)(int)Fruit.CRITICAL_COLOUR.B + ((float)(int)m_slashModColor.B - (float)(int)Fruit.CRITICAL_COLOUR.B) * num5);
						m_slashColor.A = byte.MaxValue;
					}
					else
					{
						m_slashColor = m_slashModColor;
					}
				}
				m_points[j - num2].Color = m_slashColor;
				m_points[j - num2 + 1].Color = m_slashColor;
			}
			else
			{
				m_points[j - num2].TextureCoordinate.X = (m_points[j - num2 + 1].TextureCoordinate.X = 0.98f);
				Vector3 val9 = Vector3.Cross(m_lastDir, new Vector3(0f, 0f, 1f));
				if (val9.X != 0f || val9.Y != 0f || val9.Z != 0f)
				{
					val9.Normalize();
				}
				val9 *= num3;
				Vector3 position2 = val4 - val9;
				ValFloat(position2.X);
				ValFloat(position2.Y);
				m_points[j - num2].Position = position2;
				position2 = val4 + val9;
				ValFloat(position2.X);
				ValFloat(position2.Y);
				m_points[j - num2 + 1].Position = position2;
			}
		}
		float num6 = pointTotals[num];
		for (int k = 0; k < m_drawCount; k += 2)
		{
			float x2 = pointTotals[Mortar.Math.MIN(k / 2, num)] / num6;
			m_points[k].TextureCoordinate.X = x2;
			m_points[k + 1].TextureCoordinate.X = x2;
		}
		if (m_drawCount > 2)
		{
			slashes++;
			Vector3 val10 = m_points[m_drawCount - 2].Position + m_points[m_drawCount - 1].Position;
			Vector3 val11 = val10 / 2f;
			val10 = val11 - m_points[m_drawCount - 2].Position;
			ref VertexPositionColorTexture reference = ref m_pointPoints[0];
			reference = m_points[m_drawCount - 2];
			Vector3 val12 = Vector3.Cross(val10, new Vector3(0f, 0f, 1f)) * 2.5f;
			m_pointPoints[0].Position.X = val11.X - val12.X;
			m_pointPoints[0].Position.Y = val11.Y - val12.Y;
			ValFloat(m_pointPoints[0].Position.X);
			ValFloat(m_pointPoints[0].Position.Y);
			ref VertexPositionColorTexture reference2 = ref m_pointPoints[1];
			reference2 = m_points[m_drawCount - 2];
			ref VertexPositionColorTexture reference3 = ref m_pointPoints[2];
			reference3 = m_points[m_drawCount - 1];
			m_pointPoints[0].Color = m_points[m_drawCount - 1].Color;
			m_pointPoints[1].Color = m_points[m_drawCount - 1].Color;
			m_pointPoints[2].Color = m_points[m_drawCount - 1].Color;
			m_pointPoints[0].TextureCoordinate = new Vector2(0.98f, 0.5f);
			m_pointPoints[1].TextureCoordinate = new Vector2(0.98f, 0f);
			m_pointPoints[2].TextureCoordinate = new Vector2(0.98f, 1f);
		}
		m_drawCount -= num2;
	}

	protected void PlaySwipe()
	{
		string filename = $"Sword-swipe-{1 + Mortar.Math.g_random.Rand32(6)}";
		SoundManager.GetInstance().SFXPlay(filename);
		m_shoutTime -= (Mortar.Math.g_random.RandF(0.5f) + 0.75f) * SWIPE_SHOUT_COUNT * (1f + (float)(ActorManager.GetInstance().GetNumEntities(0) + ActorManager.GetInstance().GetNumEntities(1)) * 0.4f);
		m_shoutTime = NEW_WHOA_TIME;
	}

	public void Reset()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		Color color = white;
		m_lastDir = Vector3.Zero;
		m_drawCount = 0;
		m_hitBomb = false;
		m_lastPos = new Vector3(-65535f, -65535f, -65535f);
		m_currentPoint = 0;
		m_pointsRemembered = 0;
		for (int i = 0; i < 6; i++)
		{
			ref Vector3 reference = ref m_pointTrail[i];
			reference = Vector3.Zero;
		}
		if (m_points != null)
		{
			for (int j = 0; j < m_pointsCount; j++)
			{
				m_points[j].Position.X = 0f;
				m_points[j].Position.Y = 0f;
				m_points[j].Position.Z = 0f;
				m_points[j].Color = color;
				m_points[j].TextureCoordinate.X = 0f;
				m_points[j].TextureCoordinate.Y = 0f;
			}
		}
		for (int k = 0; k < 11; k++)
		{
			m_comboFruitTypes[k] = -1;
		}
		for (int l = 0; l < m_oldPositions.Length; l++)
		{
			ref Vector3 reference2 = ref m_oldPositions[l];
			reference2 = new Vector3(-65535f, -65535f, -65535f);
		}
	}

	public override void Init(byte[] tpl_data, int tpl_size, Vector3? size)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		m_col_box = new ColLine();
		base.m_updateAlways = true;
		m_col_length = -1f;
		InitPoints(80);
		m_splatsToMake = -1;
		m_slashLength = 0f;
		m_emitter = null;
		m_criticalTime = 0f;
		m_slashModColor = Color.White;
		m_slashColor = Color.White;
		m_comboLength = 0;
		m_comboPlayer = 0;
		m_timeSinceLastHit = COMBO_TIME;
		m_averageDir = Vector3.Zero;
		m_hitBomb = false;
		m_pointsRemembered = 0;
		m_currentPoint = 0;
		for (int i = 0; i < 6; i++)
		{
			ref Vector3 reference = ref m_pointTrail[i];
			reference = Vector3.Zero;
		}
		for (int j = 0; j < 11; j++)
		{
			m_comboFruitTypes[j] = -1;
		}
		m_shoutTime = NEW_WHOA_TIME;
		m_lastShouts[0] = -1;
		m_lastShouts[1] = -1;
		m_swipeWait = 0f;
		m_comboLength = 0;
		m_comboPlayer = 0;
		for (int k = 0; k < m_oldPositions.Length; k++)
		{
			ref Vector3 reference2 = ref m_oldPositions[k];
			reference2 = new Vector3(-65535f, -65535f, -65535f);
		}
	}

	public static void LoadContent()
	{
		if (!loaded)
		{
			loaded = true;
			s_slashTexture = TextureManager.GetInstance().Load("textureswp7/blade.tex");
		}
	}

	public override void Release()
	{
		m_points = null;
		if (m_emitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_emitter);
			m_emitter = null;
		}
		base.Release();
	}

	public static void PreUpdate(float dt)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (STOP_COUNTER < 5)
		{
			STOP_COUNTER++;
		}
		else
		{
			STOP = false;
		}
		if (ModColorType == 1)
		{
			Color colourIn = Color.Black;
			UpdateModColor(ref colourIn, dt);
		}
	}

	private bool CombosEnabled()
	{
		return Game.game_work.gameMode != Game.GAME_MODE.GM_CASINO;
	}

	public override void Update(float dt)
	{
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (dt > 0f)
		{
			updated = true;
			dt = Game.game_work.dt;
			num = Game.game_work.dt;
			if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
			{
				num *= 0.666f;
				if (PowerUpManager.GetInstance().GetDtMod() < 0.9f)
				{
					num *= PowerUpManager.GetInstance().GetDtMod();
				}
			}
		}
		UpdatePoints(dt);
		if (m_touchDown != 0 && (ModPowerMask & 0x40) == 0)
		{
			if (Game.game_work.hitBombTime <= 0f)
			{
				LinkedListNode<Entity> iterator = null;
				Entity entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BEGIN, ref iterator);
				while (entity != null && !STOP)
				{
					Fruit fruit = (Fruit)entity;
					if (!fruit.Sliced() && fruit.IsActive())
					{
						if (CollideWithEntity(entity))
						{
							Vector3 proj = m_lastDir;
							m_splatsToMake++;
							m_splatInterval = 0f;
							m_timeTillNextSplat = 0f;
							m_lastSlashDir = proj;
							m_lastSlashFruitType = fruit.GetFruitType();
							m_lastSlashPos = entity.m_pos;
							if (!fruit.m_isMenuItem && (m_comboLength == 0 || (m_comboLength > 0 && m_comboPlayer == fruit.ForPlayer())))
							{
								m_timeSinceLastHit = 0f;
								m_comboFruitTypes[m_comboLength] = m_lastSlashFruitType;
								m_comboPlayer = fruit.ForPlayer();
								m_comboLength++;
								if (m_comboLength >= 10)
								{
									m_timeSinceLastHit = COMBO_TIME - 0.005f;
									STOP = true;
								}
								m_shoutTime -= (float)m_comboLength * (Mortar.Math.g_random.RandF(0.5f) + 0.75f) * FRUIT_HIT_SHOUT_COUNT;
								if (m_comboLength > 2 && CombosEnabled())
								{
									if (missControl == null)
									{
										missControl = MissControl.GetFree();
										missControl.MakeCombo(m_lastSlashPos, m_comboLength, m_comboPlayer);
										missControl.m_deleteCall = MissControlDeleted;
									}
									else
									{
										missControl.MakeCombo(missControl.m_pos, m_comboLength, m_comboPlayer);
									}
								}
							}
							else if (m_timeSinceLastHit < COMBO_TIME)
							{
								m_timeSinceLastHit = COMBO_TIME - 0.005f;
							}
							entity.CollisionResponse(this, 0u, 0u, ref proj);
							if (fruit.m_isCritical)
							{
								m_lastSlashFruitType += Fruit.MAX_FRUIT_TYPES;
								m_shoutTime -= (Mortar.Math.g_random.RandF(0.5f) + 0.75f) * CRIT_SHOUT_COUNT;
							}
							if (fruit.m_isMenuItem)
							{
								STOP_COUNTER = 0;
								STOP = true;
							}
						}
						else if ((ModPowerMask & 2) != 0)
						{
							Vector3 val = entity.m_pos - m_pos;
							float num2 = val.Length();
							val.Normalize();
							Entity entity2 = entity;
							entity2.m_vel -= val * Mortar.Math.MIN(num2 / 2f, 50f) * dt;
							float v = entity.m_vel.Length();
							entity.m_vel.Normalize();
							Entity entity3 = entity;
							entity3.m_vel *= Mortar.Math.MIN(v, 8f);
						}
						else if ((ModPowerMask & 1) == 1)
						{
							Vector3 val2 = entity.m_pos - m_pos;
							float num3 = val2.Length();
							val2.Normalize();
							Entity entity4 = entity;
							entity4.m_vel += val2 * Mortar.Math.MIN(num3 / 2f, 50f) * dt;
							float v2 = entity.m_vel.Length();
							entity.m_vel.Normalize();
							Entity entity5 = entity;
							entity5.m_vel *= Mortar.Math.MIN(v2, 8f);
						}
					}
					entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BEGIN, ref iterator);
				}
				entity = ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator);
				while (entity != null && !STOP)
				{
					Vector3 lastDir = m_lastDir;
					if (((Bomb)entity).IsActive())
					{
						if (CollideWithEntity(entity))
						{
							if ((ModPowerMask & 0x10) == 16)
							{
								Entity entity6 = entity;
								entity6.m_vel += m_lastDir * dt * 10f;
							}
							else
							{
								lastDir = m_lastDir;
								entity.CollisionResponse(this, 0u, 0u, ref lastDir);
								STOP_COUNTER = 0;
								STOP = true;
								if (((Bomb)entity).m_hit && !((Bomb)entity).m_isMenuItem)
								{
									m_hitBomb = true;
								}
							}
						}
						else if ((ModPowerMask & 4) == 4)
						{
							Vector3 val3 = entity.m_pos - m_pos;
							float num4 = val3.Length();
							val3.Normalize();
							Entity entity7 = entity;
							entity7.m_vel += val3 * Mortar.Math.MIN(num4 / 2f, 50f) * dt;
							float v3 = entity.m_vel.Length();
							entity.m_vel.Normalize();
							Entity entity8 = entity;
							entity8.m_vel *= Mortar.Math.MIN(v3, 8f);
						}
						else if ((ModPowerMask & 8) == 8)
						{
							Vector3 val4 = entity.m_pos - m_pos;
							float num5 = val4.Length();
							val4.Normalize();
							Entity entity9 = entity;
							entity9.m_vel -= val4 * Mortar.Math.MIN(num5 / 2f, 50f) * dt;
							float v4 = entity.m_vel.Length();
							entity.m_vel.Normalize();
							Entity entity10 = entity;
							entity10.m_vel *= Mortar.Math.MIN(v4, 8f);
						}
					}
					entity = ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator);
				}
			}
		}
		else
		{
			if (m_touchDown == 0 && m_emitter != null)
			{
				PSPParticleManager.GetInstance().ClearEmitter(m_emitter);
				m_emitter = null;
			}
			m_slashLength = 0f;
		}
		if (m_emitter != null && WaveManager.GetInstance().CriticalMode())
		{
			m_criticalTime = Mortar.Math.MIN(1f, m_criticalTime + dt * 2f);
		}
		else
		{
			m_criticalTime = Mortar.Math.MAX(0f, m_criticalTime - dt * 2f);
		}
		if (ModColorType < 2)
		{
			UpdateModColor(ref m_slashModColor, dt);
		}
		if (m_criticalTime > 0f)
		{
			float num6 = 1f - m_criticalTime;
			m_slashColor.R = (byte)((float)(int)Fruit.CRITICAL_COLOUR.R + (float)(m_slashModColor.R - Fruit.CRITICAL_COLOUR.R) * num6);
			m_slashColor.G = (byte)((float)(int)Fruit.CRITICAL_COLOUR.G + (float)(m_slashModColor.G - Fruit.CRITICAL_COLOUR.G) * num6);
			m_slashColor.B = (byte)((float)(int)Fruit.CRITICAL_COLOUR.B + (float)(m_slashModColor.B - Fruit.CRITICAL_COLOUR.B) * num6);
			m_slashColor.A = byte.MaxValue;
		}
		else
		{
			m_slashColor = m_slashModColor;
		}
		if (m_timeSinceLastHit < COMBO_TIME)
		{
			m_timeSinceLastHit += num;
			if (m_timeSinceLastHit >= COMBO_TIME)
			{
				if (ComboCanceledEvent != null)
				{
					ComboCanceledEvent(this);
				}
				if (m_comboLength > 1 && m_comboFruitTypes[0] >= 0)
				{
					Game.game_work.criticalChance = Mortar.Math.MAX(2, Game.game_work.criticalChance - m_comboLength);
					if (m_comboLength > 2 && m_comboFruitTypes[1] >= 0)
					{
						if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
						{
							WaveManager.GetInstance().AddSpeed((float)m_comboLength / 3f);
							Game.AddToCurrentScore(m_comboLength, m_comboPlayer);
							BonusManager.GetInstance().AddCombo(m_comboLength);
						}
						else
						{
							Game.AddToCurrentScore(m_comboLength, m_comboPlayer);
						}
						int numberOfCoins = 0;
						for (int i = 0; i < m_comboLength; i++)
						{
							FRUIT_INFO fRUIT_INFO = Fruit.FruitInfo(m_comboFruitTypes[i]);
							int coinsMax = fRUIT_INFO.coinsMax;
							if (coinsMax > 0)
							{
								numberOfCoins = m_comboLength;
								break;
							}
						}
						Vector3 pos = m_lastSlashPos;
						if (missControl != null)
						{
							pos = missControl.m_pos;
						}
						Coin.MakeCoins(numberOfCoins, 1, pos, 0, Mortar.Math.DEGREE_TO_IDX(359f));
					}
					AchievementManager.GetInstance().UnlockComboAchievement(m_comboLength, m_comboFruitTypes);
					if (m_comboLength >= 3)
					{
						int num7 = Fruit.FruitType("strawberry");
						for (int j = 0; j < m_comboLength; j++)
						{
							if (m_comboFruitTypes[j] == num7)
							{
								string text = "strawberry_combo_total";
								uint hash = StringFunctions.StringHash(text);
								Game.game_work.saveData.AddToTotal(text, hash, 1, universal: true, achievementUpload: false);
								break;
							}
						}
					}
					if (m_comboLength > Game.game_work.saveData.numFruitTypesInSliceCombo)
					{
						for (int k = 0; k < 11; k++)
						{
							Game.game_work.saveData.sliceComboFruitTypes[k] = m_comboFruitTypes[k];
						}
						Game.game_work.saveData.numFruitTypesInSliceCombo = m_comboLength;
						s_combo = ComboChecker.CheckCombo(m_comboFruitTypes, m_comboLength);
					}
					else if (m_comboLength == Game.game_work.saveData.numFruitTypesInSliceCombo)
					{
						if (s_combo == COMBO_TYPE.CT_NONE)
						{
							s_combo = ComboChecker.CheckCombo(Game.game_work.saveData.sliceComboFruitTypes, Game.game_work.saveData.numFruitTypesInSliceCombo);
						}
						COMBO_TYPE cOMBO_TYPE = ComboChecker.CheckCombo(m_comboFruitTypes, m_comboLength);
						if (cOMBO_TYPE > s_combo)
						{
							for (int l = 0; l < 11; l++)
							{
								Game.game_work.saveData.sliceComboFruitTypes[l] = m_comboFruitTypes[l];
							}
							Game.game_work.saveData.numFruitTypesInSliceCombo = m_comboLength;
						}
					}
				}
				m_comboLength = 0;
				m_comboPlayer = 0;
				missControl = null;
				for (int m = 0; m < 11; m++)
				{
					m_comboFruitTypes[m] = -1;
				}
			}
		}
		else
		{
			m_comboLength = 0;
			m_comboPlayer = 0;
			for (int n = 0; n < 11; n++)
			{
				m_comboFruitTypes[n] = -1;
			}
			missControl = null;
		}
		bool flag = m_lastDir.Length() > 35f * Game.GAME_MODE_SCALE_FIX;
		if ((m_swipeWait > 0f && m_swipeWait < SWIPE_SFX_WAIT) || m_lastDir.Length() < 20f * Game.GAME_MODE_SCALE_FIX)
		{
			m_swipeWait -= Game.game_work.dt;
		}
		else if (m_swipeWait <= 0f && flag)
		{
			PlaySwipe();
			m_swipeWait = SWIPE_SFX_WAIT;
		}
		if (m_timeTillNextSplat > -1f)
		{
			m_timeTillNextSplat -= dt;
		}
		while (m_splatsToMake >= 0 && m_timeTillNextSplat <= 0f)
		{
			if (m_lastDir.LengthSquared() > 1f && m_lastDir.LengthSquared() < 10000f)
			{
				m_lastSlashDir = m_lastDir;
			}
			m_splatsToMake--;
			m_splatInterval = Mortar.Math.MIN(m_splatInterval + Mortar.Math.g_random.RandF(0.05f) + 0.01f, 0.03f);
			m_timeTillNextSplat += m_splatInterval;
			SplatEntity free = SplatEntity.GetFree();
			if (free != null)
			{
				bool noSound = m_lastSlashFruitType < Fruit.MAX_FRUIT_TYPES && Fruit.FruitInfo(m_lastSlashFruitType).superFruit;
				free.MakeSplat(Game.game_work.camera.TranslatePos(m_pos, screenToWorld: true, includeShake: true), new Vector3(m_lastSlashDir.X * (Mortar.Math.g_random.RandF(0.75f) + 0.75f), m_lastSlashDir.Y * (Mortar.Math.g_random.RandF(0.75f) + 0.75f), 0f), canBeLong: true, noSound, m_lastSlashFruitType);
			}
		}
	}

	public override void Draw()
	{
	}

	public void DrawSlice()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (slashes > 0)
		{
			slashes = 0;
		}
		if (m_drawCount >= 4)
		{
			if (s_slashAltTexture != null)
			{
				s_slashAltTexture.Set();
			}
			else
			{
				s_slashTexture.Set();
			}
			MatrixManager.instance.Reset();
			MatrixManager.instance.Translate(Vector3.UnitZ);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawTriStripEx(m_points, m_drawCount, hasAlpha: false);
			Mesh.DrawTriStripEx(m_pointPoints, 3, hasAlpha: false);
		}
	}

	public override void DrawUpdate(float dt)
	{
		m_touchDown = (byte)((m_touchDown << 1) & 3);
		updated_last_frame = updated;
		updated = false;
	}

	public override bool CollisionResponse(Entity p_ent2, uint col_1, uint col_2, ref Vector3 proj)
	{
		return false;
	}

	public static uint ParseSlashPowerMask(string text)
	{
		if (text != null)
		{
			uint num = StringFunctions.StringHash(text);
			for (int i = 0; i < hashes.Length; i++)
			{
				if (hashes[i] == num)
				{
					return (uint)(1 << i);
				}
			}
		}
		return 0u;
	}

	public bool CollideWithEntity(Entity ent)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (m_col_box != null && m_col_length > 0f && ent != null && ent.m_col_box != null)
		{
			if (Game.game_work.pause || Game.game_work.inRetrySequence)
			{
				return false;
			}
			Vector3 proj;
			if (ent.m_col_box.GetType() != COLISIONOBJECT.COL_SPHERE)
			{
				return m_col_box.Collide(ent.m_col_box, out proj);
			}
			if (m_col_box.Collide(ent.m_col_box, out proj))
			{
				float num = ((ColSphere)ent.m_col_box).Radius * ((ColSphere)ent.m_col_box).Radius;
				Vector3 val = m_col_box.centre - ent.m_col_box.centre;
				if (val.LengthSquared() < num)
				{
					return true;
				}
				Vector3 val2 = proj + ent.m_col_box.centre;
				Vector3 val3 = Vector3.Zero;
				float num2 = proj.LengthSquared();
				if (num2 < num)
				{
					float num3 = Mortar.Math.Sqrt(num - num2);
					val3 = Vector3.Cross(proj, new Vector3(0f, 0f, 1f));
					val3.Normalize();
					val3 *= ((ColSphere)ent.m_col_box).Radius - num3;
				}
				Vector3 val4 = m_col_box.centre - (val2 + val3);
				Vector3 val5 = m_col_box.centre - (val2 - val3);
				if (val4.LengthSquared() < m_col_length || val5.LengthSquared() < m_col_length)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool TouchMoveX(InputEvent e)
	{
		if (Game.game_work.hitBombTime > 0f)
		{
			return false;
		}
		MortarRectangle windowSize = DisplayManager.GetInstance().GetWindowSize();
		Vector2 val = default(Vector2);
		val = new Vector2((float)(windowSize.right - windowSize.left), (float)(windowSize.bottom - windowSize.top));
		m_pos.X = (e.axis.absolutePos - val.X / 2f) * (Game.SCREEN_WIDTH / Game.SCREEN_SIZE_X);
		return true;
	}

	public bool TouchMoveY(InputEvent e)
	{
		if (Game.game_work.hitBombTime > 0f)
		{
			return false;
		}
		MortarRectangle windowSize = DisplayManager.GetInstance().GetWindowSize();
		Vector2 val = default(Vector2);
		val = new Vector2((float)(windowSize.right - windowSize.left), (float)(windowSize.bottom - windowSize.top));
		m_pos.Y = (0f - (e.axis.absolutePos - val.Y / 2f)) * (Game.SCREEN_HEIGHT / Game.SCREEN_SIZE_Y);
		return true;
	}

	public bool TouchDown(InputEvent e)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		if (WaveManager.GetInstance().CriticalMode())
		{
			if (m_emitter == null)
			{
				uint hash = StringFunctions.StringHash("crit_hit_stars");
				m_emitter = PSPParticleManager.GetInstance().AddEmitter(hash, null);
				if (m_emitter != null)
				{
					m_emitter.updateEvenIfPaused = true;
				}
			}
			m_emitter.pos = Game.game_work.camera.TranslatePos(m_pos, screenToWorld: true, includeShake: false);
		}
		else if (ModHasPartilces)
		{
			if (m_emitter == null)
			{
				m_emitter = PSPParticleManager.GetInstance().AddEmitter(ModPartilcesHash, null);
				if (m_emitter != null)
				{
					m_emitter.updateEvenIfPaused = true;
				}
			}
		}
		else if (m_emitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_emitter);
			m_emitter = null;
		}
		if (m_emitter != null)
		{
			m_emitter.pos = Game.game_work.camera.TranslatePos(m_pos, screenToWorld: true, includeShake: false);
		}
		if (Game.game_work.hitBombTime > 0f || !updated_last_frame)
		{
			return false;
		}
		if (m_touchDown == 0)
		{
			Reset();
			if (ModColorType == 2)
			{
				UpdateModColor(ref m_slashModColor, 1f);
			}
		}
		Vector3 dir = m_pos - m_oldPositions[0];
		if ((m_touchDown == 0 && dir.X * dir.X + dir.Y * dir.Y >= START_CHANGED_DIST * START_CHANGED_DIST) || (m_touchDown != 0 && dir.X * dir.X + dir.Y * dir.Y >= CHANGED_DIST * CHANGED_DIST))
		{
			AddPoint(m_pos, dir);
			m_lastPos = m_pos;
			for (int num = m_oldPositions.Length - 1; num > 0; num--)
			{
				ref Vector3 reference = ref m_oldPositions[num];
				reference = m_oldPositions[num - 1];
			}
			ref Vector3 reference2 = ref m_oldPositions[0];
			reference2 = m_pos;
		}
		m_touchDown |= 1;
		return true;
	}

	public void MissControlDeleted(HUDControl control)
	{
		if (missControl == control)
		{
			missControl = null;
		}
	}

	public static void InitModColors()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ModHasPartilces = false;
		ModPartilcesHash = 0u;
		ModColorTime = 0f;
		NumModColors = 1;
		ModColorType = 0;
		s_slashAltTexture = null;
		for (int i = 0; i < 16; i++)
		{
			ref Color reference = ref ModColors[i];
			reference = Color.White;
		}
	}

	public static void UpdateModColor(ref Color colourIn, float dt)
	{
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (dt != 0f)
		{
			if (NumModColors == 1)
			{
				colourOut = ModColors[0];
			}
			else
			{
				ModColorTime += dt * ModColorChangeSpeed;
				while (ModColorTime > (float)NumModColors)
				{
					ModColorTime -= NumModColors;
				}
				if (ModColorTime < 0f)
				{
					if (ModColorType == 0)
					{
						while (ModColorTime < 0f)
						{
							ModColorTime += NumModColors;
						}
					}
					else
					{
						ModColorTime = 0f;
					}
				}
				if (Mortar.Math.ABS(ModColorTime - (float)(int)(ModColorTime + 0.5f)) < 0.01f)
				{
					colourOut = ModColors[(int)(ModColorTime + 0.5f) % NumModColors];
				}
				else
				{
					float num = ModColorTime - (float)(int)ModColorTime;
					Color val = ModColors[(int)ModColorTime % NumModColors];
					Color val2 = ModColors[((int)ModColorTime + 1) % NumModColors];
					colourOut.R = (byte)((float)(int)val.R + (float)(val2.R - val.R) * num);
					colourOut.G = (byte)((float)(int)val.G + (float)(val2.G - val.G) * num);
					colourOut.B = (byte)((float)(int)val.B + (float)(val2.B - val.B) * num);
					colourOut.A = (byte)((float)(int)val.A + (float)(val2.A - val.A) * num);
				}
			}
		}
		colourIn = colourOut;
	}

	public static void SetModColors(Color[] colours, int numColors, int changeType, float speed)
	{
		SetModColors(colours, numColors, changeType, speed, null);
	}

	public static void SetModColors(Color[] colours, int numColors, int changeType, float speed, string particles)
	{
		SetModColors(colours, numColors, changeType, speed, particles, null);
	}

	public static void SetModColors(Color[] colours, int numColors, int changeType, float speed, string particles, string texture)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ModColorType = changeType;
		NumModColors = numColors;
		ModColorChangeSpeed = speed;
		for (int i = 0; i < NumModColors; i++)
		{
			ref Color reference = ref ModColors[i];
			reference = colours[i];
		}
		colourOut = ModColors[0];
		ModColorTime = 0f;
		ModHasPartilces = false;
		ModPartilcesHash = 0u;
		if (texture != null)
		{
			s_slashAltTexture = TextureManager.GetInstance().Load(texture);
		}
		else
		{
			s_slashAltTexture = null;
		}
		if (particles != null)
		{
			ModPartilcesHash = StringFunctions.StringHash(particles);
			if (PSPParticleManager.GetInstance().EmitterExists(ModPartilcesHash))
			{
				ModHasPartilces = true;
			}
		}
	}

	public static int ParseSlashModColorType(string text)
	{
		if (text != null)
		{
			uint num = StringFunctions.StringHash(text);
			for (int i = 0; i < 4; i++)
			{
				if (num == ParseSlashModColorTypehashes[i])
				{
					return i;
				}
			}
		}
		return 0;
	}

	public static void CleanupSlash()
	{
		s_slashTexture = null;
		s_slashAltTexture = null;
		loaded = false;
	}
}
