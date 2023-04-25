using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AICharacter2D : MonoBehaviour, IDamagable
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float speed;
    [SerializeField] float jumpHeight;
    [SerializeField] float doubleJumpHeight;
    [SerializeField, Range(1, 5)] float fallRateMultiplier;
    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;
    [Header("Ground")]
    [SerializeField] Transform groundTransform;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundRadius;
    [Header("AI")]
    [SerializeField] float rayDistance = 1;
    [SerializeField] string enemyTag;
    [SerializeField] LayerMask raycastLayerMask;
    [SerializeField] Transform[] waypoints;

    public float health = 100;
    Rigidbody2D rb;
    Vector2 velocity = Vector2.zero;
    GameObject player;

    bool faceRight = true;
    float groundAngle = 0;
    Transform targetWaypoint = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    enum State
    {
        IDLE,
        ATTACK,
        PATROL,
        CHASE
    }

    State state = State.IDLE;
    float stateTimer = 0;

    void Update()
    {
        // Check if enemy is there/detected
        CheckEnemySeen();

        Vector2 direction = Vector2.zero;
        // Update AI

        switch (state)
        {
            case State.IDLE:
                if (player != null) state = State.CHASE;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {

                    SetNewWayPointTarget();
                    state = State.PATROL;
                }

                break;
            case State.PATROL:
                {
                    if (player != null) state = State.CHASE;

                    direction.x = Mathf.Sign(targetWaypoint.position.x - transform.position.x);
                    Physics2D.Raycast(transform.position, Vector2.right * direction.x * rayDistance);
                    Debug.DrawRay(transform.position, Vector2.right * direction.x * rayDistance);

                    float dx = Mathf.Abs(targetWaypoint.position.x - transform.position.x);
                    if (dx <= 0.25f)
                    {
                        state = State.IDLE;
                        stateTimer = 1;
                    }

                    break;
                }
            case State.ATTACK:
                // iF enemy out of reach go after enemy
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                {
                    state = State.CHASE;
                }
                // if enemy is not seen
                if (player == null)
                {
                    // Set enemy null and set it to idle
                    player = null;
                    state = State.IDLE;
                    break;
                }

                break;
            case State.CHASE:
                {
                    // If enemy not detected go to null
                    if (player == null)
                    {
                        state = State.IDLE;
                        stateTimer = 1;
                        break;
                    }
                    // Get distance and direction, if it is within range attack, else begin chase
                    float dx = Mathf.Abs(player.transform.position.x - transform.position.x);
                    if (dx <= 1f)
                    {
                        state = State.ATTACK;
                        animator.SetTrigger("Attack");
                    }
                    else
                    {
                        direction.x = Mathf.Sign(player.transform.position.x - transform.position.x);
                    }
                    break;
                }

        }

        // Check if the character is on the ground
        bool onGround = Physics2D.OverlapCircle(groundTransform.position, groundRadius, groundLayerMask) != null;

        // get direction input

        // set velocity
        velocity.x = direction.x * speed;

        // Move character
        rb.velocity = velocity;

        // Flip character to face direction of movement (velocity
        if (velocity.x > 0 && !faceRight) Flip();
        if (velocity.x < 0 && faceRight) Flip();

        // Update animator
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        //animator.SetBool("Fall", !onGround && velocity.y < -0.1f);

    }


    private void Flip()
    {
        faceRight = !faceRight;
        spriteRenderer.flipX = !faceRight;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundTransform.position, groundRadius);
    }

    private void SetNewWayPointTarget()
    {
        Transform waypoint = null;
        do
        {
            waypoint = waypoints[Random.Range(0, waypoints.Length)];

        } while (waypoint == targetWaypoint);
        targetWaypoint = waypoint;

    }

    private void CheckEnemySeen()
    {
        player = null;

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, ((faceRight) ? Vector2.right : Vector2.left), rayDistance, raycastLayerMask);
        // if raycast detects enemy and is not null 
        if (raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag(enemyTag))
        {
            // Store to enemy game object
            player = raycastHit.collider.gameObject;
            // If raycast is hit turn color red
            Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance, Color.red);
        }
    }

    public void Damage(int damage)
    {
       health -= damage;
        print(health);
        if (health <= 0)
        {
            animator.SetTrigger("Death");

            Destroy(gameObject, 0.65f);

        }
    }

}

