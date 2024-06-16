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
    public ChatContainer chatContainer;
    public CutSceneManager cutSceneManager;
    public FileManager fileManager;
    public ImageManager imageManager;

    private void Start()
    {
        cutSceneSystem.PlayCutScene("DieFall");
        chatSystem.AddMember("HG");
        chatSystem.OnOffMemberList();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.U))
        //    cutSceneSystem.PlayCutScene("DieFall");
    }
}
