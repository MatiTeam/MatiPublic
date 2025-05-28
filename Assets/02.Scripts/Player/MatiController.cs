using TMPro;
using UnityEngine;

public class MatiController : MonoBehaviour
{
    public Vector2 offsetRight = new Vector2(1f, -0.25f);
    public Vector2 offsetLeft = new Vector2(-1f, -0.25f);

    [SerializeField] private Transform player; // 따라갈 대상
    private float followSpeed = 10f; // 따라가는 속도
    private float maxDistance = 2f; // 이 거리보다 멀어지면 따라감

    [SerializeField] private LayerMask groundLayer;

    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true; // 현재 바라보는 방향(기본적으로 오른쪽)


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // GameManager.Instance.dialogueManager.RegisterSpeaker(data.CharacterID, this);
    }

    private void LateUpdate()
    {
        if (player == null) return;
        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);
        bool playerFacingRight = player.localScale.x > 0; // 플레이어의 방향 확인

        if (playerFacingRight != isFacingRight)
        {
            // 플레이어의 방향과 마티의 방향이 다르면 방향 전환
            Flip();
        }
        if (distance > maxDistance)
        {
            // 플레이어와의 거리가 maxDistance보다 멀어지면 따라감
            FollowPlayer();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight; // 방향 전환
        spriteRenderer.flipX = !spriteRenderer.flipX; // 스프라이트 방향 전환
    }

    private void FollowPlayer()
    {
        // 플레이어 x 좌표 기준 이동 목표 설정
        float targetX = player.position.x + (isFacingRight ? offsetRight.x : offsetLeft.x);
        Vector2 rayOrigin = new Vector2(targetX, player.position.y + 5f); // 위에서 아래로 Raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 10f, groundLayer);

        if (hit.collider != null)
        {
            float groundY = hit.point.y;
            Vector2 targetPosition = new Vector2(targetX, groundY + (isFacingRight ? offsetRight.y : offsetLeft.y));
            transform.position = Vector2.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
