using ChatVisual;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
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

public class UIReader_Chatting : MonoBehaviour
{
    [Header("Member")]
    [SerializeField] List<MemberProfile> members = new List<MemberProfile>();

    // memberList arrow sprite
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;
    public float wheelSpeed = 25f;
    public float scrollEndSpeed = 0.15f;

    bool isConnectionOpen = false;
    //bool isSettingOpen = false;


    // root
    VisualElement root;

    // UXLM
    // chat and question ground
    ScrollView ui_chatGround;
    [HideInInspector] public VisualElement ui_questionGround;

    // member list
    [HideInInspector] public VisualElement ui_memberListGround;
    [HideInInspector] public Button ui_memberListButton;
    [HideInInspector] public bool isMemberListOpen;

    [HideInInspector] public Button ui_nextChatButton;
    private bool isMouseOverButton = false;

    // other member profile
    VisualElement ui_otherFace;
    VisualElement ui_myFace;
    Label ui_otherMemberName;

    // template
    [Header("Template")]
    [SerializeField] VisualTreeAsset ux_text;
    [SerializeField] VisualTreeAsset ux_highlightedtext;
    [SerializeField] VisualTreeAsset ux_button;
    [SerializeField] VisualTreeAsset ux_chat;
    [SerializeField] VisualTreeAsset ux_askChat;
    [SerializeField] VisualTreeAsset ux_hiddenAskChat;
    [SerializeField] VisualTreeAsset ux_memberList;

    // 흔들림 효과 넣어주기
    private Tween DoTween;
    public float duration = 1.0f; // 흔들기 지속 시간
    public float strength = 1; // 얼마나 멀리로 흔들리는지
    private VisualElement currentElement;

    private void Awake()
    {
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        UXML_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        root = GameObject.Find("Game").GetComponent<UIDocument>().rootVisualElement;

        ui_chatGround = UIReader_Main.Instance.root.Q<ScrollView>("ChatGround");
        ui_questionGround = UIReader_Main.Instance.root.Q<VisualElement>("QuestionGround");
        ui_otherFace = UIReader_Main.Instance.root.Q<VisualElement>("FaceGround").Q<VisualElement>("OtherFace");
        ui_myFace = UIReader_Main.Instance.root.Q<VisualElement>("FaceGround").Q<VisualElement>("MyFace");
        ui_memberListButton = UIReader_Main.Instance.root.Q<Button>("ChangeTarget");
        ui_nextChatButton = UIReader_Main.Instance.root.Q<Button>("NextChatBtn");
        ui_otherMemberName = UIReader_Main.Instance.root.Q<Label>("TargetName");
        ui_memberListGround = UIReader_Main.Instance.root.Q<VisualElement>("ChatMemberList");
    }

