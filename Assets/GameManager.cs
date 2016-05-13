using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;  

enum e_Tile
{
	TERRAIN,
	OOB
}

struct s_Tile
{
	e_Tile tile_type;
	Vector2 pos;

	public s_Tile(e_Tile type, Vector2 pos)
	{
		tile_type = type;
		this.pos = pos;
	}
}

public class GameManager : MonoBehaviour {

	public string map;
	private List<s_Tile> tiles;
	private Stopwatch last_game_tick = new Stopwatch();
	private static int GAME_TICK = 500;

	private List<Entity> entities;//people, resource tiles, buildings

	// Use this for initialization
	void Start () {
		string file = Application.dataPath + "\\" + map;
		try
		{
			string line;
			tiles = new List<s_Tile>();

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
						char[] entries = line.ToCharArray();
						if (entries.Length > 0)
						{
							for(int i = 0; i < entries.Length; i++)
							{
								if(entries[i] == '.')
								{
									s_Tile temp = new s_Tile(e_Tile.TERRAIN, new Vector2(i, counter));
									tiles.Add(temp);
								}
								else if (entries[i] == 'T')
								{
									s_Tile temp = new s_Tile(e_Tile.OOB, new Vector2(i, counter));
									tiles.Add(temp);
								}
							}
						}
					}
					counter++;
				}
				while (line != null);
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
			UnityEngine.Debug.Log("Tick");
			last_game_tick.Reset();
			last_game_tick.Start();

			//update entities with new game tick
			foreach(Entity entity in entities)
			{
				entity.GameTick();
			}
		}
	}
}
