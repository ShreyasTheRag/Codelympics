using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
used in the "if" block in Synchronized Swimming
depending on the first dropdown (height or color), choose which dropdown to dispaly
*/
public class ifScript : MonoBehaviour
{
    // called whenever the first dropdown's value changes
    public void ActivateDropdown()
    {
        int i = transform.Find("Dropdown").GetComponent<TMP_Dropdown>().value;

        // height
        if (i == 0)
        {
            transform.Find("height").gameObject.SetActive(true);
            transform.Find("color").gameObject.SetActive(false);
        }
        // color
        else
        {
            transform.Find("height").gameObject.SetActive(false);
            transform.Find("color").gameObject.SetActive(true);
        }
    }
}
