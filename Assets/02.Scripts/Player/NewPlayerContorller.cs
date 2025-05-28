using System;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerContorller : MonoBehaviour
{
    #region Field
    [SerializeField] private Transform mati;
    public float moveSpeed = 10f;
    public float jumpForce = 7f;
    public float jumpPadPower;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public LayerMask globalLayer;

    public bool IsOnGround = false;
    public bool isOnMovingPlatformClick;

    public Rigidbody2D rb;
    public float targetX;
    public StateMachine stateMachine;
    public PlayerStat playerStat;
    public SpriteRenderer mainSprite;

    public GameObject qSkillEffect;
    public GameObject w3SkillEffect;

    [Header("State")]
    public IState IdleState;
    public IState MoveState;
    public IState JumpState;
    public IState DamagedState;
    public IState BaseAttackState;
    public IState QState;
    public IState WState;
    public IState EState;
    public IState RState;
    public IState StoryState;

    [Header("SkillDatas")]
    public SkillData basicAttackData;
    public SkillData qSkillData;
    public SkillData wPassiveData;
    public SkillData wStack1Data;
    public SkillData wStack2Data;
    public SkillData wStack3Data;
    public SkillData eSkillData;
    public SkillData rSkillData;

    public int wSkillStack;
    public Vector2 MousePos;
    public Camera _camera;
    public LayerMask targetLayer;
    public LayerMask bulletLayer; //이게 최선일까? 흠
    public GameObject SelectedEnemy;
    public bool isUsingSkill = false;
    public bool isAnimationOver = false;
    public bool isInvincible = false;
    public bool isOnJumpPad = false;
    public bool isLeft = false;
    public Animator animator;
    public BoxCollider2D playerCollider;

    [Header("SkillCooltime")]
    public float lastBasicAttack;
    public float lastTimeQ;

    [Header("DetectVariable")]
    public bool EstateOver = false; //그럼 적이 움직여서 안 닿았을때는? 생각해봐야할듯

    [Header("Skill UI")]
    public StackController stackController;
    public SkillInfoUI skillInfoUI;

    PlayerSFXController playerSFXController;
    public ClickIndicatorController clickIndicatorController;

    [Header("Particle")]
    [SerializeField] private ParticleSystem dustParticle;

    public event System.Action OnSpacePressed;

    #endregion


    #region 초기화
    void Awake()
    {
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        JumpState = new JumpState(this);
        DamagedState = new DamagedState(this);
        BaseAttackState = new BaseAttackState(this);
        QState = new QState(this);
        WState = new WState(this);
        EState = new EState(this);
        RState = new RState(this);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStat = GetComponent<PlayerStat>();
        playerCollider = GetComponent<BoxCollider2D>();
        stateMachine = new StateMachine();
        stackController = FindObjectOfType<StackController>();
        skillInfoUI = FindObjectOfType<SkillInfoUI>();
        playerSFXController = GetComponent<PlayerSFXController>();
        clickIndicatorController = GetComponent<ClickIndicatorController>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        stateMachine.ChangeState(new IdleState(this));
        basicAttackData = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0001");
        qSkillData = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0002");
        wPassiveData = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0003");
        wStack1Data = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0004");
        wStack2Data = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0005");
        wStack3Data = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0006");
        eSkillData = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0007");
        rSkillData = DataManager.Instance.skillManager.skillDatas.GetSkillData("S0008");
        rb = GetComponent<Rigidbody2D>();
        wSkillStack = 0;
        _camera = Camera.main;

        //if (DataManager.Instance.progressManager.locationData.playerPosX != 0 ||
        //    DataManager.Instance.progressManager.locationData.playerPosY != 0)
        //{
        //    transform.position = new Vector2(DataManager.Instance.progressManager.locationData.playerPosX,
        //        DataManager.Instance.progressManager.locationData.playerPosY);
        //}
        targetX = transform.position.x;
    }
    #endregion

    private void FixedUpdate()
    {
        stateMachine.Update(GetMousePosition());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        if (IsOnMovingPlatform())
        {
            isOnMovingPlatformClick = true;
        }
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        if(context.phase == InputActionPhase.Started)
        {
            Vector3 pos = new Vector3(GetMousePosition().x, transform.position.y, 0f);
            clickIndicatorController.ShowClickIndicator(pos);
        }
        targetX = worldPos.x;
        if (targetX < transform.position.x)
        {
            mainSprite.flipX = true;
            isLeft = true;
        }
        else
        {
            mainSprite.flipX = false;
            isLeft = false;
        }

        if (IsGrounded())
        {
            playerSFXController.PlayWalkSound();
        }
    }

    //이거 나중에 문제가 될 지도... 지금은 너무 강제로 점프 포스가 더해짐
    public void OnJump(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        if (isOnJumpPad)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, jumpPadPower);
            OnSpacePressed?.Invoke();
            return;
        }

        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //stateMachine.ChangeState(JumpState);
            playerSFXController.PlayerJumpSound();

            if (dustParticle != null)
                dustParticle.Play();
        }
    }


    public void UseBasicAttack(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        if (context.started &&
            DataManager.Instance.skillManager.controller.
            IsSkillAvailable(basicAttackData.SkillID, basicAttackData) && isUsingSkill == false)
        {
            isUsingSkill = true;

            if (GetMousePosition().x < transform.position.x)
            {
                mainSprite.flipX = true;
                isLeft = true;
            }
            else
            {
                mainSprite.flipX = false;
                isLeft = false;
            }

            DataManager.Instance.skillManager.controller.UseSkill(basicAttackData.SkillID);
            stateMachine.ChangeState(BaseAttackState);

            playerSFXController.AttackSound();
        }
    }

    public void UseSkillQ(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        if (context.started &&
            DataManager.Instance.skillManager.controller.
            IsSkillAvailable(qSkillData.SkillID, qSkillData) && isUsingSkill == false)
        {
            isUsingSkill = true;

            if (GetMousePosition().x < transform.position.x)
            {
                mainSprite.flipX = true;
                isLeft = true;
            }
            else
            {
                mainSprite.flipX = false;
                isLeft = false;
            }
            DataManager.Instance.skillManager.controller.UseSkill(qSkillData.SkillID);
            stateMachine.ChangeState(QState);

            playerSFXController.AttackSound();
        }
    }

    public void UseSkillW(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        if (stackController.currentStackIndex == 0) return;

        if (stackController.currentStackIndex == 2)
        {
            if (GetSelectedEnemy(wStack2Data.EffectRange) == null) return;
        }
        else if (stackController.currentStackIndex == 3)
        {
            Vector2 mousePos = GetMousePosition();
            if ((mousePos - (Vector2)transform.position).magnitude > wStack3Data.EffectRange)
            {
                Debug.Log("W스택3 사용 불가 : 거리가 너무 멉니다.");
                return;
            }
        }


        if (context.started && isUsingSkill == false)
        {
            isUsingSkill = true;
            if (GetMousePosition().x < transform.position.x)
            {
                mainSprite.flipX = true;
                isLeft = true;
            }
            else
            {
                mainSprite.flipX = false;
                isLeft = false;
            }
            stateMachine.ChangeState(WState);
        }

        if (stackController.currentStackIndex == 1)
            playerSFXController.WStack1Sound();
        else if (stackController.currentStackIndex == 2)
            playerSFXController.AttackSound();
        else if (stackController.currentStackIndex == 3)
            playerSFXController.WStack3Sound();
    }

    #region E스킬
    /// <summary>
    /// E스킬 사용할 때 호출되는 함수
    /// 스킬 사거리 내에 있는 적을 선택해 점프 후 돌진하여 피해를 입힙니다.
    /// </summary>
    /// <param name="context"></param>
    public void UseSkillE(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        float eSkillRange = eSkillData.EffectRange;
        if (GetSelectedEnemy(eSkillRange) == null) return; // 마우스 커서에 적이 없거나, 있지만 스킬 사거리 밖일 때 리턴

        if (context.started &&
        DataManager.Instance.skillManager.controller.
        IsSkillAvailable(eSkillData.SkillID, eSkillData) && isUsingSkill == false)
        {
            if (GetMousePosition().x < transform.position.x)
            {
                mainSprite.flipX = true;
                isLeft = true;
            }
            else
            {
                mainSprite.flipX = false;
                isLeft = false;
            }
            DataManager.Instance.skillManager.controller.UseSkill(eSkillData.SkillID);
            stateMachine.ChangeState(EState);
        }
        //isUsingSkill = true;
        //wSkillStack = Mathf.Min(wSkillStack + 1, 3);
        //Debug.Log("E스킬 들어왔다");
        //if (rb != null)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, 0); //현재 y축 속도를 초기화해 점프를 부드럽게 만듦
        //    float heightDifference = SelectedEnemy.transform.position.y - transform.position.y; //적과 플레이어의 y 높이차 계산
        //    float jumpForce = Mathf.Clamp(heightDifference + 10f, 5f, 20f); // 최소 5, 최대 20 제한
        //    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        //    SelectedEnemy.GetComponent<DummyEnemy>().TakeDamage((int)eSkillData.Damage); // 적에게 데미지 입히기, 추후 수정 예정
        //}
        //isUsingSkill = false;
    }

    /// <summary>
    /// E스킬에서, 점프 후 데미지 주기 전 호출되는 이벤트
    /// 지정한 적을 향해 돌진합니다. 
    /// </summary>
    /// <param name="enemy"></param>
    public void OnDash()
    {
        if (SelectedEnemy == null) return;

        Debug.Log("돌진합니다");
        Vector2 dir = (SelectedEnemy.transform.position - transform.position).normalized; //돌진 방향 정하기
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * 30f, ForceMode2D.Impulse); // 돌진

        playerSFXController.AttackSound();
    }
    #endregion
    #region R스킬
    public void UseSkillR(InputAction.CallbackContext context)
    {
        if (Time.timeScale < 1) return;

        if (context.started &&
         DataManager.Instance.skillManager.controller.
         IsSkillAvailable(rSkillData.SkillID, rSkillData) && isUsingSkill == false)
        {
            if (GetSelectedEnemy(rSkillData.EffectRange) == null) return;

            if (GetMousePosition().x < transform.position.x)
            {
                mainSprite.flipX = true;
                isLeft = true;
            }
            else
            {
                mainSprite.flipX = false;
                isLeft = false;
            }

            DataManager.Instance.skillManager.controller.UseSkill(rSkillData.SkillID);
            stateMachine.ChangeState(RState);
            mati.GetComponent<MatiPathFollower>().OnUsingR(SelectedEnemy);

            playerSFXController.SkillRSound();
        }
    }
    #endregion

    #region Common
    /// <summary>
    /// 마우스 커서 위치를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z); //카메라 z축 보정
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    /// <summary>
    /// 마우스 커서와 부딪힌 적을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public GameObject GetSelectedEnemy(float skillRange)
    {
        MousePos = GetMousePosition();
        Collider2D hit = Physics2D.OverlapPoint(MousePos, targetLayer);
        if (hit != null)
        {
            GameObject target = hit.gameObject;
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= skillRange)
            {
                Debug.Log("선택된 적: " + target.name);
                SelectedEnemy = target;
                return SelectedEnemy;
            }
            else
            {
                Debug.Log("적은 있지만 사거리 밖");
                return null;
            }
        }
        Debug.Log("커서 아래에 적 없음");
        return null;
    }

    public void SetAnimationOver()
    {
        isAnimationOver = true;
    }

    //private void OnUseNormalSkill()
    //{
    //    isUsingSkill = true;
    //    //q스킬 : 레이 쏘기 테이크 데미지
    //    //w1스택 쉴드씌우기
    //    skills.wSkillStack = Mathf.Min(skills.wSkillStack + 1, 3);
    //    isUsingSkill = false;
    //}
    #endregion



    public bool IsGrounded()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.6f, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, 0.5f, groundLayer);
        return hit.collider != null;
    }

    public bool IsOnMovingPlatform()
    {
        Vector2 origin = transform.position;
        //Vector2 size = new Vector2(0.6f, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.7f, groundLayer);
        if (hit.collider == null) return false;
        return hit.collider.CompareTag("MovingPlatform");
    }

    // 추가 내용
    // 플레이어가 상하로 움직이는 플랫폼 위에 있을때 흔들리는 문제 없이 같이 이동
    // E 스킬 적에 닿았을때 판정
    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject platform = other.gameObject;

        // 레이어가 "Ground"일 때
        if (platform.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.parent = other.transform;
            IsOnGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        GameObject platform = other.gameObject;

        // 충돌에서 벗어난 오브젝트가 아직 활성화 상태고 레이어가 "Ground"일 때
        // platform.activeInHierarchy하는 이유는 오브젝트가 비활성화 상태일 수 있기에 조건에 추가
        if (platform.activeInHierarchy && platform.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.parent = null;
            IsOnGround = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        JumpPlatform jumpPad = collision.GetComponent<JumpPlatform>();
        if (jumpPad != null)
        {
            jumpPadPower = jumpPad.jumpPower;
            isOnJumpPad = true;
            return;
        }
        if (EstateOver == false)
        {
            if (collision.gameObject == SelectedEnemy)
            {
                isAnimationOver = true;
                Debug.Log("적 잡음");
                EstateOver = true;
                SelectedEnemy.GetComponent<Enemy>()?.Downward();

            }
        }

        if (isInvincible) return;

        // Debug.Log("트리거 진입");
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Boss boss = collision.gameObject.GetComponent<Boss>();
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy == null && boss == null) return;
            if (enemy != null && enemy.isCC == true) return;


            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            Debug.Log(knockbackDir);
            if (DamagedState is DamagedState damagedState)
            {
                damagedState.SetDamage(1);
                damagedState.SetKnockBackDir(knockbackDir);
            }
            stateMachine.ChangeState(DamagedState);
            //playerStat.TakeDamage(1,rb,knockbackDir);//테스트용

            playerSFXController.PlayerHitSound();
        }

        if (((1 << collision.gameObject.layer) & bulletLayer) != 0) //위와 같은 코드임, bullet을 처리하기 위한 부분... 변경 필요
        {
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            Debug.Log(knockbackDir);
            if (DamagedState is DamagedState damagedState)
            {
                damagedState.SetDamage(1);
                damagedState.SetKnockBackDir(knockbackDir);
            }
            stateMachine.ChangeState(DamagedState);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        JumpPlatform jumpPad = collision.GetComponent<JumpPlatform>();
        if (jumpPad != null)
        {
            isOnJumpPad = false;
            rb.gravityScale = 4;
            return;
        }
    }

    public void SkillInvincible()
    {
        isInvincible = true;
        Invoke("OffInvincible", 0.3f);
    }

    public void OnInvincible()
    {
        isInvincible = true;
        mainSprite.color = new Color32(255, 255, 255, 50);
        Invoke("OffInvincible", 0.3f);
    }

    private void OffInvincible()
    {
        mainSprite.color = new Color32(255, 255, 255, 255);
        isInvincible = false;
    }

    public void SetQSkillEffect(Vector2 center)
    {
        if (qSkillEffect == null) return;
        qSkillEffect.transform.position = center;
        qSkillEffect.SetActive(true);
        Invoke("HideQSkillEffect", 0.5f);
    }

    public void HideQSkillEffect()
    {
        qSkillEffect.SetActive(false);
    }

    public void SetW3SkillEffect(Vector2 center)
    {
        if (w3SkillEffect == null) return;
        w3SkillEffect.transform.position = center;
        w3SkillEffect.SetActive(true);
        Invoke("HideW3SkillEffect", 2.5f);
    }

    public void HideW3SkillEffect()
    {
        w3SkillEffect.SetActive(false);
    }
}
