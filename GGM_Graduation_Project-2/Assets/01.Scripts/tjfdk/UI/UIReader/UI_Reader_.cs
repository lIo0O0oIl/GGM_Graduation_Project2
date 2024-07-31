using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_Reader_ : MonoBehaviour
{
    Button reButton;

    private void OnEnable()
    {
        reButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("ReButton");
        reButton.clicked += () => { SceneManager.LoadScene("Intro"); };
    }
}
