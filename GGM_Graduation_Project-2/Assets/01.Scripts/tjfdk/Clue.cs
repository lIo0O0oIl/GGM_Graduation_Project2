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
    }
}
