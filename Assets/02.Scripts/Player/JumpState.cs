using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
    NewPlayerContorller player;
    public JumpState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        //애니메이션 재생
        player.animator.SetBool("IsJump",true);
    }

    public void Exit()
    {
        player.animator.SetBool("IsJump", false);
    }


    void IState.Update(Vector2 mousePos)
    {
        if (player.IsGrounded())
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
        else
            player.stateMachine.ChangeState(new IdleState(player));
    }
}
