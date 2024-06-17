using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class MemberProfile
{
    public string name;
    public ESaveLocation nickName;
    public EFace currentFace;
    public Sprite[] faces;

    public bool isOpen;

    public List<ChatNode> chattings = new List<ChatNode>();
    public List<AskNode> questions = new List<AskNode>();
    //public List<Chatting> chattings = new List<Chatting>();
    //public List<Chatting> quetions = new List<Chatting>();
}

public class UIReader_Chatting : UI_Reader
{
    [Header("Member")]
    // current member name
    //public string currentMemberName;
    // member profile
    [SerializeField] List<MemberProfile> members = new List<MemberProfile>();
    //public Dictionary<string, MemberProfile> memberList;
    //public List<AskNode> questions;

    // memberList arrow sprite
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;



    // UXLM
        // chat and question ground
    ScrollView ui_chatGround;
    [HideInInspector] public VisualElement ui_questionGround;

        // member list
    [HideInInspector] public VisualElement ui_memberListGround;
    [HideInInspector] public Button ui_memberListButton;
    [HideInInspector] public bool isMemberListOpen;

        // other member profile
    VisualElement ui_otherFace;
    Label ui_otherMemberName;



    // template
    [Header("Template")]
    [SerializeField] VisualTreeAsset ux_chat;
    [SerializeField] VisualTreeAsset ux_askChat;
    [SerializeField] VisualTreeAsset ux_hiddenAskChat;
    [SerializeField] VisualTreeAsset ux_memberList;


    private void Awake()
    {
        base.Awake();

        MinWidth = 100;
        MinHeight = 100f;
        MaxWidth = 500;
        MaxHeight = 500;

        //memberList = new Dictionary<string, MemberProfile>();
        //questions = new List<AskNode>();
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Event_Load();

        // move member profile to member dictionary
        //foreach (MemberProfile member in members)
        //    memberList.Add(member.nickName.ToString(), member);
    }

    private void UXML_Load()
    {
        ui_chatGround = root.Q<ScrollView>("ChatGround");
        ui_questionGround = root.Q<VisualElement>("QuestionGround");
        ui_otherFace = root.Q<VisualElement>("FaceGround").Q<VisualElement>("OtherFace");
        ui_memberListButton = root.Q<Button>("ChangeTarget");
        ui_otherMemberName = root.Q<Label>("TargetName");
        ui_memberListGround = root.Q<VisualElement>("ChatMemberList");
    }

    private void Event_Load()
    {
        // scrollview find, and wheel speed setting
        ui_chatGround = ui_chatGround.Q<ScrollView>(ui_chatGround.name);
        ui_chatGround.RegisterCallback<WheelEvent>(OnMouseWheel);

        // member list hidden
        OnOffMemberList();
        ui_memberListButton.clicked += OnOffMemberList;
    }
    
    // find member
    public MemberProfile FindMember(string name)
    {
        foreach (MemberProfile member in members)
        {
            if (member.nickName.ToString() == name || member.name == name)
                return member;
        }

        return null;

        //Debug.Log(name + " MemberProfile dictionary");
        //MemberProfile member = memberList[name];
        //if (member != null)
        //    return member;
        //else
        //    return null;

        //return memberList[name];
    }

    // remove all chat and question
    private void RemoveChatting()
    {
        for (int i = ui_chatGround.childCount - 1; i >= 0; i--)
            ui_chatGround.RemoveAt(i);

        for (int i = ui_questionGround.childCount - 1; i >= 0; i--)
            ui_questionGround.RemoveAt(i);
    }

    // recall member chat and question
    private void RecallChatting(MemberProfile otherName)
    {
        ui_otherMemberName.text = otherName.name;

        foreach (ChatNode chat in otherName.chattings)
            InputChat(GameManager.Instance.chapterManager.nowHumanName, chat.state, chat.type, otherName.currentFace, chat.chatText, null, false);

        foreach (AskNode chat in otherName.questions)
            InputQuestion(otherName.name, true, chat.askText, null, null, null, false);

        Invoke("EndToScroll", 0.25f);
    }

    // input chat
    public void InputChat(string toWho, EChatState who, EChatType type,
        EFace face, string text, List<EChatEvent> chatEvt = null, bool isRecord = true)
    {
        type = EChatType.Text;

        // create chat
        VisualElement chat = null;
        // find member
        MemberProfile member = FindMember(toWho);

        // chat type
        switch (type)
        {
            // if Text
            case EChatType.Text:
                // create uxml
                chat = ux_chat.Instantiate();
                // chat text setting
                chat.Q<Label>().text = text;
                break;

            // if Image
            case EChatType.Image:
                // create VisualElement
                chat = new VisualElement();
                // image size change
                ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).image);
                break;

