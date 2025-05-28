using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
// public class EnemyData
// {
//     public string EnemyID;
//     public string Name;
//     public int MaxHealth;
//     public float Speed;
//     public string PrefabPath;
// }

[System.Serializable]
public class EnemyDataList
{
    public List<EnemyData> enemies;
}


public class EnemyDataManager : MonoBehaviour
{
    public static EnemyDataManager Instance;

    private Dictionary<string, EnemyData> enemyDataDict = new Dictionary<string, EnemyData>();
    private string _path = "EnemyData/EnemyDataTable"; // Resources 폴더 기준

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadEnemyData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadEnemyData()
    {
        TextAsset json = Resources.Load<TextAsset>(_path);
        if (json == null)
        {
            Debug.LogError($"Enemy data not found at {_path}");
            return;
        }

        EnemyDataList list = JsonUtility.FromJson<EnemyDataList>(json.text);
        foreach (var enemy in list.enemies)
        {
            if (!enemyDataDict.ContainsKey(enemy.MonsterID))
            {
                enemyDataDict.Add(enemy.MonsterID, enemy);
            }
        }
    }

    public EnemyData GetEnemyData(string id)
    {
        return enemyDataDict.TryGetValue(id, out var data) ? data : null;
    }
}
