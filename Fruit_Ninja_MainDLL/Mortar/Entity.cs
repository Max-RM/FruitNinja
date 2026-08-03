using System;
using Microsoft.Xna.Framework;

namespace Mortar;

public abstract class Entity
{
	[Flags]
	public enum EntityFlagShifts
	{
		DORMANT = 1,
		UPDATE_ALWAYS = 2,
		ON_PROCESS = 4,
		ON_SCREEN = 8,
		DESTROY = 0x10,
		SELF_CLEAN_UP = 0x20
	}

	public const int ENTITY_INVALID = -1;

	public bool partOfPopup;

	public uint m_id;

	public EntityFlagShifts m_entity_flags;

	public Vector3 m_pos;

	public Vector3 m_vel;

	public Vector3 m_cur_scale;

	public byte m_type;

	public ushort m_dir_angle;

	public Col m_col_box;

	private static uint heapsize;

	public bool m_dormant
	{
		get
		{
			return (m_entity_flags & EntityFlagShifts.DORMANT) != 0;
		}
		set
		{
			m_entity_flags = (EntityFlagShifts)((int)(m_entity_flags & ~EntityFlagShifts.DORMANT) | (value ? 1 : 0));
		}
	}

	public bool m_updateAlways
	{
		get
		{
			return (m_entity_flags & EntityFlagShifts.UPDATE_ALWAYS) != 0;
		}
		set
		{
			m_entity_flags = (EntityFlagShifts)((int)(m_entity_flags & ~EntityFlagShifts.UPDATE_ALWAYS) | (value ? 2 : 0));
		}
	}

	public bool m_onProcess
	{
		get
		{
			return (m_entity_flags & EntityFlagShifts.ON_PROCESS) != 0;
		}
		set
		{
			m_entity_flags = (EntityFlagShifts)((int)(m_entity_flags & ~EntityFlagShifts.ON_PROCESS) | (value ? 4 : 0));
		}
	}

	public bool m_onScreen
	{
		get
		{
			return (m_entity_flags & EntityFlagShifts.ON_SCREEN) != 0;
		}
		set
		{
			m_entity_flags = (EntityFlagShifts)((int)(m_entity_flags & ~EntityFlagShifts.ON_SCREEN) | (value ? 8 : 0));
		}
	}

	public bool m_destroy
	{
		get
		{
			return (m_entity_flags & EntityFlagShifts.SELF_CLEAN_UP) != 0;
		}
		set
		{
			m_entity_flags = (EntityFlagShifts)((int)(m_entity_flags & ~EntityFlagShifts.SELF_CLEAN_UP) | (value ? 32 : 0));
		}
	}

	public bool m_selfCleanUp
	{
		get
		{
			return (m_entity_flags & EntityFlagShifts.DESTROY) != 0;
		}
		set
		{
			m_entity_flags = (EntityFlagShifts)((int)(m_entity_flags & ~EntityFlagShifts.DESTROY) | (value ? 16 : 0));
		}
	}

	public Entity()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_pos = Vector3.Zero;
		m_vel = Vector3.Zero;
	}

	public virtual void Init(byte[] tpl_data, int tpl_size, Vector3? size)
	{
	}

	public virtual void Release()
	{
		m_col_box = null;
	}

	public abstract void Update(float dt);

	public abstract void Draw();

	public abstract void DrawUpdate(float dt);

	public void Update()
	{
		Update(0f);
	}

	public void DrawUpdate()
	{
		DrawUpdate(0f);
	}

	public virtual void PostLoad()
	{
	}

	public virtual bool InRect(ColAABB rect)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (m_col_box != null)
		{
			m_col_box.centre.X = m_pos.X;
			m_col_box.centre.Y = m_pos.Y;
			m_col_box.centre.Z = m_pos.Z;
			Vector3 proj = Vector3.Zero;
			if (m_col_box.Collide(rect, out proj))
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool CollisionResponse(Entity p_ent2, uint col_1, uint col_2, ref Vector3 proj)
	{
		return false;
	}

	public virtual bool Collide(Entity p_ent2, Col col, out uint collide_flags, out Vector3 proj)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		proj = Vector3.Zero;
		collide_flags = 0u;
		if (m_col_box != null)
		{
			return m_col_box.Collide(col, out proj);
		}
		return false;
	}

	public virtual void ReceiveMessage(Entity p_sender, Message message)
	{
		switch (message.message)
		{
		case MessageType.MESSAGE_ACTIVE:
			m_dormant = false;
			break;
		case MessageType.MESSAGE_DORMANT:
			m_dormant = true;
			break;
		}
	}

	public virtual void ListenerCallback(Entity p_sender, Entity p_receiver, Message message)
	{
	}

	public void Activate()
	{
		m_dormant = false;
	}

	public void Deactivate()
	{
		m_destroy = true;
	}

	public static void HeapCreate(uint size)
	{
		heapsize = size;
	}

	public static void HeapCreate()
	{
		heapsize = 268435455u;
	}

	public static void HeapDestroy()
	{
		heapsize = 0u;
	}

	public static void HeapClear()
	{
	}

	public static void HeapDisplay()
	{
	}

	public static void HeapDisplay(bool full)
	{
	}

	public static uint HeapGetSize()
	{
		return heapsize;
	}

	public static uint HeapGetFree()
	{
		return heapsize;
	}

	public static bool HeapExist()
	{
		return heapsize != 0;
	}
}
