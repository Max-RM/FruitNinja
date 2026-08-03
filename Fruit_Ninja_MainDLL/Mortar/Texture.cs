using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class Texture
{
	public enum TextureType
	{
		TT_2D,
		TT_3D,
		TT_Cube
	}

	public enum TextureFormat
	{
		TF_UNKNOWN,
		TF_CUSTOM,
		TF_A8,
		TF_L8,
		TF_R5G6B5,
		TF_X1R5G5B5,
		TF_A1R5G5B5,
		TF_A4R4G4B4,
		TF_A8L8,
		TF_V8U8,
		TF_L16,
		TF_A8R8G8B8,
		TF_X8R8G8B8,
		TF_G16R16,
		TF_A2R10G10B10,
		TF_A2B10G10R10,
		TF_X8L8V8U8,
		TF_V16U16,
		TF_A16B16G16R16,
		TF_Q8W8V8U8,
		TF_DXT1,
		TF_DXT2,
		TF_DXT3,
		TF_DXT4,
		TF_DXT5,
		TF_R16F,
		TF_G16R16F,
		TF_A16B16G16R16F,
		TF_R32F,
		TF_G32R32F,
		TF_A32B32G32R32F,
		TF_RGB,
		TF_RGBA,
		TF_MAX
	}

	public enum TextureUsage
	{
		TU_None = 0,
		TU_CanRead = 1,
		TU_Dynamic = 2,
		TU_RenderTarget = 4
	}

	private enum MRImage2Formats
	{
		MRI_COLOR_FORMAT_MASK = 15,
		MRI_COLOR_FORMAT_SHIFT = 0,
		MRI_CF_BGR_888 = 1,
		MRI_CF_ABGR_8888 = 2,
		MRI_CF_RGBA_5551 = 3,
		MRI_CF_RGBA_4444 = 4,
		MRI_CF_RGB_565 = 5,
		MRI_CF_RGB_888 = 6,
		MRI_CF_ARGB_1555 = 7,
		MRI_CF_ARGB_8888 = 8,
		MRI_CF_XRGB_8888 = 9,
		MRI_CF_RGBX_5551 = 10,
		MRI_CF_ARGB_4444 = 11,
		MRI_VERTICAL_FLIP = 16,
		MRI_HORIZONTAL_FLIP = 32,
		MRI_CONTAINER_FORMAT_MASK = 192,
		MRI_CONTAINER_FORMAT_SHIFT = 6,
		MRI_CONTF_MORTAR = 128,
		MRI_CONTF_WORKING = 64,
		MRI_NATIVE_COMPRESSION_MASK = 3840,
		MRI_NATIVE_COMPRESSION_SHIFT = 8,
		MRI_NC_RAW = 256,
		MRI_NC_PVRTC_2BPP = 512,
		MRI_NC_PVRTC_4BPP = 768,
		MRI_NC_A3I5 = 1024,
		MRI_NC_A5I3 = 1280,
		MRI_NC_PAL256 = 1536,
		MRI_NC_PAL16 = 1792,
		MRI_NC_PAL4 = 2048,
		MRI_PALETTE_FORMAT_0_COLOR_TRANS = 4096,
		MRI_LINES_32BIT_ALIGNED = 8192,
		MRI_MIPMAPS_SHARE_PALETTE = 16384
	}

	public struct MRImage2Mortar
	{
		public byte pad01;

		public byte pad00;

		public ushort version;

		public uint format;

		public byte x;

		public byte y;

		public byte pad03;

		public byte mipmapLevels;

		public ushort apparentX;

		public ushort apparentY;
	}

	private const int MRI_CURRENT_VERSION = 4;

	private const int MRI_HEADER_SIZE = 16;

	public uint w;

	public uint h;

	public uint mipmaps;

	public Texture2D intex;

	public bool cached;

	public bool hasAlpha;

	public bool localise;

	public string texture_filename;

	private void SetUnCached()
	{
	}

	private void UnSetUnCached()
	{
	}

	public Texture()
	{
		localise = false;
	}

	public static bool FileExists(string fileName)
	{
		string text = (fileName.EndsWith(".tex") ? fileName.Replace(".tex", ".png") : ((!fileName.EndsWith(".tga")) ? fileName : fileName.Replace(".tga", ".png")));
		try
		{
			TitleContainer.OpenStream("Content/" + text + ".xnb");
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static Texture Load(string fileName)
	{
		return Load_Internal(new Texture(), fileName);
	}

	public static void Reload(string fileName, Texture texture)
	{
		Load_Internal(texture, fileName);
	}

	private static Texture Load_Internal(Texture newTexture, string fileName)
	{
		try
		{
			string text = (fileName.EndsWith(".tex") ? fileName.Replace(".tex", ".png") : ((!fileName.EndsWith(".tga")) ? fileName : fileName.Replace(".tga", ".png")));
			Texture2D val = ((Game)TheGame.instance).Content.Load<Texture2D>(text);
			newTexture.h = (uint)val.Height;
			newTexture.w = (uint)val.Width;
			newTexture.hasAlpha = true;
			newTexture.mipmaps = 1u;
			newTexture.cached = false;
			newTexture.intex = val;
			if (newTexture.texture_filename == null)
			{
				newTexture.texture_filename = fileName;
			}
		}
		catch (Exception)
		{
			return null;
		}
		return newTexture;
	}

	public uint GetWidth()
	{
		return w;
	}

	public uint GetHeight()
	{
		return h;
	}

	public void Set()
	{
		DisplayManager.instance.currentTexture = this;
	}

	public void UnSet()
	{
	}
}
