using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T, TData> where T : Component, IPoolable<TData>
{
    private readonly T prefab;
    private readonly Queue<T> poolQueue = new();
    private readonly Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public T Get(TData data)
    {
        T obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(true);
        obj.OnSpawn(data);
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    public int ActiveCount()
    {
        return poolQueue.Count;
    }
}
