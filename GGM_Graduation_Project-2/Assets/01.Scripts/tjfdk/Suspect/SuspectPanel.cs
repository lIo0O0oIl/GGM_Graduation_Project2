using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectPanel : MonoBehaviour
{
    private Suspcet suspect;

    [SerializeField] private Image face;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI feature;
    [SerializeField] private Slider doubtLevel;

    private void Awake()
    {
        suspect = GetComponent<Suspcet>();
        doubtLevel.maxValue = suspect.DoubtLevelMax;

        Apply();
    }

    public void Apply()
    {
        face.sprite = suspect.CurrentFace;
        name.text = suspect.name;
        doubtLevel.value = suspect.DoubtLevel;
        //feature.text = suspect.Feature;
    }

    public void InputMemo(string memo)
    {
        suspect.Memo = memo;
    }
}
