using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSMovedPicture : MonoBehaviour
{
    private bool is_Hold = false;
    private float startPosX, startPosY;

    private RSLinkedData myLinkedData;
    public ScreenClamp screenClamp;

    private void Awake()
    {
        myLinkedData = transform.GetChild(0).GetComponent<RSLinkedData>();
    }

    private void OnMouseDown()
    {
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

            myLinkedData.ChangeOtherLinePosition();
        }

        float clampedX = Mathf.Clamp(transform.localPosition.x, screenClamp.xMin, screenClamp.xMax);
        float clampedY = Mathf.Clamp(transform.localPosition.y, screenClamp.yMin, screenClamp.yMax);

        transform.localPosition = new Vector2(clampedX, clampedY);
    }
}
