using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AICharacter2D : MonoBehaviour
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
    [SerializeField] Transform[] waypoints;

    Rigidbody2D rb;
    Vector2 velocity = Vector2.zero;

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
    Vector2 direction = Vector2.zero;

    void Update()
    {
        // Update AI

        switch (state)
        {
            case State.IDLE:
                stateTimer += Time.deltaTime;
                if (stateTimer > 2)
                {

                    SetNewWayPointTarget();
                    state = State.PATROL;
                }

                break;
            case State.ATTACK:
                break;
            case State.CHASE:
                break;
            case State.PATROL:
                if (CanSeePlayer()) state = State.CHASE;

                direction.x = Mathf.Sign(targetWaypoint.position.x - transform.position.x );
                Physics2D.Raycast(transform.position, Vector2.right * direction.x * rayDistance);
                Debug.DrawRay(transform.position, Vector2.right * direction.x * rayDistance);
               
                float dx = Mathf.Abs(targetWaypoint.position.x - transform.position.x);
                if (dx <= 0.25f)
                {
                    state = State.IDLE;
                    stateTimer = 0;
                }

                break;
        }

        // Check if the character is on the ground
        bool onGround = Physics2D.OverlapCircle(groundTransform.position, groundRadius, groundLayerMask) != null;

        // get direction input
        direction.x = Input.GetAxis("Horizontal");
        // set velocity
        velocity.x = direction.x * speed;

        if (onGround)
        {

            if (velocity.y < 0) velocity.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
                StartCoroutine(DoubleJump());
                animator.SetTrigger("Jump");
            }
        }
        velocity.y += Physics.gravity.y * Time.deltaTime;


        // Adjust gravity for jump
        // Multiplier defaults to 1
        // if not on ground and it is moving downward (velocity y < 0) set the multiplier to the fall rate multiplier
        // Multiply the current physics gravity with the muliplier
        float gravityMultiplier = 1;
        if (!onGround && velocity.y < 0) gravityMultiplier = fallRateMultiplier;
        if (!onGround && velocity.y > 0 && !Input.GetButton("Jump")) gravityMultiplier = lowJumpRateMultiplier;
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // Move character
        //rb.Move(velocity * Time.deltaTime);
        rb.velocity = velocity;

        // Flip character to face direction of movement (velocity
        if (velocity.x > 0 && !faceRight) Flip();
        if (velocity.x < 0 && faceRight) Flip();

        // Update animator
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        animator.SetBool("Fall", !onGround && velocity.y < -0.1f);

    }

    IEnumerator DoubleJump()
    {
        // Wait a little afterhte jump to allow adouble jump
        yield return new WaitForSeconds(0.1f);
        // Allow a double jump while moving up
        while (velocity.y > 0)
        {
            // If "jump" preseed add the velocity
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += Mathf.Sqrt(doubleJumpHeight * -2 * Physics.gravity.y);

                break;

            }
            yield return null;
        }
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

    private bool CanSeePlayer()
    {
       RaycastHit2D raycastHit =  Physics2D.Raycast(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance);
        Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance);

        return raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag("Player");
    }
}

