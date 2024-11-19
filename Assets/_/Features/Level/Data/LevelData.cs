using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Linq;

namespace Level.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        #region Publics
        public void OpenLevel()
        {
            foreach (var scenePath in _scenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }

        public void LoadLevel()
        {
            foreach (var scenePath in _scenePath) 
            {
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if(scene != null && !scene.isLoaded) SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            }
        }

        public void CloseLevel()
        {
            foreach (var scenePath in _scenePath)
            {
                Scene scene = SceneManager.GetSceneByName(scenePath);
                if(scene.isLoaded) SceneManager.UnloadSceneAsync(scene);
            }
        }

        public void UnloadLevel()
        {
            foreach (var scenePath in _scenePath)
            {
                Scene scene = SceneManager.GetSceneByName(scenePath);
                if(scene.isLoaded) EditorSceneManager.CloseScene(scene, true);
            }
        }
        #endregion

        #region Private and Protected
        public List<string> _scenePath;

        #endregion
    }
}
