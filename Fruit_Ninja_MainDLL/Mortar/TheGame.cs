using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using FNWP72.Engine;
using FruitNinja;
using FruitNinja.Port;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Game = FruitNinja.Game;
using Microsoft.Phone.Tasks;

namespace Mortar;

public class TheGame : Microsoft.Xna.Framework.Game
{
	public class Analytics
	{
		public int games_trial;

		public int games_full;

		public int games_classic;

		public int games_arcade;

		public int games_zen;

		public int hour;

		public int min;

		public int week;

		public int games_week;

		private string titlesPlayed = "Unknown";

		private string totalAchievements = "Unknown";

		private string region = "Unknown";

		public void Load()
		{
			try
			{
				if (!DesktopStorage.DirectoryExists(CFG_DIRECTORY))
				{
					DesktopStorage.CreateDirectory(CFG_DIRECTORY);
				}
				byte[] array = DesktopStorage.ReadAllBytes(ANA_FILENAME);
				if (array != null && array.Length > 0)
				{
					int num2 = 0;
					games_trial = BitConverter.ToInt32(array, num2);
					num2 += 4;
					games_full = BitConverter.ToInt32(array, num2);
					num2 += 4;
					games_classic = BitConverter.ToInt32(array, num2);
					num2 += 4;
					games_arcade = BitConverter.ToInt32(array, num2);
					num2 += 4;
					games_zen = BitConverter.ToInt32(array, num2);
					num2 += 4;
					hour = DateTime.Now.Hour;
					num2 += 4;
					min = DateTime.Now.Minute;
					num2 += 4;
					week = BitConverter.ToInt32(array, num2);
					num2 += 4;
					games_week = BitConverter.ToInt32(array, num2);
					num2 += 4;
					int num3 = DateTime.Now.DayOfYear / 7;
					if (week != num3)
					{
						week = num3;
						games_week = 0;
					}
					games_week++;
					DebugPrint();
				}
				else
				{
					Save();
				}
			}
			catch
			{
			}
		}

		public void Save()
		{
			try
			{
				if (!DesktopStorage.DirectoryExists(CFG_DIRECTORY))
				{
					DesktopStorage.CreateDirectory(CFG_DIRECTORY);
				}
				using (var stream = DesktopStorage.OpenFile(ANA_FILENAME, FileMode.Create))
				{
					byte[] bytes = BitConverter.GetBytes(games_trial);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(games_full);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(games_classic);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(games_arcade);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(games_zen);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(hour);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(min);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(week);
					stream.Write(bytes, 0, 4);
					bytes = BitConverter.GetBytes(games_week);
					stream.Write(bytes, 0, 4);
				}
			}
			catch
			{
			}
		}

		public IDictionary<string, string> GetAnalyticsData()
		{
			int num = games_classic + games_arcade + games_zen;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Version", Game.isWP7TrialMode() ? "Trial" : "Full");
			dictionary.Add("TrialGames", games_trial.ToString());
			dictionary.Add("FullGames", games_full.ToString());
			dictionary.Add("ClassicTotal", games_classic.ToString());
			dictionary.Add("ArcadeTotal", games_arcade.ToString());
			dictionary.Add("ZenTotal", games_zen.ToString());
			dictionary.Add("ClassicPercent", (num > 0) ? (PercentageOfTotal(games_classic, num) + "%") : "0%");
			dictionary.Add("ArcadePercent", (num > 0) ? (PercentageOfTotal(games_arcade, num) + "%") : "0%");
			dictionary.Add("ZenPercent", (num > 0) ? (PercentageOfTotal(games_zen, num) + "%") : "0%");
			dictionary.Add("Time", hour.ToString("00") + ":" + min.ToString("00"));
			dictionary.Add("Weekly", week.ToString("00") + "/" + games_week);
			dictionary.Add("TitlesPlayed", titlesPlayed);
			dictionary.Add("TotalAchievements", totalAchievements);
			dictionary.Add("Region", region);
			return dictionary;
		}

		private int PercentageOfTotal(int amount, int total)
		{
			float num = amount;
			float num2 = total;
			float num3 = num / num2 * 100f;
			return (int)num3;
		}

		public void DebugPrint()
		{
		}

		public void SetPrivileged()
		{
		}
	}

	public class Settings
	{
		public int magic1;

		public int magic2;

		public int language;

		public int week;

		public int tf;

		public int aw;

		public int bestThisWeek;

		public uint[] achievements;

		public Settings()
		{
			magic1 = 195800833;
			magic2 = 33479857;
			language = 1;
			week = -1;
			tf = 0;
			aw = 0;
			bestThisWeek = 0;
			achievements = new uint[Game.MAX_SYSTEM_ACHIEVEMENTS];
			for (int i = 0; i < Game.MAX_SYSTEM_ACHIEVEMENTS; i++)
			{
				achievements[i] = 0u;
			}
		}
	}

	public struct inputInfo
	{
		public int updateStamp;

		public uint tid;

		public bool dn;

		public float x;

		public float y;
	}

	public class MyTouchState
	{
		public Vector2 lastPoint;

		public Vector2 midPoint1;

		public Vector2 midPoint2;

		public Vector2 midPoint3;

		public Vector2 movedToPoint;

		public int id;

		public bool MovedThisFrame;

