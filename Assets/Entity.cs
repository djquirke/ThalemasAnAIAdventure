using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity {
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}

	//the tiles that the entity covers
	protected List<Tile> tiles = new List<Tile>();
	public Vector2 Position = Vector2.zero;

	public virtual void GameTick() {}
	public virtual void Update() {}
}
