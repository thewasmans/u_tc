using System.IO;
using Level.Data;
using Misc.Runtime;
using UnityEditor;
using UnityEngine;

namespace Level.Editor
{
    public class LevelFolderCreator : EditorWindow
    {
        public static void ShowWindow()
        {
            LevelFolderCreator window = GetWindow<LevelFolderCreator>();
            window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 180);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Folder", EditorStyles.boldLabel);

            GUI.SetNextControlName("Level Name");
            _levelName = EditorGUILayout.TextField("Level Name", _levelName);

            bool enterPressed = Event.current is
            {
                type: EventType.KeyDown,
                keyCode: KeyCode.KeypadEnter or KeyCode.Return
            };

            GUI.enabled = !LevelExists(_levelName);
            
            if (GUILayout.Button("Generate") || enterPressed)
            {
                if (string.IsNullOrWhiteSpace(_levelName))
                {
                    EditorUtility.DisplayDialog("Error", "Feature Name is not valid!", "Ok!");
                }
                else
                {
                    CreateFolderLevel(_levelName);
                    Close();
                }
            }
            GUI.enabled = true;
            
            string folderName = Path.GetFullPath(Path.Combine(GlobalPaths.LEVEL_FOLDER, _levelName));

            if (LevelExists(_levelName))
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
        }

        public static void CreateFolderLevel(string nameLevel)
        {
            if (!Directory.Exists(GlobalPaths.LEVEL_FOLDER))
            {
                Directory.CreateDirectory(GlobalPaths.LEVEL_FOLDER);
            }

            string levelPath = Path.Combine(GlobalPaths.LEVEL_FOLDER, nameLevel);
            Directory.CreateDirectory(levelPath);
            Directory.CreateDirectory(Path.Combine(levelPath, "Scenes"));
            
            LevelData levelData = CreateInstance<LevelData>();

            string path = Path.Combine(levelPath, $"{nameLevel}.asset");
            AssetDatabase.CreateAsset(levelData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = levelData;
            EditorGUIUtility.PingObject(levelData);
        }

        public bool LevelExists(string levelName) => Directory.Exists(Path.Combine(GlobalPaths.LEVEL_FOLDER, levelName));

        private string _levelName = "Level";
    }
}
