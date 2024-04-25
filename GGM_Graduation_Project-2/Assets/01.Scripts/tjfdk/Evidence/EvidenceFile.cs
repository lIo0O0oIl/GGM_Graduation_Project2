using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Evidence")]
    [SerializeField] private bool isUseable;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string msg;
    [SerializeField] private string type = "Image";

    public bool IsUseable => isUseable;
    public Sprite Spriet => sprite;
    public string Msg => msg;
    public string Type => type;


    public GameObject copy;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        previousPosition = this.transform.position;
        parent = this.transform.parent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            this.gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        copy = Instantiate(this.gameObject);
        Destroy(copy.transform.GetChild(0).gameObject);

        copy.transform.position = this.transform.position;
        copy.transform.SetParent(canvas);
        copy.transform.SetSiblingIndex(1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        copy.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(copy);
    }
}
