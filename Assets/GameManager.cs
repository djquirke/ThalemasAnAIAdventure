using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;  

public class GameManager : MonoBehaviour {

	public string map;
	public GameObject terrain_prefab;
	public GameObject oob_prefab;

	private static Dictionary<Vector2, Tile> tiles;
	private Stopwatch last_game_tick = new Stopwatch();
	private static int GAME_TICK = 500;
	private int map_width, map_height;

	private List<Entity> entities;//people, resource tiles, buildings

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
									Instantiate(terrain_prefab, new Vector3(i, 0.5f, counter), new Quaternion());
								}
								else if (entries[i] == 'T')
								{
									Vector2 pos = new Vector2(i, counter);
									Tile temp = new Tile(e_TileType.OOB, pos);
									tiles.Add(pos, temp);
									Instantiate(oob_prefab, new Vector3(i, 0.5f, counter), new Quaternion());
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

	//Find all entities at given position
	public static List<Entity> EntitiesAtPos(Vector2 pos)
	{
		return tiles[pos].Entities;
	}

	///Find all entities on a set of tiles
	///Some buildings are bigger than 1 tile
	public static List<Entity> EntitiesOnTiles(List<Tile> these_tiles)
	{
		List<Entity> ret = new List<Entity>();
		foreach(Tile t in these_tiles)
		{
			ret.AddRange(tiles[t.Pos].Entities);
		}
		return ret;
	}
}
