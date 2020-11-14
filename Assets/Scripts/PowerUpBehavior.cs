using UnityEditor.SceneManagement;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		GetComponent<Rigidbody>().AddTorque(new Vector3(0, 5 * Time.deltaTime, 0));
	}

	void OnTriggerEnter(Collider someObject)
	{
		if (someObject.transform.gameObject.name == "Player")
		{
			GameObject player = someObject.transform.gameObject;
			PlayerProperties playerProperties = player.GetComponent<PlayerProperties>();
			playerProperties.PowerUp();
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Contains("Track"))
		{
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<BoxCollider>().isTrigger = true;
			return;
		}

		if (collision.gameObject.name.Contains("Player"))
		{
			Debug.Log("We just hit the player!!!");
			return;
		}
	}
}
