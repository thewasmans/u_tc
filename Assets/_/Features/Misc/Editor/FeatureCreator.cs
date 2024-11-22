using System.IO;
using System.Linq;
using Misc.Runtime;
using ToolBarExtender.Editor;
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
            window._featureName = "";
            window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 180);
            window.Show();
        }
        #endregion

        #region Unity API
        private void OnGUI()
        {
            GUILayout.Label("Feature Folder", EditorStyles.boldLabel);

            GUI.SetNextControlName("FeatureName");
            _featureName = EditorGUILayout.TextField("Feature Name", _featureName);

            if (!_focusApplied)
            {
                EditorGUI.FocusTextInControl("FeatureName");
                _focusApplied = false;
            }

            for (int i = 0; i < _featureParts.Length; i++)
            {
                var part = _featureParts[i];
                _featureParts[i].Included = EditorGUILayout.Toggle($"{part.Name}", part.Included);
            }

            GUILayout.Space(10);

            GUI.enabled = !FeatureExist(_featureName);

            bool enterPressed = Event.current is
            {
                type: EventType.KeyDown,
                keyCode: KeyCode.KeypadEnter or KeyCode.Return
            };

            if (GUILayout.Button("Generate") || enterPressed)
            {
                if (string.IsNullOrWhiteSpace(_featureName))
                    EditorUtility.DisplayDialog("Error", "Feature Name is not valid!", "Ok!");
                else
                    _generateStarted = CreateFolderStructure(_featureName);
            }
            GUI.enabled = true;

            string folderName = Path.GetFullPath(Path.Combine(GlobalPaths.LEVEL_FOLDER, _featureName));

            if (FeatureExists(_featureName))
            {
                GUI.contentColor = UnityEngine.Color.red;
                GUILayout.Label($"{folderName} folder already exist");
                GUI.contentColor = UnityEngine.Color.white;
            }
            else
            {
                GUI.contentColor = UnityEngine.Color.green;

                GUILayout.Label($"Will create in {folderName}");
                GUI.contentColor = UnityEngine.Color.white;
            }

            if (_generateStarted)
            {
                ToastNotification.Show($"New Feature Created [{_featureName}]");
                Close();
            }
        }

        public bool FeatureExists(string featureName) => Directory.Exists(Path.Combine(GlobalPaths.FEATURE_FOLDER, featureName));
        #endregion

        #region Utils
        private static void CreateAssemblyDefinition(string path, string name, bool includeEditor)
        {
            var asmdef = new AssemblyDefinition()
            {
                name = name,
                references = new string[] { },
                includePlatforms = includeEditor ? new string[] { "Editor" } : new string[] { },
                rootNamespce = name,
            };

            File.WriteAllText(Path.Combine(path, $"{name}.asmdef"), asmdef.ToString());
        }
        #endregion

        #region Private

        private string _featureName = "";
        private bool _focusApplied = false;
        private bool _generateStarted = false;
        private static FeaturePart[] _featureParts = new FeaturePart[]
        {
            new FeaturePart(){Name = "Data", Editor = false, Included = true},
            new FeaturePart(){Name = "Editor", Editor = true, Included = true},
            new FeaturePart(){Name = "Runtime", Editor = false, Included = true},
        };

        [System.Serializable]
        private struct AssemblyDefinition
        {
            public string name;
            public string[] references;
            public string[] includePlatforms;
            public string rootNamespce;

            public override string ToString() => JsonUtility.ToJson(this, true);
        }

        private struct FeaturePart
        {
            public string Name;
            public bool Editor;
            public bool Included;
        }

        private struct StyleFeatureStatus
        {
            public Color Color;
            public string Label;
        }

        private static bool CreateFolderStructure(string featureName)
        {
            if (!Directory.Exists(GlobalPaths.FEATURE_FOLDER)) Directory.CreateDirectory(GlobalPaths.FEATURE_FOLDER);

            string featurePath = Path.Combine(GlobalPaths.FEATURE_FOLDER, featureName);

            _featureParts.ToList().ForEach(part =>
            {
                if (part.Included)
                {
                    string p = Path.Combine(featurePath, part.Name);
                    Directory.CreateDirectory(p);
                    CreateAssemblyDefinition(p, $"{featureName}.{part.Name}", part.Editor);
                }
            });

            AssetDatabase.Refresh();

            return true;
        }

        private bool FeatureExist(string featureName) => Directory.Exists(Path.Combine(GlobalPaths.FEATURE_FOLDER, featureName));
        #endregion
    }
}