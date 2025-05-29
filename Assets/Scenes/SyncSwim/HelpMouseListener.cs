using UnityEngine;

public class HelpMouseListener : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject infoBox;
    public Sprite normal, highlight;

    private void OnMouseEnter ()
    {
        Debug.Log(gameObject.name + ": mouse in");

        spriteRenderer.sprite = highlight;
    }

    private void OnMouseExit ()
    {
        Debug.Log(gameObject.name + ": mouse out");

        spriteRenderer.sprite = normal;
    }    

    // on mouse click: log and change scenes
    private void OnMouseDown ()
    {
        Debug.Log(gameObject.name + ": clicked");
        infoBox.SetActive(true);
        // SceneManager.LoadScene("SelectMinigameLevel");
    }    
}