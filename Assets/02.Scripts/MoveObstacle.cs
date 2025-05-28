using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    public float startOffset; // 시작점에서 이동 거리
    public float moveSpeed;

    private float startPoint;
    private float turningPoint;
    private bool turnSwitch;

    public enum MoveDirection { Horizontal, Vertical };

    public MoveDirection moveDirection;

    void Start()
    {
        // 오브젝트가 왕복 이동할 범위
        if (moveDirection == MoveDirection.Horizontal)
        {
            startPoint = transform.position.x + startOffset;
            turningPoint = transform.position.x - startOffset;
        }
        else
        {
            startPoint = transform.position.y + startOffset;
            turningPoint = transform.position.y - startOffset;
        }
    }

    void Update()
    {
        MoveObject();
    }

    void MoveObject()
    {
        // X축 이동
        if (moveDirection == MoveDirection.Horizontal)
        {
            float posX = transform.position.x;

            if (posX >= startPoint)
                turnSwitch = false;
            else if (posX <= turningPoint)
                turnSwitch = true;

            transform.position += (turnSwitch ? Vector3.right : Vector3.left) * (moveSpeed * Time.deltaTime);
        }
        // Y축 이동
        else
        {
            float posY = transform.position.y;

            if (posY >= startPoint)
                turnSwitch = false;
            else if (posY <= turningPoint)
                turnSwitch = true;
            
            transform.position += (turnSwitch ? Vector3.up : Vector3.down) * (moveSpeed * Time.deltaTime);
        }
    }
}