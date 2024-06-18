using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public Node memCurrentNode;
}

public class UIReader_Chatting : UI_Reader
{
    [Header("Member")]
    [SerializeField] List<MemberProfile> members = new List<MemberProfile>();

    // memberList arrow sprite
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;
    [SerializeField]
    private float wheelSpeed = 25f;



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
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Event_Load();
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
    }

    // remove all chat and question
    public void RemoveChatting()
    {
        for (int i = ui_chatGround.childCount - 1; i >= 0; i--)
            ui_chatGround.RemoveAt(i);
    }

    public void RemoveQuestion()
    {
        for (int i = ui_questionGround.childCount - 1; i >= 0; i--)
            ui_questionGround.RemoveAt(i);
    }

    // recall member chat and question
    private void RecallChatting(MemberProfile otherName)
    {
        ui_otherMemberName.text = otherName.name;

        foreach (ChatNode chat in otherName.chattings)
            //InputChat(GameManager.Instance.chatHumanManager.nowHumanName, chat.state, chat.type, 
            //    otherName.currentFace, chat.chatText, null, false);

        Invoke("EndToScroll", 0.25f);
    }

    private void GameDown()
    {
        Application.Quit();
    }

    // input chat
    public void InputChat(string toWho, EChatState who, EChatType type,
        EFace face, string text, List<EChatEvent> chatEvt = null, bool isRecord = true)
    {
        // test
        if (text == "더 나은 빌드로 돌아오겠습니다.")
        {
            Invoke("GameDown", 3f);
        }



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
                ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).saveSprite);
                break;

            // if CutScene
            case EChatType.CutScene:
                // create Button
                chat = new Button();
                // change chat style
                chat.AddToClassList("FileChatSize");
                chat.AddToClassList("NoButtonBorder");
                // find first cut of cutscene
                ChatNode cutScene = GameManager.Instance.chatHumanManager.currentNode as ChatNode;
                GameManager.Instance.chatHumanManager.nowCondition = cutScene.childList[0] as ConditionNode;
                // change background to image
                Sprite sprite = GameManager.Instance.cutSceneManager.FindCutScene(text).cutScenes[0].cut[0];
                chat.style.backgroundImage = new StyleBackground(sprite);
                // connection click event, play cutscene
                chat.Q<Button>().clicked += (() => { GameManager.Instance.cutSceneSystem.PlayCutScene(text); });
                break;


        }

        // if you this chat record
        if (isRecord)
            RecordChat(who, toWho, type, text);

        // whose chat style setting
        Debug.Log(type.ToString() + " 타입 이상ㅎ마");
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
    public void InputQuestion(string toWho, bool isLock, AskNode askNode, bool isRecord = true)
    {
        // create chat
        VisualElement chat = null;
        // find member
        MemberProfile member = FindMember(toWho.ToString());
        // chat type
        EChatType type = EChatType.Text;
        if (isLock)
        {
            // create uxml
            chat = RemoveContainer(ux_askChat.Instantiate());
            // chat name setting
            chat.name = askNode.askText;
            // chat text setting
            chat.Q<Label>().text = askNode.askText;
            // connection click event
            chat.Q<Button>().clicked += (() =>
            {
                // add chat
                InputChat(toWho, EChatState.Me, type, member.currentFace, askNode.askText);

                // current question value list
                for (int i = 0; i < member.questions.Count; ++i)
                {
                    if (member.questions[i].askText == askNode.askText)
                        member.questions.RemoveAt(i);
                }

                // other question read false
                foreach (AskNode ask in member.questions)
                {
                    ask.test_isRead = false;
                }

                if (askNode.textEvent.Count == 1)
                {
                    Debug.Log(askNode.LoadNextDialog + " ?????怨쀫뎐???傭?");
                    GameManager.Instance.chatHumanManager.StopChatting();
                    member.memCurrentNode = askNode;
                    AddMember(askNode.LoadNextDialog);
                    ChoiceMember(GameManager.Instance.chatSystem.FindMember(askNode.LoadNextDialog));
                }
                else
                {
                    GameManager.Instance.chatHumanManager.currentNode = askNode;
                }

                // all question visualelement down
                GameManager.Instance.chatSystem.RemoveQuestion();
                member.questions.Clear();

                // currntNode, member's currentNode change
                member.memCurrentNode = askNode;

                // ?轅붽틓????ш내??쭩??????????낆젵
                askNode.is_UseThis = true;

                // chatting start
                GameManager.Instance.chatHumanManager.StartChatting();

                // question
                type = EChatType.Question;
            });
        }
        else
        {
            // create uxml
            chat = RemoveContainer(ux_hiddenAskChat.Instantiate());
            // chat name setting
            chat.name = askNode.askText;
            // question
            //type = EChatType.LockQuestion;
        }
                
        // record
        if (isRecord)
            RecordChat(EChatState.Me, toWho, type, askNode.askText);

        // add visualelement
        ui_questionGround.Add(chat);
    }

    // record chatting
    // ??????異?type question?????????댁삩????????욱룑??????嶺뚮칾怨댄뀭??癲ル슢??????????욱룕???
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
  //          case EChatType.LockQuestion:
  //          {
  //                  Debug.Log("?꿔꺂???熬곻퐢利???ㅻ쿋??");
  ///*              AskNode ask = new AskNode();
  //              ask.askText = msg;
  //              member.questions.Add(ask);*/
  //          }
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
                        break;
                }
            }
        }
    }

    //public void OpenOtherQuestion(bool isOpen)
    //{
    //    if (isOpen)
    //    {
    //        foreach (VisualElement q in ui_questionGround.Children())
    //            q.style.display = DisplayStyle.Flex;
    //    }
    //    else
    //    {
    //        foreach (VisualElement q in ui_questionGround.Children())
    //            q.style.display = DisplayStyle.None;
    //    }
    //}

    // scroll pos setting
    void OnMouseWheel(WheelEvent evt)
    {
        // working progress is stop
        evt.StopPropagation();

        // multitly scroll speed to current delta value
        float delta = evt.delta.y * wheelSpeed;

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
        // if member isn't null
        if (member != null)
        {
            // change currentMember
            GameManager.Instance.chatHumanManager.ChatResetAndStart(member.nickName.ToString());

            // change profile
            ChangeProfile(member.name, member.faces[(int)member.currentFace]);
            // off memberListGround
            isMemberListOpen = false;
            OnOffMemberList();
            // remove all chat and question
            RemoveChatting();
            RemoveQuestion();
            // recall chat and question
            RecallChatting(member);
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
