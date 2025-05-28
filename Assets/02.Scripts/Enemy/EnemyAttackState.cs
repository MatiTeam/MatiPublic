using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IState
{
    private Enemy enemy;
    private float attackCooldown;
    private float lastAttackTime;

    public EnemyAttackState(Enemy enemy)
    {
        this.enemy = enemy;
        attackCooldown = enemy.Cooltime;
    }

    public void Enter()
    {
        lastAttackTime = Time.time;
    }

    public void Update(Vector2 playerPos)
    {
        enemy.enemySprite.flipX = playerPos.x < enemy.transform.position.x;

        float dist = Vector2.Distance(enemy.transform.position, playerPos);

        if (dist > enemy.chaseRange)
        {
            enemy.StateMachine.ChangeState(new EnemyFightState(enemy));
            return;
        }

        // 쿨타임 지난 경우, 공격
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            enemy.lastAttackTime = Time.time;

            //enemy.animator.SetTrigger("IsAttack");
            if (enemy.Skill1 == "S1001")
            {
                Debug.Log("근거리 공격");
                enemy.StateMachine.ChangeState(new EnemyMeleeAttackState(enemy, enemy.Bullet));
            }
            else if (enemy.Skill1 == "S1002")
            {
                Debug.Log("돌진 공격");
                enemy.StateMachine.ChangeState(new EnemyDashAttackState(enemy));
            }
            else if (enemy.Skill1 == "S1003")
            {
                Debug.Log("원거리 공격");
                enemy.StateMachine.ChangeState(new EnemyRangeAttackState(enemy, enemy.Bullet));
            }
            else
            {
                Debug.Log("유효하지 않은 공격 타입");
            }
        }
    }

    public void Exit() { }
}
