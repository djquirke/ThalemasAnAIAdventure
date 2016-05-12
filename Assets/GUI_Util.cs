using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;

#if UNITY_EDITOR
public class GUI_Util
{
    public static string GetAssetPath()
    {
        string AssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(AssetPath))
        {
            AssetPath = "Assets";
        }
        else if (Path.HasExtension(AssetPath))
        {
            AssetPath = Path.GetDirectoryName(AssetPath);
        }

        Debug.Log(AssetPath);

        return AssetPath;
    }

    public static void AddToCreateMenu<T>(string DefaultAssetName) where T:ScriptableObject
    {
        T Obj = ScriptableObject.CreateInstance<T>();
            
        AssetDatabase.CreateAsset(Obj, GUI_Util.GetAssetPath() + "/" + DefaultAssetName + ".asset");
        AssetDatabase.SaveAssets();

        Selection.activeObject = Obj;
    }

}


/// <attribution>
/// Attributed to http://answers.unity3d.com/users/96865/it3ration.html
/// http://answers.unity3d.com/answers/801283/view.html
/// </attribution>
public class ReadOnlyAttribute : PropertyAttribute
{

}

/// <attribution>
/// Attributed to http://answers.unity3d.com/users/96865/it3ration.html
/// http://answers.unity3d.com/answers/801283/view.html
/// </attribution>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                                SerializedProperty property,
                                GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

#else

    public class ReadOnlyAttribute : PropertyAttribute
{

}

#endif


