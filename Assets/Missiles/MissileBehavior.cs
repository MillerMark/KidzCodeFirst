using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MissileBehavior : MonoBehaviour
{
	public float MissileLifeSpanSeconds = 3f;
	float creationTime;
	internal bool flying;
	public GameObject GameLogic;

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
		if (!Tags.IsObstacle(collision.gameObject))
			return;

		GameLogic gameLogic = GameLogic.GetComponent<GameLogic>();
		gameLogic.MissileHitsBlock(collision.gameObject);
		Destroy(gameObject);
	}
}
