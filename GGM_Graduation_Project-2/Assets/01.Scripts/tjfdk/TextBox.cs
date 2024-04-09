using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

//[Serializable]
//public enum FileType
//{
//    Image = 0,
//    Data
//}

public class TextBox : MonoBehaviour
{
    public static TextBox Instance;

    public int cutTextSize = 20;

    [Header("Object")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatBoxParent;       // 쳇팅이 들어갈 박스의 뿌리. 그룹이랑 싸이즈 필터 컴포넌트 들어가 있음.

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;       // 가장 최근의 대화
    [SerializeField] GameObject speechBalloon_left;      // 말하는 말풍선
    [SerializeField] GameObject speechBalloon_right;      // 말하는 말풍선

    [SerializeField] GameObject imageBackground; // 이미지 내보낼 그거 sprite만 수정하면서 사용
    [SerializeField] GameObject dataBackground; // 얘도 마찬ㄴ가지 ㅇㅇ

    [SerializeField] GameObject choiceBalloon;          // 고르는 말풍선(버튼달린)
    [SerializeField] GameObject myChatBox;          // 내 쳇팅박스
    [SerializeField] GameObject otherChatBox;           // 조수의 쳇팅박스

    public Sprite sprite;
    public string msg;

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    int myChatCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            InputFile(true, sprite, "Image");
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Text txt = gameObject.AddComponent<Text>();
            txt.text = msg;
            InputFile(true, txt, "Data");
        }
    }

    public void InputText(bool user, string msg, bool ask = true)       // user가 true 일면 플레이어가 말하는 것임.
    {
        CutText(ref msg);

        LineAlignment();

        if (currentSpeech == null || isCurrentUser != user)
        {
            GameObject temp = null;
            if (user)
            {
                temp = Instantiate(myChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = true;
            }
            else
            {
                temp = Instantiate(otherChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = false;
            }
            AssistantChatListAdd(temp);     // 만약 조수 대화면 리스트에 추가해라
            LineAlignment();
        }

        GameObject speech = null;
        if (user)
        {
            if (ask == false)
            {
                speech = Instantiate(speechBalloon_right);
            }
            else
            {
                speech = Instantiate(choiceBalloon);
                speech.GetComponent<Button>().onClick.AddListener(() => ChoiceQuestion());
            }
            speech.name += "-" + myChatCount;
            myChatCount++;
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        else
        {
            speech = Instantiate(speechBalloon_left);
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        HiddingText(speech);
        AssistantChatListAdd(speech);       // 조수랑 대화면 리스트에 추가
        speech.transform.SetParent(currentSpeech);
        StartCoroutine(OpenText(speech, user, ask));
        LineAlignment();
    }

    public void InputFile(bool user, UnityEngine.Object file, string _type)       // user가 true 일면 플레이어가 말하는 것임.
    {
        if (currentSpeech == null || isCurrentUser != user)
        {
            GameObject temp = null;
            if (user)
            {
                temp = Instantiate(myChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = true;
            }
            else
            {
                temp = Instantiate(otherChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = false;
            }
            AssistantChatListAdd(temp);     // 만약 조수 대화면 리스트에 추가해라
            LineAlignment();
        }

        GameObject data = null;

        if (_type == "Image")
        {
            data = Instantiate(imageBackground);
            SpriteRenderer sprite = file.GetComponent<SpriteRenderer>();
            data.GetComponent<Image>().sprite = sprite.sprite;
            Vector3 size = sprite.bounds.size;
            data.GetComponent<Transform>().localScale = size;
        }
        else if (_type == "Data")
        {
            data = Instantiate(dataBackground);
            data.GetComponent<TextMeshProUGUI>().text = file.GetComponentInChildren<Text>().text;
        }

        data.transform.SetParent(currentSpeech);
        LineAlignment();
    }

    private void CutText(ref string msg)
    {
        // 텍스트 내려주기 기능 만들기
        // 공백으로 나눠주고 잘리는 부분의 인덱스와 가장 가까운 것을 잡아서 거기서 줄내림을 추가해준다.
        // 그런데 인덱스보다 큰데 한... 5이상이 넘는 줄이면 자르기를 뒤에 것에서 잘라 줄내림을 해준다.

        int cutIndex = cutTextSize;
        int endIndex = 0;

        while (msg.Length > cutIndex)
        {
            if (msg[cutIndex] == ' ')     // 자르려는 곳에 공백이 있으면
            {
                // 0 부터 자르려는 곳까지 자르고 자르려던 곳에서 끝까지 잘라준다.
                msg = $"{msg.Substring(0, cutIndex)}\n{msg.Substring(cutIndex + 1)}";
                endIndex = cutIndex;
            }
            else
            {
                int space = msg.IndexOf(" ", cutIndex);       // 자르려는 곳 뒤에 첫번째로 있는 공백을 찾아준다.
                if (space == -1) space = 300;         // 공백이 안 찾아진다면

                int space2 = msg.LastIndexOf(" ", cutIndex);     // startIndex 부터 20까지 있는 문자열에서 가장 마지막에 있는 공백을 찾아준다.
                
                if (space >= cutIndex + 5)
                {
                    endIndex = space2;
                }
                else
                {
                    if (space < space2)
                    {
                        endIndex = space;
                    }
                    else endIndex = space2;
                }

                msg = $"{msg.Substring(0, endIndex)}\n{msg.Substring(endIndex + 1)}";

            }
            cutIndex = endIndex + cutTextSize;
        }

    }

    private void AssistantChatListAdd(GameObject obj)
    {
        if (ChattingManager.Instance.chats[ChattingManager.Instance.nowLevel].whoSO.humanName == "조수")
        {
            ChattingManager.Instance.assistantChatList.Add(obj);
        }
    }

    public void ChoiceQuestion()
    {
        GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;
        currentSelectedButton.GetComponent<Button>().interactable = false;

        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            if (currentSpeech.GetChild(i).name != currentSelectedButton.name)
            {
                currentSelectedButton.GetComponent<Button>().interactable = false;
                currentSelectedButton.GetComponent<Image>().color = Color.white;
                Destroy(currentSpeech.GetChild(i).gameObject);
            }     // 나머지 친구들 다 지워주기
        }

        StartCoroutine(LineRefresh());

        ChattingManager.Instance.answer(currentSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }

    public void CurrentSpeechColorChange()      // 지금 내가 말하고 있는 친구의 색을 모두 변경
    {
        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            currentSpeech.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    private void LineAlignment()
    {
        StartCoroutine(LineRefresh());
        StartCoroutine(ScrollRectDown());
    }

    private IEnumerator LineRefresh()
    {
        yield return new WaitForSeconds(0.1f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBoxParent);
    }

    private IEnumerator ScrollRectDown()
    {
        yield return new WaitForSeconds(0.1f);
        scrollRect.normalizedPosition = new Vector2(0f, 0);
    }

    private void HiddingText(GameObject _temp)
    {
        _temp.GetComponent<Image>().color = Color.clear;
        _temp.GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
    }

    public IEnumerator OpenText(GameObject _temp, bool _isUser, bool _isAsk)
    {
        yield return new WaitForSeconds(0.25f);
        if (_isUser && _isAsk)
        {
            Color color;
            UnityEngine.ColorUtility.TryParseHtmlString("#CCFFB8", out color);
            _temp.GetComponent<Image>().color = color;
        }
        else
            _temp.GetComponent<Image>().color = Color.white;
        _temp.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
}
