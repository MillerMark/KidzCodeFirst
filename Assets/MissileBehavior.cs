using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
	public Transform BlackHole;
	public float MissileLifeSpan = 3;
	public bool flying = false;
	float creationTime;

	// Start is called before the first frame update
	void Start()
	{
		creationTime = Time.time;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (flying && LifeSpanIsOver())
			Destroy(gameObject);
	}

	private bool LifeSpanIsOver()
	{
		return Time.time - creationTime > MissileLifeSpan;
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
