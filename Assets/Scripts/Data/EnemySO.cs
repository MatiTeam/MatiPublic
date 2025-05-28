using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Enemy", menuName = "GameData/CreateEnemyData")]

public class EnemySO : ScriptableObject
{
	public string MonsterID;
	public string Name;
	public string Rarity;
	public string BehaviorPattern;
	public string AttackType;
	public string Skill1ID;
	public string Skill2ID;
	public string Skill3ID;
	public int MaxHp;
	public float MoveSpeed;
	public float EmergenceTime;
	public int ContactDamage;
	public float Cooltime;
	public string AnimationHitEffect;
	public string MoveType;

}
