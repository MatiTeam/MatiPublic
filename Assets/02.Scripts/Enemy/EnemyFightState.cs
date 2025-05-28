using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFightState : IState
{
    private Enemy enemy;
    private EnemyPathFollower pathFollower;

    public EnemyFightState(Enemy enemy)
    {
        this.enemy = enemy;
        this.pathFollower = enemy.GetComponent<EnemyPathFollower>();
    }

    public void Enter()
    {
        if (pathFollower != null && enemy.MoveType == "Ground")
        {
            pathFollower.RequestPath();
            enemy.IsWalking = true;
        }
        enemy.animator.SetBool("IsMove", true);
    }

    public void Update(Vector2 playerPos)
    {
        float dist = Vector2.Distance(enemy.transform.position, playerPos);

        if (dist > enemy.detectRange + 1f) // 범위 벗어나면 Idle 복귀
        {
            enemy.StateMachine.ChangeState(new EnemyIdleState(enemy));
            return;
        }

        if (dist <= enemy.attackRange)
        {
            enemy.StateMachine.ChangeState(new EnemyAttackState(enemy));
            return;
        }

        if (enemy.MoveType == "Fly")
        {
            // 플레이어에게 날면서 이동
            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                playerPos,
                enemy.moveSpeed * Time.deltaTime
            );
        }
    }

    public void Exit()
    {
        enemy.IsWalking = false;
        if (enemy.rb != null)
        {
            enemy.rb.velocity = Vector2.zero;
        }
        enemy.animator.SetBool("IsMove", false);
    }
}