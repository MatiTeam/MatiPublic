using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInabilityState : IState
{
    private Enemy enemy;
    private Transform player;
    private Vector2 offset;

    public EnemyInabilityState(Enemy enemy)
    {
        this.enemy = enemy;
        this.player = enemy.player; // Enemy가 public Transform player 가지고 있어야 함
    }

    public void Enter()
    {
        offset = (Vector2)enemy.transform.position - (Vector2)player.position;
        Debug.Log("😵 무력화 상태 진입, 상대 거리 유지 시작");
    }

    public void Update(Vector2 playerPos)
    {
        enemy.transform.position = (Vector2)player.position + offset;
    }

    public void Exit() { }
}
