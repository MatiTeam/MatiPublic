using UnityEngine;

public class EnemyKnockbackState : IState
{
    private Enemy enemy;
    private Vector2 knockbackDirection;
    private float knockbackSpeed = 8f;
    private float knockbackDuration = 0.2f;
    private float timer = 0f;

    public EnemyKnockbackState(Enemy enemy, Vector2 knockbackDir)
    {
        this.enemy = enemy;
        this.knockbackDirection = knockbackDir.normalized;
    }

    public void Enter()
    {
        timer = 0f;
        enemy.animator.SetTrigger("IsDamaged");
    }

    public void Update(Vector2 playerPos)
    {
        timer += Time.deltaTime;

        enemy.transform.position += (Vector3)(knockbackDirection * knockbackSpeed * Time.deltaTime);

        if (timer >= knockbackDuration)
        {
            Debug.Log("🛑 넉백 종료, 기본 상태로 복귀");

            // 상황에 따라 Idle로 갈지 Fight로 갈지 결정 가능
            if (Vector2.Distance(enemy.transform.position, playerPos) <= enemy.detectRange)
            {
                enemy.StateMachine.ChangeState(new EnemyFightState(enemy));
            }
            else
            {
                enemy.StateMachine.ChangeState(new EnemyIdleState(enemy));
            }
        }
    }

    public void Exit()
    {
        // 필요하면 넉백 종료 처리
    }
}
