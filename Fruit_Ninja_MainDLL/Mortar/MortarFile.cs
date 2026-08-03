using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Mortar;

public class MortarFile
{
	public class ByteStream : Stream
	{
		private byte[] data;

		private long p;

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public override long Length => data.Length;

		public override long Position
		{
			get
			{
				return p;
			}
			set
			{
				p = value;
			}
		}

		public ByteStream(byte[] d)
		{
			data = d;
			p = 0L;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			Array.Copy(data, (int)p, buffer, offset, count);
			p += count;
			return count;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				p = offset;
				break;
			case SeekOrigin.Current:
				p += offset;
				break;
			case SeekOrigin.End:
				p = data.Length + offset;
				break;
			}
			if (p > data.Length)
			{
				p = data.Length;
			}
			if (p < 0)
			{
				p = 0L;
			}
			return p;
		}

		public override void SetLength(long value)
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
		}
	}

	public static Stream LoadBinStream(string fl)
	{
		try
		{
			string text = "Content/" + fl + ".xnb";
			Stream stream = TitleContainer.OpenStream(text);
			if (stream != null)
			{
				stream.Seek(207L, SeekOrigin.Begin);
				return stream;
			}
		}
		catch
		{
			return null;
		}
		return null;
	}

	public static BinaryReader LoadBinBR(string fl)
	{
		try
		{
			return new BinaryReader(LoadBinStream(fl));
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static byte[] LoadBin(string fl)
	{
		Stream stream = LoadBinStream(fl);
		if (stream == null)
		{
			return null;
		}
		byte[] array = new byte[stream.Length - stream.Position];
		stream.Read(array, 0, array.Length);
		return array;
	}

	public static string LoadText(string name)
	{
		try
		{
			return ((Game)TheGame.instance).Content.Load<string>(name);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
