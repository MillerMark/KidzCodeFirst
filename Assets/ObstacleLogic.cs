using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLogic : MonoBehaviour
{
	public Transform Camera;
	public Transform BlackHole;
	internal bool live;
	bool isFloating;
	internal float creationTime;
	public float LifeSpanSeconds = 3;

	void Start()
	{
		creationTime = Time.time;
	}

	void FixedUpdate()
	{
		if (transform.position.z < Camera.position.z && transform.position.y > 0)
			Destroy(gameObject);

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
}
