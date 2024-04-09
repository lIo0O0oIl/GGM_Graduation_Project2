using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvidenceFile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 만들어진 이미지, 텍스트 파일에 컴포넌트로 추가할 예정

    [Header("Drag And Drop")]
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 previousPosition;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Button button;

    public GameObject temp;

    [Header("Evidence")]
    [SerializeField] private bool isUseable;
    public bool IsUseable => isUseable;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        previousPosition = rect.transform.position;
        parent = rect.parent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            this.gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        rect.SetParent(canvas);
        //rect.SetAsFirstSibling();
        rect.SetSiblingIndex(2);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("tttt");
        temp.SetActive(true);

        Debug.Log(eventData.pointerEnter.name);
        //tt = eventData.pointerEnter;

        rect.SetParent(parent);
        rect.transform.position = previousPosition;
    }
}
