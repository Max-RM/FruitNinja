using Microsoft.Xna.Framework;

namespace Mortar;

public class MortarCamera
{
	protected Matrix m_view_mtx;

	protected Matrix m_proj_mtx;

	protected Matrix m_ortho_view_mtx;

	protected Matrix m_ortho_mtx;

	protected Vector3 m_eye;

	protected Vector3 m_look_at;

	protected Vector3 m_up;

	protected bool m_changed;

	protected bool m_recalc_ortho;

	protected MortarRectangle m_oldwnd;

	protected float m_fov_x;

	protected float m_fov_y;

	protected float m_nearClip;

	protected float m_farClip;

	public MortarCamera()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		m_changed = true;
		m_look_at = Vector3.Zero;
		m_eye = new Vector3(0f, 0f, 1f);
		m_up = new Vector3(0f, 1f, 0f);
		m_fov_x = 0f;
		m_fov_y = 0f;
		m_nearClip = 1f;
		m_farClip = 1000f;
		m_view_mtx = Matrix.Identity;
		m_proj_mtx = Matrix.Identity;
		m_ortho_view_mtx = Matrix.Identity;
		m_ortho_mtx = Matrix.Identity;
		m_recalc_ortho = true;
		m_oldwnd = default(MortarRectangle);
	}

	public virtual void Init(float snear, float sfar, float xFOV, float yFOV)
	{
		m_nearClip = snear;
		m_farClip = sfar;
		m_fov_x = xFOV;
		m_fov_y = yFOV;
		m_recalc_ortho = true;
	}

	public virtual void UpdateCamera(float dt)
	{
	}

	public virtual void SetupPerspective()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (m_changed)
		{
			MatrixManager.instance.SetupLookAt(m_eye, m_up, m_look_at);
			m_view_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
			MatrixManager.instance.SetupPerspective(m_fov_y, m_fov_x / m_fov_y, m_nearClip, m_farClip);
			m_proj_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
		}
		else
		{
			MatrixManager.instance.SetMatrix(m_view_mtx, MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
			MatrixManager.instance.SetMatrix(m_proj_mtx, MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
		}
		MatrixManager.instance.Reset(MatrixManager.MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public virtual void SetupOrtho()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		MortarRectangle windowSize = DisplayManager.GetInstance().GetWindowSize();
		if (m_recalc_ortho || (windowSize.Height() != m_oldwnd.Height() && windowSize.Width() != m_oldwnd.Width()))
		{
			MatrixManager.instance.SetupLookAt(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), Vector3.Zero);
			m_ortho_view_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
			MatrixManager.instance.SetupOrtho(windowSize.bottom >> 1, -(windowSize.bottom >> 1), -(windowSize.right >> 1), windowSize.right >> 1, -1f, 1000f);
			m_ortho_mtx = MatrixManager.instance.GetMatrix(MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
		}
		else
		{
			MatrixManager.instance.SetMatrix(m_ortho_view_mtx, MatrixManager.MatrixStackTypes.MATRIXSTACK_VIEW);
			MatrixManager.instance.SetMatrix(m_ortho_mtx, MatrixManager.MatrixStackTypes.MATRIXSTACK_PROJECTION);
		}
		m_oldwnd = windowSize;
		MatrixManager.instance.Reset(MatrixManager.MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public virtual float GetAspectRatio()
	{
		return m_fov_x / m_fov_y;
	}

	public virtual float GetFOVx()
	{
		return m_fov_x;
	}

	public virtual float GetFOVy()
	{
		return m_fov_y;
	}

	public virtual void SetLookAt(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_look_at = pos;
		m_changed = true;
	}

	public virtual Vector3 GetLookAt()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_look_at;
	}

	public virtual void SetPos(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_eye = pos;
		m_changed = true;
	}

	public virtual Vector3 GetPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_eye;
	}

	public virtual void SetUp(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_up = pos;
		m_changed = true;
	}

	public virtual Vector3 GetUp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_up;
	}
}