		public bool deletedThisFrame;

		public int moves;
	}

	private const int MAGIC1 = 195800833;

	private const int MAGIC2 = 33479857;

	private static string CFG_DIRECTORY = "FruitNinja";

	private static string CFG_FILENAME = CFG_DIRECTORY + "/setting.dat";

	private static string ANA_FILENAME = CFG_DIRECTORY + "/analytics.dat";

	public static Settings settings = new Settings();

	public static Analytics analytics = new Analytics();

	public static Microsoft.Phone.Tasks.SaveRingtoneTask srtt;

	public GraphicsDeviceManager graphics;

	public SpriteBatch spriteBatch;

	public SpriteFont font1;

	public SpriteFont font2;

	public SpriteFont font3;

	public SpriteFont font4;

	public StringTable stringTable;

	public static Texture2D picture;

	public static TheGame instance;

	public static FruitGame fgame;

	public static bool emulator = false;

	public static bool exceptionThrown = false;

	public static bool exitingGame = false;

	public static bool logInSucceeded = false;

	public static bool pomegranateThrown = false;

	private static uint error_code = 2415923200u;

	private bool init_failed;

	private bool load_failed;

	private bool quit_game;

	private static bool setup_attribute = true;

	private static Thread setup_thread = null;

	private static ConnectionAcquire ca = null;

	private static int pe_count = 0;

	private Texture hbLogoTexture;

	private Texture2D bobsCousinIt;

	public static Texture2D bobsCousinHairyMaclary;

	private Texture upsell;

	private Texture menu_purchase;

	private Texture menu_exit;

	private float timeout = 5.5f;

	private bool fadeout;

	private bool fadein;

	private FadeManager fm;

	private bool showUpsell;

	private bool exit_from_upsell;

	private Vector2 pos1;

	private Vector2 pos2;

	public static int initupdates = 5;

	public LinkedList<MyTouchState> tstate = new LinkedList<MyTouchState>();

	private static float dtCo = 1f;

	private static bool sb = false;

	private static int sbc = 0;

	private bool _BackButtonWasPressed;

	public bool BackButtonWasPressed;

	private bool backButtonDownLast;

	private bool gameUpdateRequired;

	private bool curr_down;

	private bool prev_down;

	private int uid = -1;

	private int tx;

	private int ty;

	private bool inExit;

	private bool inPurchase;

	private static bool ___isActiveSim = true;

	public static bool disableNetworkCalls = false;

	private GamePadState state_prev;

	private GamePadState state_curr;

	private MouseState _prevMouseState;

	private int skip;

	private int updates;

	public static bool switchLanguage = false;

	public static StringTableUtils.Language switchToLanguage;

	public static float loadinScreen = 0f;

	private RenderTarget2D _gameRenderTarget;

	private static Texture2D m_fruitTex;

	private static Texture2D m_ninjaTex;

	private static Texture2D m_headTex;

	private static Texture2D m_bodyTex;

	private static bool ts = false;

	private string ERROR_BUTTON_1 => instance.stringTable.GetString(927);

	private string ERROR_TITLE_1 => instance.stringTable.GetString(1143);

	public static bool TriggerShowBuyMessageBox
	{
		set
		{
			if (value && Game.isWP7TrialMode())
			{
				sb = value;
				sbc = 40;
			}
		}
	}

	public static void LoadConfig()
	{
		try
		{
			if (!DesktopStorage.DirectoryExists(CFG_DIRECTORY))
			{
				DesktopStorage.CreateDirectory(CFG_DIRECTORY);
			}
			byte[] array = DesktopStorage.ReadAllBytes(CFG_FILENAME);
			if (array != null && array.Length > 0)
			{
				int num = array.Length;
				if (num == 28 + Game.MAX_SYSTEM_ACHIEVEMENTS * 4)
				{
					int num2 = BitConverter.ToInt32(array, 0);
					int num3 = BitConverter.ToInt32(array, 4);
					int num4 = BitConverter.ToInt32(array, 8);
					int week = BitConverter.ToInt32(array, 12);
					int tf = BitConverter.ToInt32(array, 16);
					int aw = BitConverter.ToInt32(array, 20);
					int bestThisWeek = BitConverter.ToInt32(array, 24);
					if (num2 == 195800833 && num3 == 33479857 && num4 >= 0 && num4 <= 5)
					{
						settings.language = num4;
						settings.week = week;
						settings.tf = tf;
						settings.aw = aw;
						settings.bestThisWeek = bestThisWeek;
						Game.game_work.SetLanguage((StringTableUtils.Language)num4);
						int num5 = 28;
						for (int i = 0; i < Game.MAX_SYSTEM_ACHIEVEMENTS; i++)
						{
							settings.achievements[i] = BitConverter.ToUInt32(array, num5);
							num5 += 4;
						}
					}
					else
					{
						SaveConfig();
					}
				}
				else
				{
					SaveConfig();
				}
			}
			else
			{
				SaveConfig();
			}
		}
		catch
		{
		}
	}

