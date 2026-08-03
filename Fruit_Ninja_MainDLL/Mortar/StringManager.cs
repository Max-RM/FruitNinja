namespace Mortar;

internal class StringManager
{
	public static StringTable[] tables;

	public string defaultLanguage;

	public static StringManager instance;

	public static StringManager GetInstance()
	{
		if (instance == null)
		{
			instance = new StringManager();
		}
		return instance;
	}

	private StringManager()
	{
		defaultLanguage = "english_us";
	}

	public void Init(int numTables)
	{
		tables = new StringTable[numTables];
	}

	public void LoadTable(string filename, int idx)
	{
		tables[idx] = new StringTable();
		tables[idx].LoadHeader(filename);
	}

	public void UnloadTable(int idx)
	{
		tables[idx] = null;
	}

	public void UnloadAll()
	{
		tables = new StringTable[tables.Length];
	}

	public void SetDefaultLanguage(string lng)
	{
		defaultLanguage = lng;
	}
}
