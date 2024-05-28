using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public class Question
{
    public string how;
    public string msg;
    public List<Dialog> answers = new List<Dialog>();
}

[Serializable]
public class Dialog
{
    public string how;
    public string msg;
    public bool isUser;
    public bool isQuestion;
    public List<Question> questions = new List<Question>();
}

public class ChatManager : MonoBehaviour
{
    UIReader_Chatting chatting;

    public int chatIndex;
    public float chatDelay;
    public List<Dialog> dialogs = new List<Dialog>();

    private void Start()
    {
        chatting = GetComponent<UIReader_Chatting>();
        //InvokeRepeating(nameof(AddChat), chatDelay, chatDelay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            AddChat();
    }

    private void AddChat()
    {
        Dialog dialog = dialogs[chatIndex];

        chatIndex++;

        if (dialog.msg != "")
        {
            chatting.InputChat(true, dialog.isUser, chatting.FindMember(dialog.how), dialog.msg);

            if (dialog.isQuestion)
            {
                foreach (Question q in dialog.questions)
                {
                    chatting.InputQuestion(true, true, chatting.FindMember(dialog.how), q.msg,
                        (() => 
                        { 
                            // 질문 누르면 걍 대화로 넘겨주고
                            chatting.InputChat(true, !dialog.isUser, chatting.FindMember(dialog.how), q.msg);
                            // 대답 출력하기
                            foreach (Dialog answer in q.answers)
                                chatting.InputChat(true, dialog.isUser, chatting.FindMember(dialog.how), answer.msg);
                        }));
                }
            }
        }
    }
}
