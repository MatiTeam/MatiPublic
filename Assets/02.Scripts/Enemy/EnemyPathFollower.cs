using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFollower : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer; // 쫓아갈 대상 (플레이어)
    [SerializeField] private PlatformGraphBuilder graphBuilder;
    [SerializeField] private Enemy enemy;
    private Vector2 target;

    //자체 변수
    private float moveSpeed = 4f;
    private float jumpForce = 7f;
    private float nodeArrivalThreshold = 0.6f;

    //움직임에 필요한 변수
    private Rigidbody2D rb;
    private List<PlatformNode> path = new();
    private int currentIndex = 0;
    private float repathInterval = 0.2f;
    private float repathTimer = 0f;
    private float repathDistanceThreshold = 2f;

    private bool isJumping = false;
    private bool isFalling = false;

    private Vector2 jumpStart;
    private Vector2 jumpTarget;
    private float jumpDuration = 0.4f;
    private float jumpTimer = 0f;
    private float jumpHeight = 1.5f;

    private Vector2 fallStart;
    private Vector2 fallTarget;
    private float fallDuration = 0.5f;
    private float fallTimer = 0f;

    //생성 직후 타이머
    private float initialDelay = 2.0f;
    private float spawnTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        moveSpeed = enemy.moveSpeed;
        targetPlayer = enemy.player;
        graphBuilder = enemy.graphBuilder;
    }

    private void Update()
    {
        if (enemy.IsWalking == true && enemy.IsComeback == false)
        {
            target = targetPlayer.position;
        }
        else if (enemy.IsWalking == true && enemy.IsComeback == true)
        {
            target = enemy.origin;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer < initialDelay) return;

        repathTimer += Time.deltaTime;
        if (enemy.IsWalking == true && enemy.isDead == false)
        {
            if (repathTimer >= repathInterval && !isFalling && !isJumping)
            {
                float distToTarget = Vector2.Distance(transform.position, target);
                if (distToTarget > repathDistanceThreshold)
                {
                    RequestPath();
                }
                repathTimer = 0f;
            }
            FollowPath();
        }

    }

    private void FollowPath()
    {
        if (target == null) return;

        if (isJumping)
        {
            JumpUpdate();
            return;
        }

        if (isFalling)
        {
            FallUpdate();
            return;
        }

        if (path == null || path.Count == 0)
        {
            // Debug.Log("패스 없음 오류");
            return;
        }

        if (currentIndex >= path.Count)
        {
            // Debug.Log("도착");
            path.Clear();
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 targetPos = path[currentIndex].Position;
        Vector2 direction = targetPos - (Vector2)transform.position;
        float dist = direction.magnitude;

        if (dist < nodeArrivalThreshold && !isFalling && !isJumping)
        {
            // Debug.Log("도착2");
            currentIndex++;
            return;
        }

        if ((direction.y > 0.5f || Mathf.Abs(direction.x) > 2.5f) && direction.y < 5f && !isJumping && !isFalling)
        {
            Debug.Log("점프 시작");
            JumpStart(targetPos);
            return;
        }

        if (direction.y < -0.5f && !isFalling && !isJumping)
        {
            Debug.Log("떨어지기 시작");
            FallStart(targetPos);
            return;
        }

        if (Mathf.Abs(direction.y) < 0.5f)
        {
            // Debug.Log("적 걷기");
            float moveDir = Mathf.Sign(direction.x);
            rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
            return;
        }

        // Debug.Log("아무것도 안함");
        return;
    }

    private void JumpStart(Vector2 targetPos)
    {
        jumpStart = transform.position;
        jumpTarget = targetPos;
        jumpTimer = 0f;
        isJumping = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        enemy.footTransform.gameObject.SetActive(false);
    }

    private void JumpUpdate()
    {
        // Debug.Log("적 점프");
        jumpTimer += Time.deltaTime;
        float t = jumpTimer / jumpDuration;
        float FixedJumpHeight = jumpHeight;
        // Debug.Log($"점프 중 : {t}");

        // 새로운 타겟 좌표 확인
        Vector2 newTarget = path[currentIndex].Position;

        // ✅ jumpTarget이 변경됐으면, 현재 위치를 jumpStart로 새로 잡고 타겟 갱신
        if (jumpTarget != newTarget)
        {
            jumpStart = transform.position;  // 현재 위치를 새 시작점
            jumpTarget = newTarget;          // 타겟 갱신
            FixedJumpHeight = 0;
        }

        Vector2 mid = Vector2.Lerp(jumpStart, jumpTarget, t);
        mid.y += FixedJumpHeight * Mathf.Sin(Mathf.PI * t);
        transform.position = mid;

        if (t >= 1f)
        {
            isJumping = false;

            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 물리 복원
            rb.gravityScale = 3;

            currentIndex++;
            enemy.footTransform.gameObject.SetActive(true);
            Debug.Log("점프 끝");
        }
    }

    private void FallStart(Vector2 targetPos)
    {
        fallStart = transform.position;
        fallTarget = targetPos;
        fallTimer = 0f;
        isFalling = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        enemy.footTransform.gameObject.SetActive(false);
    }

    private void FallUpdate()
    {
        // Debug.Log("적 떨어짐");
        fallTimer += Time.deltaTime;
        float t = fallTimer / fallDuration;
        Vector2 mid = Vector2.Lerp(fallStart, fallTarget, t);
        mid.y -= 0.5f * Mathf.Sin(Mathf.PI * t);
        transform.position = mid;

        if (t >= 1f)
        {
            isFalling = false;

            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 물리 복원
            rb.gravityScale = 3;

            currentIndex++;
            enemy.footTransform.gameObject.SetActive(true);
        }
    }

    public void RequestPath()
    {
        PlatformNode start = graphBuilder.FindClosestNodeTo(transform.position);
        PlatformNode goal = graphBuilder.FindClosestNodeTo(target);
        // if (start == null)
        //     Debug.Log("start 문제");
        // if (goal == null)
        //     Debug.Log("player 문제");
        if (start == null || goal == null)
        {
            path = null;
            return;
        }
        path = AStarPathfinder.FindPath(start, goal);
        currentIndex = 0;
    }
}
