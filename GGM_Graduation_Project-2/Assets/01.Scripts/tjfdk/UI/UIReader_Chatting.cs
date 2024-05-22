using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public struct Chatting
{
    public bool isBool;
    public string msg;
}

[Serializable]
public class MemberChat
{
    public string name;
    public Sprite face;
    public List<Chatting> chattings = new List<Chatting>();
    public List<Chatting> quetions = new List<Chatting>();
    public MemberChat(string name)
    {
        this.name = name;
    }
}

public class UIReader_Chatting : UI_Reader
{
    [SerializeField]
    private List<MemberChat> memberChats;

    // UXLM
    VisualElement chatGround;
    VisualElement chattingFace;
    Button changeMemberButton;
    Label memberName;
    VisualElement memberList;

    // template
    VisualTreeAsset ux_myChat;
    VisualTreeAsset ux_otherChat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;
    VisualTreeAsset ux_memberList;

    private void Awake()
    {
        base.Awake();
        memberChats = new List<MemberChat>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AddMember("강지현");
        if (Input.GetKeyDown(KeyCode.A))
            AddMember("이채민");
        if (Input.GetKeyDown(KeyCode.W))
            InputChat(true, true, FindMember("강지현"), "Test");
        if (Input.GetKeyDown(KeyCode.E))
            InputChat(true, false, FindMember("강지현"), "Test1");
        if (Input.GetKeyDown(KeyCode.S))
            InputChat(true, true, FindMember("이채민"), "Test2");
        if (Input.GetKeyDown(KeyCode.D))
            InputChat(true, false, FindMember("이채민"), "Test3");
    }

    private MemberChat FindMember(string name)
    {
        foreach(MemberChat member in memberChats)
        {
            if (member.name == name)
                return member;
        }

        return null;
    }

    private void OnEnable()
    {
        base.OnEnable();

        Template_Load();
        UXML_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        chatGround = root.Q<VisualElement>("ChatGround");
        chattingFace = root.Q<VisualElement>("ChatFace");
        changeMemberButton = root.Q<Button>("ChangeTarget");
        memberName = root.Q<Label>("TargetName");
        memberList = root.Q<VisualElement>("ChatMemberList");
    }

    private void Template_Load()
    {
        ux_myChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\MyChat.uxml");
        ux_otherChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\OtherChat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\HiddenAskChat.uxml");
        ux_memberList = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\ChatMember.uxml");
    }

    private void Event_Load()
    {
        // 멤버 변경 버튼
        changeMemberButton.clicked += ChangeMember;
        // 채팅 스크롤뷰 속도 변경
        //Debug.Log(chatGround.Q<ScrollView>(chatGround.name));
        //chatGround.Q<ScrollView>(chatGround.name).scrollDecelerationRate = 5f;
    }

    private void RemoveChatting()
    {
        for (int i = chatGround.childCount - 1; i >= 0; i--)
            chatGround.RemoveAt(i);
    }

    private void RecallChatting(MemberChat otherName)
    {
        memberName.text = otherName.name;

        foreach (Chatting chat in otherName.chattings)
            InputChat(false, chat.isBool, otherName, chat.msg);

        foreach(Chatting chat in otherName.quetions)
            InputQuestion(false, chat.isBool, otherName, chat.msg, () => { });

        // 스크롤바 맨 아래로 내리기
        //chatGround.Q<ScrollView>(chatGround.name).verticalScroller.value 
        //    = chatGround.Q<ScrollView>(chatGround.name).verticalScroller.highValue;
    }

    // Function
    public void InputChat(bool isRecord, bool isUser, MemberChat other, string msg, Sprite face = null)
    {
        // 생성
        VisualElement chat = null;

        // 유저의 대사라면
        if (isUser)
            chat = RemoveContainer(ux_myChat.Instantiate());
        else
            chat = RemoveContainer(ux_otherChat.Instantiate());

        // 지정 표정으로 바꿔주기
        if (face != null)
            chattingFace.style.backgroundImage = new StyleBackground(face);
        // 대사 변경
        chat.Q<Label>().text = msg;
        // 대화에 추가
        chatGround.Add(chat);

        if (isRecord)
        {
            Debug.Log("기록함");
            Chatting chatting = new Chatting();
            chatting.isBool = isUser;
            chatting.msg = msg;
            other.chattings.Add(chatting);
        }
    }

    public void InputQuestion(bool isRecord, bool isOpen, MemberChat other, string msg, Action action)
    {
        // 생성
        VisualElement chat;

        // 잠금이 풀린 대사라면
        if (isOpen)
        {
            chat = RemoveContainer(ux_askChat.Instantiate());
            // 대사 변경
            chat.Q<Label>().text = msg;
            // 대사 이벤트 연결
            chat.Q<Button>().clicked += action;
        }
        else
            chat = RemoveContainer(ux_hiddenAskChat.Instantiate());

        // 대화에 추가
        chatGround.Add(chat);

        if (isRecord)
        {
            Chatting chatting = new Chatting();
            chatting.isBool = isOpen;
            chatting.msg = msg;
            other.chattings.Add(chatting);
        }
    }

    public void AddMember(string memberName)
    {
        if (FindMember(memberName) == null)
        {
            VisualElement newMember = RemoveContainer(ux_memberList.Instantiate());
            newMember.Q<Button>("ChatMember").text = memberName;
            newMember.Q<Button>("ChatMember").clicked += () =>
            {
                ChoiceMember(newMember.Q<Button>("ChatMember"));
            };

            memberList.Add(newMember);
            memberChats.Add(new MemberChat(memberName));
        }
    }

    private void Test()
    {
        Debug.Log("이이");
    }

    public void ChangeMember()
    {
        if (memberList.style.display.value == DisplayStyle.Flex)
            memberList.style.display = DisplayStyle.None;
        else
            memberList.style.display = DisplayStyle.Flex;
    }

    public void ChoiceMember(Button button)
    {
        Debug.Log("드렁옴");
        //foreach (MemberChat member in memberChats)
        //{
        //    if (member.name == button.text)
        //    {
        //        ChangeMember(); // 이름 목록 닫고
        //        RemoveChatting(); // 채팅 날리고
        //        RecallChatting(member); // 새로 쓰고
        //    }
        //}
        MemberChat member = FindMember(button.text);
        if (member != null)
        {
            ChangeMember(); // 이름 목록 닫고
            RemoveChatting(); // 채팅 날리고
            RecallChatting(member); // 새로 쓰고
        }
    }
}
