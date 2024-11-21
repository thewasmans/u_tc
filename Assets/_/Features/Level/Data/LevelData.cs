using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEditor;
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
            for (int i = 0; i < _sceneReferences.Count; i++)
            {
                var sceneReference = _sceneReferences[i];
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneReference.AssetGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
#endif

        public void OpenLevel()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < _sceneReferences.Count; i++)
                {
                    var scenePath = _sceneReferences[i];
                    scenePath.LoadSceneAsync(LoadSceneMode.Additive);
                }
            }
        }

#if UNITY_EDITOR
        public void CloseLevelEditor()
        {
            for (int i = 0; i < _sceneReferences.Count; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(_sceneReferences[i].AssetGUID);
                Scene scene = SceneManager.GetSceneByPath(path);
                EditorSceneManager.CloseScene(scene, true);
            }
        }
#endif

        public void CloseLevel()
        {
            for (int i = 0; i < _sceneReferences.Count; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(_sceneReferences[i].AssetGUID);
                Scene scene = SceneManager.GetSceneByPath(path);
                EditorSceneManager.CloseScene(scene, true);
            }
        }
        #endregion

        #region Private and Protected
        public List<AssetReference> _sceneReferences;
        #endregion
    }
}