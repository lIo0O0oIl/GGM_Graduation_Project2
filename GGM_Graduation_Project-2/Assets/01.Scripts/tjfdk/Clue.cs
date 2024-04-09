using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clue : MonoBehaviour
{
    [SerializeField] private string msg;

    public void ClickClue()
    {
        ClueManager.Instance.Texting(msg);
        this.GetComponent<Button>().interactable = false;

        //Color color;
        //UnityEngine.ColorUtility.TryParseHtmlString("#A4A4A4", out color);
        //this.GetComponent<Image>().color = color;
    }
}
