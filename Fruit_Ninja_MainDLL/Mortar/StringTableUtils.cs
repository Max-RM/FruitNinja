namespace Mortar;

public class StringTableUtils
{
	public enum Table
	{
		TABLE_COMMON,
		TABLE_LEVEL,
		TABLE_MAX
	}

	public enum Language
	{
		LANGUAGE_ENGLISH,
		LANGUAGE_ENGLISH_UK,
		LANGUAGE_FRENCH,
		LANGUAGE_SPANISH,
		LANGUAGE_GERMAN,
		LANGUAGE_ITALIAN,
		LANGUAGE_DUTCH,
		LANGUAGE_SWEDISH,
		LANGUAGE_DANISH,
		LANGUAGE_NORWEGIAN,
		LANGUAGE_FINNISH,
		LANGUAGE_KOREAN,
		LANGUAGE_JAPANESE,
		LANGUAGE_CHINESE_TRADITIONAL,
		LANGUAGE_CHINESE_SIMPLIFIED,
		LANGUAGE_MAX
	}

	public enum LM
	{
		LEVEL_MENU,
		CHAPTER_TUTORIAL,
		CHAPTER_1,
		CHAPTER_2,
		CHAPTER_3,
		CHAPTER_4,
		CHAPTER_5,
		CHAPTER_6A,
		CHAPTER_6B,
		LEVEL_CREDITS
	}

	public enum FontFamily
	{
		WestEuropean,
		EastAsian
	}

	private const int NUM_STRING_TABLES = 2;

	public static FontFamily GetFontFamily(Language language)
	{
		switch (language)
		{
		case Language.LANGUAGE_KOREAN:
		case Language.LANGUAGE_JAPANESE:
		case Language.LANGUAGE_CHINESE_TRADITIONAL:
		case Language.LANGUAGE_CHINESE_SIMPLIFIED:
			return FontFamily.EastAsian;
		case Language.LANGUAGE_ENGLISH:
		case Language.LANGUAGE_ENGLISH_UK:
		case Language.LANGUAGE_FRENCH:
		case Language.LANGUAGE_SPANISH:
		case Language.LANGUAGE_GERMAN:
		case Language.LANGUAGE_ITALIAN:
		case Language.LANGUAGE_DUTCH:
		case Language.LANGUAGE_SWEDISH:
		case Language.LANGUAGE_DANISH:
		case Language.LANGUAGE_NORWEGIAN:
		case Language.LANGUAGE_FINNISH:
			return FontFamily.WestEuropean;
		default:
			return FontFamily.WestEuropean;
		}
	}

	public static void StringTableUtilInit()
	{
	}

	public static void StringTableUtilLoadStrings()
	{
	}

	public static void StringTableUtilLoadStringsTable(int i)
	{
	}

	public static void StringTableUtilUnload()
	{
	}

	public static void StringTableUtilUnloadTable(int i)
	{
	}

	public static bool StringTableUtilLoaded()
	{
		return false;
	}

	internal static Language ParseLanguageFromCulture(string cultureName)
	{
		switch (cultureName)
		{
		case "en":
		case "en-AU":
		case "en-BZ":
		case "en-CA":
		case "en-CB":
		case "en-IE":
		case "en-JM":
		case "en-NZ":
		case "en-PH":
		case "en-ZA":
		case "en-TT":
		case "en-GB":
		case "en-ZW":
			return Language.LANGUAGE_ENGLISH_UK;
		case "en-US":
			return Language.LANGUAGE_ENGLISH;
		case "fr":
		case "fr-BE":
		case "fr-CA":
		case "fr-FR":
		case "fr-LU":
		case "fr-MC":
		case "fr-CH":
			return Language.LANGUAGE_FRENCH;
		case "de":
		case "de-AT":
		case "de-DE":
		case "de-LI":
		case "de-LU":
		case "de-CH":
			return Language.LANGUAGE_GERMAN;
		case "it":
		case "it-IT":
		case "it-CH":
			return Language.LANGUAGE_ITALIAN;
		case "es":
		case "es-AR":
		case "es-BO":
		case "es-CL":
		case "es-CO":
		case "es-CR":
		case "es-DO":
		case "es-EC":
		case "es-SV":
		case "es-GT":
		case "es-HN":
		case "es-MX":
		case "es-NI":
		case "es-PA":
		case "es-PY":
		case "es-PE":
		case "es-PR":
		case "es-ES":
		case "es-UY":
		case "es-VE":
			return Language.LANGUAGE_SPANISH;
		case "ko-KR":
			return Language.LANGUAGE_ENGLISH;
		case "zh-CN":
			return Language.LANGUAGE_CHINESE_SIMPLIFIED;
		default:
			return Language.LANGUAGE_ENGLISH;
		}
	}
}
