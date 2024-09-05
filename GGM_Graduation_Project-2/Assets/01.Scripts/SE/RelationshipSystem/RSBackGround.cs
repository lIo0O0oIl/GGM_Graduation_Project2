using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ScreenClamp
{
    public float xMin, xMax;
    public float yMin, yMax;
}

public class RSBackGround : MonoBehaviour
{
    private bool is_Hold = false;
    private float startPosX, startPosY;

    public Transform backGroundTrm;
    public ScreenClamp screenClamp;

    private void OnMouseDown()
    {
        Debug.Log("배경 눌러짐!");

        Vector3 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        startPosX = mousePos.x - backGroundTrm.position.x;
        startPosY = mousePos.y - backGroundTrm.position.y;

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
            backGroundTrm.position = movedPos;
        }

        float clampedX = Mathf.Clamp(backGroundTrm.position.x, screenClamp.xMin, screenClamp.xMax);
        float clampedY = Mathf.Clamp(backGroundTrm.position.y, screenClamp.yMin, screenClamp.yMax);

        backGroundTrm.position = new Vector2(clampedX, clampedY);
    }
}