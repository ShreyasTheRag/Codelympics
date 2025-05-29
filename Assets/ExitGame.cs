using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGame : MonoBehaviour
{
	[SerializeField] public Button button;
	// Start is called before the first frame update
	void Start()
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(OnClick);
		Debug.Log("Added event listener");
	}

	// when the exit button is clicked, close the game
	public void OnClick() {
		Debug.Log("Exit Game");
		Application.Quit(0);
	}
}
