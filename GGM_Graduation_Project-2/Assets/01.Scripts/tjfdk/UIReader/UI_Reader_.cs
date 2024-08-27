using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_Reader_ : MonoBehaviour
{
    Button reButton;
    Button pathButton;
    Button pathPanel;

    private void OnEnable()
    {
        reButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("ReButton");
        reButton.clicked += () => { SceneManager.LoadScene("Intro"); SoundManager.Instance.StopBGM(); };

        pathButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("PathButton");
        pathButton.clicked += () => { pathPanel.style.display = DisplayStyle.Flex; };

        pathPanel = GetComponent<UIDocument>().rootVisualElement.Q<Button>("SoundPath");
        pathPanel.clicked += () => { pathPanel.style.display = DisplayStyle.None; };
    }
}
