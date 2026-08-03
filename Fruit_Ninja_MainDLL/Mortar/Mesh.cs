using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class Mesh
{
	private static Matrix view;

	private static Matrix proj;

	public static bool newframe;

	private static BasicEffect basicEffect = null;

	private static VertexPositionColorTexture[] qverts = new VertexPositionColorTexture[4];

	public static VertexPositionColorTexture[] verts = new VertexPositionColorTexture[10000];

	public static int vertsCurrentOffset = 0;

	public static void DrawQuad(Color col)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		DrawQuad(col, 0f, 1f, 0f, 1f);
	}

	public static void DrawQuad(Color col, float u0, float u1, float v0, float v1)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		if (basicEffect == null)
		{
			basicEffect = new BasicEffect(((Game)TheGame.instance).GraphicsDevice);
			basicEffect.VertexColorEnabled = true;
			qverts[0].Position = new Vector3(-0.5f, 0.5f, 0f);
			qverts[1].Position = new Vector3(-0.5f, -0.5f, 0f);
			qverts[2].Position = new Vector3(0.5f, 0.5f, 0f);
			qverts[3].Position = new Vector3(0.5f, -0.5f, 0f);
		}
		DisplayManager.instance.SetRasterizeStateCullOff();
		if (newframe)
		{
			view = DisplayManager.instance.currentViewMtx;
			proj = DisplayManager.instance.currentProjMtx;
			newframe = false;
		}
		bool flag = false;
		basicEffect.Projection = proj;
		basicEffect.World = DisplayManager.instance.currentWorldMtx;
		basicEffect.View = view;
		if (DisplayManager.instance.currentTexture == null)
		{
			basicEffect.TextureEnabled = false;
		}
		else
		{
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
			flag = DisplayManager.instance.currentTexture.hasAlpha;
		}
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		if (col.A != byte.MaxValue || flag)
		{
			DisplayManager.instance.SetBlendStateDefault();
		}
		else
		{
			DisplayManager.instance.SetBlendStateOff();
		}
		qverts[0].TextureCoordinate = new Vector2(u0, v0);
		qverts[0].Color = col;
		qverts[1].TextureCoordinate = new Vector2(u0, v1);
		qverts[1].Color = col;
		qverts[2].TextureCoordinate = new Vector2(u1, v0);
		qverts[2].Color = col;
		qverts[3].TextureCoordinate = new Vector2(u1, v1);
		qverts[3].Color = col;
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>((PrimitiveType)1, qverts, 0, 2);
	}

	public static void DrawQuad(Color col, float u0, float u1, float v0, float v1, bool alpha)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		if (basicEffect == null)
		{
			basicEffect = new BasicEffect(((Game)TheGame.instance).GraphicsDevice);
			basicEffect.VertexColorEnabled = true;
			qverts[0].Position = new Vector3(-0.5f, 0.5f, 0f);
			qverts[1].Position = new Vector3(-0.5f, -0.5f, 0f);
			qverts[2].Position = new Vector3(0.5f, 0.5f, 0f);
			qverts[3].Position = new Vector3(0.5f, -0.5f, 0f);
		}
		DisplayManager.instance.SetRasterizeStateCullOff();
		if (newframe)
		{
			view = DisplayManager.instance.currentViewMtx;
			proj = DisplayManager.instance.currentProjMtx;
			newframe = false;
		}
		basicEffect.Projection = proj;
		basicEffect.World = DisplayManager.instance.currentWorldMtx;
		basicEffect.View = view;
		if (DisplayManager.instance.currentTexture == null)
		{
			basicEffect.TextureEnabled = false;
		}
		else
		{
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
		}
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		qverts[0].TextureCoordinate = new Vector2(u0, v0);
		qverts[0].Color = col;
		qverts[1].TextureCoordinate = new Vector2(u0, v1);
		qverts[1].Color = col;
		qverts[2].TextureCoordinate = new Vector2(u1, v0);
		qverts[2].Color = col;
		qverts[3].TextureCoordinate = new Vector2(u1, v1);
		qverts[3].Color = col;
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>((PrimitiveType)1, qverts, 0, 2);
	}

	public static void DrawTriList(GameVertex[] data, int numPoints)
	{
		DrawTriList(data, numPoints, hasAlpha: false);
	}

	public static void DrawTriList(GameVertex[] data, int numPoints, bool hasAlpha)
	{
		DrawTriList(data, numPoints, hasAlpha, 0);
	}

	public static void DrawTriList(GameVertex[] data, int numPoints, bool hasAlpha, int offset)
	{
		DrawPrimitives((PrimitiveType)0, data, numPoints, hasAlpha, numPoints / 3, offset);
	}

	public static void DrawTriListEx(VertexPositionColorTexture[] data, int numPoints, bool hasAlpha, int offset)
	{
		DrawPrimitivesEx((PrimitiveType)0, data, numPoints, hasAlpha, numPoints / 3, offset);
	}

	public static void DrawTriStrip(GameVertex[] data, int numPoints)
	{
		DrawTriStrip(data, numPoints, hasAlpha: false);
	}

	public static void DrawTriStrip(GameVertex[] data, int numPoints, bool hasAlpha)
	{
		DrawPrimitives((PrimitiveType)1, data, numPoints, hasAlpha, numPoints - 2, 0);
	}

	public static void DrawTriStripEx(VertexPositionColorTexture[] data, int numPoints, bool hasAlpha)
	{
		DrawPrimitivesEx((PrimitiveType)1, data, numPoints, hasAlpha, numPoints - 2, 0);
	}

	private static void WriteVertData(ref VertexPositionColorTexture vert1, ref GameVertex pData1, ref VertexPositionColorTexture vert2, ref GameVertex pData2, ref VertexPositionColorTexture vert3, ref GameVertex pData3)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		vert1.Position.X = pData1.X;
		vert1.Position.Y = pData1.Y;
		vert1.Position.Z = pData1.Z;
		vert1.Color = pData1.color;
		vert1.TextureCoordinate.X = pData1.u;
		vert1.TextureCoordinate.Y = pData1.v;
		vert2.Position.X = pData2.X;
		vert2.Position.Y = pData2.Y;
		vert2.Position.Z = pData2.Z;
		vert2.Color = pData2.color;
		vert2.TextureCoordinate.X = pData2.u;
		vert2.TextureCoordinate.Y = pData2.v;
		vert3.Position.X = pData3.X;
		vert3.Position.Y = pData3.Y;
		vert3.Position.Z = pData3.Z;
		vert3.Color = pData3.color;
		vert3.TextureCoordinate.X = pData3.u;
		vert3.TextureCoordinate.Y = pData3.v;
	}

	private static void WriteVertData(ref VertexPositionColorTexture vert, ref GameVertex pData)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		vert.Position.X = pData.X;
		vert.Position.Y = pData.Y;
		vert.Position.Z = pData.Z;
		vert.Color = pData.color;
		vert.TextureCoordinate.X = pData.u;
		vert.TextureCoordinate.Y = pData.v;
	}

	private static void DrawPrimitives(PrimitiveType ptype, GameVertex[] data, int numPoints, bool hashAlpah, int numPrims, int offset)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		DisplayManager.instance.SetRasterizeStateCullOff();
		basicEffect.Projection = DisplayManager.instance.currentProjMtx;
		basicEffect.World = DisplayManager.instance.currentWorldMtx;
		basicEffect.View = DisplayManager.instance.currentViewMtx;
		if (DisplayManager.instance.currentTexture == null)
		{
			basicEffect.TextureEnabled = false;
		}
		else
		{
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
			if (DisplayManager.instance.currentTexture.hasAlpha)
			{
				hashAlpah = true;
			}
		}
		if (hashAlpah)
		{
			DisplayManager.instance.SetBlendStateDefault();
		}
		else
		{
			DisplayManager.instance.SetBlendStateOff();
		}
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		int num = vertsCurrentOffset;
		for (int i = 0; i < numPoints; i++)
		{
			WriteVertData(ref verts[vertsCurrentOffset++], ref data[i + offset]);
		}
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(ptype, verts, num, numPrims);
	}

	public static void DrawPrimitives2(PrimitiveType ptype, GameVertex[] data, int numPoints, bool hashAlpah, int numPrims, int offset)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		DisplayManager.instance.SetRasterizeStateCullOff();
		basicEffect.Projection = DisplayManager.instance.currentProjMtx;
		basicEffect.World = DisplayManager.instance.currentWorldMtx;
		basicEffect.View = DisplayManager.instance.currentViewMtx;
		if (DisplayManager.instance.currentTexture == null)
		{
			basicEffect.TextureEnabled = false;
		}
		else
		{
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
			if (DisplayManager.instance.currentTexture.hasAlpha)
			{
				hashAlpah = true;
			}
		}
		if (hashAlpah)
		{
			DisplayManager.instance.SetBlendStateDefault2();
		}
		else
		{
			DisplayManager.instance.SetBlendStateOff();
		}
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		int num = vertsCurrentOffset;
		int num2 = 0;
		while (num2 < numPoints)
		{
			if (numPoints >= 3)
			{
				WriteVertData(ref verts[vertsCurrentOffset++], ref data[num2 + offset], ref verts[vertsCurrentOffset++], ref data[num2 + offset + 1], ref verts[vertsCurrentOffset++], ref data[num2 + offset + 2]);
				num2 += 3;
			}
			else
			{
				WriteVertData(ref verts[vertsCurrentOffset++], ref data[num2 + offset]);
				num2++;
			}
		}
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(ptype, verts, num, numPrims);
	}

	public static void DrawPrimitivesEx(PrimitiveType ptype, VertexPositionColorTexture[] data, int numPoints, bool hashAlpha, int numPrims, int offset)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		DisplayManager.instance.SetRasterizeStateCullOff();
		basicEffect.Projection = DisplayManager.instance.currentProjMtx;
		basicEffect.World = DisplayManager.instance.currentWorldMtx;
		basicEffect.View = DisplayManager.instance.currentViewMtx;
		if (DisplayManager.instance.currentTexture == null)
		{
			basicEffect.TextureEnabled = false;
		}
		else
		{
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
			if (DisplayManager.instance.currentTexture.hasAlpha)
			{
				hashAlpha = true;
			}
		}
		if (hashAlpha)
		{
			DisplayManager.instance.SetBlendStateDefault();
		}
		else
		{
			DisplayManager.instance.SetBlendStateOff();
		}
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(ptype, data, offset, numPrims);
	}

	public static void DrawPrimitivesEx3(PrimitiveType ptype, VertexPositionColorTexture[] data, int numPoints, int numPrims, int offset, int cnt)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (cnt == 0)
		{
			DisplayManager.instance.SetRasterizeStateCullOff();
			basicEffect.Projection = DisplayManager.instance.currentProjMtx;
			basicEffect.World = DisplayManager.instance.currentWorldMtx;
			basicEffect.View = DisplayManager.instance.currentViewMtx;
			basicEffect.TextureEnabled = true;
		}
		if (DisplayManager.instance.currentTexture.hasAlpha)
		{
			DisplayManager.instance.SetBlendStateDefault();
		}
		else
		{
			DisplayManager.instance.SetBlendStateOff();
		}
		basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(ptype, data, offset, numPrims);
	}

	public static void DrawPrimitives2Ex(PrimitiveType ptype, VertexPositionColorTexture[] data, int numPoints, bool hashAlpah, int numPrims, int offset)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (offset == 0)
		{
			if (DisplayManager.instance.currentTexture == null)
			{
				basicEffect.TextureEnabled = false;
			}
			else
			{
				basicEffect.TextureEnabled = true;
				basicEffect.Texture = DisplayManager.instance.currentTexture.intex;
				if (DisplayManager.instance.currentTexture.hasAlpha)
				{
					hashAlpah = true;
				}
			}
			if (hashAlpah)
			{
				DisplayManager.instance.SetBlendStateDefault2();
			}
			else
			{
				DisplayManager.instance.SetBlendStateOff();
			}
		}
		basicEffect.Projection = DisplayManager.instance.currentProjMtx;
		basicEffect.World = DisplayManager.instance.currentWorldMtx;
		basicEffect.View = DisplayManager.instance.currentViewMtx;
		((Effect)basicEffect).CurrentTechnique.Passes[0].Apply();
		((Game)TheGame.instance).GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(ptype, data, 0, numPrims);
	}
}
