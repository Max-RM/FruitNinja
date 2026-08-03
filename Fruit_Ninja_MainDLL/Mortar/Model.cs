using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class Model
{
	public struct Submesh
	{
		public VertexPositionColorTexture[] vertecies;

		public Texture tex;
	}

	public Submesh[] meshes;

	public Matrix amatrix = Matrix.Identity;

	private static BasicEffect basicEffect;

	public void Draw(Matrix? mtx)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (basicEffect == null)
		{
			basicEffect = new BasicEffect(((Game)TheGame.instance).GraphicsDevice);
			basicEffect.VertexColorEnabled = true;
		}
		DisplayManager.instance.SetRasterizeStateCullCwise();
		bool flag = false;
		basicEffect.Projection = DisplayManager.instance.currentProjMtx;
		Matrix val = Matrix.Identity;
		if (mtx.HasValue)
		{
			val = mtx.Value;
		}
		basicEffect.World = amatrix * val;
		basicEffect.View = DisplayManager.instance.currentViewMtx;
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i].tex == null)
			{
				basicEffect.TextureEnabled = false;
			}
			else
			{
				basicEffect.TextureEnabled = true;
				basicEffect.Texture = meshes[i].tex.intex;
				flag = meshes[i].tex.hasAlpha;
			}
			((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
			if (flag)
			{
				DisplayManager.instance.SetBlendStateDefault();
			}
			else
			{
				DisplayManager.instance.SetBlendStateOff();
			}
			((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>((PrimitiveType)0, meshes[i].vertecies, 0, meshes[i].vertecies.Length / 3);
		}
	}
}
