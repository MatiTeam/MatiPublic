using UnityEngine;

public class InfinityBaseItem : MonoBehaviour
{
    private InfinityScrollView infinityScrollView;
    private int index = int.MinValue;
    public int Index
    {
        private set
        {
            index = value;
        }
        get
        {
            return index;
        }
    }
    public InfinityScrollView GetInfinityScrollView()
    {
        return infinityScrollView;
    }
    public virtual void Reload(InfinityScrollView infinity, int _index)
    {
        infinityScrollView = infinity;
        Index = _index;
        //todo
    }

    public virtual void SelfReload()
    {
        if (Index != int.MinValue)
        {
            //todo
        }
    }
}
