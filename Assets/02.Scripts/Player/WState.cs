using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WState : IState
{
    NewPlayerContorller player;
    public WState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.isAnimationOver = false;
        //애니메이션 재생
        switch (player.stackController.currentStackIndex)
        {
            case 1:
                break;
            case 2:
                player.isInvincible = true;
                player.animator.SetTrigger("WStack2");
                float chargeSpeed = 10;
                player.rb.AddForce((player.SelectedEnemy.transform.position - player.transform.position).normalized *
                    chargeSpeed, ForceMode2D.Impulse);
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    public void Exit()
    {

    }


    void IState.Update(Vector2 mousePos)
    {
        switch (player.stackController.currentStackIndex)
        {
            case 1:
                W_Stack1();
                break;
            case 2:
                W_Stack2();
                break;
            case 3:
                W_Stack3(mousePos);
                break;
            default:
                Debug.Log("W스택이 없습니다.");
                player.stateMachine.ChangeState(player.IdleState);
                break;
        }
    }

    void W_Stack1()
    {
        player.playerStat.ActivateShield();
        player.stackController.UsingStack();
        player.isUsingSkill = false;
        player.stateMachine.ChangeState(player.IdleState);
    }

    void W_Stack2()
    {
        Boss boss = player.SelectedEnemy.GetComponent<Boss>();
        Enemy enemy = player.SelectedEnemy.GetComponent<Enemy>();
        if (enemy != null)
        {
            if ((player.transform.position - enemy.transform.position).magnitude < 0.6f)
            {
                Debug.Log("W2 플레이어 적과 접촉함");
                //enemy.transform.SetParent(player.transform);
                // enemy.StateMachine.ChangeState(enemy.enemyInabilityState);
                enemy.StateMachine.ChangeState(new EnemyInabilityState(enemy));
            }

            if (player.isAnimationOver)
            {

                enemy.TakeDamage((int)player.wStack2Data.Damage);

                if (enemy.enemyStunState is Enemy_StunState stunstate)
                {
                    stunstate.duration = player.wStack2Data.CCTime;
                }
                enemy.StateMachine.ChangeState(enemy.enemyStunState);
                player.stackController.UsingStack();
                //player.SelectedEnemy.transform.SetParent(null);
                Debug.Log("W2스택 끝남");
                player.SkillInvincible();
                player.isUsingSkill = false;
                player.targetX = player.transform.position.x;
                player.stateMachine.ChangeState(player.IdleState);
            }
        }
        else if(boss != null)
        {
            if (player.isAnimationOver)
            {
                boss.TakeDamage(player.wStack2Data.Damage);
                player.stackController.UsingStack();
                //player.SelectedEnemy.transform.SetParent(null);
                Debug.Log("W2스택 끝남");
                player.SkillInvincible();
                player.isUsingSkill = false;
                player.targetX = player.transform.position.x;
                player.stateMachine.ChangeState(player.IdleState);
            }
        }
        else
        {
            player.isUsingSkill = false;
            player.targetX = player.transform.position.x;
            player.stateMachine.ChangeState(player.IdleState);
        }

    }
    void W_Stack3(Vector2 mousePos)
    {
        player.SetW3SkillEffect(mousePos);
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, player.wStack3Data.EffectArea, player.targetLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("W3스택 맞음: " + hit.name);

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (enemy.enemyAirborneState is Enemy_AirborneState airboneState)
                {
                    airboneState.duration = player.wStack3Data.CCTime;
                }
                enemy.StateMachine.ChangeState(enemy.enemyAirborneState);
            }
        }

        foreach (Collider2D hit in hits)
        {
            Debug.Log("W3스택 맞음: " + hit.name);

            Boss boss = hit.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(player.wStack3Data.Damage);
            }
        }




        DebugDrawCircle(player.transform.position, player.wStack3Data.EffectRange, Color.blue, 32, 2f);
        DebugDrawCircle(mousePos, player.wStack3Data.EffectArea, Color.cyan, 32, 2f);
        player.isUsingSkill = false;
        player.stackController.UsingStack();
        player.stateMachine.ChangeState(player.IdleState);
    }

    void DebugDrawCircle(Vector3 center, float radius, Color color, int segments = 32, float duration = 0f)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), Mathf.Sin(0f)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
        }
    }
}
