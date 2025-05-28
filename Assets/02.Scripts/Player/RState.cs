using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RState : IState
{
    NewPlayerContorller player;
    public RState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        //애니메이션 재생
        //player.StackController.UsingSkill();
        player.skillInfoUI.StartCoroutine(player.skillInfoUI.SkillCoolTime(3, player.rSkillData.CoolTime, player.rSkillData.SkillID));
        //
        player.stateMachine.ChangeState(player.IdleState);
    }

    public void Exit()
    {
        player.isUsingSkill = false;
    }


    void IState.Update(Vector2 mousePos)
    {

    }
}
