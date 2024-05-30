using ChatVisual;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public struct Chatting
{
    public EChatState who;
    public ESaveLocation toWho;
    public EChatType chatType;
    //public EFace currentFace;
    public string text;
}

[Serializable]
public class MemberChat
{
    public string name;
    //public string nickName;
    public EFace face;
    public Sprite[] faces;
    public bool isOpen;
    public string chapterName;
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
    [SerializeField]
    private ChatContainer chatContainer;

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
            AddMember("JiHyeon");
        if (Input.GetKeyDown(KeyCode.A))
            AddMember("JunWon");
        if (Input.GetKeyDown(KeyCode.W))
            InputChat(EChatState.Me, ESaveLocation.JiHyeon, EChatType.Text, EFace.Default, EChatEvent.Default, "지현아");
        if (Input.GetKeyDown(KeyCode.E))
            InputChat(EChatState.Other, ESaveLocation.JiHyeon, EChatType.Image, EFace.Blush, EChatEvent.Default, "담배");
        if (Input.GetKeyDown(KeyCode.G))
            InputChat(EChatState.Other, ESaveLocation.JiHyeon, EChatType.Text, EFace.Difficult, EChatEvent.Default, "담배");
        if (Input.GetKeyDown(KeyCode.S))
            InputChat(EChatState.Me, ESaveLocation.JunWon, EChatType.Text, EFace.Default, EChatEvent.Default, "준원아");
        if (Input.GetKeyDown(KeyCode.D))
            InputChat(EChatState.Other, ESaveLocation.JiHyeon, EChatType.CutScene, EFace.Default, EChatEvent.Default, "Start");

        if (Input.GetKeyDown(KeyCode.R))
            InputQuestion(ESaveLocation.JiHyeon, EChatType.Question, EFace.Default, EChatEvent.Default, "점심 메뉴 뭐야?");

        //if (Input.GetKeyDown(KeyCode.T))
        //    InputChatting(true, ChatType.Image, "담배");
        //if (Input.GetKeyDown(KeyCode.Y))
        //    InputChatting(true, ChatType.CutScene, "Start");

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
            InputChat(chat.who, chat.toWho, chat.chatType, otherName.face, EChatEvent.Default, chat.text, false);

        foreach (Chatting chat in otherName.quetions)
            InputQuestion(chat.toWho, chat.chatType, otherName.face, EChatEvent.Default, chat.text, false);

