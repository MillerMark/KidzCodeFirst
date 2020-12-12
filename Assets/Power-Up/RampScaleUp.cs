using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampScaleUp : MonoBehaviour
{
	internal bool weHaveAlreadyDoubledTheSize;
	private const float scaleUpTime = 1.3f;
	const float xScaleFinal = 1f;
	const float yScaleFinal = 3.029372f;
	const float zScaleFinal = 2.247998f;
	public Transform BlackHole;
	SmoothMover scaleX;
	SmoothMover scaleY;
	SmoothMover scaleZ;
	// Start is called before the first frame update
	void Start()
	{

	}

	void InstantiateSmoothMovers()
	{
		scaleX = new SmoothMover(scaleUpTime, 0, xScaleFinal);
		scaleY = new SmoothMover(scaleUpTime, 0, yScaleFinal);
		scaleZ = new SmoothMover(scaleUpTime, 0, zScaleFinal);
	}

	void FixedUpdate()
	{
		if (scaleX == null)
			return;
		Vector3 scale = new Vector3(scaleX.Value, scaleY.Value, scaleZ.Value);
		gameObject.transform.localScale = scale;
	}

	public void ScaleUpNow()
	{
		InstantiateSmoothMovers();
		scaleX.Start();
		scaleY.Start();
		scaleZ.Start();
	}

	public void DoubleRampSize()
	{
		if (weHaveAlreadyDoubledTheSize)
			return;

		scaleX.Reset();
		scaleY.Reset();
		scaleZ.Reset();
		scaleX.EndValue = xScaleFinal;
		scaleY.EndValue = yScaleFinal * 2;
		scaleZ.EndValue = zScaleFinal * 2;
		scaleX.Start();
		scaleY.Start();
		scaleZ.Start();
		
		weHaveAlreadyDoubledTheSize = true;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (Tags.IsObstacle(collision.gameObject))
			GameLogic.RandomBlowUp(collision.gameObject, BlackHole);
	}
}
