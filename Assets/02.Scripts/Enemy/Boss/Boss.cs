using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Boss : MonoBehaviour
{
    public BossDataSO bossData;
    public List<Action> bossSkillList = new List<Action>();

    public NewPlayerContorller player;
    public Animator animator;

    protected float curHP;
    public float CurHP
    {
        get { return curHP; }
        set { curHP = value; }
    }
    public BossUI bossHPBar;

    public abstract void TakeDamage(float damage);

    public void SetHPBar()
    {
        bossHPBar.SetHPFillAmount(CurHP / bossData.MaxHP);
    }
}
