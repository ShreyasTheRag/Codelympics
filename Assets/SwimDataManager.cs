using System.IO; // save files
using System.Linq; // string[] .Select()
using TMPro; // textmeshpro
using UnityEngine;
using UnityEngine.SceneManagement; // loading scenes
using UnityEngine.UI; // buttons

/**
"SaveData" scene
manage SyncSwim save data
*/
public class SwimDataManager : MonoBehaviour
{
	[SerializeField] private Button loadButton, deleteButton;
	[SerializeField] private TextMeshProUGUI statusText;
	[SerializeField] private TextMeshProUGUI loadButtonText, deleteButtonText;
	private bool saveDataExists;
    private int levelsComplete = 0;
	private string txt = "No Data"; // status text

	// Start is called before the first frame update
	void Start()
	{
		loadButton.onClick.RemoveAllListeners();
		deleteButton.onClick.RemoveAllListeners();
		loadButton.onClick.AddListener(LoadDataFromHurdlesSaveFile);
		deleteButton.onClick.AddListener(WarnBeforeDeleting);
		saveDataExists = File.Exists("swimData.txt");

		if (saveDataExists)
		{
			// extract data from swimData and swimQuiz
            bool[] swimData = File.ReadAllLines("swimData.txt").Select(line => bool.Parse(line)).ToArray();
			int[] scores = File.ReadAllLines("swimQuiz.txt").Select(line => int.Parse(line)).ToArray();

			// count # levels completed
            foreach(bool datum in swimData)
                if (datum) ++levelsComplete;

			// has taken postquiz
			if (scores.Length == 2)
				txt = $"All levels complete\nPrequiz score: {scores[0]}, Postquiz score: {scores[1]}";
			else
				txt = $"{levelsComplete} levels complete\nPrequiz score: {scores[0]}";
		}
		
		statusText.text = txt; // set it
	}

	// read save data for syncswim
	// if there is data, go to minigame
	// otherwise select minigame
	private void LoadDataFromHurdlesSaveFile() {
		SceneManager.LoadScene(saveDataExists ? "SyncSwim" : "SelectMinigame");
	}

	// change listeners on buttons for confirmation
	// only if there is save data to delete
	private void WarnBeforeDeleting()
	{
		if (saveDataExists)
		{
			loadButton.onClick.RemoveAllListeners();
			deleteButton.onClick.RemoveAllListeners();
			loadButton.onClick.AddListener(RevertBack);
			deleteButton.onClick.AddListener(DeleteHurdlesSaveFile);
			statusText.text = "Delete saved data?";
			loadButtonText.text = "No";
			deleteButtonText.text = "Yes";
		}
	}

	// deciding not to delete swimData
	private void RevertBack() {
		loadButton.onClick.RemoveAllListeners();
		deleteButton.onClick.RemoveAllListeners();
		loadButton.onClick.AddListener(LoadDataFromHurdlesSaveFile);
		deleteButton.onClick.AddListener(WarnBeforeDeleting);
		loadButtonText.text = "Load";
		deleteButtonText.text = "Delete";
		statusText.text = txt; // set it
	}

	// delete swimData
	private void DeleteHurdlesSaveFile() {
		File.Delete("swimData.txt");
		File.Delete("swimQuiz.txt");
		saveDataExists = false;
		RevertBack();
		txt = "No data";
		statusText.text = txt; // set it
	}
}