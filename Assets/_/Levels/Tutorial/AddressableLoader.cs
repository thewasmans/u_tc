using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableLoader : MonoBehaviour
{
    AssetReference AssetReference;

    private async void Start()
    {
        var task = await AssetReference.LoadAssetAsync<GameObject>().Task;
        if(task) Instantiate(task);
    }
}
