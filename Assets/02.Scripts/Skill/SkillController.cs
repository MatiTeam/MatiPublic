using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class SkillController
{
    public Dictionary<string, float> lastUsedTime = new(); //사용한 스킬의 마지막 사용 시간을 저장한 딕셔너리
    //#region Field
    //[Header("SkillDatas")]
    //public SkillData basicAttackData;
    //public SkillData qSkillData;
    //public SkillData wPassiveData;
    //public SkillData wStack1Data;
    //public SkillData wStack2Data;
    //public SkillData wStack3Data;
    //public SkillData eSkillData;
    //public SkillData rSkillData;

    //public int wSkillStack;
    //public Vector2 MousePos;
    //public LayerMask targetLayer;
    //private Rigidbody2D rb;
    //private PlayerController playerController;
    //public GameObject SelectedEnemy;
    //private bool isUsingSkill = false;

    //[Header("SkillCooltime")]
    //private float lastBasicAttack;
    //private float lastTimeQ;

    //#endregion
    //private void Start()
    //{
    //    Init();
    //}

    //#region 초기화
    //private void Init()
    //{
    //    basicAttackData = GameManager.Instance.skillDatas.GetSkillData("S0001");
    //    qSkillData = GameManager.Instance.skillDatas.GetSkillData("S0002");
    //    wPassiveData = GameManager.Instance.skillDatas.GetSkillData("S0003");
    //    wStack1Data = GameManager.Instance.skillDatas.GetSkillData("S0004");
    //    wStack2Data = GameManager.Instance.skillDatas.GetSkillData("S0005");
    //    wStack3Data = GameManager.Instance.skillDatas.GetSkillData("S0006");
    //    eSkillData = GameManager.Instance.skillDatas.GetSkillData("S0007");
    //    rSkillData = GameManager.Instance.skillDatas.GetSkillData("S0008");

    //    rb = GetComponent<Rigidbody2D>();
    //    playerController = GetComponent<PlayerController>();
    //    wSkillStack = 0;

    //    lastBasicAttack = 0;
    //    lastTimeQ = 0;
    //}
    //#endregion

    //private void Update()
    //{
    //    lastBasicAttack -= Time.deltaTime;
    //    lastTimeQ -= Time.deltaTime;
    //}

    /// <summary>
    /// 키 입력시 호출되는 함수에서 현재 쿨타임이 남아있는지 확인해주는 함수
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsSkillAvailable(string skillID, SkillData data)
    {
        if (!lastUsedTime.ContainsKey(skillID))
            return true;

        float timeSinceUse = Time.time - lastUsedTime[skillID];
        Debug.Log($"{timeSinceUse} >= {data.CoolTime}, 사용가능여부: {timeSinceUse >= data.CoolTime}");
        return timeSinceUse >= data.CoolTime;
    }

    /// <summary>
    /// 스킬 사용시 현재 시간으로 갱신해주기.
    /// 
    /// </summary>
    /// <param name="skillID"></param>
    public void UseSkill(string skillID)
    {
        lastUsedTime[skillID] = Time.time;
    }

    
    /// <summary>
    /// 기본공격시 쿨타임 감소하는 로직
    /// </summary>
    /// <param name="reductionAmount"></param>
    public void ReduceCoolTimeOnBaseAttack(float reductionAmount) //매개변수는 쿨감 적용할 시간
    {
        foreach (var skill in DataManager.Instance.skillManager.skillDatas.skillDic.Values)
        {
            string id = skill.SkillID;

            if (!lastUsedTime.ContainsKey(id)) //게임 시작 후로 한번도 사용하지 않아서 딕셔너리에 없는 예외를 처리해줌
                continue;

            float currentTime = Time.time; //게임 시작 이후로 흐른 시간
            float lastTimeUsed = lastUsedTime[id]; //각각 스킬을 사용했을 때의 마지막 시점
            float cooldown = skill.CoolTime; //쿨타임

            float remainingCooldown = cooldown - (currentTime - lastTimeUsed); //쿨타임에서,현재만큼 흐른시간에서 스킬을 사용한 마지막시점을 빼준값을 빼줌=> 다시 사용가능한 시간까지 남은 시간

            if (remainingCooldown > 0f)
            {
                lastUsedTime[id] -= reductionAmount; //쿨감적용할 시간만큼 마지막 사용시간에 더함=> 상대적으로 현재만큼 흐른 시간에 가까워지면서 쿨감 효과

                float newRemaining = cooldown - (currentTime - lastUsedTime[id]);
                if (newRemaining < 0f) //만약 쿨감 효과가 과하게 적용될 경우, 쿨타임이 마이너스가 되어 계산이 이상하게 동작하게 됨, 예외처리로 강제로 남은 시간을 0으로 맞춤
                {
                    lastUsedTime[id] = currentTime - cooldown; //쿨타임이 마이너스가 되지 않도록 마지막으로 사용했던 시점을 쿨타임이 끝나는 시점(남은시간==0)으로 강제 조정
                }
            }
        }
    }

    //public void UseBasicAttack(InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Started && isUsingSkill == false)
    //    {
    //        if (lastBasicAttack > 0)
    //        {
    //            Debug.Log(lastBasicAttack);
    //            return;
    //        }
    //        isUsingSkill = true;
    //        Vector2 dir;
    //        if (GetMousePosition().x > 0.5f)
    //            dir = Vector2.right;
    //        else
    //            dir = Vector2.left;

    //        RaycastHit hit;
    //        if (Physics.Raycast(transform.position, dir, out hit, basicAttackData.EffectRange, targetLayer))
    //        {
    //            Debug.Log("기본 공격 맞음 : " + hit.collider.name);
    //            DummyEnemy enemy = hit.collider.GetComponent<DummyEnemy>();
    //            if (enemy != null)
    //                enemy.TakeDamage((int)basicAttackData.Damage);
    //        }

    //        lastBasicAttack = basicAttackData.CoolTime;

    //        Debug.DrawRay(transform.position, dir.normalized * basicAttackData.EffectRange, Color.yellow, 1f);
    //        isUsingSkill = false;
    //    }
    //}

    //public void UseSkillQ(InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Started && isUsingSkill == false)
    //    {
    //        if (lastTimeQ > 0)
    //        {
    //            Debug.Log(lastTimeQ);
    //            return;
    //        }
    //        isUsingSkill = true;
    //        wSkillStack = Mathf.Min(wSkillStack + 1, 3);
    //        Vector2 dir;
    //        if (GetMousePosition().x > 0.5f)
    //            dir = Vector2.right;
    //        else
    //            dir = Vector2.left;

    //        Ray ray = new Ray(transform.position, dir.normalized);
    //        Debug.Log(qSkillData.Name);
    //        RaycastHit[] hits = Physics.RaycastAll(ray, qSkillData.EffectRange, targetLayer);

    //        foreach (RaycastHit hit in hits)
    //        {
    //            Debug.Log("Q스킬 맞음: " + hit.collider.name);

    //            DummyEnemy enemy = hit.collider.GetComponent<DummyEnemy>();
    //            if (enemy != null)
    //            {
    //                enemy.TakeDamage((int)qSkillData.Damage);
    //            }
    //        }
    //        lastTimeQ = qSkillData.CoolTime;

    //        Debug.DrawRay(transform.position, dir.normalized * qSkillData.EffectRange, Color.red, 1f);
    //        isUsingSkill = false;
    //    }
    //}

    //public void UseSkillW(InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Started && isUsingSkill == false)
    //    {

    //    }
    //}

    //#region E스킬
    ///// <summary>
    ///// E스킬 사용할 때 호출되는 함수
    ///// 스킬 사거리 내에 있는 적을 선택해 점프 후 돌진하여 피해를 입힙니다.
    ///// </summary>
    ///// <param name="context"></param>
    //public void UseSkillE(InputAction.CallbackContext context)
    //{
    //    //스킬 쿨타임 중이라면 리턴 => 추후 예정
    //    float eSkillRange = eSkillData.EffectRange;
    //    if (GetSelectedEnemy(eSkillRange) == null) return; // 마우스 커서에 적이 없거나, 있지만 스킬 사거리 밖일 때 리턴

    //    if (context.phase == InputActionPhase.Started && isUsingSkill == false)
    //    {
    //        isUsingSkill = true;
    //        wSkillStack = Mathf.Min(wSkillStack + 1, 3);
    //        Debug.Log("E스킬 들어왔다");
    //        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
    //        Vector2 enemyPos = new Vector2(SelectedEnemy.transform.position.x, SelectedEnemy.transform.position.y);
    //        if (rb != null)
    //        {
    //            rb.velocity = new Vector2(rb.velocity.x, 0); //현재 y축 속도를 초기화해 점프를 부드럽게 만듦
    //            rb.AddForce(Vector2.up * (SelectedEnemy.transform.position.y + 3f), ForceMode2D.Impulse);
    //            //연출로 이 타이밍에 애니메이션으로 제어해주는 것도 추가하면 좋을듯

    //            Vector2 dir = (SelectedEnemy.transform.position - transform.position).normalized; //돌진 방향 정하기
    //            //rb.velocity = Vector2.zero;
    //            rb.AddForce(dir * 100f, ForceMode2D.Impulse); // 돌진

    //            SelectedEnemy.GetComponent<DummyEnemy>().TakeDamage((int)eSkillData.Damage); // 적에게 데미지 입히기, 추후 수정 예정
    //        }
    //        isUsingSkill = false;
    //    }
    //}
    //#endregion
    //#region R스킬
    //public void UseSkillR(InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Started && isUsingSkill == false)
    //    {

    //    }
    //}
    //#endregion

    //#region Common
    ///// <summary>
    ///// 마우스 커서와 부딪힌 적을 반환합니다.
    ///// </summary>
    ///// <returns></returns>
    //private Vector2 GetMousePosition()
    //{
    //    Vector2 mousePos = Mouse.current.position.ReadValue();
    //    return Camera.main.ScreenToWorldPoint(mousePos);
    //}
    //private GameObject GetSelectedEnemy(float skillRange)
    //{
    //    MousePos = GetMousePosition();
    //    Collider2D hit = Physics2D.OverlapPoint(MousePos, targetLayer);
    //    if (hit != null)
    //    {
    //        GameObject target = hit.gameObject;
    //        float distance = Vector2.Distance(transform.position, target.transform.position);
    //        if (distance <= skillRange)
    //        {
    //            Debug.Log("선택된 적: " + target.name);
    //            SelectedEnemy = target;
    //            return SelectedEnemy;
    //        }
    //        else
    //        {
    //            Debug.Log("적은 있지만 사거리 밖");
    //            return null;
    //        }
    //    }
    //    Debug.Log("커서 아래에 적 없음");
    //    return null;
    //}



    ////private void OnUseNormalSkill()
    ////{
    ////    isUsingSkill = true;
    ////    //q스킬 : 레이 쏘기 테이크 데미지
    ////    //w1스택 쉴드씌우기
    ////    skills.wSkillStack = Mathf.Min(skills.wSkillStack + 1, 3);
    ////    isUsingSkill = false;
    ////}
    //#endregion
}
