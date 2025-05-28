using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossBehavior
{
    Static
}

[CreateAssetMenu(fileName ="Boss",menuName = "New Boss")]
public class BossDataSO : ScriptableObject
{
    public string bossID;
    public string bossName;
    public BossBehavior bossBehavior;
    public float MaxHP;
    public float MoveSpeed;
    public float AttackSpeed;
    public float AttackDamage;
    public string AttackType;
    public float ContactDamage;
    public float CoolTime;
}
