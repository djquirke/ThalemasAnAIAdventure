using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingSite : Entity, IStorage
{
    public BuildingBlueprint BuildingBlueprints;

    public bool BuildingStarted = false;
    private int TicksSinceBuildingStarted = 0;

    private Storage m_Store;
    public Storage Store
    {
        get { return m_Store; }
    }

    public bool HaveNeededMaterials()
    {
        bool HaveMaterials = true;

        foreach(ResourceRequirement NeededResource in BuildingBlueprints.ResourcesRequired)
        {
            if(!Store.CheckResource(NeededResource.RequiredResource, NeededResource.RequiredAmount))
            {
                HaveMaterials = false;
                break;
            }
        }

        return HaveMaterials;
    }

    public bool HaveNeededWorkers()
    {
        bool HaveWorkers = true;

		List<PersonType> peopleInBuilding = GetPeopleInBuilding();

        foreach(WorkerRequirement WorkersNeeded in BuildingBlueprints.WorkersRequired)
        {
            //TODO: Have to have way to check which persons are on the building sites tile
			if(!peopleInBuilding.Contains(WorkersNeeded.Person)) HaveWorkers = false;

        }

        return HaveWorkers;
    }

	List<PersonType> GetPeopleInBuilding()
	{
		//There may be a better way to do this?
		List<Entity> inBuilding = GameManager.EntitiesOnTiles(tiles);
		List<PersonType> peopleInBuilding = new List<PersonType>();
		foreach(Entity entity in inBuilding)
		{
			if(entity is Person)
			{
				peopleInBuilding.Add((entity as Person).Type);
			}
		}
		return peopleInBuilding;
	}

    public bool HasEnoughRoom()
    {
        //TODO: Have some way of checking the tiles around building site
        //Building site will be be top left corner of building
        throw new System.NotImplementedException();
    }

    //Checks to see 
    public bool CanBuild()
    {
        //Will also need to check the builders, can't decide how to do this yet, will check with group on Friday
        //TODO: Will also need to check there's enough space on the map
        return HaveNeededMaterials() && HaveNeededWorkers();
    }

    // Use this for initialization
    void Start()
    {
        m_Store = new Storage(10, 10);
    }

    public override void GameTick()
    {
        if(BuildingStarted)
        {
            //TODO: Probalby do a check for required resources and workers here as well, Any resources stored will be deleted once building is complete
            ++TicksSinceBuildingStarted;

            if(TicksSinceBuildingStarted >= BuildingBlueprints.TimeToBuild)
            {
                //TODO: Building Built, remove site and replace with created building
            }
        }
    }

}

public class Building : Entity, IStorage
{
    private Storage m_Store = new Storage(10, 10);
    public Storage Store
    {
        get
        {
            return m_Store;
        }
    }

    public Vector2 Dimensions = Vector2.zero;



}
