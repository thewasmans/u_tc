using Level.Data;
using UnityEngine;

namespace Level.Runtime
{
    public class LevelLoader : MonoBehaviour
    {
        #region Unity API
        private void Awake()
        {
            if(_levelData == null)
            {
                Debug.Log("Error level null");
                return;
            }
            
            _levelData?.LoadLevel();

        }
        #endregion

        #region Private and Protected
        [SerializeField]
        private LevelData _levelData;
        #endregion    
    }
}
