using UnityEngine;
using System.Collections;

public enum e_Resource
{
	COAL,
	ORE,
	WOOD,
	STONE,
	TIMBER,
	IRON,
	RIFLE,
	CART,
	AXE,
	NONE
}

public class Resource : MonoBehaviour {

	public e_Resource resource_type;

	public void Initialise(e_Resource type)
	{
		resource_type = type;
	}
}
