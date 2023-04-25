using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CompanionCharacter2D : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField]public float stoppingDistance;
    public float speed;
    private Transform target;
    Vector2 velocity = Vector2.zero;
    Rigidbody2D rb;
    bool faceRight = true;

    enum State
    {
        FOLLOW,
        CHASE,
        ATTACK
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Vector2 direction = Vector2.zero;


        if (Mathf.Abs(target.transform.position.x - transform.position.x) > stoppingDistance)
        {
            // Move towards the player position
            direction.x = Mathf.Sign(target.transform.position.x - transform.position.x );

        }
        // set velocity
        velocity.x = direction.x * speed;

        // Move character
        rb.velocity = velocity;

        // Flip character to face direction of movement (velocity
        if (velocity.x > 0 && !faceRight) Flip();
        if (velocity.x < 0 && faceRight) Flip();

        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
    }
    private void Flip()
    {
        faceRight = !faceRight;
        spriteRenderer.flipX = !faceRight;
    }

}

