using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BaseAttackState : IState
{
    NewPlayerContorller player;
    public BaseAttackState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.isAnimationOver = false;
        player.animator.SetTrigger("BaseAttack");
    }

    public void Exit()
    {

    }


    void IState.Update(Vector2 mousePos)
    {
        if (player.isAnimationOver == true)
        {
            Vector2 dir;
            if (mousePos.x - player.transform.position.x >= 0f)
                dir = Vector2.right;
            else
                dir = Vector2.left;
            Vector3 pivot = new Vector3(0, -0.5f, 0);
            RaycastHit2D hit;
            for (int i = -1; i < 2; i++)
            {
                hit = Physics2D.Raycast(player.transform.position - pivot * i, dir,
                    player.basicAttackData.EffectRange + 0.5f, player.targetLayer);
                if (hit.collider != null)
                {
                    Debug.Log("기본 공격 맞음 : " + hit.collider.name);
                    Boss boss = hit.collider.GetComponent<Boss>();
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage((int)player.basicAttackData.Damage);
                        Vector2 knockbackDir = (enemy.transform.position - player.transform.position).normalized;
                        enemy.StateMachine.ChangeState(new EnemyKnockbackState(enemy, knockbackDir));
                    }
                    else if(boss != null)
                    {
                        boss.TakeDamage(player.basicAttackData.Damage);
                    }
                    DataManager.Instance.skillManager.controller.ReduceCoolTimeOnBaseAttack(1f);
                    break;
                }
            }

            player.lastBasicAttack = player.basicAttackData.CoolTime;

            for (int i = -1; i < 2; i++)
            {
                Debug.DrawRay(player.transform.position - pivot * i,
                dir.normalized * (player.basicAttackData.EffectRange + 0.5f), Color.yellow, 1f);
            }

            player.isUsingSkill = false;
            player.stateMachine.ChangeState(player.IdleState);
        }
    }
}
