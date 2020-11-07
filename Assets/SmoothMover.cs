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

	}

	// constructor...
	public SmoothMover(int durationSeconds, int startValue, int endValue)
	{
		DurationSeconds = durationSeconds;
		StartValue = startValue;
		EndValue = endValue;
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

		float endTime = StartTime + DurationSeconds;
		bool alreadyFinished = currentTime >= endTime;
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
