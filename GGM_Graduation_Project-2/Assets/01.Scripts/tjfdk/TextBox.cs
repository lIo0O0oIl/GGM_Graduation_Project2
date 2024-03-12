using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.EventSystems;

public class TextBox : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatBoxParent;
    [SerializeField] TMP_InputField inputField;

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;
    [SerializeField] GameObject speechBalloon;
    [SerializeField] GameObject myChatBox;
    [SerializeField] GameObject otherChatBox;

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    EventSystem evt;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            InputText("야이 개1새야");

        if (inputField.isFocused == false)
            inputField.OnPointerClick(new PointerEventData(evt));
    }

    public void InputText(string msg = null)
    {
        bool user;
        if (msg == "")
            user = true;
        else
            user = false;

        if (currentSpeech == null || isCurrentUser != user)
        {
            if (user)
            {
                GameObject temp = Instantiate(myChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = true;
            }
            else
            {
                GameObject temp = Instantiate(otherChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = false;
            }
        }

        GameObject speech = Instantiate(speechBalloon);
        if (msg == "")
            speech.GetComponentInChildren<TextMeshProUGUI>().text = inputField.text;
        else
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        speech.transform.SetParent(currentSpeech);
        inputField.text = null;
        LineAlignment();
    }

    private void LineAlignment()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBoxParent);
        StartCoroutine(ScrollRectDown());
    }

    private IEnumerator ScrollRectDown()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0;
    }
}
