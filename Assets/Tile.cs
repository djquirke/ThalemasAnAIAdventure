using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum e_TileType
{
	TERRAIN,
	OOB
}

public class Tile : MonoBehaviour {

	e_TileType tile_type;
	Vector2 pos;
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
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
