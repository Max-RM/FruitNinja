using System;
using Microsoft.Xna.Framework;

namespace Mortar;

public class StringTable
{
	private string name;

	private int defaultLangauge;

	private string[] stringHashes;

	private string[] languageHashes;

	private StringTableLanguageStringSet[] languages;

	public static string[] langnames = new string[8] { "english_us", "english_uk", "french", "spanish", "german", "italian", "chinese", "traditional_chinese" };

	public static int[] hashes = new int[8]
	{
		langnames[0].GetHashCode(),
		langnames[1].GetHashCode(),
		langnames[2].GetHashCode(),
		langnames[3].GetHashCode(),
		langnames[4].GetHashCode(),
		langnames[5].GetHashCode(),
		langnames[6].GetHashCode(),
		langnames[7].GetHashCode()
	};

	public int GetStringIdx(string str)
	{
		return Array.BinarySearch(stringHashes, str);
	}

	public int GetLanguageIdx(string lng)
	{
		int hashCode = lng.GetHashCode();
		int num = 0;
		int[] array = hashes;
		foreach (int num2 in array)
		{
			if (hashCode == num2)
			{
				return num;
			}
			num++;
		}
		return 0;
	}

	public string GetString(string str)
	{
		return GetString(str, defaultLangauge);
	}

	public string GetString(int sidx)
	{
		return GetString(sidx, defaultLangauge);
	}

	public string GetString(int sidx, string lng)
	{
		return GetString(sidx, GetLanguageIdx(lng));
	}

	public string GetString(string str, string lng)
	{
		return GetString(str, GetLanguageIdx(lng));
	}

	public string GetString(string str, int lidx)
	{
		return GetString(GetStringIdx(str), lidx);
	}

	public string GetString(int sidx, int lidx)
	{
		return languages[lidx].strings[sidx];
	}

	public void UpdateDefaultLanguage()
	{
		defaultLangauge = GetLanguageIdx(StringManager.GetInstance().defaultLanguage);
	}

	public void LoadHeader(string filename)
	{
		name = filename;
		string[][] array = ((Game)TheGame.instance).Content.Load<string[][]>(filename + "_header.str");
		stringHashes = array[0];
		languageHashes = array[1];
		languages = new StringTableLanguageStringSet[languageHashes.Length];
		UpdateDefaultLanguage();
	}

	public void LoadLanguage(string lng)
	{
		LoadLanguage(GetLanguageIdx(lng));
	}

	public void LoadLanguage(int lidx)
	{
		if (languages[lidx] == null)
		{
			languages[lidx] = new StringTableLanguageStringSet();
			languages[lidx].strings = ((Game)TheGame.instance).Content.Load<string[]>(name + "_" + langnames[lidx] + ".str");
			for (int i = 0; i < languages[lidx].strings.Length; i++)
			{
				languages[lidx].strings[i] = languages[lidx].strings[i].Replace("%i", "{0}");
			}
		}
	}

	public void UnloadLanguage(int lidx)
	{
		languages[lidx] = null;
	}

	public void LoadAllLanguages()
	{
		string[] array = languageHashes;
		foreach (string lng in array)
		{
			LoadLanguage(lng);
		}
	}

	public void UnloadAllLanguages()
	{
		languages = new StringTableLanguageStringSet[languageHashes.Length];
	}
}
