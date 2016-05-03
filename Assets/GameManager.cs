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

public class GameManager : MonoBehaviour {

	public string map;
	private List<e_Tile> tiles;
	private Stopwatch last_game_tick = new Stopwatch();
	private static int GAME_TICK = 500;

	// Use this for initialization
	void Start () {
		string file = Application.dataPath + "\\" + map;
		try
		{
			string line;
			tiles = new List<e_Tile>();

			StreamReader theReader = new StreamReader(file, Encoding.Default);

			using (theReader)
			{
				do
				{
					line = theReader.ReadLine();
					
					if (line != null)
					{
						char[] entries = line.ToCharArray();
						if (entries.Length > 0)
						{
							foreach(char c in entries)
							{
								if(c == '.')
								{
									tiles.Add(e_Tile.TERRAIN);
								}
								else if (c == 'T')
								{
									tiles.Add(e_Tile.OOB);
								}
							}
						}
					}
				}
				while (line != null);
				theReader.Close();
				UnityEngine.Debug.Log(tiles.Count);
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
		if(last_game_tick.ElapsedMilliseconds >= GAME_TICK)
		{
			UnityEngine.Debug.Log("Tick");
			last_game_tick.Reset();
			last_game_tick.Start();

			//Update entities
		}
	}
}
