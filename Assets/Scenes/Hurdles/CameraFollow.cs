using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] public GameObject player;
	[SerializeField] public float smoothSpeed = 0.125f, cameraSpeed;
	[SerializeField] public Vector3 offset;
	private Transform playerTransform;
	private Interpreter interpreter;
	private ProgrammingCanvas canvasScript;
	private bool followPlayer;
	// Start is called before the first frame update
	void Start()
	{
		playerTransform = player.transform;
		interpreter = player.GetComponent<Interpreter>();
		canvasScript = GameObject.Find("Programming Canvas").GetComponent<ProgrammingCanvas>();
		followPlayer = true;
	}

	void Update()
	{
		if (interpreter.running || canvasScript.IsShowing())
		{
			followPlayer = true;
		}
		else {
			followPlayer = false;
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				transform.position = new Vector3(transform.position.x - cameraSpeed, transform.position.y, transform.position.z);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				transform.position = new Vector3(transform.position.x + cameraSpeed, transform.position.y, transform.position.z);
			}
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (followPlayer) {
			// Desired position of the camera
			Vector3 desiredPosition = new Vector3(playerTransform.position.x + offset.x, transform.position.y, transform.position.z);

			// Smoothly interpolate to the desired position
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

			// Update the camera's position
			transform.position = smoothedPosition;
		}
	}
}
