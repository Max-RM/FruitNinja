using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class Bomb : Entity
{
	protected float m_newBlastWait;

	protected MenuButton.MenuCallback m_callback;

	protected int m_forPlayer;

	public bool m_hit;

	public float m_z;

	public ushort[] m_rotation_speed = new ushort[2];

	public ushort[] m_rotation_piece = new ushort[2];

	public bool m_disabled;

	public byte PAD00;

	public short PAD01;

	public PSPParticleEmitter m_emitter;

	public bool m_gravityEnabled;

	public MenuButton m_hudControl;

	public bool m_isMenuItem;

	public Vector3 m_gravity;

	public float m_chuckWait;

	public Vector3 m_orig_scale;

	public float m_explosionTime;

	public static Model[] m_bombModel = new Model[Game.MAX_PLAYERS * 2];

	public static uint[] particleHash = new uint[Game.MAX_PLAYERS * 2];

	public static Model s_arcadeBombModel = null;

	public static bool s_isContentLoaded = false;

	public static Texture s_minus_10 = null;

	public static Texture m_blastTexture = null;

	public static Texture[] s_flashTexture = new Texture[2];

	public static Bomb highestBomb = null;

	public static bool s_sfxPlayedThisFrame = false;

	public float m_dtMod;

	public static int MAX_ROTATION_SPEED => 8;

	public static int MAX_ROTATION_ANGLE => 359;

	public static float SIXTY_FPS_DT => 1f / 60f;

	public static float BOMB_CHUCK_WAIT => 0.2f;

	public static float FUSE_DIST(float mcsx)
	{
		return 0.9f * mcsx * 100f;
	}

	public bool GetHit()
	{
		return m_hit;
	}

	public static void LoadContent()
	{
		if (!s_isContentLoaded)
		{
			m_bombModel[0] = MeshManager.GetInstance().Load("models/Fruit/Bomb.mmd");
			particleHash[0] = StringFunctions.StringHash("bomb_smoke");
			int num = 1;
			m_bombModel[num] = MeshManager.GetInstance().Load("models/Fruit/Bomb_purple.mmd");
			s_minus_10 = TextureManager.GetInstance().Load("textureswp7/minus_10.tex");
			for (int i = 1; i < num; i++)
			{
				particleHash[i] = particleHash[0];
			}
			particleHash[num] = StringFunctions.StringHash("purple_bomb_smoke");
			s_isContentLoaded = true;
		}
	}

	public static float GetHeighestBomb()
	{
		float num = -10000f;
		uint num2 = 0u;
		for (Entity entity = ActorManager.GetInstance().GetEntity(EntityTypes.ENTITY_BOMB, num2++); entity != null; entity = ActorManager.GetInstance().GetEntity(EntityTypes.ENTITY_BOMB, num2++))
		{
			float num3 = entity.m_pos.Y + Game.SCREEN_HEIGHT / 2f;
			if (Game.IsMultiplayer())
			{
				num3 = Game.SCREEN_WIDTH / 2f - Math.ABS(entity.m_pos.X);
			}
			if (!((Bomb)entity).m_isMenuItem && num3 > num)
			{
				num = num3;
			}
		}
		return num;
	}

	public override void Init(byte[] tpl_data, int tpl_size, Vector3? size)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		if (m_col_box == null)
		{
			m_col_box = new ColSphere();
		}
		float num = (size.HasValue ? size.Value.X : 1f);
		((ColSphere)m_col_box).centre = new Vector3(m_pos.X, m_pos.Y, 0f);
		((ColSphere)m_col_box).Radius = Game.game_work.bombCollision * 0.5f * num;
		if (m_blastTexture == null)
		{
			m_blastTexture = TextureManager.GetInstance().Load("Textureswp7/bomb_explode.tex");
		}
		m_forPlayer = 0;
		base.m_updateAlways = true;
		base.m_destroy = false;
		m_disabled = false;
		m_gravityEnabled = true;
		m_hit = false;
		m_dtMod = 1f;
		m_explosionTime = 0f;
		m_newBlastWait = 0.6f;
		for (int i = 0; i < 2; i++)
		{
			m_rotation_speed[i] = (ushort)(Math.g_random.Rand32(MAX_ROTATION_SPEED - 1) + 1);
			m_rotation_piece[i] = (ushort)Math.g_random.Rand32(MAX_ROTATION_ANGLE);
		}
		m_isMenuItem = false;
		m_emitter = null;
		m_hudControl = null;
		m_cur_scale = Vector3.One * Game.game_work.bombSize * 0.01f * num;
		m_orig_scale = m_cur_scale;
		m_chuckWait = 0f;
		m_gravity = new Vector3(0f, 0f - Game.GRAVITY / SIXTY_FPS_DT, 0f);
		m_z = GameTask.GetBombZPosition();
	}

	public override void Release()
	{
		if (m_emitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_emitter);
			m_emitter = null;
		}
		if (m_hudControl != null && m_hudControl.m_entity == this)
		{
			m_hudControl.m_entity = null;
		}
		if (highestBomb == this)
		{
			highestBomb = null;
		}
		base.Release();
	}

	public override void Update(float dt)
	{
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		dt *= m_dtMod;
		float num = dt / SIXTY_FPS_DT;
		if (m_hit)
		{
			if (!m_isMenuItem)
			{
				m_newBlastWait -= Game.game_work.dt;
				if (m_newBlastWait < 0f)
				{
					Entity entity = ActorManager.GetInstance().Add(EntityTypes.ENTITY_BOMB_BLAST);
					entity.m_pos = m_pos;
					entity.Init(null, 0, null);
					m_newBlastWait = 0.05f;
				}
			}
			else
			{
				if (m_gravityEnabled)
				{
					m_vel -= m_gravity * dt;
					if ((m_gravity.Y < 0f && m_vel.Y < 0f) || (m_gravity.Y > 0f && m_vel.Y > 0f) || (m_gravity.X < 0f && m_vel.X < 0f) || (m_gravity.X > 0f && m_vel.X > 0f))
					{
						float num2 = m_gravity.Length();
						m_gravity.Normalize();
						m_gravity *= num2 + Game.GRAVITY * num * 2f;
					}
				}
				m_pos += m_vel * num;
				if (dt > 0f)
				{
					for (int i = 0; i < 2; i++)
					{
						m_rotation_piece[i] += m_rotation_speed[i];
					}
				}
			}
			((ColSphere)m_col_box).centre = new Vector3(1000f, 1000f, 0f);
			((ColSphere)m_col_box).Radius = 0.01f;
		}
		else
		{
			if (m_chuckWait > 0f)
			{
				if (Game.game_work.hitBombTime > 0f || Game.game_work.gameOver)
				{
					m_chuckWait = 0f;
					m_pos.Y = 0f - Game.SCREEN_HEIGHT;
					m_vel = new Vector3(0f, -1f, 0f);
				}
				float chuckWait = m_chuckWait;
				if (!Game.game_work.pause)
				{
					m_chuckWait -= Game.game_work.dt;
				}
				if (m_chuckWait <= 0.2f && chuckWait > 0.2f && !s_sfxPlayedThisFrame && !Game.game_work.gameOver)
				{
					SoundManager.GetInstance().SFXPlay(SoundDef.SND_THROW_BOMB);
					s_sfxPlayedThisFrame = true;
				}
				if (!(m_chuckWait <= 0f))
				{
					return;
				}
				float num3 = WaveManager.GetInstance().BombMultiplyer();
				int num4 = (int)num3;
				if (num3 > (float)num4 + 0.01f && (num3 - (float)num4) * 100f > (float)Math.g_random.Rand32(100))
				{
					num4++;
				}
				if (num4 <= 0)
				{
					m_chuckWait = 0f;
					m_pos.Y = 0f - Game.SCREEN_HEIGHT;
					m_vel = new Vector3(0f, -1f, 0f);
				}
				else if (num4 > 1)
				{
					WaveManager.GetInstance().SpawnBomb(num4 - 1);
				}
			}
			if (m_gravityEnabled)
			{
				m_vel += m_gravity * dt;
				if (m_gravityEnabled && ((m_gravity.Y < 0f && m_vel.Y < 0f) || (m_gravity.Y > 0f && m_vel.Y > 0f) || (m_gravity.X < 0f && m_vel.X < 0f) || (m_gravity.X > 0f && m_vel.X > 0f)))
				{
					float num5 = m_gravity.Length();
					m_gravity.Normalize();
					m_gravity *= num5 + Game.GRAVITY * num * 2f;
				}
			}
			m_pos += m_vel * num;
			if (dt > 0f)
			{
				for (int j = 0; j < 2; j++)
				{
					m_rotation_piece[j] += m_rotation_speed[j];
				}
			}
			m_col_box.centre = m_pos;
			m_col_box.centre.Z = 0f;
		}
		if (m_pos.Y <= (0f - Game.SCREEN_HEIGHT) * 0.75f || m_pos.Y >= Game.SCREEN_HEIGHT * 0.75f || m_pos.X <= (0f - Game.SCREEN_WIDTH) * 0.75f || m_pos.X >= Game.SCREEN_WIDTH * 0.75f)
		{
			KillBomb();
		}
		else if (m_emitter == null)
		{
			m_emitter = PSPParticleManager.GetInstance().AddEmitter(particleHash[m_forPlayer], null);
			if (m_emitter != null)
			{
				m_emitter.pos = m_pos;
			}
		}
	}

	public void KillBomb()
	{
		base.m_destroy = true;
		if (m_hudControl != null && object.ReferenceEquals(m_hudControl.m_entity, this))
		{
			m_hudControl.m_entity = null;
		}
		if (m_emitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_emitter);
			m_emitter = null;
		}
	}

	public override void Draw()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (PopOverControl.IsInPopup && !partOfPopup)
		{
			return;
		}
		s_sfxPlayedThisFrame = false;
		if (!(m_chuckWait > 0f))
		{
			float num = -1000f;
			if (this != highestBomb && !m_isMenuItem && m_pos.Y > num)
			{
				highestBomb = this;
			}
			if (m_bombModel[m_forPlayer] != null)
			{
				Matrix val = Matrix.CreateScale(m_cur_scale);
				val *= Matrix.CreateRotationX(MathHelper.ToRadians(270f));
				val *= Matrix.CreateRotationY(MathHelper.ToRadians((float)(int)m_rotation_piece[0]));
				val *= Matrix.CreateRotationZ(MathHelper.ToRadians((float)(int)m_rotation_piece[1]));
				val *= Matrix.CreateTranslation(m_pos + Vector3.UnitZ * m_z);
				m_bombModel[m_forPlayer].Draw(val);
			}
		}
	}

	public override void DrawUpdate(float dt)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (m_emitter != null)
		{
			m_emitter.pos = m_pos + new Vector3(Math.SinIdx((ushort)(-Math.DEGREE_TO_IDX((int)m_rotation_piece[1]))) * FUSE_DIST(m_cur_scale.X), Math.CosIdx((ushort)(-Math.DEGREE_TO_IDX((int)m_rotation_piece[1]))) * FUSE_DIST(m_cur_scale.X), 5f);
			m_emitter.cosz = Math.CosIdx((ushort)(-Math.DEGREE_TO_IDX((int)m_rotation_piece[1])));
			m_emitter.sinz = 0f - Math.SinIdx((ushort)(-Math.DEGREE_TO_IDX((int)m_rotation_piece[1])));
		}
	}

	public override bool CollisionResponse(Entity p_ent2, uint col_1, uint col_2, ref Vector3 proj)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (PopOverControl.IsInPopup && !partOfPopup)
		{
			return false;
		}
		if (!m_disabled)
		{
			if (m_isMenuItem)
			{
				if (m_hudControl == null || (m_hudControl != null && m_hudControl.m_clearOthers))
				{
					Game.ClearMenuItems();
				}
				m_callback();
			}
			else if (p_ent2 != null)
			{
				if (Game.game_work.gameMode == Game.GAME_MODE.GM_ARCADE)
				{
					uint hash = StringFunctions.StringHash("bombs_hit");
					Game.game_work.saveData.AddToTotal("bombs_hit", hash, 1, universal: false, achievementUpload: false);
					WaveManager.GetInstance().ResetSpeed();
					m_isMenuItem = true;
					Game.HitMenuBomb(m_pos);
					Game.game_work.camera.CreateCameraShake(m_pos, 2f, 3f);
					Game.AddToCurrentScore(-10);
					PowerUpManager.GetInstance().ClearTimedPowers();
					MissControl free = MissControl.GetFree();
					free.MakeDisappear(m_pos, 0, s_minus_10);
					free.m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_BOMB;
				}
				else
				{
					if (Game.game_work.gameOver)
					{
						return false;
					}
					Game.HitBomb(m_pos);
				}
			}
			m_hit = true;
		}
		return false;
	}

	public void EnableGravity(bool state)
	{
		m_gravityEnabled = state;
	}

	public bool GravityEnabled()
	{
		return m_gravityEnabled;
	}

	public void Disable()
	{
		m_disabled = true;
	}

	public bool Enabled()
	{
		return !m_disabled;
	}

	public void SetCallback(MenuButton.MenuCallback call, MenuButton button)
	{
		m_isMenuItem = true;
		m_callback = call;
		m_hudControl = button;
		m_rotation_speed[0] = 2;
		m_rotation_speed[1] = 0;
		m_rotation_piece[0] = 0;
		m_rotation_piece[1] = 45;
	}

	public bool IsActive()
	{
		return m_chuckWait <= 0f;
	}

	public void Chuck()
	{
		Chuck(0f);
	}

	public void Chuck(float chuck)
	{
		if (chuck <= 0f)
		{
			chuck = BOMB_CHUCK_WAIT;
		}
		m_chuckWait = chuck;
	}

	public float GetWait()
	{
		if (!m_hit)
		{
			return m_chuckWait;
		}
		return m_newBlastWait;
	}

	public void SetHit(float wait)
	{
		m_hit = true;
		m_newBlastWait = wait;
	}

	public int ForPlayer()
	{
		return m_forPlayer;
	}

	public void SetForPlayer(int forPlayer)
	{
		m_forPlayer = forPlayer;
	}

	public static int GetNumActiveForPlayer(int player)
	{
		return GetNumActiveForPlayer(player, waitForDeath: true);
	}

	public static int GetNumActiveForPlayer(int player, bool waitForDeath)
	{
		LinkedListNode<Entity> iterator = null;
		Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator);
		int num = 0;
		if (waitForDeath)
		{
			while (bomb != null)
			{
				if ((player == -1 && bomb.ForPlayer() < Game.MAX_PLAYERS) || player == bomb.ForPlayer())
				{
					num++;
				}
				bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator);
			}
		}
		else
		{
			while (bomb != null)
			{
				if (bomb.GetWait() > 0f && !bomb.GetHit())
				{
					num++;
				}
				bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator);
			}
		}
		return num;
	}

	public static void ClearUnspawned()
	{
		LinkedListNode<Entity> iterator = null;
		for (Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); bomb != null; bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			if (bomb.GetWait() > 0f && !bomb.GetHit())
			{
				bomb.KillBomb();
			}
		}
	}

	public static void DeactivateAll()
	{
		LinkedListNode<Entity> iterator = null;
		for (Bomb bomb = (Bomb)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_BOMB, ref iterator); bomb != null; bomb = (Bomb)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_BOMB, ref iterator))
		{
			bomb.Disable();
		}
	}

	public void MakeFat()
	{
		MakeFat(fromLoad: false);
	}

	public void MakeFat(bool fromLoad)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		m_dtMod = 0.666f;
		m_cur_scale *= 1.33f;
		m_orig_scale = m_cur_scale;
		((ColSphere)m_col_box).Radius *= 1.33f;
		if (!fromLoad)
		{
			m_forPlayer += Game.MAX_PLAYERS - 1;
			Vector3 gravity = m_gravity;
			gravity.Normalize();
			bool flag = m_forPlayer == Game.MAX_PLAYERS + 1;
			PSPParticleEmitter pSPParticleEmitter = PSPParticleManager.GetInstance().AddEmitter(StringFunctions.StringHash(flag ? "red_bomb_warning" : "blue_bomb_warning"), null);
			if (pSPParticleEmitter != null)
			{
				pSPParticleEmitter.pos = m_pos;
				pSPParticleEmitter.pos += m_vel * 7.5f;
				pSPParticleEmitter.pos.X = Game.SCREEN_WIDTH * (float)Math.MATH_SIGN(m_pos.X) / 2f;
				Vector2 val = default(Vector2);
				val = new Vector2(m_vel.X, m_vel.Y);
				val.Normalize();
				pSPParticleEmitter.cosz = val.Y;
				pSPParticleEmitter.sinz = val.X;
			}
			SoundManager.GetInstance().SFXPlay("player-bomb-launch");
			Chuck(0.25f);
		}
	}
}
