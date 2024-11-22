using Level.Editor;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

namespace ToolBarExtender.Editor
{
    [MainToolbarElement(id: "CreateLevel", ToolbarAlign.Left, order: 2)]
    public class LevelButtonCreator : Button
    {
        public void InitializeElement()
        {
            text = "New Level";
            tooltip = "Create a new Level folder with LevelData asset";
            clicked += () => LevelFolderCreator.ShowWindow();
        }
    }
}