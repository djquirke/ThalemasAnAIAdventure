﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;

[System.Serializable]
public struct BuildingPrefab
{
	public string name;
	public GameObject prefab;
}

public class GameManager : MonoBehaviour {

	public string map;
	public GameObject terrain_prefab;
	public GameObject oob_prefab;
	public GameObject set_building_site_prefab;
	public GameObject set_house_prefab;
	public BuildingPrefab[] set_building_prefabs;
	public GameObject coal_prefab, ore_prefab, timber_prefab, stone_prefab;
	public static GameObject building_site_prefab;
	public static GameObject house_prefab;
	public static Dictionary<string, GameObject> building_prefabs;

	private Stopwatch last_game_tick = new Stopwatch();
	
    public static Dictionary<Vector2, Tile> tiles;
	
    private static int GAME_TICK = 500;
	private static int map_width, map_height;
	private static float floor_height = 0.5f;
	private static float building_height = 0.6f;

	private List<Entity> entities;//people, resource tiles, buildings

	void Awake()
	{
		building_site_prefab = set_building_site_prefab;
		house_prefab = set_house_prefab;
		building_prefabs = new Dictionary<string, GameObject>();
		foreach (BuildingPrefab building_prefab in set_building_prefabs) {
			building_prefabs.Add(building_prefab.name, building_prefab.prefab);
		}
	}

