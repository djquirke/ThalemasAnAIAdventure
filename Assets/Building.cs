using UnityEngine;
using System.Collections;

public class BuildingSite : Entity, IStorage
{
    public BuildingBlueprint BuildingBlueprints;
    public Vector2 Position = Vector2.zero;

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

        foreach(WorkerRequirement WorkersNeeded in BuildingBlueprints.WorkersRequired)
        {
            //TODO: Have to have way to check which persons are on the building sites tile
        }

        return HaveWorkers;
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
