using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class B1001 : Boss
{
    public Action skill01;
    public Action skill02;

    public GameObject projectile01;
    public GameObject projectile02;

    public bool isInAction;
    public AudioSource audioSource;

    private void Awake()
    {
        CurHP = bossData.MaxHP;
        skill01 = PlayAnimationSkill_01;
        skill02 = PlaytAnimationSkill_02;
        bossSkillList.Add(skill01);
        bossSkillList.Add(skill02);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        player = FindObjectOfType<NewPlayerContorller>().GetComponent<NewPlayerContorller>();
        bossHPBar = FindObjectOfType<BossUI>().GetComponent<BossUI>();
        SetHPBar();
        StartCoroutine(SkillLoop());
    }

    public void StartCoSkill01()
    {
        Debug.Log("애니메이션 이벤트 실행");
        StartCoroutine(Skill_01());
    }

    IEnumerator Skill_01()
    {
        Debug.Log("코루틴 실행");
        int fireCount = 3;

        for (int i = 0; i < fireCount; i++)
        {
            FireCircle();
            yield return new WaitForSeconds(0.5f);
        }

        isInAction = false; // 스킬 종료
    }

    public void PlayAnimationSkill_01()
    {
        //애니메이션
        animator.SetTrigger("IsUseSkill01");
    }

    public void FireCircle()
    {
        float angleStep = 360f / 12f;
        float angle = 0f;
        AudioManager.Instance.PlaySFX("BossSkillBullet");
        for (int i = 0; i < 12; i++)
        {
            // 방향 계산 (2D 기준)
            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 dir = new Vector3(dirX, dirY, 0).normalized;

            // 총알 생성
            Projectile bullet = Instantiate(projectile01, transform.position, Quaternion.identity).GetComponent<Projectile>();
            bullet.speed = 7f;
            bullet.transform.localScale = new Vector2(0.3f, 0.3f);
            // 총알에 방향 적용
            bullet.Init(dir, 10);

            angle += angleStep;
        }
    }

    public void StartCoSkill02()
    {
        Debug.Log("애니메이션2 이벤트 실행");
        StartCoroutine(Skill_02());
    }

    IEnumerator Skill_02()
    {
        Debug.Log("코루틴 실행");
        for (int i = 0; i < 5; i++)
        {
            FallStone();
            yield return new WaitForSeconds(0.8f);
        }
        isInAction = false; // 스킬 종료
    }

    public void PlaytAnimationSkill_02()
    {
        animator.SetTrigger("IsUseSkill02");
    }

    public void Skill02Sound()
    {
        AudioManager.Instance.PlaySFX("BossSkillJumpDown");
    }

    public void FallStone()
    {
        Vector3 spawnPos = new Vector3(player.transform.position.x, 12, 0);
        Projectile bullet = Instantiate(projectile02, spawnPos, Quaternion.identity).GetComponent<Projectile>();
        bullet.transform.localScale = Vector2.one;
        bullet.Init(Vector2.down, 10);
    }

    IEnumerator SkillLoop()
    {
        yield return new WaitForSeconds(1f); // 처음 대기 시간

        while (true)
        {
            isInAction = true;

            int num = UnityEngine.Random.Range(0, bossSkillList.Count);
            bossSkillList[num]?.Invoke();

            // 스킬 끝날 때까지 대기
            yield return new WaitUntil(() => isInAction == false);

            // 스킬 종료 후 쿨타임만큼 대기
            yield return new WaitForSeconds(bossData.CoolTime);
        }
    }

    public override void TakeDamage(float damage)
    {
        CurHP -= damage;
        SetHPBar();
        if (CurHP <= 0)
        {
            StopAllCoroutines();
            animator.SetTrigger("IsDie");
            //Die;
        }
    }

    public void Die()
    {
        DataManager.Instance.flowManager.MoveToNextFlow();
        DataManager.Instance.flowManager.MoveToStoryScene();
    }
}
