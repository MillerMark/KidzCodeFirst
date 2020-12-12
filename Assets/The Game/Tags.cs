using System;
using UnityEngine;
public static class Tags
{
	public const string STR_Obstacle = "Obstacle";
	public const string STR_Ramp = "Ramp";
	public const string STR_Track = "Track";
	const string STR_Player = "Player";

	public static bool IsPlayer(GameObject someObject)
	{
		return someObject.tag == STR_Player;
	}

	public static bool IsObstacle(GameObject gameObject)
	{
		return gameObject.tag == STR_Obstacle;
	}

	public static bool IsRamp(GameObject gameObject)
	{
		return gameObject.tag == STR_Ramp;
	}

	public static bool IsTrack(GameObject gameObject)
	{
		return gameObject.tag == STR_Track;
	}
}
