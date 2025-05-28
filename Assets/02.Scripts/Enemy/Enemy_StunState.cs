using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_StunState : IState
{
    Enemy enemy;
    public float duration;
    public Enemy_StunState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.isCC = true;
        enemy.enemySprite.color = Color.yellow;
        enemy.SetCCCo(WaitForDuration());
    }

    public void Exit()
    {
        enemy.isCC = false;
        enemy.enemySprite.color = Color.white;
        enemy.ClearCCCo();
    }

    public void Update(Vector2 mousePos)
    {

    }

    IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(duration);
        enemy.StateMachine.ChangeState(new EnemyIdleState(enemy));
    }
}
