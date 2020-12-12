using System;
public class SuperSize : PowerUpBehavior
{
	protected override void PowerUp(PlayerProperties playerScript)
	{
		playerScript.ScaleUp();
	}
	public SuperSize()
	{

	}
}

