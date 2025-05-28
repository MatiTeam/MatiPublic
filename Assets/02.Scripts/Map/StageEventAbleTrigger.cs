using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//매니저 싱글톤으로 바꾸면 변환 필요
public class StageEventAbleTrigger : MonoBehaviour
{
    [SerializeField] private string targetStageID;
    [SerializeField] private List<GameObject> targetObjects;
    private StageManager stageManager;

    private void OnEnable()
    {
        stageManager = FindObjectOfType<StageManager>();
        if (stageManager != null)
        {
            stageManager.OnStageCleared += HandleStageCleared;
        }
        else
        {
            Debug.LogWarning("StageManager를 찾을 수 없습니다.");
        }
    }

    private void OnDisable()
    {
        if (stageManager != null)
        {
            stageManager.OnStageCleared -= HandleStageCleared;
        }
    }

    private void HandleStageCleared(string clearedStageID)
    {
        if (clearedStageID == targetStageID)
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true); // 여기만 Destroy 대신 SetActive로 변경
                }
            }
        }
    }

}
