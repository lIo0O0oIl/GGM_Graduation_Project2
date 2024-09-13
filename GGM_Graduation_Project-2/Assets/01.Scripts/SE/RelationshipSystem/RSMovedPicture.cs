using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSMovedPicture : MonoBehaviour
{
    private RSEvidenceData myEvidenceData;

    private bool is_Hold = false;
    private float startPosX, startPosY;
    public ScreenClamp screenClamp;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.5f;

    private void Awake()
    {
        myEvidenceData = transform.GetChild(0).GetComponent<RSEvidenceData>();
    }

    private void OnMouseDown()
    {
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            Debug.Log("더블클릭 : " + myEvidenceData.fileType);
            return;
        }
        else lastClickTime = Time.time;

            Vector3 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        startPosX = mousePos.x - transform.position.x;
        startPosY = mousePos.y - transform.position.y;

        is_Hold = true;
    }

    private void OnMouseUp()
    {
        is_Hold = false;
    }

    private void Update()
    {
        if (is_Hold)
        {
            Vector2 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 movedPos = new Vector2(mousePos.x - startPosX, mousePos.y - startPosY);
            transform.position = movedPos;

            myEvidenceData.ChangeOtherLinePosition();
        }

        float clampedX = Mathf.Clamp(transform.localPosition.x, screenClamp.xMin, screenClamp.xMax);
        float clampedY = Mathf.Clamp(transform.localPosition.y, screenClamp.yMin, screenClamp.yMax);

        transform.localPosition = new Vector2(clampedX, clampedY);
    }
}
