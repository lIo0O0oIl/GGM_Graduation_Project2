using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_Chatting : MonoBehaviour
{
    // other system
    TestUI mainSystem;
    UIReader_FileSystem fileSystem;
    UIReader_Connection connectionSystem;

    // main
    private UIDocument document;
    private VisualElement root;
    private VisualElement chatRoot;

    // UXLM
    VisualElement chatGround;
    VisualElement chattingFace;

    // template
    VisualTreeAsset ux_myChat;
    VisualTreeAsset ux_otherChat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;

    private void Awake()
    {
        mainSystem = GetComponent<TestUI>();
        fileSystem = GetComponent<UIReader_FileSystem>();
        connectionSystem = GetComponent<UIReader_Connection>();
    }

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        chatRoot = root.Q<VisualElement>("");

        UXML_Load();
        Template_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        chatGround = root.Q<VisualElement>("ChatGround");
        chattingFace = root.Q<VisualElement>("ChatFace");
    }

    private void Template_Load()
    {
        ux_myChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\MyChat.uxml");
        ux_otherChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\OtherChat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\HiddenAskChat.uxml");
    }

    private void Event_Load()
    {
    }

    // Function
    public void InputChat(bool isUser, string msg, Sprite face = null)
    {
        // 생성
        VisualElement chat = null;

        // 유저의 대사라면
        if (isUser)
            chat = ux_myChat.Instantiate();
        else
            chat = ux_otherChat.Instantiate();

        // 지정 표정으로 바꿔주기
        if (face != null)
            chattingFace.style.backgroundImage = new StyleBackground(face);
        // 대사 변경
        chat.Q<Label>().text = msg;
        // 대화에 추가
        chatGround.Add(chat);
    }

    public void InputQuestion(bool isOpen, string msg, Action action)
    {
        // 생성
        VisualElement chat;

        // 잠금이 풀린 대사라면
        if (isOpen)
        {
            chat = ux_askChat.Instantiate();
            // 대사 변경
            chat.Q<Label>().text = msg;
            // 대사 이벤트 연결
            chat.Q<Button>().clicked += action;
        }
        else
            chat = ux_hiddenAskChat.Instantiate();

        // 대화에 추가
        chatGround.Add(chat);
    }
}
