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
		PlaceSkeletonAtEndOfTrack();
	}

	void CreateNextTrack(float yOffset = 0, float zOffset = 0)
	{
		nextFloor = Instantiate(Floor);
		nextLeftWall = Instantiate(LeftWall);
		nextRightWall = Instantiate(RightWall);
		PlaceSkeletonAtEndOfTrack();
		//SkeletonController controller = Skeleton.GetComponent<SkeletonController>();
		//controller.Stand();

		//int randomNumber = UnityEngine.Random.Range(0, 4);
		//switch (randomNumber)
		//{
		//	case 0:
		//		controller.Attack();
		//		break;
		//	case 1:
		//		controller.Stand();
		//		break;
		//	case 2:
		//		controller.Skill();
		//		break;
		//	case 3:
		//		controller.Damage();
		//		break;
		//}
		
		SetTrackLengthAndPosition(nextFloor, yOffset, zOffset);
		SetTrackLengthAndPosition(nextLeftWall, yOffset, zOffset);
		SetTrackLengthAndPosition(nextRightWall, yOffset, zOffset);
		endOfNextTrackZ = zOffset + TrackLength;
	}

	public static bool IsPlayer(GameObject someObject)
	{
		return someObject.tag == "Player";
	}



	private void PlaceSkeletonAtEndOfTrack()
	{
		Skeleton.position = new Vector3(0, Floor.transform.position.y + 0.5f, Floor.transform.position.z + Floor.transform.localScale.z / 2);
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
			float secToCompleteThisLap = endTime - lapStartTime;
			extraTime = secToCompleteThisLap - lapTime;
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
			MoveCameraDownToNewLevel();
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

		DropBlocksIfNeeded();
	}

	private void MoveCameraDownToNewLevel()
	{
		CameraFollow cameraFollow = Camera.GetComponent<CameraFollow>();
		cameraFollow.CameraY.Shift(distanceToDropBetweenLevels);
	}

	private void DropBlocksIfNeeded()
	{
		float playerZ = Player.transform.position.z;
		float distancePlayerHasMovedSinceLastDrop = playerZ - LastDropPositionZ;

		bool weHaveMovedFarEnoughSinceTheLastDrop = distancePlayerHasMovedSinceLastDrop > DistanceBetweenDrops;
		bool thereIsRoomToDropBlockOnCurrentTrack = playerZ + DistanceAheadToDrop < endOfTrackZ;

		if (!weHaveMovedFarEnoughSinceTheLastDrop)
			return;

		if (!thereIsRoomToDropBlockOnCurrentTrack)
			return;

		LastDropPositionZ = playerZ;
		DropRectangularArrayOfBlocks();
		MakeArrayBiggerIfNeeded();
	}

	private void MakeArrayBiggerIfNeeded()
	{
		bool tooManyBlocks = StackWidth * StackHeight > 24 * 24;
		if (!tooManyBlocks)
			MakeArrayBigger();
	}

	private void MakeArrayBigger()
	{
		if (StackWidth < StackHeight)
			StackWidth++;
		else
			StackHeight++;
	}

	private void DropRectangularArrayOfBlocks()
	{
		int width = StackWidth;
		int height = StackHeight;

		if (UnityEngine.Random.value > 0.5)
		{
			// Swap the aspect ratio.
			width = StackHeight;
			height = StackWidth;
		}

		for (int xOffset = 0; xOffset < width; xOffset++)
			for (int yOffset = 0; yOffset < height; yOffset++)
				DropBlock(xOffset - StackWidth / 2.0f + 0.5f, yOffset, 0 /* zOffset */);
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
		//BlowUpBlockIntoSmallerPieces(block, BlackHole);
		BlowUpBlockIntoIndividualParts(block, BlackHole);
	}

	public static void BlowUpBlockIntoIndividualParts(GameObject gameObject, Transform gravityCenter)
	{
		ObstacleLogic originalObstacleLogic = gameObject.GetComponent<ObstacleLogic>();
		if (originalObstacleLogic == null)
		{
			Debug.LogError("You can only blow up objects that have the ObstacleLogic script attached!");
			return;
		}
		Vector3 blockPosition = gameObject.transform.position;
		//Vector3 localScale = gameObject.transform.localScale;
		//Vector3 scaleVector = new Vector3(localScale.x, localScale.y, localScale.z);
		Vector3 positionVector = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z);

		foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
		{
			if (child.gameObject.tag == gameObject.tag)
			{
				Debug.Log("Skipping the parent game object");
				continue;
			}

			Debug.Log($"Separating: {child.gameObject.tag}");
			GameObject part = CreateParticle(child.gameObject, child.gameObject.transform.localScale, positionVector, gravityCenter);
			part.transform.parent = null;
			Debug.Log("Particle created!");
			part.AddComponent<Rigidbody>();
			part.AddComponent<BoxCollider>();
			Rigidbody rigidbody = part.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				Debug.Log("Adding rigid body worked!");
			}
			else
			{
				Debug.Log("Adding rigid body failed!");
			}
			ObstacleLogic obstacleLogic = part.AddComponent<ObstacleLogic>();
			obstacleLogic.BlackHole = originalObstacleLogic.BlackHole;
			obstacleLogic.LifeSpanSeconds = originalObstacleLogic.LifeSpanSeconds;
			AddBlackHole(gravityCenter, part);
		}

		Destroy(gameObject);
	}

	public static void BlowUpBlockIntoSmallerClones(GameObject gameObject, Transform gravityCenter, int numBlocksPerSide = 2)
	{
		if (gameObject.transform.localScale.x < 1)
			return;

		float scale = 1f / numBlocksPerSide;
		Vector3 blockPosition = gameObject.transform.position;
		float localScale = scale * gameObject.transform.localScale.x;
		Vector3 scaleVector = new Vector3(localScale, localScale, localScale);

		Vector3 positionVector = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z);

		for (int i = 0; i < Math.Pow(numBlocksPerSide, 3); i++)
			CreateParticle(gameObject, scaleVector, positionVector, gravityCenter);

		Destroy(gameObject);
	}

	private static GameObject CreateParticle(GameObject gameObject, Vector3 scaleVector, Vector3 positionVector, Transform gravityCenter)
	{
		GameObject newParticle = Instantiate(gameObject, positionVector, Quaternion.identity);
		newParticle.transform.localScale = scaleVector;
		AddBlackHole(gravityCenter, newParticle);
		return newParticle;
	}

	private static void AddBlackHole(Transform gravityCenter, GameObject newParticle)
	{
		Rigidbody rigidbody = newParticle.GetComponent<Rigidbody>();
		if (rigidbody != null)
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
