using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageSpawnData
{
    public string FlowID;
    public string StageLvID;
    public string Name;
    public int Wave;
    public string MobID;
    public float SpawnPositionX;
    public float SpawnPositionY;
    public bool IsPatrol;
    public float PatrolPositionMinX;
    public float PatrolPositionMinY;
    public float PatrolPositionMaxX;
    public float PatrolPositionMaxY;
    public float SpawnDelay;
}

[System.Serializable]
public class StageSpawnDataList
{
    public List<StageSpawnData> datas;
}