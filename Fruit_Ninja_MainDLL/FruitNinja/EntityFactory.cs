using Mortar;

namespace FruitNinja;

public static class EntityFactory
{
	public struct EntityHash(bool u, string h, EntityTypes t)
	{
		public bool update = u;

		public uint hash = StringFunctions.StringHash(h);

		public uint type = (uint)t;
	}

	private static EntityHash[] hashes = new EntityHash[7]
	{
		new EntityHash(u: true, "fruit", EntityTypes.ENTITY_BEGIN),
		new EntityHash(u: true, "bomb", EntityTypes.ENTITY_BOMB),
		new EntityHash(u: true, "slash", EntityTypes.ENTITY_SLASH),
		new EntityHash(u: true, "blast", EntityTypes.ENTITY_BOMB_BLAST),
		new EntityHash(u: true, "coin", EntityTypes.ENTITY_COIN),
		new EntityHash(u: true, "fruitray", EntityTypes.ENTITY_FRUIT_RAY),
		new EntityHash(u: true, "jiblet", EntityTypes.ENTITY_JIBLET)
	};

	public static Entity CreateEntity(int type)
	{
		Entity result = null;
		switch ((EntityTypes)type)
		{
		case EntityTypes.ENTITY_BEGIN:
			result = new Fruit();
			break;
		case EntityTypes.ENTITY_BOMB_BLAST:
			result = new BombBlast();
			break;
		case EntityTypes.ENTITY_BOMB:
			result = new Bomb();
			break;
		case EntityTypes.ENTITY_SLASH:
			result = new SlashEntity();
			break;
		case EntityTypes.ENTITY_COIN:
			result = new Coin();
			break;
		case EntityTypes.ENTITY_FRUIT_RAY:
			result = new FruitRay();
			break;
		case EntityTypes.ENTITY_JIBLET:
			result = new Jiblet();
			break;
		}
		return result;
	}

	public static int HashTypeConvert(uint hash, ref bool update)
	{
		for (int i = 0; i < hashes.Length; i++)
		{
			if (hash == hashes[i].hash)
			{
				update = hashes[i].update;
				return (int)hashes[i].type;
			}
		}
		update = false;
		return -1;
	}
}
