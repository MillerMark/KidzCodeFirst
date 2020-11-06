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

	void BlowUpBlock(GameObject gameObject)
	{
		const int numBlocksPerSide = 2;
		const float scale = 1f / numBlocksPerSide;
		Vector3 blockPosition = gameObject.transform.position;
		float localScale = scale * gameObject.transform.localScale.x;
		Vector3 scaleVector = new Vector3(localScale, localScale, localScale);

		Vector3 positionVector = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z);

		for (int i = 0; i < Math.Pow(numBlocksPerSide, 3); i++)
		{
			CreateParticle(gameObject, scaleVector, positionVector);
		}

		Destroy(gameObject);

		numBlocksDestroyed++;

		if (numBlocksDestroyed >= numBlocksToPowerUp)
		{
			const float dropAheadPosition = 19f;
			const float dropAheadHeight = 10f;
			Vector3 powerUpPosition = new Vector3(transform.position.x, transform.position.y + dropAheadHeight, transform.position.z + dropAheadPosition);
			GameObject powerUp = Instantiate(PowerUp, powerUpPosition, Quaternion.identity);
			Rigidbody rigidbody = powerUp.GetComponent<Rigidbody>();
			rigidbody.useGravity = true;
			numBlocksDestroyed = 0;

			Debug.Log("Power Up Created");
		}
	}

	private void CreateParticle(GameObject gameObject, Vector3 scaleVector, Vector3 positionVector)
	{
		GameObject newParticle = Instantiate(gameObject, positionVector, Quaternion.identity);
		newParticle.transform.localScale = scaleVector;
		Rigidbody rigidbody = newParticle.GetComponent<Rigidbody>();
		rigidbody.useGravity = false;
		FloatingBehavior floatingBehavior = newParticle.AddComponent<FloatingBehavior>();
		floatingBehavior.BlackHole = BlackHole;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!collision.gameObject.name.Contains("PrototypeBlock"))
		{
			//Debug.Log("Only blowing up blocks!!!");
			return;
		}
		BlowUpBlock(collision.gameObject);
		Destroy(gameObject);
	}
}
