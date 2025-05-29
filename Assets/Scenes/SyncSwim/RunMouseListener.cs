using UnityEngine;

public class RunMouseListener : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    // sprites
    [SerializeField] private SpriteRenderer spriteRenderer;
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
        Debug.Log(gameObject.name + ": clicked, level: " + sceneController.levelSelected);
        if (sceneController.levelSelected == 0)
            Debug.Log("doing nothing");
        else
            sceneController.dive();
    }
}