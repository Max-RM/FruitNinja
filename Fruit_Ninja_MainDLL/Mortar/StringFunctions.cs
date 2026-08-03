using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Mortar;

public class StringFunctions
{
	public static uint StringHash(string s)
	{
		return (uint)(s?.GetHashCode() ?? 0);
	}

	public static bool CompareWords(string text, string word)
	{
		if (text != null && word != null)
		{
			return text == word;
		}
		return false;
	}

	public static Texture LoadTexture(string file)
	{
		if (file != null)
		{
			string texture = $"textureswp7/{file}.tex";
			return TextureManager.GetInstance().Load(texture);
		}
		return null;
	}

	public static void ParseColour(ref Color color, string text)
	{
		if (text != null)
		{
			int[] array = new int[4] { 255, 255, 255, 255 };
			string[] array2 = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < 4 && i < array2.Length; i++)
			{
				array[i] = MParser.ParseInt(array2[i].Trim());
			}
			color.R = (byte)array[0];
			color.G = (byte)array[1];
			color.B = (byte)array[2];
			color.A = (byte)array[3];
		}
	}

	public static int SplitWords(string text, ref List<string> words)
	{
		if (text == null)
		{
			return 0;
		}
		string[] array = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			words.Add(text2.Trim());
		}
		return words.Count;
	}

	public static uint ParseMaskWords(string text, uint[] hashes, int numTypes)
	{
		if (text == null)
		{
			return 0u;
		}
		uint num = 0u;
		string[] array = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			uint value = StringHash(text2.Trim());
			int num2 = Array.IndexOf(hashes, value);
			if (num2 != -1)
			{
				num |= (uint)(1 << num2);
			}
		}
		return num;
	}

	public static int FindIndex(string text, uint[] hashes, int numHashes)
	{
		if (text != null)
		{
			uint num = StringHash(text);
			for (int i = 0; i < numHashes; i++)
			{
				if (hashes[i] == num)
				{
					return i;
				}
			}
		}
		return 0;
	}

	public static bool StartsWithWord(string text, string word)
	{
		if (text == null || word == null || text.Length < word.Length)
		{
			return false;
		}
		for (int i = 0; i < word.Length; i++)
		{
			if (word[i] != text[i])
			{
				return false;
			}
		}
		return true;
	}

	public static void ParseFloats(string text, float[] floats, int max)
	{
		if (text == null)
		{
			return;
		}
		string[] array = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < max; i++)
		{
			if (i < array.Length)
			{
				floats[i] = MParser.ParseFloat(array[i].Trim());
			}
			else
			{
				floats[i] = floats[i - 1];
			}
		}
	}
}
