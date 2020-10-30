using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public Transform PlayerPosition;
	public Transform Track;
	public Transform LeftWall;
	public Transform RightWall;
	public float TrackLength = 500;
	float LastDropPositionZ = -510;
	int StackSize = 1;
	public float DistanceBetweenDrops = 10;
	public float DistanceAheadToDrop = 20;
	public GameObject Prototype;
	
	// Start is called before the first frame update
	void Start()
	{
		SetupNewTrack();
	}

	private void SetupNewTrack()
	{
		SetTrackLength(Track);
		SetTrackLength(LeftWall);
		SetTrackLength(RightWall);
	}

	private void SetTrackLength(Transform transform)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, TrackLength);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, TrackLength / 2);
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
						DropBlock(xOffset - StackSize / 2.0f + 0.5f, yOffset, zOffset);

			StackSize++;
		}
	}

	private void DropBlock(float xOffset, int yOffset, int zOffset)
	{
		Vector3 position = new Vector3(PlayerPosition.position.x + xOffset, PlayerPosition.position.y + 2 + yOffset, PlayerPosition.position.z + zOffset + DistanceAheadToDrop);
		GameObject obstacle = Instantiate(Prototype, position, Quaternion.identity);
		ObstacleLogic obstacleLogic = obstacle.GetComponent<ObstacleLogic>();
		obstacleLogic.live = true;
		obstacleLogic.creationTime = Time.time;
	}
}
