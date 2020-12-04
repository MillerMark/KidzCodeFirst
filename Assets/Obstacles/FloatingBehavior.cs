using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBehavior : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform BlackHole;
	public Rigidbody body;

	void Start()
	{
		body = gameObject.GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		const float minDistanceToBlackHole = 1000f;
		const int speed = 1;
		Vector3 gravitationalVector = BlackHole.transform.position - transform.position;
		body.AddForce(gravitationalVector * speed * Time.smoothDeltaTime);
		if (gravitationalVector.magnitude < minDistanceToBlackHole)
			Destroy(gameObject);
	}
}
