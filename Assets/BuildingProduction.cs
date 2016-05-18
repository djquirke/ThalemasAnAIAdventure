using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BuildingProductionType
{
    NONE = 0,
    RESOURCE,
    POPULATION,
    SKILL,
    TOOL,
}

public abstract class BaseBuildingProduction : GUI_Util.ICustomEditor
{
    public abstract int TimeToProduce
    {
        get;
    }

    private List<Vector2> m_Tiles = null;
    public List<Vector2> Tiles
    {
        get { return m_Tiles; }
    }

    public void InitiliseBuildingProduction(List<Vector2> Tiles)
    {
        m_Tiles = Tiles;
    }

    public abstract bool CanProduce(IEnumerable<Person> PeoplePresent, Storage ResourcesInStorage);
    public abstract bool Produce(IEnumerable<Person> PeoplePresent, Storage BuildingStorage);


#if UNITY_EDITOR
    public abstract void OnEditor();
#endif

}


//The only problem with the school production is what to do when two unskilled labarours are teaching and being taught respectively
[System.Serializable]
public class SchoolProduction : BaseBuildingProduction
{
    public int UnskilledTimeToProduce = 100;
    public int SkilledTimeToProduce = 50;

    public PersonType SkillsThatCanBeTaught = PersonType.BLACKSMITH | PersonType.CARPENTER | PersonType.LABOURER | PersonType.LUMBERJACK | PersonType.MINER | PersonType.TRADER;

#if UNITY_EDITOR
    public override void OnEditor()
    {
        UnskilledTimeToProduce  = EditorGUILayout.IntField("Unskilled Teaching Time", UnskilledTimeToProduce);
        SkilledTimeToProduce    = EditorGUILayout.IntField("Skilled Teaching Time", SkilledTimeToProduce);

        SkillsThatCanBeTaught = (PersonType)EditorGUILayout.EnumMaskField("Taught Skills", SkillsThatCanBeTaught);
    }
#endif

    public override int TimeToProduce
    {
        get
        {
            int Time = UnskilledTimeToProduce;
            IEnumerable<Person> People = GameManager.EntitiesOnTiles<Person>(Tiles);
            
            if(People.Count() >= 2 && People.FirstOrDefault(P => P.Type != PersonType.LABOURER) != null)
            {
                Time = SkilledTimeToProduce;
            }

            return Time;
        }
    }

    //This variable is the skill that will be taught when both teacher and student are unskilled labourers
    private PersonType m_SkillToBeTaughtOnDefault = PersonType.LABOURER;
    public PersonType SkillToBeTaughtOnDefault
    {
        get { return m_SkillToBeTaughtOnDefault; }
        set { m_SkillToBeTaughtOnDefault = value; }
    }

    //This function assumes that there are at least two people in PoeplePresent, if there aren't two people it return the default skill to be taught
    public PersonType SkillToBeTaught(IEnumerable<Person> PeoplePresent)
    {
        PersonType Skill = SkillToBeTaughtOnDefault;

        if(PeoplePresent.Count() >= 2)
        {
            //If Teacher is null then both are labourours so use the default skill
            Person Teacher = PeoplePresent.FirstOrDefault(P => P.Type != PersonType.LABOURER);
            if(Teacher != null)
            {
                Skill = Teacher.Type;
            }
        }

        return Skill;
    }

    public override bool CanProduce(IEnumerable<Person> PeoplePresent, Storage BuildingStorgage)
    {
        bool HasResources = false;

        //Must have two poeple, one of which has to be a labourer who will be the student
        if (PeoplePresent.Count() >= 2 && PeoplePresent.FirstOrDefault(P => P.Type == PersonType.LABOURER) != null)
        {
            HasResources = true;
        }

        return HasResources;
    }

    public override bool Produce(IEnumerable<Person> PeoplePresent, Storage BuildingStorage)
    {
        bool HasResources = CanProduce(PeoplePresent, BuildingStorage);

        if(HasResources)
        {
            PersonType Skill = SkillToBeTaught(PeoplePresent);
            Person Student = PeoplePresent.FirstOrDefault(P => P.Type == PersonType.LABOURER);
            
            Student.Type = Skill;
        }

        return HasResources;
    }
}

[System.Serializable]
public class PopulationProduction : BaseBuildingProduction
{
    public int TimeToReproduce = 40;
    public override int TimeToProduce
    {
        get {return TimeToReproduce; }
    }

    public int AmountOfPeopleProduced = 1;

#if UNITY_EDITOR
    public override void OnEditor()
    {
        TimeToReproduce = EditorGUILayout.IntField("Time To Reproduce", TimeToReproduce);
        AmountOfPeopleProduced = EditorGUILayout.IntField("Amount of People Produced", AmountOfPeopleProduced);
    }
#endif

    public override bool CanProduce(IEnumerable<Person> PeoplePresent, Storage ResourcesInStorage)
    {
        return PeoplePresent.Count() >= 2;
    }

