using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** sync swim pre and post quiz management */
// assume we are sent here because there is quiz to take, otherwise we wouldn't be here
public class QuizManager : MonoBehaviour
{
	// track current question and correct answers
	private int question = 0, score = 0;
	private int[] quizAnswers = {0, 0, 0, 3, 1, 1}; // dummy index 0
	private bool isPre;

	// check for save data
	// if none, this is pre quiz
	// if exists, this is post quiz
	void Start()
	{
		isPre = !File.Exists("swimQuiz.txt");

		// show postquiz title & description, hide exit button
		if (!isPre)
		{
			transform.Find("title").GetComponent<TextMeshProUGUI>().text = "Synchronized Swimming Postquiz";
			transform.Find("0").Find("preText").gameObject.SetActive(false);
			transform.Find("0").Find("postText").gameObject.SetActive(true);
			transform.Find("leftButton").gameObject.SetActive(false);
		}
	}

	// called by "rightButton"
	// hide buttons, show next question
	public void continueQuiz () {
		if (question != 5)
		{
			transform.Find(""+question).gameObject.SetActive(false);
			++question;
			transform.Find(""+question).gameObject.SetActive(true);

			transform.Find("leftButton").gameObject.SetActive(false);
			transform.Find("rightButton").gameObject.SetActive(false);
			transform.Find("rightButton").GetComponentInChildren<TextMeshProUGUI>().text = "Next";
		}
		else // quiz is over, write to file then load next scene
		{
			// either creates the file or adds to it (score)
			File.AppendAllText("swimQuiz.txt", $"{score}\n");
			SceneManager.LoadScene(isPre ? "SyncSwim" : "SelectMinigame");
		}
	}

	// called by answer bubble
	// change visuals, add to score tally
	public void answer (int choice)
	{
		// mark correct answer and make buttons not interactable
		foreach (Transform bubble in transform.Find(""+question))
		{
			bubble.gameObject.GetComponent<Button>().interactable = false;
		}
		transform.Find("rightButton").gameObject.SetActive(true);

		// red and green for chosen and correct choices
		transform.Find(""+question).Find(""+choice).gameObject.GetComponent<Image>().color = new Color(0.9f,0.2f,0.2f,1f);
		transform.Find(""+question).Find(""+quizAnswers[question]).gameObject.GetComponent<Image>().color = new Color(0.2f,0.8f,0.2f,1f);

		// if answer was correct, add to score
		if (quizAnswers[question] == choice)
			++score;

		// user can continue
		transform.Find("rightButton").gameObject.SetActive(true);
	}

	// called by "leftButton"
	// user decides not to take the quiz
	// only available for pre-quiz
	public void Exit()
	{
		SceneManager.LoadScene("SelectMinigame");
	}
}