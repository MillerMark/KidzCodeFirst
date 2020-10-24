using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLogic : MonoBehaviour
{
	public Transform Camera;
	void Start()
	{

	}

	void FixedUpdate()
	{
		if (transform.position.z < Camera.position.z && transform.position.y > 0)
			Destroy(this.gameObject);
	}
}
