using System;
using System.Collections.Generic;
using System.Linq;

namespace Mortar;

public class InputManager
{
	protected struct KeyStringMap(uint h, uint k)
	{
		public uint hash = h;

		public uint key = k;
	}

	protected volatile bool m_loadingFile;

	protected volatile bool m_updating;

	protected LinkedList<InputDevice> m_deviceList = new LinkedList<InputDevice>();

	private static KeyStringMap[] ParseKeykeymaps = new KeyStringMap[61]
	{
		new KeyStringMap(StringFunctions.StringHash("MouseButton1"), 108u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton2"), 109u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton3"), 110u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton4"), 111u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton5"), 112u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton6"), 113u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton7"), 114u),
		new KeyStringMap(StringFunctions.StringHash("MouseButton8"), 115u),
		new KeyStringMap(StringFunctions.StringHash("MouseAxisX"), 116u),
		new KeyStringMap(StringFunctions.StringHash("MouseAxisY"), 117u),
		new KeyStringMap(StringFunctions.StringHash("Touch1"), 137u),
		new KeyStringMap(StringFunctions.StringHash("Touch2"), 138u),
		new KeyStringMap(StringFunctions.StringHash("Touch3"), 139u),
		new KeyStringMap(StringFunctions.StringHash("Touch4"), 140u),
		new KeyStringMap(StringFunctions.StringHash("Touch5"), 141u),
		new KeyStringMap(StringFunctions.StringHash("Touch6"), 142u),
		new KeyStringMap(StringFunctions.StringHash("Touch7"), 143u),
		new KeyStringMap(StringFunctions.StringHash("Touch8"), 144u),
		new KeyStringMap(StringFunctions.StringHash("Touch9"), 145u),
		new KeyStringMap(StringFunctions.StringHash("Touch10"), 146u),
		new KeyStringMap(StringFunctions.StringHash("Touch11"), 147u),
		new KeyStringMap(StringFunctions.StringHash("Touch12"), 148u),
		new KeyStringMap(StringFunctions.StringHash("Touch13"), 149u),
		new KeyStringMap(StringFunctions.StringHash("Touch14"), 150u),
		new KeyStringMap(StringFunctions.StringHash("Touch15"), 151u),
		new KeyStringMap(StringFunctions.StringHash("Touch16"), 152u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX1"), 153u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX2"), 154u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX3"), 155u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX4"), 156u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX5"), 157u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX6"), 158u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX7"), 159u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX8"), 160u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX9"), 161u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX10"), 162u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX11"), 163u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX12"), 164u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX13"), 165u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX14"), 166u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX15"), 167u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisX16"), 168u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY1"), 169u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY2"), 170u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY3"), 171u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY4"), 172u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY5"), 173u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY6"), 174u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY7"), 175u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY8"), 176u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY9"), 177u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY10"), 178u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY11"), 179u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY12"), 180u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY13"), 181u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY14"), 182u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY15"), 183u),
		new KeyStringMap(StringFunctions.StringHash("TouchAxisY16"), 184u),
		new KeyStringMap(StringFunctions.StringHash("AccelAxisX"), 185u),
		new KeyStringMap(StringFunctions.StringHash("AccelAxisY"), 186u),
		new KeyStringMap(StringFunctions.StringHash("AccelAxisZ"), 187u)
	};

	private static KeyStringMap[] ParseActionkeymaps = new KeyStringMap[7]
	{
		new KeyStringMap(StringFunctions.StringHash("pressed"), 1u),
		new KeyStringMap(StringFunctions.StringHash("released"), 4u),
		new KeyStringMap(StringFunctions.StringHash("down"), 2u),
		new KeyStringMap(StringFunctions.StringHash("up"), 8u),
		new KeyStringMap(StringFunctions.StringHash("active"), 16u),
		new KeyStringMap(StringFunctions.StringHash("move"), 32u),
		new KeyStringMap(StringFunctions.StringHash("dead"), 64u)
	};

	private static InputManager instance;

	private InputManager()
	{
	}

	protected uint ParseKey(uint hash)
	{
		for (int i = 0; i < ParseKeykeymaps.Length; i++)
		{
			if (hash == ParseKeykeymaps[i].hash)
			{
				return ParseKeykeymaps[i].key;
			}
		}
		return 0u;
	}

	protected uint ParseAction(uint hash)
	{
		for (int i = 0; i < ParseActionkeymaps.Length; i++)
		{
			if (hash == ParseActionkeymaps[i].hash)
			{
				return ParseActionkeymaps[i].key;
			}
		}
		return 0u;
	}

	protected bool ValidCharacter(byte c)
	{
		return Math.BETWEEN(c, 32, 175);
	}

	public static InputManager GetInstance()
	{
		if (instance == null)
		{
			instance = new InputManager();
		}
		return instance;
	}

	public void Init()
	{
		InputDevice inputDevice = new InputDeviceIphoneTouch();
		inputDevice.Init();
		m_deviceList.AddLast(inputDevice);
	}

	public void Update(float dt)
	{
		if (m_loadingFile)
		{
			return;
		}
		m_updating = true;
		foreach (InputDevice device in m_deviceList)
		{
			device.Update(dt);
		}
		m_updating = false;
	}

	public void Destroy()
	{
		m_loadingFile = false;
		m_updating = false;
		int num = 0;
		foreach (InputDevice device in m_deviceList)
		{
			InputDevice dl = device;
			if (num++ == 0)
			{
				dl.ClearActions(0u, del: true);
			}
			dl.Destroy();
			Delete.SAFE_DELETE(ref dl);
		}
		m_deviceList.Clear();
	}

	public bool LoadConfigFile(string fileName)
	{
		m_loadingFile = true;
		while (m_updating)
		{
		}
		if (TextureManager.TextureFileExists(fileName))
		{
			uint creationHash = StringFunctions.StringHash(fileName);
			string text = MortarFile.LoadText(fileName);
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = 0u;
			char[] array = new char[255];
			int num4 = 0;
			if (text != null)
			{
				int num5 = 0;
				InputEvent inputEvent = new InputEvent();
				inputEvent.eventType = 0u;
				InputActionMapper inputActionMapper = null;
				for (int i = 0; i < text.Length; i++)
				{
					switch (text[i])
					{
					case ',':
						array[num4] = '\0';
						if (num5 == 1)
						{
							string s4 = new string(array, 0, num4);
							uint num8 = ParseKey(StringFunctions.StringHash(s4));
							num2 |= num8;
						}
						else
						{
							string s5 = new string(array, 0, num4);
							uint num9 = ParseAction(StringFunctions.StringHash(s5));
							num3 |= num9;
						}
						num4 = 0;
						break;
					case ':':
					{
						array[num4] = '\0';
						string s3 = new string(array, 0, num4);
						num = StringFunctions.StringHash(s3);
						num4 = 0;
						num5++;
						break;
					}
					case ';':
					{
						_ = 1;
						array[num4] = '\0';
						string s = new string(array, 0, num4);
						uint num6 = ParseKey(StringFunctions.StringHash(s));
						num2 = num6;
						num4 = 0;
						num3 = 0u;
						num5++;
						break;
					}
					case '\n':
					{
						_ = 2;
						array[num4] = '\0';
						string s2 = new string(array, 0, num4);
						uint num7 = ParseAction(StringFunctions.StringHash(s2));
						num3 = (inputEvent.eventType = num3 | num7);
						if (inputEvent.eventType < 16)
						{
							inputEvent.eventType |= 65536u;
							inputEvent.button.key = num2;
							inputEvent.button.pressure = 0f;
						}
						else
						{
							inputEvent.eventType |= 131072u;
							inputEvent.axis.axis = (int)num2;
						}
						if (num != 0 && num2 != 0 && num3 != 0)
						{
							inputActionMapper = new InputActionMapper(inputEvent, null, num, creationHash);
							GetInstance().AddActionMapper(inputActionMapper);
						}
						num4 = 0;
						num5 = 0;
						break;
					}
					default:
						if (ValidCharacter((byte)text[i]))
						{
							array[num4++] = text[i];
						}
						break;
					case '\t':
					case ' ':
						break;
					}
				}
				if (num5 == 2)
				{
					array[num4] = '\0';
					num3 = (inputEvent.eventType = num3 | ParseAction(StringFunctions.StringHash(new string(array))));
					if (inputEvent.eventType < 16)
					{
						inputEvent.eventType |= 65536u;
						inputEvent.button.key = num2;
						inputEvent.button.pressure = 0f;
					}
					else
					{
						inputEvent.eventType |= 131072u;
						inputEvent.axis.axis = (int)num2;
					}
					if (num != 0 && num2 != 0 && num3 != 0)
					{
						inputActionMapper = new InputActionMapper(inputEvent, null, num, creationHash);
						GetInstance().AddActionMapper(inputActionMapper);
					}
					num4 = 0;
					num5 = 0;
				}
				m_loadingFile = false;
				return true;
			}
			m_loadingFile = false;
			return false;
		}
		throw new Exception("Input file is missing");
	}

	public void ResetDevices()
	{
		foreach (InputDevice device in m_deviceList)
		{
			device.Reset();
		}
	}

	public void ClearActions(uint ParentHash)
	{
		int num = 0;
		foreach (InputDevice device in m_deviceList)
		{
			device.ClearActions(ParentHash, num == m_deviceList.Count() - 1);
			num++;
		}
	}

	public void AddActionMapper(InputActionMapper action)
	{
		foreach (InputDevice device in m_deviceList)
		{
			device.AddAction(action);
		}
	}

	public void RegisterInputCallback(string hash, InputActionMapper.InputCallback callback)
	{
		foreach (InputDevice device in m_deviceList)
		{
			device.RegisterInputCallback(StringFunctions.StringHash(hash), hash, callback);
		}
	}
}
