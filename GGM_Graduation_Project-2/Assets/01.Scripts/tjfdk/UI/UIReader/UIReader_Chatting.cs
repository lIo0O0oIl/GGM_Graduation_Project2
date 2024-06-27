using ChatVisual;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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

        // other member profile
    VisualElement ui_otherFace;
    VisualElement ui_myFace;
    Label ui_otherMemberName;

    // template
    [Header("Template")]
    [SerializeField] VisualTreeAsset ux_text;
    [SerializeField] VisualTreeAsset ux_highlightedtext;
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

    private void OpenConnection()
    {
        if (isConnectionOpen)
            UIReader_Main.Instance.connectionPanel.style.display = DisplayStyle.Flex;
        else
            UIReader_Main.Instance.connectionPanel.style.display = DisplayStyle.None;

        isConnectionOpen = !isConnectionOpen;
    }

    private void UXML_Load()
    {
        root = GameObject.Find("Game").GetComponent<UIDocument>().rootVisualElement;

        ui_chatGround = UIReader_Main.Instance.root.Q<ScrollView>("ChatGround");
        ui_questionGround = UIReader_Main.Instance.root.Q<VisualElement>("QuestionGround");
        ui_otherFace = UIReader_Main.Instance.root.Q<VisualElement>("FaceGround").Q<VisualElement>("OtherFace");
        ui_myFace = UIReader_Main.Instance.root.Q<VisualElement>("FaceGround").Q<VisualElement>("MyFace");
        ui_memberListButton = UIReader_Main.Instance.root.Q<Button>("ChangeTarget");
        ui_otherMemberName = UIReader_Main.Instance.root.Q<Label>("TargetName");
        ui_memberListGround = UIReader_Main.Instance.root.Q<VisualElement>("ChatMemberList");
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
    private void RecallChatting(MemberProfile member)
    {
        foreach (ChatNode chat in member.chattings)
        {
            InputChat(member.name, chat.state, chat.type, member.currentFace, chat.chatText, false);
        }

        Invoke("EndToScroll", 0.25f);
    }

    private void GameDown()
    {
        Application.Quit();
    }

    private void TutorialEnd()
    {
        SceneManager.LoadScene("Game");
        GameManager.Instance.GameStart();
    }

    // input chat
    public void InputChat(string toWho, EChatState who, EChatType type,
        EFace face, string text, bool isRecord = true)
    {
        // test
        if (text == "더 나은 빌드로 돌아오겠습니다.")
        {
            Invoke("GameDown", 3f);
        }
        if (text == "튜토리얼은 여기까지 입니다.")
        {
            GameManager.Instance.chatHumanManager.StopChatting();
            GameManager.Instance.chatHumanManager.currentNode = null;
            Invoke("TutorialEnd", 3f);
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
                chat = UIReader_Main.Instance.RemoveContainer(ux_chat.Instantiate());
                chat.name = "chat";

                HighlightingChatText(chat, text);
                break;

            // if Image
            case EChatType.Image:
                // create VisualElement
                chat = new VisualElement();
                chat.name = "image";
                // image size change
                UIReader_Main.Instance.ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).saveSprite);
                break;

            // if CutScene
            case EChatType.CutScene:
                // create Button
                chat = new Button();
                chat.name = "cutScene";
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

        //// chat type
        //switch (type)
        //{
        //    // if Text
        //    case EChatType.Text:
        //        // create uxml
        //        chat = UIReader_Main.Instance.RemoveContainer(ux_chat.Instantiate());
        //        chat.name = "chat";

        //        // chat text setting
        //        chat.Q<Label>().text = text;
        //        break;

        //    // if Image
        //    case EChatType.Image:
        //        // create VisualElement
        //        chat = new VisualElement();
        //        chat.name = "image";
        //        // image size change
        //        UIReader_Main.Instance.ReSizeImage(chat, GameManager.Instance.imageManager.FindPng(text).saveSprite);
        //        break;

        //    // if CutScene
        //    case EChatType.CutScene:
        //        // create Button
        //        chat = new Button();
        //        chat.name = "cutScene";
        //        // change chat style
        //        chat.AddToClassList("FileChatSize");
        //        chat.AddToClassList("NoButtonBorder");
        //        // find first cut of cutscene
        //        ChatNode cutScene = GameManager.Instance.chatHumanManager.currentNode as ChatNode;
        //        GameManager.Instance.chatHumanManager.nowCondition = cutScene.childList[0] as ConditionNode;
        //        // change background to image
        //        Sprite sprite = GameManager.Instance.cutSceneManager.FindCutScene(text).cutScenes[0].cut[0];
        //        chat.style.backgroundImage = new StyleBackground(sprite);
        //        // connection click event, play cutscene
        //        chat.Q<Button>().clicked += (() => { GameManager.Instance.cutSceneSystem.PlayCutScene(text); });
        //        break;
        //}

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
        Invoke("EndToScroll", 0.5f);
    }

    private void HighlightingChatText(VisualElement chat, string text)
    {
        VisualElement speech = chat.Q<VisualElement>("Speech");

        if (text.Contains("/"))
        {
            string[] segments = text.Split('/');
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
                    speech.Add(highlightedLabel);
                }
                else
                {
                    Label textLabel = UIReader_Main.Instance.RemoveContainer(ux_text.Instantiate()).Q<Label>();
                    textLabel.text = segment;
                    speech.Add(textLabel);
                }

                isHighlight = !isHighlight;
            }
        }
        else
        {
            Label textLabel = UIReader_Main.Instance.RemoveContainer(ux_text.Instantiate()).Q<Label>();
            textLabel.text = text;
            speech.Add(textLabel);
        }
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
            chat = UIReader_Main.Instance.RemoveContainer(ux_askChat.Instantiate());
            // chat name setting
            chat.name = askNode.askText;
            // chat text setting
            chat.Q<Label>().text = askNode.askText;
            // connection click event
            chat.Q<Button>().clicked += (() =>
            {
                // add chat
                InputChat(toWho, EChatState.Me, type, member.currentFace, askNode.askText, false);

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
            RecordChat(EChatState.Me, toWho, type, askNode.askText);

        // add visualelement
        ui_questionGround.Add(chat);
    }

    // record chatting
    // ????????type question????????????ㅳ늾??????????濚밸Ŧ援????????饔낅떽??吏??筌뚮?????닿튃????耀붾굝??????????????濚밸Ŧ援욃퐲???
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
  //                  Debug.Log("??遺얘턁??????????鶯??????袁④뎬??");
  ///*              AskNode ask = new AskNode();
  //              ask.askText = msg;
  //              member.questions.Add(ask);*/
  //          }
            break;
        }
    }

    // setting Face and event
    public void SettingChat(MemberProfile member, EChatState who, Node node, EFace face, List<EChatEvent> evts)
    {
        // if current face of member is the same new face
        if (member.currentFace != face)
        {
            // find member face
            VisualElement memberFace = null;

            if (who == EChatState.Me)
            {
                Debug.Log("나");
                member = FindMember("HG");
                memberFace = ui_myFace.Q<VisualElement>("Face");
            }
            else
            {
                Debug.Log("상대");
                memberFace = ui_otherFace.Q<VisualElement>("Face");
            }

            // face type
            switch (face)
            {
                case EFace.Default:
                    Debug.Log(face.ToString() + " " + ((int)EFace.Default));
                    memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Default - 1]);
                    break;
                case EFace.Blush:
                    Debug.Log(face.ToString() + " " + ((int)EFace.Blush));
                    memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Blush - 1]);
                    break;
                case EFace.Angry:
                    Debug.Log(face.ToString() + " " + ((int)EFace.Angry));
                    memberFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Angry - 1]);
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
                                    Debug.Log(randomOffset);
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
                                    Debug.Log(randomOffset);
                                })
                                .SetLoops(-1, LoopType.Restart);
                        }
                        break;
                }
            }
            if (evts.Count <= 0/* || evts.Contains(EChatEvent.Vibration)*/)
            {
                // 만약 트윈이 되고 있는 중이라면 꺼주기
                if (DoTween != null && DoTween.IsPlaying())
                {
                    DoTween.Kill();
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
