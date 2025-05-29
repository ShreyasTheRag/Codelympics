using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // save files

public class ExitMouseListener : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SceneController SceneController;
    public Sprite exit, exith;

    private void OnMouseEnter ()
    {
        Debug.Log(gameObject.name + ": mouse in");

        spriteRenderer.sprite = exith;
    }

    private void OnMouseExit ()
    {
        Debug.Log(gameObject.name + ": mouse out");

        spriteRenderer.sprite = exit;
    }    

    // on mouse click: log and change scenes
    private void OnMouseDown ()
    {
        Debug.Log(gameObject.name + ": clicked");

        // todo if all levels are finished, and user hasn't yet done post-survey, go to post survey
        // length == 1 if postquiz not taken yet
        // Debug.Log(File.ReadAllLines("swimQuiz.txt").Length);
        if (SceneController.allLevelsComplete && File.ReadAllLines("swimQuiz.txt").Length == 1)
            SceneManager.LoadScene("QuizSyncSwim");
        else
            SceneManager.LoadScene("SelectMinigame");
    }    
}