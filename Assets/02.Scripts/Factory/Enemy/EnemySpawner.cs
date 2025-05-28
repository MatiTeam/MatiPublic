using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private GameObject bullet;
    [SerializeField] private EnemyDataTableSO enemyDatas;
    [SerializeField] private PlatformGraphBuilder graphBuilder;
    private Factory<Enemy, EnemyData> enemyFactory;
    public Transform Player;

    //스테이지 데이터 로딩
    [SerializeField] private StageDataLoader dataLoader;

    void Start()
    {
        enemyFactory = new Factory<Enemy, EnemyData>(enemyPrefab, 10, this.transform);
    }

    public void SpawnEnemyById(
    string id,
    Vector2 spawnPos,
    bool isPatrol,
    Vector2 patrolMin,
    Vector2 patrolMax,
    Action onDeathCallback)
    {
        var data = enemyDatas.datas.Find(e => e.MonsterID == id);
        if (data == null)
        {
            Debug.LogError($"적 데이터 {id}를 찾을 수 없습니다.");
            return;
        }

        var enemy = enemyFactory.Create(data);
        enemy.transform.position = spawnPos;
        enemy.SetSpawner(this, this.transform);
        enemy.SetPlayer(Player);
        enemy.SetBullet(bullet);
        enemy.SetOnDeathCallback(onDeathCallback);
        enemy.SetGraphBuilder(graphBuilder);

        if (isPatrol)
            enemy.SetPatrol(patrolMax, patrolMin, spawnPos);
        else
            enemy.SetPatrol(spawnPos, spawnPos, spawnPos);

    }

    public void ReturnEnenmy(Enemy enemy)
    {
        enemyFactory.Release(enemy);
    }
}
