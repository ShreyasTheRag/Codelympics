using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
used in `MainMenu` scene
depending on button, when selected open corresponding scene
*/
public class LoadMenu : MonoBehaviour
{
	[SerializeField] public Button button;
	[SerializeField] public string scene;

	// Start is called before the first frame update
	void Start()
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(OnClick);
		Debug.Log("Added event listener");
	}

	private void OnClick()
	{
		SceneManager.LoadScene(scene);
	}
}
