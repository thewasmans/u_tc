using System;
using System.Linq;
using Level.Editor;
using Misc.Editor;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

public static class VaulActionsConst
{
    public static string ADDRESSABLE = "Addressable";
    public static string FEATURE = "Features";
    public static string LEVELS = "Levels";
    public static readonly string[] DROP_OPTIONS = new string[] { ADDRESSABLE, FEATURE};
    public static Action<int> onChanged;
}

public struct VaultAction
{
    public string Property;
    public string Text;
    public Action Action;
}

public static class VaultActions
{
    public static VaultAction[] Actions =
    {
        new VaultAction()
        {
            Property = VaulActionsConst.FEATURE,
            Text = "New Feature",
            Action = FeatureCreator.ShowWindow
        },
        new VaultAction()
        {
            Property = VaulActionsConst.LEVELS,
            Text = "New Level",
            Action = LevelFolderCreator.ShowWindow
        },
        new VaultAction()
        {
            Property = VaulActionsConst.ADDRESSABLE,
            Text = "Build Addressable",
            Action = () => Debug.LogError("New Addresaable Window not implemented")
        },
    };
}

namespace ToolBarExtender.Editor
{
    [MainToolbarElement(id: "LevelSelector", ToolbarAlign.Right, order: 0)]
    public class VaultActionsDropdown : DropdownField
    {
        public void InitializeElement()
        {
            choices = VaultActions.Actions.Select(a => a.Property).ToList();
            this.RegisterValueChangedCallback(e => VaulActionsConst.onChanged.Invoke(index));
            value = choices.FirstOrDefault();
        }
    }

    [MainToolbarElement(id: "LevelSelector", ToolbarAlign.Right, order: 1)]
    public class VaultActionButton : Button
    {
        protected VaultAction _action;

        protected void InitializeElement()
        {
            VaulActionsConst.onChanged += OnChange;
            text = VaultActions.Actions.Select(a => a.Property).ToList().FirstOrDefault();
        }

        public void OnChange(int indexDropdown)
        {
            clicked -= _action.Action;
            _action = VaultActions.Actions[indexDropdown];
            text = _action.Text;
            clicked += _action.Action;
        }
    }
}