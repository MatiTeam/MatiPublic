using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Shield
}

public enum CCType
{
    Stun, Airborne, Polymorph
}
[System.Serializable]
public class SkillData
{
    public string SkillID;
    public string Icon;
    public string Name;
    public string Description;
    public string Type;
    public string TargetLayerMask;
    public bool IsArea;
    public float Damage;
    public float EffectRange;
    public float EffectArea;
    public BuffType BuffType;
    public float BuffTime;
    public CCType CCType;
    public float CCTime;
    public float MoveSpeed;
    public float CoolTime;
    public string Range;
    public float ProjectileSpeed;
    public string Animation;
}
