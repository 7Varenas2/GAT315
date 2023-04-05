using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControllerCharacter : MonoBehaviour
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

	CharacterController characterController;
	Vector3 velocity = Vector3.zero;

	void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		// Check if the character is on the ground
		bool onGround = Physics.CheckSphere(groundTransform.position, 0.2f, groundLayerMask, QueryTriggerInteraction.Ignore);

		// get direction input
		Vector3 direction = Vector3.zero;
		direction.x = Input.GetAxis("Horizontal");
		direction.z = Input.GetAxis("Vertical");

		velocity.x = direction.x * speed;
		velocity.z = direction.z * speed;

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
        characterController.Move(velocity * Time.deltaTime);

		// Rotate character
		Vector3 face = new Vector3(velocity.x, 0, velocity.z);
		if (face.magnitude > 0)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(face), Time.deltaTime * turnRate);
		}

		// move character
		characterController.Move(velocity * Time.deltaTime);
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody body = hit.collider.attachedRigidbody;

		// no rigidbody
		if (body == null || body.isKinematic)
		{
			return;
		}

		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3)
		{
			return;
		}

		// Calculate push direction from move direction,
		// we only push objects to the sides never up and down
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

		// If you know how fast your character is trying to move,
		// then you can also multiply the push velocity by that.

		// Apply the push
		body.velocity = pushDir * hitForce;
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
