using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
	bool scaledUp;
	public Rigidbody Player;
	public GameObject MissilePrototype;
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
	float lastJumpTimeSec = -2;

	// For screen rendering updates. About 30/s
	// Check for input here.
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (transform.position.y < 3.1 && Time.time - lastJumpTimeSec > 0.2)
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

	// For physics engine updates. About 100/s
	// Apply forces here.
	void FixedUpdate()
	{
		// new Vector3(0, 0, 10)
		Player.AddForce(transform.forward * 2);
		if (canJump)
		{
			Player.AddForce(transform.up * 500);
			canJump = false;
		}

		if (canTurnLeft)
		{
			//Player.AddTorque(new Vector3(0, SidewaysTorque, 0));
			Player.AddForce(transform.right * SidewaysForce);
			canTurnLeft = false;
		}

		if (canTurnRight)
		{
			//Player.AddTorque(new Vector3(0, -SidewaysTorque, 0));
			Player.AddForce(transform.right * -SidewaysForce);
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
	}

	public void PowerUp()
	{
		if (scaledUp)
			return;
		scaledUp = true;
		const float multiplier = 2f;
		transform.localScale = new Vector3(transform.localScale.x * multiplier, transform.localScale.y * multiplier, transform.localScale.z * multiplier);
		transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y * multiplier / 2, transform.position.z);
	}
}