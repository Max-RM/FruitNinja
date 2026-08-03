using System;
using System.Collections.Generic;
using FNWP72.Engine;

namespace Mortar;

public class TextureManager
{
	private static TextureManager instance;

	private static List<Texture> loadedTextures = new List<Texture>();

	public static TextureManager GetInstance()
	{
		if (instance == null)
		{
			instance = new TextureManager();
		}
		return instance;
	}

	public void Initialise()
	{
	}

	public void Initialise(int heapsize)
	{
	}

	public static bool TextureFileExists(string fileName)
	{
		return Texture.FileExists(fileName);
	}

	public Texture Load(string texture)
	{
		Texture texture2 = Texture.Load(texture);
		if (texture2 != null)
		{
			loadedTextures.Add(texture2);
		}
		return texture2;
	}

	public Texture Load(string texture, bool localise)
	{
		string fileName = MTLocalisation.GetLocalisedTexturePath() + "/" + texture;
		Texture texture2 = Texture.Load(fileName);
		if (texture2 != null)
		{
			loadedTextures.Add(texture2);
			texture2.localise = localise;
			texture2.texture_filename = texture;
		}
		return texture2;
	}

	public void ReloadLocalisedTextures(StringTableUtils.Language language)
	{
		foreach (Texture loadedTexture in loadedTextures)
		{
			if (loadedTexture.localise)
			{
				loadedTexture.intex = null;
			}
		}
		GC.Collect();
		List<Texture>.Enumerator enumerator2 = loadedTextures.GetEnumerator();
		enumerator2.MoveNext();
		while (enumerator2.Current != null)
		{
			Texture current2 = enumerator2.Current;
			if (current2.localise)
			{
				string text = MTLocalisation.GetLocalisedTexturePath(language) + "/" + current2.texture_filename;
				if (!text.EndsWith(".tex"))
				{
					text += ".tex";
				}
				Texture.Reload(text, current2);
			}
			enumerator2.MoveNext();
		}
	}
}
