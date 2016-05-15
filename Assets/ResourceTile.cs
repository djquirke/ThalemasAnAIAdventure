using UnityEngine;
using System.Collections;

public class ResourceTile : Entity {

	private e_Resource resource;
	private int resource_remaining = 5;
	private int max_resource_count = 5;
	private bool depleted = false;
	private int time_since_last_replenish = 0;
	private int time_to_replenish = 10;
	private Vector2 dims = new Vector2(1, 1);

	public void Initialise(e_Resource resource, Vector2 pos)
	{
		this.resource = resource;

		this.Position = new Vector2 (pos.x, pos.y);
	}

	public e_Resource Deplete()
	{
		if(depleted)
			return e_Resource.NONE;

		resource_remaining--;
		if (resource_remaining == 0)
			depleted = true;
		return resource;
	}

	public override void GameTick()
	{
		time_since_last_replenish++;
		if (time_since_last_replenish >= time_to_replenish) {
			time_since_last_replenish = 0;
			if(resource_remaining < max_resource_count)
				resource_remaining++;
			depleted = false;
		}
	}

	public e_Resource GetResourceType() {return resource;}
}
