using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    // (set in Inspector) retrieve public variables
    [SerializeField] private SceneController sceneController;
    // choose sprites
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Sprite normal, highlighted, selected, finished;

    private int level;

    // ex: "1", "2"
    // awake: NEED THIS to run before SceneController's Start()
    void Awake ()
    {
        level = int.Parse(gameObject.name);
    }

    // called by scene controller
    public void select ()
    {
        if (!sceneController.levelComplete[level])
            spriteRenderer.sprite = selected;
        else
            spriteRenderer.sprite = finished;
    }

    // called by scene controller
    // case 1: level is de-selected
    // case 2: on first scene load
    public void deselect ()
    {
        if (!sceneController.levelComplete[level])
            spriteRenderer.sprite = normal;
        else
            spriteRenderer.sprite = finished;

        Debug.Log($"deselected {level}");
    }

    void OnMouseDown ()
    {
        sceneController.changeLevel(level, true);
    }

    // if not complete and not selected
    void OnMouseEnter ()
    {
        if (!sceneController.levelComplete[level] && sceneController.levelSelected != level)
            spriteRenderer.sprite = highlighted;
    }

    void OnMouseExit ()
    {
        if (!sceneController.levelComplete[level] && sceneController.levelSelected != level)
            spriteRenderer.sprite = normal;
    }
}