using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Level.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        #region Publics

#if UNITY_EDITOR
        public void OpenLevelEditor()
        {
            foreach (var scenePath in _scenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
#endif

        public void OpenLevel()
        {
            foreach (var scenePath in _scenePath)
            {
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if (scene != null && !scene.isLoaded) SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            }
        }

#if UNITY_EDITOR
        public void CloseLevelEditor()
        {
            foreach (var scenePath in _scenePath)
            {
                Scene scene = SceneManager.GetSceneByName(scenePath);
                if (scene.isLoaded) EditorSceneManager.CloseScene(scene, true);
            }
        }
#endif

        public void CloseLevel()
        {
            foreach (var scenePath in _scenePath)
            {
                Scene scene = SceneManager.GetSceneByName(scenePath);
                if (scene.isLoaded) SceneManager.UnloadSceneAsync(scene);
            }
        }
        #endregion

        #region Private and Protected
        public List<string> _scenePath;

        #endregion
    }
}
