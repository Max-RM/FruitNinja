using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mortar;

namespace FruitNinja;

public class SplatEntity
{
	protected bool m_hasSeeds;

	protected float m_criticalFadeTime;

	protected Color m_colour;

	protected float m_startAlpha;

	protected float m_rotation;

	protected int m_fruitType;

	protected bool m_canBeLong;

	protected Vector3 m_xVec;

	protected Vector3 m_yVec;

	protected bool m_flip;

	public ushort m_dir_angle;

	public Vector3 m_pos;

	public Vector3 m_cur_scale;

	public Vector3 m_vel;

	public int m_onWall;

	public Vector3 m_orig_scale;

	public float m_fadeTime;

	public float m_fadeSpeed;

	public bool m_update;

	public bool m_noSound;

	public static VertexPositionColorTexture[] m_points = ArrayInit.CreateFilledArray<VertexPositionColorTexture>(3072);

	public static int m_curr_drawing_splat = 0;

	public static SplatEntity[] pool;

	public static int poolCount;

	public static int currentFree;

	public static int WALL_DIST = 50;

	public static int WALL_SPLASH_VARIANTS = 6;

	public static float SPLAT_FRAME_LENGTH = 0.05f;

	public static int SPLAT_FRAMES = 2;

	public static float SPLAT_FRAME_WIDTH = 1f / (float)SPLAT_FRAMES;

	public static float SPLAT_ANGLE_VARIANT = 45f;

	public static float SPLAT_FADE_LENGTH = 5f;

	public static float SPLAT_FADE_SPEED = 0.5f;

	public static float SPLAT_SPRINKLE_WAIT = 0.25f;

	public static float SPLAT_SPRINKLE_SEPARATION = 0.5f;

	public static float CRITICAL_FADE_TIME = 0.5f;

	private static Mortar.Texture splatTexture;

	private static float[] wallMaxDrop = new float[6] { 2.5f, 2.5f, 2.5f, 2.9f, 0f, 0f };

	private static float[] splatScale = new float[6] { 1.6f, 1.6f, 1.6f, 1.6f, 2.9f, 2.9f };

	private static float[] splatUV = new float[24]
	{
		0f, 0.5f, 0f, 0.25f, 0.5f, 1f, 0f, 0.25f, 0f, 0.5f,
		0.25f, 0.5f, 0.5f, 1f, 0.25f, 0.5f, 0f, 1f, 0.5f, 0.75f,
		0f, 1f, 0.75f, 1f
	};

	private static bool loadedSplat = false;

	private static float sprinkleWait = 0f;

	public static float SPLAT_SFX_WAIT = 0.5f;

	private static float splatSpeed = 0f;

	private static float[] sfxWait;

	private static int numActiveSplats;

	private static int coconut;

	public static int RAND_SPLAT
	{
		get
		{
			if (Math.g_random.Rand32(6) <= 0)
			{
				return 0;
			}
			return 1;
		}
	}

	public static int RAND_SPRINKLE
	{
		get
		{
			if (Math.g_random.Rand32(2) != 0)
			{
				return 3;
			}
			return 2;
		}
	}

	public static int RAND_LONG_SPLAT
	{
		get
		{
			if (Math.g_random.Rand32(2) != 0)
			{
				return 5;
			}
			return 4;
		}
	}

	public static bool IS_LONG_SPLAT(int s)
	{
		if (s != 4)
		{
			return s == 5;
		}
		return true;
	}

