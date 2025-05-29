/**
 * change scene upon colliding with player
 */
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalCollision : MonoBehaviour
{
	public string nextScene;

	/** built-in function called when `other` collides with trigger bounds */
	private void OnTriggerEnter2D(Collider2D other)
	{
		// check if colliding object has "Player" tag
		if (other.CompareTag("Player"))
		{
			// behavior depending on scene
			switch (nextScene)
			{
				case "Hurdles":
					if (File.Exists("hurdlessave.txt"))
					{
						SceneManager.LoadScene("Hurdles " + (int.Parse(Regex.Split(File.ReadAllText("hurdlessave.txt"), "[$]")[0])));
					}
					else
					{
						SceneManager.LoadScene("Hurdles Pre Game Survey");
					}
					break;
				case "SyncSwim":
					if (File.Exists("swimData.txt"))
						SceneManager.LoadScene("SyncSwim");
					else
						SceneManager.LoadScene("QuizSyncSwim");
					break;
				default:
					SceneManager.LoadScene(nextScene);
					break;
			}
		}
	}
}