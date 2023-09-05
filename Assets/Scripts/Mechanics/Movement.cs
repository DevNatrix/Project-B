using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	//input
	[HideInInspector] public Vector2 horizontalInput;

	//controlled by server:
	[HideInInspector] public float acceleration = 11f;
	[HideInInspector] public float inAirAcceleration = 1f;
	[HideInInspector] public float decceleration = .95f;
	[HideInInspector] public float inAirDecceleration = .03f;
	[HideInInspector] public float jumpPower = 3.5f;
	[HideInInspector] public float maxSpeed;
	[HideInInspector] public float minYPos = -10f;
	[HideInInspector] public float addedGravity;
	[HideInInspector] public float dashSeconds;
	[HideInInspector] public float dashSpeed;
	[HideInInspector] public float dashCooldown;
	[HideInInspector] public float slidingDecceleration;
	[HideInInspector] public float speedBoostOnSlide;

	[Header("References:")]
	[SerializeField] Rigidbody playerRB;
	[SerializeField] GameObject playerCam;
	[SerializeField] Look look;
	[SerializeField] PlayerManager playerManager;

	[Header("Basic Movement Settings:")]
	[HideInInspector] public bool jump;
	[SerializeField] float jumpSpeed;
	int jumps = 0;
	bool isGrounded;
	[SerializeField] float groundDistance = 1f;


	[Header("Advanced Movement Settings:")]
	public LayerMask groundMask;
	[SerializeField] float groundCheckRadius;
	[SerializeField] float groundCheckHeight;
	[SerializeField] float stairFriction = .8f;

	[Header("Wallrunning settings:")]
	[SerializeField] float wallDetectionHeight;
	[SerializeField] float wallDetectionRadius;
	bool wallRunning = false;
	bool nearbyWall;
	[Range(4, 360)]
	[SerializeField] int nearestPointSubs = 5;
	float closestAngle = 0;
	[SerializeField] float stickToWallForce = 1f;
	[SerializeField] float FOVIncreaseOnWallride = 10f;
	[SerializeField] float minVelocityForWallrun;
	[SerializeField] float wallRunningTilt = 10f;
	[SerializeField] float wallRunningGravity;
	[SerializeField] float wallRunningAcceleration;
	[SerializeField] float wallRunningDecceleration;

	[Header("Walljumping settings:")]
	[SerializeField] float secondsBetweenWallJumps = 1f;
	[SerializeField] float wallJumpingTimer = 0f;
	[SerializeField] float wallJumpForceAway;
	[SerializeField] float wallJumpForceUp;
	[SerializeField] float wallJumpForceCam;

	[Header("Dash Settings:")]
	float dashTimer = 0f;

	[Header("Slide Settings:")]
	[HideInInspector] public bool slidingRequested = false;
	[SerializeField] float slidingLean = 10;
	[SerializeField] float slidingHeightChange = .5f;
	[SerializeField] float slidingFOVChange;
	bool sliding = false;
	public static bool crouching = false;

	[Header("Audio Settings")]
	[SerializeField] AudioPlayer audioPlayer;
	[SerializeField] float distanceForFootstep;
	[SerializeField] AudioClip footstepClip;
	[SerializeField] float footstepVolume;
	Vector3 pastStepPosition;

	[Header("Debug")]
	[SerializeField] bool showClosestWallCasts = false;
	[SerializeField] bool showGroundCheck = false;
	[SerializeField] bool showWallRadius = false;
	[HideInInspector] public bool jumping = false;

	public void ReceiveInput(Vector2 _horizontalInput)
	{
		horizontalInput = _horizontalInput;
	}

	private void Update()
	{
		//kill if falling
		if (transform.position.y < minYPos)
		{
			playerManager.respawn();
		}

        //wallrunning
        float horizontalSpeed = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z).magnitude;
		nearbyWall = false;// Physics.CheckSphere(transform.position + Vector3.up * wallDetectionHeight, wallDetectionRadius, groundMask);
		wallJumpingTimer -= Time.deltaTime;
		if (nearbyWall && horizontalSpeed >= minVelocityForWallrun && jumping)
		{
			//start wall running
			if (!wallRunning)
			{
				look.targetFOV += FOVIncreaseOnWallride;
				wallRunning = true;
				playerRB.useGravity = false;

				//float speed = playerRB.velocity.magnitude;
				//playerRB.velocity = playerCam.transform.forward * speed;
			}
			playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

			//lean towards the closest wall - this code is disgusting but idk how to make it better
			RaycastHit hit1;
			RaycastHit hit2;
			float lean = 0f;
			if(Physics.Raycast(transform.position + Vector3.up * wallDetectionHeight, transform.right, out hit1, wallDetectionRadius))
			{
				lean = wallRunningTilt;
			}
			if (Physics.Raycast(transform.position + Vector3.up * wallDetectionHeight, -transform.right, out hit2, wallDetectionRadius))
			{
				if(lean != 0 && hit1.distance < hit2.distance)
				{
					lean = -wallRunningTilt;
				}
				else if(lean == 0)
				{
					lean = -wallRunningTilt;
				}
			}
			look.targetCamRoll = lean;

			//stick to wall
			closestAngle = closestWallAngle();
			playerRB.AddForce(Quaternion.Euler(0, closestAngle, 0) * Vector3.forward * stickToWallForce * Time.deltaTime);
		}
		else if(wallRunning)
		{
			if (!jumping && wallJumpingTimer <= 0)
			{
				playerRB.AddForce(Quaternion.Euler(0, closestAngle, 0) * -Vector3.forward * wallJumpForceAway + Vector3.up * wallJumpForceUp);
				playerRB.AddForce(playerCam.transform.forward * wallJumpForceCam);
				wallJumpingTimer = secondsBetweenWallJumps;
			}
			look.targetFOV -= FOVIncreaseOnWallride;
			wallRunning = false;
			playerRB.useGravity = true;
			look.targetCamRoll = 0;
		}

		//dash
		dashTimer -= Time.deltaTime;
	}

	public IEnumerator dash()
	{
		Vector2 direction = horizontalInput;
		if (dashTimer <= 0 && !MenuController.movementLocked && Upgrades.instance.isUpgradeUnlocked("Dash"))
		{
			dashTimer = dashCooldown + dashSeconds;
			float timer = dashSeconds;
			while(timer > 0)
			{
				timer -= Time.deltaTime;
				//wall detection
				if (!Physics.Raycast(playerRB.position + Vector3.up * wallDetectionHeight, playerRB.transform.forward * direction.y + playerRB.transform.right * direction.x, wallDetectionRadius))
				{
					playerRB.MovePosition(playerRB.position + playerRB.transform.forward * direction.y * dashSpeed * Time.deltaTime + playerRB.transform.right * direction.x * dashSpeed * Time.deltaTime);
				}
				yield return new WaitForEndOfFrame();
			}
		}
		yield return null;
	}

	private void FixedUpdate()
	{
		//ground check
		//isGrounded = Physics.CheckSphere(transform.position + Vector3.up * groundCheckHeight, groundCheckRadius, groundMask);
		RaycastHit hit;
		isGrounded = Physics.Raycast(transform.position + Vector3.up * groundCheckHeight, -Vector3.up, out hit, groundDistance, groundMask);
		
		if(isGrounded && hit.collider.gameObject.layer == 18) //if on stairs
		{
			playerRB.useGravity = false;
			playerRB.velocity = playerRB.velocity * stairFriction;
		}
		else
		{
			playerRB.useGravity = true;
		}

		//sliding
		crouching = slidingRequested;
		bool newSliding = !wallRunning && isGrounded && slidingRequested;
		if(!sliding && newSliding) //start
		{
			//redirect velocity
			float speed = playerRB.velocity.magnitude + speedBoostOnSlide;
			Vector3 direction = playerCam.transform.forward;
			direction.y = 0f;
			direction.Normalize();
			playerRB.velocity = direction * speed;

            look.targetFOV += slidingFOVChange;
            look.camHeightOffset += slidingHeightChange;
			look.targetXRotOffset += slidingLean;
        }
		else if(sliding && !newSliding) //stop
		{
			look.targetFOV -= slidingFOVChange;
            look.camHeightOffset -= slidingHeightChange;
            look.targetXRotOffset -= slidingLean;
        }
		sliding = newSliding;

		//movement
		if (!MenuController.movementLocked)
		{
			if (isGrounded)
			{
				//footsteps
				if(Vector3.Distance(pastStepPosition, transform.position) >= distanceForFootstep)
				{
					pastStepPosition = transform.position;
					audioPlayer.createAudio(footstepClip, transform.position, footstepVolume, 1, Client.ID);
				}

				if (!sliding)
				{
					float speedMult = 1 + Upgrades.instance.speedMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Speed");
					playerRB.AddForce((transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * acceleration * speedMult);
				}
			}
			else if (wallRunning)
			{
				playerRB.AddForce((transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * wallRunningAcceleration);
				playerRB.velocity += -Vector3.up * wallRunningGravity;
			}
			else
			{
				playerRB.velocity += -Vector3.up * addedGravity;
				playerRB.AddForce((transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * inAirAcceleration);
			}
		}

		//jumping
		if(isGrounded && playerRB.velocity.y < 1)
		{
			jumps = 1 + Upgrades.instance.getUpgradeLevel("Extra Jump");
		}

		if(jump && jumps > 0 && !MenuController.movementLocked)
		{
			//playerRB.AddForce(Vector3.up * jumpPower);// += new Vector3(0f, jumpPower, 0f);
			playerRB.velocity = new Vector3(playerRB.velocity.x, jumpSpeed, playerRB.velocity.z);
			jumps--;
		}
		jump = false;

		//friction (not vertical)
		Vector3 velocity = playerRB.velocity;
		Vector3 yVelocity = Vector3.up * velocity.y;
		velocity.y = 0f;
		if(isGrounded )
		{
			if(sliding)
			{
                playerRB.velocity = velocity * slidingDecceleration + yVelocity;
            }
			else
			{
				playerRB.velocity = velocity * decceleration + yVelocity;
			}
		}
		else if (wallRunning)
		{
			playerRB.velocity = velocity * wallRunningDecceleration + yVelocity;
		}
		else
		{
			playerRB.velocity = velocity * inAirDecceleration + yVelocity;
		}

        //limit speed
        float horizontalSpeed = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z).magnitude;
        if (horizontalSpeed > maxSpeed)
        {
            playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z).normalized * maxSpeed + new Vector3(0f, playerRB.velocity.y, 0f);
        }
    }

	private float closestWallAngle()
	{
		float anglePerSub = 360 / (float)nearestPointSubs;
		float minDist = wallDetectionRadius + 1;
		int closestWallAngle = 0;

		RaycastHit hit;
		for (int sub = 0; sub < nearestPointSubs; sub++)
		{
			if (Physics.Raycast(transform.position + Vector3.up * wallDetectionHeight, Quaternion.Euler(0, sub * anglePerSub, 0) * Vector3.forward, out hit, wallDetectionRadius))
			{
				if (showClosestWallCasts)
				{
					Debug.DrawRay(transform.position + Vector3.up * wallDetectionHeight, Quaternion.Euler(0, sub * anglePerSub, 0) * Vector3.forward * wallDetectionRadius, Color.green);
				}
				//print(hit.collider.gameObject.name);
				if (hit.distance < minDist)
				{
					closestWallAngle = sub;
					minDist = hit.distance;
				}
			}
			else
			{
				if (showClosestWallCasts) {
					Debug.DrawRay(transform.position + Vector3.up * wallDetectionHeight, Quaternion.Euler(0, sub * anglePerSub, 0) * Vector3.forward * wallDetectionRadius, Color.red);
				}
			}
		}
		if (showClosestWallCasts)
		{
			Debug.DrawRay(transform.position + Vector3.up * wallDetectionHeight, Quaternion.Euler(0, closestWallAngle * anglePerSub, 0) * Vector3.forward * wallDetectionRadius, Color.blue);
		}
		return (float) closestWallAngle * anglePerSub;
	}

	private void OnDrawGizmos()
	{
		if (showGroundCheck)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position + Vector3.up * groundCheckHeight, groundCheckRadius);
		}
		if (showWallRadius)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position + Vector3.up * wallDetectionHeight, wallDetectionRadius);
		}
	}
}
	