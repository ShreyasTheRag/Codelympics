using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
"SaveGame" scene
manage hurdles save data
*/
public class HurdlesSaveDataManager : MonoBehaviour
{
	[SerializeField] private Button loadButton, deleteButton;
	[SerializeField] private TextMeshProUGUI statusText, preGamePostGameText;
	[SerializeField] private TextMeshProUGUI loadButtonText, deleteButtonText;
	private const string EMPTY_FILE = "No Data";
	private bool saveDataExists, load;
	private string levelString, surveyString;
	private int levelNumber;

	// Start is called before the first frame update
	void Start()
	{
		loadButton.onClick.RemoveAllListeners();
		deleteButton.onClick.RemoveAllListeners();
		loadButton.onClick.AddListener(LoadDataFromHurdlesSaveFile);
		deleteButton.onClick.AddListener(WarnBeforeDeleting);
		saveDataExists = File.Exists("hurdlessave.txt");
		load = true;
		if (saveDataExists)
		{
			levelNumber = int.Parse(Regex.Split(File.ReadAllText("hurdlessave.txt"), "[$]")[0]);
			Debug.Log(levelNumber);
			if (levelNumber >= 7) {
				statusText.text = "Complete";
				load = false;
			} else {
				statusText.text = "Level " + levelNumber;
			}
		}
		else {
			levelNumber = 0;
			statusText.text = EMPTY_FILE;
		}
		byte[] surveyScores = { 0, 0 };
		string[] surveyFiles = { "pregamesurvey.txt", "postgamesurvey.txt" };
		for (byte i = 0; i < surveyScores.Length; i++)
		{
			if (File.Exists(surveyFiles[i]))
			{
				foreach (string score in Regex.Split(File.ReadAllText(surveyFiles[i]), "[\r\n]"))
				{
					surveyScores[i] += byte.Parse(score);
				}
			}
		}
		surveyString = "Pregame Survey Score: " + (surveyScores[0] == 0 ? "N/A" : surveyScores[0]) + ", Postgame Survey Score: " + (surveyScores[1] == 0 ? "N/A" : surveyScores[1]);
		if (saveDataExists)
			preGamePostGameText.text = surveyString;
	}

	// read the save file (txt) for hurdles data
	private void LoadDataFromHurdlesSaveFile() {
		if (saveDataExists && load)
			SceneManager.LoadScene("Hurdles " + levelNumber);
		else
			SceneManager.LoadScene("SelectMinigame");
	}

	// change listeners on buttons for confirmation
	// only if there is save data to delete
	private void WarnBeforeDeleting()
	{
		if (saveDataExists)
		{
			levelString = statusText.text;
			loadButton.onClick.RemoveAllListeners();
			deleteButton.onClick.RemoveAllListeners();
			loadButton.onClick.AddListener(RevertBack);
			deleteButton.onClick.AddListener(DeleteHurdlesSaveFile);
			statusText.text = "Delete Save File?";
			preGamePostGameText.text = "";
			loadButtonText.text = "No";
			deleteButtonText.text = "Yes";
		}
	}

	// deciding not to delete hurdles savedata
	private void RevertBack() {
		loadButton.onClick.RemoveAllListeners();
		deleteButton.onClick.RemoveAllListeners();
		loadButton.onClick.AddListener(LoadDataFromHurdlesSaveFile);
		deleteButton.onClick.AddListener(WarnBeforeDeleting);
		loadButtonText.text = "Load";
		deleteButtonText.text = "Delete";
		preGamePostGameText.text = surveyString;
		statusText.text = levelString;
	}

	// delete the file
	private void DeleteHurdlesSaveFile() {
		File.Delete("hurdlessave.txt");
		File.Delete("pregamesurvey.txt");
		File.Delete("postgamesurvey.txt");
		loadButtonText.text = "Load";
		deleteButtonText.text = "Delete";
		Start();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}