using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class Font
{
	public class CharTemplate
	{
		public ushort id;

		public float X;

		public float Y;

		public float width;

		public float height;

		public float xOffset;

		public float yOffset;

		public float xAdvance;

		public byte page;
	}

	public struct Page
	{
		public string pageName;

		public Texture texture;
	}

	public struct Kerning
	{
		public int first;

		public int second;

		public float amount;
	}

	public class AString
	{
		public string astring;

		public AString(string str)
		{
			astring = str;
		}
	}

	public struct CharPtr
	{
		public AString str;

		public int idx;

		public char Val => str.astring[idx];

		public bool isNull => str == null;

		public bool isNotNull => str != null;

		public bool isEos => idx == str.astring.Length;

		public bool isNotEos => idx != str.astring.Length;

		public string Str => str.astring.Substring(idx);

		public CharPtr(AString s)
		{
			if (s == null)
			{
				str = null;
			}
			else
			{
				str = s;
			}
			idx = 0;
		}

		public CharPtr(AString s, int i)
		{
			if (s == null)
			{
				str = null;
				idx = 0;
			}
			else
			{
				str = s;
				idx = i;
			}
		}

		public void Dec()
		{
			idx--;
		}

		public char arr(int i)
		{
			if (i + idx >= str.astring.Length)
			{
				return '\0';
			}
			return str.astring[idx + i];
		}

		public bool Cmp(CharPtr b)
		{
			if (object.ReferenceEquals(str, b.str))
			{
				return b.idx == idx;
			}
			return false;
		}

		public CharPtr Offset(int o)
		{
			return new CharPtr
			{
				str = str,
				idx = idx + o
			};
		}
	}

	private enum FontReaderState
	{
		Idle,
		Info,
		Common,
		Page,
		Chars,
		Char,
		SkipLine
	}

	private enum FontReaderState_Info
	{
		Idle,
		Face,
		Size,
		Bold,
		Italic,
		Charset,
		Unicode,
		StretchH,
		Smooth,
		Padding,
		Spacing,
		Outline,
		SkipValue
	}

	private enum FontReaderState_Common
	{
		Idle,
		LineHeight,
		Base,
		ScaleW,
		ScaleH,
		Pages,
		Packed,
		AlphaChnl,
		RedChnl,
		GreenChnl,
		BlueChnl,
		SkipValue
	}

	private enum FontReaderState_Page
	{
		Idle,
		Id,
		File,
		FileQuotes,
		SkipValue
	}

	private enum FontReaderState_Chars
	{
		Idle,
		Count,
		SkipValue
	}

	private enum FontReaderState_Char
	{
		Idle,
		Id,
		X,
		Y,
		Width,
		Height,
		XOffset,
		YOffset,
		XAdvance,
		Page,
		Chnl,
		SkipValue
	}

	public static readonly int QUICKFIND_CHARS = 256;

	public static readonly int QUADS_IN_BUFFER = 256;

	private CharTemplate[] m_charTemplates;

	private CharTemplate[] m_charLookUp = new CharTemplate[QUICKFIND_CHARS];

	private int m_charTemplateCnt;

	private Page[] m_pages;

	private int m_pageCnt;

	private Kerning[] m_kernings;

	private int m_kerningCnt;

	public uint m_hash_name;

	public int m_textureWidth;

	public int m_textureHeight;

	public float m_lineHeight;

	public float m_baseLine;

	public VertexPositionColorTexture[] quad_verts;

	public static readonly int SPACE_CHAR = 32;

	public static readonly int NEW_LINE_CHAR = 13;

	public static readonly int QUOTES_CHAR = 34;

	public static readonly int INVALID_NUMBER = -43710;

	public static bool strnicmp(CharPtr p, string tst, int l)
	{
		return p.str.astring.Substring(p.idx, l).ToLower() == tst.Substring(0, l).ToLower();
	}

	public static void cpStrcpy(CharPtr p, string src)
	{
		p.str.astring = p.str.astring.Substring(0, p.idx) + src;
	}

	public static int cpStrlen(CharPtr p)
	{
		return p.str.astring.Length - p.idx;
	}

	private int Get_Next_Value(CharPtr readingFrom, CharPtr valueName, Action<int> intValue, Action<CharPtr> stringValue)
	{
		int num = 0;
		while (readingFrom.arr(num) != '=')
		{
			num++;
			if (readingFrom.arr(num) == NEW_LINE_CHAR || readingFrom.Offset(num).isEos)
			{
				return -num;
			}
		}
		int num2 = num - 1;
		while (readingFrom.arr(num2) != SPACE_CHAR && num2 > 0)
		{
			num2--;
		}
		if (valueName.isNotNull)
		{
			int num3 = num2 + 1;
			cpStrcpy(valueName, readingFrom.Str.Substring(num3, num - num3));
		}
		num++;
		if (readingFrom.arr(num) == NEW_LINE_CHAR || readingFrom.Offset(num).isEos)
		{
			return -num;
		}
		if (intValue == null || stringValue == null)
		{
			return num;
		}
		bool flag = false;
		if (readingFrom.arr(num) == '-')
		{
			flag = true;
			num++;
			if (readingFrom.arr(num) == NEW_LINE_CHAR || readingFrom.Offset(num).isEos)
			{
				return -num;
			}
		}
		if (readingFrom.arr(num) == QUOTES_CHAR)
		{
			num++;
			if (readingFrom.arr(num) == NEW_LINE_CHAR || readingFrom.Offset(num).isEos)
			{
				return -num;
			}
			int num4 = num;
			while (readingFrom.arr(num) != QUOTES_CHAR && readingFrom.arr(num) != '.')
			{
				num++;
				if (readingFrom.arr(num) == NEW_LINE_CHAR || readingFrom.Offset(num).isEos)
				{
					return -num;
				}
			}
			if (readingFrom.arr(num) == '.')
			{
				stringValue(new CharPtr(new AString(readingFrom.Str.Substring(num4, num - num4) + ".tex")));
			}
			else
			{
				stringValue(new CharPtr(new AString(readingFrom.Str.Substring(num4, num - num4))));
			}
		}
		else
		{
			int num5 = num;
			for (; readingFrom.arr(num) != SPACE_CHAR && readingFrom.arr(num) != NEW_LINE_CHAR && readingFrom.arr(num) != 0; num++)
			{
			}
			int num6 = 0;
			int num7 = 1;
			for (int num8 = num - 1; num8 >= num5; num8--)
			{
				if (readingFrom.arr(num8) < '0' || readingFrom.arr(num8) > '9')
				{
					return num;
				}
				num6 += num7 * (readingFrom.arr(num8) - 48);
				num7 *= 10;
			}
			intValue(num6 * ((!flag) ? 1 : (-1)));
		}
		if (readingFrom.arr(num) == NEW_LINE_CHAR || readingFrom.Offset(num).isEos)
		{
			return -num;
		}
		return num;
	}

	private bool Next_Word_Is(CharPtr readingFrom, string test)
	{
		int length = test.Length;
		int i;
		for (i = 0; i < length && readingFrom.arr(i) != 0 && readingFrom.arr(i) == test[i] && readingFrom.arr(i) != SPACE_CHAR; i++)
		{
		}
		return i >= length;
	}

	private int Parse_Char(CharPtr readingFrom, ref CharTemplate newChar, int sizeLeft)
	{
		int i = 0;
		newChar = new CharTemplate();
		CharPtr stringVal = default(CharPtr);
		int num;
		for (; i < sizeLeft; i += num)
		{
			int value = INVALID_NUMBER;
			ref CharPtr reference = ref stringVal;
			reference = new CharPtr(null);
			CharPtr charPtr = new CharPtr(new AString(""));
			num = Get_Next_Value(readingFrom.Offset(i), charPtr, delegate(int v)
			{
				value = v;
			}, delegate(CharPtr f)
			{
				stringVal = f;
			});
			if (Next_Word_Is(charPtr, "id"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.id = (ushort)value;
				}
			}
			else if (Next_Word_Is(charPtr, "xadvance"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.xAdvance = value;
				}
			}
			else if (Next_Word_Is(charPtr, "xoffset"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.xOffset = value;
				}
			}
			else if (Next_Word_Is(charPtr, "x"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.X = value;
				}
			}
			else if (Next_Word_Is(charPtr, "yoffset"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.yOffset = value;
				}
			}
			else if (Next_Word_Is(charPtr, "y"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.Y = value;
				}
			}
			else if (Next_Word_Is(charPtr, "width"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.width = value;
				}
			}
			else if (Next_Word_Is(charPtr, "height"))
			{
				if (value != INVALID_NUMBER)
				{
					newChar.height = value;
				}
			}
			else if (Next_Word_Is(charPtr, "page") && value != INVALID_NUMBER)
			{
				newChar.page = (byte)value;
			}
			if (num < 0)
			{
				return i - num + 2;
			}
		}
		return i;
	}

	private int Parse_Page(CharPtr readingFrom, ref Page newPage, int sizeLeft)
	{
		int i = 0;
		newPage.pageName = null;
		newPage.texture = null;
		CharPtr stringVal = default(CharPtr);
		int num;
		for (; i < sizeLeft; i += num)
		{
			int value = INVALID_NUMBER;
			ref CharPtr reference = ref stringVal;
			reference = new CharPtr(null);
			CharPtr charPtr = new CharPtr(new AString(""));
			num = Get_Next_Value(readingFrom.Offset(i), charPtr, delegate(int v)
			{
				value = v;
			}, delegate(CharPtr f)
			{
				stringVal = f;
			});
			if (Next_Word_Is(charPtr, "file") && stringVal.isNotNull)
			{
				newPage.pageName = stringVal.Str;
			}
			if (num < 0)
			{
				return i - num + 2;
			}
		}
		return i;
	}

	private int Parse_Kerning(CharPtr readingFrom, ref Kerning newKern, int sizeLeft)
	{
		int i = 0;
		newKern.first = 0;
		newKern.second = 0;
		newKern.amount = 0f;
		CharPtr stringVal = default(CharPtr);
		int num;
		for (; i < sizeLeft; i += num)
		{
			int value = INVALID_NUMBER;
			ref CharPtr reference = ref stringVal;
			reference = new CharPtr(null);
			CharPtr charPtr = new CharPtr(new AString(""));
			num = Get_Next_Value(readingFrom.Offset(i), charPtr, delegate(int v)
			{
				value = v;
			}, delegate(CharPtr f)
			{
				stringVal = f;
			});
			if (Next_Word_Is(charPtr, "first"))
			{
				if (value != INVALID_NUMBER)
				{
					newKern.first = value;
				}
			}
			else if (Next_Word_Is(charPtr, "second"))
			{
				if (value != INVALID_NUMBER)
				{
					newKern.second = value;
				}
			}
			else if (Next_Word_Is(charPtr, "amount") && value != INVALID_NUMBER)
			{
				newKern.amount = value;
			}
			if (num < 0)
			{
				return i - num + 2;
			}
		}
		return i;
	}

	private CharTemplate GetCharTemplate(int charID)
	{
		return GetCharTemplate(charID, 0);
	}

	private CharTemplate GetCharTemplate(int charID, int type)
	{
		if (charID < 0)
		{
			charID = 256 + charID;
		}
		if (charID < QUICKFIND_CHARS && m_charLookUp[charID] != null)
		{
			return m_charLookUp[charID];
		}
		for (int i = 0; i < m_charTemplateCnt; i++)
		{
			if (m_charTemplates[i].id == charID)
			{
				return m_charTemplates[i];
			}
		}
		return null;
	}

	private float GetKerning(uint first, uint second)
	{
		return 0f;
	}

	private CharPtr FindAdvanceOfNextWord(CharPtr str, float fontSpaceX, float wrapWidth)
	{
		CharPtr result = str;
		float num = fontSpaceX;
		if (wrapWidth <= 0f)
		{
			return new CharPtr(null);
		}
		while (str.isNotNull && str.isNotEos && str.Val != '\n' && str.Val != ' ')
		{
			CharTemplate charTemplate = GetCharTemplate(str.Val);
			str.idx++;
			if (charTemplate != null)
			{
				num += charTemplate.xAdvance;
				if (fontSpaceX < wrapWidth * 0.75f && num >= wrapWidth)
				{
					return result;
				}
			}
		}
		if (num > wrapWidth)
		{
			return result;
		}
		return new CharPtr(null);
	}

	private float GetLineLength(string str)
	{
		return GetLineLength(str, 0f);
	}

	private float GetLineLength(string str, float wrapWidth)
	{
		return GetLineLength(str, wrapWidth, null);
	}

	private float GetLineLength(string str, float wrapWidth, Action<float> charSpacing)
	{
		return GetLineLength(new CharPtr(new AString(str)), wrapWidth, charSpacing);
	}

	private float GetLineLength(CharPtr str, float wrapWidth, Action<float> charSpacing)
	{
		charSpacing?.Invoke(0f);
		float num = 0f;
		float num2 = 0f;
		if (wrapWidth <= 0f)
		{
			while (str.isNotNull && str.isNotEos)
			{
				if (str.Val == '\n')
				{
					return num2;
				}
				CharTemplate charTemplate = GetCharTemplate(str.arr(0));
				str.idx++;
				if (charTemplate != null)
				{
					num2 += charTemplate.xAdvance;
				}
			}
		}
		else
		{
			CharPtr b = new CharPtr(null);
			while (str.isNotNull && str.isNotEos)
			{
				if (str.Val == '\n' || str.Cmp(b))
				{
					if (charSpacing != null && str.Cmp(b))
					{
						charSpacing((wrapWidth - num2) / num);
						return wrapWidth;
					}
					return num2;
				}
				CharTemplate charTemplate2 = GetCharTemplate(str.Val);
				str.idx++;
				if (b.isNull)
				{
					b = FindAdvanceOfNextWord(str, num2, wrapWidth);
					if (b.Cmp(str))
					{
						if (charSpacing != null && num > 0f && num2 > wrapWidth * 0.75f)
						{
							charSpacing((wrapWidth - num2) / num);
							return wrapWidth;
						}
						return num2;
					}
				}
				if (charTemplate2 != null)
				{
					num += 1f;
					if (charTemplate2.id == 32)
					{
						num += 2f;
					}
					num2 += charTemplate2.xAdvance;
				}
			}
		}
		return num2;
	}

	public Font()
	{
		m_charTemplateCnt = 0;
		m_charTemplates = null;
		m_pageCnt = 0;
		m_pages = null;
		m_kerningCnt = 0;
		m_kernings = null;
		for (int i = 0; i < QUICKFIND_CHARS; i++)
		{
			m_charLookUp[i] = null;
		}
		quad_verts = new VertexPositionColorTexture[QUADS_IN_BUFFER * 6];
		for (int j = 0; j < QUADS_IN_BUFFER * 6; j++)
		{
			quad_verts[j].Position.Z = 0f;
		}
	}

	public uint GetPageCount()
	{
		return (uint)m_pageCnt;
	}

	public Page GetPage(uint pageIdx)
	{
		return m_pages[pageIdx];
	}

	public uint GetCharTemplateCount()
	{
		return (uint)m_charTemplateCnt;
	}

	public CharTemplate[] GetCharTemplateArray()
	{
		return m_charTemplates;
	}

	public float GetLineHeight()
	{
		return m_lineHeight;
	}

	public float GetBaseLine()
	{
		return m_baseLine;
	}

	public void Load(string file)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		Load_Internal(file);
		stopwatch.Stop();
		TimeSpan elapsed = stopwatch.Elapsed;
		_ = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds:000}";
	}

	private void Load_Internal(string file)
	{
		string text = MortarFile.LoadText(file);
		int length = text.Length;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		FontReaderState fontReaderState = FontReaderState.Idle;
		FontReaderState_Common fontReaderState_Common = FontReaderState_Common.Idle;
		FontReaderState_Page fontReaderState_Page = FontReaderState_Page.Idle;
		FontReaderState_Chars fontReaderState_Chars = FontReaderState_Chars.Idle;
		FontReaderState_Char fontReaderState_Char = FontReaderState_Char.Idle;
		StringBuilder stringBuilder = new StringBuilder(1048576);
		int num = 1;
		int num2 = 0;
		int num3 = 0;
		Page page = default(Page);
		int num4 = 0;
		CharTemplate charTemplate = null;
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			switch (fontReaderState)
			{
			case FontReaderState.SkipLine:
				if (c == '\n')
				{
					num++;
					fontReaderState = FontReaderState.Idle;
				}
				break;
			case FontReaderState.Idle:
				switch (c)
				{
				case '\n':
					fontReaderState = FontReaderState.Idle;
					num++;
					stringBuilder.Clear();
					break;
				case '\t':
				case ' ':
				{
					string text6 = stringBuilder.ToString();
					stringBuilder.Clear();
					if (text6.Equals("char", StringComparison.CurrentCultureIgnoreCase))
					{
						fontReaderState = FontReaderState.Char;
						fontReaderState_Char = FontReaderState_Char.Idle;
						charTemplate = new CharTemplate();
					}
					else if (text6.Equals("page", StringComparison.CurrentCultureIgnoreCase))
					{
						fontReaderState = FontReaderState.Page;
						fontReaderState_Page = FontReaderState_Page.Idle;
						num3 = num2;
						page = default(Page);
					}
					else if (text6.Equals("chars", StringComparison.CurrentCultureIgnoreCase))
					{
						fontReaderState = FontReaderState.Chars;
						fontReaderState_Chars = FontReaderState_Chars.Idle;
					}
					else if (text6.Equals("common", StringComparison.CurrentCultureIgnoreCase))
					{
						fontReaderState = FontReaderState.Common;
						fontReaderState_Common = FontReaderState_Common.Idle;
					}
					else
					{
						fontReaderState = (text6.Equals("info", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState.Info : FontReaderState.SkipLine);
					}
					break;
				}
				default:
					stringBuilder.Append(c);
					break;
				}
				break;
			case FontReaderState.Info:
				if (c == '\n')
				{
					fontReaderState = FontReaderState.Idle;
					num++;
				}
				break;
			case FontReaderState.Common:
				switch (c)
				{
				case '=':
					if (fontReaderState_Common == FontReaderState_Common.Idle)
					{
						string text5 = stringBuilder.ToString();
						fontReaderState_Common = (text5.Equals("lineHeight", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.LineHeight : (text5.Equals("base", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.Base : (text5.Equals("scaleW", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.ScaleW : (text5.Equals("scaleH", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.ScaleH : (text5.Equals("pages", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.Pages : (text5.Equals("packed", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.Packed : (text5.Equals("alphaChnl", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.AlphaChnl : (text5.Equals("redChnl", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.RedChnl : (text5.Equals("greenChnl", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Common.GreenChnl : ((!text5.Equals("blueChnl", StringComparison.CurrentCultureIgnoreCase)) ? FontReaderState_Common.SkipValue : FontReaderState_Common.BlueChnl))))))))));
					}
					else
					{
						fontReaderState = FontReaderState.SkipLine;
					}
					stringBuilder.Clear();
					break;
				case '\t':
				case '\n':
				case ' ':
					switch (fontReaderState_Common)
					{
					case FontReaderState_Common.Idle:
					case FontReaderState_Common.SkipValue:
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					case FontReaderState_Common.LineHeight:
					{
						string s14 = stringBuilder.ToString();
						if (float.TryParse(s14, out var result14))
						{
							m_lineHeight = result14;
						}
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					}
					case FontReaderState_Common.Base:
					{
						string s16 = stringBuilder.ToString();
						if (float.TryParse(s16, out var result16))
						{
							m_baseLine = result16;
						}
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					}
					case FontReaderState_Common.ScaleW:
					{
						string s13 = stringBuilder.ToString();
						if (int.TryParse(s13, out var result13))
						{
							m_textureWidth = result13;
						}
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					}
					case FontReaderState_Common.ScaleH:
					{
						string s15 = stringBuilder.ToString();
						if (int.TryParse(s15, out var result15))
						{
							m_textureHeight = result15;
						}
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					}
					case FontReaderState_Common.Pages:
					{
						string s12 = stringBuilder.ToString();
						if (int.TryParse(s12, out var result12))
						{
							m_pageCnt = result12;
							m_pages = new Page[m_pageCnt];
							for (int j = 0; j < m_pageCnt; j++)
							{
								m_pages[j] = default(Page);
							}
						}
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					}
					case FontReaderState_Common.Packed:
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					case FontReaderState_Common.AlphaChnl:
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					case FontReaderState_Common.RedChnl:
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					case FontReaderState_Common.GreenChnl:
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					case FontReaderState_Common.BlueChnl:
						fontReaderState_Common = FontReaderState_Common.Idle;
						break;
					default:
						throw new Exception(string.Concat("Unhandled state ", fontReaderState_Common, "."));
					}
					if (c == '\n')
					{
						fontReaderState = FontReaderState.Idle;
						num++;
					}
					stringBuilder.Clear();
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				break;
			case FontReaderState.Page:
				switch (fontReaderState_Page)
				{
				case FontReaderState_Page.File:
					switch (c)
					{
					case '"':
						fontReaderState_Page = FontReaderState_Page.FileQuotes;
						break;
					case '\t':
					case '\n':
					case ' ':
					{
						string pageName2 = stringBuilder.ToString();
						page.pageName = pageName2;
						if (c == '\n')
						{
							num2++;
							m_pages[num3] = page;
							fontReaderState = FontReaderState.Idle;
							num++;
							stringBuilder.Clear();
						}
						break;
					}
					default:
						stringBuilder.Append(c);
						break;
					}
					break;
				case FontReaderState_Page.FileQuotes:
					switch (c)
					{
					case '\n':
						num2++;
						m_pages[num3] = page;
						fontReaderState = FontReaderState.Idle;
						num++;
						stringBuilder.Clear();
						break;
					case '"':
					{
						string pageName = stringBuilder.ToString();
						page.pageName = pageName;
						fontReaderState_Page = FontReaderState_Page.Idle;
						break;
					}
					default:
						stringBuilder.Append(c);
						break;
					}
					break;
				default:
					switch (c)
					{
					case '=':
						if (fontReaderState_Page == FontReaderState_Page.Idle)
						{
							string text4 = stringBuilder.ToString();
							fontReaderState_Page = (text4.Equals("id", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Page.Id : ((!text4.Equals("file", StringComparison.CurrentCultureIgnoreCase)) ? FontReaderState_Page.SkipValue : FontReaderState_Page.File));
						}
						else
						{
							fontReaderState = FontReaderState.SkipLine;
						}
						stringBuilder.Clear();
						break;
					case '\t':
					case '\n':
					case ' ':
						switch (fontReaderState_Page)
						{
						case FontReaderState_Page.Idle:
						case FontReaderState_Page.SkipValue:
							fontReaderState_Page = FontReaderState_Page.Idle;
							break;
						case FontReaderState_Page.Id:
						{
							string s11 = stringBuilder.ToString();
							if (int.TryParse(s11, out var result11))
							{
								num3 = result11;
							}
							fontReaderState_Page = FontReaderState_Page.Idle;
							break;
						}
						default:
							throw new Exception(string.Concat("Unhandled state ", fontReaderState_Page, "."));
						}
						if (c == '\n')
						{
							num2++;
							m_pages[num3] = page;
							fontReaderState = FontReaderState.Idle;
							num++;
						}
						stringBuilder.Clear();
						break;
					default:
						stringBuilder.Append(c);
						break;
					}
					break;
				}
				break;
			case FontReaderState.Chars:
				switch (c)
				{
				case '=':
					if (fontReaderState_Chars == FontReaderState_Chars.Idle)
					{
						string text3 = stringBuilder.ToString();
						fontReaderState_Chars = (text3.Equals("count", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Chars.Count : FontReaderState_Chars.SkipValue);
					}
					else
					{
						fontReaderState = FontReaderState.SkipLine;
					}
					stringBuilder.Clear();
					break;
				case '\t':
				case '\n':
				case ' ':
					switch (fontReaderState_Chars)
					{
					case FontReaderState_Chars.Idle:
					case FontReaderState_Chars.SkipValue:
						fontReaderState_Chars = FontReaderState_Chars.Idle;
						break;
					case FontReaderState_Chars.Count:
					{
						string s10 = stringBuilder.ToString();
						if (int.TryParse(s10, out var result10))
						{
							m_charTemplateCnt = result10;
							m_charTemplates = new CharTemplate[m_charTemplateCnt];
						}
						fontReaderState_Chars = FontReaderState_Chars.Idle;
						break;
					}
					default:
						throw new Exception(string.Concat("Unhandled state ", fontReaderState_Chars, "."));
					}
					if (c == '\n')
					{
						fontReaderState = FontReaderState.Idle;
						num++;
					}
					stringBuilder.Clear();
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				break;
			case FontReaderState.Char:
				switch (c)
				{
				case '=':
					if (fontReaderState_Char == FontReaderState_Char.Idle)
					{
						string text2 = stringBuilder.ToString();
						fontReaderState_Char = (text2.Equals("id", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.Id : (text2.Equals("x", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.X : (text2.Equals("y", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.Y : (text2.Equals("width", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.Width : (text2.Equals("height", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.Height : (text2.Equals("xoffset", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.XOffset : (text2.Equals("yoffset", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.YOffset : (text2.Equals("xadvance", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.XAdvance : (text2.Equals("page", StringComparison.CurrentCultureIgnoreCase) ? FontReaderState_Char.Page : ((!text2.Equals("chnl", StringComparison.CurrentCultureIgnoreCase)) ? FontReaderState_Char.SkipValue : FontReaderState_Char.Chnl))))))))));
					}
					else
					{
						fontReaderState = FontReaderState.SkipLine;
					}
					stringBuilder.Clear();
					break;
				case '\t':
				case '\n':
				case ' ':
					switch (fontReaderState_Char)
					{
					case FontReaderState_Char.Idle:
					case FontReaderState_Char.SkipValue:
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					case FontReaderState_Char.Id:
					{
						string s4 = stringBuilder.ToString();
						if (int.TryParse(s4, out var result4))
						{
							charTemplate.id = (ushort)result4;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.X:
					{
						string s2 = stringBuilder.ToString();
						if (float.TryParse(s2, out var result2))
						{
							charTemplate.X = result2 / (float)m_textureWidth;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.Y:
					{
						string s8 = stringBuilder.ToString();
						if (float.TryParse(s8, out var result8))
						{
							charTemplate.Y = result8 / (float)m_textureHeight;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.Width:
					{
						string s7 = stringBuilder.ToString();
						if (float.TryParse(s7, out var result7))
						{
							charTemplate.width = result7 / m_lineHeight;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.Height:
					{
						string s5 = stringBuilder.ToString();
						if (float.TryParse(s5, out var result5))
						{
							charTemplate.height = result5 / m_lineHeight;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.XOffset:
					{
						string s9 = stringBuilder.ToString();
						if (float.TryParse(s9, out var result9))
						{
							charTemplate.xOffset = result9 / m_lineHeight;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.YOffset:
					{
						string s6 = stringBuilder.ToString();
						if (float.TryParse(s6, out var result6))
						{
							charTemplate.yOffset = result6 / m_lineHeight;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.XAdvance:
					{
						string s3 = stringBuilder.ToString();
						if (float.TryParse(s3, out var result3))
						{
							charTemplate.xAdvance = result3 / m_lineHeight;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.Page:
					{
						string s = stringBuilder.ToString();
						if (byte.TryParse(s, out var result))
						{
							charTemplate.page = result;
						}
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					}
					case FontReaderState_Char.Chnl:
						fontReaderState_Char = FontReaderState_Char.Idle;
						break;
					default:
						throw new Exception(string.Concat("Unhandled state ", fontReaderState_Page, "."));
					}
					if (c == '\n')
					{
						if (charTemplate.id < QUICKFIND_CHARS)
						{
							m_charLookUp[charTemplate.id] = charTemplate;
						}
						m_charTemplates[num4] = charTemplate;
						charTemplate = null;
						num4++;
						fontReaderState = FontReaderState.Idle;
						num++;
					}
					stringBuilder.Clear();
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				break;
			}
		}
		stopwatch.Stop();
		TimeSpan elapsed = stopwatch.Elapsed;
		_ = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds:000}";
		string fileName = Path.GetFileName(file);
		string text7 = file.Substring(0, file.Length - fileName.Length);
		for (int k = 0; k < m_pageCnt; k++)
		{
			m_pages[k].texture = Texture.Load(text7 + m_pages[k].pageName);
		}
		m_baseLine /= m_lineHeight;
	}

	public void DrawString(string stringToDraw, Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, pos, Color.White);
	}

	public void DrawString(string stringToDraw, Vector3 pos, Color colour)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, pos, colour, 1f);
	}

	public void DrawString(string stringToDraw, Vector3 pos, Color colour, float scale)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, pos, colour, scale, Vector2.Zero);
	}

	public void DrawString(string stringToDraw, Vector3 pos, Color colour, float scale, Vector2 wrap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, pos, colour, scale, wrap, ALIGNMENT_TYPE.ALIGN_LEFT);
	}

	public void DrawString(string stringToDraw, Vector3 pos, Color colour, float scale, Vector2 wrap, ALIGNMENT_TYPE alignType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, pos, colour, scale, wrap, alignType, 1f);
	}

	public void DrawString(string _stringToDraw, Vector3 pos, Color colour, float scale, Vector2 wrap, ALIGNMENT_TYPE alignType, float vDir)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(_stringToDraw, pos, colour, scale, wrap, alignType, vDir, null);
	}

	public void DrawString(string stringToDraw, float x, float y)
	{
		DrawString(stringToDraw, x, y, 0f);
	}

	public void DrawString(string stringToDraw, float x, float y, float z)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, x, y, z, Color.White);
	}

	public void DrawString(string stringToDraw, float x, float y, float z, Color colour)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, x, y, z, colour, 1f);
	}

	public void DrawString(string stringToDraw, float x, float y, float z, Color colour, float scale)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, x, y, z, colour, scale, 0f);
	}

	public void DrawString(string stringToDraw, float x, float y, float z, Color colour, float scale, float wrapWidth)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, x, y, z, colour, scale, wrapWidth, 0f);
	}

	public void DrawString(string stringToDraw, float x, float y, float z, Color colour, float scale, float wrapWidth, float textBoxHeight)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, x, y, z, colour, scale, wrapWidth, textBoxHeight, ALIGNMENT_TYPE.ALIGN_LEFT);
	}

	public void DrawString(string stringToDraw, float x, float y, float z, Color colour, float scale, float wrapWidth, float textBoxHeight, ALIGNMENT_TYPE alignType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		DrawString(stringToDraw, new Vector3(x, y, z), colour, scale, new Vector2(wrapWidth, textBoxHeight), alignType);
	}

	public void DrawString(string _stringToDraw, Vector3 pos, Color colour, float scale, Vector2 wrap, ALIGNMENT_TYPE alignType, float vDir, MortarRectangleDec? rect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		DrawString(_stringToDraw, pos, colour, new Vector2(scale, scale), wrap, alignType, vDir, rect);
	}

	public void DrawString(string stringToDraw, Vector3 pos, Color colour, Vector2 scale, Vector2 wrap, ALIGNMENT_TYPE alignType, float vDir, MortarRectangleDec? rect)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		wrap.X /= scale.X;
		wrap.Y /= vDir * scale.Y;
		MatrixManager.instance.Reset();
		MatrixManager.instance.Scale(new Vector3(scale.X, scale.Y, 1f));
		MatrixManager.instance.TranslateGlobal(new Vector3(pos.X, pos.Y, pos.Z));
		byte b = 0;
		m_pages[b].texture.Set();
		float num = 0f;
		float num2 = 0f;
		int iCurrentLineStart = 0;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < stringToDraw.Length; i++)
		{
			char c = stringToDraw[i];
			CharTemplate charTemplate = GetCharTemplate(c);
			if (charTemplate == null)
			{
				continue;
			}
			float num5 = num3 + charTemplate.xAdvance;
			num = MathHelper.Max(num, num5);
			bool flag = wrap.X > 0f && num5 > wrap.X;
			if (c == '\n' || flag)
			{
				if (flag)
				{
					float fLineWidth = num5;
					SearchForSpaceForWordWrap(stringToDraw, ref iCurrentLineStart, ref i, ref fLineWidth, out var _);
					num = MathHelper.Max(num, fLineWidth);
				}
				num5 = 0f;
				num4 += vDir;
				iCurrentLineStart = i + 1;
			}
			num3 = num5;
		}
		num2 = num4 + vDir;
		Vector3 zero = Vector3.Zero;
		if ((alignType & ALIGNMENT_TYPE.ALIGN_VCENTER) == ALIGNMENT_TYPE.ALIGN_VCENTER)
		{
			zero.Y = (0f - num2) * 0.5f;
		}
		else if ((alignType & ALIGNMENT_TYPE.ALIGN_TOP) == ALIGNMENT_TYPE.ALIGN_TOP)
		{
			zero.Y = 0f;
		}
		else
		{
			zero.Y = 0f - num2;
		}
		MatrixManager.instance.TranslateLocal(zero);
		float fLineWidth2 = 0f;
		int iCurrentLineStart2 = 0;
		int iCurrentLineEnd2 = stringToDraw.Length;
		float num6 = 0f;
		float num7 = 0f;
		for (int j = 0; j < stringToDraw.Length; j++)
		{
			char c2 = stringToDraw[j];
			CharTemplate charTemplate2 = GetCharTemplate(c2);
			if (charTemplate2 == null)
			{
				continue;
			}
			float num8 = num6 + charTemplate2.xAdvance;
			fLineWidth2 = num8;
			bool flag2 = wrap.X > 0f && num8 > wrap.X;
			if (c2 == '\n' || flag2)
			{
				if (flag2)
				{
					SearchForSpaceForWordWrap(stringToDraw, ref iCurrentLineStart2, ref j, ref fLineWidth2, out iCurrentLineEnd2);
				}
				DrawLine(stringToDraw, iCurrentLineStart2, iCurrentLineEnd2, 0f, num2 - num7, alignType, fLineWidth2, ref wrap, b, ref colour, vDir);
				num8 = 0f;
				num7 += vDir;
				iCurrentLineStart2 = iCurrentLineEnd2;
				iCurrentLineEnd2 = stringToDraw.Length;
			}
			num6 = num8;
		}
		if (iCurrentLineStart2 != iCurrentLineEnd2)
		{
			DrawLine(stringToDraw, iCurrentLineStart2, iCurrentLineEnd2, 0f, num2 - num7, alignType, fLineWidth2, ref wrap, b, ref colour, vDir);
		}
	}

	private void SearchForSpaceForWordWrap(string stringToDraw, ref int iCurrentLineStart, ref int iChar, ref float fLineWidth, out int iCurrentLineEnd)
	{
		iCurrentLineEnd = iChar + 1;
		int num = -1;
		for (int num2 = iCurrentLineEnd - 1; num2 > iCurrentLineStart; num2--)
		{
			char c = stringToDraw[num2];
			if (c == ' ')
			{
				num = num2;
				break;
			}
		}
		if (num <= -1)
		{
			return;
		}
		iChar = num;
		iCurrentLineEnd = iChar + 1;
		fLineWidth = 0f;
		for (int i = iCurrentLineStart; i < iCurrentLineEnd; i++)
		{
			char charID = stringToDraw[i];
			CharTemplate charTemplate = GetCharTemplate(charID);
			if (charTemplate != null)
			{
				fLineWidth += charTemplate.xAdvance;
			}
		}
	}

	private void DrawLine(string stringToDraw, int iCurrentLineStart, int iCurrentLineEnd, float fCurrentX, float fCurrentY, ALIGNMENT_TYPE alignType, float fLineWidth, ref Vector2 wrap, byte iCurrentFontPage, ref Color colour, float vDir)
	{
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		byte b = iCurrentFontPage;
		float num2 = fCurrentX;
		float num3 = m_lineHeight / (float)m_textureHeight;
		float num4 = m_lineHeight / (float)m_textureWidth;
		int num5 = iCurrentLineEnd - iCurrentLineStart;
		float num6 = 0f;
		float num7 = 0f;
		switch (alignType & ALIGNMENT_TYPE.ALIGN_HCENTER)
		{
		case ALIGNMENT_TYPE.ALIGN_LEFT:
			num7 = 0f;
			break;
		case ALIGNMENT_TYPE.ALIGN_RIGHT:
			num7 = wrap.X - fLineWidth;
			break;
		case ALIGNMENT_TYPE.ALIGN_HCENTER:
			num7 = (wrap.X - fLineWidth) * 0.5f;
			break;
		}
		if ((alignType & ALIGNMENT_TYPE.ALIGN_JUSTIFY) != 0)
		{
			num6 = (wrap.X - fLineWidth) / (float)num5;
		}
		for (int i = iCurrentLineStart; i < iCurrentLineEnd; i++)
		{
			char charID = stringToDraw[i];
			CharTemplate charTemplate = GetCharTemplate(charID);
			if (charTemplate == null)
			{
				continue;
			}
			if (charTemplate != null)
			{
				b = charTemplate.page;
				if (b != iCurrentFontPage)
				{
					if (num > 0)
					{
						MatrixManager.instance.UploadCurrentMatrices(quick: true);
						Mesh.DrawTriStripEx(quad_verts, num * 6, hasAlpha: true);
						num = 0;
					}
					m_pages[b].texture.Set();
					iCurrentFontPage = b;
				}
			}
			float num8 = charTemplate.width * num4;
			float num9 = charTemplate.height * num3;
			float num10 = num2 + charTemplate.xOffset + num7;
			float num11 = fCurrentY - vDir * charTemplate.yOffset;
			int num12 = num * 6;
			quad_verts[num12].Position = new Vector3(num10, num11 - vDir * charTemplate.height, 0f);
			quad_verts[num12].TextureCoordinate = new Vector2(charTemplate.X, charTemplate.Y + num9);
			if (num12 != 0)
			{
				quad_verts[num12 - 1].Position = quad_verts[num12].Position;
			}
			quad_verts[num12 + 1].Position = new Vector3(num10, num11, 0f);
			quad_verts[num12 + 1].TextureCoordinate = new Vector2(charTemplate.X, charTemplate.Y);
			quad_verts[num12 + 2].Position = new Vector3(num10 + charTemplate.width, num11 - vDir * charTemplate.height, 0f);
			quad_verts[num12 + 2].TextureCoordinate = new Vector2(charTemplate.X + num8, charTemplate.Y + num9);
			quad_verts[num12 + 3].Position = new Vector3(num10 + charTemplate.width, num11, 0f);
			quad_verts[num12 + 3].TextureCoordinate = new Vector2(charTemplate.X + num8, charTemplate.Y);
			quad_verts[num12 + 4].Position = quad_verts[num12 + 3].Position;
			quad_verts[num12].Color = colour;
			quad_verts[num12 + 1].Color = colour;
			quad_verts[num12 + 2].Color = colour;
			quad_verts[num12 + 3].Color = colour;
			quad_verts[num12 + 4].Color = colour;
			quad_verts[num12 + 5].Color = colour;
			num2 += charTemplate.xAdvance + num6 * ((charTemplate.id == 32) ? 3f : 1f);
			num++;
			if (num == QUADS_IN_BUFFER)
			{
				MatrixManager.instance.UploadCurrentMatrices(quick: true);
				Mesh.DrawTriStripEx(quad_verts, num * 6, hasAlpha: true);
				num = 0;
			}
		}
		if (num > 0)
		{
			MatrixManager.instance.UploadCurrentMatrices(quick: true);
			Mesh.DrawTriStripEx(quad_verts, num * 6, hasAlpha: true);
			num = 0;
		}
	}

	public float MeasureString(string stringToDraw)
	{
		return GetLineLength(stringToDraw);
	}
}
