using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
    public Button changeMemberButton;
    public bool isMemberListOpen;
    public VisualElement memberList;
    Label memberName;

    // template
    VisualTreeAsset ux_chat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;
    VisualTreeAsset ux_memberList;

    public bool isChapterProcessing;
    ScrollView scrollView;

    private void Awake()
    {
        base.Awake();

        MinWidth = 100;
        MinHeight = 100f;
        MaxWidth = 500;
        MaxHeight = 500;
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        base.OnEnable();

        Template_Load();
        UXML_Load();
        Event_Load();
    }

    void OnMouseWheel(WheelEvent evt)
    {
        Debug.Log("?????濚?????怨쀪퐨??");

        // ??れ삀??????袁⑹뵫?????濚??癲ル슪?ｇ몭???袁⑸젻泳?
        evt.StopPropagation();

        // ???嶺? ??좊즴??????袁⑹뵫??????뽦뵣 ???ㅼ굣??
        float delta = evt.delta.y * 500f;

        // ???袁⑹뵫????ш끽維???釉뚰???
        scrollView.scrollOffset += new Vector2(0, delta);
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
        scrollView = chatGround.Q<ScrollView>(chatGround.name);

        // 癲ル슢???볥뼀??怨뚮뼚????類????
        ChangeMember();
        changeMemberButton.clicked += ChangeMember;

        // 癲???????袁⑹뵫?棺堉???????뽦뵣 ?怨뚮뼚???
        scrollView.RegisterCallback<WheelEvent>(OnMouseWheel);

        //ScrollView scrollView = chatGround.Q<ScrollView>(chatGround.name);
        //scrollView.RegisterCallback<GeometryChangedEvent>(evt => EndToScroll());
        //Debug.Log(chatGround.Q<ScrollView>(chatGround.name).verticalPageSize);
        //chatGround.Q<ScrollView>(chatGround.name).verticalPageSize = 500f;
        //Debug.Log(chatGround.Q<ScrollView>(chatGround.name).verticalPageSize);
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

        for (int i = questionGround.childCount - 1; i >= 0; i--)
            questionGround.RemoveAt(i);
    }

    private void RecallChatting(MemberChat otherName)
    {
        memberName.text = otherName.name;

        foreach (Chatting chat in otherName.chattings)
            InputChat(chat.who, chat.toWho, chat.chatType, otherName.face, chat.text, false);

        foreach (Chatting chat in otherName.quetions)
            InputQuestion(chat.toWho, chat.chatType, chat.text, false, null);

        Invoke("EndToScroll", 0.5f);
    }

    // ??숆강?????ш낄猷???筌?????숆강???????ш퐨??釉먮폁????????앗낆땡??낆쑋??????ル쵐???????癲ル슢???????縕????れ삀?????? ????곕럡...
    //private void Te(VisualElement chat, Sprite sprite)
    //{
    //    chat.style.backgroundImage = new StyleBackground(sprite);
    //    float size = 0;

    //    if (sprite.rect.width >= sprite.rect.height)
    //        size = sprite.rect.width;
    //    else
    //        size = sprite.rect.height;

    //    if (size < 100)
    //        size = 3;
    //    else if (size < 150)
    //        size = 2;
    //    else
    //        size = 1;

    //    chat.style.width = sprite.rect.width * size;
    //    chat.style.height = sprite.rect.height * size;
    //}

    public void InputChat(EChatState who, ESaveLocation toWho, EChatType type, EFace face, 
        string msg, bool isRecord, EChatEvent evt = EChatEvent.Default)
    {
        // ??獄쏅똻??
        VisualElement chat = null;
        MemberChat suspect = FindMember(toWho.ToString());

        // ?????嶺뚮Ĳ?됭린?
        switch (type)
        {
            case EChatType.Text:
                chat = ux_chat.Instantiate();
                chat.Q<Label>().text = msg;
                break;
            case EChatType.Image:
                chat = new VisualElement();
                ReSizeImage(chat, imageManager.FindPNG(msg).image);
                break;
            case EChatType.CutScene:
                chat = new Button();
                chat.AddToClassList("FileChatSize");
                chat.AddToClassList("NoButtonBorder");
                Sprite sprite = cutSceneManager.FindCutScene(msg).cutScenes[0].cut[0];
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

        // ????????겾??
        chatGround.Add(chat);
        Invoke("EndToScroll", 0.5f);
    }

    public void InputQuestion(ESaveLocation toWho, EChatType type, string msg, bool isRecord, IEnumerator reply, Action action = null)
    {
        // ??獄쏅똻??
        VisualElement chat = null ;
        MemberChat suspect = FindMember(toWho.ToString());

        // ?????嶺뚮Ĳ?됭린?
        switch (type)
        {
            case EChatType.Question:
                chat = RemoveContainer(ux_askChat.Instantiate());
                chat.name = msg;
                //chat.name = msg;
                chat.Q<Label>().text = msg;
                //chat.Q<Button>().clicked += action; // ???????쒓낫???
                chat.Q<Button>().clicked += (() => 
                { 
                    chat.parent.Remove(chat);
                    for (int i = 0; i < suspect.quetions.Count - 1; ++i)
                    {
                        if (suspect.quetions[i].text == msg)
                        {
                            Debug.Log("癲ル슣??袁ｋ즵 ?????");
                            suspect.quetions.Remove(suspect.quetions[i]);
                        }
                    }
                    // reply ??⑥レ툓??
                    if (reply != null)
                        StartCoroutine(reply);
                    action?.Invoke();
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

        // ????釉먯뒠????⑤베堉?
        questionGround.Add(chat);
    }

    private void RecordChat(MemberChat member, EChatState who, ESaveLocation toWho, EChatType type, string msg)
    {
        // ??れ삀??쎈뭄?
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

    //private void ChangeT(MemberChat member, string msg)
    //{
    //    ??筌믨퀣???怨뚮뼚???
    //    if (member.face != face)
    //    {
    //        VisualElement suspectFace = chattingFace.Q<VisualElement>("Face");
    //        switch (face)
    //        {
    //            case EFace.Default:
    //                suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Default]);
    //                break;
    //            case EFace.Blush:
    //                suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Blush]);
    //                break;
    //            case EFace.Difficult:
    //                suspectFace.style.backgroundImage = new StyleBackground(member.faces[(int)EFace.Difficult]);
    //                break;
    //        }

    //        member.face = face;
    //    }

    //    ???濚??
    //    switch (evt)
    //    {
    //        case EChatEvent.Vibration:
    //            break;
    //        case EChatEvent.Round:
    //            break;
    //        case EChatEvent.Camera:
    //            break;
    //    }
    //}
    public void EndToScroll()
    {
        scrollView.verticalScroller.value = scrollView.verticalScroller.highValue;
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
            //memberChats.Add(new MemberChat(memberName));
        }
    }

    public void ChangeMember()
    {
        isMemberListOpen = !isMemberListOpen;

        if (isMemberListOpen)
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
            ChangeMember(); // ?????癲ル슢?꾤땟戮⑤뭄?????됀?
            RemoveChatting(); // 癲???????ル깼???
            RecallChatting(member); // ????궈????ㅼ뒭??

            if (member.chapterName != "")
            {
                chapterManager.ChatStart(/*member.name, */member.chapterName);
            }
            else
                Debug.Log("癲?甕겹끂?사??놁씀? ????룹젂???源낆쓱");
        }
    }
}
