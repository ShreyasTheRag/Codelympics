using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostSurvey : MonoBehaviour
{
	[SerializeField] private TMP_InputField varRating, ifRating, whileRating, overallRating;
	[SerializeField] private Button finishButton;
	// Start is called before the first frame update
	void Start()
	{
		finishButton.onClick.RemoveAllListeners();
		finishButton.onClick.AddListener(Exit);
	}

	private void Exit()
	{
		try
		{
			byte[] ratings = { byte.Parse(varRating.text), byte.Parse(ifRating.text), byte.Parse(whileRating.text), byte.Parse(overallRating.text) };
			foreach (byte rating in ratings)
			{
				if (!(rating >= 1 && rating <= 5)) throw new FormatException();
			}
			File.WriteAllText("postgamesurvey.txt", "" + varRating.text + '\n' + ifRating.text + '\n' + whileRating.text + '\n' + overallRating.text);
			SceneManager.LoadScene("SelectMinigame");
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
}
