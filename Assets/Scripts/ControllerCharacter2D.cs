using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ControllerCharacter2D : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] float speed;
	[SerializeField] float jumpHeight;
	[SerializeField] float doubleJumpHeight;
	[SerializeField, Range(1,5)] float fallRateMultiplier;
	[SerializeField, Range(1,5)] float lowJumpRateMultiplier;
	[Header("Ground")]
	[SerializeField] Transform groundTransform;
	[SerializeField] LayerMask groundLayerMask;
	[SerializeField] float groundRadius;
	[Header("Attack")]
	[SerializeField] Transform attackTransform;
	[SerializeField] float attackRadius;

	Rigidbody2D rb;
	Vector2 velocity = Vector2.zero;
	float groundAngle = 0;
	
	bool faceRight = true;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		//spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		
		// Check if the character is on the ground
		bool onGround = UpdateGroundCheck() && (rb.velocity.y <= 0);

		// get direction input
		Vector2 direction = Vector2.zero;
		direction.x = Input.GetAxis("Horizontal");
		

		velocity.x = direction.x * speed;
		

		// set velocity
		if (onGround)
		{

			if (velocity.y < 0) velocity.y = 0;
			if (Input.GetButtonDown("Jump"))
			{
				velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
				StartCoroutine(DoubleJump());
				animator.SetTrigger("Jump");
			}

			if (Input.GetMouseButtonDown(0))
			{
				animator.SetTrigger("Attack");
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
		rb.velocity= velocity;

		// Flip character to face direction of movement (velocity
		if (velocity.x > 0 && !faceRight) Flip();
		if (velocity.x < 0 && faceRight) Flip();

		// Update animator
		animator.SetFloat("Speed", Mathf.Abs(velocity.x));
		animator.SetBool("Fall", !onGround && velocity.y < -0.1f);
		
	}

    private bool UpdateGroundCheck()
    {
        // check if the character is on the ground
        Collider2D collider = Physics2D.OverlapCircle(groundTransform.position, groundRadius, groundLayerMask);
        if (collider != null)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(groundTransform.position, Vector2.down, groundRadius, groundLayerMask);
            if (raycastHit.collider != null)
            {
                // get the angle of the ground (angle between up vector and ground normal)
                groundAngle = Vector2.SignedAngle(Vector2.up, raycastHit.normal);
                Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.red);
            }
        }

        return (collider != null);
    }

    IEnumerator DoubleJump ()
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

    private void CheckAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackTransform.position, attackRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject) continue;

            if (collider.gameObject.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.Damage(10);
            }
        }
    }

}