	// Use this for initialization
	void Start () {
		entities = new List<Entity> ();

		string file = Application.dataPath + "\\" + map;
		try
		{
			string line;
			tiles = new Dictionary<Vector2, Tile>();

			StreamReader theReader = new StreamReader(file, Encoding.Default);

			using (theReader)
			{
				//offset height counter as the first few lines are garbage
				int counter = -4;
				do
				{
					line = theReader.ReadLine();

					if (line != null)
					{
						map_width = line.Length;

						char[] entries = line.ToCharArray();
						if (entries.Length > 0)
						{
							for(int i = 0; i < entries.Length; i++)
							{
								if(entries[i] == '.')
								{
									Vector2 pos = new Vector2(i, counter);
									Tile temp = new Tile(e_TileType.TERRAIN, pos);
									tiles.Add(pos, temp);
									Instantiate(terrain_prefab, new Vector3(i, floor_height, counter), new Quaternion());
								}
								else if (entries[i] == 'T')
								{
									Vector2 pos = new Vector2(i, counter);
									Tile temp = new Tile(e_TileType.OOB, pos);
									tiles.Add(pos, temp);
									Instantiate(oob_prefab, new Vector3(i, floor_height, counter), new Quaternion());
								}
							}
						}
					}
					counter++;
				}
				while (line != null);
				map_height = counter - 1;
				theReader.Close();
			}

            foreach(KeyValuePair<Vector2, Tile> KVP in tiles)
            {
                KVP.Value.InitiliseSurroundingTiles();
            }

			SpawnRandomResources();

			last_game_tick.Start();
		}
		catch (IOException e)
		{
			UnityEngine.Debug.Log("Error loading map file at: " + file);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//update all entities... may not be needed?
		foreach(Entity entity in entities)
		{
			entity.Update();
		}

		if(last_game_tick.ElapsedMilliseconds >= GAME_TICK)
		{
			last_game_tick.Reset();
			last_game_tick.Start();

			//update entities with new game tick
			foreach(Entity entity in entities)
			{
				entity.GameTick();
			}
		}
	}

	private void SpawnRandomResources()
	{
		List<e_Resource> rawResources = new List<e_Resource>();
		rawResources.Add(e_Resource.COAL);
		rawResources.Add(e_Resource.ORE);
		rawResources.Add(e_Resource.STONE);
		rawResources.Add(e_Resource.TIMBER);

		foreach(e_Resource res in rawResources)
		{
			int x = Random.Range(3, 6);
			for(int i = 0; i < x; i++)
			{
				SpawnResourceAtRandomPos(res);
			}
		}
	}

	void SpawnResourceAtRandomPos(e_Resource res)
	{
		Vector2 spawnPos = GenerateRandomPosition();
		Vector3 worldSpawnPos = new Vector3(spawnPos.x, building_height, spawnPos.y);
		ResourceTile temp = new ResourceTile();
		temp.Initialise(res, spawnPos);
		tiles[spawnPos].AddEntity(temp);
		entities.Add(temp);

		switch (res) {
		case e_Resource.COAL:
			Instantiate(coal_prefab, worldSpawnPos, new Quaternion());
			break;
		case e_Resource.ORE:
			Instantiate(ore_prefab, worldSpawnPos, new Quaternion());
			break;
		case e_Resource.STONE:
			Instantiate(stone_prefab, worldSpawnPos, new Quaternion());
			break;
		case e_Resource.TIMBER:
			Instantiate(timber_prefab, worldSpawnPos, new Quaternion());
			break;
		default:
			break;
		}
	}

	Vector2 GenerateRandomPosition()
	{
		Vector2 ret = new Vector2();
		bool running = true;
		while(running)
		{
			ret.x = Random.Range(0, map_width);
			ret.y = Random.Range(0, map_height);
			if(!tiles[ret].Occupied && tiles[ret].TileType == e_TileType.TERRAIN) running = false;
		}
		return ret;
	}

	//Find all entities at given position
	public static List<Entity> EntitiesAtPos(Vector2 pos)
	{
		return tiles[pos].Entities;
	}

	///Find all entities on a set of tiles
	///Some buildings are bigger than 1 tile
	public static List<Entity> EntitiesOnTiles(List<Vector2> these_tiles)
	{
		List<Entity> ret = new List<Entity>();
		foreach(Vector2 t in these_tiles)
		{
			ret.AddRange(tiles[t].Entities);
		}
		return ret;
	}

    public static IEnumerable<T> EntitiesOnTiles<T>(List<Vector2> these_tiles) where T : Entity
    {
        foreach(Vector2 tile in these_tiles)
        {
            foreach(T t in tiles[tile].Entities.OfType<T>())
            {
                yield return t;
            }
        }
    }

	public static bool CanBuild(Vector2 topLeft, Vector2 dims)
	{
		for(int i = 0; i < dims.x; i++)
		{
			for(int j = 0; j < dims.y; j++)
			{
				Vector2 key = new Vector2(topLeft.x + i, topLeft.y + j);
				if(!tiles[key].CanBuild()) return false;
			}
		}

		return true;
	}

	public static void StartConstruction(Vector2 topLeft, Vector2 dims, Entity entity)
	{
		for(int i = 0; i < dims.x; i++)
		{
			for(int j = 0; j < dims.y; j++)
			{
				Vector2 key = new Vector2(topLeft.x + i, topLeft.y + j);
				tiles[key].AddEntity(entity);
				Instantiate(building_site_prefab, new Vector3(key.x, building_height, key.y), Quaternion.identity);
			}
		}
	}

	public static void FinishConstruction(Vector2 topLeft, Vector2 dims, string name, Building Build)
	{
		GameObject[] constructionTiles = GameObject.FindGameObjectsWithTag("ConstructionTile");

		//remove all construction tiles from the scene that are now becoming buildings
		for(int i = 0; i < dims.x; i++)
		{
			for(int j = 0; j < dims.y; j++)
			{
				Vector3 pos = new Vector3(topLeft.x + i, building_height, topLeft.y + j);
				foreach(GameObject obj in constructionTiles)
				{
					//Only doing it as distance comparison for potential floating point error
					//maybe this won't happen and it can be an equals instead
					if(Vector3.Distance(pos, obj.transform.position) < 0.1f)
					{
						Destroy(obj);
						//update tiles with what it now contains
						Vector2 key = new Vector2(topLeft.x + i, topLeft.y + j);
						tiles[key].RemoveEntity(e_EntityType.BUILDING_SITE);

						tiles[key].AddEntity(Build);
					}
				}
			}
		}

		SpawnBuilding(name, topLeft);
	}

	private static void SpawnBuilding(string name, Vector2 pos)
	{
		Vector3 spawnPos = new Vector3(pos.x, building_height, pos.y);
		Instantiate(building_prefabs[name], spawnPos, Quaternion.identity);
	}

	public static Dictionary<Vector2, Tile> WorldState
	{
		get {return tiles;}
		set {tiles = value;}
	}
}
