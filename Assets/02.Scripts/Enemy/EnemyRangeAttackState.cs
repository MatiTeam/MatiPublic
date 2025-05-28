using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이것도 오브젝트 풀 해야하겠네...
public class EnemyRangeAttackState : IState
{
    private Enemy enemy;
    private GameObject projectilePrefab;

    public EnemyRangeAttackState(Enemy enemy, GameObject projectilePrefab)
    {
        this.enemy = enemy;
        this.projectilePrefab = projectilePrefab;
    }

    public void Enter()
    {
        Debug.Log("원거리 공격 스테이트");
        FireProjectile();
        enemy.StateMachine.ChangeState(new EnemyAttackState(enemy)); // 즉시 상태 복귀
    }

    public void Update(Vector2 playerPos) { }

    public void Exit() { }

    private void FireProjectile()
    {
        Vector2 spawnPos = enemy.transform.position;
        Vector2 target = enemy.player.position;
        Vector2 direction = (target - spawnPos).normalized;

        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Init(direction);
        Debug.Log("🎯 투사체 발사 완료");
    }
}
