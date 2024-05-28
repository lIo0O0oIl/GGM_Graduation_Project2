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
    public bool isOpen;
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
    private List<MemberChat> memberChats = new List<MemberChat>();
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;

    // UXLM
    VisualElement chatGround;
    VisualElement questionGround;
    VisualElement chattingFace;
    Button changeMemberButton;
    Label memberName;
    VisualElement memberList;

    // template
    VisualTreeAsset ux_chat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;
    VisualTreeAsset ux_memberList;

    private void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AddMember("강지현");
        if (Input.GetKeyDown(KeyCode.A))
            AddMember("이채민");
        if (Input.GetKeyDown(KeyCode.W))
            InputChat(true, true, FindMember("강지현"), "지현아");
        if (Input.GetKeyDown(KeyCode.E))
            InputChat(true, false, FindMember("강지현"), "싫어");
        if (Input.GetKeyDown(KeyCode.S))
            InputChat(true, true, FindMember("이채민"), "채민아");
        if (Input.GetKeyDown(KeyCode.D))
            InputChat(true, false, FindMember("이채민"), "내 이름 부르지 마");

        //EndToScroll();
    }

    public MemberChat FindMember(string name)
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

        chatGround.Q<ScrollView>("ChatGround").scrollDecelerationRate = 0.01f;
    }

    private void UXML_Load()
    {
        chatGround = root.Q<VisualElement>("ChatGround");
        questionGround = root.Q<VisualElement>("QuestionGround");
        chattingFace = root.Q<VisualElement>("FaceGround").Q<VisualElement>("OtherFace");
        changeMemberButton = root.Q<Button>("ChangeTarget");
        memberName = root.Q<Label>("TargetName");
        memberList = root.Q<VisualElement>("ChatMemberList");
    }

    private void Template_Load()
    {
        ux_chat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\Chat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\HiddenAskChat.uxml");
        ux_memberList = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\ChatMember.uxml");
    }

    private void Event_Load()
    {
        // 멤버 변경 버튼
        ChangeMember();
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

    public enum ChatType
    {
        String,
        Question,
        Image,
        CutScene
    }

    public void InputChatting(bool isUser, bool isChat, ChatType chatType, string msg)
    {
        VisualElement chat = null;

        switch (chatType)
        {
            case ChatType.String:
                chat = ux_chat.Instantiate();
                chat.Q<Label>().text = msg;
                break;
            case ChatType.Image:
                chat = new VisualElement();
                chat.style.backgroundImage = new StyleBackground(imageManager.FindPNG(msg).image);
                break;
            case ChatType.CutScene:
                chat = new Button();
                chat.style.backgroundImage = new StyleBackground(imageManager.FindPNG(msg).image);
                break;
        }

        // 유저의 대사라면
        if (isUser)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");


    }

    public void InputChat(bool isRecord, bool isUser, MemberChat other, string msg, Sprite face = null)
    {
        // 생성
        VisualElement chat = RemoveContainer(ux_chat.Instantiate());

        // 유저의 대사라면
        if (isUser)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");

        // 지정 표정으로 바꿔주기
        if (face != null)
            chattingFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
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

        EndToScroll();
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
            chat.Q<Button>().clicked += (() => { chat.parent.Remove(chat); });
        }
        else
            chat = RemoveContainer(ux_hiddenAskChat.Instantiate());

        // 대화에 추가
        questionGround.Add(chat);
        EndToScroll();

        if (isRecord)
        {
            Chatting chatting = new Chatting();
            chatting.isBool = isOpen;
            chatting.msg = msg;
            other.chattings.Add(chatting);
        }

    }

    private void EndToScroll()
    {
        ScrollView scrollView = chatGround.Q<ScrollView>("ChatGround");

        scrollView.schedule.Execute(() =>
        {
            float contentHeight = scrollView.contentContainer.layout.height;
            float viewportHeight = scrollView.contentViewport.layout.height;

            scrollView.scrollOffset = new Vector2(0, contentHeight - viewportHeight);
        });
    }

    public void AddMember(string memberName)
    {
        if (FindMember(memberName).isOpen == false)
        {
            FindMember(memberName).isOpen = true;

            VisualElement newMember = RemoveContainer(ux_memberList.Instantiate());
            newMember.Q<Label>("Name").text = memberName;
            newMember.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(FindMember(memberName).face);
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
        {
            changeMemberButton.style.backgroundImage = new StyleBackground(changeMemberBtnOn);
            memberList.style.display = DisplayStyle.None;
        }
        else
        {
            changeMemberButton.style.backgroundImage = new StyleBackground(changeMemberBtnOff);
            memberList.style.display = DisplayStyle.Flex;
        }
    }

    public void ChangeProfile(string name, Sprite face)
    {
        chattingFace.Q<Label>("Name").text = name;
        chattingFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
    }

    public void ChoiceMember(Button button)
    {
        Debug.Log("dhktek");
        MemberChat member = FindMember(button.Q<Label>("Name").text);
        if (member != null)
        {
            ChangeProfile(member.name, member.face);
            ChangeMember(); // 이름 목록 닫고
            RemoveChatting(); // 채팅 날리고
            RecallChatting(member); // 새로 쓰고
        }
    }
}
