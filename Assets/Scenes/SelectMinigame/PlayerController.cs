using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 10f;
	private float objectWidth;
	private float objectHeight;

	// animator changes sprite based on location and movement
	private Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();
		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		// get sprite bounding box dimensions
		objectWidth = sr.bounds.extents.x;
		objectHeight = sr.bounds.extents.y;
	}

	// once per frame
	void Update()
	{
		// get input
		float xinput = Input.GetAxis("Horizontal");
		float yinput = Input.GetAxis("Vertical");

		// sprite animation on player input
		animator.SetBool("moving", (xinput != 0 || yinput != 0));
		// set sprite on object position
		animator.SetBool("swimming", (transform.position.y <= 0));

		// calculate player position
		Vector3 newPosition = transform.position + new Vector3(xinput, yinput, 0) * moveSpeed * Time.deltaTime;

		// clamp position to playable space
		newPosition.x = Mathf.Clamp(newPosition.x, -15.5f+objectWidth, 15.5f-objectWidth);
		newPosition.y = Mathf.Clamp(newPosition.y, -8.5f+objectHeight, 8.5f-objectHeight);



		// set position
		transform.position = newPosition;
	}
}