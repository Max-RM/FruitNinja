using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class MissControl : HUDControl3d
{
	public class TextureStore
	{
		public Texture s_cross;

		public Texture s_critical;

		public Texture s_ultraRare;

		public Texture[] s_combo = new Texture[10];
	}

	public const int SHAKE_TIME = 30;

	public const int MAX_RANGE = 8;

	public const float DISAPPEAR_TIME = 1.66f;

	public const float FADE_OUT_TIME = 0f;

	public const float DISAPPEAR_WAVES = 2.25f;

	public const float DISAPPEAR_Y = 0f;

	public const float DISAPPEAR_X = 3f;

	public const float DISAPPEAR_SCALE = 48f;

	public const int MAX_COMBO = 10;

	public const int REVOLUTIONS = 6;

	public const float MIDDLE_SCALE = 0.65f;

	public int m_id;

	public bool m_registered;

	public short m_shakeTimer;

	public float m_fadeAway;

	public bool m_isCritical;

	public bool m_isCombo;

	public int m_fruitsInCombo;

	private bool m_playSound;

	public static int s_numCriticals;

	public static float s_dtMod = 1f;

	public float m_dtMod;

	public static int s_refCount = 0;

	private static WeakReference s_sharedStore;

	private TextureStore m_store;

	public static MissControl[] pool;

	public static int poolCount;

	public static int curentFree;

	private Texture s_cross
	{
		get
		{
			return m_store.s_cross;
		}
		set
		{
			m_store.s_cross = value;
		}
	}

	private Texture s_critical
	{
		get
		{
			return m_store.s_critical;
		}
		set
		{
			m_store.s_critical = value;
		}
	}

	private Texture s_ultraRare
	{
		get
		{
			return m_store.s_ultraRare;
		}
		set
		{
			m_store.s_ultraRare = value;
		}
	}

	private Texture[] s_combo
	{
		get
		{
			return m_store.s_combo;
		}
		set
		{
			m_store.s_combo = value;
		}
	}

	public static void MakeEmAllDissappear()
	{
		for (int i = 0; i < poolCount; i++)
		{
			MissControl missControl = pool[i];
			if (missControl.m_update)
			{
				missControl.m_fadeAway = Mortar.Math.MIN(missControl.m_fadeAway, 0.06916667f);
			}
		}
	}

	public MissControl()
	{
		if (s_sharedStore != null)
		{
			m_store = s_sharedStore.Target as TextureStore;
		}
		if (m_store == null)
		{
			m_store = new TextureStore();
			s_sharedStore = new WeakReference(m_store);
			s_cross = TextureManager.GetInstance().Load("textureswp7/hud_cross.tex");
			s_critical = TextureManager.GetInstance().Load("critical.tex", localise: true);
			s_ultraRare = TextureManager.GetInstance().Load("ultra_rare_plus_50.tex", localise: true);
			for (int i = 1; i < 11; i++)
			{
				if (i < 3)
				{
					s_combo[i - 1] = null;
					continue;
				}
				string texture = $"combo_{i}.tex";
				s_combo[i - 1] = TextureManager.GetInstance().Load(texture, localise: true);
			}
		}
		s_refCount++;
		Init();
		m_update = false;
	}

	public override void Init()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		m_update = true;
		m_playSound = true;
		m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_NORMAL;
		m_id = 0;
		m_isCritical = false;
		m_rotation = 0f;
		m_id = 0;
		m_texture = s_cross;
		m_update = true;
		m_fadeAway = 0f;
		m_terminate = false;
		m_isCombo = false;
		m_dtMod = 1f;
		m_fadeAway = 0f;
		m_fruitsInCombo = 0;
		m_scale = new Vector3((float)(m_texture.GetWidth() / 2 + 1), (float)(m_texture.GetWidth() / 2 + 1), 0f);
		Reset();
	}

	public override void Release()
	{
		m_texture = null;
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (Game.game_work.gameMode != Game.GAME_MODE.GM_CLASSIC && !m_isCombo && !m_isCritical)
		{
			return;
		}
		Vector3 zero = Vector3.Zero;
		float num = 1f;
		if (m_shakeTimer > 0)
		{
			int num2 = Mortar.Math.g_random.Rand32(8) - 4;
			int num3 = Mortar.Math.g_random.Rand32(8) - 4;
			zero = new Vector3((float)num2, (float)num3, 0f);
			m_shakeTimer--;
		}
		if (m_fadeAway > 0f)
		{
			if (m_fadeAway > 1.66f)
			{
				return;
			}
			float num4 = m_fadeAway / 1.66f * 360f * 6f * 182f;
			num = Mortar.Math.Abs(Mortar.Math.SinIdx((ushort)num4));
			if (num4 > (float)(int)Mortar.Math.DEGREE_TO_IDX(90f) && num4 < 376740f)
			{
				num = ((!(num4 < (float)(int)Mortar.Math.DEGREE_TO_IDX(180f)) && !(num4 > 360360f)) ? 0.65f : Mortar.Math.MAX(num, 0.65f));
			}
		}
		else if (Game.FailureEnabled() && !Game.IsMultiplayer())
		{
			zero.Y += Mortar.Math.Abs(Game.game_work.gameOverTransition) * (Game.SCREEN_HEIGHT / 2f - m_pos.Y) * 3f;
		}
		else
		{
			zero.Y += 1f * (Game.SCREEN_HEIGHT / 2f - m_pos.Y) * 3f;
		}
		if (m_texture != null)
		{
			m_texture.Set();
			MatrixManager.instance.Reset();
			MatrixManager.instance.Scale(m_scale * num);
			if (m_rotation != 0f)
			{
				MatrixManager.instance.RotZ(MathHelper.ToRadians(m_rotation));
			}
			MatrixManager.instance.TranslateGlobal(m_pos + zero);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(m_color, m_isCritical ? 0f : (m_registered ? 0.5f : 0f), m_isCritical ? 1f : (m_registered ? 1f : 0.5f), m_isCritical ? 0f : 0.25f, m_isCritical ? 1f : 0.75f);
		}
	}

	public override void Update(float dt)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		int currentMissCount = Game.game_work.currentMissCount;
		if (!m_registered && currentMissCount >= m_id + 1)
		{
			m_color.A = byte.MaxValue;
			m_shakeTimer = 30;
			m_registered = true;
		}
		float num = dt;
		if (m_isCritical)
		{
			s_numCriticals++;
			Vector2 val = Vector2.Zero;
			Vector2 val2 = default(Vector2);
			for (int i = 0; i < poolCount; i++)
			{
				MissControl missControl = pool[i];
				if (!missControl.m_update || missControl == this)
				{
					continue;
				}
				val2 = new Vector2(missControl.m_pos.X - m_pos.X, missControl.m_pos.Y - m_pos.Y);
				float num2 = val2.LengthSquared();
				if (num2 < 4900f)
				{
					float num3 = 1f;
					if (num2 > 0f)
					{
						num3 = Mortar.Math.Sqrt(num2);
					}
					else
					{
						ushort idx = (ushort)Mortar.Math.g_random.Rand32(Mortar.Math.DEGREE_TO_IDX(359f));
						val2 = new Vector2(Mortar.Math.SinIdx(idx), Mortar.Math.CosIdx(idx));
					}
					val -= val2 / num3 * (70f - num3) * num * 15f;
				}
			}
			ref Vector3 pos = ref m_pos;
			pos.X += val.X;
			ref Vector3 pos2 = ref m_pos;
			pos2.Y += val.Y;
			dt *= s_dtMod * m_dtMod;
		}
		if (m_fadeAway > 0f)
		{
			if (!Game.game_work.pause)
			{
				float fadeAway = m_fadeAway;
				m_fadeAway -= dt;
				m_pos.Z = 0f;
				if (fadeAway >= 1.66f && m_fadeAway < 1.66f && m_isCritical && m_playSound)
				{
					string filename = ((!m_isCombo) ? SoundDef.SND_NEW_BEST : $"combo-{Mortar.Math.CLAMP(m_fruitsInCombo - 2, 1, 8)}");
					SoundManager.GetInstance().SFXPlay(filename);
				}
				if (m_fadeAway <= 0f)
				{
					m_deleteCall(this);
					m_update = false;
				}
			}
		}
		else if (m_registered && currentMissCount <= m_id)
		{
			m_color.A = byte.MaxValue;
			m_shakeTimer = 30;
			m_registered = false;
		}
	}

	public override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_color = Color.White;
		m_color.A = byte.MaxValue;
		m_shakeTimer = 0;
		m_registered = false;
		if (m_fadeAway > 0f)
		{
			m_color.A = 0;
			m_update = false;
		}
	}

	public static void PreUpdate(float dt)
	{
		s_dtMod = 0.5f + (float)s_numCriticals * 1f;
		s_numCriticals = 0;
	}

	public void MakeDisappear(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MakeDisappear(pos, 0);
	}

	public void MakeDisappear(Vector3 pos, int forPlayer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MakeDisappear(pos, forPlayer, null);
	}

	public void MakeDisappear(Vector3 pos, int forPlayer, Texture texture)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		Init();
		m_pos = pos;
		m_color.A = byte.MaxValue;
		if (texture != null)
		{
			m_playSound = false;
			m_texture = texture;
			m_isCritical = true;
			m_fadeAway = 1.81f;
			m_id = Game.MAX_FRUIT_MISSES;
			m_shakeTimer = 0;
			m_registered = true;
			m_scale = new Vector3((float)(m_texture.GetWidth() + 1), (float)(m_texture.GetHeight() + 1), 0f);
			SetPlayer(forPlayer);
		}
		else
		{
			m_fadeAway = 1.66f;
			m_id = Game.MAX_FRUIT_MISSES;
			m_shakeTimer = (short)((forPlayer > 0) ? 30 : 0);
			m_registered = true;
			m_scale = Vector3.One * 48f;
			SetPlayer(forPlayer);
			m_scale = Vector3.One * 48f;
			m_pos.X = Mortar.Math.CLAMP(m_pos.X, 0f - Game.SCREEN_WIDTH / 2f + m_scale.X * 0.5f, Game.SCREEN_WIDTH / 2f - m_scale.X * 0.5f);
			m_pos.Y = Mortar.Math.CLAMP(m_pos.Y, 0f - Game.SCREEN_HEIGHT / 2f + m_scale.Y * 0.5f, Game.SCREEN_HEIGHT / 2f - m_scale.Y * 0.5f);
		}
	}

	public void MakeCritical(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MakeCritical(pos, 0);
	}

	public void MakeCritical(Vector3 pos, int forPlayer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		Init();
		m_texture = s_critical;
		m_isCritical = true;
		m_pos = pos;
		m_fadeAway = 1.81f;
		m_color.A = byte.MaxValue;
		m_id = Game.MAX_FRUIT_MISSES;
		m_shakeTimer = 0;
		m_registered = true;
		if (Game.game_work.language == StringTableUtils.Language.LANGUAGE_ITALIAN || Game.game_work.language == StringTableUtils.Language.LANGUAGE_FRENCH || Game.game_work.language == StringTableUtils.Language.LANGUAGE_GERMAN || Game.game_work.language == StringTableUtils.Language.LANGUAGE_SPANISH)
		{
			m_scale = new Vector3((float)(m_texture.GetWidth() * 2), (float)(m_texture.GetHeight() * 2), 0f);
		}
		else
		{
			m_scale = new Vector3((float)(m_texture.GetWidth() + 20), (float)(m_texture.GetHeight() + 20), 0f);
		}
		m_scale *= 0.5f;
		if (m_pos.X + m_scale.X > Game.SCREEN_WIDTH * 0.5f)
		{
			m_pos.X = Game.SCREEN_WIDTH * 0.5f - m_scale.X;
		}
		if (m_pos.Y + m_scale.Y > Game.SCREEN_HEIGHT * 0.5f)
		{
			m_pos.Y = Game.SCREEN_HEIGHT * 0.5f - m_scale.Y;
		}
		if (m_pos.X - m_scale.X < (0f - Game.SCREEN_WIDTH) * 0.5f)
		{
			m_pos.X = (0f - Game.SCREEN_WIDTH) * 0.5f + m_scale.X;
		}
		if (m_pos.Y - m_scale.Y < (0f - Game.SCREEN_HEIGHT) * 0.5f)
		{
			m_pos.Y = (0f - Game.SCREEN_HEIGHT) * 0.5f + m_scale.Y;
		}
		m_scale += m_scale;
		SetPlayer(forPlayer);
	}

	public void MakeRare(Vector3 pos)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		Init();
		m_texture = s_ultraRare;
		m_isCritical = true;
		m_pos = pos;
		m_fadeAway = 1.81f;
		m_color.A = byte.MaxValue;
		m_id = Game.MAX_FRUIT_MISSES;
		m_shakeTimer = 0;
		m_registered = true;
		m_scale = new Vector3((float)(m_texture.GetWidth() + 1), (float)(m_texture.GetHeight() + 1), 0f);
		m_dtMod = 0.5f;
		m_scale *= 0.5f;
		if (m_pos.X + m_scale.X > Game.SCREEN_WIDTH * 0.5f)
		{
			m_pos.X = Game.SCREEN_WIDTH * 0.5f - m_scale.X;
		}
		if (m_pos.Y + m_scale.Y > Game.SCREEN_HEIGHT * 0.5f)
		{
			m_pos.Y = Game.SCREEN_HEIGHT * 0.5f - m_scale.Y;
		}
		if (m_pos.X - m_scale.X < (0f - Game.SCREEN_WIDTH) * 0.5f)
		{
			m_pos.X = (0f - Game.SCREEN_WIDTH) * 0.5f + m_scale.X;
		}
		if (m_pos.Y - m_scale.Y < (0f - Game.SCREEN_HEIGHT) * 0.5f)
		{
			m_pos.Y = (0f - Game.SCREEN_HEIGHT) * 0.5f + m_scale.Y;
		}
		m_scale += m_scale;
	}

	public void MakeCombo(Vector3 pos, int combo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MakeCombo(pos, combo, 0);
	}

	public void MakeCombo(Vector3 pos, int combo, int forPlayer)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		if (!TheGame.pomegranateThrown)
		{
			Init();
			m_texture = s_combo[Mortar.Math.CLAMP(combo - 1, 0, 9)];
			m_isCritical = true;
			m_isCombo = true;
			m_fruitsInCombo = combo;
			if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
			{
				m_fruitsInCombo = (int)(WaveManager.GetInstance().GetSpeed() + 0.65f);
			}
			m_pos = pos;
			m_fadeAway = 1.81f;
			m_color.A = byte.MaxValue;
			m_id = Game.MAX_FRUIT_MISSES;
			m_shakeTimer = 0;
			m_registered = true;
			if (Game.game_work.language == StringTableUtils.Language.LANGUAGE_ITALIAN || Game.game_work.language == StringTableUtils.Language.LANGUAGE_FRENCH || Game.game_work.language == StringTableUtils.Language.LANGUAGE_GERMAN || Game.game_work.language == StringTableUtils.Language.LANGUAGE_SPANISH)
			{
				m_scale = new Vector3((float)(m_texture.GetWidth() * 2), (float)(m_texture.GetHeight() * 2), 0f);
			}
			else
			{
				m_scale = new Vector3((float)(m_texture.GetWidth() + 1), (float)(m_texture.GetHeight() + 1), 0f);
			}
			m_scale *= 0.5f;
			if (m_pos.X + m_scale.X > Game.SCREEN_WIDTH * 0.5f)
			{
				m_pos.X = Game.SCREEN_WIDTH * 0.5f - m_scale.X;
			}
			if (m_pos.Y + m_scale.Y > Game.SCREEN_HEIGHT * 0.5f)
			{
				m_pos.Y = Game.SCREEN_HEIGHT * 0.5f - m_scale.Y;
			}
			if (m_pos.X - m_scale.X < (0f - Game.SCREEN_WIDTH) * 0.5f)
			{
				m_pos.X = (0f - Game.SCREEN_WIDTH) * 0.5f + m_scale.X;
			}
			if (m_pos.Y - m_scale.Y < (0f - Game.SCREEN_HEIGHT) * 0.5f)
			{
				m_pos.Y = (0f - Game.SCREEN_HEIGHT) * 0.5f + m_scale.Y;
			}
			m_scale += m_scale;
			SetPlayer(forPlayer);
		}
	}

	public void SetPlayer(int player)
	{
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_MISS;
	}

	public override void Skip()
	{
		int currentMissCount = Game.game_work.currentMissCount;
		if (currentMissCount >= m_id + 1)
		{
			m_color.A = byte.MaxValue;
			m_shakeTimer = 0;
			m_registered = true;
		}
	}

	public static void CreatePool(int num, HUD hud)
	{
		pool = ArrayInit.CreateFilledArray<MissControl>(num);
		poolCount = num;
		curentFree = 0;
		MissControl[] array = pool;
		foreach (MissControl missControl in array)
		{
			hud.AddControl(missControl);
			missControl.m_selfCleanUp = true;
		}
	}

	public static void CleanPool()
	{
		poolCount = 0;
	}

	public static MissControl GetFree()
	{
		int num = 0;
		while (pool[curentFree].m_update && num < poolCount)
		{
			num++;
			if (++curentFree >= poolCount)
			{
				curentFree = 0;
			}
		}
		return pool[curentFree];
	}
}
