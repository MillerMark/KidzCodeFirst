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
	public static bool IsObstacle(GameObject gameObject)
	{
		return gameObject.tag == "Obstacle";
	}
}