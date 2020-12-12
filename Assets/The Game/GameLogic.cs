using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic: MonoBehaviour
{
	int numRampPowerUps;
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
	float startOfNextTrackZ;
	int StackWidth = 1;
	int StackHeight = 1;
	int ObstacleCount = 0;
	public float DistanceBetweenDrops = 10;
	float nextTrackY;
	public float DistanceAheadToDrop = 20;
	public float SecondsPerLap = 12;
	public GameObject Obstacles;
	public GameObject PowerUps;
	public GameObject Ramp;
	public GameObject Camera;
	public Text LevelText;
	public Text PowerUpText;
	public Text CountdownText;
	public Transform BlackHole;
	public Transform Skeleton;
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
		ShowNumPowerUps();
		SetupInitialTrack();
		CreateNextTrack(distanceToDropBetweenLevels, endOfTrackZ + distanceBetweenTracks);
		StartNewLevel();
	}

	
	public int NumRampPowerUps
	{
		get
		{
			PlayerProperties playerScript = Player.GetComponent<PlayerProperties>();
			return playerScript.NumRampPowerUps;
		}
		set
		{
			PlayerProperties playerScript = Player.GetComponent<PlayerProperties>();
			playerScript.NumRampPowerUps = value;
			Debug.Log($"playerScript.NumRampPowerUps: {playerScript.NumRampPowerUps}");
			ShowNumPowerUps();
		}
	}

	private void ShowNumPowerUps()
	{
		PowerUpText.text = new string('+', NumRampPowerUps);
	}

	private void SetupInitialTrack()
	{
		SetTrackLengthAndPosition(Floor);
		SetTrackLengthAndPosition(LeftWall);
		SetTrackLengthAndPosition(RightWall);
		endOfTrackZ = TrackLength;
		PlaceSkeletonAtEndOfTrack();
	}

	void CreateNextTrack(float verticalDrop = 0, float trackStartZ = 0)
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
		
		SetTrackLengthAndPosition(nextFloor, verticalDrop, trackStartZ);
		SetTrackLengthAndPosition(nextLeftWall, verticalDrop, trackStartZ);
		SetTrackLengthAndPosition(nextRightWall, verticalDrop, trackStartZ);
		endOfNextTrackZ = trackStartZ + TrackLength;
		startOfNextTrackZ = trackStartZ;
		nextTrackY = nextFloor.transform.position.y;
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
		CalculateTrackTimeout(extraTime);

		TrackLength += TrackIncreasePerLevel;
	}

	private void CalculateTrackTimeout(float extraTime)
	{
		endTime = lapStartTime + SecondsPerLap + Math.Max(0, extraTime);

		trackOpacity = new SmoothMover(SecondsPerLap * 0.75f, 1, 0.1f);
		trackOpacity.StartTime = endTime - trackOpacity.DurationSeconds;
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
			CountdownText.text = "0!!!!";
		else
			CountdownText.text = $"{timeRemaining:F1}s";
	}

	private void ShowGameOver()
	{
		CountdownText.text = "Game Over!";
	}

	private void SetTrackLengthAndPosition(GameObject gameObject, float verticalDrop = 0, float zOffset = 0)
	{
		Transform transform = gameObject.transform;
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, TrackLength);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + verticalDrop, TrackLength / 2 + zOffset);
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

		if (PlayerStartsNextLevel())
		{
			NextLevel();
			MoveCameraDownToNewLevel();
		}
		else
		{
			if (Player.transform.position.y < nextTrackY)
			{
				GameOver();
				return;
			}

			float timeRemaining = GetTimeRemaining();

			if (timeRemaining < 0)
			{
				Destroy(Floor);
				Floor = null;
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

	private bool PlayerStartsNextLevel()
	{
		return Player.transform.position.z >= startOfNextTrackZ;
	}

	private float GetTimeRemaining()
	{
		return endTime - Time.time;
	}

	int lastNumPowerUps;
	void Update()
	{
		if (gameIsOver)
			return;

		PlayerProperties playerScript = Player.GetComponent<PlayerProperties>();
		bool powerUpCountHasChanged = playerScript.NumRampPowerUps != lastNumPowerUps;
		if (powerUpCountHasChanged)
		{
			SavePowerUpCount(playerScript);
			ShowNumPowerUps();
		}

		ShowTrackCountdown();
		if (trackOpacity != null)
			SetTrackOpacity(trackOpacity.Value);
		if (Input.GetKeyDown(KeyCode.Return))
		{
			LaunchRamp();
		}
	}

	private void SavePowerUpCount(PlayerProperties playerScript)
	{
		lastNumPowerUps = playerScript.NumRampPowerUps;
	}

	GameObject lastRampCreated;

	public enum RampDoubleResult
	{
		Success,
		Failed
	}

	RampDoubleResult DoubleRampSize(GameObject gameObject)
	{
		RampScaleUp rampScaleUp = gameObject.GetComponent<RampScaleUp>();
		if (rampScaleUp.weHaveAlreadyDoubledTheSize)
			return RampDoubleResult.Failed;
		rampScaleUp.DoubleRampSize();
		NumRampPowerUps--;
		return RampDoubleResult.Success;
	}

	private void LaunchRamp()
	{
		if (NumRampPowerUps <= 0)
			return;

		if (lastRampCreated != null)
		{
			bool rampIsInFrontOfPlayer = lastRampCreated.transform.localPosition.z > Player.transform.localPosition.z;
			if (rampIsInFrontOfPlayer)
			{
				if (DoubleRampSize(lastRampCreated) == RampDoubleResult.Success)
					return;
				Destroy(lastRampCreated);
				lastRampCreated = null;
			}
		}

		NumRampPowerUps--;
		Vector3 rampStartPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z + 0.5f);
		GameObject ramp = Instantiate(Ramp, rampStartPosition, Ramp.transform.rotation);
		lastRampCreated = ramp;
		Rigidbody rigidbody = ramp.GetComponent<Rigidbody>();
		Vector3 playerVelocity = Player.GetComponent<Rigidbody>().velocity;
		Vector3 zOnly = new Vector3(0, 0, playerVelocity.z);

		if (zOnly.z < 10)
		{
			rigidbody.AddForce(transform.forward * 10000 + zOnly * 800);
		}
		else
		{
			rigidbody.AddForce(transform.forward * 6000 + zOnly * 800);
		}
		rigidbody.AddForce(transform.up * 2500);
		RampScaleUp rampScaleUp = ramp.GetComponent<RampScaleUp>();
		rampScaleUp.ScaleUpNow();
		ramp.transform.localScale = Vector3.zero;
	}

	private void SetTrackOpacity(float opacity)
	{
		if (Floor == null)
			return;
		MeshRenderer meshRenderer = Floor.GetComponent<MeshRenderer>();
		Color color = meshRenderer.material.color;
		meshRenderer.material.color = new Color(color.r, color.g, color.b, opacity);
	}

	bool TagMatches(GameObject gameObject, string tag)
	{
		return gameObject.tag == tag;
	}

	GameObject GetRandomChild(GameObject parent, string tag = null)
	{
		Transform[] childTransforms = parent.GetComponentsInChildren<Transform>();
		const int maxTries = 40;
		int numTries = 0;
		while (numTries < maxTries)
		{
			int randomIndex = UnityEngine.Random.Range(0, childTransforms.Length);
			Transform maybeObstacle = childTransforms[randomIndex];
			if (tag == null || TagMatches(maybeObstacle.gameObject, tag))
				return maybeObstacle.gameObject;
			numTries++;
		}

		foreach (Transform transform in childTransforms)
			if (tag == null || TagMatches(transform.gameObject, tag))
				return transform.gameObject;

		return null;

	}
	private void DropBlock(float xOffset, int yOffset, int zOffset)
	{
		Vector3 playerPosition = Player.transform.position;
		Vector3 position = new Vector3(playerPosition.x + xOffset, playerPosition.y + 2 + yOffset, playerPosition.z + zOffset + DistanceAheadToDrop);

		GameObject prototype = GetRandomChild(Obstacles, Tags.STR_Obstacle);
		GameObject obstacle = Instantiate(prototype, position, Quaternion.identity);

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
		RandomBlowUp(block, BlackHole);
	}

	public static void BlowUpBlockIntoIndividualParts(GameObject gameObject, Transform gravityCenter)
	{
		if (!Tags.IsObstacle(gameObject))
			return;

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
			bool isParent = child.gameObject.tag == gameObject.tag;
			if (isParent)
				continue;

			//Debug.Log($"Separating: {child.gameObject.name}");
			GameObject part = CreateParticle(child.gameObject, child.gameObject.transform.localScale, positionVector, gravityCenter);
			part.transform.parent = null;
			part.AddComponent<Rigidbody>();
			part.AddComponent<BoxCollider>();
			Rigidbody rigidbody = part.GetComponent<Rigidbody>();
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
			CreatePowerUp(block);
	}

	private void CreatePowerUp(GameObject block)
	{
		const float dropAheadPosition = 30f;
		const float dropAheadHeight = 10f;
		Vector3 powerUpPosition = new Vector3(block.transform.position.x, block.transform.position.y + dropAheadHeight, block.transform.position.z + dropAheadPosition);
		GameObject randomPowerUp = GetRandomChild(PowerUps, PowerUpBehavior.STR_PowerUp);
		GameObject powerUp = Instantiate(randomPowerUp, powerUpPosition, randomPowerUp.transform.rotation);
		Rigidbody rigidbody = powerUp.GetComponent<Rigidbody>();
		rigidbody.useGravity = true;
		numBlocksDestroyed = 0;
	}

	public static void RandomBlowUp(GameObject gameObject, Transform blackHole)
	{
		if (UnityEngine.Random.Range(0, 100) < 50)
			BlowUpBlockIntoIndividualParts(gameObject, blackHole);
		else
			BlowUpBlockIntoSmallerClones(gameObject, blackHole);
	}
}
