using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{
	[SerializeField] private Button newGameButton;

	// Start is called before the first frame update
	void Start()
	{
		newGameButton.onClick.RemoveAllListeners();
		newGameButton.onClick.AddListener(Load);
	}

	// when 'newGameButton' is pressed
	// delete previous save files
	// scene -> "SelectMinigame"
	private void Load()
	{
		if (gameObject.GetComponentInChildren<TextMeshProUGUI>().text == "Are you sure?")
		{
			// hurdles
			File.Delete("hurdlessave.txt");
			File.Delete("pregamesurvey.txt");
			File.Delete("postgamesurvey.txt");
			// sync swim
			File.Delete("swimData.txt");
			File.Delete("swimQuiz.txt");
			// load next scene
			SceneManager.LoadScene("SelectMinigame");
		}
		else
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Are you sure?";
	}
}