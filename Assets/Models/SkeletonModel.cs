using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonModel : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
	void OnTriggerEnter(Collider other)
	{
		if (Tags.IsPlayer(other.gameObject))
			Debug.Log("Player just hit skeleton!");
	}

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("OnCollisionEnter - Skeleton model!");
	}
}
