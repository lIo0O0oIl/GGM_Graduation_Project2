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
    public CutScene cutSceneSystem;
    public Chatting chatSystem;
    public UIReader_Relationship relationshipSystem;
    public UIReader_SelectHuman selectHumanSystem;
    public FileSystem fileSystem;
    public Investigation imageSystem;

    [Header("Manager")]
    public ChatHumanManager chatHumanManager;
    public ChatContainer chatContainer;
    public CutSceneManager cutSceneManager;
    public FileManager fileManager;
    public ImageManager imageManager;

    [SerializeField] private bool is_tutorial = false;
    [SerializeField] private Sprite myFaec;

    // 내가 임의로 추가한거임 충돌나면 내껄 버려
    public Sprite cutScenePlayIcon;


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

        //if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.I))
        if (Input.GetKey(KeyCode.I))
            chatHumanManager.NextChat();
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