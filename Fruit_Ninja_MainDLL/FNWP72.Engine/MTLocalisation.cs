using FruitNinja;
using Mortar;

namespace FNWP72.Engine;

public class MTLocalisation
{
	private static bool bFontsLoaded;

	private static Font pCachedFont_WestEuropean;

	private static Font pCachedFont_EastAsian;

	public static bool FontsLoaded => bFontsLoaded;

	public static StringTableUtils.Language GetCurrentLanguage()
	{
		return Game.game_work.language;
	}

	public static string GetLocalisedTexturePath()
	{
		return GetLocalisedTexturePath(GetCurrentLanguage());
	}

	public static string GetLocalisedTexturePath(StringTableUtils.Language language)
	{
		switch (language)
		{
		case StringTableUtils.Language.LANGUAGE_ENGLISH:
		case StringTableUtils.Language.LANGUAGE_ENGLISH_UK:
			return "localisedwp7/en";
		case StringTableUtils.Language.LANGUAGE_FRENCH:
			return "localisedwp7/fr";
		case StringTableUtils.Language.LANGUAGE_ITALIAN:
			return "localisedwp7/it";
		case StringTableUtils.Language.LANGUAGE_GERMAN:
			return "localisedwp7/de";
		case StringTableUtils.Language.LANGUAGE_SPANISH:
			return "localisedwp7/es";
		case StringTableUtils.Language.LANGUAGE_CHINESE_TRADITIONAL:
		case StringTableUtils.Language.LANGUAGE_CHINESE_SIMPLIFIED:
			return "localisedwp7/zh";
		default:
			return "localisedwp7/en";
		}
	}

	public static string GetLocalisedFontsPath()
	{
		return GetLocalisedFontsPath(GetCurrentLanguage());
	}

	public static string GetLocalisedFontsPath(StringTableUtils.Language language)
	{
		StringTableUtils.FontFamily fontFamily = StringTableUtils.GetFontFamily(language);
		return GetLocalisedFontsPath(fontFamily);
	}

	public static string GetLocalisedFontsPath(StringTableUtils.FontFamily languageFontFamily)
	{
		return languageFontFamily switch
		{
			StringTableUtils.FontFamily.WestEuropean => "fonts", 
			StringTableUtils.FontFamily.EastAsian => "fontseastasian", 
			_ => "fonts", 
		};
	}

	public static void LoadFonts()
	{
		pCachedFont_EastAsian = LoadDefaultFont(StringTableUtils.FontFamily.EastAsian);
		pCachedFont_WestEuropean = LoadDefaultFont(StringTableUtils.FontFamily.WestEuropean);
		bFontsLoaded = true;
	}

	private static Font LoadDefaultFont(StringTableUtils.FontFamily fontFamily)
	{
		return LoadFont("font_fruit_ninja.fnt", fontFamily);
	}

	public static Font LoadFont(string fontFileName, StringTableUtils.FontFamily fontFamily)
	{
		Font font = new Font();
		font.Load(GetLocalisedFontsPath(fontFamily) + "/" + fontFileName);
		return font;
	}

	public static Font GetCachedFont(StringTableUtils.Language language)
	{
		return GetCachedFont(StringTableUtils.GetFontFamily(language));
	}

	public static Font GetCachedFont(StringTableUtils.FontFamily fontFamily)
	{
		return fontFamily switch
		{
			StringTableUtils.FontFamily.EastAsian => pCachedFont_EastAsian, 
			StringTableUtils.FontFamily.WestEuropean => pCachedFont_WestEuropean, 
			_ => pCachedFont_WestEuropean, 
		};
	}
}