	public static void SaveConfig()
	{
		try
		{
			if (!DesktopStorage.DirectoryExists(CFG_DIRECTORY))
			{
				DesktopStorage.CreateDirectory(CFG_DIRECTORY);
			}
			using (var stream = DesktopStorage.OpenFile(CFG_FILENAME, FileMode.Create))
			{
				settings.language = (int)Game.game_work.language;
				byte[] bytes = BitConverter.GetBytes(settings.magic1);
				byte[] bytes2 = BitConverter.GetBytes(settings.magic2);
				byte[] bytes3 = BitConverter.GetBytes(settings.language);
				byte[] bytes4 = BitConverter.GetBytes(settings.week);
				byte[] bytes5 = BitConverter.GetBytes(settings.tf);
				byte[] bytes6 = BitConverter.GetBytes(settings.aw);
				byte[] bytes7 = BitConverter.GetBytes(settings.bestThisWeek);
				stream.Write(bytes, 0, 4);
				stream.Write(bytes2, 0, 4);
				stream.Write(bytes3, 0, 4);
				stream.Write(bytes4, 0, 4);
				stream.Write(bytes5, 0, 4);
				stream.Write(bytes6, 0, 4);
				stream.Write(bytes7, 0, 4);
				for (int i = 0; i < Game.MAX_SYSTEM_ACHIEVEMENTS; i++)
				{
					byte[] bytes8 = BitConverter.GetBytes(settings.achievements[i]);
					stream.Write(bytes8, 0, 4);
				}
			}
		}
		catch
		{
		}
	}

	private static bool ReadIniBool(string key, bool defaultValue)
	{
		try
		{
			var path = System.IO.Path.Combine(AppContext.BaseDirectory, "FruitNinja.ini");
			if (System.IO.File.Exists(path))
			{
				foreach (var line in System.IO.File.ReadAllLines(path))
				{
					var trimmed = line.Trim();
					if (trimmed.StartsWith(";") || trimmed.StartsWith("#"))
						continue;
					var parts = trimmed.Split('=', 2);
					if (parts.Length == 2 && parts[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
					{
						var val = parts[1].Trim().ToLowerInvariant();
						return val == "1" || val == "true" || val == "yes";
					}
				}
			}
		}
		catch { }
		return defaultValue;
	}

	private static string ERROR_MESSAGE(uint code)
	{
		return $"Error 0x{code:x} occurred. Fruit Ninja cannot continue and needs to exit.";
	}

	private static string ERROR_MESSAGE_GAME()
	{
		return $"Error 0x{GetErrorCode():x} occurred. Please check your memory usage before continuing.";
	}

	public static void SetErrorCode(uint code)
	{
		error_code = code;
	}

	public static uint GetErrorCode()
	{
		return error_code;
	}

	private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
	}

	public TheGame()
	{
		instance = this;
		graphics = new GraphicsDeviceManager((Microsoft.Xna.Framework.Game)(object)this);
		graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
		((Microsoft.Xna.Framework.Game)this).Content.RootDirectory = "Content";
		((Microsoft.Xna.Framework.Game)this).TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
		graphics.PreferredBackBufferWidth = 800;
		graphics.PreferredBackBufferHeight = 480;
		graphics.IsFullScreen = false;
		graphics.SupportedOrientations = (DisplayOrientation)3;
		graphics.ApplyChanges();
		((Microsoft.Xna.Framework.Game)this).Window.AllowUserResizing = true;
		((Microsoft.Xna.Framework.Game)this).IsMouseVisible = ReadIniBool("ShowCursor", false);
		fgame = new FruitGame();
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		StringTableUtils.Language language = StringTableUtils.ParseLanguageFromCulture(currentCulture.Name);
		Game.game_work.SetLanguage(language);
		Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		try
		{
			if (!DesktopStorage.FileExists(CFG_FILENAME))
			{
				SaveConfig();
			}
		}
		catch
		{
		}
		stringTable = new StringTable();
	}

	public static IDictionary<string, string> MGSStatistics_Setup()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Trial", Game.isWP7TrialMode().ToString());
		dictionary.Add("Tombstone", "True");
		dictionary.Add("Connection", ca?.GetInterfaceType() ?? "Unknown");
		dictionary.Add("Manufacturer", "Desktop");
		dictionary.Add("Model", Environment.OSVersion.ToString());
		dictionary.Add("FirmwareVersion", "1.0");
		dictionary.Add("HardwareVersion", "1.0");
		dictionary.Add("Memory", "0");
		dictionary.Add("Locale", CultureInfo.CurrentUICulture.Name);
		dictionary.Add("UtcBaseOffset", TimeZoneInfo.Local.BaseUtcOffset.TotalHours.ToString());
		dictionary.Add("UtcCurrentOffset", TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours.ToString());
		dictionary.Add("Experiment", "");
		return dictionary;
	}

