using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Stage", menuName = "GameData/CreateStageData")]

public class StageSO : ScriptableObject
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
