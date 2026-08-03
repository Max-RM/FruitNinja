namespace Mortar;

public class RingBufferT<T>
{
	private T[] mem;

	private int memsize;

	private int inptr;

	private int outptr;

	public void Init(int size)
	{
		Clear();
		mem = new T[size];
		memsize = size;
		inptr = 1;
		outptr = 0;
	}

	public bool Push(ref T v)
	{
		if (inptr == outptr)
		{
			return false;
		}
		mem[inptr] = v;
		if (inptr == memsize - 1)
		{
			inptr = 0;
		}
		else
		{
			inptr++;
		}
		return true;
	}

	public bool Peek(ref T ans)
	{
		int num = ((outptr != memsize - 1) ? (outptr + 1) : 0);
		if (num == inptr)
		{
			return false;
		}
		ans = mem[num];
		return true;
	}

	public bool Pop(ref T ans)
	{
		int num = ((outptr != memsize - 1) ? (outptr + 1) : 0);
		if (num == inptr)
		{
			return false;
		}
		ans = mem[num];
		outptr = num;
		return true;
	}

	public void Clear()
	{
		if (mem != null)
		{
			Delete.SAFE_DELETE_ARRAY(ref mem);
		}
	}
}
