using System;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class BombFlash
{
	protected float m_time;

	protected Color m_startColor;

	protected Color m_colour;

	protected float m_sinA;

	protected float m_cosA;

	protected Texture m_texture;

	public Vector3 m_pos;

	public Vector3 m_vel;

	public Vector3 m_cur_scale;

	public bool m_update;

	public ushort m_dir_angle;

	public static BombFlash[] pool;

	public static int poolCount;

	public static int currentFree;

	public static float TOTAL_FLASH_TIME => 0.6f;

	public static float FLASH_FADE_IN_TIME => 0.02f;

	public static float FLASH_FADE_OUT_TIME => 0.4f;

	public static float FLASH_START_SCALE_X => 150f;

	public static float FLASH_FULL_SCALE_X => 200f;

	public static float FLASH_START_SCALE_Y => 100f;

	public static float FLASH_FULL_SCALE_Y => 300f;

	public void Destroy()
	{
		m_update = false;
	}

	public virtual void Init(byte[] tpl_data, out Vector3 size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		size = default(Vector3);
	}

	public virtual void Update(float dt)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		m_time += dt;
		float num = m_time / TOTAL_FLASH_TIME;
		m_cur_scale = new Vector3(FLASH_START_SCALE_X + (FLASH_FULL_SCALE_X - FLASH_START_SCALE_X) * num * num, FLASH_START_SCALE_Y + (FLASH_FULL_SCALE_Y - FLASH_START_SCALE_Y) * num * num, 0f);
		if (m_time < FLASH_FADE_IN_TIME)
		{
			m_colour.A = (byte)Mortar.Math.CLAMP((float)(int)m_startColor.A * (m_time / FLASH_FADE_IN_TIME), 0f, (int)m_startColor.A);
		}
		else if (m_time > TOTAL_FLASH_TIME - FLASH_FADE_OUT_TIME)
		{
			float num2 = (TOTAL_FLASH_TIME - m_time) / FLASH_FADE_OUT_TIME;
			num2 *= num2;
			m_colour.A = (byte)Mortar.Math.CLAMP((float)(int)m_startColor.A * num2, 0f, (int)m_startColor.A);
		}
		else
		{
			m_colour.A = m_startColor.A;
		}
		if (m_time > TOTAL_FLASH_TIME)
		{
			m_update = false;
			m_time = 0f;
		}
	}

	public virtual void Draw()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			MatrixManager.instance.SetMatrix(Matrix.Identity);
			MatrixManager.instance.Scale(m_cur_scale);
			MatrixManager.instance.RotZ(Mortar.Math.IDX_TO_RADIANS(m_dir_angle));
			MatrixManager.instance.TranslateGlobal(m_pos);
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			m_texture.Set();
			Mesh.DrawQuad(m_colour);
		}
	}

	public virtual void DrawUpdate(float dt)
	{
	}

	public void MakeFlash(Color colour, Vector3 pos, Vector3 vel, Texture texture)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		m_texture = texture;
		m_pos = pos;
		m_pos.Z = -5400f;
		m_vel = vel;
		m_colour = colour;
		m_startColor = colour;
		m_pos += vel * 7.5f;
		m_pos.X = Game.SCREEN_WIDTH * (float)Mortar.Math.MATH_SIGN(m_pos.X) / 2f;
		m_cur_scale = new Vector3(128f, 128f, 128f);
		m_dir_angle = Mortar.Math.Atan2Idx(m_vel.X, 0f - m_vel.Y);
		m_time = 0f;
		m_update = true;
		m_sinA = Mortar.Math.SinIdx(m_dir_angle);
		m_cosA = Mortar.Math.CosIdx(m_dir_angle);
		Update(0f);
	}

	public static void CreatePool()
	{
		CreatePool(512);
	}

	public static void CreatePool(int num)
	{
	}

	public static BombFlash GetFree()
	{
		if (pool == null)
		{
			return null;
		}
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

	public static void UpdateActiveFlashes(float dt)
	{
	}

	public static void DrawActiveFlashes()
	{
	}

	public static void RemoveAllFlashes()
	{
		for (int i = 0; i < poolCount; i++)
		{
			pool[i].Destroy();
		}
	}

	public static int NumActiveFlashes()
	{
		throw new MissingMethodException();
	}
}
