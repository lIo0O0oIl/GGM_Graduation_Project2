using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ChatVisual;       // ?섏쨷??吏?곌린

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    [Header("ChattingContainer")]
    public GameObject chatContainer;        // 爾뉙똿???닿릿 怨녹엫.
    public TMP_Text chattingHumanName;
    [HideInInspector]
    public List<GameObject> assistantChatList = new List<GameObject>();    // 議곗닔? ?섎늿 ??붾뒗 ??ν빐二쇨린
    //[SerializeField] private Chapter[] chapters;      // 爾뉙똿 SO?ㅼ쓣 ?ｌ뼱以?
    //public Chapter[] Chapters {  get { return chapters; } set { chapters = value; } }

    [Header("ChatDelay")]       // 爾뉙똿 ?쒕젅??愿??
    public float delayTime = 0.75f;
    private float currentTime = 0.0f;
    private bool is_Chatting = false;       // 梨쀭똿???섎뒗 以묒씠?쇰㈃

    [Header("ChatCount")]       // 吏湲?爾뉙똿???쇰쭏??吏꾪뻾?섏뿀?붿?
    [HideInInspector]
    public int nowLevel = 0;            // ?꾩옱 爾뉙똿???덈꺼
    private int nowChatIndex = 0;            // ?꾩옱 爾뉙똿 ?몃뜳??

    [Header("Ask")]     // 臾쇱뼱蹂대뒗 寃?愿??
    private int askLenght = 0;
    private bool is_Choosing;       // ?좏깮吏媛 ?덉뼱???좏깮以묒씪 ?? 硫덉떠?덈뒗 ?쒓컙??留먰븯??寃?
    private bool is_AskChat;
    private int nowAskLevel = 0;        // 吏湲?吏덈Ц???덈꺼   
    private int nowAskChatIndex = 0;        // 吏湲???듭쓽 ?몃뜳??
    private List<string> notUseAskList = new List<string>();

    private void Start()
    {
        Instance = this;
    }

    private void Update()       // 爾뉙똿 ?쒖뒪??
    {
        if (is_Chatting && !is_Choosing)
        {
            currentTime += Time.deltaTime;
            if (currentTime > delayTime || Input.GetMouseButtonDown(0))     // ?쇱そ 踰꾪듉???뚮??ㅻ㈃
            {
                if (is_AskChat) AskChapter();       // 吏덈Ц??????듭쓣 異쒕젰??
                else Chapter();

                currentTime = 0.0f;
            }
        }
    }

    public void StartChatting(int index)
    {
        Debug.Log("泥댄똿???쒖옉?? + index");
        // 留뚯빟 ??爾뉙똿??爰쇱졇?덉쑝硫?耳쒖쭏 ?뚭퉴吏???湲??묒뀡?쇰줈??
        // 梨쀭똿??耳쒖?硫??≪뀡?쇰줈 ?ㅼ떆 ???⑥닔瑜?遺瑜닿쾶 ?쒕떎?
        //if (UIManager.Instance.panels[0].activeSelf == false)
        //{
        //    UIManager.Instance.alarmIcon.SetActive(true);
        //    UIManager.Instance.chatIndex = index;
        //    UIManager.Instance.startChatEvent += (chatIndex) => StartChatting(chatIndex);
        //    return; 
        //}

        nowChatIndex = 0;
        nowAskChatIndex = 0;
        nowLevel = index;

        // 爾뉙똿李??뺣낫 ?ㅼ젙?댁＜湲?
       /* if (chattingHumanName.text != chapters[index].who)     // ?ㅻⅨ ?щ엺怨???붾? ?섎뒗 寃껋씠?쇰㈃
        {
            // 吏湲덇퉴吏 ?덈뜕 ?????吏?뚯＜湲?
            for (int i = 0; i < chatContainer.transform.childCount; i++)
            {
                chatContainer.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (chattingHumanName.text != "議곗닔")      // ?⑹쓽?먮옉 ??뷀븳 ?댁뿭?댁??ㅻ㈃ ????댁뿭???뚯씪??png 濡???ν븯怨?????댁뿭? 紐⑤몢 吏?대떎. 洹몃━怨?議곗닔????붾떎 耳쒖＜湲?
            {
                Debug.Log("?⑹쓽??????댁뿭 ?ъ쭊?앹쑝濡???ν빐二쇨린!");
                for (int i = 0; i < assistantChatList.Count; i++)
                {
                    if (assistantChatList[i].gameObject != null)
                    {
                        assistantChatList[i].gameObject.SetActive(true);
                    }
                }
            }

            //chattingHumanName.text = chapters[index].who;      // ?대쫫 ?ｌ뼱二쇨린
        }*/

        //int chatLenght = chapters[index].chat.Count;       // 爾뉙똿?ㅼ쓽 湲몄씠
        //askLenght = chapters[index].askAndReply.Count;      // 吏덈Ц?ㅼ쓽 媛쒖닔

        is_Chatting = true;
    }

    private void Chapter()
    {
       // if (is_Choosing == false && nowChatIndex < chapters[nowLevel].chat.Count)        // ?좏깮以묒씠 ?꾨땲?쇰㈃
       // {
            bool state = false;       // 議곗닔?몄? ?뚮젅?댁뼱(?뺤궗) ?몄? ?뺣??? 1???뚮젅?댁뼱??
            //switch (chapters[nowLevel].chat[nowChatIndex].state)
         //   {
                /*case E_ChatState.Other:
                    state = false;
                    break;
                case E_ChatState.Me:
                    state = true;
                    break;
                case E_ChatState.Ask:
                    notUseAskList.Clear();      // ?꾩뿉 ?덈뜕 寃?紐⑤몢 吏?뚯＜湲?
                    for (int i = 0; i < askLenght; i++)
                    {
                        //TextBox.Instance.InputText(true, chapters[nowLevel].chat[nowChatIndex].text, true);
                        notUseAskList.Add(chapters[nowLevel].chat[nowChatIndex].text);            // 吏덈Ц??異붽?
                        nowChatIndex++;
                    }
                    is_Choosing = true;
                    return;
                case E_ChatState.LoadNext:
                    Debug.LogError("?꾩쭅 留뚮뱾吏 ?딅뒗 LoadNext ?덉슂.");
                    return;     // ?꾩삁 ?뚮젮*/
                //default:
                    //Debug.LogError($"{chapters[nowLevel].chat[nowChatIndex].state} ???) ?녿뒗 ?좏삎?댁삁??");
                    //break;
        //    }
            //TextBox.Instance.InputText(state, chapters[nowLevel].chat[nowChatIndex].text, false);
            //nowChatIndex++;
       // }
        //else
        //{
            is_Chatting = false;
            //Debug.LogError($"?닿쾶 ???섏?, ?좏깮以? : {is_Choosing}, 吏湲??대뵒?? : {nowLevel}, 吏湲?梨꾪똿?? : {nowChatIndex}");
        //}
    }

    public void answer(string str)     // 踰꾪듉???대┃?덉쓣 ??
    {
        //Debug.Log(str);
        //TextBox.Instance.CurrentSpeechColorChange();
        //for (int i = 0; i < notUseAskList.Count; i++)
        //{
        //    if (notUseAskList[i] == str)        // ?닿쾬???レ옄?뚮Ц???낅젰?먯꽌 ?섎굹????듬쭔???섏삤??寃껋엫.
        //    {
        //        nowAskLevel = i;
        //        nowAskChatIndex = 0;
        //        notUseAskList[i] = "";

        //        is_AskChat = true;
        //        is_Choosing = false;
        //    }
        //}
    }

    private void AskChapter()
    {
        /*if (nowAskChatIndex < chapters[nowLevel].askAndReply[nowAskLevel].reply.Count)
        {
            //TextBox.Instance.InputText(false, chapters[nowLevel].askAndReply[nowAskLevel].reply[nowAskChatIndex]);
            nowAskChatIndex++;
        }
        else
        {
            is_AskChat = false;

            if (notUseAskList.Count == 0)       // ??吏덈Ц??寃껋씠 ?놁쑝硫?
            {
                is_Choosing = false;
                return;
            }*/

            //for (int i = 0; i < notUseAskList.Count; i++)
            //{
            //    TextBox.Instance.InputText(true, notUseAskList[i], true);
            //}
            is_Choosing = true;
        }

    public void ChangeDelaySpeed(float _value)
    {
        delayTime = _value;
    }       // 爾뉙똿 ?쒕젅???쒓컙 蹂寃?
}