            // if CutScene
            case EChatType.CutScene:
                // create Button
                chat = new Button();
                // change chat style
                chat.AddToClassList("FileChatSize");
                chat.AddToClassList("NoButtonBorder");
                // find first cut of cutscene
                Sprite sprite = GameManager.Instance.cutSceneManager.FindCutScene(text).cutScenes[0].cut[0];
                // change background to image
                chat.style.backgroundImage = new StyleBackground(sprite);
                // connection click event, play cutscene
                chat.Q<Button>().clicked += (() => { GameManager.Instance.cutSceneSystem.PlayCutScene(text); });
                break;
            case EChatType.Default:
            case EChatType.Question:
            case EChatType.LockQuestion:
            {
                Debug.LogError("chat type이 아님");
            }
            break;
        }

        // if you this chat record
        if (isRecord)
            RecordChat(who, toWho, type, text);

        // whose chat style setting
        if (who == EChatState.Me)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");

        // add UI
        ui_chatGround.Add(chat);
        // scroll pos to end
        Invoke("EndToScroll", 0.5f);
    }

    // input question
    public void InputQuestion(string toWho, bool isLock, string msg, 
        List<EChatEvent> chatEvt = null, string nextMember = "", Action action = null, bool isRecord = true)
    {
        // create chat
        VisualElement chat = null;
        // find member
        MemberProfile member = FindMember(toWho.ToString());
        // chat type
        EChatType type = EChatType.Default;

        // chat type
        if (isLock)
        {
            // create uxml
            chat = RemoveContainer(ux_askChat.Instantiate());
            // chat name setting
            chat.name = msg;
            // chat text setting
            chat.Q<Label>().text = msg;
            // connection click event
            chat.Q<Button>().clicked += (() =>
            {
                // remove chat
                chat.parent.Remove(chat);
                // find record question, and remove this question
                //foreach (AskNode questions in member.questions) // new! need test
                //{s
                //    if (questions.askText == msg)
                //        member.questions.Remove(questions);
                //}

                for (int i = 0; i < member.questions.Count; ++i)
                {
                    if (member.questions[i].askText == msg)
                        member.questions.RemoveAt(i);
                }

                if (nextMember != "")
                {
                    GameManager.Instance.chapterManager.StopChatting();
                    ChoiceMember(GameManager.Instance.chatSystem.FindMember(nextMember));
                }

                // original, don't remove this cord...
                //for (int i = 0; i < member.quetions.Count - 1; ++i)
                //{
                //    if (member.quetions[i].text == msg)
                //    {
                //        Debug.Log("癲ル슣??袁ｋ즵 ?????");
                //        member.quetions.Remove(member.quetions[i]);
                //    }
                //}

                // if reply isn't null to start coroutine
                //if (reply != null)
                //    StartCoroutine(reply);

                // if action isn't null to start action
                action?.Invoke();

                // question
                type = EChatType.Question;
            });
        }
        else
        {
            // create uxml
            chat = RemoveContainer(ux_hiddenAskChat.Instantiate());
            // chat name setting
            chat.name = msg;
            // question
            type = EChatType.LockQuestion;
        }
                
        if (isRecord)
            RecordChat(EChatState.Me, toWho, type, msg);

        ////evnet;
        //SettingChat(member, member.currentFace, chatEvt);
        // add UI
        ui_questionGround.Add(chat);
    }

    // record chatting
    // 이거 type question에는 관계 없어서 빼든 뭐든 해야함
    private void RecordChat(EChatState who, string toWho, EChatType type, string msg)
    {
        // find member
        MemberProfile member = FindMember(toWho);

        // chatting setting
        switch (type)
        {
            case EChatType.Text:
            case EChatType.Image:
            case EChatType.CutScene:
            {
                ChatNode chat = new ChatNode();
                chat.state = who;
                chat.type = type;
                chat.chatText = msg;
                member.chattings.Add(chat);
            }
            break;
            case EChatType.Question:
            case EChatType.LockQuestion:
            {
                AskNode ask = new AskNode();
                ask.askText = msg;
                member.questions.Add(ask);
            }
            break;
        }
    }

    // setting Face and event
    public void SettingChat(MemberProfile member, Node node, EFace face, List<EChatEvent> evts)
    {
        // if current face of member is the same new face
        if (member.currentFace != face)
        {
            // find member face
            VisualElement memberFace = ui_otherFace.Q<VisualElement>("Face");
            // face type
            switch (face)
            {
                case EFace.Default:
                    memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Default]);
                    break;
                case EFace.Blush:
                    memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Blush]);
                    break;
                case EFace.Difficult:
                    memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Difficult]);
                    break;
            }

            // change current face of member
            member.currentFace = face;
        }

        if (node is ChatNode chatNode)
        {
            // event type
            for (int i = 0; i < evts.Count; i++)
            {
                switch (evts[i])
                {
                    case EChatEvent.LoadFile:
                    {
                        FileSO file = GameManager.Instance.fileManager.FindFile(chatNode.loadFileName[i]);
                        if (file != null)
                            GameManager.Instance.fileSystem.AddFile(file.fileType, file.fileName, file.fileParentName);
                        else
                            Debug.Log("this file not exist");
                    }
                    break;
                    case EChatEvent.Default:
                    case EChatEvent.Camera:
                    case EChatEvent.Vibration:
                    case EChatEvent.LoadNextDialog:
                        break;
                }
            }
        }
        else if (node is AskNode askNode)
        {
            // event type
            foreach (EChatEvent evt in evts)
            {
                switch (evt)
                {
                    case EChatEvent.Default:
                    case EChatEvent.Camera:
                    case EChatEvent.Vibration:
                    case EChatEvent.LoadFile:
                    case EChatEvent.LoadNextDialog:
                        break;
                }
            }
        }
    }

    // scroll pos setting
    void OnMouseWheel(WheelEvent evt)
    {
        // working progress is stop
        evt.StopPropagation();

        // multitly scroll speed to current delta value
        float delta = evt.delta.y * 500f;

        // scroll pos setting
        ui_chatGround.scrollOffset += new Vector2(0, delta);
    }

    // scroll pos to end
    public void EndToScroll()
    {
        ui_chatGround.verticalScroller.value = ui_chatGround.verticalScroller.highValue;
    }

    // add member
    public void AddMember(string memberName)
    {
        // find member
        MemberProfile member = FindMember(memberName);
        
        // if this member is not open
        if (member.isOpen == false)
        {
            // member open
            member.isOpen = true;

            // create uxml
            VisualElement newMember = RemoveContainer(ux_memberList.Instantiate());
            // change member name 
            newMember.Q<Label>("Name").text = member.name;
            // change member face 
            newMember.Q<VisualElement>("Face").style.backgroundImage
                = new StyleBackground(member.faces[(int)member.currentFace]);
            // connection click event
            newMember.Q<Button>("ChatMember").clicked += () =>
            {
                ChoiceMember(member);
            };

            // add member to memberListGround
            ui_memberListGround.Add(newMember);
        }
    }

    // change member
    public void ChoiceMember(MemberProfile member)
    {
        // change currentMember
        GameManager.Instance.chapterManager.ChatStart(member.nickName.ToString());

        // if member isn't null
        if (member != null)
        {
            // change profile
            ChangeProfile(member.name, member.faces[(int)member.currentFace]);
            // off memberListGround
            OnOffMemberList();
            // remove all chat and question
            RemoveChatting();
            // recall chat and question
            RecallChatting(member);

            ////// start chapter.
            //if (member.nickName.ToString() != "")
            //{
            //    GameManager.Instance.chapterManager.ChatStart(member.nickName.ToString());
            //}
            //else
            //    Debug.Log("this member hasn't name");
        }
    }

    // changed profile setting
    public void ChangeProfile(string name, Sprite face)
    {
        ui_otherFace.Q<Label>("Name").text = name;
        ui_otherFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
    }

    // member list on/off
    public void OnOffMemberList()
    {
        isMemberListOpen = !isMemberListOpen;

        if (isMemberListOpen)
        {
            ui_memberListButton.style.backgroundImage = new StyleBackground(changeMemberBtnOn);
            ui_memberListGround.style.display = DisplayStyle.None;
        }
        else
        {
            ui_memberListButton.style.backgroundImage = new StyleBackground(changeMemberBtnOff);
            ui_memberListGround.style.display = DisplayStyle.Flex;
        }
    }
}
