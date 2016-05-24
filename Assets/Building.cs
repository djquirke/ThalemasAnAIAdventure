using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        IEnumerable<Person> peopleInBuilding = GameManager.EntitiesOnTiles<Person>(tiles);

        foreach(WorkerRequirement WorkersNeeded in BuildingBlueprints.WorkersRequired)
        {
            if(peopleInBuilding.Count(P => P.Type == WorkersNeeded.Person) < WorkersNeeded.RequiredWorkers)
            {
                HaveWorkers = false;
                break;
            }
        }

        return HaveWorkers;
    }


    public bool HasEnoughRoom()
    {
        //TODO: Have some way of checking the tiles around building site
        //Building site will be top left corner of building

		//TODO: need to know which position to want to start building, replace the (5, 5) vector with top left pos of building
		return GameManager.CanBuild(new Vector2(5, 5), BuildingBlueprints.Dimensions);
    }

    public bool CanBuild()
    {
        //Will also need to check the builders, can't decide how to do this yet, will check with group on Friday
        //TODO: Will also need to check there's enough space on the map
		//Done
        return HaveNeededMaterials() && HaveNeededWorkers() && HasEnoughRoom();
    }

    public BuildingSite()
    {
        m_Store = new Storage(10, 10);
		entityType = e_EntityType.BUILDING_SITE;
    }

    public override void GameTick()
    {
        if(BuildingStarted)
        {
            //TODO: Probably do a check for required resources and workers here as well, Any resources stored will be deleted once building is complete
            ++TicksSinceBuildingStarted;

            if(TicksSinceBuildingStarted >= BuildingBlueprints.TimeToBuild)
            {
                //TODO: Building Built, remove site and replace with created building
				//use GameManager.FinishConstruction(top left position, BuildingBlueprints.Dimensions, BuildingBlueprints.name)
                Building Built = new Building(BuildingBlueprints);
                GameManager.FinishConstruction(this.Position, BuildingBlueprints.Dimensions, BuildingBlueprints.name, Built);
            }
        }
    }

}

public class Building : Entity, IStorage
{
    private Storage m_Store = new Storage(1000, 10000);
    public Storage Store
    {
        get
        {
            return m_Store;
        }
    }

    private Vector2 m_Dimensions = Vector2.zero;
    public Vector2 Dimensions
    {
        get
        {
            return m_Dimensions;
        }
    }

    private BuildingProductionType m_BuildingType;
    public BuildingProductionType BuildingType
    {
        get
        {
            return m_BuildingType;
        }
    }

    private BaseBuildingProduction m_BuildingProduction;
    public BaseBuildingProduction BuildingProduction
    {
        get
        {
            return m_BuildingProduction;
        }
    }

    public bool Produce()
    {
        IEnumerable<Person> PersonsOnSite = GameManager.EntitiesOnTiles<Person>(tiles);
        bool CanProduce = BuildingProduction.CanProduce(PersonsOnSite, this.Store);

        if(CanProduce)
        {
            BuildingProduction.Produce(PersonsOnSite, this.Store);
        }

        return CanProduce;
    }

	public Building(BuildingBlueprint Blueprints)
	{
        m_Dimensions = Blueprints.Dimensions;
        m_BuildingType = Blueprints.BuildingProduces;
        m_BuildingProduction = Blueprints.BuildingProduction;

		entityType = e_EntityType.BUILDING;
	}

}
