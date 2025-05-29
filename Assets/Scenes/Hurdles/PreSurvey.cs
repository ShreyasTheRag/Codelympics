using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreSurvey : MonoBehaviour
{
	[SerializeField] private TMP_InputField varRating, ifRating, whileRating, overallRating;
	[SerializeField] private Button beginButton, mainMenuButton;

	// Start is called before the first frame update
	void Start()
	{
		beginButton.onClick.RemoveAllListeners();
		beginButton.onClick.AddListener(saveRatingsAndStart);
		mainMenuButton.onClick.RemoveAllListeners();
		mainMenuButton.onClick.AddListener(Exit);
	}

	private void saveRatingsAndStart() {
		try
		{
			byte[] ratings = { byte.Parse(varRating.text), byte.Parse(ifRating.text), byte.Parse(whileRating.text), byte.Parse(overallRating.text) };
			foreach (byte rating in ratings) {
				if (!(rating >= 1 && rating <= 5)) throw new FormatException();
			}
			File.WriteAllText("pregamesurvey.txt", "" + varRating.text + '\n' + ifRating.text + '\n' + whileRating.text + '\n' + overallRating.text);
			SceneManager.LoadScene("Hurdles 1");
		}
		catch (FormatException)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		catch (OverflowException)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	private void Exit()
	{
		SceneManager.LoadScene("SelectMinigame");
	}
}
