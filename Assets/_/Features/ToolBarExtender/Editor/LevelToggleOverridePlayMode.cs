using System;
using System.Linq;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misc.Editor
{
    [MainToolbarElement("TogglePlayMode", ToolbarAlign.Left, order:1)]
    public class LevelToggleOverridePlayMode : Button
    {
        [Serialize]
        private bool _enabled;
        private Color colorEnable = new Color(0.27450980392156865f, 0.3764705882352941f, 0.48627450980392156f);
        private StyleColor colorDefault;

        public void InitializeElement()
        {
            clicked += OnClick;
            tooltip = "Override Play Mode";
            AddIcon();
            colorDefault = style.backgroundColor;
        }

        private void OnClick()
        {
            _enabled = !_enabled;
            style.backgroundColor = _enabled ? colorEnable : colorDefault;
        }

        public void AddIcon()
        {
            string pathIcon = AssetDatabase.FindAssets("scene_icon").FirstOrDefault();

            if(pathIcon != null) 
            {
                string path = AssetDatabase.GUIDToAssetPath(pathIcon);
                var iconImage = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                var icon = new Image();
                icon.image = iconImage;
                icon.style.width = 24;
                icon.style.height = 24;

                Add(icon);
            }
        }
    }
}
