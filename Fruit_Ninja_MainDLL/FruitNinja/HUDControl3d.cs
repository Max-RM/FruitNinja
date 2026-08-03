using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class HUDControl3d : HUDControl
{
	public Texture m_texture;

	public override void Save()
	{
	}

	public override void Release()
	{
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (m_color.A > 0 && m_texture != null)
		{
			m_texture.Set();
			MatrixManager.GetInstance().Reset();
			Matrix val = Matrix.CreateScale(m_scale);
			if (m_rotation != 0f)
			{
				val *= Matrix.CreateRotationZ(MathHelper.ToRadians(m_rotation));
			}
			val *= Matrix.CreateTranslation(m_pos);
			MatrixManager.GetInstance().SetMatrix(val);
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			Mesh.DrawQuad(HUDControl.TintColor(m_color, tintChannels), m_uvs[0].X, m_uvs[1].X, m_uvs[0].Y, m_uvs[1].Y);
		}
	}

	public override void Update(float dt)
	{
		base.Update(dt);
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_3D;
	}

	public override void Skip()
	{
	}
}
