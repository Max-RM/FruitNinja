using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class FruitRay : Entity
{
	public float m_alpha;

	public Vector3 m_startScale;

	public Vector3 m_maxScale;

	public bool m_fadeOut;

	private Matrix m_oreintation;

	private Matrix m_oreintationOffset;

	private Fruit m_fruit;

	private float m_time;

	public static Texture RayTexture = null;

	private static GameVertex[] verts = new GameVertex[3];

	public void Init(Fruit fruit, Quaternion offset)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		base.m_destroy = false;
		base.m_dormant = false;
		m_fadeOut = false;
		m_fruit = fruit;
		m_maxScale = Vector3.One * Utils.GetRandBetween(70f, 120f);
		m_startScale = Vector3.One * 40f;
		m_cur_scale = m_startScale;
		m_time = 0f;
		m_alpha = 1f;
		m_pos = m_fruit.m_pos;
		offset.Normalize();
		m_oreintationOffset = Matrix.CreateFromQuaternion(offset);
		m_oreintation = Matrix.CreateFromQuaternion(m_fruit.m_rotation_piece[0]);
	}

	public override void Draw()
	{
	}

	public override void DrawUpdate(float dt)
	{
	}

	public override void Update(float dt)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (m_fadeOut)
		{
			m_alpha -= dt * 1.6f;
			m_fruit = null;
			if (m_alpha <= 0f)
			{
				base.m_destroy = true;
			}
		}
		else
		{
			m_time += Game.game_work.dt;
			m_pos = m_fruit.m_pos + Vector3.UnitZ * m_fruit.m_z;
			m_oreintation = Matrix.CreateFromQuaternion(m_fruit.m_rotation_piece[0]);
			m_cur_scale = TransitionFunctions.LerpF(m_startScale, m_maxScale, TransitionFunctions.GetProgressBetween(m_time, 0f, 0.15f, clamp: true));
		}
	}

	private void DrawRay()
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			verts[i].X = (verts[i].Y = (verts[i].Z = (verts[i].nx = (verts[i].ny = 0f))));
			verts[i].nz = 1f;
			verts[i].v = ((i == 0) ? 1f : 0.05f);
			verts[i].color = new Color(255f, 255f, 255f, 255f * m_alpha);
		}
		verts[0].u = 0.5f;
		verts[1].u = 0f;
		verts[2].u = 1f;
		verts[1].X = -0.25f;
		verts[2].X = 0.25f;
		verts[1].Z = (verts[2].Z = 1f);
		Matrix val = Matrix.CreateScale(m_cur_scale * (3f - m_alpha * 2f));
		val = val * m_oreintationOffset * m_oreintation;
		val *= Matrix.CreateTranslation(m_pos);
		if (RayTexture != null)
		{
			RayTexture.Set();
		}
		MatrixManager.GetInstance().SetMatrix(val);
		MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
		Mesh.DrawTriStrip(verts, 3, hasAlpha: true);
	}

	public static void DrawRays()
	{
		LinkedListNode<Entity> iterator = null;
		for (FruitRay fruitRay = (FruitRay)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_FRUIT_RAY, ref iterator); fruitRay != null; fruitRay = (FruitRay)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_FRUIT_RAY, ref iterator))
		{
			fruitRay.DrawRay();
		}
	}
}
