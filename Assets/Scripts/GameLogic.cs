using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
	public Transform PlayerPosition;
	public Transform Track;
	public GameObject Floor;
	public Transform LeftWall;
	public Transform RightWall;

	public float TrackLength = 500;
	public float TrackIncreasePerLevel = 100;

	float LastDropPositionZ = -510;
	int StackWidth = 1;
	int StackHeight = 1;
	int ObstacleCount = 0;
	public float DistanceBetweenDrops = 10;
	public float DistanceAheadToDrop = 20;
	public float SecondsPerLap = 7;
	public GameObject Prototype;
	public GameObject Camera;
	public Text LevelText;
	public Text CountdownText;
	public Transform BlackHole;
	public GameObject PowerUp;
	public int NumBlocksToPowerUp = 4;
	float endOfTrackZ;
	float trackCount;
	float lapStartTime = -1;
	float endTime;
	static int numBlocksDestroyed;

	// Start is called before the first frame update
	void Start()
	{
		SetupNewTrack();
	}

	void SetupNewTrack(float yOffset = 0, float zOffset = 0)
	{
		SetTrackLength(Track, yOffset, zOffset);
		SetTrackLength(LeftWall, yOffset, zOffset);
		SetTrackLength(RightWall, yOffset, zOffset);
		trackCount++;
		ShowTrackLevel();
		float lapTime = Time.time - lapStartTime;
		float extraTime = 0;
		if (lapStartTime >= 0)
		{
			extraTime = SecondsPerLap - lapTime;
			Debug.Log($"Lap Time is {lapTime}");
			Debug.Log($"extraTime {extraTime}");
		}

		lapStartTime = Time.time;
		// TODO: Show extra bonus time in some way!
		endTime = lapStartTime + SecondsPerLap + extraTime;
		TrackLength = TrackLength + TrackIncreasePerLevel;
	}

	void ShowTrackLevel()
	{
		//$"" Means that you can mix {data} with text
		LevelText.text = $"Level {trackCount}";
	}
	void ShowTrackCountdown()
	{
		float timeRemaining = GetTimeRemaining();
		if (timeRemaining < 0)
			CountdownText.text = "Game Over!😭😭😭";
		else
			CountdownText.text = $"{timeRemaining:F1}s";
	}

	private void SetTrackLength(Transform transform, float yOffset, float zOffset)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, TrackLength);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + yOffset, TrackLength / 2 + zOffset);
		endOfTrackZ = zOffset + TrackLength;
	}


	bool gameIsOver = false;
	void GameOver()
	{
		Destroy(Floor);
		gameIsOver = true;
	}

	void FixedUpdate()
	{
		if (gameIsOver)
			return;

		if (PlayerPosition.position.z >= endOfTrackZ)
		{
			SetupNewTrack(-3, endOfTrackZ + 5);
			CameraFollow cameraFollow = Camera.GetComponent<CameraFollow>();
			cameraFollow.CameraY.Shift(-3);
		}
		else
		{
			float timeRemaining = GetTimeRemaining();
			if (timeRemaining < 0)
				GameOver();
		}

		float deltaZ = PlayerPosition.position.z - LastDropPositionZ;
		if (deltaZ > DistanceBetweenDrops)
		{
			LastDropPositionZ = PlayerPosition.position.z;

			for (int xOffset = 0; xOffset < StackWidth; xOffset++)
				for (int yOffset = 0; yOffset < StackHeight; yOffset++)
					//for (int zOffset = 0; zOffset < StackSize; zOffset++)
					DropBlock(xOffset - StackWidth / 2.0f + 0.5f, yOffset, 0 /* zOffset */);


			if (StackWidth * StackHeight < 24 * 24)
			{
				if (StackWidth < StackHeight)
					StackWidth++;
				else
					StackHeight++;
			}
		}
	}

	private float GetTimeRemaining()
	{
		return endTime - Time.time;
	}

	void Update()
	{
		ShowTrackCountdown();
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

	/// <summary>
	/// Called when a missile hits an obstacle block.
	/// </summary>
	public void MissileHitsBlock(GameObject block)
	{
		numBlocksDestroyed++;
		CheckForPowerUp(block);
		BlowUpBlock(block, BlackHole);
	}

	public static void BlowUpBlock(GameObject gameObject, Transform gravityCenter)
	{
		if (gameObject.transform.localScale.x < 1)
			return;

		const int numBlocksPerSide = 2;
		const float scale = 1f / numBlocksPerSide;
		Vector3 blockPosition = gameObject.transform.position;
		float localScale = scale * gameObject.transform.localScale.x;
		Vector3 scaleVector = new Vector3(localScale, localScale, localScale);

		Vector3 positionVector = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z);

		for (int i = 0; i < Math.Pow(numBlocksPerSide, 3); i++)
			CreateParticle(gameObject, scaleVector, positionVector, gravityCenter);

		Destroy(gameObject);
	}

	private static void CreateParticle(GameObject gameObject, Vector3 scaleVector, Vector3 positionVector, Transform gravityCenter)
	{
		GameObject newParticle = Instantiate(gameObject, positionVector, Quaternion.identity);
		newParticle.transform.localScale = scaleVector;
		Rigidbody rigidbody = newParticle.GetComponent<Rigidbody>();
		rigidbody.useGravity = false;
		FloatingBehavior floatingBehavior = newParticle.AddComponent<FloatingBehavior>();
		floatingBehavior.BlackHole = gravityCenter;
	}

	private void CheckForPowerUp(GameObject block)
	{
		if (numBlocksDestroyed >= NumBlocksToPowerUp)
		{
			const float dropAheadPosition = 19f;
			const float dropAheadHeight = 10f;
			Vector3 powerUpPosition = new Vector3(block.transform.position.x, block.transform.position.y + dropAheadHeight, block.transform.position.z + dropAheadPosition);
			GameObject powerUp = Instantiate(PowerUp, powerUpPosition, Quaternion.identity);
			Rigidbody rigidbody = powerUp.GetComponent<Rigidbody>();
			rigidbody.useGravity = true;
			numBlocksDestroyed = 0;
		}
	}
}
