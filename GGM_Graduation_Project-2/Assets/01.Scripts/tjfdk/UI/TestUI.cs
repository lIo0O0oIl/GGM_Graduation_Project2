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
    private Button test;
    private Button testButton;
    public bool movetest = false;

    [Header("UXML")]

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


    [Header("Sprite")]
    [Header("Chat")]
    [SerializeField] private Sprite sp_speechrSprite;
    [Header("File")]
    [SerializeField] private Sprite sp_folderSprite;
    [SerializeField] private Sprite sp_imageSprite;
    [SerializeField] private Sprite sp_textSprite;


    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        test = root.Q<Button>("EvidenceImage");

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InputQuestion(true, "크킄", actionTest);
        }
    }

    // 기능 독립 시키기...

    public void InputChat(bool isUser, string msg)
    {
        VisualElement chat = null;

        if (isUser)
            chat = ux_myChat.Instantiate();
        else
            chat = ux_otherChat.Instantiate();

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
