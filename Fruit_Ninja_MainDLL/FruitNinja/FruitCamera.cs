using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class FruitCamera : MortarCamera
{
	public enum CameraState
	{
		CAMERA_STATE_IDLE,
		CAMERA_STATE_FOLLOW,
		CAMERA_TRANSITION_IN,
		CAMERA_TRANSITION_OUT
	}

	public enum PERSPECIVE_TYPE
	{
		ORIENTATION_NORMAL,
		ORIENTATION_BACKGROUND,
		ORIENTATION_LEFT,
		ORIENTATION_RIGHT,
		ORIENTATION_NORMAL_NO_SHAKE
	}

	public delegate void TransitionFinished();

	private const float CAMERA_SHAKE_RADIUS = 9f;

	private const float CAMERA_SHAKE_SPEED = 0.2f;

	protected Entity m_target;

	protected CameraState m_state;

	protected ushort m_rot_z;

	protected ushort m_rot_x;

	protected Vector2 m_cameraShakeDest;

	protected ushort m_cameraShakeDirection;

	public Vector2 m_cameraShake;

	private float m_cameraShakeRadius;

	public float m_cameraShakeTime;

	public float m_cameraShakeMaxTime;

	public Vector3 Eye;

	public Vector3 LookAt;

	public Vector3 m_pos;

	public float m_zoom;

	public ushort m_rotation;

	public float m_transitionAmount;

	public Vector3 m_transitionToPos;

	public float m_transitionToZoom;

	public float m_transitionToRot;

	private float m_degree;

	public TransitionFinished m_transitionFinished;

	protected void UpdateIdle(float dt)
	{
	}

	protected void UpdateFollow(float dt)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (m_target != null)
		{
			Vector3 val = m_look_at - m_target.m_pos;
			m_look_at = m_target.m_pos;
			m_eye -= val;
		}
		else
		{
			IdleCamera();
		}
	}

	public FruitCamera()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		m_target = null;
		m_state = CameraState.CAMERA_STATE_IDLE;
		m_rot_z = 0;
		m_rot_x = 0;
		m_cameraShake = Vector2.Zero;
		m_cameraShakeDest = Vector2.Zero;
		m_cameraShakeTime = 0f;
		m_transitionAmount = 0f;
		m_transitionToPos = Vector3.Zero;
		m_transitionToZoom = 1f;
		m_zoom = 1f;
		m_rotation = 0;
		m_cameraShakeRadius = 0f;
	}

	public override void UpdateCamera(float dt)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		UpdateShake(dt);
		m_pos = Vector3.Zero;
		switch (m_state)
		{
		case CameraState.CAMERA_STATE_IDLE:
			UpdateIdle(dt);
			break;
		case CameraState.CAMERA_STATE_FOLLOW:
			UpdateFollow(dt);
			break;
		case CameraState.CAMERA_TRANSITION_OUT:
			if (!(m_transitionAmount > 0f))
			{
				break;
			}
			m_transitionAmount -= dt * 10f;
			if (m_transitionAmount <= 0f)
			{
				m_transitionAmount = 0f;
				if (m_transitionFinished != null)
				{
					m_transitionFinished();
				}
			}
			break;
		case CameraState.CAMERA_TRANSITION_IN:
			if (!(m_transitionAmount < 1f))
			{
				break;
			}
			m_transitionAmount += dt * 3f;
			if (m_transitionAmount >= 1f)
			{
				m_transitionAmount = 1f;
				if (m_transitionFinished != null)
				{
					m_transitionFinished();
				}
			}
			break;
		}
		m_changed = true;
		m_zoom = TransitionFunctions.LerpF(1f, m_transitionToZoom, TransitionFunctions.InverseSquareTransition(m_transitionAmount));
		m_pos = TransitionFunctions.LerpF(Vector3.Zero, m_transitionToPos, TransitionFunctions.SinTransition(m_transitionAmount, 90f));
		m_degree = TransitionFunctions.LerpF(0f, m_transitionToRot, TransitionFunctions.InverseSquareTransition(m_transitionAmount));
		m_rotation = Math.DEGREE_TO_IDX(m_degree);
		m_pos += new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f);
	}

	public Vector3 TranslatePos(Vector3 pos, bool screenToWorld, bool includeShake)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (m_transitionAmount <= 0f)
		{
			return pos;
		}
		if (screenToWorld)
		{
			Matrix val = Matrix.CreateRotationZ(MathHelper.ToRadians(m_degree));
			Vector3 val2 = pos;
			Vector3.Transform(ref val2, ref val, out pos);
			pos *= m_zoom;
			pos += m_pos - (Vector3)(includeShake ? Vector3.Zero : new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f));
		}
		else
		{
			pos -= m_pos - (Vector3)(includeShake ? Vector3.Zero : new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f));
			pos /= m_zoom;
			Matrix val3 = Matrix.CreateRotationZ(MathHelper.ToRadians(m_degree));
			Vector3 val4 = pos;
			Vector3.Transform(ref val4, ref val3, out pos);
		}
		return pos;
	}

	public void UpdateShake(float dt)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (m_cameraShakeTime > 0f || m_cameraShakeRadius > 0f)
		{
			m_cameraShakeTime = Math.MAX(0f, m_cameraShakeTime - dt);
			Vector2 val = m_cameraShakeDest - m_cameraShake;
			if (val.LengthSquared() < 16f)
			{
				m_cameraShakeDirection += (ushort)(Math.DEGREE_TO_IDX(140f) + Math.g_random.Rand32(Math.DEGREE_TO_IDX(80f)));
				float num = 9f * (m_cameraShakeTime / m_cameraShakeMaxTime) + m_cameraShakeRadius;
				m_cameraShakeDest.X = Math.CosIdx(m_cameraShakeDirection) * num;
				m_cameraShakeDest.Y = Math.SinIdx(m_cameraShakeDirection) * num;
			}
			m_cameraShake += (m_cameraShakeDest - m_cameraShake) * 0.2f * (1f + m_cameraShakeTime / m_cameraShakeMaxTime);
			m_changed = true;
		}
		else
		{
			m_cameraShakeDest = Vector2.Zero;
			if (Math.ABS(m_cameraShake.X) > 0.01f)
			{
				ref Vector2 cameraShake = ref m_cameraShake;
				cameraShake.X *= 0.8f;
			}
			else
			{
				m_cameraShake.X = 0f;
			}
			if (Math.ABS(m_cameraShake.Y) > 0.01f)
			{
				ref Vector2 cameraShake2 = ref m_cameraShake;
				cameraShake2.Y *= 0.8f;
			}
			else
			{
				m_cameraShake.Y = 0f;
			}
		}
	}

	public void CreateCameraShake(Vector3 origin, float length)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CreateCameraShake(origin, length, 1f);
	}

	public void CreateCameraShake(Vector3 origin, float length, float strength)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		m_cameraShakeDirection = Math.Atan2Idx(origin.Y, origin.X);
		m_cameraShakeDest.X = Math.CosIdx(m_cameraShakeDirection) * 9f;
		m_cameraShakeDest.Y = Math.SinIdx(m_cameraShakeDirection) * 9f;
		m_cameraShakeDest *= strength;
		m_cameraShakeMaxTime = (m_cameraShakeTime = length);
	}

	public Entity GetFollowEntity()
	{
		Entity result = null;
		if (m_state == CameraState.CAMERA_STATE_FOLLOW)
		{
			result = m_target;
		}
		return result;
	}

	public void FollowEntity(Entity ent)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (ent != null)
		{
			m_state = CameraState.CAMERA_STATE_FOLLOW;
			m_target = ent;
		}
		m_rot_x = 0;
		m_rot_z = 0;
		m_up = new Vector3(0f, 1f, 0f);
	}

	public void IdleCamera()
	{
		m_state = CameraState.CAMERA_STATE_IDLE;
		m_target = null;
	}

	public void SetupPerspective(PERSPECIVE_TYPE orientation, bool changed)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		float num = m_zoom;
		Vector3 val = m_pos;
		ushort idx = m_rotation;
		if (orientation == PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE)
		{
			num = 1f;
			val = Vector3.Zero;
			idx = 0;
		}
		if (m_changed || changed)
		{
			float num2 = 240f * num;
			float num3 = 400f * num;
			Vector3 camUp = default(Vector3);
			camUp = new Vector3(Math.SinIdx(idx), Math.CosIdx(idx), 0f);
			_ = Vector3.Zero;
			switch (orientation)
			{
			case PERSPECIVE_TYPE.ORIENTATION_NORMAL:
			case PERSPECIVE_TYPE.ORIENTATION_BACKGROUND:
			case PERSPECIVE_TYPE.ORIENTATION_NORMAL_NO_SHAKE:
				MatrixManager.GetInstance().SetupLookAt(new Vector3(0f, 0f, 1f) + val, camUp, val);
				m_view_mtx = MatrixManager.GetInstance().GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
				MatrixManager.GetInstance().SetupOrtho(num2, 0f - num2, 0f - num3, num3, -6000f, 6000f);
				m_proj_mtx = MatrixManager.GetInstance().GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
				break;
			case PERSPECIVE_TYPE.ORIENTATION_LEFT:
				MatrixManager.instance.SetupLookAt(new Vector3(0f, 0f, 1f) + new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f), new Vector3(1f, 0f, 0f), new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f));
				m_view_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
				MatrixManager.instance.SetupOrtho(-(DisplayManager.GetInstance().Res.X >> 1), DisplayManager.GetInstance().Res.X >> 1, DisplayManager.GetInstance().Res.Y >> 1, (float)(-(DisplayManager.GetInstance().Res.Y >> 1)) - (float)DisplayManager.GetInstance().Res.Y, -6000f, 6000f);
				break;
			case PERSPECIVE_TYPE.ORIENTATION_RIGHT:
				MatrixManager.instance.SetupLookAt(new Vector3(0f, 0f, 1f) + new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f), new Vector3(1f, 0f, 0f), new Vector3(m_cameraShake.X, m_cameraShake.Y, 0f));
				m_view_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
				MatrixManager.instance.SetupOrtho(DisplayManager.GetInstance().Res.X >> 1, -(DisplayManager.GetInstance().Res.X >> 1), (float)(-(DisplayManager.GetInstance().Res.Y >> 1)) - (float)DisplayManager.GetInstance().Res.Y, DisplayManager.GetInstance().Res.Y >> 1, -6000f, 6000f);
				m_proj_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
				break;
			}
		}
		else
		{
			MatrixManager.instance.SetMatrix(m_view_mtx, MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
			MatrixManager.instance.SetMatrix(m_proj_mtx, MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
		}
		MatrixManager.instance.Reset(MatrixManager.MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void SetupPerspective(PERSPECIVE_TYPE orientation)
	{
		SetupPerspective(orientation, changed: false);
	}

	public new void SetupPerspective()
	{
		SetupPerspective(PERSPECIVE_TYPE.ORIENTATION_NORMAL, changed: false);
	}

	public void TransitionOut()
	{
		m_state = CameraState.CAMERA_TRANSITION_OUT;
	}

	public bool IsTransitionIn()
	{
		return m_state == CameraState.CAMERA_TRANSITION_IN;
	}

	public void Transition(Vector3 pos, float zoom)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Transition(pos, zoom, 0f, null);
	}

	public void Transition(Vector3 pos, float zoom, float rot, TransitionFinished del)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_state = CameraState.CAMERA_TRANSITION_IN;
		m_transitionToPos = pos;
		m_transitionToZoom = zoom;
		m_transitionToRot = rot;
		m_transitionFinished = del;
	}

	public bool ViewIsNormal()
	{
		if (m_cameraShake.X == 0f && m_cameraShake.Y == 0f)
		{
			return m_transitionAmount <= 0f;
		}
		return false;
	}
}
