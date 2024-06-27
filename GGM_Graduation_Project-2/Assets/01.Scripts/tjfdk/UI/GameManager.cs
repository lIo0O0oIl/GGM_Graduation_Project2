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
    public ChatHumanManager chatHumanManager;
    public ChatContainer chatContainer;
    public CutSceneManager cutSceneManager;
    public FileManager fileManager;
    public ImageManager imageManager;

    [SerializeField] private bool is_tutorial;
    [SerializeField] private Sprite myFaec;


    private void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        //if (!is_tutorial)
        //{
        
            cutSceneSystem.PlayCutScene("DieFall");
            chatSystem.AddMember("GJH");
            chatSystem.OnOffMemberList();
        //}
        //else
        //{
        //    chatSystem.AddMember("Tutorial1");
        //    chatSystem.ChoiceMember(chatSystem.FindMember("Tutorial1"));
        //    chatSystem.ChangeMyProfile("플레이어", myFaec);
        //    is_tutorial = false;
        //}
    }
}
