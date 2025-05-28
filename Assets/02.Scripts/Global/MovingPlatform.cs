using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;   // 시작 지점
    public Transform pointB;   // 끝 지점
    public float speed = 1f;   // 이동 속도 조절 (크면 빠름)

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f); // 0~1 왕복 값
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }
}
