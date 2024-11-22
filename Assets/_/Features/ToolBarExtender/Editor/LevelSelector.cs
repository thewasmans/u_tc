using System.Collections.Generic;
using System.Linq;
using Level.Data;
using Misc.Runtime;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolBarExtender.Editor
{
    [MainToolbarElement(id: "LevelSelector", ToolbarAlign.Left, order:1)]
    public class LevelSelector : DropdownField
    {
        public void InitializeElement()
        {
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
                    if(e.newValue != choiceName) return;
                    levelData.OpenLevelEditor();
                    style.unityFontStyleAndWeight = FontStyle.Bold;
                    ElementAt(0).style.backgroundColor = new Color(0.27450980392156865f, 0.3764705882352941f, 0.48627450980392156f);
                }); 
            }

            style.unityFontStyleAndWeight = FontStyle.Italic;
            if(levelDataGuids.Length == 0)
            {
                value = "No LevelData in Project";
                tooltip = "Create a New Level before to select a level to open";
            }
            else
            {
                value = "No Level Selected";
                tooltip = "Select a level to open it";
            }
        }
    }
}

