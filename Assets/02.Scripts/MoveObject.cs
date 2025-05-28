using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float moveSpeed;     // 물체 이동 속도
    private float stopDistance = 1f;
    [SerializeField] private bool isMati = false;
    private bool isMoving = true;
    private Vector3 targetPosition;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        targetPosition = new Vector3(transform.position.x + 7f, transform.position.y, transform.position.z);

    }

    void Update()
    {
        if(!isMoving) return;

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if(isMati && animator != null)
            {
                animator.SetBool("IsMoving", true);
            }

            if(Vector3.Distance(transform.position, targetPosition) <= stopDistance)
            {
                isMoving = false;

                if(isMati && animator != null)
                {
                    animator.SetBool("IsMoving", false);
                }
            }
        }
          
    }
}