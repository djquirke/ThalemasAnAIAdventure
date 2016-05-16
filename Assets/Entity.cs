using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum e_EntityType
{
	BUILDING,
	BUILDING_SITE,
	PERSON,
	RESOURCE
}

public class Entity {

	//the tiles that the entity covers
	protected List<Vector2> tiles = new List<Vector2>();
	protected e_EntityType entityType;
	//top left position
	public Vector2 Position = Vector2.zero;

	public virtual void GameTick() {}
	public virtual void Update() {}

	public e_EntityType EntityType
	{
		get {return entityType;}
	}
}
