using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ResourceRequirement : GUI_Util.ICustomEditor
{
    public int RequiredAmount;
    public e_Resource RequiredResource;

    public ResourceRequirement()
    {
        RequiredAmount = 0;
        RequiredResource = e_Resource.NONE;
    }

#if UNITY_EDITOR
    public void OnEditor()
    {
        RequiredAmount = EditorGUILayout.IntField("Number Required", RequiredAmount);
        RequiredResource = (e_Resource)EditorGUILayout.EnumPopup("Resource Type", RequiredResource);
    }
#endif
}


[System.Serializable]
public class WorkerRequirement : GUI_Util.ICustomEditor
{
    public int RequiredWorkers;
    public PersonType Person;

    public WorkerRequirement()
    {
        RequiredWorkers = 0;
        Person = PersonType.LABOURER;
    }

#if UNITY_EDITOR
    public void OnEditor()
    {
        RequiredWorkers = EditorGUILayout.IntField("Number Required", RequiredWorkers);
        Person = (PersonType)EditorGUILayout.EnumPopup("Worker Type", Person);
    }
#endif

}

//TODO: Add tiles bitmask, The building action bit (i.e. resource requirements, worker requirements, and then something is produced)
public class BuildingBlueprint : ScriptableObject
{
    public int TimeToBuild = 10;

    public Vector2 Dimensions = Vector2.one;

    public List<ResourceRequirement> ResourcesRequired = new List<ResourceRequirement>();
    public List<WorkerRequirement> WorkersRequired = new List<WorkerRequirement>();

    public BuildingProductionType BuildingProduces = BuildingProductionType.NONE;
    
    public BaseBuildingProduction BuildingProduction
    {
        get
        {
            BaseBuildingProduction Production = null;
            switch(BuildingProduces)
            {
                case BuildingProductionType.POPULATION:
                    Production = m_PopulationProduction;
                    break;
                case BuildingProductionType.RESOURCE:
                    Production = m_ResourceProduction;
                    break;
                case BuildingProductionType.SKILL:
                    Production = m_SchoolProduction;
                    break;
                case BuildingProductionType.TOOL:
                    Production = m_ToolProduction;
                    break;
            }

            return Production;
        }
    }

    [SerializeField][HideInInspector]
    private SchoolProduction m_SchoolProduction = new SchoolProduction();
    
    [SerializeField][HideInInspector]
    private ResourceProduction m_ResourceProduction = new ResourceProduction();
    
    [SerializeField][HideInInspector]
    private PopulationProduction m_PopulationProduction = new PopulationProduction();

    [SerializeField][HideInInspector]
    private ToolProduction m_ToolProduction = new ToolProduction();

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

    private bool ShowBuildingProduction = false;
    private bool ShowBuildingRequirements = false;

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

        EditorGUILayout.Separator();
        ShowBuildingRequirements = EditorGUILayout.Foldout(ShowBuildingRequirements, "Requirements to Build");

        if (ShowBuildingRequirements)
        {
            m_Req.TimeToBuild = EditorGUILayout.IntField("Time To Build", m_Req.TimeToBuild);

            ShowResourceList = GUI_Util.FoldOutListEditor(m_Req.ResourcesRequired, "Required Resources", ShowResourceList);
            ShowPeopleList = GUI_Util.FoldOutListEditor(m_Req.WorkersRequired, "Required Workers", ShowPeopleList);
        }
        EditorGUILayout.Separator();
        ShowBuildingProduction = EditorGUILayout.Foldout(ShowBuildingProduction, "Building Production");
        
        if (ShowBuildingProduction)
        {
            m_Req.BuildingProduces = (BuildingProductionType)EditorGUILayout.EnumPopup("Building Production Type", m_Req.BuildingProduces);

            if (m_Req.BuildingProduction != null)
            {
                m_Req.BuildingProduction.OnEditor();
            }
        }
    }
}

#endif
