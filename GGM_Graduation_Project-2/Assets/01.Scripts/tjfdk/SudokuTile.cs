using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SudokuTile : MonoBehaviour
{
    [SerializeField] private int correct = 0;

    public TextMeshProUGUI text;
    public TMP_InputField input;

    public bool isHide;

    private Image backGround;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        input = GetComponentInChildren<TMP_InputField>();
        backGround = GetComponent<Image>();
    }

    public void ClearSetting()
    {
        isHide = false;
        text.gameObject.SetActive(true);
        input.gameObject.SetActive(true);
        input.interactable = true;
    }

    public void InputNumber()
    {
        if (input.text != "")
        {
            CheckAnwser();
        }
    }

    public void Init(int _value)
    {
        correct = _value;
        text.text = _value.ToString();
        input.interactable = false;
    }

    public void Hide()
    {
        text.gameObject.SetActive(false);
        input.interactable = true;
        isHide = true;
    }

    public void open()
    {
        Sudoku.Instance.down();
        text.gameObject.SetActive(true);
        input.interactable = false;
        isHide = false;
    }
    public int GetCorrect()
    {
        return correct;
    }

    public void SetColor(Color _color)
    {
        backGround.color = _color;
    }

    public void CheckAnwser()
    {
        if (int.Parse(input.text) == correct)
        {
            SetColor(Color.green);
            StartCoroutine(DelayColor());
            open();
        }
        else
        {
            input.text = "";
            SetColor(Color.red);
            StartCoroutine(DelayColor());
        }
    }

    IEnumerator DelayColor()
    {
        yield return new WaitForSeconds(0.5f);
        SetColor(Color.white);
    }
}
