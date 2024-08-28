using ChatVisual;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct MemberEvidence
{
    public Sprite[] spriteEvidence;
    // Text인 것 적용하기
}

[Serializable]
public class MemberProfile
{
    public string name;
    public ESaveLocation nickName;
    public EFace currentFace;
    public Sprite[] faces;

    public MemberEvidence evidence;

    public bool isOpen;

    public List<ChatNode> chattings = new List<ChatNode>();
    public List<AskNode> questions = new List<AskNode>();
    public Node currentNode;

    public AskNode currentAskNode;
}

public class Chatting : MonoBehaviour
{
    [Header("Member")]
    [SerializeField] List<MemberProfile> members = new List<MemberProfile>();
    public List<MemberProfile> Members { get { return members; } set { members = value; } }

    // memberList arrow sprite
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;
    public float wheelSpeed = 25f;
    public float scrollEndSpeed = 0.15f;

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
    [SerializeField] VisualTreeAsset ux_textFile;
    [SerializeField] VisualTreeAsset ux_memberList;

    // 흔들림 효과 넣어주기
    private Tween DoTween;
    public float duration = 1.0f; // 흔들기 지속 시간
    public float strength = 1; // 얼마나 멀리로 흔들리는지
    private VisualElement currentElement;

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
        UIReader_Main.Instance.RemoveSlider(ui_chatGround);

        // member list hidden
        OnOffMemberList();
        ui_memberListButton.clicked += OnOffMemberList;

