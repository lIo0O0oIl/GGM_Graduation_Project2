using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.EventSystems;
using UnityEditor.Tilemaps;
using JetBrains.Annotations;

public class TextBox : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatBoxParent;
    [SerializeField] TMP_InputField inputField;

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;
    [SerializeField] GameObject speechBalloon;
    [SerializeField] GameObject choiceBalloon;
    [SerializeField] GameObject myChatBox;
    [SerializeField] GameObject otherChatBox;

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    EventSystem evt;

    int myChatCount = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            InputText("조수의 말");

        if (Input.GetKeyDown(KeyCode.T))
            InputText("");

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

        GameObject speech = null;
        if (msg == "")
        {
            speech = Instantiate(choiceBalloon);
            speech.name += "-" + myChatCount;
            myChatCount++;
            speech.GetComponent<Button>().onClick.AddListener(() => ChoiceQuestion());
            speech.GetComponentInChildren<TextMeshProUGUI>().text = "test";
        }
        else
        {
            speech = Instantiate(speechBalloon);
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        speech.transform.SetParent(currentSpeech);
        inputField.text = null;
        LineAlignment();
    }

    public void ChoiceQuestion()
    {
        GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            if (currentSpeech.GetChild(i).name != currentSelectedButton.name)
            {
                currentSelectedButton.GetComponent<Button>().interactable = false;
                Destroy(currentSpeech.GetChild(i).gameObject);
            }
        }
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
