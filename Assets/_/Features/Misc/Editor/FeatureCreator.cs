using System.IO;
using Misc.Runtime;
using UnityEditor;
using UnityEngine;

namespace Misc.Editor
{
    public class FeatureCreator : EditorWindow
    {
        #region Public
        [MenuItem("Assets/Create/New Feature %#f", priority = 11)]
        public static void ShowWindow()
        {
            FeatureCreator window = GetWindow<FeatureCreator>("Create new Feature");
            window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 180);
            window.Show();
        }
        
        public static void CreateFolderStructure(string featureName, bool includeRuntime, bool includeEditor, bool includeData)
        {
            if (!Directory.Exists(GlobalPaths.FEATURE_FOLDER))
            {
                Directory.CreateDirectory(GlobalPaths.FEATURE_FOLDER);
            }

            string featurePath = Path.Combine(GlobalPaths.FEATURE_FOLDER, featureName);

            if (includeRuntime)
            {
                string p = Path.Combine(featurePath, "Runtime");
                Directory.CreateDirectory(p);
                CreateAssemblyDefinition(p, $"{featureName}.Runtime");
            }

            if (includeData)
            {
                string p = Path.Combine(featurePath, "Data");
                Directory.CreateDirectory(p);
                CreateAssemblyDefinition(p, $"{featureName}.Data");
            }

            if (includeEditor)
            {
                string p = Path.Combine(featurePath, "Editor");
                Directory.CreateDirectory(p);
                CreateEditorAssemblyDefinition(p, $"{featureName}.Editor");
            }

            AssetDatabase.Refresh();
        }

        public bool FeatureExist(string featureName) => Directory.Exists(Path.Combine(GlobalPaths.FEATURE_FOLDER, featureName));
        #endregion

        private void OnGUI()
        {
            GUILayout.Label("Feature Folder", EditorStyles.boldLabel);

            GUI.SetNextControlName("FeatureName");
            _featureName = EditorGUILayout.TextField("Feature Name", _featureName);

            if (!focusApplied)
            {
                EditorGUI.FocusTextInControl("FeatureName");
                focusApplied = false;
            }

            _includeRuntime = EditorGUILayout.Toggle("Include Runtime", _includeRuntime);
            _includeEditor = EditorGUILayout.Toggle("Include Editor", _includeEditor);
            _includeData = EditorGUILayout.Toggle("Include Data", _includeData);

            GUILayout.Space(10);

            bool enterPressed = Event.current is
            {
                type: EventType.KeyDown,
                keyCode: KeyCode.KeypadEnter or KeyCode.Return
            };

            GUI.enabled = !FeatureExist(_featureName);

            if (GUILayout.Button("Generate") || enterPressed)
            {
                if (string.IsNullOrWhiteSpace(_featureName))
                {
                    EditorUtility.DisplayDialog("Error", "Feature Name is not valid!", "Ok!");
                }
                else
                {
                    CreateFolderStructure(_featureName, _includeRuntime, _includeEditor, _includeData);
                    generateStarted = true;
                }
            }
            GUI.enabled = true;

            if (FeatureExist(_featureName))
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("Feature already exist");
                GUI.contentColor = Color.white;
            }
            else
            {
                GUI.contentColor = Color.green;
                GUILayout.Label("Feature can be create");
                GUI.contentColor = Color.white;
            }

            if(generateStarted)
            {
                GUIContent notification = new("Assemblies Created");
                SceneView.lastActiveSceneView.ShowNotification(notification);
                Close();
            }
        }

        #region Utils
        private static void CreateAssemblyDefinition(string path, string name)
        {
            var asmdef = new AssemblyDefinition()
            {
                name = name,
                references = new string[] { },
                includes = new string[] { },
                rootNamespce = name,
            };

            File.WriteAllText(Path.Combine(path, $"{name}.asmdef"), JsonUtility.ToJson(asmdef, true));
        }

        private static void CreateEditorAssemblyDefinition(string path, string name)
        {
            var asmdef = new AssemblyDefinition()
            {
                name = name,
                references = new string[] { },
                includes = new string[] { "Editor" },
                rootNamespce = name,
            };

            File.WriteAllText(Path.Combine(path, $"{name}.asmdef"), JsonUtility.ToJson(asmdef, true));
        }
        #endregion

        #region Private
        private string _featureName = "";
        private bool focusApplied = false;
        private bool _includeRuntime = true;
        private bool _includeEditor = true;
        private bool _includeData = true;
        private bool generateStarted = false;

        [System.Serializable]
        private class AssemblyDefinition
        {
            public string name;
            public string[] references;
            public string[] includes;
            public string rootNamespce;
        }
        #endregion
    }
}