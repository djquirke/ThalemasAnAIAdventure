using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum e_TileType
{
	TERRAIN,
	OOB
}

public class Tile {

	e_TileType tile_type;
	Vector2 pos;
	bool occupied = false; //becomes occupied when a building/resource tile/building site is spawned on it, not when a player is on it
	//a tile could have multiple entities on it i.e building and people
	List<Entity> entities = new List<Entity>();
	
	public Tile(e_TileType type, Vector2 pos)
	{
		tile_type = type;
		this.pos = pos;
	}
	
	public e_TileType TileType {
		get { return tile_type; }
	}
	
	public Vector2 Pos {
		get {return pos;}
	}

	public List<Entity> Entities
	{
		get {return entities;}
	}

	public void AddEntity(Entity entity)
	{
		entities.Add(entity);
		if(entity.EntityType != e_EntityType.PERSON) occupied = true;
	}

	public void RemoveEntity(e_EntityType type)
	{
		for(int i = 0; i < entities.Count; i++)
		{
			if(entities[i].EntityType == type) {entities.RemoveAt(i); break;}
		}
	}

	public bool CanBuild()
	{
		if(tile_type == e_TileType.OOB) return false;

		foreach(Entity entity in entities)
		{
			//Building already at location or another building already being built
			if(entity is Building || entity is BuildingSite) return false;
		}

		return true;
	}

	public bool Occupied
	{
		get {return occupied;}
	}
}
