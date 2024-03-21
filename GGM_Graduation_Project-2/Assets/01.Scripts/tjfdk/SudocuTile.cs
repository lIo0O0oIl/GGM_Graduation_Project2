using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SudocuTile : MonoBehaviour
{
    public int number = 0;
    public int correct = 0;
    public TextMeshProUGUI text;
    public Button button;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void SetNumber(int _number)
    {
        number = _number;
        text.text = number.ToString();
    }

    public void SetCorrect(int _correct)
    {
        correct = _correct;
    }

    public void SetActive()
    {
        button.interactable = !button.interactable;
    }
}
