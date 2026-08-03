using System.Collections.Generic;

namespace Mortar;

public static class ArrayInit
{
	public static List<T> CreateInitedList<T>(int initsize)
	{
		List<T> list = new List<T>(initsize);
		for (int i = 0; i < initsize; i++)
		{
			list.Add(default(T));
		}
		return list;
	}

	public static T[] CreateFilledArray<T>(int size) where T : new()
	{
		T[] array = new T[size];
		for (int i = 0; i < size; i++)
		{
			array[i] = new T();
		}
		return array;
	}

	public static T[,] CreateFilledArray<T>(int size, int size2) where T : new()
	{
		T[,] array = new T[size, size2];
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size2; j++)
			{
				array[i, j] = new T();
			}
		}
		return array;
	}
}
