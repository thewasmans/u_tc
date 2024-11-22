using System.Collections.Generic;
using System.Linq;
using Level.Data;
using Misc.Runtime;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace ToolBarExtender.Editor
{
    [MainToolbarElement(id: "LevelSelector", ToolbarAlign.Left)]
    public class LevelSelector : DropdownField
    {
        public void InitializeElement()
        {
            label = "Level Selector";

            EditorApplication.projectChanged += RefreshLevelSelection;
            RefreshLevelSelection();
        }

        public void RefreshLevelSelection()
        {
            string[] levelDataGuids = AssetDatabase.FindAssets($"t:{typeof(LevelData)}", new []{GlobalPaths.LEVEL_FOLDER});
            
            choices = new List<string>();

            for (int i = 0; i < levelDataGuids.Length; i++)
            {
                string levelDataGuid = levelDataGuids[i];

                string path = AssetDatabase.GUIDToAssetPath(levelDataGuid);

                var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);

                string choiceName = string.Join(" \\ ", path.Split('/').Skip(path.Split('/').Length - 2)).Split(".")[0];

                choices.Add(choiceName);

                this.RegisterValueChangedCallback(e => {
                    if(e.newValue == choiceName) levelData.OpenLevelEditor();
                }); 
            }
        }
    }
}

