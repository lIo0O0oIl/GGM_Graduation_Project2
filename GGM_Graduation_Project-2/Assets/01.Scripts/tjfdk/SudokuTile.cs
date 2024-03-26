using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.UI;

public class SudokuTile : MonoBehaviour
{
    //[SerializeField] private int number = 0;
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
        //button = GetComponent<Button>();
    }

    //public bool IsEnd()
    //{
    //    return isEnd;
    //}

    public void InputNumber()
    {
        if (input.text != "")
        {
            //SetNumber(int.Parse(input.text));
            //input(int.Parse(input.text));
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

    //public void SetNumber(int _number = 0)
    //{
    //    number = _number;

    //    if (_number != 0)
    //        text.text = _number.ToString();
    //    else
    //        text.text = "";

    //    input.interactable = false;
    //}

    //public int GetNumber()
    //{
    //    return number;
    //}

    //public void SetCorrect(int _correct)
    //{
    //    correct = _correct;
    //    text.text = _correct.ToString();
    //    input.interactable = false;
    //}

    public int GetCorrect()
    {
        return correct;
    }

    public void SetColor(Color _color)
    {
        backGround.color = _color;
    }

    public void SetTxtColor(Color _color)
    {
        //text.color = _color;
    }

    //public void SetLook()
    //{
    //    //button.interactable = false;
    //    isEnd = true;
    //}

    public void CheckAnwser()
    {
        if (int.Parse(input.text) == correct)
        {
            SetColor(Color.green);
            StartCoroutine(DelayColor());
            open();
            //SetLook();
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
