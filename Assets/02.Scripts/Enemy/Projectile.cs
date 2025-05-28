using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 180f;
    public LayerMask groundLayer;

    private Vector2 direction;

    public void Init(Vector2 dir, float time = 5f)
    {
        direction = dir.normalized;
        Destroy(gameObject, time);
    }

    void Update()
    {
        // 이동
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // 회전
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
