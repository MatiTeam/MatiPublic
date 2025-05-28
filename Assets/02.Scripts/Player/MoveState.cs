using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IState
{
    NewPlayerContorller player;
    public MoveState(NewPlayerContorller player)
    {
        this.player = player;
    }

    public void Enter()
    {
        if (player.IsOnMovingPlatform() && player.isOnMovingPlatformClick == false)
        {
            player.stateMachine.ChangeState(player.IdleState);
        }
        //애니메이션 재생
        player.animator.SetBool("IsMove", true);
    }

    public void Exit()
    {
        player.animator.SetBool("IsMove", false);
        player.isOnMovingPlatformClick = false;
    }


    void IState.Update(Vector2 mousePos)
    {
        float distance = player.targetX - player.transform.position.x;
        float direction = Mathf.Sign(distance);
        float nextMove = direction * player.moveSpeed * Time.fixedDeltaTime;

        // 앞으로 이동했을 때 targetX를 넘어갈 것 같으면 딱 거기서 멈춤
        if (Mathf.Abs(distance) > 0.05f)
        {
            if (Mathf.Abs(distance) <= Mathf.Abs(nextMove))
            {
                // targetX 도달 or 지나칠 예정 → 위치 조정 + 정지
                player.transform.position = new Vector2(player.targetX, player.transform.position.y);
                player.rb.velocity = new Vector2(0, player.rb.velocity.y);
            }
            else
            {
                player.rb.velocity = new Vector2(direction * player.moveSpeed, player.rb.velocity.y);
            }
        }
        else
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y);
            player.stateMachine.ChangeState(player.IdleState);
        }

        //벽에 달라붙지 못하게 하기 위한 기능
        //if (!player.IsGrounded())
        //{
        Vector2 origin;
        bool isGroundLeft;
        bool isGroundRight;
        LayerMask combineLayer = player.groundLayer | player.obstacleLayer |player.globalLayer;

        for (int i = 0; i < 5; i++)
        {
            origin = new Vector2(player.transform.position.x, player.playerCollider.bounds.min.y + (player.playerCollider.size.y / 4 * i));
            RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.left, 0.460f, combineLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, 0.460f, combineLayer);

            isGroundLeft = hitLeft.collider != null;
            isGroundRight = hitRight.collider != null;


            if (isGroundLeft || isGroundRight)
            {
                if (isGroundLeft)
                {
                    player.targetX = Mathf.Max(player.transform.position.x, player.targetX);
                }
                if (isGroundRight)
                {
                    player.targetX = Mathf.Min(player.transform.position.x, player.targetX);
                }
                break;
            }


            Debug.DrawRay(origin, Vector2.left * 0.460f, isGroundLeft ? Color.red : Color.green);
            Debug.DrawRay(origin, Vector2.right * 0.460f, isGroundRight ? Color.red : Color.green);
        }
        //}
    }
}
