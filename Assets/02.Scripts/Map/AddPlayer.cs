using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AddPlayer : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        if (virtualCamera != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                virtualCamera.Follow = player.transform;
                virtualCamera.LookAt = player.transform;
            }
            else
            {
                Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
            }
        }
    }
}
