using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_Relationship : MonoBehaviour
{
    // UIElement
    public VisualElement ui_RelationshipGround;            // 파일 끌어다 둘 때 이 공간을 킨다음 여기에 두면 UI 에서도 생성하는 식으로 사용하기

    // Line
    public GameObject linkLineDotPrefabs;
    private Charging charging;

    // InputField
    public GameObject inputFieldPrefabs;
    public GameObject canvas;

    // ShowHumanPicture
    public GameObject[] humans;         // 미리 만들어두고 셋엑티브만 꺼두기. 이후 메서드 호출마다 사람이 생겨남.
    private int nowHumanIndex = 0;

    // Evidence Instantiate
    public GameObject evidencePrefabs;

    private void Awake()
    {
        charging = GameObject.Find("Game").GetComponent<Charging>();
    }

    private void Start()
    {
        //ui_RelationshipGround = UIReader_Main.Instance.RelationshipPanel;
    }

    public void AddHumanPicture()
    {
        humans[nowHumanIndex].gameObject.SetActive(true);
        nowHumanIndex++;
    }

    public void AddEvidence(string fileName)
    {
        PngSO png = GameManager.Instance.imageManager.FindPng(fileName);

        Vector3 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject evidence = Instantiate(evidencePrefabs, mousePos, Quaternion.identity, canvas.transform);
        evidence.GetComponent<SpriteRenderer>().sprite = png.image;
    }

    public void Charging(bool state)
    {
        charging.isFile = state;
    }
}
