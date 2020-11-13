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
	int ObstacleCount;
	public float DistanceBetweenDrops = 10;
	public float DistanceAheadToDrop = 20;
	public GameObject Prototype;
	public GameObject Camera;
	float endOfTrackZ;

	// Start is called before the first frame update
	void Start()
	{
		SetupNewTrack();
	}

	private void SetupNewTrack(float yOffset = 0, float zOffset = 0)
	{
		SetTrackLength(Track, yOffset, zOffset);
		SetTrackLength(LeftWall, yOffset, zOffset);
		SetTrackLength(RightWall, yOffset, zOffset);
		endOfTrackZ = zOffset + TrackLength;
	}

	private void SetTrackLength(Transform transform, float yOffset, float zOffset)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, TrackLength);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + yOffset, TrackLength / 2 + zOffset);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (PlayerPosition.position.z >= endOfTrackZ)
		{
			SetupNewTrack(-3, endOfTrackZ + 5);
			CameraFollow cameraFollow = Camera.GetComponent<CameraFollow>();
			cameraFollow.CameraY.Shift(-3);
		}
		float deltaZ = PlayerPosition.position.z - LastDropPositionZ;
		if (deltaZ > DistanceBetweenDrops)
		{
			LastDropPositionZ = PlayerPosition.position.z;

			for (int xOffset = 0; xOffset < StackSize; xOffset++)
				for (int yOffset = 0; yOffset < StackSize; yOffset++)
					//for (int zOffset = 0; zOffset < StackSize; zOffset++)
						DropBlock(xOffset - StackSize / 2.0f + 0.5f, yOffset, 0 /* zOffset */);

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
		ObstacleCount++;
	}
}
