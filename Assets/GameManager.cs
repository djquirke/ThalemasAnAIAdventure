using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;  

enum TILE
{
	TERRAIN,
	OOB
}

public class GameManager : MonoBehaviour {

	public string map;
	private List<TILE> tiles;

	// Use this for initialization
	void Start () {

		try
		{
			string line;
			tiles = new List<TILE>();

			StreamReader theReader = new StreamReader(map, Encoding.Default);

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
									tiles.Add(TILE.TERRAIN);
								}
								else if (c == 'T')
								{
									tiles.Add(TILE.OOB);
								}
							}
						}
					}
				}
				while (line != null);
				theReader.Close();
				Debug.Log(tiles.Count);
			}
		}
		catch (IOException e)
		{
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
