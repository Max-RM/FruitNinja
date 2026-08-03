using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class BombBlast : Entity
{
	protected Vector3 m_xVec;

	protected Vector3 m_yVec;

	protected Vector3 m_origXVec;

	protected Vector3 m_origYVec;

	public float m_time;

	public static GameVertex[] m_points = ArrayInit.CreateFilledArray<GameVertex>(3072);

	public static int m_curr_drawing_blast;

	public override void Init(byte[] tpl_data, int tpl_size, Vector3? size)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		m_time = 0f;
		m_dir_angle = Math.DEGREE_TO_IDX(Math.g_random.RandF(360f));
		m_origXVec = new Vector3(Math.CosIdx(m_dir_angle), Math.SinIdx(m_dir_angle), 0f) * 0.5f;
		m_origYVec = new Vector3(Math.CosIdx((ushort)(m_dir_angle + Math.DEGREE_TO_IDX(90f))), Math.SinIdx((ushort)(m_dir_angle + Math.DEGREE_TO_IDX(90f))), 0f);
		m_xVec = m_origXVec;
		m_yVec = m_origYVec;
		m_cur_scale = new Vector3(5f, 50f, 1f);
		base.m_destroy = false;
		base.m_dormant = false;
	}

	public override void Update(float dt)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		m_time += Game.game_work.dt;
		Vector3 origXVec = m_origXVec;
		ref Vector3 cur_scale = ref m_cur_scale;
		m_xVec = origXVec * (cur_scale.X += 100f * Game.game_work.dt);
		Vector3 origYVec = m_origYVec;
		ref Vector3 cur_scale2 = ref m_cur_scale;
		m_yVec = origYVec * (cur_scale2.Y += 2500f * Game.game_work.dt);
		if (m_time > 3f)
		{
			base.m_destroy = true;
			base.m_destroy = true;
		}
	}

	public override void Draw()
	{
	}

	public override void DrawUpdate(float dt)
	{
	}

	public virtual void DrawBlast()
	{
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		int num = m_curr_drawing_blast * 6;
		float u = 0f;
		float u2 = 1f;
		float v = 0f;
		float v2 = 1f;
		m_points[num].X = m_pos.X + (m_xVec.X + m_yVec.X);
		m_points[num].Y = m_pos.Y + (m_xVec.Y + m_yVec.Y);
		m_points[num].u = u2;
		m_points[num].v = v;
		m_points[num + 1].X = m_pos.X + (0f - m_xVec.X + m_yVec.X);
		m_points[num + 1].Y = m_pos.Y + (0f - m_xVec.Y + m_yVec.Y);
		m_points[num + 1].u = u;
		m_points[num + 1].v = v;
		m_points[num + 2].X = m_pos.X + m_xVec.X * 0.25f;
		m_points[num + 2].Y = m_pos.Y + m_xVec.Y * 0.25f;
		m_points[num + 2].u = u2;
		m_points[num + 2].v = v2;
		ref GameVertex reference = ref m_points[num + 3];
		reference = m_points[num + 2];
		ref GameVertex reference2 = ref m_points[num + 4];
		reference2 = m_points[num + 1];
		m_points[num + 5].X = m_pos.X + (0f - m_xVec.X) * 0.25f;
		m_points[num + 5].Y = m_pos.Y + (0f - m_xVec.Y) * 0.25f;
		m_points[num + 5].u = u;
		m_points[num + 5].v = v2;
		Color white = Color.White;
		for (int i = 0; i < 6; i++)
		{
			m_points[num + i].Z = 0f;
			m_points[num + i].nx = 0f;
			m_points[num + i].ny = 0f;
			m_points[num + i].nz = 1f;
			m_points[num + i].color = white;
		}
	}

	public static void DrawActiveBlasts()
	{
		if (Bomb.m_blastTexture != null)
		{
			m_curr_drawing_blast = 0;
			LinkedListNode<Entity> iterator = null;
			for (BombBlast bombBlast = (BombBlast)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB_BLAST, ref iterator); bombBlast != null; bombBlast = (BombBlast)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB_BLAST, ref iterator))
			{
				bombBlast.DrawBlast();
				m_curr_drawing_blast++;
			}
			if (m_curr_drawing_blast > 0)
			{
				Bomb.m_blastTexture.Set();
				MatrixManager.instance.Reset();
				MatrixManager.instance.UploadCurrentMatrices(quick: true);
				Mesh.DrawTriList(m_points, m_curr_drawing_blast * 6);
			}
		}
	}

	public static void CleanupBomb()
	{
		for (int i = 0; i < Game.MAX_PLAYERS * 2; i++)
		{
			Bomb.m_bombModel[i] = null;
		}
		BombFlash.CleanUp();
		Bomb.s_flashTexture[0] = null;
		Bomb.s_flashTexture[1] = null;
		Bomb.m_blastTexture = null;
		Bomb.s_minus_10 = null;
	}
}
