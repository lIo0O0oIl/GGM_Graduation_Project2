using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SudokuTile : MonoBehaviour
{
    private int number = 0;
    private int correct = 0;
    public TextMeshProUGUI text;
    public Button button;

    private Image image;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void SetNumber(int _number)
    {
        number = _number;
        text.text = number.ToString();
    }

    public int GetNumber()
    {
        return number;
    }

    public void SetCorrect(int _correct)
    {
        correct = _correct;
    }

    public int GetCorrect()
    {
        return correct;
    }

    public void SetColor(Color _color)
    {
        image.color = _color;
    }

    public void SetTxtColor(Color _color)
    {
        text.color = _color;
    }

    public void SetLook()
    {
        button.interactable = false;
    }

    public void CheckAnwser()
    {
        if (number == correct)
        {
            SetTxtColor(Color.green);
            SetLook();
            StartCoroutine(DelayColor());
        }
        else
        {
            SetColor(Color.red);
            SetNumber(0);
            StartCoroutine(DelayColor());
        }
    }

    IEnumerator DelayColor()
    {
        yield return new WaitForSeconds(0.5f);
        SetColor(Color.white);
    }
}
