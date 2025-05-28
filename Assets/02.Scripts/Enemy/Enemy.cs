using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable<EnemyData>
{
    //기본 정보
    private EnemySpawner spawner;
    private Transform spawnerTransform;
    [SerializeField] private EnemyData enemyData;
    public Transform player;
    public GameObject Bullet;
    public float lastPositionX;
    public PlatformGraphBuilder graphBuilder;
    public Rigidbody2D rb;
    [SerializeField] public Transform footTransform;
    private bool isKilled = false;
    public bool isDead = false;
    private Coroutine CCCo;

    private Action onDeath;

    //적 타입별 정보
    public string MonsterID;
    private string MonsterName;
    public string Behavior;
    public string AttckType;
    public string MoveType;
    public string Skill1;
    public string Skill2;
    public string Skill3;
    private int maxHealth;
    private int currentHealth;
    public float detectRange = 10f;
    public float attackRange = 3f;
    public float chaseRange = 7f;
    public float lastAttackTime;
    public float moveSpeed;
    public float Cooltime;
    public float ContactDamage;
    public Vector2 patrolLeft;
    public Vector2 patrolRight;
    public Vector2 origin;
    public Animator animator;

    //상태머신 정보
    public StateMachine StateMachine;
    public bool IsWalking = false;
    public bool IsComeback = false;
    public SpriteRenderer enemySprite;
    public bool isCC = false;
    public bool isAnimationOver = false;

    //강제 스테이트
    public IState enemyStunState;
    public IState enemyAirborneState;
    public IState enemyPolymorphState;
    public IState enemyInabilityState;

    void Start()
    {
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new EnemyIdleState(this));
        enemySprite = GetComponent<SpriteRenderer>();
        enemyStunState = new Enemy_StunState(this);
        enemyAirborneState = new Enemy_AirborneState(this);
        enemyPolymorphState = new Enemy_PolymorphState(this);

        lastAttackTime = Time.time;
        lastPositionX = transform.position.x;
    }

    void Update()
    {
        Vector2 playerPos = player.position;
        StateMachine.Update(playerPos);
    }

    void FixedUpdate()
    {
        SetSpriteFlip();
    }

    //오브젝트 풀
    public void SetSpawner(EnemySpawner spawner, Transform spawnerTransform)
    {
        this.spawner = spawner;
        this.spawnerTransform = spawnerTransform;
    }

    public void SetPlayer(Transform input)
    {
        player = input;
    }

    public void SetPatrol(Vector2 start, Vector2 end, Vector2 spawnPos)
    {
        patrolLeft = start;
        patrolRight = end;
        origin = spawnPos;
    }

    public void SetBullet(GameObject input)
    {
        Bullet = input;
    }

    public void SetOnDeathCallback(Action callback)
    {
        onDeath = callback;
    }

    public void SetGraphBuilder(PlatformGraphBuilder input)
    {
        graphBuilder = input;
    }

    //초기화 함수
    public void OnSpawn(EnemyData data)
    {
        isDead = false;
        enemyData = data;

        MonsterID = data.MonsterID;
        MonsterName = data.Name;
        Behavior = data.BehaviorPattern;
        if (Behavior == "Close")
        {
            detectRange = 15f;
            attackRange = 2f;
            chaseRange = 2f;
        }
        else if (Behavior == "Chasing")
        {
            detectRange = 10f;
            attackRange = 3f;
            chaseRange = 6f;
        }
        else if (Behavior == "Patrol")
        {
            detectRange = 10f;
            attackRange = 5f;
            chaseRange = 10f;
        }
        else if (Behavior == "Dummy")
        {
            detectRange = 0f;
            attackRange = 0f;
            chaseRange = 0f;
        }
        else
        {
            Debug.Log("유효하지 않은 행동 패턴");
            detectRange = 1f;
            attackRange = 1f;
            chaseRange = 1f;
        }
        AttckType = data.AttackType;
        MoveType = data.MoveType;
        Skill1 = data.Skill1ID;
        Skill2 = data.Skill2ID;
        Skill3 = data.Skill3ID;
        maxHealth = data.MaxHp;
        currentHealth = maxHealth;
        moveSpeed = data.MoveSpeed;
        Cooltime = data.Cooltime;
        ContactDamage = data.ContactDamage;

        animator = GetComponent<Animator>();
        var loadController = Resources.Load<RuntimeAnimatorController>($"Animations/" + data.MonsterID);
        if (loadController != null)
        {
            // Debug.Log("들어있긴 함");
            animator.runtimeAnimatorController = loadController;
        }
        Debug.Log($"적 생성됨: {MonsterName}, 체력: {currentHealth}");

        //땅에서 움직이는 적 콜라이더 추가
        AddCollider();
    }

    //초기화 함수
    public void OnDespawn()
    {
        enemyData = null;

        name = null;
        maxHealth = 0;
        currentHealth = 0;

        graphBuilder = null;

        // Debug.Log("적 소멸");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("적이 " + damage + "만큼 데미지를 입음. 현재 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            PlayDieAnimation();
            //Die();
        }
    }

    //상태이상 함수 - 지면으로 내리 침
    public void Downward()
    {
        // 레이캐스트 시작 위치 (현재 위치)
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        Vector2 direction = Vector2.down;
        float maxDistance = 100f;
        int groundLayerMask = LayerMask.GetMask("Ground");

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, groundLayerMask);

        if (hit.collider != null)
        {
            // 히트 지점의 y에 약간 여유를 두고 배치
            float offsetY = 0.1f; // 적의 pivot 위치에 따라 조절 필요
            transform.position = new Vector2(transform.position.x, hit.point.y + offsetY);

            // Debug.Log("지면 감지됨. 위치 이동: " + transform.position);
        }
        else
        {
            // Debug.LogWarning("지면을 찾지 못했습니다.");
        }
    }

    private void PlayDieAnimation()
    {
        animator.SetTrigger("IsDie");
    }

    public void Killed(string reason = "")
    {
        if (isKilled == false && isDead == false)
        {
            isKilled = true;
            Debug.Log($"적 {MonsterName} 제거됨: {reason}");
            Die();
        }
    }

    public void Die()
    {
        if (isDead == false)
        {
            isDead = true;
            onDeath?.Invoke();
            spawner.ReturnEnenmy(this);
        }
    }

    public void SetAnimationOver()
    {
        isAnimationOver = true;
    }

    private void SetSpriteFlip()
    {
        if (lastPositionX == transform.position.x) return;
        enemySprite.flipX = transform.position.x < lastPositionX;
        lastPositionX = transform.position.x;
    }

    public void SetCCCo(IEnumerator enumerator)
    {
        CCCo = StartCoroutine(enumerator);
    }

    public void ClearCCCo()
    {
        StopCoroutine(CCCo);
    }

    private void AddCollider()
    {
        if (MoveType == "Ground")
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3;
            if (footTransform != null)
            {
                footTransform.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("하위에 'Foot' 오브젝트를 찾을 수 없습니다.");
            }
        }
        else
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            if (footTransform != null)
            {
                footTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("하위에 'Foot' 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject platform = other.gameObject;

        if (platform.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.parent = other.transform;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        GameObject platform = other.gameObject;

        if (platform.activeInHierarchy && platform.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.parent = spawnerTransform;
        }
    }
}
