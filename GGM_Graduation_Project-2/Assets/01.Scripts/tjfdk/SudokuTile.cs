using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SudokuTile : MonoBehaviour
{
    [SerializeField] private int number = 0;
    [SerializeField] private int correct = 0;
    public TextMeshProUGUI text;
    public Button button;

    private Image image;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void SetNumber(int _number = 0)
    {
        number = _number;
        if (_number != 0)
            text.text = _number.ToString();
        else
            text.text = "";
        button.interactable = true;
    }

    public int GetNumber()
    {
        return number;
    }

    public void SetCorrect(int _correct)
    {
        correct = _correct;
        text.text = _correct.ToString();
        button.interactable = false;
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
            SetColor(Color.green);
            StartCoroutine(DelayColor());
            SetLook();
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
