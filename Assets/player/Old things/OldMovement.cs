using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMovement : MonoBehaviour
{
	[Header("References:")]
	[SerializeField] CharacterController characterController;

	[Header("Grounded Collider:")]
    [SerializeField] float groundSenseHeight;
    [SerializeField] float groundSenseRadius;
	[SerializeField] bool showGroundSenseCollider;
	[SerializeField] LayerMask groundMask;

	[Header("Other Basic Settings:")]
	[SerializeField] float gravity = -9.81f;
	[SerializeField] float acceleration = 1f;
	[SerializeField] float friction = 1f;
	[SerializeField] float jumpVelocity = 1f;


	Vector3 velocity;
	bool isGrounded = true;

	private void Update()
	{
		//apply velocity here so it is smooth
		characterController.Move(velocity * Time.deltaTime);
	}

	private void FixedUpdate()
	{
		//testing if on the ground
		isGrounded = Physics.CheckSphere(transform.position + Vector3.up * groundSenseHeight, groundSenseRadius, groundMask);

		//gravity
		if (!isGrounded)
		{
			velocity.y += gravity;
		}
		else
		{
			velocity.y = Mathf.Clamp(velocity.y, 0, Mathf.Abs(velocity.y));
		}

		//jumping
		if (isGrounded && Input.GetButton("Jump"))
		{
			velocity.y = jumpVelocity;
		}

		//horizontal movement
		if (isGrounded)
		{
			velocity += transform.right * Input.GetAxis("Horizontal") * acceleration + transform.forward * Input.GetAxis("Vertical") * acceleration;
		}

		//friction
		Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
		if (isGrounded)
		{
			velocity = horizontalVelocity * friction + Vector3.up * velocity.y;
		}
	}

	private void OnDrawGizmos()
	{
		//debug
		if (showGroundSenseCollider)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position + Vector3.up * groundSenseHeight, groundSenseRadius);
		}
	}
}
