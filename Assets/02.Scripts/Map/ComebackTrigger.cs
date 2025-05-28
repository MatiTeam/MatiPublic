using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComebackTrigger : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (enemy.MonsterID == "M1001")
                {
                    enemy.StateMachine.ChangeState(new EnemyComebackState(enemy));
                }
            }
        }
    }
}