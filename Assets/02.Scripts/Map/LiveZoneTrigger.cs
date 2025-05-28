using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveZoneTrigger : MonoBehaviour
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
                    enemy.Killed("맵 이탈");
                }
            }
        }
    }
}