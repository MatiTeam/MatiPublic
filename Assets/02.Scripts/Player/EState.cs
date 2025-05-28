using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EState : IState
{
    NewPlayerContorller player;

    public EState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.isAnimationOver = false;
        player.isInvincible = true;
        player.EstateOver = false;

        player.skillInfoUI.StartCoroutine(player.skillInfoUI.SkillCoolTime(2, player.eSkillData.CoolTime, player.eSkillData.SkillID));

        Debug.Log("E스킬 들어왔다");

        player.isUsingSkill = true;
        player.wSkillStack = Mathf.Min(player.wSkillStack + 1, 3);
        if (player.IsOnGround)
        {
            player.animator.SetTrigger("ESkill_Ground");
            if (player.rb != null)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0); //현재 y축 속도를 초기화해 점프를 부드럽게 만듦
                float heightDifference = player.SelectedEnemy.transform.position.y - player.transform.position.y; //적과 플레이어의 y 높이차 계산
                float jumpForce = Mathf.Clamp(heightDifference + 10f, 10f, 30f); // 최소 5, 최대 20 제한
                player.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            }
        }
        else
        {
            player.animator.SetTrigger("ESkill_Jump");
            player.OnDash();
        }
        player.isUsingSkill = false;
    }

    public void Exit()
    {
        player.stackController.UsingSkill();
        player.SkillInvincible();
    }

    void IState.Update(Vector2 mousePos)
    {
        if (player.isAnimationOver == true)
        {
            player.SelectedEnemy.GetComponent<Enemy>()?.TakeDamage((int)player.eSkillData.Damage); // 적에게 데미지 입히기
            player.SelectedEnemy.GetComponent<Boss>()?.TakeDamage(player.eSkillData.Damage);
            player.targetX = player.transform.position.x;
            player.stateMachine.ChangeState(player.IdleState);
        }
    }
}
