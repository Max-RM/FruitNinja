using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class DisplayManager
{
	public Matrix currentViewMtx;

	public Matrix currentWorldMtx;

	public Matrix currentProjMtx;

	public Matrix currentTextureMtx;

	public Matrix topMatrix;

	public Point Res;

	public Texture currentTexture;

	private Color clearcolor = Color.Black;

	private bool denabled;

	private bool dwriteenabled = true;

	private DepthStencilState[] depthStates;

	private BlendState bsOff;

	private BlendState bsDef;

	private RasterizerState rsCullOff;

	private RasterizerState rsCullCwise;

	public static DisplayManager instance = new DisplayManager();

	public DisplayManager()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		Res = new Point(800, 480);
		depthStates = ArrayInit.CreateFilledArray<DepthStencilState>(4);
		depthStates[0].DepthBufferEnable = false;
		depthStates[0].DepthBufferWriteEnable = false;
		depthStates[1].DepthBufferEnable = false;
		depthStates[1].DepthBufferWriteEnable = true;
		depthStates[2].DepthBufferEnable = true;
		depthStates[2].DepthBufferWriteEnable = false;
		depthStates[3].DepthBufferEnable = true;
		depthStates[3].DepthBufferWriteEnable = true;
		bsOff = new BlendState();
		bsDef = new BlendState();
		bsDef.ColorBlendFunction = (BlendFunction)0;
		bsDef.ColorSourceBlend = (Blend)4;
		bsDef.ColorDestinationBlend = (Blend)5;
		bsDef.AlphaBlendFunction = (BlendFunction)0;
		bsDef.AlphaSourceBlend = (Blend)4;
		bsDef.AlphaDestinationBlend = (Blend)5;
		rsCullCwise = new RasterizerState();
		rsCullCwise.CullMode = (CullMode)1;
		rsCullOff = new RasterizerState();
		rsCullOff.CullMode = (CullMode)0;
	}

	public static DisplayManager GetInstance()
	{
		return instance;
	}

	public void SetWindowSize(int xpos, int xsize, int ypos, int ysize)
	{
	}

	public void Init(string name)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		topMatrix = Matrix.CreateScale(new Vector3(1.6666666f, 1.5f, 1f));
	}

	public void SetClearColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		clearcolor = color;
	}

	public void SetLightDirection(Vector3 ld)
	{
	}

	public void SetGlobalAmbience(Color col)
	{
	}

	public void DidUploadMatrixies()
	{
	}

	public void BeginFrame()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Game)TheGame.instance).GraphicsDevice.Clear((ClearOptions)3, clearcolor, 1f, 0);
		Mesh.newframe = true;
		SetBlendStateOff();
		Mesh.vertsCurrentOffset = 0;
	}

	public void EndFrame()
	{
	}

	public void SwapBuffers()
	{
	}

	public void SetNewDeptStensileState(DepthStencilState d)
	{
		((Game)TheGame.instance).GraphicsDevice.DepthStencilState = d;
	}

	public void SetNewBlendState(BlendState b)
	{
		((Game)TheGame.instance).GraphicsDevice.BlendState = b;
	}

	public void SetNewRasterizeState(RasterizerState r)
	{
		((Game)TheGame.instance).GraphicsDevice.RasterizerState = r;
	}

	private void GetNewDepthStensileState()
	{
		SetNewDeptStensileState(depthStates[(denabled ? 2 : 0) + (dwriteenabled ? 1 : 0)]);
	}

	public void SetBlendStateOff()
	{
		SetNewBlendState(bsOff);
	}

	public void SetBlendStateDefault()
	{
		SetNewBlendState(BlendState.NonPremultiplied);
	}

	public void SetBlendStateDefault2()
	{
		SetNewBlendState(BlendState.NonPremultiplied);
	}

	public void SetRasterizeStateCullOff()
	{
		SetNewRasterizeState(rsCullOff);
	}

	public void SetRasterizeStateCullCwise()
	{
		SetNewRasterizeState(rsCullCwise);
	}

	public void SetDepthBuffer(bool en)
	{
		denabled = en;
		GetNewDepthStensileState();
	}

	public void SetDepthBufferWrite(bool en)
	{
		dwriteenabled = en;
		GetNewDepthStensileState();
	}

	public MortarRectangle GetWindowSize()
	{
		MortarRectangle result = default(MortarRectangle);
		result.left = 0;
		result.top = 0;
		result.right = Res.X;
		result.bottom = Res.Y;
		return result;
	}
}
