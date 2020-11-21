using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
	private const float distanceToDropBetweenLevels = -3f;
	private const float distanceBetweenTracks = 5f;
	SmoothMover trackOpacity;
	public GameObject Player;
	public GameObject Floor;
	public GameObject LeftWall;
	public GameObject RightWall;
	GameObject nextFloor;
	GameObject nextLeftWall;
	GameObject nextRightWall;

	public float TrackLength = 500;
	public float TrackIncreasePerLevel = 100;

	float LastDropPositionZ = -510;
	int StackWidth = 1;
	int StackHeight = 1;
	int ObstacleCount = 0;
	public float DistanceBetweenDrops = 10;
	public float DistanceAheadToDrop = 20;
	public float SecondsPerLap = 12;
	public GameObject Obstacle;
	public GameObject Camera;
	public Text LevelText;
	public Text CountdownText;
	public Transform BlackHole;
	public Transform Skeleton;
	public GameObject PowerUp;
	public int NumBlocksToPowerUp = 4;
	float endOfTrackZ;
	float endOfNextTrackZ;
	int levelCount;
	float lapStartTime = -1;
	float endTime;
	static int numBlocksDestroyed;

	// Start is called before the first frame update
	void Start()
	{
		SetupInitialTrack();
		CreateNextTrack(distanceToDropBetweenLevels, endOfTrackZ + distanceBetweenTracks);
		StartNewLevel();
	}

	private void SetupInitialTrack()
	{
		SetTrackLengthAndPosition(Floor);
		SetTrackLengthAndPosition(LeftWall);
		SetTrackLengthAndPosition(RightWall);
		endOfTrackZ = TrackLength;
		Skeleton.position = new Vector3(0, Floor.transform.position.y + 0.5f, Floor.transform.position.z + Floor.transform.localScale.z / 2);
	}

	void CreateNextTrack(float yOffset = 0, float zOffset = 0)
	{
		nextFloor = Instantiate(Floor);
		nextLeftWall = Instantiate(LeftWall);
		nextRightWall = Instantiate(RightWall);

		Skeleton.position = new Vector3(0, Floor.transform.position.y + 0.5f, Floor.transform.position.z + Floor.transform.localScale.z / 2);
		SkeletonController controller = Skeleton.GetComponent<SkeletonController>();
		controller.Attack();

		SetTrackLengthAndPosition(nextFloor, yOffset, zOffset);
		SetTrackLengthAndPosition(nextLeftWall, yOffset, zOffset);
		SetTrackLengthAndPosition(nextRightWall, yOffset, zOffset);
		endOfNextTrackZ = zOffset + TrackLength;
	}

	private void StartNewLevel()
	{
		levelCount++;
		StackHeight = levelCount;
		StackWidth = 1;
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

		trackOpacity = new SmoothMover(SecondsPerLap * 0.75f, 1, 0.1f);
		trackOpacity.StartTime = endTime - trackOpacity.DurationSeconds;

		TrackLength += TrackIncreasePerLevel;
	}

	void ShowTrackLevel()
	{
		//$"" Means that you can mix {data} with text
		LevelText.text = $"Level {levelCount}";
	}
	void ShowTrackCountdown()
	{
		float timeRemaining = GetTimeRemaining();
		if (timeRemaining < 0)
			ShowGameOver();
		else
			CountdownText.text = $"{timeRemaining:F1}s";
	}

	private void ShowGameOver()
	{
		CountdownText.text = "Game Over!";
	}

	private void SetTrackLengthAndPosition(GameObject gameObject, float yOffset = 0, float zOffset = 0)
	{
		Transform transform = gameObject.transform;
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, TrackLength);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + yOffset, TrackLength / 2 + zOffset);
	}


	bool gameIsOver = false;
	void GameOver()
	{
		Destroy(Floor);
		Floor = null;
		gameIsOver = true;
		ShowGameOver();
		//SceneManager.LoadScene("Game Over");
	}

	void FixedUpdate()
	{
		if (gameIsOver)
			return;

		if (PlayerFinishedLevel())
		{
			NextLevel();
			CameraFollow cameraFollow = Camera.GetComponent<CameraFollow>();
			cameraFollow.CameraY.Shift(distanceToDropBetweenLevels);
		}
		else
		{
			if (Floor != null && Player.transform.position.y < Floor.transform.position.y)
			{
				GameOver();
				return;
			}

			float timeRemaining = GetTimeRemaining();

			if (timeRemaining < 0)
			{
				GameOver();
				return;
			}
		}

		// Refactoring...

		float playerZ = Player.transform.position.z;
		float deltaZ = playerZ - LastDropPositionZ;
		if (deltaZ > DistanceBetweenDrops && playerZ + DistanceAheadToDrop < endOfTrackZ)
		{
			LastDropPositionZ = playerZ;

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

	private void NextLevel()
	{
		TransferVariables();
		CreateNextTrack(distanceToDropBetweenLevels, endOfTrackZ + distanceBetweenTracks);
		StartNewLevel();
	}

	private void TransferVariables()
	{
		endOfTrackZ = endOfNextTrackZ;
		Floor = nextFloor;
		LeftWall = nextLeftWall;
		RightWall = nextRightWall;
		PlayerProperties playerProperties = Player.GetComponent<PlayerProperties>();
		playerProperties.Track = Floor.GetComponent<Transform>();
	}

	private bool PlayerFinishedLevel()
	{
		return Player.transform.position.z >= endOfTrackZ;
	}

	private float GetTimeRemaining()
	{
		return endTime - Time.time;
	}

	void Update()
	{
		if (gameIsOver)
			return;
		ShowTrackCountdown();
		if (trackOpacity != null)
			SetTrackOpacity(trackOpacity.Value);
	}

	private void SetTrackOpacity(float opacity)
	{
		if (Floor == null)
			return;
		MeshRenderer meshRenderer = Floor.GetComponent<MeshRenderer>();
		Color color = meshRenderer.material.color;
		meshRenderer.material.color = new Color(color.r, color.g, color.b, opacity);
	}

	private void DropBlock(float xOffset, int yOffset, int zOffset)
	{
		Vector3 playerPosition = Player.transform.position;
		Vector3 position = new Vector3(playerPosition.x + xOffset, playerPosition.y + 2 + yOffset, playerPosition.z + zOffset + DistanceAheadToDrop);
		GameObject obstacle = Instantiate(Obstacle, position, Quaternion.identity);

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
