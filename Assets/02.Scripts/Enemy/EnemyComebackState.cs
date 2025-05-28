using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComebackState : IState
{
    private Enemy enemy;
    private EnemyPathFollower pathFollower;

    public EnemyComebackState(Enemy enemy)
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
            enemy.IsComeback = true;
        }
        enemy.animator.SetBool("IsMove", true);
    }

    public void Update(Vector2 playerPos)
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.origin);

        if (dist < 2f) // 범위 벗어나면 Idle 복귀
        {
            enemy.StateMachine.ChangeState(new EnemyIdleState(enemy));
            return;
        }

        if (enemy.MoveType == "Fly")
        {
            // 원위치로 날면서 이동
            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                enemy.origin,
                enemy.moveSpeed * Time.deltaTime
            );
        }
    }

    public void Exit()
    {
        enemy.IsWalking = false;
        enemy.IsComeback = false;
        if (enemy.rb != null)
        {
            enemy.rb.velocity = Vector2.zero;
        }
        enemy.animator.SetBool("IsMove", false);
    }
}