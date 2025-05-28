using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextFlow : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";  // 플레이어 태그
    [SerializeField] private bool isToStory = false;
    [SerializeField] private bool isBossDoor = false;

    private bool enter = false;

    BattleFlowManager flowManager;

    void Start()
    {
        flowManager = GetComponentInParent<BattleFlowManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"충돌 감지됨: {other.name}");

        if (other.CompareTag(playerTag))
        {
            if (enter == false)
            {
                enter = true;
                if (isBossDoor)
                {
                    if (isToStory == false)
                    {
                        if (DataManager.Instance.flowManager != null)
                        {
                            DataManager.Instance.flowManager.MoveToNextFlow();
                        }
                        flowManager.StartNextStageCoroutine();
                    }
                    else
                    {
                        DataManager.Instance.flowManager.MoveToStoryScene();
                    }
                }
                else
                {
                    if (isToStory == false)
                    {
                        Debug.Log("플레이어가 문에 닿음!");
                        flowManager.StartNextStageCoroutine();
                    }
                    else
                    {
                        DataManager.Instance.flowManager.MoveToStoryScene();
                    }
                }
            }
        }
    }
}
