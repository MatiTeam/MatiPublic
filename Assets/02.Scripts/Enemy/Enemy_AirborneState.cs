using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_AirborneState : IState
{
    Enemy enemy;
    public float duration;
    public Enemy_AirborneState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("에어본 상태 진입");
        enemy.isCC = true;
        enemy.enemySprite.color = Color.cyan;
        if (enemy.rb != null)
        {
            enemy.rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        enemy.SetCCCo(WaitForDuration());
    }

    public void Exit()
    {
        if (enemy.rb != null)
        {
            enemy.rb.bodyType = RigidbodyType2D.Dynamic;
        }
        enemy.isCC = false;
        enemy.enemySprite.color = Color.white;
        Debug.Log("에어본 해제");
        enemy.TakeDamage((int)DataManager.Instance.skillManager.skillDatas.skillDic["S0006"].Damage);
        enemy.ClearCCCo();
    }

    public void Update(Vector2 mousePos)
    {
        enemy.transform.position += Vector3.up * 0.5f * Time.deltaTime;
    }

    IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(duration);
        enemy.StateMachine.ChangeState(new EnemyIdleState(enemy));
    }
}
