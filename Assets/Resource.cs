using UnityEngine;
using System.Collections;

[System.Flags]
public enum e_Resource
{
    NONE    = 0,
	COAL    = 1,
	ORE     = 2,
	WOOD    = 4,
	STONE   = 8,
	TIMBER  = 16,
	IRON    = 32,
	RIFLE   = 64,
	CART    = 128,
	AXE     = 256,
}

public class Resource : MonoBehaviour {

	public e_Resource resource_type;

	public void Initialise(e_Resource type)
	{
		resource_type = type;
	}
}
