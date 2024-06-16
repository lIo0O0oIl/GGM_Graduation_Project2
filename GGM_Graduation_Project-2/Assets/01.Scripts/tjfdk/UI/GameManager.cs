using ChatVisual;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    [Header("UI Manager")]
    public UIReader_CutScene cutSceneSystem;
    public UIReader_Chatting chatSystem;
    public UIReader_FileSystem fileSystem;
    public UIReader_ImageFinding imageSystem;

    [Header("Manager")]
    public ChatHumanManager chapterManager;
    public ChatProgress chatProgress;
    public ChatContainer chatContainer;
    public CutSceneManager cutSceneManager;
    public FileManager fileManager;
    public ImageManager imageManager;

    private float chatDelay = 0.5f;
    private float currentTime = 0;
    private bool is_Chat = false;

    private void Start()
    {
        //cutSceneSystem.PlayCutScene("DieFall");         // StartCutScene
        //chapterManager.AddHuman("HG");
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
