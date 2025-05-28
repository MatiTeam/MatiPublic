using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDashAttackState : IState
{
    private Enemy enemy;
    private Vector2 dashDirection;
    private float dashSpeed = 15f;

    private Vector2 startPosition;
    private Vector2 dashTarget;
    private bool returning = false;

    public EnemyDashAttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        startPosition = enemy.transform.position;
        Vector2 playerPos = enemy.player.position;

        dashDirection = (playerPos - startPosition).normalized;

        dashTarget = playerPos;

        returning = false;
        enemy.animator.SetTrigger("IsAttack");
    }

    public void Update(Vector2 playerPos)
    {
        if (!returning)
        {
            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                dashTarget,
                dashSpeed * Time.deltaTime
            );

            if (Vector2.Distance(enemy.transform.position, dashTarget) < 0.05f)
            {
                // Debug.Log("🔙 복귀 시작");
                returning = true;
            }
        }
        else
        {
            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                startPosition,
                dashSpeed * Time.deltaTime
            );

            if (Vector2.Distance(enemy.transform.position, startPosition) < 0.05f)
            {
                // Debug.Log("💥 복귀 완료, 공격 상태 복귀");
                enemy.StateMachine.ChangeState(new EnemyAttackState(enemy));
            }
        }
    }

    public void Exit() { }
}
