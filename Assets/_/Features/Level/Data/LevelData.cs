using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.WindowsStandalone;

namespace Level.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        #region Publics
        public void OpenLevel()
        {
            foreach (var indexScene in _sceneIndex)
            {
                string pathScene = EditorBuildSettings.scenes[indexScene].path;
                EditorSceneManager.OpenScene(pathScene, OpenSceneMode.Additive);
            }
        }

        public void LoadLevel()
        {
            foreach (var indexScene in _sceneIndex) 
            {
                string pathScene = EditorBuildSettings.scenes[indexScene].path;
                Scene scene = SceneManager.GetSceneByPath(pathScene);
                if(scene != null && !scene.isLoaded) SceneManager.LoadSceneAsync(indexScene, LoadSceneMode.Additive);
            }
        }

        public void CloseLevel()
        {
            foreach (var indexScene in _sceneIndex)
            {
                Scene scene = SceneManager.GetSceneAt(indexScene);
                if(scene.isLoaded) SceneManager.UnloadSceneAsync(scene);
            }
        }

        public void UnloadLevel()
        {
            foreach (var indexScene in _sceneIndex)
            {
                Scene scene = SceneManager.GetSceneAt(indexScene);
                if(scene.isLoaded) EditorSceneManager.CloseScene(scene, true);
            }
        }
        #endregion

        #region Private and Protected
        public List<int> _sceneIndex;

        #endregion
    }
}
