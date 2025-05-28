using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    NewPlayerContorller player;
    public IdleState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("아이들로 진입");
        player.isAnimationOver = false;
        //애니메이션 재생
    }

    public void Exit()
    {

    }


    void IState.Update(Vector2 mousePos)
    {
        float distance = player.targetX - player.transform.position.x;
        float direction = Mathf.Sign(distance);

        if (player.IsOnMovingPlatform())
        {
            if(player.isOnMovingPlatformClick == true)
            {
                if (Mathf.Abs(distance) > 0.05f)
                {
                    player.stateMachine.ChangeState(new MoveState(player));
                }
            }
        }
        else
        {

            if (Mathf.Abs(distance) > 0.05f)
            {
                player.stateMachine.ChangeState(new MoveState(player));
            }
        }
    }
}
