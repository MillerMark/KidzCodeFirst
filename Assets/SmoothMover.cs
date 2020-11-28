using System;
using UnityEngine;

public class SmoothMover
{
	public float StartTime { get; set; }
	public float DurationSeconds { get; set; }
	public float StartValue { get; set; }
	public float EndValue { get; set; }


	// constructor...
	public SmoothMover()
	{
		Start();
	}


	// constructor...
	public SmoothMover(float durationSeconds, float startValue, float endValue): this()
	{
		DurationSeconds = durationSeconds;
		StartValue = startValue;
		EndValue = endValue;
	}

	public float EndTime
	{
		get
		{
			return StartTime + DurationSeconds;
		}
	}


	public bool HasEnded
	{
		get
		{
			return Time.time > EndTime;
		}
	}


	public void Start()
	{
		StartTime = Time.time;
	}

	float GetValueFromPercent(float percentComplete)
	{
		if (percentComplete <= 0)
			return StartValue;
		if (percentComplete >= 1)
			return EndValue;
		float distanceBetweenValues = EndValue - StartValue;

		float distanceTraveledSoFar = percentComplete * distanceBetweenValues;
		return StartValue + distanceTraveledSoFar;
	}


	public float Value => GetValue();

	public float GetValue(float currentTime = float.NaN)
	{
		if (float.IsNaN(currentTime))
			currentTime = Time.time;

		float percentComplete = GetPercentComplete(currentTime);
		return GetValueFromPercent(percentComplete);
	}

	float GetPercentComplete(float currentTime)
	{
		bool weHaveNotStartedYet = currentTime <= StartTime;
		if (weHaveNotStartedYet)
			return 0;

		bool alreadyFinished = currentTime >= EndTime;	
		if (alreadyFinished)
			return 1;

		float howMuchTimeHasPassedSeconds = currentTime - StartTime;
		return howMuchTimeHasPassedSeconds / DurationSeconds;
	}

	/// <summary>
	/// Sets both start and end to the current value.
	/// </summary>
	public void Reset()
	{
		EndValue = Value;
		StartValue = EndValue;
	}

	/// <summary>
	/// Shifts the value forwards or backward from the current position
	/// </summary>
	public void Shift(float delta, float newDurationSeconds = float.NaN)
	{
		Reset();
		EndValue = EndValue + delta;
		bool hasNewDuration = !float.IsNaN(newDurationSeconds);
		if (hasNewDuration)
			DurationSeconds = newDurationSeconds;
		Start();
	}
}

