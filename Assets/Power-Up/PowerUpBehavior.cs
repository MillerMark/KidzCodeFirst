using System;
using UnityEditor.SceneManagement;
using UnityEngine;
public class PowerUpBehavior : MonoBehaviour
{


	// Start is called before the first frame update
	public const string STR_PowerUp = "Power Up";
	void Start()
	{
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		GetComponent<Rigidbody>().AddTorque(new Vector3(0, 5 * Time.deltaTime, 0));
	}

	protected virtual void PowerUp(PlayerProperties playerScript)
	{
		Debug.Log("Virtual PowerUp called!");
	}
	void OnTriggerEnter(Collider collider)
	{
		bool weHitPlayer = Tags.IsPlayer(collider.gameObject);
		if (!weHitPlayer)
			return;

		GameObject player = collider.transform.gameObject;
		PlayerProperties playerScript = player.GetComponent<PlayerProperties>();
		PowerUp(playerScript);
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Contains("Track"))
		{
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			GetComponent<BoxCollider>().isTrigger = true;
			return;
		}
	}
}
