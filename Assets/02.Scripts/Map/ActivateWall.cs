using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnPlayerY : MonoBehaviour
{
    [SerializeField] private Transform player;   // 플레이어 트랜스폼
    [SerializeField] private GameObject targetObject; // 활성화할 오브젝트
    [SerializeField] private float triggerY = 5f; // 기준이 되는 Y 값
    [SerializeField] private bool activateAbove = true; // true면 Y 이상일 때, false면 Y 이하일 때 활성화

    private bool isActivated = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }


    void Update()
    {
        if (player == null || targetObject == null)
            return;

        if (activateAbove)
        {
            if (!isActivated && player.position.y > triggerY)
            {
                targetObject.SetActive(true);
                isActivated = true;
            }
        }
        else
        {
            if (!isActivated && player.position.y < triggerY)
            {
                targetObject.SetActive(true);
                isActivated = true;
            }
        }
    }
}
