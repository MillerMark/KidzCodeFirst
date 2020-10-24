using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public Transform PlayerPosition;
	float LastDropPositionZ = -510;
	int StackSize = 1;
	public float DistanceBetweenDrops = 10;
	public float DistanceAheadToDrop = 20;
	public GameObject Prototype;
	
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		float deltaZ = PlayerPosition.position.z - LastDropPositionZ;
		if (deltaZ > DistanceBetweenDrops)
		{
			LastDropPositionZ = PlayerPosition.position.z;

			for (int xOffset = 0; xOffset < StackSize; xOffset++)
				for (int yOffset = 0; yOffset < StackSize; yOffset++)
					for (int zOffset = 0; zOffset < StackSize; zOffset++)
						DropBlock(xOffset, yOffset, zOffset);

			StackSize++;
		}
	}

	private void DropBlock(int xOffset, int yOffset, int zOffset)
	{
		Vector3 position = new Vector3(PlayerPosition.position.x + xOffset, PlayerPosition.position.y + 2 + yOffset, PlayerPosition.position.z + zOffset + DistanceAheadToDrop);
		Instantiate(Prototype, position, Quaternion.identity);
	}
}