    private void Event_Load()
    {
        // scrollview find, and wheel speed setting
        ui_chatGround = ui_chatGround.Q<ScrollView>(ui_chatGround.name);
        //ui_chatGround.RegisterCallback<WheelEvent>(OnMouseWheel);

        // member list hidden
        OnOffMemberList();
        ui_memberListButton.clicked += OnOffMemberList;

        ui_nextChatButton.clicked += () => { GameManager.Instance.chatHumanManager.NextChat(); };
        ui_nextChatButton.RegisterCallback<PointerEnterEvent>(OnMouseEnterButton);
        ui_nextChatButton.RegisterCallback<PointerLeaveEvent>(OnMouseLeaveButton);

        ui_chatGround.Q<VisualElement>("unity-content-and-vertical-scroll-container").pickingMode = PickingMode.Ignore;
        ui_chatGround.Q<VisualElement>("unity-content-viewport").pickingMode = PickingMode.Ignore;
        ui_chatGround.Q<VisualElement>("unity-content-container").pickingMode = PickingMode.Ignore;

        root.RegisterCallback<WheelEvent>(OnMouseWheel);
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
    private void RecallChatting(MemberProfile member)
    {
        foreach (ChatNode chat in member.chattings)
        {
            InputChat(member.name, chat.state, chat.type, member.currentFace, chat.chatText, false);
        }

        Invoke("EndToScroll", scrollEndSpeed);
    }

    // input chat
    public void InputChat(string toWho, EChatState who, EChatType type,
        EFace face, string text, bool isRecord = true, bool isQuestion = false)
    {
        // create chat
        VisualElement chat = null;
        // find member
        MemberProfile member = FindMember(toWho);

        // chat type
        switch (type)
        {
            case EChatType.Text:
                // create uxml
                chat = UIReader_Main.Instance.RemoveContainer(ux_chat.Instantiate());
                chat.name = "chat";
                if (isQuestion)
                    chat.AddToClassList("Question");
                EventChatText(chat, text);
                break;
            case EChatType.Image:
                // create VisualElement
                chat = new VisualElement();
                chat.name = "image";
                // image size change
                UIReader_Main.Instance.ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).saveSprite);
                break;
            case EChatType.CutScene:
                // create Button
                chat = new Button();
                chat.name = "cutScene";
                
                // change chat style
                chat.AddToClassList("FileChatSize");
                chat.AddToClassList("NoButtonBorder");
                
                // find first cut of cutscene
                ChatNode cutScene = GameManager.Instance.chatHumanManager.currentNode as ChatNode;
                //GameManager.Instance.chatHumanManager.nowCondition = cutScene.childList[0] as ConditionNode;
                
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
        if (who == EChatState.Me)
        {
            chat.AddToClassList("MyChat");
        }
        else
        {
            chat.AddToClassList("OtherChat");
        }

        ui_chatGround.Add(chat);
        currentElement = chat;
        // scroll pos to end
        Invoke("EndToScroll", scrollEndSpeed);
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
        if (!isLock)
        {
            // create uxml
            chat = UIReader_Main.Instance.RemoveContainer(ux_askChat.Instantiate());
            // chat name setting
            chat.name = askNode.askText;
            // chat text setting
            chat.Q<Label>().text = askNode.askText;
            // connection click event
            chat.Q<Button>().clicked += (() =>
            {
                // add chat
                InputChat(toWho, EChatState.Me, type, member.currentFace, askNode.askText, true, true);

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
                    Debug.Log(askNode.LoadNextDialog + " ????????????ъ몥?????");
                    GameManager.Instance.chatHumanManager.StopChatting();
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
            chat = UIReader_Main.Instance.RemoveContainer(ux_hiddenAskChat.Instantiate());
            // chat name setting
            chat.name = askNode.askText;
            // question
            //type = EChatType.LockQuestion;
        }

        // record
        if (isRecord)
            RecordChat(EChatState.Me, toWho, type, askNode.askText, true);

        // add visualelement
        ui_questionGround.Add(chat);
    }

    // setting Face and event
    public void SettingChat(MemberProfile member, EChatState who, Node node, EFace face, List<EChatEvent> evts)
    {
        // find member face
        VisualElement memberFace = null;

        if (who == EChatState.Me)
        {
            member = FindMember("HG");
            memberFace = ui_myFace.Q<VisualElement>("Face");
        }
        else
        {
            memberFace = ui_otherFace.Q<VisualElement>("Face");
        }

        // face type
        switch (face)
        {
            case EFace.Default:
                memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Default - 1]);
                break;
            case EFace.Blush:
                memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Blush - 1]);
                break;
            case EFace.Angry:
                memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Angry - 1]);
                break;
        }

        // change current face of member
        member.currentFace = face;

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
                    case EChatEvent.Vibration:
                        {
                            Debug.Log("진동 시작");
                            Vector3 originalPosition = currentElement.transform.position;
                            Vector3 randomOffset = Vector3.zero;
                            float elapsed = 0f;

                            DoTween = DOTween.To(() => elapsed, x => elapsed = x, 1f, 0.25f)
                                .OnStart(() =>
                                {
                                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                                    float x = strength * Mathf.Cos(randomAngle);
                                    float y = strength * Mathf.Sin(randomAngle);

                                    randomOffset = new Vector3(x, y, 0);
                                })
                                .OnUpdate(() =>
                                {
                                    Vector3 movePos = Vector3.zero;
                                    if (elapsed < (duration / 2))       // 밖으로 나가는 중
                                    {
                                        movePos = Vector3.Lerp(originalPosition, randomOffset, elapsed / duration);
                                    }
                                    else
                                    {
                                        movePos = Vector3.Lerp(randomOffset, originalPosition, elapsed / duration);
                                    }
                                    currentElement.transform.position = movePos;
                                })
                                .OnStepComplete(() =>
                                {
                                    currentElement.transform.position = originalPosition;

                                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                                    float x = strength * Mathf.Cos(randomAngle);
                                    float y = strength * Mathf.Sin(randomAngle);

                                    randomOffset = new Vector3(x, y, 0);
                                    //Debug.Log(randomOffset);
                                })
                                .SetLoops(-1, LoopType.Restart);
                            return;
                        }
                    case EChatEvent.OneVibration:
                        {
                            Vector3 originalPosition = currentElement.transform.position;
                            Vector3 randomOffset = Vector3.zero;
                            float elapsed = 0f;
                            float _duration = 1;
                            float _strength = 40;

                            DOTween.To(() => elapsed, x => elapsed = x, 1f, 0.03f)
                                .OnStart(() =>
                                {
                                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                                    float x = _strength * Mathf.Cos(randomAngle);
                                    float y = _strength * Mathf.Sin(randomAngle);

                                    randomOffset = new Vector3(x, y, 0);
                                    Debug.Log(randomOffset);
                                })
                                .OnUpdate(() =>
                                {
                                    Vector3 movePos = Vector3.zero;
                                    if (elapsed < (_duration / 2))       // 밖으로 나가는 중
                                    {
                                        movePos = Vector3.Lerp(originalPosition, randomOffset, elapsed / _duration);
                                    }
                                    else
                                    {
                                        movePos = Vector3.Lerp(randomOffset, originalPosition, elapsed / _duration);
                                    }
                                    currentElement.transform.position = movePos;
                                })
                                .OnStepComplete(() =>
                                {
                                    currentElement.transform.position = originalPosition;

                                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                                    float x = _strength * Mathf.Cos(randomAngle);
                                    float y = _strength * Mathf.Sin(randomAngle);

                                    randomOffset = new Vector3(x, y, 0);
                                    Debug.Log(randomOffset);
                                })
                                .SetLoops(20, LoopType.Restart);
                        }
                        break;
                }
            }
            // 만약 트윈이 되고 있는 중이라면 꺼주기
            if (DoTween != null && DoTween.IsPlaying())
            {
                DoTween.Kill();
            }
        }
    }

    private void EventChatText(VisualElement chat, string text)
    {
        if (text.Contains("*"))
        {
            string[] segments = text.Split('*');
            bool isHighlight = false;

            foreach (var segment in segments)
            {
                if (string.IsNullOrEmpty(segment))
                {
                    isHighlight = !isHighlight;
                    continue;
                }

                if (isHighlight)
                {
                    Label highlightedLabel = UIReader_Main.Instance.RemoveContainer(ux_highlightedtext.Instantiate()).Q<Label>();
                    highlightedLabel.text = segment;
                    chat.Add(highlightedLabel);
                }
                else
                {
                    Label textLabel = UIReader_Main.Instance.RemoveContainer(ux_text.Instantiate()).Q<Label>();
                    textLabel.text = segment;
                    chat.Add(textLabel);
                }

                isHighlight = !isHighlight;
            }
        }
        else if (text.Contains("/"))
        {
            string[] segments = text.Split('/');
            bool isHyperlink = false;

            foreach (var segment in segments)
            {
                if (isHyperlink)
                {
                    string removeSegment = segment;
                    string insideParentheses = "";

                    if (segment.Contains("[") && segment.Contains("]"))
                    {
                        int startIndex = segment.IndexOf("[") + 1;
                        int endIndex = segment.IndexOf("]");
                        if (startIndex < endIndex)
                        {
                            insideParentheses = segment.Substring(startIndex, endIndex - startIndex);

                            removeSegment = segment.Remove(startIndex - 1, endIndex - startIndex + 2);
                        }
                    }

                    Button textButton = UIReader_Main.Instance.RemoveContainer(ux_button.Instantiate())?.Q<Button>();

                    if (textButton != null)
                    {
                        Label textLabel = textButton.Q<Label>();
                        textLabel.text = removeSegment;

                        textButton.RegisterCallback<MouseEnterEvent>(evt =>
                        {
                            textLabel.style.color = new UnityEngine.Color(98f / 255f, 167f / 255f, 255f / 255f, 255f / 255f);
                        });

                        textButton.RegisterCallback<MouseLeaveEvent>(evt =>
                        {
                            textLabel.style.color = new UnityEngine.Color(0f / 255f, 112f / 255f, 255f / 255f, 255f / 255f);
                        });

                        textButton.RegisterCallback<ClickEvent>(evt =>
                        {
                            UIReader_FileSystem.Instance.HighlightingFolderPathEvent(insideParentheses);
                        });

                        chat.Add(textButton);
                    }
                    else
                    {
                        Debug.LogWarning("Failed to instantiate textButton from RemoveContainer.");
                    }
                }
                else
                {
                    Label textb = UIReader_Main.Instance.RemoveContainer(ux_text.Instantiate())?.Q<Label>();
                    textb.text = segment;
                    chat.Add(textb);
                }

                isHyperlink = !isHyperlink;
            }
        }
        else
        {
            Label textLabel = UIReader_Main.Instance.RemoveContainer(ux_text.Instantiate()).Q<Label>();
            textLabel.text = text;
            chat.Add(textLabel);
        }
    }

    private void RecordChat(EChatState who, string toWho, EChatType type, string msg, bool isQuestion = false)
    {
        // find member
        MemberProfile member = FindMember(toWho);

        // chatting setting
        switch (type)
        {
            case EChatType.Text: break;
            case EChatType.Image: break;
            case EChatType.CutScene:
                {
                    ChatNode chat = ScriptableObject.CreateInstance("ChatNode") as ChatNode;
                    chat.state = who;
                    chat.type = type;
                    chat.chatText = msg;
                    member.chattings.Add(chat);
                }
                break;
            case EChatType.Question:
                {
                    Debug.Log("질문으로 들어옴");
                    //Quesion

                }
                break;
        }
    }

    public void ChangeMemberName(string name)
    {
        ui_otherMemberName.text = name;
    }

    void OnMouseEnterButton(PointerEnterEvent evt)
    {
        isMouseOverButton = true;
    }

    void OnMouseLeaveButton(PointerLeaveEvent evt)
    {
        isMouseOverButton = false;
    }

    // scroll pos setting
    void OnMouseWheel(WheelEvent evt)
    {
        //// working progress is stop
        //evt.StopPropagation();

        //// multitly scroll speed to current delta value
        //float delta = evt.delta.y * wheelSpeed;

        //// scroll pos setting
        //ui_chatGround.scrollOffset += new Vector2(0, delta);



        if (isMouseOverButton)
        {
            // 마우스 휠 이벤트에 따라 ScrollView를 스크롤합니다.
            float delta = evt.delta.y * wheelSpeed;
            ui_chatGround.scrollOffset += new Vector2(0, delta);
            evt.StopPropagation(); // 이벤트 전파를 막습니다.
        }
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
            VisualElement newMember = UIReader_Main.Instance.RemoveContainer(ux_memberList.Instantiate());
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
            GameManager.Instance.chatHumanManager.checkEvidence.Clear();
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

    public void ChangeMyProfile(string name, Sprite face)
    {
        ui_myFace.Q<Label>("Name").text = name;
        ui_myFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
    }

    // member list button on/off
    public void OnOffMemberListButton(bool isOpen)
    {
        if (isOpen)
        {
            ui_memberListButton.style.backgroundImage = new StyleBackground(changeMemberBtnOn);
            ui_memberListButton.pickingMode = PickingMode.Position;
            ui_memberListButton.RemoveFromClassList("translucence");
        }
        else
        {
            ui_memberListButton.style.backgroundImage = new StyleBackground(changeMemberBtnOff);
            ui_memberListButton.pickingMode = PickingMode.Ignore;
            ui_memberListButton.AddToClassList("translucence");
            ui_memberListGround.style.display = DisplayStyle.None;
        }
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
