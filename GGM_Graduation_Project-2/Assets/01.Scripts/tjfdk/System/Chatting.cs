using ChatVisual;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct MemberEvidence
{
    public Sprite[] spriteEvidence;
    // Text인 것 적용하기
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
    public UIDocument document;
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
    private Tween chatEventDTW;
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
        root = document.rootVisualElement;
        UXML_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        root = GameObject.Find("Game").GetComponent<UIDocument>().rootVisualElement;

        ui_chatGround = root.Q<ScrollView>("ChatGround");
        ui_questionGround = root.Q<VisualElement>("QuestionGround");
        ui_otherFace = root.Q<VisualElement>("FaceGround").Q<VisualElement>("OtherFace");
        ui_myFace = root.Q<VisualElement>("FaceGround").Q<VisualElement>("MyFace");
        ui_memberListButton = root.Q<Button>("ChangeTarget");
        ui_nextChatButton = root.Q<Button>("NextChatBtn");
        ui_otherMemberName = root.Q<Label>("TargetName");
        ui_memberListGround = root.Q<VisualElement>("ChatMemberList");
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
        foreach (Chat chat in member.recode)
            InputChat(member.name, chat.who, chat.type, chat.text, false);

        StartCoroutine(EndToScroll(scrollEndSpeed));
    }

    // input chat
    public void InputChat(string toWho, EChatState who, EChatType type,
        string text, bool isRecord = true)
    {
        // create chat
        VisualElement chat = null;

        // chat type
        switch (type)
        {
            case EChatType.Chat:
                // create uxml
                chat = UIReader_Main.Instance.RemoveContainer(ux_chat.Instantiate());
                chat.name = "chat";                    
                EventChatText(chat, text);
                break;
            case EChatType.Question:
                // create uxml
                chat = UIReader_Main.Instance.RemoveContainer(ux_chat.Instantiate());
                chat.name = "question";
                AddNormalText(chat, text);
                chat.AddToClassList("Question");
                break;
            case EChatType.Image:
                // create VisualElement
                chat = new VisualElement();
                chat.name = "image";
                // image size change
                UIReader_Main.Instance.ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).saveSprite);
                break;
            case EChatType.Text:
                // create visualElement
                chat = UIReader_Main.Instance.RemoveContainer(ux_textFile.Instantiate());
                chat.name = "textFile";
                chat.Q<Button>().text = text + ".txt";
                chat.Q<Button>().clicked += () => { GameManager.Instance.imageSystem.OpenText(null, text); };
                break;
            case EChatType.CutScene:
                // create Button
                chat = new Button();
                chat.name = "cutScene";
                
                // change chat style
                chat.AddToClassList("FileChatSize");
                chat.AddToClassList("NoButtonBorder");
                
                //// find first cut of cutscene
                //ChatNode cutScene = GameManager.Instance.chatHumanManager.currentNode as ChatNode;
                //GameManager.Instance.chatHumanManager.nowCondition = cutScene.childList[0] as ConditionNode;

                // change background to image
                Sprite sprite = GameManager.Instance.cutSceneManager.FindCutScene(text).cutScenes[0].cut[0];
                chat.style.backgroundImage = new StyleBackground(sprite);
                
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

        // if you this chat record
        if (isRecord)
            RecordChat(who, toWho, type, text);

        // whose chat style setting        
        if (chat != null)
        {
            if (who == EChatState.Me)
                chat.AddToClassList("MyChat");
            else
                chat.AddToClassList("OtherChat");
        }
        else
            Debug.Log("chat null임 " + type);

        ui_chatGround.Add(chat);
        currentElement = chat;

        // scroll pos to end
        StartCoroutine(EndToScroll(scrollEndSpeed));
    }

    // input question
    public void InputQuestion(string toWho, bool isLock, Question ask)
    {
        // create chat
        VisualElement chat = null;
        // find member
        MemberProfile member = FindMember(toWho);

        if (!isLock)
        {
            // create uxml
            chat = UIReader_Main.Instance.RemoveContainer(ux_askChat.Instantiate());
            // chat text setting
            chat.Q<Label>().text = ask.text;
            // connection click event
            chat.Q<Button>().clicked += (() =>
            {
                ask.isUse = true;

                // add chat
                InputChat(toWho, EChatState.Me, EChatType.Question, ask.text);

                // current question value list
                for (int i = 0; i < member.questions.Count; ++i)
                {
                    if (member.questions[i].text == ask.text)
                        member.questions.RemoveAt(i);
                }

                // other question read false
                foreach (Question ask in member.questions)
                    ask.isRead = false;

                // all question visualelement down
                GameManager.Instance.chatSystem.RemoveQuestion();
                //member.questions.Clear();

                // type
                if (ask.type == EAskType.All)
                {
                    bool is_allClear = true;
                    foreach (Question q in member.questions)
                    {
                        if (q.isUse == false)
                        {
                            is_allClear = false;
                            break;
                        }
                    }

                    // 수정이 필요함..?.
                    // 결과에 따라 current index를 잘 조절한다...
                    if (is_allClear)
                    {
                        // ask index++;
                        member.currentAskIdx++;
                    }
                    else
                    {
                        // chat index++;
                        member.currentIdx++;
                    }
                }
                else if (ask.type == EAskType.Lock)
                    Debug.LogError("Lock type의 question이 있을 수 없음");
                // answer or inanswer
                else
                {
                    // when correct
                    if (ask.type == EAskType.Answer)
                    {
                        // the rest of the question
                        foreach (Question otherAsk in member.questions)
                        {
                            // check
                            otherAsk.isRead = true;
                            otherAsk.isUse = true;
                        }

                        UIReader_Main.Instance.PlusHP();
                    }
                    // when incorrect
                    else if (ask.type == EAskType.NoAnswer)
                        UIReader_Main.Instance.MinusHP();
                }

                // move next human
                if (!string.IsNullOrEmpty(ask.nextName))
                {
                    //GameManager.Instance.chatHumanManager.currentNode = ask.child; // 음
                    GameManager.Instance.chatHumanManager.chapterMember 
                        = GameManager.Instance.chatSystem.FindMember(ask.nextName);

                    GameManager.Instance.chatHumanManager.IsChat(false);

                    AddMember(ask.nextName);
                    ChoiceMember(GameManager.Instance.chatSystem.FindMember(ask.nextName), false);
                }
                else
                {
                    GameManager.Instance.chatHumanManager.currentNode = ask;
                }

                // 아래 코드 지우말아주세요 보면서 수정해야함
                //// all question visualelement down
                //GameManager.Instance.chatSystem.RemoveQuestion();
                //member.questions.Clear();

                //// currntNode, member's currentNode change
                //if (ask.child is ConditionNode conditionNode)
                //{
                //    if (conditionNode.is_AllQuestion)
                //    {
                //        if (conditionNode.asks.Count > 0)
                //        {
                //            if (conditionNode.Checkk())
                //            {
                //                conditionNode.is_UseThis = true;
                //                member.currentNode = conditionNode;
                //            }
                //            else
                //            {
                //                if (conditionNode.asks[0].parent is ChatNode)
                //                {
                //                    member.currentNode = (conditionNode.asks[0].parent as ChatNode);
                //                }
                //                else if (conditionNode.asks[0].parent is ConditionNode)
                //                {
                //                    ConditionNode parent = conditionNode.asks[0].parent as ConditionNode;
                //                    member.currentNode = parent.parentList[0];
                //                }
                //            }
                //        }
                        
                //    }
                //}

                GameManager.Instance.chatHumanManager.IsChat(true);
            });
        }
        else
        {
            // create uxml
            chat = UIReader_Main.Instance.RemoveContainer(ux_hiddenAskChat.Instantiate());
        }

        // add visualelement
        ui_questionGround.Add(chat);
    }

    // setting Face and event
    public void SettingChat(MemberProfile member, EChatState who, EFace face, string[] files, EChatEvent evt)
    {
        // find member face
        VisualElement memberFace = null;

        if (who == EChatState.Me)
        {
            if (FindMember("HG") != null)
                member = FindMember("HG");

            memberFace = ui_myFace.Q<VisualElement>("Face");
            memberFace.RemoveFromClassList("defaultSize");
            memberFace.AddToClassList("talkingSize");
        }
        else
        {
            memberFace = ui_myFace.Q<VisualElement>("Face");
            memberFace.AddToClassList("defaultSize");
            memberFace.RemoveFromClassList("talkingSize");
        }

        if (who == EChatState.Other)
        {
            memberFace = ui_otherFace.Q<VisualElement>("Face");
            memberFace.RemoveFromClassList("defaultSize");
            memberFace.AddToClassList("talkingSize");
        }
        else
        {
            memberFace = ui_otherFace.Q<VisualElement>("Face");
            memberFace.AddToClassList("defaultSize");
            memberFace.RemoveFromClassList("talkingSize");
        }

        if (who == EChatState.Me)
            memberFace = ui_myFace.Q<VisualElement>("Face");
        else
            memberFace = ui_otherFace.Q<VisualElement>("Face");

        // face type
        switch (face)
        {
            case EFace.Normal:
                memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Normal - 1]);
                break;
            case EFace.Shy:
                memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Shy - 1]);
                break;
            case EFace.Angry:
                memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Angry - 1]);
                break;
        }

        // change current face of member
        member.currentFace = face;

        if (files != null)
        {
            foreach (string fileName in files)
            {
                if (string.IsNullOrEmpty(fileName)) continue;

                FileSO file = GameManager.Instance.fileManager.FindFile(fileName);
                if (file != null)
                    GameManager.Instance.fileSystem.AddFile(file.fileType, file.fileName, file.fileParentName);
                else
                    Debug.Log("this file not exist");
            }
        }
            //// Evnet
        // 만약 트윈이 되고 있는 중이라면 꺼주기 (이전 것이기 때문..)
        if (chatEventDTW != null && chatEventDTW.IsPlaying())
        {
            chatEventDTW.Complete();
            chatEventDTW.Kill();
        }

        // event type
        switch (evt)
        {
            case EChatEvent.Vibration:
                {
                    Vector3 originalPosition = currentElement.transform.position;
                    Vector3 randomOffset = Vector3.zero;
                    float elapsed = 0f;

                    chatEventDTW = DOTween.To(() => elapsed, x => elapsed = x, 1f, 0.25f)
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
            case EChatEvent.Camera:
                {
                    // coming soon...
                }
                break;
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

    private List<(string text, bool isHighlight, bool isHyperlink, string hyperlink, bool isWhisper)> 
        ParseTextSegments(string text)
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

    private List<(string text, bool isHighlight, bool isHyperlink, string hyperlink, bool isWhisper)> 
        ParseHyperlinks(string text)
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
                textLabel.style.color = new Color(42f / 255f, 152f / 255f, 219f / 255f, 255f / 255f);
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

    private void RecordChat(EChatState who, string toWho, EChatType type, string text/*, bool isQuestion = false*/)
    {
        // find member
        MemberProfile member = FindMember(toWho);

        Chat chat = new Chat();
        chat.who = who;
        chat.type = type;
        chat.text = text;
        member.recode.Add(chat);
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

    public void ChoiceMember(MemberProfile member, bool is_chapter)
    {
        // 사람이 바뀔 때 해야할 것...

        // 1, 챕터 변경인가? (스토리 상의 사람 변경인가?)
        // 네 - 현재 멤버의 대화 상태를 저장해야함, 그리고 IsChat true 풀어야함 + 화살표 기능 false
        // 아뇨 - 현재 멤버의 대화 상태를 저장해야함, 그리고 IsChat false로 잠궈둬야함 + 화살표 기능 true + 현재 mem의 ask 전부 is_read = false
        // 2, 인물을 변경해야함 (대화를 전부 지우고 새로 작성, 사람 이미지를 변경)
        // 3, 

        if (is_chapter)
        {
            GameManager.Instance.chatHumanManager.IsChat(true);
            OnOffMemberList();
        }
        else
        {
            GameManager.Instance.chatHumanManager.IsChat(false);
            OnOffMemberList();
        }

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

    //// change member -- 지우지 마십시오
    //public void ChoiceMember(MemberProfile member, bool test)
    //{
    //    ////GameManager.Instance.chatHumanManager.currentMember.currentNode = GameManager.Instance.chatHumanManager.currentNode;

    //    // 다소 수정이 필요하지만 작동하긴함
    //    if (GameManager.Instance.chatHumanManager.currentMember.currentNode is ChatNode chatNode)
    //    {
    //        if (chatNode.type == EChatType.CutScene)
    //        {
    //            Debug.Log("컷씬이다!!");
    //            if (GameManager.Instance.cutSceneManager.FindCutScene(chatNode.chatText))
    //            {
    //                Debug.Log("자동 컷씬");
    //                GameManager.Instance.chatHumanManager.currentMember.currentNode = chatNode.childList[0];
    //            }
    //            else
    //            {
    //                Debug.Log("수동 컷씬");
    //                //GameManager.Instance.chatHumanManager.currentMember.currentNode = chatNode.childList[0];
    //            }

    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("컷씬아님");
    //        if (test)
    //        {
    //            if (member.currentAskNode != null)
    //            {
    //                if (member.currentNode.is_UseThis == false)
    //                {
    //                    member.currentNode = member.currentAskNode.parent;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            member.currentNode = member.currentAskNode;
    //        }
    //    }

    //    MemberProfile beforeMember = GameManager.Instance.chatHumanManager.currentMember;
    //    foreach (AskNode askNode in beforeMember.questions)
    //        askNode.is_readThis = false;

    //    // if member isn't null
    //    if (member != null)
    //    {
    //        // change currentMember
    //        GameManager.Instance.chatHumanManager.checkEvidence.Clear();
    //        GameManager.Instance.chatHumanManager.StartChat(member.nickName.ToString());

    //        // change profile
    //        ChangeProfile(member.name, member.faces[(int)member.currentFace]);
    //        // off memberListGround
    //        isMemberListOpen = false;
    //        OnOffMemberList();
    //        // remove all chat and question
    //        RemoveChatting();
    //        RemoveQuestion();

    //        // recall chat and question
    //        RecallChatting(member);
    //    }
    //}

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
    public void OnOffMemberList(/*bool is_open*/)
    {
        isMemberListOpen = !isMemberListOpen;

        if (!isMemberListOpen)
        {
            ui_memberListButton.style.backgroundImage = new StyleBackground(changeMemberBtnOn);
            ui_memberListGround.style.display = DisplayStyle.None;
            isMemberListOpen = false;
        }
        else
        {
            ui_memberListButton.style.backgroundImage = new StyleBackground(changeMemberBtnOff);
            ui_memberListGround.style.display = DisplayStyle.Flex;
            isMemberListOpen = true;
        }
    }
}