	private void PlaySplat(int smallMediumLarge)
	{
		smallMediumLarge = Math.CLAMP(smallMediumLarge, 0, 2);
		sfxWait[smallMediumLarge] -= 0.05f;
		if (sfxWait[smallMediumLarge] <= 0f)
		{
			sfxWait[smallMediumLarge] = SPLAT_SFX_WAIT;
			switch (smallMediumLarge)
			{
			case 0:
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_DRIP);
				break;
			case 1:
				SoundManager.GetInstance().SFXPlay((Math.g_random.Rand32(2) != 0) ? SoundDef.SND_SPLATTER_SMALL_1 : SoundDef.SND_SPLATTER_SMALL_2);
				break;
			case 2:
				SoundManager.GetInstance().SFXPlay((Math.g_random.Rand32(2) != 0) ? SoundDef.SND_SPLATTER_MEDIUM_1 : SoundDef.SND_SPLATTER_MEDIUM_2);
				break;
			case 3:
				SoundManager.GetInstance().SFXPlay((Math.g_random.Rand32(2) != 0) ? SoundDef.SND_SPLATTER_LARGE_1 : SoundDef.SND_SPLATTER_LARGE_2);
				break;
			}
		}
	}

	public void MakeSplat(Vector3 pos, Vector3 dir, bool canBeLong)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeSplat(pos, dir, canBeLong, 0);
	}

	public void MakeSplat(Vector3 pos, Vector3 dir, bool canBeLong, int fruitType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeSplat(pos, dir, canBeLong, noSound: false, fruitType);
	}

	public void MakeSplat(Vector3 pos, Vector3 dir, bool canBeLong, bool noSound, int fruitType)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		m_noSound = noSound;
		m_canBeLong = canBeLong;
		if (fruitType >= Fruit.MAX_FRUIT_TYPES)
		{
			m_criticalFadeTime = 1f + SPLAT_SPRINKLE_SEPARATION;
			m_colour = Fruit.CRITICAL_COLOUR;
		}
		else
		{
			m_criticalFadeTime = 0f;
			m_colour = Fruit.FruitTypeColor(fruitType);
		}
		m_flip = Math.g_random.Rand32(2) != 0;
		m_startAlpha = (int)m_colour.A;
		m_pos = pos;
		m_pos.Z = 0f;
		m_vel = dir;
		float num = m_vel.Length();
		m_vel.Z = (0f - num) * 0.5f - 150f - Math.g_random.RandF(10f);
		ref Vector3 vel = ref m_vel;
		vel.Y *= 1.5f;
		m_vel *= 6f;
		m_rotation = Math.g_random.Rand32(360);
		m_fruitType = fruitType;
		FRUIT_INFO fRUIT_INFO = Fruit.FruitInfo(m_fruitType % Fruit.MAX_FRUIT_TYPES);
		m_hasSeeds = fRUIT_INFO.hasSplatSeeds;
		m_cur_scale.X = (m_cur_scale.Y = (m_cur_scale.Z = Math.g_random.RandF(10f) + 15f));
		ref Vector3 cur_scale = ref m_cur_scale;
		cur_scale.Y *= -1f;
		m_orig_scale = m_cur_scale;
		m_onWall = -1;
		Vector3 size = default(Vector3);
		Init(null, out size);
		if (Math.g_random.Rand32(4) == 0 || m_colour.A == 0 || (m_fruitType < Fruit.MAX_FRUIT_TYPES && Fruit.FruitInfo(m_fruitType).onlySprinkle && Math.g_random.Rand32(3) == 0))
		{
			m_update = false;
		}
		m_xVec = new Vector3(Math.CosIdx(Math.DEGREE_TO_IDX(m_rotation)), Math.SinIdx(Math.DEGREE_TO_IDX(m_rotation)), 0f) * 0.5f;
		m_yVec = new Vector3(Math.CosIdx(Math.DEGREE_TO_IDX(m_rotation + 90f)), Math.SinIdx(Math.DEGREE_TO_IDX(m_rotation + 90f)), 0f) * 0.5f;
	}

	public SplatEntity()
	{
		m_onWall = -1;
		m_update = false;
	}

	public static void LoadContent()
	{
		if (!loadedSplat)
		{
			loadedSplat = true;
			splatTexture = TextureManager.GetInstance().Load("textureswp7/white_splash.tex");
		}
	}

	public virtual void Init(byte[] tpl_data, out Vector3 size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		size = default(Vector3);
		m_onWall = -1;
		m_update = true;
	}

	public virtual void Release()
	{
	}

	public static void CreatePool()
	{
		CreatePool(512);
	}

	public static void CreatePool(int num)
	{
		Delete.SAFE_DELETE_ARRAY(ref pool);
		pool = ArrayInit.CreateFilledArray<SplatEntity>(num);
		poolCount = num;
		currentFree = 0;
	}

	public static SplatEntity GetFree()
	{
		int num = 0;
		while (pool[currentFree].m_update && num < poolCount)
		{
			num++;
			if (++currentFree >= poolCount)
			{
				currentFree = 0;
			}
		}
		return pool[currentFree];
	}

	public static void CleanUp()
	{
		Delete.SAFE_DELETE_ARRAY(ref pool);
		poolCount = 0;
	}

	public static void UpdateActiveSplats(float dt)
	{
		for (int i = 0; i < 3; i++)
		{
			if (sfxWait[i] > 0f)
			{
				sfxWait[i] -= dt;
			}
		}
		if (sprinkleWait > 0f)
		{
			sprinkleWait -= dt;
			if (sprinkleWait <= 0f)
			{
				SoundManager.GetInstance().SFXPlay(SoundDef.SND_DRIP);
			}
		}
		else if (sprinkleWait >= 0f - SPLAT_SPRINKLE_SEPARATION)
		{
			sprinkleWait -= dt;
		}
		splatSpeed = 1.25f + Math.CLAMP((float)(ActorManager.GetInstance().GetNumEntities() + NumActiveSplats()) / 15f - 0.15f, 0f, 3f);
		if (!Game.IsFastHardware() || Game.IsMultiplayer())
		{
			splatSpeed *= 1.5f;
		}
		splatSpeed *= 1.5f;
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < poolCount; j++)
		{
			SplatEntity splatEntity = pool[num2];
			if (splatEntity.m_update)
			{
				splatEntity.Update(dt);
				num++;
			}
			num2++;
		}
		numActiveSplats = num;
	}

	public static void DrawActiveSplats()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		m_curr_drawing_splat = 0;
		int num = 0;
		for (int i = 0; i < poolCount; i++)
		{
			SplatEntity splatEntity = pool[num];
			if (splatEntity.m_update && splatEntity.m_onWall >= 0)
			{
				splatEntity.DrawSplat();
				m_curr_drawing_splat++;
			}
			num++;
		}
		if (m_curr_drawing_splat > 0 && splatTexture != null && m_points != null)
		{
			splatTexture.Set();
			MatrixManager.instance.Reset();
			MatrixManager.instance.Translate(Vector3.UnitZ * -5500f);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawTriListEx(m_points, m_curr_drawing_splat * 6, hasAlpha: true, 0);
		}
	}

	public static void RemoveAllSplats()
	{
		for (int i = 0; i < poolCount; i++)
		{
			pool[i].Destroy();
		}
	}

	public static int NumActiveSplats()
	{
		return numActiveSplats;
	}

	public void Destroy()
	{
		m_update = false;
	}

	private void SetVertex(ref VertexPositionColorTexture vertex, float x, float y, float u, float v, Color c)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		vertex.Position = new Vector3(x, y, 0f);
		vertex.TextureCoordinate = new Vector2(u, v);
		vertex.Color = c;
	}

	private void SetVertex2(ref VertexPositionColorTexture vertex, Color c)
	{
	}

	public virtual void DrawSplat()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		int num = m_curr_drawing_splat * 6;
		float num2 = splatUV[m_onWall << 2] * 0.5f;
		float num3 = splatUV[(m_onWall << 2) + 1] * 0.5f;
		if (m_hasSeeds)
		{
			num2 += 0.5f;
			num3 += 0.5f;
		}
		Color c = Game.TintColour(m_colour, Game.game_work.hud.m_backTint);
		c.R = (byte)(c.R + (byte)((c.R < 224) ? 32 : 0));
		c.G = (byte)(c.G + (byte)((c.G < 224) ? 32 : 0));
		c.B = (byte)(c.B + (byte)((c.B < 224) ? 32 : 0));
		c.A = (byte)(c.A + (byte)((c.A < 224) ? 32 : 0));
		float num4 = splatUV[(m_onWall << 2) + 2];
		float v = splatUV[(m_onWall << 2) + 3];
		if (m_flip)
		{
			v = num4;
			num4 = splatUV[(m_onWall << 2) + 3];
		}
		SetVertex(ref m_points[num], m_pos.X + (m_xVec.X + m_yVec.X) * m_cur_scale.X, m_pos.Y + (m_xVec.Y + m_yVec.Y) * m_cur_scale.Y, num3, num4, c);
		SetVertex(ref m_points[num + 1], m_pos.X + (0f - m_xVec.X + m_yVec.X) * m_cur_scale.X, m_pos.Y + (0f - m_xVec.Y + m_yVec.Y) * m_cur_scale.Y, num2, num4, c);
		SetVertex(ref m_points[num + 2], m_pos.X + (m_xVec.X - m_yVec.X) * m_cur_scale.X, m_pos.Y + (m_xVec.Y - m_yVec.Y) * m_cur_scale.Y, num3, v, c);
		ref VertexPositionColorTexture reference = ref m_points[num + 3];
		reference = m_points[num + 2];
		ref VertexPositionColorTexture reference2 = ref m_points[num + 4];
		reference2 = m_points[num + 1];
		SetVertex(ref m_points[num + 5], m_pos.X + (0f - m_xVec.X - m_yVec.X) * m_cur_scale.X, m_pos.Y + (0f - m_xVec.Y - m_yVec.Y) * m_cur_scale.Y, num2, v, c);
	}

	public virtual void Update(float dt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		m_pos += m_vel * Game.game_work.dt;
		if (m_onWall < 0)
		{
			if (m_pos.Z < (float)(-WALL_DIST))
			{
				m_onWall = ((Math.g_random.Rand32(4) > 0) ? RAND_SPLAT : RAND_SPRINKLE);
				if (m_canBeLong && Math.g_random.Rand32(2) == 0)
				{
					m_onWall = RAND_LONG_SPLAT;
				}
				if (m_fruitType < Fruit.MAX_FRUIT_TYPES && Fruit.FruitInfo(m_fruitType).onlySprinkle)
				{
					m_onWall = RAND_SPRINKLE;
				}
				if (IS_LONG_SPLAT(m_onWall))
				{
					m_rotation = (0f - (float)(int)Math.Atan2Idx(m_vel.Y, m_vel.X)) / 182f;
					m_rotation += Math.g_random.RandF(SPLAT_ANGLE_VARIANT) - SPLAT_ANGLE_VARIANT / 2f;
					m_xVec = new Vector3(Math.CosIdx(Math.DEGREE_TO_IDX(m_rotation)), Math.SinIdx(Math.DEGREE_TO_IDX(m_rotation)), 0f) * 0.5f;
					m_yVec = new Vector3(Math.CosIdx(Math.DEGREE_TO_IDX(m_rotation + 90f)), Math.SinIdx(Math.DEGREE_TO_IDX(m_rotation + 90f)), 0f) * 0.25f;
				}
				m_pos.Z = -WALL_DIST;
				m_vel = Vector3.Zero;
				m_cur_scale *= 2.5f * splatScale[m_onWall];
				if (!m_noSound)
				{
					if (m_fruitType == coconut)
					{
						PlaySplat(0);
					}
					else if (m_cur_scale.X > 100f)
					{
						PlaySplat(3);
					}
					else if (m_cur_scale.X > 30f)
					{
						PlaySplat(2);
					}
					else
					{
						PlaySplat(1);
					}
				}
				if (Game.IsMultiplayer())
				{
					m_cur_scale *= Game.SPLIT_SCREEN_SCALE;
				}
				m_fadeTime = Math.g_random.RandF(SPLAT_FADE_LENGTH * 0.5f) + SPLAT_FADE_LENGTH * 0.75f;
				m_fadeSpeed = Math.g_random.RandF(SPLAT_FADE_SPEED * 0.5f) + SPLAT_FADE_SPEED * 0.75f;
				if (Math.g_random.Rand32(10) == 0 && sprinkleWait < 0f - SPLAT_SPRINKLE_SEPARATION)
				{
					sprinkleWait = SPLAT_SPRINKLE_WAIT;
				}
			}
			if (Game.IsMultiplayer())
			{
				float num = Game.game_work.dt * 10f;
				ref Vector3 vel = ref m_vel;
				vel.X += ((m_pos.X < 0f) ? (0f - num) : num);
			}
			else
			{
				ref Vector3 vel2 = ref m_vel;
				vel2.Y -= Game.game_work.dt * 10f;
			}
			return;
		}
		dt *= splatSpeed;
		if (m_fadeTime <= 1.25f)
		{
			if (Game.IsMultiplayer())
			{
				float num2 = dt * wallMaxDrop[m_onWall];
				ref Vector3 vel3 = ref m_vel;
				vel3.X += ((m_pos.X < 0f) ? (0f - num2) : num2);
				m_vel.X = Math.CLAMP(m_vel.X, -50f, 50f);
				ref Vector3 cur_scale = ref m_cur_scale;
				cur_scale.X += ((m_pos.X < 0f) ? (0f - num2) : num2);
				m_cur_scale.X = Math.CLAMP(m_cur_scale.X, -200f, 200f);
			}
			else
			{
				ref Vector3 vel4 = ref m_vel;
				vel4.Y -= dt * wallMaxDrop[m_onWall];
				m_vel.Y = Math.MAX(-50f, m_vel.Y);
				ref Vector3 cur_scale2 = ref m_cur_scale;
				cur_scale2.Y -= dt * wallMaxDrop[m_onWall];
				m_cur_scale.Y = Math.MAX(-200f, m_cur_scale.Y);
			}
		}
		if (m_criticalFadeTime > 0f)
		{
			m_criticalFadeTime -= Game.game_work.dt;
			if (m_criticalFadeTime <= 0f)
			{
				m_criticalFadeTime = 0f;
			}
			float num3 = 1f - Math.CLAMP(m_criticalFadeTime / CRITICAL_FADE_TIME, 0f, 1f);
			Color val = Fruit.FruitTypeColor(m_fruitType % Fruit.MAX_FRUIT_TYPES);
			m_colour.R = (byte)((float)(int)Fruit.CRITICAL_COLOUR.R + (float)(val.R - Fruit.CRITICAL_COLOUR.R) * num3);
			m_colour.G = (byte)((float)(int)Fruit.CRITICAL_COLOUR.G + (float)(val.G - Fruit.CRITICAL_COLOUR.G) * num3);
			m_colour.B = (byte)((float)(int)Fruit.CRITICAL_COLOUR.B + (float)(val.B - Fruit.CRITICAL_COLOUR.B) * num3);
			m_startAlpha = (float)(int)Fruit.CRITICAL_COLOUR.A + (float)(val.A - Fruit.CRITICAL_COLOUR.A) * num3;
		}
		m_fadeTime -= dt * m_fadeSpeed;
		if (m_fadeTime <= 0f)
		{
			m_fadeTime = 0f;
			Destroy();
		}
		m_colour.A = (byte)Math.MIN(m_startAlpha, m_startAlpha * m_fadeTime);
	}

	public virtual void Draw()
	{
	}

	public virtual void DrawUpdate(float dt)
	{
	}

	public static void CleanUpSplat()
	{
		CleanUp();
		loadedSplat = false;
		splatTexture = null;
	}

	static SplatEntity()
	{
		float[] array = new float[4];
		sfxWait = array;
		numActiveSplats = 0;
		coconut = Fruit.FruitType("coconut");
	}
}
