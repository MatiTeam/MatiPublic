using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QState : IState
{
    NewPlayerContorller player;


    public QState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("QState Enter(), stackController: " + (player.stackController != null));
        player.isAnimationOver = false;
        player.rb.velocity = Vector2.zero;
        player.targetX = player.transform.position.x;
        player.animator.SetTrigger("Qskill");
    }

    public void Exit()
    {

    }

    void IState.Update(Vector2 mousePos)
    {
        if (player.isAnimationOver)
        {
            player.wSkillStack = Mathf.Min(player.wSkillStack + 1, 3);
            Vector2 dir;
            if (mousePos.x - player.transform.position.x >= 0f)
                dir = Vector2.right;
            else
                dir = Vector2.left;

            Vector2 center = (Vector2)player.transform.position + dir * 2f + Vector2.up;
            Vector2 size = new Vector2(player.qSkillData.EffectRange, player.qSkillData.EffectRange);
            player.SetQSkillEffect(center);

            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0, player.targetLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if(hits[i].tag == "Breakable")
                {
                    Breakable breakable = hits[i].GetComponent<Breakable>();
                    breakable.Break();
                }
                else if(hits[i].tag == "Healer")
                {
                    Healer healer = hits[i].GetComponent<Healer>();
                    healer.Heal(player.playerStat);
                }
            }

            for (int i = 0; i < hits.Length; i++)
            {
                Enemy enemy = hits[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    if (i == 0)
                    {
                        player.stackController.UsingSkill();
                    }
                    enemy.TakeDamage((int)player.qSkillData.Damage);
                    Vector2 knockbackDir = (enemy.transform.position - player.transform.position).normalized;
                    enemy.StateMachine.ChangeState(new EnemyKnockbackState(enemy, knockbackDir));
                }

                Boss boss = hits[i].GetComponent<Boss>();
                if (boss != null)
                {
                    if (i == 0)
                    {
                        player.stackController.UsingSkill();
                    }
                    boss.TakeDamage(player.qSkillData.Damage);
                }
            }

            player.lastTimeQ = player.qSkillData.CoolTime;

            // 디버그용 박스 시각화
            Debug.DrawLine(center + new Vector2(-size.x, size.y) * 0.5f, center + new Vector2(size.x, size.y) * 0.5f, Color.red, 1f);
            Debug.DrawLine(center + new Vector2(size.x, size.y) * 0.5f, center + new Vector2(size.x, -size.y) * 0.5f, Color.red, 1f);
            Debug.DrawLine(center + new Vector2(size.x, -size.y) * 0.5f, center + new Vector2(-size.x, -size.y) * 0.5f, Color.red, 1f);
            Debug.DrawLine(center + new Vector2(-size.x, -size.y) * 0.5f, center + new Vector2(-size.x, size.y) * 0.5f, Color.red, 1f);

            //Debug.DrawRay(player.transform.position, dir.normalized * player.qSkillData.EffectRange, Color.red, 1f);
            player.isUsingSkill = false;
            player.stateMachine.ChangeState(player.IdleState);

            // 추가 내용
            player.skillInfoUI.StartCoroutine(player.skillInfoUI.SkillCoolTime(0, player.qSkillData.CoolTime, player.qSkillData.SkillID));
        }
    }
}
