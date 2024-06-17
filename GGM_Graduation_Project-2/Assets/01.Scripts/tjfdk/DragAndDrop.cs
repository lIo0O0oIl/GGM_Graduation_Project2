using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Outline outline;

    [SerializeField] private bool canDrag = false;
    private Vector2 dragStartPosition;
    private Vector2 dragEndPosition;

    [SerializeField] private LineRenderer line; // 현재 그려지고 있는 선
    [SerializeField] private Material lineMaterial; // 선을 그리기 위한 프리팹
    [SerializeField] private int currentLineCnt; // 현재 선 개수
    [SerializeField] private Transform lineParent; // 현재 선 개수
    private Vector3 mousePos;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        outline = GetComponent<Outline>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragEndPosition = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            //Debug.Log(dragStartPosition +  " " + dragEndPosition);
            UpdateLine();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canDrag = !canDrag;
        outline.enabled = !outline.IsActive();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPosition = Input.mousePosition;

        StartLine();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canDrag = false;
        outline.enabled = false;

        EndLine();
    }

    private void StartLine()
    {
        if (line == null)
        {
            line = new GameObject("Line " + currentLineCnt).AddComponent<LineRenderer>();
            line.transform.SetParent(lineParent);
            line.material = lineMaterial;
            line.positionCount = 2;
            line.startWidth = 1f;
            line.endWidth = 1f;
            line.useWorldSpace = true;
            line.numCapVertices = 50;
        }

        mousePos = Input.mousePosition;
        mousePos.z = 0;
        line.SetPosition(0, mousePos);
        line.SetPosition(1, mousePos);
    }

    private void UpdateLine()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 0;
        line.SetPosition(1, mousePos);
    }

    private void EndLine()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 0;
        line.SetPosition(1, mousePos);
        line = null;
        currentLineCnt++;
    }
}
