using UnityEngine;

public class EnemyIdleState : IState
{
    private Enemy enemy;
    private Vector2 targetPos;
    private bool goingRight = true;

    public EnemyIdleState(Enemy enemy)
    {
        this.enemy = enemy;
        targetPos = enemy.patrolRight;
    }

    public void Enter() { }

    public void Update(Vector2 playerPos)
    {
        // 플레이어 감지
        if (Vector2.Distance(enemy.transform.position, playerPos) < enemy.detectRange)
        {
            enemy.StateMachine.ChangeState(new EnemyFightState(enemy));
            return;
        }

        if (enemy.MoveType == "Fly")
        {
            // 순찰 이동
            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                targetPos,
                enemy.moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(enemy.transform.position, targetPos) < 0.1f)
            {
                goingRight = !goingRight;
                targetPos = goingRight ? enemy.patrolRight : enemy.patrolLeft;
            }
        } //지상 적 적용 불가
    }

    public void Exit() { }
}