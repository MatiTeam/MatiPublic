using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//싱글톤으로 바꾸는 작업이 필요
public class StageManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private StageDataLoader dataLoader;

    private string currentFlow;
    private string currentStageID = "SL001";
    private int aliveEnemyCount = 0;

    //옵저버 패턴용 Action
    // public static event Action<string> OnStageCleared;
    public event Action<string> OnStageCleared;

    void Start()
    {
        DataManager.Instance.flowManager.LoadFlowData();
        // currentFlow = DataManager.Instance.flowManager.GetCurrentFlowID();
        // StartStage(currentFlow, currentStageID);
    }

    public void StartNextFlow()
    {
        currentFlow = DataManager.Instance.flowManager.GetCurrentFlowID();
        currentStageID = "SL001";
        StartStage(currentFlow, currentStageID);
    }

    public void StartNextFlowForTest(string flowID)
    {
        currentFlow = flowID;
        currentStageID = "SL001";
        StartStage(currentFlow, currentStageID);
    }

    public void StartStage(string FlowID, string stageId)
    {
        Debug.Log("스테이지 시작 " + FlowID + ", " + stageId);
        currentStageID = stageId;

        var stageDataList = dataLoader.GetFlowData(FlowID, stageId);
        aliveEnemyCount = stageDataList.Count;
        Debug.Log("생성된 적 수" + aliveEnemyCount);

        foreach (var data in stageDataList)
        {
            StartCoroutine(SpawnEnemyWithDelay(data));
        }
    }

    private IEnumerator SpawnEnemyWithDelay(StageSpawnData data)
    {
        yield return new WaitForSeconds(data.SpawnDelay);

        // Debug.Log(data.PatrolPositionMinX + " " + data.PatrolPositionMinY);

        spawner.SpawnEnemyById(
            data.MobID,
            new Vector2(data.SpawnPositionX, data.SpawnPositionY),
            data.IsPatrol,
            new Vector2(data.PatrolPositionMinX, data.PatrolPositionMinY),
            new Vector2(data.PatrolPositionMaxX, data.PatrolPositionMaxY),
            OnEnemyKilled
        );
    }

    private void OnEnemyKilled()
    {
        aliveEnemyCount--;
        Debug.Log($"적 죽음. 남은 적 : {aliveEnemyCount}");
        if (aliveEnemyCount <= 0)
        {
            Debug.Log("스테이지 완료!");

            OnStageCleared?.Invoke(currentStageID); // 옵저버용

            string nextStageLvID = dataLoader.GetNextStageID(currentFlow, currentStageID);
            if (!string.IsNullOrEmpty(nextStageLvID))
            {
                aliveEnemyCount = 0;
                StartStage(currentFlow, nextStageLvID);
            }
            else
            {
                DataManager.Instance.flowManager.MoveToNextFlow();
                currentFlow = DataManager.Instance.flowManager.GetCurrentFlowID();
                if (currentFlow == null)
                {
                    Debug.Log("모든 플로우 완료! 게임 클리어");
                }
                else
                {
                    Debug.Log("모든 스테이지 완료! 다음 플로우 대기");
                    currentStageID = "SL001";
                }
            }
        }
    }
}
