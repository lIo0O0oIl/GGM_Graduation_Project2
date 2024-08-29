using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RSLinkedData : MonoBehaviour
{
    private UIReader_Relationship mainSystem;
    private LineRenderer lineRenderer;
    public List<GameObject> linkdataList = new List<GameObject>();

    private GameObject movedDot;
    private BoxCollider2D movedDotCollider;
    private Vector2 dotOriginPos;
    private bool is_Hold = false;
    private float startPosX, startPosY;

    public GameObject touchObj;
    public bool is_LinkedObject = false;

    private void Awake()
    {
        mainSystem = GetComponentInParent<UIReader_Relationship>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnMouseDown()
    {
        if (movedDot == null)
        {
            movedDot = Instantiate(mainSystem.linkLineDotPrefabs, transform.position, Quaternion.identity, gameObject.transform);
            movedDot.GetComponent<RSDot>().myData = this;
            movedDotCollider = movedDot.GetComponent<BoxCollider2D>();
            dotOriginPos = movedDot.transform.localPosition;
        }
        else
        {
            movedDotCollider.enabled = true;
        }

        Vector3 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        startPosX = mousePos.x - movedDot.transform.position.x;
        startPosY = mousePos.y - movedDot.transform.position.y;

        lineRenderer.SetPosition(0, transform.position);

        is_Hold = true;
    }

    private void OnMouseUp()
    {
        movedDot.transform.localPosition = dotOriginPos;     // 원위치

        if (is_LinkedObject == true && linkdataList.Contains(touchObj) == false)
        {
            Debug.Log("새로 연결해야함.");
            lineRenderer.SetPosition(1, touchObj.transform.position);
            linkdataList.Add(touchObj);
            touchObj.GetComponent<RSLinkedData>().linkdataList.Add(gameObject);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position);
        }

        movedDotCollider.enabled = false;
        is_Hold = false;
    }

    private void Update()
    {
        if (is_Hold)
        {
            Vector2 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 movedPos = new Vector2(mousePos.x - startPosX, mousePos.y - startPosY);

            movedDot.transform.position = movedPos;

            lineRenderer.SetPosition(1, LinePosition(movedPos));
        }
    }

    private Vector2 LinePosition(Vector2 originPos)
    {
        return new Vector2(originPos.x - 0.1f, originPos.y - 0.2f);
    }
}
