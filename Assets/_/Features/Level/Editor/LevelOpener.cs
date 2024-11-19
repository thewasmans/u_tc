using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Level.Data;

namespace Level.Editor
{
    public class LevelOpener : MonoBehaviour
    {
        [OnOpenAsset]
        public static bool OnDoubleClick(int instanceID, int line, int row)
        {
            Object target = EditorUtility.InstanceIDToObject(instanceID);
            
            switch (target)
            {
                case LevelData levelData:
                    levelData.OpenLevelEditor();
                    return true;

                default:
                    return false;
            }
        }
    }
}
