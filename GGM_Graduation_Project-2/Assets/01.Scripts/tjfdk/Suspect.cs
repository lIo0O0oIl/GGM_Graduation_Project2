using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Suspect : MonoBehaviour
{
    [SerializeField] private string nickName;
    [SerializeField] private string feature;
    [SerializeField] private Sprite image;
    [SerializeField] private int doubtLevel;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void ChangeName(string _name)
    {
        name = _name;
        slider.GetComponentInChildren<TextMeshProUGUI>().text =
            $"ÀÌ¸§ : {name} \nÆ¯Â¡ : {feature}";
    }

    public void ChangeFeature(string _feature)
    {
        feature = _feature;
        slider.GetComponentInChildren<TextMeshProUGUI>().text =
            $"ÀÌ¸§ : {name} \nÆ¯Â¡ : {feature}";
    }

    public void ChangeDoubt(int _doubt)
    {
        doubtLevel = _doubt;
        slider.GetComponentInChildren<Slider>().value = doubtLevel;
    }
}
