using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.anyKeyDown) {
			SceneManager.LoadScene("MainMenu");
		}
	}
}
