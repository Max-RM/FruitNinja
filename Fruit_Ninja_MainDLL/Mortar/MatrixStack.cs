using Microsoft.Xna.Framework;

namespace Mortar;

public class MatrixStack
{
	protected Matrix[] m_mtxStack;

	protected Matrix m_currentMtx;

	private byte m_currentStackIndex;

	public uint version;

	public MatrixStack()
	{
		m_mtxStack = ArrayInit.CreateFilledArray<Matrix>(64);
		Reset();
		version = 1u;
	}

	public void Push()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ref Matrix reference = ref m_mtxStack[m_currentStackIndex];
		reference = m_currentMtx;
		m_currentStackIndex++;
	}

	public void Pop(int num)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		m_currentStackIndex = (byte)(m_currentStackIndex - num);
		m_currentMtx = m_mtxStack[m_currentStackIndex];
		version++;
	}

	public void Store(int num)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ref Matrix reference = ref m_mtxStack[num];
		reference = m_currentMtx;
	}

	public void Restore(int num)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx = m_mtxStack[num];
		version++;
	}

	public void Reset()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		m_currentStackIndex = 0;
		ref Matrix reference = ref m_mtxStack[m_currentStackIndex];
		reference = Matrix.Identity;
		m_currentMtx = Matrix.Identity;
		version++;
	}

	public Matrix GetCurrentMatrix()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_currentMtx;
	}

	public void SetCurrentMatrix(Matrix mtx)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx = mtx;
		version++;
	}

	public void Translate(Vector3 amount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx *= Matrix.CreateTranslation(amount);
		version++;
	}

	public void Translate2D(Vector2 amount)
	{
		ref Matrix currentMtx = ref m_currentMtx;
		currentMtx.M31 += amount.X;
		ref Matrix currentMtx2 = ref m_currentMtx;
		currentMtx2.M32 += amount.Y;
		version++;
	}

	public void TranslateLocal(Vector3 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx = Matrix.CreateTranslation(amount) * m_currentMtx;
		version++;
	}

	public void Scale(Vector3 amount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx *= Matrix.CreateScale(amount);
		version++;
	}

	public void RotX(float amount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx *= Matrix.CreateRotationX(MathHelper.ToRadians(amount));
		version++;
	}

	public void RotY(float amount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx *= Matrix.CreateRotationY(MathHelper.ToRadians(amount));
		version++;
	}

	public void RotZ(float amount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx *= Matrix.CreateRotationZ(MathHelper.ToRadians(amount));
		version++;
	}

	public void MtxMul(Matrix mul)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_currentMtx *= mul;
		version++;
	}
}
