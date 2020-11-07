using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
	public Transform BlackHole;
	public float MissileLifeSpanSeconds = 3f;
	float creationTime;
	internal bool flying;
	public int numBlocksToPowerUp = 1;
	static int numBlocksDestroyed;
	public GameObject PowerUp;

	// Start is called before the first frame update
	void Start()
	{
		creationTime = Time.time;
	}

	bool LifeHasExpired()
	{
		return Time.time - creationTime > MissileLifeSpanSeconds;
	}

	void FixedUpdate()
	{
		if (LifeHasExpired() && flying)
		{
			//Debug.Log($"Destroying the missile which was created at {creationTime} (current time is {Time.time}).");
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!collision.gameObject.name.Contains("PrototypeBlock"))
		{
			//Debug.Log("Only blowing up blocks!!!");
			return;
		}
		ObstacleLogic.BlowUpBlock(collision.gameObject, BlackHole);
		numBlocksDestroyed++;

		CheckForPowerUp();
		Destroy(gameObject);
	}

	private void CheckForPowerUp()
	{
		if (numBlocksDestroyed >= numBlocksToPowerUp)
		{
			const float dropAheadPosition = 19f;
			const float dropAheadHeight = 10f;
			Vector3 powerUpPosition = new Vector3(transform.position.x, transform.position.y + dropAheadHeight, transform.position.z + dropAheadPosition);
			GameObject powerUp = Instantiate(PowerUp, powerUpPosition, Quaternion.identity);
			Rigidbody rigidbody = powerUp.GetComponent<Rigidbody>();
			rigidbody.useGravity = true;
			numBlocksDestroyed = 0;
		}
	}
}
