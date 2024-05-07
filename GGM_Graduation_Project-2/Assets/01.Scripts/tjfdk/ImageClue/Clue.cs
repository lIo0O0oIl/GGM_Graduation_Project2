using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string msg;
    [SerializeField] private bool isEnd;
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void ClickClue()
    {
        ClueManager.Instance.Texting(msg);
        isEnd = true;
        outline.enabled = false;
        this.GetComponent<Button>().interactable = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnd == false)
            outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnd == false)
            outline.enabled = false;
    }
}
