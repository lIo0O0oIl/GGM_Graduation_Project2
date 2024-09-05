using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_Relationship : MonoBehaviour
{
    // UIElement
    private VisualElement ui_RelationshipGround;            // 파일 끌어다 둘 때 이 공간을 킨다음 여기에 두면 UI 에서도 생성하는 식으로 사용하기

    // Line
    public GameObject linkLineDotPrefabs;
    private Charging charging;

    // InputField
    public GameObject inputFieldPrefabs;
    public GameObject canvas;

    // ShowHumanPicture
    public GameObject[] humans;
    private int nowHumanIndex = 0;

    private void Awake()
    {
        charging = GetComponent<Charging>();
    }

    public void AddHumanPicture()
    {
        humans[nowHumanIndex].gameObject.SetActive(true);
        nowHumanIndex++;
    }

    public void Charging(bool state)
    {
        charging.isFile = state;
    }
}
