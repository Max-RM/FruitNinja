using Microsoft.Xna.Framework;

namespace Mortar;

public class MatrixManager
{
	public enum MatrixStackTypes
	{
		MATRIXSTACK_PROJECTION,
		MATRIXSTACK_VIEW,
		MATRIXSTACK_WORLD,
		MATRIXSTACK_TEXTURE,
		MATRIXSTACK_MAX
	}

	public static MatrixManager instance = new MatrixManager();

	protected MatrixStack[] m_stacks = ArrayInit.CreateFilledArray<MatrixStack>(4);

	protected uint[] m_stack_versions = new uint[4];

	private MatrixManager()
	{
		m_stacks = ArrayInit.CreateFilledArray<MatrixStack>(4);
		m_stack_versions = new uint[4];
		for (int i = 0; i < 4; i++)
		{
			m_stacks[i] = new MatrixStack();
		}
	}

	private void _UploadCurrentMatrices(bool quick)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!quick)
		{
			DisplayManager.instance.currentViewMtx = m_stacks[1].GetCurrentMatrix();
			DisplayManager.instance.currentProjMtx = m_stacks[0].GetCurrentMatrix() * DisplayManager.instance.topMatrix;
		}
		DisplayManager.instance.currentWorldMtx = m_stacks[2].GetCurrentMatrix();
		DisplayManager.instance.currentTextureMtx = m_stacks[3].GetCurrentMatrix();
	}

	public static MatrixManager GetInstance()
	{
		return instance;
	}

	public void Init()
	{
		ResetAllStacks();
	}

	public void ResetAllStacks()
	{
		for (int i = 0; i < 4; i++)
		{
			m_stacks[i].Reset();
		}
	}

	public void UploadCurrentMatrices()
	{
		UploadCurrentMatrices(quick: true);
	}

	public void UploadCurrentMatrices(bool quick)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!quick)
		{
			DisplayManager.instance.currentViewMtx = m_stacks[1].GetCurrentMatrix();
			DisplayManager.instance.currentProjMtx = m_stacks[0].GetCurrentMatrix() * DisplayManager.instance.topMatrix;
		}
		DisplayManager.instance.currentWorldMtx = m_stacks[2].GetCurrentMatrix();
		DisplayManager.instance.currentTextureMtx = m_stacks[3].GetCurrentMatrix();
	}

	public void Push()
	{
		m_stacks[2].Push();
	}

	public void Pop()
	{
		m_stacks[2].Pop(1);
	}

	public void Store(byte num)
	{
		m_stacks[2].Store(num);
	}

	public void Restore(byte num)
	{
		m_stacks[2].Restore(num);
	}

	public void Reset()
	{
		m_stacks[2].Reset();
	}

	public Matrix GetMatrix()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return m_stacks[2].GetCurrentMatrix();
	}

	public void SetMatrix(Matrix mtx)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[2].SetCurrentMatrix(mtx);
	}

	public void Translate(Vector3 amount)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[2].Translate(amount);
	}

	public void TranslateGlobal(Vector3 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Translate(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void TranslateLocal(Vector3 amount)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[2].TranslateLocal(amount);
	}

	public void Scale(Vector3 amount)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[2].Scale(amount);
	}

	public void RotX(float amount)
	{
		m_stacks[2].RotX(amount);
	}

	public void RotY(float amount)
	{
		m_stacks[2].RotY(amount);
	}

	public void RotZ(float amount)
	{
		m_stacks[2].RotZ(amount);
	}

	public void Translate2D(Vector2 amount)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[2].Translate2D(amount);
	}

	public void Push(MatrixStackTypes type)
	{
		m_stacks[(int)type].Push();
	}

	public void Pop(int num, MatrixStackTypes type)
	{
		m_stacks[(int)type].Pop(num);
	}

	public void Store(int num, MatrixStackTypes type)
	{
		m_stacks[(int)type].Store(num);
	}

	public void Restore(int num, MatrixStackTypes type)
	{
		m_stacks[(int)type].Restore(num);
	}

	public void Reset(MatrixStackTypes type)
	{
		m_stacks[(int)type].Reset();
	}

	public Matrix GetMatrix(MatrixStackTypes type)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return m_stacks[(int)type].GetCurrentMatrix();
	}

	public void SetMatrix(Matrix mtx, MatrixStackTypes type)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[(int)type].SetCurrentMatrix(mtx);
	}

	public void Translate(Vector3 amount, MatrixStackTypes type)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[(int)type].Translate(amount);
	}

	public void TranslateGlobal(Vector3 amount, MatrixStackTypes type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Translate(amount, type);
	}

	public void TranslateLocal(Vector3 amount, MatrixStackTypes type)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[(int)type].TranslateLocal(amount);
	}

	public void Scale(Vector3 amount, MatrixStackTypes type)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[(int)type].Scale(amount);
	}

	public void RotX(float amount, MatrixStackTypes type)
	{
		m_stacks[(int)type].RotX(amount);
	}

	public void RotY(float amount, MatrixStackTypes type)
	{
		m_stacks[(int)type].RotY(amount);
	}

	public void RotZ(float amount, MatrixStackTypes type)
	{
		m_stacks[(int)type].RotZ(amount);
	}

	public void Translate2D(Vector2 amount, MatrixStackTypes type)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_stacks[(int)type].Translate2D(amount);
	}

	public void SetupPerspective(float fovy, float aspect, float n, float f)
	{
		SetupPerspective(fovy, aspect, n, f, out var _);
	}

	public void SetupPerspective(float fovy, float aspect, float n, float f, out Matrix mtx)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fovy), aspect, n, f, out mtx);
		m_stacks[0].SetCurrentMatrix(mtx);
		UploadCurrentMatrices(quick: false);
	}

	public void SetupOrtho(float t, float b, float l, float r, float n, float f)
	{
		SetupOrtho(t, b, l, r, n, f, out var _);
		UploadCurrentMatrices(quick: false);
	}

	public void SetupOrtho(float t, float b, float l, float r, float n, float f, out Matrix mtx)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreateOrthographicOffCenter(l, r, b, t, n, f, out mtx);
		m_stacks[0].SetCurrentMatrix(mtx);
	}

	public void SetupLookAt(Vector3 camPos, Vector3 camUp, Vector3 target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		SetupLookAt(camPos, camUp, target, out var _);
	}

	public void SetupLookAt(Vector3 camPos, Vector3 camUp, Vector3 target, out Matrix mtx)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreateLookAt(ref camPos, ref target, ref camUp, out mtx);
		m_stacks[1].SetCurrentMatrix(mtx);
	}

	public void WorldTranslate(Vector3 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Translate(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void WorldTranslateGlobal(Vector3 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TranslateGlobal(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void WorldScale(Vector3 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Scale(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void WorldRotX(float amount)
	{
		RotX(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void WorldRotY(float amount)
	{
		RotY(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}

	public void WorldRotZ(float amount)
	{
		RotZ(amount, MatrixStackTypes.MATRIXSTACK_WORLD);
	}
}
