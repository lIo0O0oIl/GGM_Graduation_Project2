using ChatVisual;
using System;
using System.Collections;
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
    public ESaveLocation nickName;
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
    public string currentMemberName;
    [SerializeField]
    private List<MemberChat> memberChats = new List<MemberChat>();
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;

    // UXLM
    VisualElement chatGround;
    public VisualElement questionGround;
    VisualElement chattingFace;
    Button changeMemberButton;
    Label memberName;
    VisualElement memberList;

    // template
    VisualTreeAsset ux_chat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;
    VisualTreeAsset ux_memberList;

    public bool isChapterProcessing;


    private void Awake()
    {
        base.Awake();
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

    

    public MemberChat FindMember(string name)
    {
        foreach(MemberChat member in memberChats)
        {
            if (member.nickName.ToString() == name || member.name == name)
                return member;
        }

        return null;
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
            InputChat(chat.who, chat.toWho, chat.chatType, otherName.face, chat.text, false);

        foreach (Chatting chat in otherName.quetions)
            InputQuestion(chat.toWho, chat.chatType, chat.text, false, null);
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

    public void InputChat(EChatState who, ESaveLocation toWho, EChatType type, EFace face, 
        string msg, bool isRecord, EChatEvent evt = EChatEvent.Default)
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

        if (who == EChatState.Me)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");

        // 대화 업로드
        chatGround.Add(chat);
    }

    public void InputQuestion(ESaveLocation toWho, EChatType type, string msg, bool isRecord, IEnumerator reply)
    {
        // 생성
        VisualElement chat = null ;
        MemberChat suspect = FindMember(toWho.ToString());

        // 대화 정의
        switch (type)
        {
            case EChatType.Question:
                chat = RemoveContainer(ux_askChat.Instantiate());
                chat.name = msg;
                //chat.name = msg;
                chat.Q<Label>().text = msg;
                //chat.Q<Button>().clicked += action; // 대답 나오게
                chat.Q<Button>().clicked += (() => 
                { 
                    chat.parent.Remove(chat);
                    // reply 출력
                    StartCoroutine(reply);
                });
                break;
            case EChatType.LockQuestion:
                chat = RemoveContainer(ux_hiddenAskChat.Instantiate());
                chat.name = msg;
                //chat.name = msg;
                break;
        }

        if (isRecord)
            RecordChat(suspect, EChatState.Me, toWho, type, msg);

        //ChangeT(suspect, msg);

        // 대화에 추가
        questionGround.Add(chat);
    }

    private void RecordChat(MemberChat member, EChatState who, ESaveLocation toWho, EChatType type, string msg)
    {
        // 기록
        Chatting chatting = new Chatting();
        chatting.who = who;
        chatting.toWho = toWho;
        chatting.chatType = type;
        chatting.text = msg;

        switch (type)
        {
            case EChatType.Text:
            case EChatType.Image:
            case EChatType.CutScene:
                member.chattings.Add(chatting);
                break;
            case EChatType.Question:
            case EChatType.LockQuestion:
                member.quetions.Add(chatting);
                break;
        }
    }

    ////////private void ChangeT(MemberChat member, string msg)
    ////////{
    ////////    표정 변화
    ////////    if (member.face != face)
    ////////    {
    ////////        VisualElement suspectFace = chattingFace.Q<VisualElement>("Face");
    ////////        switch (face)
    ////////        {
    ////////            case EFace.Default:
    ////////                suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Default]);
    ////////                break;
    ////////            case EFace.Blush:
    ////////                suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Blush]);
    ////////                break;
    ////////            case EFace.Difficult:
    ////////                suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Difficult]);
    ////////                break;
    ////////        }

    ////////        member.face = face;
    ////////    }

    ////////    이벤트
    ////////    switch (evt)
    ////////    {
    ////////        case EChatEvent.Vibration:
    ////////            break;
    ////////        case EChatEvent.Round:
    ////////            break;
    ////////        case EChatEvent.Camera:
    ////////            break;
    ////////    }
    ////////}

    public void EndToScroll()
    {
        //Debug.Log("줄내림 입력");
        //ScrollView scrollView = chatGround.Q<ScrollView>("ChatGround");

        //scrollView.schedule.Execute(() =>
        //{
        //    float contentHeight = scrollView.contentContainer.layout.height;
        //    float viewportHeight = scrollView.contentViewport.layout.height;

        //    scrollView.scrollOffset = new Vector2(0, contentHeight - viewportHeight);
        //});

        chatGround.Q<ScrollView>(chatGround.name).verticalScroller.value
            = chatGround.Q<ScrollView>(chatGround.name).verticalScroller.highValue;
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
        currentMemberName = member.name;

        if (member != null)
        {
            ChangeProfile(member.name, member.faces[(int)member.face]);
            ChangeMember(); // 이름 목록 닫고
            RemoveChatting(); // 채팅 날리고
            RecallChatting(member); // 새로 쓰고

            // 해당 인물에게 챕터를 읽자
            if (member.chapterName != "")
            {
                // 챕터 불러주고
                chapterManager.Chapter(member.name, member.chapterName);
                // 끝났으면 현재 챕터 초기화
                //member.chapterName = "";
            }
            else
                Debug.Log("챕터가 비어있음");
        }
    }
}
