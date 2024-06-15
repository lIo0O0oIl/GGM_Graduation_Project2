using ChatVisual;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : UI_Reader
{
    [SerializeField] private ChatProgress chatProgress;

    private float chatDelay = 0.5f;
    private float currentTime = 0;
    private bool is_Chat = false;

    private void Start()
    {
        cutSceneSystem.PlayCutScene("DieFall");         // StartCutScene
        chapterManager.AddHuman("HG");
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= chatDelay && is_Chat)
        {
            chatProgress.NextChat();
            currentTime = 0;
        }
    }
}
