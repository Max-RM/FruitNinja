using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class ActorManager
{
	public delegate Entity ActorFactory(int n);

	public delegate int ActorTypeHashConvert(uint hash, ref bool b);

	public const int MAX_NEW_ENTITIES = 512;

	public const int MAX_OLD_ENTITIES = 512;

	protected uint m_heap;

	protected int m_heap_size;

	protected Entity[] m_inactivelist = new Entity[512];

	protected uint m_inactiveCount;

	protected Entity[] m_kill_list = new Entity[512];

	protected uint m_killListSize;

	protected LinkedList<Entity>[] m_entities;

	protected LinkedList<MessageListener> m_listeners = new LinkedList<MessageListener>();

	protected int m_max_entities;

	protected byte m_collision_visible;

	protected bool PAD00;

	protected byte m_filler0;

	protected byte m_filler1;

	protected ActorFactory m_factory;

	protected ActorTypeHashConvert m_typeConverter;

	private static ActorManager instance = new ActorManager();

	private ActorManager()
	{
	}

	private void Deactivate(Entity p_ent)
	{
		for (LinkedListNode<Entity> linkedListNode = m_entities[p_ent.m_type].First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value == p_ent)
			{
				m_entities[p_ent.m_type].Remove(linkedListNode);
				m_inactivelist[m_inactiveCount++] = p_ent;
				break;
			}
		}
	}

	public static ActorManager GetInstance()
	{
		return instance;
	}

	public void Initialise(int max_Entities)
	{
		Initialise(max_Entities, 1000);
	}

	public void Initialise(int max_Entities, int heap_size)
	{
		m_heap = 1u;
		m_heap_size = heap_size;
		m_max_entities = max_Entities;
		m_entities = ArrayInit.CreateFilledArray<LinkedList<Entity>>(m_max_entities);
		for (int i = 0; i < m_max_entities; i++)
		{
			m_entities[i] = new LinkedList<Entity>();
		}
		Entity.HeapExist();
	}

	public void PostLoad()
	{
		for (int i = 0; i < m_max_entities; i++)
		{
			foreach (Entity item in m_entities[i])
			{
				item.PostLoad();
			}
		}
	}

	public void Destroy()
	{
		Clear();
		m_heap = 0u;
		m_heap_size = 0;
		m_entities = null;
		m_inactiveCount = 0u;
		m_killListSize = 0u;
	}

	public int GetHeapFree()
	{
		return m_heap_size;
	}

	public int GetHeapSize()
	{
		return m_heap_size;
	}

	public void HeapDisplay()
	{
		HeapDisplay(full: false);
	}

	public void HeapDisplay(bool full)
	{
		throw new MissingMethodException();
	}

	public bool LoadEntity(EntityChunk p_chunk, byte[] tpl_data, int tpl_size, int pos_scale)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		bool b = false;
		int num = m_typeConverter(p_chunk.type_hash, ref b);
		if (-1 == num)
		{
			return false;
		}
		Entity entity = Add(num, b);
		if (entity == null)
		{
			return false;
		}
		entity.m_id = p_chunk.name_hash;
		Vector3 val = default(Vector3);
		val = new Vector3(p_chunk.volume_max.X * (float)(1 << pos_scale), p_chunk.volume_max.Y * (float)(1 << pos_scale), p_chunk.volume_max.Z * (float)(1 << pos_scale));
		Vector3 val2 = default(Vector3);
		val2 = new Vector3(p_chunk.volume_min.X * (float)(1 << pos_scale), p_chunk.volume_min.Y * (float)(1 << pos_scale), p_chunk.volume_min.Z * (float)(1 << pos_scale));
		Vector3 cur_scale = val - val2;
		Vector3 pos = val2 + new Vector3(cur_scale.X / 2f, cur_scale.Y / 2f, cur_scale.Z / 2f);
		entity.m_dir_angle = (ushort)p_chunk.rotate.Y;
		entity.m_pos = pos;
		entity.m_cur_scale = cur_scale;
		entity.m_dormant = BitConverter.ToUInt32(tpl_data, 0) != 0;
		tpl_size -= 4;
		tpl_data.CopyTo(tpl_data, 4);
		entity.Init(tpl_data, tpl_size, p_chunk.scale);
		entity.m_id = p_chunk.name_hash;
		return true;
	}

	public void Update(float dt)
	{
		Update(dt, null, null);
	}

	public void Update(float dt, ColAABB draw_rect, ColAABB process_rect)
	{
		if (m_heap != 0 && m_entities != null)
		{
			for (int i = 0; i < m_max_entities; i++)
			{
				foreach (Entity item in m_entities[i])
				{
					if (!item.m_dormant && !item.m_destroy)
					{
						item.m_onProcess = true;
						item.m_onScreen = true;
						item.Update(dt);
						item.DrawUpdate(dt);
					}
					if (item.m_destroy)
					{
						m_kill_list[m_killListSize++] = item;
					}
				}
			}
		}
		for (int j = 0; j < m_killListSize; j++)
		{
			Deactivate(m_kill_list[j]);
		}
		m_killListSize = 0u;
	}

	public void DeactivateAllEntities(int type)
	{
		foreach (Entity item in m_entities[type])
		{
			item.m_destroy = true;
		}
	}

	public void RegisterFactory(ActorFactory factory)
	{
		m_factory = factory;
	}

	public void UnregisterFactory()
	{
		m_factory = null;
	}

	public void RegisterHashConverter(ActorTypeHashConvert convert)
	{
		m_typeConverter = convert;
	}

	public void UnregisterHashConverter()
	{
		m_typeConverter = null;
	}

	public void AddMessageListener(MessageListener listen)
	{
		m_listeners.AddLast(listen);
	}

	public void RemoveMessageListener(MessageListener listen)
	{
		m_listeners.Remove(listen);
	}

	public void ClearAllListeners()
	{
		for (LinkedListNode<MessageListener> linkedListNode = m_listeners.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			MessageListener dl = linkedListNode.Value;
			m_listeners.Remove(linkedListNode);
			Delete.SAFE_DELETE(ref dl);
		}
		m_listeners.Clear();
	}

	public void Draw()
	{
		if (m_entities == null)
		{
			return;
		}
		for (int i = 0; i < m_max_entities; i++)
		{
			foreach (Entity item in m_entities[i])
			{
				if (!item.m_dormant && !item.m_destroy)
				{
					item.Draw();
				}
			}
		}
	}

	public Entity Add(int type)
	{
		return Add(type, update_entity: true);
	}

	public Entity Add(int type, bool update_entity)
	{
		return Add((EntityTypes)type, update_entity: true);
	}

	public Entity Add(Entity p_entity, int type)
	{
		if (p_entity != null)
		{
			m_entities[type].AddLast(p_entity);
			p_entity.m_type = (byte)type;
		}
		return p_entity;
	}

	public Entity Add(EntityTypes type)
	{
		return Add(type, update_entity: true);
	}

	public Entity Add(EntityTypes type, bool update_entity)
	{
		Entity entity;
		for (int num = (int)(m_inactiveCount - 1); num >= 0; num--)
		{
			entity = m_inactivelist[num];
			if ((EntityTypes)entity.m_type == type)
			{
				m_entities[(int)type].AddLast(entity);
				m_inactiveCount--;
				for (int i = num; i < m_inactiveCount; i++)
				{
					m_inactivelist[i] = m_inactivelist[i + 1];
				}
				entity.Activate();
				return entity;
			}
		}
		entity = m_factory((int)type);
		if (entity != null)
		{
			m_entities[(int)type].AddLast(entity);
			entity.m_type = (byte)type;
		}
		return entity;
	}

	public Entity Add(Entity p_entity, EntityTypes type)
	{
		return Add(p_entity, (int)type);
	}

	public void Remove(Entity p_ent)
	{
		foreach (Entity item in m_entities[p_ent.m_type])
		{
			if (item == p_ent)
			{
				if (!item.m_selfCleanUp)
				{
					item.Release();
				}
				m_entities[p_ent.m_type].Remove(item);
				break;
			}
		}
	}

	public void Clear()
	{
		m_killListSize = 0u;
		if (m_heap == 0 || m_entities == null)
		{
			return;
		}
		for (int i = 0; i < m_max_entities; i++)
		{
			foreach (Entity item in m_entities[i])
			{
				if (!item.m_selfCleanUp)
				{
					item.Release();
				}
			}
			m_entities[i].Clear();
		}
		for (int j = 0; j < m_inactiveCount; j++)
		{
			Entity entity = m_inactivelist[j];
			entity.Release();
		}
		m_inactiveCount = 0u;
	}

	public uint GetNumEntities()
	{
		int num = 0;
		for (int i = 0; i < m_max_entities; i++)
		{
			num += m_entities[i].Count;
		}
		return (uint)num;
	}

	public uint GetNumEntities(int type)
	{
		return (uint)m_entities[type].Count;
	}

	public uint GetNumEntities(int type_start, int type_end)
	{
		if (type_start > type_end)
		{
			Mortar.Math.Swap(ref type_start, ref type_end);
		}
		int num = 0;
		for (int i = type_start; i < type_end; i++)
		{
			num += m_entities[i].Count;
		}
		return (uint)num;
	}

	public uint GetNumEntities(int[] types)
	{
		int num = 0;
		for (int i = 0; types[i] != -1; i++)
		{
			int num2 = types[i];
			if (num2 != -1)
			{
				num += m_entities[num2].Count;
			}
		}
		return (uint)num;
	}

	public uint GetNumTypes()
	{
		int num = 0;
		for (int i = 0; i < m_max_entities; i++)
		{
			if (m_entities[i].Count > 0)
			{
				num++;
			}
		}
		return (uint)num;
	}

	public Entity Find(uint id)
	{
		for (int i = 0; i < m_max_entities; i++)
		{
			foreach (Entity item in m_entities[i])
			{
				if (item.m_id == id)
				{
					return item;
				}
			}
		}
		return null;
	}

	public Entity Find(EntityTypes type, uint id)
	{
		foreach (Entity item in m_entities[(int)type])
		{
			if (item.m_id == id)
			{
				return item;
			}
		}
		return null;
	}

	public Entity GetEntity(EntityTypes type, uint idx)
	{
		uint num = 0u;
		foreach (Entity item in m_entities[(int)type])
		{
			if (num == idx)
			{
				return item;
			}
			num++;
		}
		return null;
	}

	public Entity GetEntityFirst(EntityTypes type, ref LinkedListNode<Entity> iterator)
	{
		iterator = m_entities[(int)type].First;
		if (iterator != null)
		{
			return iterator.Value;
		}
		return null;
	}

	public Entity GetEntityNext(EntityTypes type, ref LinkedListNode<Entity> iterator)
	{
		iterator = iterator.Next;
		if (iterator != null)
		{
			return iterator.Value;
		}
		return null;
	}

	public int GetEntityIdx(Entity p_ent)
	{
		int type = p_ent.m_type;
		if (Mortar.Math.BETWEEN(type, 0, m_max_entities - 1))
		{
			int num = 0;
			foreach (Entity item in m_entities[type])
			{
				if (p_ent == item)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public bool SendMessage(uint id, Entity src, Message msg)
	{
		Entity entity = Find(id);
		foreach (MessageListener listener in m_listeners)
		{
			if (listener.message == (uint)msg.message && (listener.to_id == 0 || listener.to_id == id) && (listener.from_id == 0 || (src != null && listener.from_id == src.m_id)))
			{
				listener.p_target.ListenerCallback(src, entity, msg);
			}
		}
		m_listeners.Clear();
		if (entity != null)
		{
			entity.ReceiveMessage(src, msg);
			return true;
		}
		return false;
	}

	public uint GetNumInAABB(ColAABB aabb)
	{
		return 0u;
	}

	public uint GetNumInProcess()
	{
		return 0u;
	}

	public uint GetNumInDraw()
	{
		return 0u;
	}

	public void DisplayUsage()
	{
		DisplayUsage(full: false);
	}

	public void DisplayUsage(bool full)
	{
		throw new MissingMethodException();
	}

	public void DrawDebug()
	{
		throw new MissingMethodException();
	}

	public void SetCollisionVisible(byte state)
	{
		throw new MissingMethodException();
	}

	public byte GetCollisionVisible()
	{
		return m_collision_visible;
	}
}