        ui_nextChatButton.clicked += () => { GameManager.Instance.chatHumanManager.GoChat(); };
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
            InputChat(member.name, chat.state, chat.type, member.currentFace, chat.chatText, 
                false, chat.type == EChatType.Question);
        }

        StartCoroutine(EndToScroll(scrollEndSpeed));
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
            case EChatType.Chat:
            case EChatType.Question:
                {
                    // create uxml
                    chat = UIReader_Main.Instance.RemoveContainer(ux_chat.Instantiate());
                    chat.name = "chat";
                    if (isQuestion)
                        chat.AddToClassList("Question");
                    EventChatText(chat, text);
                    break;
                }
            case EChatType.Image:
                {
                    // create VisualElement
                    chat = new VisualElement();
                    chat.name = "image";
                    // image size change
                    UIReader_Main.Instance.ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).saveSprite);
                    break;
                }
            case EChatType.Text:
                {
                    // create visualElement
                    chat = UIReader_Main.Instance.RemoveContainer(ux_textFile.Instantiate());
                    chat.name = "textFile";
                    chat.Q<Button>().text = text + ".txt";
                    chat.Q<Button>().clicked += () => { GameManager.Instance.imageSystem.OpenText(null, text); };
                    break;
                }
            case EChatType.CutScene:
                {
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

                    // Create Play Icon
                    chat.Add(new VisualElement());
                    chat.Q<VisualElement>().style.backgroundImage = new StyleBackground(GameManager.Instance.cutScenePlayIcon);

                    // connection click event, play cutscene
                    chat.Q<Button>().clicked += (() =>
                    {
                        GameManager.Instance.cutSceneManager.FindCutScene(text).test = false;
                        GameManager.Instance.cutSceneSystem.PlayCutScene(text);
                    });

                    // play cutScene
                    GameManager.Instance.cutSceneSystem.PlayCutScene(text);

                    break;
                }
        }

        // if you this chat record
        if (isRecord)
            RecordChat(who, toWho, type, text, isQuestion);

        // whose chat style setting        
        if (chat != null)
        {
            if (who == EChatState.Me)
            {   
                chat.AddToClassList("MyChat"); // 여기서 널레퍼
            }
            else
            {
                chat.AddToClassList("OtherChat");
            }
        }
        else
            Debug.Log("chat null임 " + type);

        ui_chatGround.Add(chat);
        currentElement = chat;

        // scroll pos to end
        StartCoroutine(EndToScroll(scrollEndSpeed));
    }

    // input question
    public void InputQuestion(string toWho, bool isLock, AskNode askNode, bool isRecord = false)
    {
        // create chat
        VisualElement chat = null;
        // find member
        MemberProfile member = FindMember(toWho.ToString());
        // chat type
        EChatType type = EChatType.Chat;

        if (!isLock)
        {
            // create uxml
            chat = UIReader_Main.Instance.RemoveContainer(ux_askChat.Instantiate());
            // chat name setting
            //chat.name = askNode.askText;
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
                    ask.is_readThis = false;
                }

                if (askNode.askType != EAskType.Common)
                {
                    if (askNode.askType == EAskType.Answer)
                    {
                        ChatNode parent = askNode.parent as ChatNode;
                        foreach (AskNode ask1 in parent.childList)
                        {
                            ask1.is_readThis = true;
                            ask1.is_UseThis = true;
                        }
                    }
                    else if (askNode.askType == EAskType.NoAnswer)
                        UIReader_Main.Instance.MinusHP();
                }

                // move next human
                if (askNode.textEvent.Count == 1)
                {
                    GameManager.Instance.chatHumanManager.currentNode = askNode.child;
                    GameManager.Instance.chatHumanManager.chapterMember 
                        = GameManager.Instance.chatSystem.FindMember(askNode.LoadNextDialog);

                    GameManager.Instance.chatHumanManager.IsChat(false);

                    AddMember(askNode.LoadNextDialog);
                    ChoiceMember(GameManager.Instance.chatSystem.FindMember(askNode.LoadNextDialog), false);

                    //if (askNode.askText == "*(돌아가자)*")
                    //{
                    //    if (GameManager.Instance.chatHumanManager.currentMember.currentAskNode != null)
                    //        GameManager.Instance.chatHumanManager.currentMember.currentAskNode.is_UseThis = true;
                    //}
                    //else
                    //{
                    //    GameManager.Instance.chatSystem.FindMember(askNode.LoadNextDialog).currentAskNode = askNode;
                    //}
                }
                else
                {
                    GameManager.Instance.chatHumanManager.currentNode = askNode;
                }

                askNode.is_UseThis = true;

                // all question visualelement down
                GameManager.Instance.chatSystem.RemoveQuestion();
                member.questions.Clear();

                // currntNode, member's currentNode change
                if (askNode.child is ConditionNode conditionNode)
                {
                    if (conditionNode.is_AllQuestion)
                    {
                        if (conditionNode.asks.Count > 0)
                        {
                            if (conditionNode.Checkk())
                            {
                                conditionNode.is_UseThis = true;
                                member.currentNode = conditionNode;
                            }
                            else
                            {
                                if (conditionNode.asks[0].parent is ChatNode)
                                {
                                    member.currentNode = (conditionNode.asks[0].parent as ChatNode);
                                }
                                else if (conditionNode.asks[0].parent is ConditionNode)
                                {
                                    ConditionNode parent = conditionNode.asks[0].parent as ConditionNode;
                                    member.currentNode = parent.parentList[0];
                                }
                            }
                        }
                        
                    }
                }

                // chatting start
                GameManager.Instance.chatHumanManager.IsChat(true);

                // question
                type = EChatType.Question;
            });
        }
        else
        {
            // create uxml
            chat = UIReader_Main.Instance.RemoveContainer(ux_hiddenAskChat.Instantiate());
            // chat name setting
            //chat.name = askNode.askText;
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
                    //case EChatEvent.LoadFile:
                    //    {
                    //        FileSO file = GameManager.Instance.fileManager.FindFile(chatNode.loadFileName[i]);
                    //        if (file != null)
                    //            GameManager.Instance.fileSystem.AddFile(file.fileType, file.fileName, file.fileParentName);
                    //        else
                    //            Debug.Log("this file not exist");
                    //    }
                    //    break;
                    case EChatEvent.Vibration:
                        {
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

    // chat event - highligh, hyperlink, whisper
    private void EventChatText(VisualElement chat, string text)
    {
        var segments = ParseTextSegments(text);

        foreach (var (segmentText, highlight, hyperlink, link, whisper) in segments)
        {
            if (whisper)
            {
                AddWhisperText(chat, segmentText);
            }
            else if (highlight)
            {
                AddHighlightedText(chat, segmentText);
            }
            else if (hyperlink)
            {
                AddHyperlinkText(chat, segmentText, link);
            }
            else
            {
                AddNormalText(chat, segmentText);
            }
        }
    }

    private List<(string text, bool isHighlight, bool isHyperlink, string hyperlink, bool isWhisper)> ParseTextSegments(string text)
    {
        var segments = new List<(string, bool, bool, string, bool)>();
        bool isWhisper = false, isHighlight = false, isHyperlink = false;

        foreach (var whisperSegment in text.Split('$'))
        {
            if (isWhisper)
            {
                segments.Add((whisperSegment, false, false, null, true));
            }
            else
            {
                foreach (var segment in whisperSegment.Split('*'))
                {
                    if (isHighlight)
                    {
                        segments.Add((segment, true, false, null, false));
                    }
                    else
                    {
                        segments.AddRange(ParseHyperlinks(segment));
                    }
                    isHighlight = !isHighlight;
                }
            }
            isWhisper = !isWhisper;
        }

        return segments;
    }

    private List<(string text, bool isHighlight, bool isHyperlink, string hyperlink, bool isWhisper)> ParseHyperlinks(string text)
    {
        var segments = new List<(string, bool, bool, string, bool)>();
        bool isHyperlink = false;

        foreach (var segment in text.Split('/'))
        {
            if (isHyperlink)
            {
                string linkText = segment;
                string hyperlink = null;

                if (segment.Contains("[") && segment.Contains("]"))
                {
                    int startIndex = segment.IndexOf("[") + 1;
                    int endIndex = segment.IndexOf("]");
                    hyperlink = segment.Substring(startIndex, endIndex - startIndex);
                    linkText = segment.Remove(startIndex - 1, endIndex - startIndex + 2);
                }

                segments.Add((linkText, false, true, hyperlink, false));
            }
            else
            {
                segments.Add((segment, false, false, null, false));
            }
            isHyperlink = !isHyperlink;
        }

        return segments;
    }

    // Whisper 
    private void AddWhisperText(VisualElement chat, string segmentText)
    {
        for (int i = 0; i < segmentText.Length; i++)
        {
            Label whisperLabel = UIReader_Main.Instance.RemoveContainer(ux_highlightedtext.Instantiate()).Q<Label>();

            float grayScale = 0.2f + (0.7f * i / (segmentText.Length - 1));
            int grayScaleValue = (int)(grayScale * 255);

            whisperLabel.style.color = new Color(grayScaleValue / 255f, grayScaleValue / 255f, grayScaleValue / 255f, 1f);
            whisperLabel.text = segmentText[i].ToString();

            chat.Add(whisperLabel);
        }
    }

    // Highlight
    private void AddHighlightedText(VisualElement chat, string segmentText)
    {
        Label highlightedLabel = UIReader_Main.Instance.RemoveContainer(ux_highlightedtext.Instantiate()).Q<Label>();
        highlightedLabel.text = segmentText;
        chat.Add(highlightedLabel);
    }

    // Hyperlink
    private void AddHyperlinkText(VisualElement chat, string segmentText, string hyperlink)
    {
        Button textButton = UIReader_Main.Instance.RemoveContainer(ux_button.Instantiate())?.Q<Button>();

        if (textButton != null)
        {
            Label textLabel = textButton.Q<Label>();
            textLabel.text = segmentText;

            textButton.RegisterCallback<MouseEnterEvent>(evt =>
            {
                textLabel.style.color = new Color(98f / 255f, 167f / 255f, 255f / 255f, 255f / 255f);
            });

            textButton.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                textLabel.style.color = new Color(0f / 255f, 112f / 255f, 255f / 255f, 255f / 255f);
            });

            textButton.RegisterCallback<ClickEvent>(evt =>
            {
                FileSystem.Instance.HyperLinkEvent(hyperlink);
            });

            chat.Add(textButton);
        }
    }

    // narmal text 
    private void AddNormalText(VisualElement chat, string segmentText)
    {
        Label textLabel = UIReader_Main.Instance.RemoveContainer(ux_text.Instantiate()).Q<Label>();
        textLabel.text = segmentText;
        chat.Add(textLabel);
    }

    private void RecordChat(EChatState who, string toWho, EChatType type, string msg, bool isQuestion = false)
    {
        // find member
        MemberProfile member = FindMember(toWho);

        // chatting setting
        switch (type)
        {
            case EChatType.Chat:
            case EChatType.Image:
            case EChatType.CutScene:
                {
                    ChatNode chat = ScriptableObject.CreateInstance("ChatNode") as ChatNode;
                    chat.state = who;
                    if (isQuestion)
                        chat.type = EChatType.Question;
                    else
                        chat.type = type;
                    chat.chatText = msg;
                    member.chattings.Add(chat);
                }
                break;
            case EChatType.Question:
                {
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
        if (isMouseOverButton)
        {
            // 마우스 휠 이벤트에 따라 ScrollView를 스크롤합니다.
            float delta = evt.delta.y * wheelSpeed;
            ui_chatGround.scrollOffset += new Vector2(0, delta);
            evt.StopPropagation(); // 이벤트 전파를 막습니다.
        }
    }

    // scroll pos to end
    public IEnumerator EndToScroll(float timer)
    {
        yield return new WaitForSeconds(timer);
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
                = new StyleBackground(member.faces[0]);
            // connection click event
            newMember.Q<Button>("ChatMember").clicked += () =>
            {
                ChoiceMember(member, true);
                //GameManager.Instance.chatHumanManager.chapterHuman = GameManager.Instance.chatHumanManager.nowHuman;
            };

            // add member to memberListGround
            ui_memberListGround.Add(newMember);
        }
    }

    // change member
    public void ChoiceMember(MemberProfile member, bool test)
    {
        ////GameManager.Instance.chatHumanManager.currentMember.currentNode = GameManager.Instance.chatHumanManager.currentNode;

        // 다소 수정이 필요하지만 작동하긴함
        if (GameManager.Instance.chatHumanManager.currentMember.currentNode is ChatNode chatNode)
        {
            if (chatNode.type == EChatType.CutScene)
            {
                Debug.Log("컷씬이다!!");
                if (GameManager.Instance.cutSceneManager.FindCutScene(chatNode.chatText))
                {
                    Debug.Log("자동 컷씬");
                    GameManager.Instance.chatHumanManager.currentMember.currentNode = chatNode.childList[0];
                }
            }
        }
        else
        {
            Debug.Log("컷씬아님");
            if (test)
            {
                if (member.currentAskNode != null)
                {
                    if (member.currentNode.is_UseThis == false)
                    {
                        member.currentNode = member.currentAskNode.parent;
                    }
                }
            }
            else
            {
                member.currentNode = member.currentAskNode;
            }
        }

        MemberProfile beforeMember = GameManager.Instance.chatHumanManager.currentMember;
        foreach (AskNode askNode in beforeMember.questions)
            askNode.is_readThis = false;

        // if member isn't null
        if (member != null)
        {
            // change currentMember
            GameManager.Instance.chatHumanManager.checkEvidence.Clear();
            GameManager.Instance.chatHumanManager.StartChat(member.nickName.ToString());

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
    public void MemberList(bool isOpen)
    {
        Debug.Log("이게 챗팅 버튼 여는건가?");

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