	public static IDictionary<string, string> MGSStatistics_Teardown()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Trial", ts.ToString());
		return dictionary;
	}

	public static IDictionary<string, string> Halfbrick_Analytics()
	{
		return analytics.GetAnalyticsData();
	}

	public static void PE_SetupAttributeMGS(string Trial, string Tombstone, string Connection, string Manufacturer, string Model, string FirmwareVersion, string HardwareVersion, string Memory, string Locale, string UtcBaseOffset, string UtcCurrentOffset, string Experiment)
	{
		pe_count++;
	}

	public static void PE_SetupAttribute()
	{
		pe_count++;
	}

	public static void PE_TeardownAttribute()
	{
		pe_count++;
	}

	public static void PE_ShowMarketplace()
	{
		pe_count++;
	}

	public static void PE_AchievementAwarded(string name)
	{
		pe_count++;
	}

	public static void PE_GameMode(string name)
	{
		if (Game.isWP7TrialMode())
		{
			analytics.games_trial++;
		}
		else
		{
			analytics.games_full++;
		}
		switch (name)
		{
		case "Classic":
			analytics.games_classic++;
			break;
		case "Arcade":
			analytics.games_arcade++;
			break;
		case "Zen":
			analytics.games_zen++;
			break;
		}
		pe_count++;
	}

	public static void PE_GameInformation()
	{
		pe_count++;
	}

	protected override void Initialize()
	{
		try
		{
			LoadConfig();
			analytics.Load();
			stringTable.LoadHeader("stringtables/translations");
			stringTable.LoadAllLanguages();
			SwitchToLanguage(Game.game_work.language);
		}
		catch
		{
			exitingGame = true;
			((Microsoft.Xna.Framework.Game)this).Exit();
		}
		try
		{
			emulator = true;
			base.Initialize();
		}
		catch (Exception ex)
		{
			for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
			{
			}
			string text = ERROR_MESSAGE(2147487744u);
			init_failed = true;
			string[] array = new string[1] { ERROR_BUTTON_1 };
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			if (text.Length > 255)
			{
				text = text.Substring(0, 250);
			}
			Guide.BeginShowMessageBox(ERROR_TITLE_1, text, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)InitOrLoadFailedCallback, (object)null);
		}
	}

	private void InitOrLoadFailedCallback(IAsyncResult result)
	{
		quit_game = true;
	}

	private void IngameException(IAsyncResult result)
	{
		if (GameTask.GameReset(0u))
		{
			Game.QuitToMenu();
		}
		else
		{
			quit_game = true;
		}
	}

	public void DoUpsell(bool exitGame)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (Game.isWP7TrialMode() && !showUpsell)
		{
			exit_from_upsell = exitGame;
			showUpsell = true;
			pos1 = new Vector2((float)(800 - (menu_exit.intex.Width + 16)), (float)(480 - (menu_exit.intex.Height + 20)));
			pos2 = new Vector2(pos1.X - (float)(menu_purchase.intex.Width + 16), pos1.Y);
			GameTask.SkipToPause(atStart: false);
		}
	}

	public bool UpsellComplete()
	{
		return !showUpsell;
	}

	protected override void LoadContent()
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		try
		{
			Game.SetTrialModeState();
			hbLogoTexture = Texture.Load(MTLocalisation.GetLocalisedTexturePath() + "/HB_logo.tex");
			bobsCousinIt = ((Microsoft.Xna.Framework.Game)this).Content.Load<Texture2D>("extra/MGS_WP7_Horiz_Still");
			bobsCousinHairyMaclary = ((Microsoft.Xna.Framework.Game)this).Content.Load<Texture2D>("extra/black");
			upsell = TextureManager.GetInstance().Load("trial_upsell.tex", localise: true);
			menu_purchase = TextureManager.GetInstance().Load("menu_purchase.tex", localise: true);
			menu_exit = TextureManager.GetInstance().Load("quit_title.tex", localise: true);
			fm = new FadeManager(bobsCousinHairyMaclary, bobsCousinHairyMaclary);
			((Microsoft.Xna.Framework.Game)instance).GraphicsDevice.Clear((ClearOptions)3, Color.Black, 1f, 0);
			spriteBatch = new SpriteBatch(((Microsoft.Xna.Framework.Game)this).GraphicsDevice);
			font1 = ((Microsoft.Xna.Framework.Game)this).Content.Load<SpriteFont>("extra/mainFont");
			font2 = ((Microsoft.Xna.Framework.Game)this).Content.Load<SpriteFont>("extra/detailFont");
			font3 = ((Microsoft.Xna.Framework.Game)this).Content.Load<SpriteFont>("extra/titleFont");
			font4 = ((Microsoft.Xna.Framework.Game)this).Content.Load<SpriteFont>("extra/titleFontSmall");
		}
		catch (Exception ex)
		{
			for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
			{
			}
			string text = ERROR_MESSAGE(2147491840u);
			load_failed = true;
			string[] array = new string[1] { ERROR_BUTTON_1 };
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			if (text.Length > 255)
			{
				text = text.Substring(0, 250);
			}
			Guide.BeginShowMessageBox(ERROR_TITLE_1, text, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)InitOrLoadFailedCallback, (object)null);
		}
	}

	protected override void UnloadContent()
	{
	}

	private void NotConnected(IAsyncResult result)
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (updates > 0)
		{
			return;
		}
		updates++;
		float timeSinceLastUpdate = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;
		if (!((Microsoft.Xna.Framework.Game)this).IsActive || !((Microsoft.Xna.Framework.Game)instance).IsActive)
		{
			return;
		}
		if (skip > 0)
		{
			skip--;
			return;
		}
		if (gameUpdateRequired)
		{
			gameUpdateRequired = false;
			HandleGameUpdateException();
			return;
		}
		if (sb)
		{
			sbc--;
			if (sbc <= 0)
			{
				DoUpsell(exitGame: false);
				sb = false;
				sbc = 0;
				return;
			}
		}
		_BackButtonWasPressed = false;
		GamePadState state = GamePad.GetState((PlayerIndex)0);
		GamePadButtons buttons = state.Buttons;
		bool flag = (int)buttons.Back == 1;
		if (!flag && backButtonDownLast)
		{
			BackButtonWasPressed = true;
		}
		backButtonDownLast = flag;
		if (instance != null && !((Microsoft.Xna.Framework.Game)instance).IsActive)
		{
			return;
		}
		if (quit_game)
		{
			exitingGame = true;
			((Microsoft.Xna.Framework.Game)this).Exit();
			return;
		}
		if (exceptionThrown)
		{
			try
			{
				base.Update(gameTime);
				return;
			}
			catch (GameUpdateRequiredException)
			{
				gameUpdateRequired = true;
				disableNetworkCalls = true;
				return;
			}
		}
		TouchPanel.EnabledGestures = (GestureType)0;
		if (showUpsell)
		{
			Touch.instance.Clear();
			try
			{
				state_prev = state_curr;
				state_curr = GamePad.GetState((PlayerIndex)0);
				GamePadButtons buttons2 = state_prev.Buttons;
				if ((int)buttons2.Back == 1)
				{
					GamePadButtons buttons3 = state_curr.Buttons;
					if ((int)buttons3.Back == 0)
					{
						if (!exit_from_upsell)
						{
							skip = 1;
							showUpsell = false;
							Thread.Sleep(32);
							return;
						}
						exitingGame = true;
						((Microsoft.Xna.Framework.Game)this).Exit();
					}
				}
				Microsoft.Xna.Framework.Input.Touch.TouchCollection state2 = TouchPanel.GetState();
				curr_down = state2.Count > 0;
				if (prev_down != curr_down)
				{
					prev_down = curr_down;
					if (curr_down)
					{
						if (uid == -1)
						{
							foreach (var loc in state2)
							{
								if (loc.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed)
								{
									uid = loc.Id;
									tx = (int)loc.Position.X;
									ty = (int)loc.Position.Y;
									break;
								}
							}
						}
					}
					else
					{
						if (inExit)
						{
							if (exit_from_upsell)
							{
								exitingGame = true;
								((Microsoft.Xna.Framework.Game)this).Exit();
							}
							else
							{
								showUpsell = false;
							}
						}
						else if (inPurchase)
						{
							showUpsell = false;
						}
						inExit = false;
						inPurchase = false;
						uid = -1;
						tx = -1;
						ty = -1;
					}
				}
				if (curr_down)
				{
					foreach (var loc in state2)
					{
						if (uid == loc.Id && loc.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Released)
						{
							tx = (int)loc.Position.X;
							ty = (int)loc.Position.Y;
							break;
						}
					}
					Rectangle val = default(Rectangle);
					val = new Rectangle((int)pos1.X, (int)pos1.Y, menu_exit.intex.Width, menu_exit.intex.Height);
					Rectangle val2 = default(Rectangle);
					val2 = new Rectangle((int)pos2.X, (int)pos2.Y, menu_purchase.intex.Width, menu_purchase.intex.Height);
					if (val.Contains(tx, ty))
					{
						inExit = true;
					}
					else
					{
						inExit = false;
					}
					if (val2.Contains(tx, ty))
					{
						inPurchase = true;
					}
					else
					{
						inPurchase = false;
					}
				}
				else
				{
					inExit = false;
					inPurchase = false;
				}
				return;
			}
			catch
			{
				return;
			}
		}
		if (timeout > 0f)
		{
			if (timeout > 0f)
			{
				var touchAny = TouchPanel.GetState().Count > 0
					|| Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
				if (touchAny)
				{
					timeout = 0f;
					loadinScreen = 5f;
				}
			}
			timeout -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;
			if (timeout < 0f)
			{
				timeout = 0f;
			}
			if (!fadeout && timeout < 0.5f)
			{
				fm.Start(FadeState.ToBlack, 250f);
				fadeout = true;
			}
			if (!fadein && timeout < 0.25f)
			{
				fm.Start(FadeState.ToNormal, 250f);
				fadein = true;
			}
			fm.Update(gameTime);
			return;
		}
		try
		{
			if (load_failed || init_failed || quit_game)
			{
				return;
			}
			if (initupdates > 0)
			{
				initupdates--;
				if (initupdates == 0)
				{
					fgame.Init(0u, "");
				}
				return;
			}
			float scaleX = 800f / (float)((Microsoft.Xna.Framework.Game)this).Window.ClientBounds.Width;
			float scaleY = 480f / (float)((Microsoft.Xna.Framework.Game)this).Window.ClientBounds.Height;
			var state4 = TouchPanel.GetState();
			foreach (var loc in state4)
			{
				float tx = loc.Position.X * scaleX;
				float ty = loc.Position.Y * scaleY;
				if (loc.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed || loc.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Moved)
				{
					Touch.instance.__UpdateInternal((uint)loc.Id, dn: true, tx, ty, 0f);
				}
				else if (loc.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Released || loc.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Invalid)
				{
					Touch.instance.__UpdateInternal((uint)loc.Id, dn: false, tx, ty, 0f);
				}
			}
			MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
			bool mouseDown = mouseState.LeftButton == ButtonState.Pressed;
			if (mouseDown || _prevMouseState.LeftButton == ButtonState.Pressed)
			{
				Touch.instance.__UpdateInternal(0xFEu, mouseDown, (float)mouseState.X * scaleX, (float)mouseState.Y * scaleY, 0f);
			}
			_prevMouseState = mouseState;
			if (loadinScreen < 5f && (Touch.instance.GetTouchCount() > 0 || mouseDown))
			{
				loadinScreen = 5f;
			}
			Touch.instance.Update();
			float dt = 0f;
			if (!SystemManager.GetInstance().Update(ref dt))
			{
				fgame.End();
				exitingGame = true;
				((Microsoft.Xna.Framework.Game)this).Exit();
			}
			fgame.Update(timeSinceLastUpdate);
			AchievementManager.Update(gameTime);
			BackButtonWasPressed = false;
			try
			{
				base.Update(gameTime);
			}
			catch (GameUpdateRequiredException)
			{
				gameUpdateRequired = true;
			}
		}
		catch (GameUpdateRequiredException)
		{
			gameUpdateRequired = true;
		}
		catch (Exception ex4)
		{
			GameException();
			for (Exception innerException = ex4.InnerException; innerException != null; innerException = innerException.InnerException)
			{
			}
			string text = ERROR_MESSAGE_GAME();
			string[] array = new string[1] { ERROR_BUTTON_1 };
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			if (text.Length > 255)
			{
				text = text.Substring(0, 250);
			}
			Guide.BeginShowMessageBox(ERROR_TITLE_1, text, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)IngameException, (object)null);
		}
	}

	private void HandleGameUpdateException()
	{
		string text = instance.stringTable.GetString(931);
		string text2 = instance.stringTable.GetString(926);
		string text3 = " ";
		string[] array = new string[2] { text2, text };
		string text4 = Game.game_work.language switch
		{
			StringTableUtils.Language.LANGUAGE_FRENCH => "Une mise à jour est disponible ! Cette mise à jour est requise pour se connecter à Xbox LIVE. Mettre à jour dès maintenant ?", 
			StringTableUtils.Language.LANGUAGE_ITALIAN => "È disponibile un aggiornamento! Questo aggiornamento è necessario per connettersi a Xbox LIVE. Aggiorna ora?", 
			StringTableUtils.Language.LANGUAGE_GERMAN => "Ein Update ist verfügbar! Für dieses Update muss eine Verbindung zu Xbox LIVE hergestellt werden. Jetzt updaten?", 
			StringTableUtils.Language.LANGUAGE_SPANISH => "¡Hay una actualización disponible! Esta actualización es necesaria para conectarse a Xbox LIVE. ¿Actualizar ahora?", 
			_ => "An update is available! This update is required to connect to Xbox LIVE. Update now?", 
		};
		while (Guide.IsVisible)
		{
			Thread.Sleep(32);
		}
		Guide.BeginShowMessageBox(text3, text4, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)GameUpdateCallback, (object)null);
	}

	private static void GameUpdateCallback(IAsyncResult result)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		try
		{
			int? num = Guide.EndShowMessageBox(result);
			if (!num.HasValue || !num.HasValue || num.Value != 1)
			{
				return;
			}
			if (Game.isWP7TrialMode())
			{
				if (Guide.IsVisible)
				{
					Thread.Sleep(128);
				}
				PE_ShowMarketplace();
				Guide.ShowMarketplace((PlayerIndex)0);
			}
			else
			{
				MarketplaceDetailTask val = new MarketplaceDetailTask();
				val.ContentType = (MarketplaceContentType)1;
				val.Show();
			}
		}
		catch
		{
		}
	}

	private void PresentRenderTarget()
	{
		if (_gameRenderTarget == null || spriteBatch == null)
		{
			return;
		}
		((Microsoft.Xna.Framework.Game)this).GraphicsDevice.SetRenderTarget(null);
		((Microsoft.Xna.Framework.Game)this).GraphicsDevice.Clear(Color.Black);
		int winW = ((Microsoft.Xna.Framework.Game)this).Window.ClientBounds.Width;
		int winH = ((Microsoft.Xna.Framework.Game)this).Window.ClientBounds.Height;
		float gameAspect = 800f / 480f;
		float winAspect = (float)winW / (float)winH;
		int destW;
		int destH;
		if (winAspect > gameAspect)
		{
			destH = winH;
			destW = (int)((float)winH * gameAspect);
		}
		else
		{
			destW = winW;
			destH = (int)((float)winW / gameAspect);
		}
		int destX = (winW - destW) / 2;
		int destY = (winH - destH) / 2;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
		spriteBatch.Draw(_gameRenderTarget, new Rectangle(destX, destY, destW, destH), Color.White);
		spriteBatch.End();
	}

	protected override void Draw(GameTime gameTime)
	{
		updates = 0;
		if (_gameRenderTarget == null)
		{
			_gameRenderTarget = new RenderTarget2D(((Microsoft.Xna.Framework.Game)this).GraphicsDevice, 800, 480, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
		}
		((Microsoft.Xna.Framework.Game)this).GraphicsDevice.SetRenderTarget(_gameRenderTarget);
		if (!((Microsoft.Xna.Framework.Game)this).IsActive || !((Microsoft.Xna.Framework.Game)instance).IsActive)
		{
			try
			{
				if (m_fruitTex == null || m_ninjaTex == null || m_headTex == null || m_bodyTex == null)
				{
					m_fruitTex = ((Microsoft.Xna.Framework.Game)this).Content.Load<Texture2D>("textureswp7/fruit_text.tga");
					m_ninjaTex = ((Microsoft.Xna.Framework.Game)this).Content.Load<Texture2D>("textureswp7/ninja_text.tga");
					m_headTex = ((Microsoft.Xna.Framework.Game)this).Content.Load<Texture2D>("textureswp7/sensei_head_02.tga");
					m_bodyTex = ((Microsoft.Xna.Framework.Game)this).Content.Load<Texture2D>("textureswp7/sensei_body_02.tga");
				}
			}
			catch (Exception)
			{
				m_fruitTex = null;
				m_ninjaTex = null;
				m_headTex = null;
				m_bodyTex = null;
			}
			((Microsoft.Xna.Framework.Game)this).GraphicsDevice.Clear(Color.Black);
			if (m_fruitTex != null && m_ninjaTex != null && m_headTex != null && m_bodyTex != null && spriteBatch != null)
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				spriteBatch.Draw(m_fruitTex, new Rectangle(280, 260, m_fruitTex.Width, m_fruitTex.Height), Color.White);
				spriteBatch.Draw(m_ninjaTex, new Rectangle(240 + m_fruitTex.Width + 40, 310, m_ninjaTex.Width, m_ninjaTex.Height), Color.White);
				spriteBatch.Draw(m_bodyTex, new Rectangle(80, 220, m_bodyTex.Width, m_bodyTex.Height), Color.White);
				spriteBatch.Draw(m_headTex, new Rectangle(156, 244, m_headTex.Width, m_headTex.Height), Color.White);
				spriteBatch.End();
			}
			PresentRenderTarget();
			base.Draw(gameTime);
			return;
		}
		if (switchLanguage)
		{
			DisplayManager.GetInstance().currentTexture = null;
			switchLanguage = false;
			SwitchToLanguage(switchToLanguage, bReload: true);
		}
		if (showUpsell && upsell != null)
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			spriteBatch.Draw(upsell.intex, new Rectangle(0, 0, 800, 480), (Rectangle?)new Rectangle(0, 0, 800, 480), Color.White);
			if (inExit)
			{
				spriteBatch.Draw(menu_exit.intex, new Vector2(pos1.X - (float)(menu_exit.intex.Width / 10), pos1.Y - (float)(menu_exit.intex.Height / 10)), (Rectangle?)null, Color.White, 0f, Vector2.Zero, new Vector2(1.2f, 1.2f), (SpriteEffects)0, 0f);
			}
			else
			{
				spriteBatch.Draw(menu_exit.intex, pos1, Color.White);
			}
			if (inPurchase)
			{
				spriteBatch.Draw(menu_purchase.intex, new Vector2(pos2.X - (float)(menu_purchase.intex.Width / 10), pos2.Y - (float)(menu_purchase.intex.Height / 10)), (Rectangle?)null, Color.White, 0f, Vector2.Zero, new Vector2(1.2f, 1.2f), (SpriteEffects)0, 0f);
			}
			else
			{
				spriteBatch.Draw(menu_purchase.intex, pos2, Color.White);
			}
			spriteBatch.End();
			PresentRenderTarget();
			return;
		}
		if (timeout > 0f)
		{
			spriteBatch.Begin();
			if (timeout < 0.25f)
			{
				spriteBatch.Draw(hbLogoTexture.intex, new Vector2(0f, 0f), (Rectangle?)null, Color.White, MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(1.6666666f, 1.5f), (SpriteEffects)0, 0f);
			}
			else
			{
				spriteBatch.Draw(bobsCousinIt, new Vector2(0f, 0f), Color.White);
			}
			fm.Draw(graphics.GraphicsDevice, spriteBatch);
			spriteBatch.End();
			PresentRenderTarget();
			return;
		}
		if (setup_attribute)
		{
			if (setup_thread == null)
			{
				ca = new ConnectionAcquire();
				setup_thread = new Thread(ca.DoWork);
				setup_thread.Start();
			}
			else if (ca.IsComplete())
			{
				setup_attribute = false;
				PE_SetupAttribute();
				PE_SetupAttributeMGS(Game.isWP7TrialMode().ToString(), "True", ca.GetInterfaceType(), "Desktop", Environment.OSVersion.ToString(), "1.0", "1.0", "0", CultureInfo.CurrentUICulture.Name, TimeZoneInfo.Local.BaseUtcOffset.TotalHours.ToString(), TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours.ToString(), "");
			}
		}
		if (exceptionThrown)
		{
			base.Draw(gameTime);
			return;
		}
		try
		{
			if (load_failed || init_failed || quit_game)
			{
				PresentRenderTarget();
				return;
			}
			if (initupdates == 0)
			{
				if (loadinScreen < 5f)
				{
					loadinScreen += 1f / 60f;
				}
				float timeSinceLastUpdate = 1f / 60f * dtCo;
				DisplayManager.GetInstance().BeginFrame();
				fgame.Draw(timeSinceLastUpdate);
				if (Game.isWP7TrialMode())
				{
					AchievementManager.UpdatePretendAchievements(spriteBatch);
				}
				AchievementManager.UpdateUnlockDropdown(spriteBatch);
			}
			if (loadinScreen < 0.5f)
			{
				float num = loadinScreen * 2f;
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				spriteBatch.Draw(hbLogoTexture.intex, new Vector2(400f, 240f), (Rectangle?)null, new Color(1f, 1f, 1f, 1f - num), MathHelper.ToRadians(0f), new Vector2(240f, 160f), new Vector2(1.6666666f, 1.5f) * (1f + num), (SpriteEffects)0, 0f);
				spriteBatch.End();
			}
			PresentRenderTarget();
		}
		catch (Exception ex2)
		{
			GameException();
			for (Exception innerException = ex2.InnerException; innerException != null; innerException = innerException.InnerException)
			{
			}
			string text = ERROR_MESSAGE_GAME();
			string[] array = new string[1] { ERROR_BUTTON_1 };
			while (Guide.IsVisible)
			{
				Thread.Sleep(32);
			}
			if (text.Length > 255)
			{
				text = text.Substring(0, 250);
			}
			Guide.BeginShowMessageBox(ERROR_TITLE_1, text, (IEnumerable<string>)array, 0, (MessageBoxIcon)3, (AsyncCallback)IngameException, (object)null);
		}
	}

	private void SwitchToLanguage(StringTableUtils.Language switchToLanguage)
	{
		SwitchToLanguage(switchToLanguage, bReload: false);
	}

	private void SwitchToLanguage(StringTableUtils.Language switchToLanguage, bool bReload)
	{
		Game.game_work.SetLanguage(switchToLanguage);
		switch (switchToLanguage)
		{
		default:
			StringManager.GetInstance().SetDefaultLanguage("english_us");
			break;
		case StringTableUtils.Language.LANGUAGE_ENGLISH_UK:
			StringManager.GetInstance().SetDefaultLanguage("english_uk");
			break;
		case StringTableUtils.Language.LANGUAGE_FRENCH:
			StringManager.GetInstance().SetDefaultLanguage("french");
			break;
		case StringTableUtils.Language.LANGUAGE_ITALIAN:
			StringManager.GetInstance().SetDefaultLanguage("italian");
			break;
		case StringTableUtils.Language.LANGUAGE_GERMAN:
			StringManager.GetInstance().SetDefaultLanguage("german");
			break;
		case StringTableUtils.Language.LANGUAGE_SPANISH:
			StringManager.GetInstance().SetDefaultLanguage("spanish");
			break;
		case StringTableUtils.Language.LANGUAGE_CHINESE_SIMPLIFIED:
			StringManager.GetInstance().SetDefaultLanguage("chinese");
			break;
		case StringTableUtils.Language.LANGUAGE_CHINESE_TRADITIONAL:
			StringManager.GetInstance().SetDefaultLanguage("traditional_chinese");
			break;
		}
		instance.stringTable.UpdateDefaultLanguage();
		if (bReload)
		{
			StringTableUtils.FontFamily fontFamily = StringTableUtils.GetFontFamily(Game.game_work.language);
			Game.game_work.pGameFont = MTLocalisation.GetCachedFont(fontFamily);
			SaveConfig();
			TextureManager.GetInstance().ReloadLocalisedTextures(Game.game_work.language);
		}
	}

	private void GameException()
	{
		exceptionThrown = true;
	}

	protected override void OnExiting(object sender, ExitingEventArgs args)
	{
		analytics.Save();
		PE_TeardownAttribute();
		PE_GameInformation();
		try
		{
			if (!load_failed && !init_failed && !quit_game)
			{
				try
				{
					GameTask.SaveCurrentData(isClosingSave: true);
				}
				catch
				{
				}
			}
			base.OnExiting(sender, args);
		}
		catch
		{
		}
	}

	protected override void OnActivated(object sender, EventArgs args)
	{
		try
		{
			Game.SetTrialModeState();
			Thread.Sleep(70);
			Game.SetTrialModeState();
			SoundManager.GetInstance().SetSFXVolume(Game.game_work.soundEnabled ? SoundDef.DEFAULT_SFX_VOL : 0f);
			base.OnActivated(sender, args);
		}
		catch
		{
		}
	}

	protected override void OnDeactivated(object sender, EventArgs args)
	{
		try
		{
			SoundManager.GetInstance().SetSFXVolume(0f);
			Game.OnActivate();
			base.OnDeactivated(sender, args);
			SaveConfig();
		}
		catch
		{
		}
	}

	public static string NotSignedInMessage()
	{
		return Game.game_work.language switch
		{
			StringTableUtils.Language.LANGUAGE_FRENCH => "Vous n'êtes pas connecté à XBOX live.", 
			StringTableUtils.Language.LANGUAGE_ITALIAN => "Non hai effettuato l'accesso a XBOX live.", 
			StringTableUtils.Language.LANGUAGE_GERMAN => "Du bist nicht auf XBOX Live angemeldet.", 
			StringTableUtils.Language.LANGUAGE_SPANISH => "No has iniciado sesión en XBOX Live.", 
			_ => "You are not signed in to XBOX live.", 
		};
	}
}
