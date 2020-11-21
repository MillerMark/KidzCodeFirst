using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deletes cloned instances instantiated with a prototype. 
/// Keep all your prototypes **behind** the start of the track 
/// (z less than 0).
/// </summary>
public class DeleteBehindCamera : MonoBehaviour
{
	public Transform Camera;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		bool onTheTrack = transform.position.z > 0;
		bool behindCamera = transform.position.z + transform.localScale.z / 2f < Camera.position.z;

		if (onTheTrack && behindCamera)
			Destroy(gameObject);
	}
}
