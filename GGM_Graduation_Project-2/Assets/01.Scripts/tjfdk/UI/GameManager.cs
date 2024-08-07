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
    public UIReader_Relationship relationshipSystem;
    public UIReader_FileSystem fileSystem;
    public UIReader_ImageFinding imageSystem;

    [Header("Manager")]
    public ChatHumanManager chatHumanManager;
    public ChatContainer chatContainer;
    public CutSceneManager cutSceneManager;
    public FileManager fileManager;
    public ImageManager imageManager;

    [SerializeField] private bool is_tutorial = false;
    [SerializeField] private Sprite myFaec;


    private void Start()
    {
        GameStart();
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.P))
                UIReader_Main.Instance.PlusHP();

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.O))
            UIReader_Main.Instance.MinusHP();

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.I))
            chatHumanManager.NextChat();

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (imageSystem.ui_panelGround.childCount > 0)
        //        imageSystem.ui_panelGround.Remove(imageSystem.ui_panelGround.Q<VisualElement>("panel"));
        //}
    }

    public void GameStart()
    {
        if (!is_tutorial)
        {
            cutSceneSystem.PlayCutScene("DieFall");
            chatSystem.AddMember("GJH");
            chatSystem.OnOffMemberList();
    }
        else
        {
            chatSystem.AddMember("Test");
            chatSystem.ChoiceMember(chatSystem.FindMember("Test"), false);
            chatSystem.ChangeMyProfile("플레이어", myFaec);
            Instance.chatHumanManager.chapterMember = Instance.chatHumanManager.currentMember;
            is_tutorial = false;
        }
    }
}
