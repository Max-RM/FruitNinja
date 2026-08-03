using System;
using System.Collections.Generic;
using FruitNinja;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Mortar;

public class SoundManager
{
	public float soundFadeOut;

	private Dictionary<string, Microsoft.Xna.Framework.Audio.SoundEffect> sfxs = new Dictionary<string, Microsoft.Xna.Framework.Audio.SoundEffect>();

	private LinkedList<SoundEffectInstance> sysManagedSfx = new LinkedList<SoundEffectInstance>();

	private static float sfx_volume = 1f;

	private static float mus_volume = 1f;

	private static SoundManager instance = new SoundManager();

	private bool customMusic;

	private bool allowMusic;

	private Song currentSong;

	private static bool curr = false;

	private static bool prev = false;

	public static int SOUND_MANAGER_HEAP_SIZE => 524288;

	public static byte DEFAULT_NOTE => 64;

	public bool CustomMusic
	{
		get
		{
			return customMusic;
		}
		set
		{
			customMusic = value;
		}
	}

	public static SoundManager GetInstance()
	{
		return instance;
	}

	public void Initialise(string project)
	{
		Initialise(project, SOUND_MANAGER_HEAP_SIZE);
	}

	public void Initialise(string project, int heap_size)
	{
		CacheSFX();
	}

	public void Destroy()
	{
	}

	private void CacheSFX()
	{
		try
		{
			string text = ((Microsoft.Xna.Framework.Game)TheGame.instance).Content.Load<string>("sfxCacheFile.txt");
			string[] array = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				Microsoft.Xna.Framework.Audio.SoundEffect value = null;
				try
				{
					value = ((Microsoft.Xna.Framework.Game)TheGame.instance).Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("sound/" + text2 + ".wav");
				}
				catch (Exception)
				{
				}
				sfxs.Add(text2, value);
			}
		}
		catch (Exception)
		{
		}
	}

	public MortarSound SFXPlay(string filename)
	{
		SFXPlay(filename, 0u);
		return new MortarSound();
	}

	public void SFXPlay(string filename, uint flags)
	{
		SFXPlay(filename, flags, null);
	}

	public void SFXPlay(string filename, uint flags, MortarSound snd)
	{
		SFXPlay(filename, flags, snd, DEFAULT_NOTE);
	}

	public void SFXPlay(string filename, uint flags, MortarSound snd, byte note)
	{
		SFXPlay(filename, flags, snd, note, -1);
	}

	public void SFXPlay(string filename, uint flags, MortarSound snd, byte note, int pitch)
	{
		SFXPlay(filename, flags, snd, note, pitch, loop: false);
	}

	public void SFXPlay(string filename, uint flags, MortarSound snd, byte note, int pitch, bool loop)
	{
		if (!sfxs.TryGetValue(filename, out var value))
		{
			value = null;
			try
			{
				string text = "sound/" + filename + ".wav";
				value = ((Microsoft.Xna.Framework.Game)TheGame.instance).Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(text);
			}
			catch (Exception)
			{
			}
			sfxs.Add(filename, value);
		}
		if (value != null)
		{
			SoundEffectInstance val = value.CreateInstance();
			if (snd != null)
			{
				snd.inst = val;
			}
			else
			{
				sysManagedSfx.AddLast(val);
			}
			if (FruitNinja.Game.game_work.soundEnabled)
			{
				val.IsLooped = loop;
				val.Volume = sfx_volume;
				val.Play();
			}
		}
	}

	public void SFXStop(int index)
	{
	}

	public void SFXStop(string filename)
	{
	}

	public void SFXPause(int index)
	{
	}

	public void SFXPause(string filename)
	{
	}

	public void SFXPauseAll()
	{
	}

	public static MortarSound CreateNewSound()
	{
		return new MortarSound();
	}

	public void AllowMusic()
	{
		allowMusic = true;
	}

	private void PlaySong(string name)
	{
		mus_volume = 0.01f;
		ApplyMusicVolume();
	}

	public void SongPlay(string filename)
	{
		if (!allowMusic)
		{
			return;
		}
		try
		{
			PlaySong("music/" + filename + ".wma");
		}
		catch (Exception)
		{
			currentSong = null;
			try
			{
				PlaySong("music/" + filename);
			}
			catch (Exception)
			{
				currentSong = null;
			}
		}
	}

	public bool UserPlayingMusic()
	{
		return false;
	}

	public void SongSwitchPlay(string filename)
	{
	}

	public void SongStop()
	{
	}

	public void SongPause()
	{
	}

	public void SongResume()
	{
	}

	public void SongSetStreaming(bool streaming)
	{
	}

	public void SongSetMemorySize(int size)
	{
	}

	public void MuteSound()
	{
		MuteSound(mute: true);
	}

	public void MuteSound(bool mute)
	{
	}

	public void Update(float dt)
	{
		LinkedListNode<SoundEffectInstance> linkedListNode = sysManagedSfx.First;
		while (linkedListNode != null)
		{
			LinkedListNode<SoundEffectInstance> next = linkedListNode.Next;
			if (linkedListNode.Value.State == SoundState.Stopped)
			{
				sysManagedSfx.Remove(linkedListNode);
			}
			linkedListNode = next;
		}
		if (currentSong != null && allowMusic && mus_volume < 1f)
		{
			mus_volume += 1f / 60f;
			if (mus_volume > 1f)
			{
				mus_volume = 1f;
			}
		}
	}

	private void ApplyMusicVolume()
	{
	}

	public void Draw(SpriteBatch batch)
	{
	}

	public void Release(MortarSound obj)
	{
	}

	public void ReleaseAll()
	{
	}

	public void SetSFXVolume(float amount)
	{
	}
}
