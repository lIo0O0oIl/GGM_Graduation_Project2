using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageZoom : MonoBehaviour, IScrollHandler
{
    public TMP_Text ZoomPersent;
    public float zoomSpeed = 0.05f;
    public float minScale = 0.7f;      // 최소로 보여질 값은 70% 임.
    public float maxScale = 10.0f;         // 최대로 보여질 값은 1000% 까지임.

    private Vector3 originScale;       // 오리지널 스케일, min 스케일.

    private void Start()
    {
        originScale = transform.localScale;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 move = Vector3.one * (eventData.scrollDelta.y + zoomSpeed);     // 백터를 새로 만들어줌.
        Vector3 newScale = transform.localScale + move;

        newScale = Vector3.Max(originScale * minScale, newScale);      // 둘 중 큰 것들을 반환
        newScale = Vector3.Min(originScale * maxScale, newScale);            // 둘 중 작은 것들을 반환

        ScaleApply(ClampScale(newScale));
    }

    private Vector3 ClampScale(Vector3 newScale)
    {
        newScale = Vector3.Max(originScale * minScale, newScale);      // 둘 중 큰 것들을 반환
        newScale = Vector3.Min(originScale * maxScale, newScale);            // 둘 중 작은 것들을 반환

        return newScale;
    }

    public void IncreaseBtn()
    {
        transform.localScale += Vector3.one * 0.1f;
        ScaleApply(ClampScale(transform.localScale));
    }

    public void DecreaseBtn()
    {
        transform.localScale -= Vector3.one * 0.1f;
        ScaleApply(ClampScale(transform.localScale));
    }

    private void ScaleApply(Vector3 newScale)
    {
        transform.localScale = newScale;
        ScalePersent();
    }

    private void ScalePersent()
    {
        float currentScale = transform.localScale.x / originScale.x;        // 현재 스케일을 본래 스케일로 나눈 비율
        float persent = MathF.Round(currentScale * 100);
        ZoomPersent.text = $"{persent}%";
    }

}
