using System.Linq;
using Misc.Editor;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

namespace ToolBarExtender.Editor
{
    [MainToolbarElement(id: "LevelSelector", ToolbarAlign.Right, order:0)]
    public class VaultActionsDropdown : DropdownField
    {
        public void InitializeElement()
        {
            choices = new string[]{"Features"}.ToList();
            value = choices.FirstOrDefault();
        }
    }

    [MainToolbarElement(id: "StyledButton", ToolbarAlign.Right, order:1)]
    public class VaultActionButton : Button
    {
        public VaultActionButton()
        {
            text = "New Feature";
            clicked += () => FeatureCreator.ShowWindow();
        }
    }
}