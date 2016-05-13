using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ResourceRequirement
{
    public int RequiredAmount;
    public e_Resource RequiredResource;

    public ResourceRequirement()
    {
        RequiredAmount = 0;
        RequiredResource = e_Resource.NONE;
    }

#if UNITY_EDITOR
    public void ResourceRequirementsEditor()
    {
        RequiredAmount = EditorGUILayout.IntField("Number Required", RequiredAmount);
        RequiredResource = (e_Resource)EditorGUILayout.EnumPopup("Resource Type", RequiredResource);
    }
#endif
}

[System.Serializable]
public class WorkerRequirement
{
    public int RequiredWorkers;
    public PersonType Person;

    public WorkerRequirement()
    {
        RequiredWorkers = 0;
        Person = PersonType.LABOURER;
    }

#if UNITY_EDITOR
    public void WorkerRequirementsEditor()
    {
        RequiredWorkers = EditorGUILayout.IntField("Number Required", RequiredWorkers);
        Person = (PersonType)EditorGUILayout.EnumPopup("Woker Type", Person);
    }
#endif

}

public interface IProduction
{
    int TimeToProduce
    {
        get;
    }

    //TODO: Tiles have to be done by something other than positiion
    bool CanProduce(IStorage ResourceStore, Vector2 TilePosition);

    bool Produce(IStorage ResourceStore, Vector2 TilePosition);
}

public class SchoolProduction : ScriptableObject, IProduction
{
    public int TimeRequired = 10;
    public int TimeToProduce
    {
        get { return TimeRequired; }
    }

    bool CanProduce(IStorage ResourceStore, Vector2 TilePosition)
    {
        //TODO: Need to check what workers are on the tile

        //Then we'll
        return false;
    }

    bool Produce(IStorage ResourceStore, Vector2 TilePosition)
    {
        bool Produced = CanProduce(ResourceStore, TilePosition);

        if(Produced)
        {

        }

        return Produced;
    }
}

public class BuildingProduction : ScriptableObject
{
    public int TimeToProdcue = 10;

    public List<ResourceRequirement> ResourcesRequired = new List<ResourceRequirement>();
    public List<WorkerRequirement> WorkersRequired = new List<WorkerRequirement>();

    public List<ResourceRequirement> ResourcesProduced = new List<ResourceRequirement>();
    
}

//TODO: Add tiles bitmask, The building action bit (i.e. resource requirements, worker requirements, and then something is produced)
public class BuildingBlueprint : ScriptableObject
{
    public int TimeToBuild = 10;

    public Vector2 Dimensions = Vector2.one;

    public List<ResourceRequirement> ResourcesRequired = new List<ResourceRequirement>();
    public List<WorkerRequirement> WorkersRequired = new List<WorkerRequirement>();

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/New Building")]
    public static void AddBuildingToCreateMenu()
    {
        GUI_Util.AddToCreateMenu<BuildingBlueprint>("NewBuilding");
    }

#endif

}

#if UNITY_EDITOR
[CustomEditor(typeof(BuildingBlueprint))]
public class BuildingRequirementWindowEditor : Editor
{
    private BuildingBlueprint m_Req = null;

    private bool ShowResourceList = false;
    private bool ShowPeopleList = false;

    void Awake()
    {
        m_Req = target as BuildingBlueprint;
    }

    void OnEnable()
    {
        m_Req = target as BuildingBlueprint;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        m_Req.name = EditorGUILayout.TextField("Name", m_Req.name);

        if (GUI.changed)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(Selection.activeObject), m_Req.name);
        }

        m_Req.Dimensions = EditorGUILayout.Vector2Field("Dimensions", m_Req.Dimensions);
        m_Req.TimeToBuild = EditorGUILayout.IntField("Time To Build", m_Req.TimeToBuild);

        ShowResourceList = EditorGUILayout.Foldout(ShowResourceList, "Required Resources");

        if (ShowResourceList)
        {
            int Size = EditorGUILayout.IntField("Size", m_Req.ResourcesRequired.Count);

            if (Size > m_Req.ResourcesRequired.Count)
            {
                //There's definetly a less stupid way of doing this.
                while (Size > m_Req.ResourcesRequired.Count)
                {
                    m_Req.ResourcesRequired.Add(new ResourceRequirement());
                }
            }
            else if(Size < m_Req.ResourcesRequired.Count)
            {
                m_Req.WorkersRequired.RemoveRange(Size, m_Req.ResourcesRequired.Count - Size);
            }

            foreach(ResourceRequirement ResourcesNeeded in m_Req.ResourcesRequired)
            {
                EditorGUILayout.Separator();
                ResourcesNeeded.ResourceRequirementsEditor();
            }
            EditorGUILayout.Separator();
        }

        ShowPeopleList = EditorGUILayout.Foldout(ShowPeopleList, "Required Workers");

        if (ShowPeopleList)
        {
            int Size = EditorGUILayout.IntField("Size", m_Req.WorkersRequired.Count);

            if (Size > m_Req.WorkersRequired.Count)
            {
                //There's definetly a less stupid way of doing this.
                while (Size > m_Req.WorkersRequired.Count)
                {
                    m_Req.WorkersRequired.Add(new WorkerRequirement());
                }
            }
            else if (Size < m_Req.WorkersRequired.Count)
            {
                m_Req.WorkersRequired.RemoveRange(Size, m_Req.WorkersRequired.Count - Size);
            }

            foreach(WorkerRequirement WorkersNeeded in m_Req.WorkersRequired)
            {
                EditorGUILayout.Separator();
                WorkersNeeded.WorkerRequirementsEditor();
            }
            EditorGUILayout.Separator();
        }

    }
}

#endif
