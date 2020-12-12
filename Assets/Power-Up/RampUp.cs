using System;
using UnityEngine;

public class RampUp : PowerUpBehavior
{
	protected override void PowerUp(PlayerProperties playerScript)
	{
		playerScript.AddRamp();
	}
	public RampUp()
	{

	}
}

