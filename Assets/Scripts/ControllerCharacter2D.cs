using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ControllerCharacter2D : MonoBehaviour
{
	
	[SerializeField] float speed;
	[SerializeField] float turnRate;
	[SerializeField] float jumpHeight;
	[SerializeField] float doubleJumpHeight;
	[SerializeField] float hitForce;
	[SerializeField, Range(1,5)] float fallRateMultiplier;
	[SerializeField, Range(1,5)] float lowJumpRateMultiplier;
	[Header("Ground")]
	[SerializeField] Transform groundTransform;
	[SerializeField] LayerMask groundLayerMask;

	Rigidbody2D rb;
	Vector2 velocity = Vector2.zero;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		// Check if the character is on the ground
		bool onGround = Physics.CheckSphere(groundTransform.position, 0.2f, groundLayerMask) != null;

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

		// Rotate character
		Vector2 face = new Vector2(velocity.x, 0);
		if (face.magnitude > 0)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(face), Time.deltaTime * turnRate);
		}

		// move character
		//rb.MovePosition(velocity * Time.deltaTime);
		rb.velocity = velocity;
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
				velocity.y += Mathf.Sqrt(doubleJumpHeight * -2 * Physics.clothGravity.y);
				
				break;

			}
			yield return null;
		}
	}
}
