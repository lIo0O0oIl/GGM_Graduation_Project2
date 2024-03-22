using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SudokuInput : MonoBehaviour
{
    public SudokuTile focusTile = null;
    public bool isAnswer = true;

    public GameObject currentButton;
    public GameObject oldButton;

    //public List<Transform> buttons = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; ++i)
        {
            transform.GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            //buttons.Add(transform.GetChild(0).GetChild(i));
        }
    }

    public void InputNumber()
    {
        if (currentButton != null && focusTile.IsEnd() == false)
        {
            int num = int.Parse(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text);
            focusTile.SetNumber(num);
            if (isAnswer)
            {
                focusTile.SetTxtColor(Color.black);
                focusTile.CheckAnwser();
            }
            else
            {
                focusTile.SetTxtColor(Color.red);
            }
        }
    }
    
    public void FocusTile()
    {
        if (focusTile)
            focusTile.SetColor(Color.white);
        focusTile = EventSystem.current.currentSelectedGameObject.GetComponent<SudokuTile>();
        focusTile.SetColor(Color.yellow);
    }

    public void ChangeMode(bool _isAnswer)
    {
        isAnswer = _isAnswer;

        oldButton = currentButton;
        currentButton = EventSystem.current.currentSelectedGameObject;
        if (oldButton)
            oldButton.GetComponent<Image>().color = Color.white;
        currentButton.GetComponent<Image>().color = Color.yellow;
    }
}
