using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedState : IState
{
    NewPlayerContorller player;
    int damage;
    Vector2 knockBackDir;
    public DamagedState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        if (player.playerStat.IsShield)
        {
            player.playerStat.IsShield = false;
            player.playerStat.CancelInvoke("DeactivateShield");
            Debug.Log("쉴드 덕에 피해 입지 않음");
            player.stateMachine.ChangeState(new IdleState(player));
            return;
        }


        player.rb.velocity = Vector2.zero;
        player.isAnimationOver = false;
        player.isInvincible = true;
        player.mainSprite.color = new Color32(255, 255, 255, 50);
        player.rb.AddForce(knockBackDir * 5, ForceMode2D.Impulse);
        // Debug.Log("넉백 적용됨");
        player.animator.SetTrigger("Damaged");
        player.playerStat.TakeDamage(damage);
    }

    public void Exit()
    {
        player.isUsingSkill = false;
        player.OnInvincible();
    }

    public void Update(Vector2 mousePos)
    {
        if (player.isAnimationOver)
        {
            player.targetX = player.transform.position.x;
            player.stateMachine.ChangeState(player.IdleState);
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetKnockBackDir(Vector2 dir)
    {
        knockBackDir = dir;
    }
}
