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
    public bool isAnwser = true;

    public List<Transform> buttons = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; ++i)
        {
            transform.GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            buttons.Add(transform.GetChild(0).GetChild(i));
        }
    }

    public void InputNumber()
    {
        int num = int.Parse(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text);
        focusTile.SetNumber(num);
        if (isAnwser)
        {
            focusTile.SetTxtColor(Color.black);
            focusTile.CheckAnwser();
        }
        else
        {
            focusTile.SetTxtColor(Color.gray);
        }
    }
    
    public void FocusTile()
    {
        if (focusTile)
            focusTile.SetColor(Color.white);
        focusTile = EventSystem.current.currentSelectedGameObject.GetComponent<SudokuTile>();
        focusTile.SetColor(Color.yellow);
    }

    public void ChangeMode()
    {
        isAnwser = !isAnwser;
    }
}
