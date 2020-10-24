using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public float CameraY;
	public Transform Player;
	// Start is called before the first frame update
	void Start()
	{

	}

	void FixedUpdate()
	{
		float averageY = (Player.position.y + CameraY) / 2;
		transform.position = new Vector3(Player.position.x, averageY, Player.position.z - 7);
	}
}
