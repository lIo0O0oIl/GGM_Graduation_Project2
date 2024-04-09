using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropEvidence : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rect;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.GetComponent<EvidenceFile>().IsUseable)
                Debug.Log("아앗 개 지리는 단서다!");
            else
                Debug.Log("크킄 의미 없는 단서죠?");
        }
    }
}
