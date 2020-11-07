using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLogic : MonoBehaviour
{
	public Transform BlackHole;
	bool isFloating;
	internal bool live; // <<
	internal float creationTime;
	public float LifeSpanSeconds = 3;

	void Start()
	{
		creationTime = Time.time;
	}

	void FixedUpdate()
	{
		float deathTime = creationTime + LifeSpanSeconds;
		if (Time.time > deathTime && live && !isFloating)
		{
			Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
			rigidbody.useGravity = false;
			FloatingBehavior floatingBehavior = gameObject.AddComponent<FloatingBehavior>();
			floatingBehavior.BlackHole = BlackHole;
			isFloating = true;
		}
	}

	public static void BlowUpBlock(GameObject gameObject, Transform blackHole)
	{
		const int numBlocksPerSide = 2;
		const float scale = 1f / numBlocksPerSide;
		Vector3 blockPosition = gameObject.transform.position;
		float localScale = scale * gameObject.transform.localScale.x;
		Vector3 scaleVector = new Vector3(localScale, localScale, localScale);

		Vector3 positionVector = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z);

		for (int i = 0; i < Math.Pow(numBlocksPerSide, 3); i++)
		{
			CreateParticle(gameObject, scaleVector, positionVector, blackHole);
		}

		Destroy(gameObject);
	}

	private static void CreateParticle(GameObject gameObject, Vector3 scaleVector, Vector3 positionVector, Transform blackHole)
	{
		GameObject newParticle = Instantiate(gameObject, positionVector, Quaternion.identity);
		newParticle.transform.localScale = scaleVector;
		Rigidbody rigidbody = newParticle.GetComponent<Rigidbody>();
		rigidbody.useGravity = false;
		FloatingBehavior floatingBehavior = newParticle.AddComponent<FloatingBehavior>();
		floatingBehavior.BlackHole = blackHole;
	}
}
