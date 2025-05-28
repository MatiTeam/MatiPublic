using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MatiPathFollower : MonoBehaviour
{
    //기본 움직임 설정 변수
    [SerializeField] private Transform player;
    [SerializeField] private PlatformGraphBuilder graphBuilder;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float nodeArrivalThreshold = 0.8f;

    //길찾기 변수
    private Rigidbody2D rb;
    private Collider2D col;
    [SerializeField] private List<PlatformNode> path = new();
    private int currentIndex = 0;
    private float repathInterval = 0.2f;
    private float repathTimer = 0f;
    private float repathDistanceThreshold = 1f;

    //점프에 관한 변수
    private bool isJumping = false;
    private Vector2 jumpStart;
    private Vector2 jumpTarget;
    private float jumpDuration = 0.4f;
    private float jumpTimer = 0f;
    private float jumpHeight = 1.2f;

    //점프플랫폼 관련 변수
    private bool isOnJumpPad;
    private float jumpPadPower;
    private bool shouldJumpOnPad = false;

    //낙하에 관한 변수
    private bool isFalling = false;
    private Vector2 fallStart;
    private Vector2 fallTarget;
    private float fallTimer = 0f;
    private float fallDuration = 0.3f;

    //너무 멀어졌을때 텔레포트에 관한 변수
    [SerializeField] private float teleportDistance = 10f;
    [SerializeField] private Vector2 teleportOffset = new Vector2(-1f, 0f);
    private float teleportCooldown = 1f;
    private float lastTeleportTime = -999f;

    //R 스킬에 관한 변수
    private bool isUsingR = false;
    private Vector2 rStartPos;
    private Vector2 rTargetPos;
    private GameObject rTargetObject;
    private float rSpeed = 15f;
    private bool returningFromR = false;
    private NewPlayerContorller controller;

    //이동 체크 관련 변수
    private Vector3 lastPos;
    public bool isMove = false;

    //마티 애니메이션 관련 변수
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    //디버그용 변수
    private Vector2 debugJumpTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        controller = player.GetComponent<NewPlayerContorller>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 플레이어의 스페이스바 입력 이벤트 구독
        if (controller != null)
        {
            controller.OnSpacePressed += HandleSpacePressed;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (controller != null)
        {
            controller.OnSpacePressed -= HandleSpacePressed;
        }
    }

    private void HandleSpacePressed()
    {
        if (isOnJumpPad)
        {
            shouldJumpOnPad = true;
        }
    }

    private void Update()
    {
        SpriteFlip();
        AnimationUpdate();
        //R 스킬 사용 구현
        if (isUsingR)
        {
            HandleRDash();
            return;
        }

        // 경로 재요청 타이머
        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval && !isFalling && !isJumping)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > repathDistanceThreshold)
            {
                RequestPath();
            }
            repathTimer = 0f;
        }

        FollowPath();
    }

    private void FixedUpdate()
    {
        CheckMoving();
    }

    public void ClearPath()
    {
        path?.Clear();
    }

    private void FollowPath()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer > teleportDistance && Time.time - lastTeleportTime > teleportCooldown)
        {
            lastTeleportTime = Time.time;

            Vector2 teleportPos = (Vector2)player.position + teleportOffset;

            isJumping = false;
            isFalling = false;
            path?.Clear();
            rb.simulated = true;
            rb.velocity = Vector2.zero;

            transform.position = teleportPos;
            currentIndex = 0;

            RequestPath();
            return;
        }

        if (isOnJumpPad && shouldJumpOnPad && !isJumping && !isFalling)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, jumpPadPower);
            return;
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float t = jumpTimer / jumpDuration;
            float FixedJumpHeight = jumpHeight;

            // 새로운 타겟 좌표 확인
            Vector2 newTarget = path[currentIndex].Position;

            // ✅ jumpTarget이 변경됐으면, 현재 위치를 jumpStart로 새로 잡고 타겟 갱신
            if (jumpTarget != newTarget)
            {
                jumpStart = transform.position;  // 현재 위치를 새 시작점
                jumpTarget = newTarget;          // 타겟 갱신
                debugJumpTarget = jumpTarget;
                FixedJumpHeight = 0;
            }

            // 현재 점프 진행 비율로 계산
            Vector2 mid = Vector2.Lerp(jumpStart, jumpTarget, t);
            mid.y += FixedJumpHeight * Mathf.Sin(Mathf.PI * t);
            transform.position = mid;

            if (t >= 1f)
            {
                isJumping = false;
                rb.simulated = true;

                currentIndex++;
                if (path == null || currentIndex >= path.Count)
                {
                    if (path != null)
                        path.Clear();
                    rb.velocity = Vector2.zero;
                    return;
                }
            }

            return; // 점프 중에는 다른 동작 무시
        }

        if (isFalling)
        {
            fallTimer += Time.deltaTime;
            float t = fallTimer / fallDuration;
            Vector2 mid = Vector2.Lerp(fallStart, fallTarget, t);
            mid.y -= 0.5f * Mathf.Sin(Mathf.PI * t);
            transform.position = mid;
            fallTarget = path[currentIndex].Position;

            if (t >= 1f)
            {
                isFalling = false;
                rb.simulated = true;

                currentIndex++;
                if (path == null || currentIndex >= path.Count)
                {
                    if (path != null)
                        path.Clear();
                    rb.velocity = Vector2.zero;
                    return;
                }
            }

            return;
        }

        if (path == null || path.Count == 0) return;
        if (currentIndex >= path.Count)
        {
            path.Clear();
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 targetPos = path[currentIndex].Position;
        Vector2 direction = targetPos - (Vector2)transform.position;
        float dist = direction.magnitude;

        if (dist < nodeArrivalThreshold && !isFalling && !isJumping)
        {
            currentIndex++;
            return;
        }

        // 점프 조건
        if ((direction.y > 0.5f || Mathf.Abs(direction.x) > 2.5f) && direction.y < 5f && !isJumping && !isFalling)
        {
            // 점프 시작
            jumpStart = transform.position;
            jumpTarget = targetPos;
            jumpTimer = 0f;
            isJumping = true;
            rb.simulated = false;
            return;
        }

        // 낙하
        if (direction.y < -0.5f && !isFalling && !isJumping)
        {
            fallStart = transform.position;
            fallTarget = targetPos;
            fallTimer = 0f;
            isFalling = true;
            rb.simulated = false;
            return;
        }

        // 걷기 이동 (y 차이 없을 때만)
        if (Mathf.Abs(direction.y) < 0.5f)
        {
            float moveDir = Mathf.Sign(direction.x);
            rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
        }
    }


    public void RequestPath()
    {
        PlatformNode start = graphBuilder.FindClosestNodeTo(transform.position);
        PlatformNode goal = graphBuilder.FindClosestNodeTo(player.position);
        if (start == null && goal == null)
        {
            // Debug.LogWarning("경로 탐색 실패: 시작 노드 또는 목표 노드가 null입니다.");
            path = null;
            return;
        }
        path = AStarPathfinder.FindPath(start, goal);
        currentIndex = 0;
    }

    public void OnUsingR(GameObject target)
    {
        if (isUsingR) return; // 이미 사용 중이면 무시

        Debug.Log("unisg R");

        isUsingR = true;
        animator.SetTrigger("IsUsingR");
        returningFromR = false;
        rStartPos = transform.position;
        rTargetObject = target;
        rTargetPos = target.transform.position;

        rb.simulated = false; // 물리 끔
        col.isTrigger = true; // 충돌 무시
        path?.Clear();        // 기존 경로 중단
    }

    private void HandleRDash()
    {
        Vector2 target = returningFromR ? rStartPos : rTargetPos;
        float step = rSpeed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, target, step);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            if (!returningFromR)
            {
                Debug.Log("도착! 대상 함수 호출 예정");
                Enemy enemy = rTargetObject.GetComponent<Enemy>();
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    if (enemy.enemyPolymorphState is Enemy_PolymorphState polymorph)
                        polymorph.duration = DataManager.Instance.skillManager.skillDatas.skillDic["S0008"].CCTime;


                    enemy.StateMachine.ChangeState(enemy.enemyPolymorphState);
                }
                controller.stackController.UsingSkill();
                // ⚡ 나중에 여기에 target 함수 호출 추가
                // 예: rTargetObject.GetComponent<SomeComponent>()?.OnHitByMati();

                returningFromR = true;
            }
            else
            {
                Debug.Log("복귀 완료");
                isUsingR = false;
                rb.simulated = true;
                col.isTrigger = false; // ✅ 다시 충돌 정상 처리
                RequestPath();
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
            transform.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsingR && !returningFromR && other.gameObject == rTargetObject)
        {
            Debug.Log("트리거로 대상에 충돌!");
        }

        JumpPlatform jumpPlatform = other.GetComponent<JumpPlatform>();
        if (jumpPlatform != null)
        {
            jumpPadPower = jumpPlatform.jumpPower;
            isOnJumpPad = true;
            shouldJumpOnPad = false;  // 초기화
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<JumpPlatform>() != null)
        {
            isOnJumpPad = false;
            shouldJumpOnPad = false;  // 초기화
            rb.gravityScale = 1;  // 중력 복원
            rb.velocity = Vector2.zero;  // 속도 초기화
        }
    }

    private void CheckMoving()
    {
        if (lastPos != null)
        {
            Vector3 positionDifference = transform.position - lastPos;
            isMove = !Mathf.Approximately(positionDifference.magnitude, 0);
        }
        lastPos = transform.position;
    }

    private void SpriteFlip()
    {
        spriteRenderer.flipX = player.position.x - transform.position.x < 0;
    }

    private void AnimationUpdate()
    {
        animator.SetBool("IsMove", isMove);
        animator.SetBool("IsJump", isJumping);
        animator.SetBool("IsFall", isFalling);
    }

    //디버그용
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debugJumpTarget, 0.2f);
    }
}
