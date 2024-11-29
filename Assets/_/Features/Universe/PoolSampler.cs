using UnityEngine;
using UnityEngine.AddressableAssets;
using Universe;

public class PoolSampler : UBehavior
{
    public AssetReference m_caSpawn;

    public void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            Spawn(m_caSpawn, Random.insideUnitSphere * .15f, Quaternion.identity, null, OnSpawned, 1);
        }
    }

    private void OnSpawned(Object o)
    {
        GameObject go = o as GameObject;
        go.SetActive(true);
    }
}