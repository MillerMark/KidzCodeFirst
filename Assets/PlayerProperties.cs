using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
	public Rigidbody Player;
	public GameObject MissilePrototype;

	Vector3 lastPosition;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// ![](8AE0076460CA94E7503B247B8104AE13.png)
	// Variables are like a box or a container. They hold whatever you put within them.
	// They can have really cool names. Like this one:
	bool canJump;
	bool canMoveLeft;
	bool canFireMissile;
	bool canMoveRight;
	// Variables can also be of different types. The variable above is a bool, which means true or false.

	public float SidewaysTorque = 50;
	public float SidewaysForce = 10;
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
			canMoveLeft = true;
		}

		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
		{
			canFireMissile = true;
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			canMoveRight = true;
		}
	}

	// For physics engine updates. About 100/s
	// Apply forces here.
	void FixedUpdate()
	{
		// new Vector3(0, 0, 10)
		Player.AddForce(transform.forward * 5);
		if (canJump)
		{
			Player.AddForce(transform.up * 500);
			canJump = false;
		}

		if (canMoveLeft)
		{
			//Player.AddTorque(new Vector3(0, SidewaysTorque, 0));
			Player.AddForce(transform.right * SidewaysForce);
			canMoveLeft = false;
		}

		if (canMoveRight)
		{
			//Player.AddTorque(new Vector3(0, -SidewaysTorque, 0));
			Player.AddForce(transform.right * -SidewaysForce);
			canMoveRight = false;
		}

		if (canFireMissile)
		{
			canFireMissile = false;
			Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
			GameObject missile = Instantiate(MissilePrototype, position, new Quaternion(0, 90, 90, 0));
			const float missileSpeed = 2500f;
			float playerSpeed = (transform.position - lastPosition).magnitude;
			//Debug.Log(playerSpeed);
			missile.GetComponent<Rigidbody>().AddForce(transform.forward * (missileSpeed + playerSpeed * 2000));
			missile.GetComponent<MissileBehavior>().flying = true;
		}
		// 
		lastPosition = transform.position;
	}
}
