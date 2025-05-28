using UnityEngine;

public class Factory<T, Tdata> where T : Component, IPoolable<Tdata>
{
    private readonly ObjectPool<T, Tdata> objectPool;

    public Factory(T prefab, int initialSize, Transform parent = null)
    {
        objectPool = new ObjectPool<T, Tdata>(prefab, initialSize, parent);
    }

    public T Create(Tdata data)
    {
        return objectPool.Get(data);
    }

    public void Release(T obj)
    {
        objectPool.ReturnToPool(obj);
    }

    public int ActiveCount()
    {
        return objectPool.ActiveCount();
    }
}