        ////스크롤바 맨 아래로 내리기
        //chatGround.Q<ScrollView>(chatGround.name).verticalScroller.value
        //    = chatGround.Q<ScrollView>(chatGround.name).verticalScroller.highValue;
    }

    private void Te(VisualElement chat, Sprite sprite)
    {
        chat.style.backgroundImage = new StyleBackground(sprite);
        float size = 0;

        if (sprite.rect.width >= sprite.rect.height)
            size = sprite.rect.width;
        else
            size = sprite.rect.height;

        if (size < 100)
            size = 3;
        else if (size < 150)
            size = 2;
        else
            size = 1;

        chat.style.width = sprite.rect.width * size;
        chat.style.height = sprite.rect.height * size;
    }

    public void Chapter(string name)
    {
        // 챕터를... 멤버한테 붙여주고...
        // 유저가 멤버를 이동할 때 마다
        // 붙어있는 챕터를 보고 챕터가 있다면 실행해주고...
        //
        //MemberChat member = FindMember(name);
        //if (member.chapterName != "")
        //{
        //    foreach (Chapter chapter in chatContainer.MainChapter)
        //    {
        //        if (chapter.showName == member.chapterName)
        //        {
        //            chapter.chat[0].
        //        }
        //    }
        //}
    }

    public void InputChat(EChatState who, ESaveLocation toWho, EChatType type, EFace face, EChatEvent evt, string msg, bool isRecord = true)
    {
        // 생성
        VisualElement chat = null;
        MemberChat suspect = FindMember(toWho.ToString());

        // 대화 정의
        switch (type)
        {
            case EChatType.Text:
                chat = ux_chat.Instantiate();
                chat.Q<Label>().text = msg;
                break;
            case EChatType.Image:
                chat = new VisualElement();
                Te(chat, imageManager.FindPNG(msg).image);
                break;
            case EChatType.CutScene:
                chat = new Button();
                chat.AddToClassList("FileChatSize");
                chat.AddToClassList("NoButtonBorder");
                Sprite sprite = cutSceneManager.FindCutScene(msg).cutScenes[0].cut;
                chat.style.backgroundImage = new StyleBackground(sprite);
                chat.Q<Button>().clicked += (() => { cutSceneSystem.PlayCutScene(msg); });
                break;
        }

        if (isRecord)
            RecordChat(suspect, who, toWho, type, msg);

        ChangeT(suspect, evt, face, msg);

        // 말하는 게 누구인지
        if (who == EChatState.Me)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");

        // 대화 업로드
        chatGround.Add(chat);
    }

    public void InputQuestion(ESaveLocation toWho, EChatType type, EFace face, EChatEvent evt, string msg, bool isRecord = true)
    {
        // 생성
        VisualElement chat = null ;
        MemberChat suspect = FindMember(toWho.ToString());

        // 대화 정의
        switch (type)
        {
            case EChatType.Question:
                chat = RemoveContainer(ux_askChat.Instantiate());
                chat.Q<Label>().text = msg;
                //chat.Q<Button>().clicked += action; // 대답 나오게
                chat.Q<Button>().clicked += (() => { chat.parent.Remove(chat); });
                break;
            case EChatType.LockQuestion:
                chat = RemoveContainer(ux_hiddenAskChat.Instantiate());
                break;
        }

        if (isRecord)
            RecordChat(suspect, EChatState.Me, toWho, type, msg);

        ChangeT(suspect, evt, face, msg);

        // 대화에 추가
        questionGround.Add(chat);
        EndToScroll();
    }

    private void RecordChat(MemberChat member, EChatState who, ESaveLocation toWho, EChatType type, string msg)
    {
        // 기록
        Chatting chatting = new Chatting();
        chatting.who = who;
        chatting.toWho = toWho;
        chatting.chatType = type;
        chatting.text = msg;
        member.chattings.Add(chatting);
    }

    private void ChangeT(MemberChat member, EChatEvent evt, EFace face, string msg)
    {
        // 표정 변화
        if (member.face != face)
        {
            VisualElement suspectFace = chattingFace.Q<VisualElement>("Face");
            switch (face)
            {
                case EFace.Default:
                    suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Default]);
                    break;
                case EFace.Blush:
                    suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Blush]);
                    break;
                case EFace.Difficult:
                    suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Difficult]);
                    break;
            }

            member.face = face;
        }

        // 이벤트
        switch (evt)
        {
            case EChatEvent.Vibration:
                break;
            case EChatEvent.Round:
                break;
            case EChatEvent.Camera:
                break;
        }
    }

    //public void InputChatting(bool isUser, ChatType chatType, string msg)
    //{
    //    VisualElement chat = null;
    //    VisualElement parent = null;

    //    switch (chatType)
    //    {
    //        case ChatType.String:
    //            chat = ux_chat.Instantiate();
    //            chat.Q<Label>().text = msg;
    //            parent = chatGround;
    //            break;
    //        case ChatType.Question:
    //            chat = ux_askChat.Instantiate();
    //            chat.Q<Label>().text = msg;
    //            parent = questionGround;
    //            break;
    //        case ChatType.Image:
    //            chat = new VisualElement();
    //            Te(chat, imageManager.FindPNG(msg).image);
    //            parent = chatGround;
    //            break;
    //        case ChatType.CutScene:
    //            chat = new Button();
    //            chat.AddToClassList("FileChatSize");
    //            chat.AddToClassList("NoButtonBorder");
    //            Sprite sprite = cutSceneManager.FindCutScene(msg).cutScenes[0].cut;
    //            chat.style.backgroundImage = new StyleBackground(sprite);
    //            chat.Q<Button>().clicked += (() => { cutSceneSystem.PlayCutScene(msg); });
    //            parent = chatGround;
    //            break;
    //    }
    //    Debug.Log(chat.name);
    //    // 유저의 대사라면
    //    if (isUser)
    //        chat.AddToClassList("MyChat");
    //    else
    //        chat.AddToClassList("OtherChat");

    //    // question이라면 생성 위치 다르게
    //}

    //public void InputChat(bool isRecord, bool isUser, MemberChat other, string msg, Sprite face = null)
    //{
    //    // 생성
    //    VisualElement chat = RemoveContainer(ux_chat.Instantiate());

    //    // 유저의 대사라면
    //    if (isUser)
    //        chat.AddToClassList("MyChat");
    //    else
    //        chat.AddToClassList("OtherChat");

    //    // 지정 표정으로 바꿔주기
    //    if (face != null)
    //        chattingFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
    //    // 대사 변경
    //    chat.Q<Label>().text = msg;
    //    // 대화에 추가
    //    chatGround.Add(chat);

    //    if (isRecord)
    //    {
    //        Debug.Log("기록함");
    //        Chatting chatting = new Chatting();
    //        chatting.isBool = isUser;
    //        chatting.msg = msg;
    //        other.chattings.Add(chatting);
    //    }

    //    EndToScroll();
    //}

    //public void InputQuestion(bool isRecord, bool isOpen, MemberChat other, string msg, Action action)
    //{
    //    // 생성
    //    VisualElement chat;

    //    // 잠금이 풀린 대사라면
    //    if (isOpen)
    //    {
    //        chat = RemoveContainer(ux_askChat.Instantiate());
    //        // 대사 변경
    //        chat.Q<Label>().text = msg;
    //        // 대사 이벤트 연결
    //        chat.Q<Button>().clicked += action;
    //        chat.Q<Button>().clicked += (() => { chat.parent.Remove(chat); });
    //    }
    //    else
    //        chat = RemoveContainer(ux_hiddenAskChat.Instantiate());

    //    // 대화에 추가
    //    questionGround.Add(chat);
    //    EndToScroll();

    //    if (isRecord)
    //    {
    //        Chatting chatting = new Chatting();
    //        chatting.isBool = isOpen;
    //        chatting.msg = msg;
    //        other.chattings.Add(chatting);
    //    }

    //}

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
        MemberChat member = FindMember(memberName);
        if (member.isOpen == false)
        {
            member.isOpen = true;

            VisualElement newMember = RemoveContainer(ux_memberList.Instantiate());
            newMember.Q<Label>("Name").text = member.name;
            newMember.Q<VisualElement>("Face").style.backgroundImage 
                = new StyleBackground(member.faces[0]);
            newMember.Q<Button>("ChatMember").clicked += () =>
            {
                ChoiceMember(member);
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

    public void ChoiceMember(MemberChat member)
    {
        Debug.Log("dhktek");
        if (member != null)
        {
            ChangeProfile(member.name, member.faces[(int)member.face]);
            ChangeMember(); // 이름 목록 닫고
            RemoveChatting(); // 채팅 날리고
            RecallChatting(member); // 새로 쓰고
        }
    }
}