    public override bool Produce(IEnumerable<Person> PeoplePresent, Storage BuildingStorage)
    {
        bool HasResources = CanProduce(PeoplePresent, BuildingStorage);

        if(HasResources)
        {
           //TODO: Spawn in Laborours here
        }

        return HasResources;
    }
}


[System.Serializable]
public class ResourceProduction : BaseBuildingProduction
{
    public int TimeToCreate = 20;
    public override int TimeToProduce
    {
        get { return TimeToCreate; }
    }

    public List<ResourceRequirement> ResourcesRequired = new List<ResourceRequirement>();
    public List<WorkerRequirement> WorkersRequired = new List<WorkerRequirement>();
    public List<ResourceRequirement> ResourcesProduced = new List<ResourceRequirement>();

    protected virtual void RemoveResourcesFromStorage(Storage BuildingStorage)
    {
        foreach(ResourceRequirement ResourceToRemove in ResourcesRequired)
        {
            BuildingStorage.TakeResource(ResourceToRemove.RequiredResource, ResourceToRemove.RequiredAmount);
        }
    }

    protected virtual void ProduceResources(Storage BuildingStorage)
    {
        foreach(ResourceRequirement ResourceToProduce in ResourcesProduced)
        {
            BuildingStorage.StoreResource(ResourceToProduce.RequiredResource, ResourceToProduce.RequiredAmount);
        }
    }

    public virtual bool HaveNeededMaterials(Storage Store)
    {
        bool HaveMaterials = true;

        foreach (ResourceRequirement NeededResource in ResourcesRequired)
        {
            if (!Store.CheckResource(NeededResource.RequiredResource, NeededResource.RequiredAmount))
            {
                HaveMaterials = false;
                break;
            }
        }

        return HaveMaterials;
    }

    public virtual bool HaveNeededWorkers(IEnumerable<Person> peopleInBuilding)
    {
        bool HaveWorkers = true;

        foreach (WorkerRequirement WorkersNeeded in WorkersRequired)
        {
            if (peopleInBuilding.Count(P => P.Type == WorkersNeeded.Person) < WorkersNeeded.RequiredWorkers)
            {
                HaveWorkers = false;
                break;
            }
        }

        return HaveWorkers;
    }

    public override bool CanProduce(IEnumerable<Person> PeoplePresent, Storage ResourcesInStorage)
    {
        return HaveNeededMaterials(ResourcesInStorage) && HaveNeededWorkers(PeoplePresent);
    }

    public override bool Produce(IEnumerable<Person> PeoplePresent, Storage BuildingStorage)
    {
        bool HasResources = CanProduce(PeoplePresent, BuildingStorage);

        if(HasResources)
        {
            RemoveResourcesFromStorage(BuildingStorage);
            ProduceResources(BuildingStorage);
        }

        return HasResources;
    }

#if UNITY_EDITOR
    private bool FoldOutRequiredResource = false;
    private bool FoldOutRequiredWorkers = false;
    private bool FoldOutProducedResource = false;

    public override void OnEditor()
    {
        TimeToCreate = EditorGUILayout.IntField("Time to Produce", TimeToCreate);
        FoldOutRequiredResource = GUI_Util.FoldOutListEditor(ResourcesRequired, "Required Resources", FoldOutRequiredResource);
        FoldOutRequiredWorkers  = GUI_Util.FoldOutListEditor(WorkersRequired, "Required Workers", FoldOutRequiredWorkers);
        FoldOutProducedResource = GUI_Util.FoldOutListEditor(ResourcesProduced, "Resources Produced", FoldOutProducedResource);
    }

#endif
}

[System.Serializable]
public class ToolProduction : ResourceProduction
{
    public e_Resource CanProduceResources = e_Resource.NONE;

    private e_Resource m_CurrentlyProducing = e_Resource.NONE;
    public e_Resource CurrentlyProducing
    {
        set
        {
            if ((CanProduceResources & value) != 0 && ((value & (value - 1)) == 0))
            {
                m_CurrentlyProducing = value;
            }
            else
            {
                Debug.LogError("Trying To produce product that can't be produced at this building");
            }
        }

        get
        {
            return m_CurrentlyProducing;
        }
    }

    protected override void ProduceResources(Storage BuildingStorage)
    {
        if (m_CurrentlyProducing != e_Resource.NONE)
        {
            BuildingStorage.StoreResource(CurrentlyProducing, 1);
        }
    }

#if UNITY_EDITOR
    private bool FoldOutRequiredResource = false;
    private bool FoldOutRequiredWorkers = false;

    public override void OnEditor()
    {
        TimeToCreate = EditorGUILayout.IntField("Time to Produce", TimeToCreate);

        CanProduceResources = (e_Resource)EditorGUILayout.EnumMaskField("Resource Produced", CanProduceResources);

        FoldOutRequiredResource = GUI_Util.FoldOutListEditor(ResourcesRequired, "Required Resources", FoldOutRequiredResource);
        FoldOutRequiredWorkers = GUI_Util.FoldOutListEditor(WorkersRequired, "Required Workers", FoldOutRequiredWorkers);
    }

#endif

}