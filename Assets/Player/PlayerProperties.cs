using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerProperties : MonoBehaviour
{
	bool scaledUp;

	public Rigidbody Player;

	public GameObject MissilePrototype;

	public Transform BlackHole;
	public Transform Track;

	SmoothMover scaleAnimation;
	// Start is called before the first frame update
	void Start()
	{

	}

	// ![](8AE0076460CA94E7503B247B8104AE13.png)
	// Variables are like a box or a container. They hold whatever you put within them.
	// They can have really cool names. Like this one:
	bool canJump;
	bool canTurnLeft;
	bool canFireMissile;
	bool canTurnRight;
	Vector3 lastPosition;
	// Variables can also be of different types. The variable above is a bool, which means true or false.

	public float SidewaysTorque = 50;
	public float SidewaysForce = 5;
	float powerUpForceMultiplier = 1;
	float powerUpScaleMultiplier = 1;
	public float ForwardForce = 2;
	public float PowerUpDuration = 3;
	float powerUpEndTime;
	float lastJumpTimeSec = -2;

	// For screen rendering updates. About 30/s
	// Check for input here.
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (IsNearGround() && Time.time - lastJumpTimeSec > 0.2)
			{
				lastJumpTimeSec = Time.time;
				canJump = true;
			}
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			canTurnLeft = true;
		}

		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
		{
			canFireMissile = true;
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			canTurnRight = true;
		}
	}

	private bool IsNearGround()
	{
		return transform.position.y <= Track.position.y + 0.5 + powerUpScaleMultiplier * 2.5;
	}

	float yPositionBeforeScaleAnimation;

	// For physics engine updates. About 100/s
	// Apply forces here.
	void FixedUpdate()
	{
		// new Vector3(0, 0, 10)
		Player.AddForce(transform.forward * ForwardForce * powerUpForceMultiplier);
		if (canJump)
		{
			Player.AddForce(transform.up * 500 * powerUpForceMultiplier);
			canJump = false;
		}

		if (canTurnLeft)
		{
			//Player.AddTorque(new Vector3(0, SidewaysTorque, 0));
			Player.AddForce(transform.right * SidewaysForce * powerUpForceMultiplier);
			canTurnLeft = false;
		}

		if (canTurnRight)
		{
			//Player.AddTorque(new Vector3(0, -SidewaysTorque, 0));
			Player.AddForce(transform.right * -SidewaysForce * powerUpForceMultiplier);
			canTurnRight = false;
		}

		if (canFireMissile)
		{
			canFireMissile = false;
			Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
			GameObject missile = Instantiate(MissilePrototype, position, new Quaternion(0, 90, 90, 0));
			const float missileSpeed = 750f;
			float playerSpeed = (transform.position - lastPosition).magnitude;
			//Debug.Log(playerSpeed);
			missile.GetComponent<Rigidbody>().AddForce(transform.forward * (missileSpeed + playerSpeed * 700));
			missile.GetComponent<MissileBehavior>().flying = true;
			lastPosition = transform.position;
		}

		if (scaledUp && Time.time > powerUpEndTime)
			PowerDown();

		if (scaleAnimation != null)
		{
			ScaleTo(scaleAnimation.Value, yPositionBeforeScaleAnimation);
			if (scaleAnimation.HasEnded)
				scaleAnimation = null;
		}
	}

	private void PowerDown()
	{
		powerUpForceMultiplier = 1;
		GetComponent<Rigidbody>().mass = 1;
		AnimateScaleTo(1);
		scaledUp = false;
	}

	private void AnimateScaleTo(float newScaleMultiplier)
	{
		powerUpScaleMultiplier = newScaleMultiplier;
		scaleAnimation = new SmoothMover(0.5f, transform.localScale.x, powerUpScaleMultiplier);
		yPositionBeforeScaleAnimation = transform.position.y;
	}

	public void PowerUp()
	{
		if (scaledUp)
			return;
		scaledUp = true;
		AnimateScaleTo(2f);
		powerUpForceMultiplier = 100;
		GetComponent<Rigidbody>().mass = powerUpForceMultiplier;
		powerUpEndTime = Time.time + PowerUpDuration;
	}

	private void ScaleTo(float newScale, float savedY)
	{
		transform.localScale = new Vector3(newScale, newScale, newScale);
		transform.position = new Vector3(transform.position.x, savedY + newScale / 2, transform.position.z);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!scaledUp)
			return;
		if (ObstacleLogic.IsObstacle(collision.gameObject))
			GameLogic.RandomBlowUp(collision.gameObject, BlackHole);

		//if (collision.gameObject.tag == "Track")
		//	GameLogic.PlayerHitTrack();

	}
}
