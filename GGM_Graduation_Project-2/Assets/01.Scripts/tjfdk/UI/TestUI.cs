using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum FileType
{
    FOLDER,
    IMAGE,
    TEXT
}

public class TestUI : MonoBehaviour
{
    private UIDocument document;
    private VisualElement root;

    public GameObject settingPanel;

    [Header("UXML")]
    [Header("Button")]
    Button chattingButton;
    Button connectionButton;
    Button settingButton;
    [Header("Panel")]
    VisualElement chattingPanel;
    VisualElement connectionPanel;
    VisualElement imageFindingPanel;
    [Header("Chat")]
    VisualElement chattingFace;

    [Header("Template")]
    [Header("Chat")]
    VisualElement chatGround;
    VisualTreeAsset ux_myChat;
    VisualTreeAsset ux_otherChat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;

    [Header("File")]
    public VisualElement fileGround;
    VisualTreeAsset ux_folderFile;
    VisualTreeAsset ux_imageFile;
    VisualTreeAsset ux_textFile;


    //[Header("Sprite")]
    //[Header("Chat")]
    [SerializeField] private Sprite sp_speechrSprite;
    //[Header("File")]
    //[SerializeField] private Sprite sp_folderSprite;
    //[SerializeField] private Sprite sp_imageSprite;
    //[SerializeField] private Sprite sp_textSprite;


    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        Load();
        AddEvent();
    }

    private void Load()
    {
        // Button
        chattingButton = root.Q<Button>("ChattingBtn");
        connectionButton = root.Q<Button>("ConnectionBtn");
        settingButton = root.Q<Button>("SoundSettingBtn");

        // Panel
        chattingPanel = root.Q<VisualElement>("Chatting");
        connectionPanel = root.Q<VisualElement>("Connection");
        imageFindingPanel = root.Q<VisualElement>("ImageFinding");

        // Chat
        chattingFace = root.Q<VisualElement>("ChatFace");

        // System Ground
        chatGround = root.Q<VisualElement>("ChatGround");
        fileGround = root.Q<VisualElement>("FileGround");

        // UXML Load

        // Chat
        ux_myChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\MyChat.uxml");
        ux_otherChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\OtherChat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\HiddenAskChat.uxml");

        // File
        ux_folderFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\MyChat.uxml");
        ux_imageFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\OtherChat.uxml");
        ux_textFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\AskChat.uxml");

        // Sprite Load

        // Chat

        // File

    }

    private void AddEvent()
    {
        chattingButton.clickable.clicked += () =>
        {
            chattingPanel.SetEnabled(true);
            connectionPanel.SetEnabled(false);
        };

        connectionButton.clickable.clicked += () =>
        {
            chattingPanel.SetEnabled(false);
            connectionPanel.SetEnabled(true);
        };

        settingButton.clickable.clicked += () =>
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InputQuestion(true, "크킄", actionTest);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            InputChat(true, "크킄", sp_speechrSprite);
        }
    }

    // 기능 독립 시키기...

    public void InputChat(bool isUser, string msg, Sprite face)
    {
        VisualElement chat = null;

        if (isUser)
            chat = ux_myChat.Instantiate();
        else
            chat = ux_otherChat.Instantiate();

        chattingFace.style.backgroundImage = new StyleBackground(face);
        chat.Q<Label>().text = msg;
        chatGround.Add(chat);
    }

    public void InputQuestion(bool isOpen, string msg, Action action)
    {
        VisualElement chat;

        if (isOpen)
        {
            chat = ux_askChat.Instantiate();
            chat.Q<Button>().clicked += action;
            chat.Q<Label>().text = msg;
        }
        else
            chat = ux_hiddenAskChat.Instantiate();

        chatGround.Add(chat);
    }

    public void AddFile(FileType fieType, string fileName, Action action, bool isRock)
    {
        VisualElement file = null;

        switch (fieType)
        {
            case FileType.FOLDER:
                file = ux_folderFile.Instantiate();
                break;
            case FileType.IMAGE:
                file = ux_imageFile.Instantiate();
                break;
            case FileType.TEXT:
                file = ux_textFile.Instantiate();
                break;
        }

        file.Q<Button>().clicked += action;
        file.Q<Label>().text = fileName;

        fileGround.Add(file);
    }

    private void actionTest()       
    {
        Debug.Log("tlqkf");
    }
}
