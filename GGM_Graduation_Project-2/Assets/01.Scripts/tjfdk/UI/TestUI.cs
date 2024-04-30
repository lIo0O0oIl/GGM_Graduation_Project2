using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUI : MonoBehaviour
{
    private UIDocument document;
    private VisualElement root;
    private Button testButton;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        testButton = root.Q<Button>("EvidenceImage");

        testButton.clicked += Test;
    }

    private void Test()
    {
        Debug.Log("살려주세요");
    }
}
