using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    [OnOpenAsset]
    public static bool OnDoubleClick(int instanceID, int line, int row)
    {
        Object instance = EditorUtility.InstanceIDToObject(instanceID);

        if(instance is LevelData) Debug.Log("Level Data Opened");

        return true;
    }
}