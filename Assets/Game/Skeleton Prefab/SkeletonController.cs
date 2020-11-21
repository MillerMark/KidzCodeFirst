using UnityEngine;

public class SkeletonController : MonoBehaviour
{
	// reference to our skeleton anim
	private Animator anim;

	// Start is called before the first frame update
	void Start()
	{
		// get a reference to our attached animation for playing via public methods
		anim = GetComponent<Animator>();
	}

	// public method connected to UI button click event
	public void Death()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Death");
	}

	// public method connected to UI button click event
	public void Attack()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Attack");
	}

	// public method connected to UI button click event
	public void Walk()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Walk");
	}

	// public method connected to UI button click event
	public void Stand()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Stand");
	}

	// public method connected to UI button click event
	public void Skill()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Skill");
	}

	// public method connected to UI button click event
	public void Run()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Run");
	}

	// public method connected to UI button click event
	public void Damage()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Damage");
	}

	// public method connected to UI button click event
	public void Knockback()
	{
		// the stateName here has to match the name of the state in the Animator window exactly
		anim.Play("Knockback");
	}
}
