using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvidencePanel : MonoBehaviour
{
    private Evidence evidence;

    [SerializeField] private Image image;
    [SerializeField] private string who;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI feature;

    private void Awake()
    {
        evidence = GetComponent<Evidence>();
        Apply();
    }

    public void Apply()
    {
        image.sprite = evidence.Image;
        name.text = evidence.name;
        feature.text = evidence.Feature;
    }
}
