using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelReader : MonoBehaviour
{
    private const string address = "https://docs.google.com/spreadsheets/d/1ogh_aUgnIo8FNzaShRvGjYOCyq1bT2eX";
    private string locations = "B4:B5";     // 채팅들의 위치를 모아둔 것.
    private const long id = 2137741761; 
    private int nowReadLine = 0;        // 내가 지금 읽고 있는 줄
    private int nowAskIndex = 0;        // 내가 지금 가지고있는 질문 인덱스

    private Chat[] chat;            // 채팅들
    private AskAndReply[] askAndReplySO;   // 질문들
    private Round[] round;

    private void Start()
    {
        locations = "B4:B6";        // 3개만 임의로 불러와줌. 그대로는 너무 길어.
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        // 인스턴스 리소스 해제를 위한 using.
        using (UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(locations, id)))     /// 채팅들이 있는 곳 가져오기
        {
            yield return www.SendWebRequest();

            string[] chatsLocation = www.downloadHandler.text.Split('\n');       // 쳇들의 위치에서 줄 내림 한 것으로 나눠주기
            ChattingManager.Instance.Chapters = new Chapters[chatsLocation.Length];      // 총 대화의 개수

            for (int i = 0; i < chatsLocation.Length; i++)
            {
                using (UnityWebRequest www2 = UnityWebRequest.Get(GetTSVAddress(chatsLocation[i].Trim(), id)))        // 하나의 쳇팅에서 모든 것을 가져와주기
                {
                    yield return www2.SendWebRequest();

                    string[] lineCut = www2.downloadHandler.text.Split("\n");       // 줄 내림 한 것으로 나눠주기

                    ChattingManager.Instance.Chapters[i].who = lineCut[0].Split('\t')[0];        // 이름 넣어주기

                    // 질문과 텍스트의 크기를 가져오기
                    string[] chatSize = lineCut[2].Split('\t');
                    if (int.TryParse(chatSize[0], out int askCount))
                    {
                        askAndReplySO = new AskAndReply[askCount];
                    }
                    else { Debug.LogError("질문 갯수 설정 실패"); }

                    if (int.TryParse(chatSize[1], out int chatCount))
                    {
                        chat = new Chat[chatCount + askCount];
                    }
                    else { Debug.LogError("대화 갯수 설정 실패"); }

                    //int chatStartRow = 

                    for (int j = 4; j < chat.Length + 4; j++)         // 대화 넣어주기
                    {
                        string[] chatAndState = lineCut[j + nowReadLine].Split('\t');
                        chat[j - 4].text = chatAndState[1];       // 택스트 넣어줌.

                        ChatState state = (ChatState)Enum.Parse(typeof(ChatState), chatAndState[0]);
                        if (state == ChatState.Ask)
                        {
                            chat[j - 4].state = ChatState.Ask;            // 질문이라는 타입을 넣어줌.
                            askAndReplySO[nowAskIndex].ask = chatAndState[1];       // 질문쪽에 텍스트를 넣어줌. 나중에 이걸로 찾아서 대답을 해줄거임. 딕셔너리를 쓸까? 스트링과 배열스트링으로?
                            if (int.TryParse(lineCut[j + nowReadLine + 1].Split('\t')[1], out int replyCount))      // 질문에 대한 대답의 갯수
                            {
                                nowReadLine += 2;       // 대답들 부터 시작
                                askAndReplySO[nowAskIndex].reply = new string[replyCount];
                                for (int k = 0; k < replyCount; k++)
                                {
                                    askAndReplySO[nowAskIndex].reply[k] = lineCut[j + nowReadLine].Split('\t')[1];
                                    nowReadLine++;
                                }
                                nowReadLine--;
                                nowAskIndex++;
                            }
                        }
                        else
                        {
                            chat[j - 4].state = state;
                        }
                    }

                    int roundStartLine = nowReadLine + chat.Length + 5;        // Round 시작 지점.
                    if (lineCut.Length - roundStartLine > 0)
                    {
                        round = new Round[lineCut.Length - roundStartLine];
                        int roundCount = 0;
                        for (int j = roundStartLine; j < lineCut.Length; j++)
                        {
                            round[roundCount].round = lineCut[j].Split('\t')[0];
                            round[roundCount].text = lineCut[j].Split('\t')[1];
                            roundCount++;
                        }
                    }
                    else round = new Round[0];

                    // 챕터 만들어주기
                    ChattingManager.Instance.Chapters[i].chat = chat;
                    ChattingManager.Instance.Chapters[i].askAndReply = askAndReplySO;
                    ChattingManager.Instance.Chapters[i].round = round;

                    // 초기화
                    nowAskIndex = 0;
                    nowReadLine = 0;
                }
            }
        }

        // 쳇팅 시스템 켜주기
        ChattingManager.Instance.StartChatting(0);
    }

    public string GetTSVAddress(string range, long sheetID)
    {
        //Debug.Log($"{address}/export?format=tsv&range={range}&gid={sheetID}");
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}
