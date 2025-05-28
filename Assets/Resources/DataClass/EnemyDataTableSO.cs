using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataTableSO", menuName = "Game Data/Enemy Data Table")]
public class EnemyDataTableSO : ScriptableObject
{
    public List<EnemyData> datas = new List<EnemyData>();

    public int Length()
    {
        return datas.Count;
    }
}
