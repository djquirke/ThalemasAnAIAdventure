using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum e_TileType
{
	TERRAIN,
	OOB
}

public class Tile : System.IEquatable<Tile>
{

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

    //Yeah, even I know this is lazy
    public void InitiliseSurroundingTiles()
    {
         Vector2 N = new Vector2(pos.x, pos.y - 1);
        if(GameManager.tiles.ContainsKey(N))
        {
            North = GameManager.tiles[N];
        }

        Vector2 NE = new Vector2(pos.x + 1, pos.y - 1);
        if (GameManager.tiles.ContainsKey(NE))
        {
            NorthEast = GameManager.tiles[NE];
        }

        Vector2 E = new Vector2(pos.x + 1, pos.y);
        if (GameManager.tiles.ContainsKey(E))
        {
            East = GameManager.tiles[E];
        }

        Vector2 SE = new Vector2(pos.x + 1, pos.y + 1);
        if (GameManager.tiles.ContainsKey(SE))
        {
            SouthEast = GameManager.tiles[SE];
        }

        Vector2 S = new Vector2(pos.x, pos.y + 1);
        if (GameManager.tiles.ContainsKey(S))
        {
            South = GameManager.tiles[S];
        }

        Vector2 SW = new Vector2(pos.x - 1, pos.y + 1);
        if (GameManager.tiles.ContainsKey(SW))
        {
            SouthWest = GameManager.tiles[SW];
        }

        Vector2 W = new Vector2(pos.x - 1, pos.y);
        if (GameManager.tiles.ContainsKey(W))
        {
            West = GameManager.tiles[W];
        }

        Vector2 NW = new Vector2(pos.x - 1, pos.y - 1);
        if (GameManager.tiles.ContainsKey(NW))
        {
            NorthWest = GameManager.tiles[NW];
        }
    }

    //Also lazy
    public IEnumerable<Tile> AccessibleSurroundingTiles()
    {
        if(North != null && North.TileType == e_TileType.TERRAIN)
        {
            yield return North;
        }

        if (NorthEast != null && NorthEast.TileType == e_TileType.TERRAIN)
        {
            yield return NorthEast;
        }

        if (East != null && East.TileType == e_TileType.TERRAIN)
        {
            yield return East;
        }

        if (SouthEast != null && SouthEast.TileType == e_TileType.TERRAIN)
        {
            yield return SouthEast;
        }

        if (South != null && South.TileType == e_TileType.TERRAIN)
        {
            yield return South;
        }

        if (SouthWest != null && SouthWest.TileType == e_TileType.TERRAIN)
        {
            yield return SouthWest;
        }

        if (West != null && West.TileType == e_TileType.TERRAIN)
        {
            yield return West;
        }

        if (NorthWest != null && NorthWest.TileType == e_TileType.TERRAIN)
        {
            yield return NorthWest;
        }
    }

    public Tile NorthWest
    {
        get;
        private set;
    }

    public Tile North
    {
        get;
        private set;
    }

    public Tile NorthEast
    {
        get;
        private set;
    }

    public Tile East
    {
        get;
        private set;
    }

    public Tile SouthEast
    {
        get;
        private set;
    }

    public Tile South
    {
        get;
        private set;
    }

    public Tile SouthWest
    {
        get;
        private set;
    }

    public Tile West
    {
        get;
        private set;
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

    public bool Equals(Tile other)
    {
        return pos.Equals(other.pos);
    }
}
