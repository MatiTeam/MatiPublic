public interface IPoolable<T>
{
    void OnSpawn(T data);
    void OnDespawn();
}