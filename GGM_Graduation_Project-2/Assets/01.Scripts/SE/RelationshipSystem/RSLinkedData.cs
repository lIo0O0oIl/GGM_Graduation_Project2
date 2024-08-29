using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RSLinkedData : MonoBehaviour
{
    // Main
    private UIReader_Relationship mainSystem;
    public List<GameObject> linkdataList = new List<GameObject>();
    private LineRenderer lineRenderer;

    // Line
    private GameObject movedDot;
    private BoxCollider2D movedDotCollider;
    private EdgeCollider2D lineEdgeCollider;
    private List<Vector2> linePosList = new List<Vector2>();

    private Vector2 dotOriginPos;
    private bool is_Hold = false;
    private float startPosX, startPosY;
    private int nowLineIndex = 0;

    public GameObject touchObj;
    public bool is_LinkedObject = false;

    private void Awake()
    {
        mainSystem = GetComponentInParent<UIReader_Relationship>();
        lineRenderer = GetComponent<LineRenderer>();
        lineEdgeCollider = GetComponent<EdgeCollider2D>();
        linePosList.Add(new Vector2(0, 0));
    }

    private void OnMouseDown()
    {
        Debug.Log("날 누름");

        Vector3 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log(Vector2.Distance(mousePos, transform.position));
        if (Vector2.Distance(mousePos, transform.position) > 0.1f)
        {
            Debug.Log("선을 건듦");
            // 마우스와 가장 가까운 선 포지션을 찾음. 자기 빼고 말야.
            // 걔를 가져와서 옮겨주기
        }
        //else { return; }

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

        startPosX = mousePos.x - movedDot.transform.position.x;
        startPosY = mousePos.y - movedDot.transform.position.y;

        lineRenderer.SetPosition(nowLineIndex, transform.position);

        is_Hold = true;
    }

    private void OnMouseUp()
    {
        movedDot.transform.localPosition = dotOriginPos;     // 원위치

        if (is_LinkedObject == true && linkdataList.Contains(touchObj) == false)
        {
            lineRenderer.SetPosition(nowLineIndex + 1, touchObj.transform.position);
            linePosList.Add(transform.InverseTransformPoint(touchObj.transform.position));
            linePosList.Add(new Vector2(0, 0));

            linkdataList.Add(touchObj);
            touchObj.GetComponent<RSLinkedData>().linkdataList.Add(gameObject);

            lineRenderer.positionCount += 2;
            nowLineIndex += 2;
            
            lineRenderer.SetPosition(nowLineIndex, transform.position);
            lineRenderer.SetPosition(nowLineIndex + 1, transform.position);
            lineEdgeCollider.points = linePosList.ToArray();
        }
        else
        {
            lineRenderer.SetPosition(nowLineIndex + 1, transform.position);
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

            lineRenderer.SetPosition(nowLineIndex + 1, LinePosition(movedPos));
        }
    }

    private Vector2 LinePosition(Vector2 originPos)
    {
        return new Vector2(originPos.x - 0.1f, originPos.y - 0.2f);
    }
}
