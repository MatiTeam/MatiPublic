using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackState : IState
{
    private Enemy enemy;
    private GameObject projectilePrefab;

    public EnemyMeleeAttackState(Enemy enemy, GameObject projectilePrefab)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
    }

    public void Enter()
    {
        enemy.animator.SetTrigger("IsAttack");
        FireProjectile();
        enemy.StateMachine.ChangeState(new EnemyAttackState(enemy)); // 즉시 상태 복귀
    }

    public void Update(Vector2 playerPos) { }

    public void Exit() { }

    private void FireProjectile()
    {
        Vector2 spawnPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y + 0.77f);
        Vector2 target = enemy.player.position;
        Vector2 direction = (target - spawnPos).normalized;

        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Init(direction, 0.2f);
        // Debug.Log("근접 공격 완료");
    }
}
