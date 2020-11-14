using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public SmoothMover CameraY;
	public Transform Player;
	// Start is called before the first frame update
	void Start()
	{
		CameraY = new SmoothMover(1, 5, 5);
	}

	void FixedUpdate()
	{
		float averageY = (Player.position.y + CameraY.Value) / 2;
		transform.position = new Vector3(Player.position.x / 2, averageY, Player.position.z - 7);
	}
}
