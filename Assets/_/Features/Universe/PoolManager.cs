using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PoolManager
{

    public void GetOrcreateInstanceOf(AssetReference original, Action<GameObject> onSpawned, int poolQuantity) =>
        GetPoolAsync(original, pool => pool.WaitForItemCreated(() => pool.PopAsync(onSpawned)), poolQuantity);

    public void GetPoolAsync(AssetReference original, Action<Pool> callback, int pool)
    {
        if (HasPool(original)) _pools[original].ExtendTo(pool, callback);
        else CreatePool(original, callback, pool);
    }

    private static Pool CreatePool(AssetReference original, Action<Pool> callback, int pool)
    {
        var poolCreated = new Pool(original, pool, callback);
        poolCreated.OnInstanceCreated += (GameObject go, Pool pool) => _bindings.Add(go, pool);
        _pools.Add(original, poolCreated);
        return poolCreated;
    }

    public static void Release(GameObject go)
    {
        _bindings[go].Push(go);
        _bindings.Remove(go);
    }

    public bool HasPool(AssetReference original) => _pools.ContainsKey(original);

    private static Dictionary<AssetReference, Pool> _pools = new();
    private static Dictionary<GameObject, Pool> _bindings = new();
}

public class Pool
{
    public int m_capacity;
    public Action<GameObject, Pool> OnInstanceCreated;

    public Action<GameObject, Pool> OnGetItem;

    public Pool(AssetReference assetRef, int capacity, Action<Pool> callback = null)
    {
        _assetReference = assetRef;
        _stack = new(capacity);
        ExtendTo(capacity, callback);
    }

    public void ExtendTo(int capactiy, Action<Pool> callback)
    {
        if (capactiy <= m_capacity)
        {
            callback.Invoke(this);
            return;
        }

        int add = capactiy - m_capacity;
        m_capacity = capactiy;
        Prepare(add, callback);
    }

    private void Prepare(int amount = 1, Action<Pool> callback = null)
    {
        int remaining = amount;

        for (int i = 0; i < amount; i++)
        {
            var handle = _assetReference.InstantiateAsync();
            handle.Completed += OnAssetPrepared;
            _remainingTaskList.Add(handle.Task);
        }

        void OnAssetPrepared(AsyncOperationHandle<GameObject> handle)
        {
            handle.Result.SetActive(false);
            _stack.Push(handle.Result);
            OnInstanceCreated?.Invoke(handle.Result, this);
            if (--remaining <= 0) callback?.Invoke(this);
        }
    }

    public void Push(GameObject go)
    {
        go.SetActive(false);
        _stack.Push(go);
    }

    public void PopAsync(Action<GameObject> callback)
    {
        if (_stack.Count == 0)
        {
            ExtendTo(m_capacity + 1, pool => GetItem(callback));
#if UNITY_EDITOR
            Debug.LogWarning($"[POOL MANAGER] The pooling size created for {AssetDatabase.GUIDToAssetPath(_assetReference.AssetGUID)}");
#endif
        }
        else
        {
            GetItem(callback);
        }
    }

    private void GetItem(Action<GameObject> callback)
    {
        GameObject go = _stack.Pop();
        go.SetActive(true);
        OnGetItem?.Invoke(go, this);
        callback?.Invoke(go);
    }

    private bool TryPop(out GameObject go) => _stack.TryPop(out go);

    public async void Wait(Action onCompleted)
    {
        await Task.WhenAny(_remainingTaskList);
        onCompleted?.Invoke();
        await Task.WhenAll(_remainingTaskList);
        _remainingTaskList.Clear();
    }

    public void WaitForItemCreated(Action value)
    {
        throw new NotImplementedException();
    }

    private AssetReference _assetReference;
    public Stack<GameObject> _stack;
    public List<Task<GameObject>> _remainingTaskList = new();
}