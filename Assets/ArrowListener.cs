using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrowListener : MonoBehaviour
{
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    [SerializeField] private Transform canv;

    void Start ()
    {
        left.onClick.AddListener(leftC);
        right.onClick.AddListener(rightC);
    }

    public void leftC ()
    {
        Debug.Log(gameObject.name);
        gameObject.SetActive(false);

        int x = int.Parse(gameObject.name) - 1;
        Debug.Log(""+x);
        canv.Find(""+x).gameObject.SetActive(true);
    }

    public void rightC ()
    {
        Debug.Log(gameObject.name);
        gameObject.SetActive(false);

        if (gameObject.name != "4")
        {
            int x = int.Parse(gameObject.name) + 1;
            Debug.Log(""+x);
            canv.Find(""+x).gameObject.SetActive(true);
        }
    }
}
