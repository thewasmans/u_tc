using UnityEditor;
using UnityEngine;

namespace ToolBarExtender.Editor
{
    public static class ToastNotification
    {
        public static void Show(string message) =>
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent(message));
    }
}