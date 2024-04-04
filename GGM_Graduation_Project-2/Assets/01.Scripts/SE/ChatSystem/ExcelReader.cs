using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelReader : MonoBehaviour
{
    private const string address = "https://docs.google.com/spreadsheets/d/1MEv_FujCwHkHfq2Dti7aphwSL4xOQ2Zz";
    public string locations = "B3";     // 채팅들의 위치를 모아둔 것.
    public const long id = 385840303;
    private int nowReadLine = 0;        // 내가 지금 읽고 있는 줄
    private int nowAskIndex = 0;        // 내가 지금 가지고있는 질문 인덱스

    public Chapters[] chapters;        // 채팅들 모음.

    private Chat[] chat;            // 채팅들
    private AskAndReply[] askAndReplySO;   // 질문들

    private void Start()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(locations, id));        /// 채팅들이 있는 곳 가져오기
        yield return www.SendWebRequest();

        string[] chatsLocation = www.downloadHandler.text.Split('\n');       // 쳇들의 위치에서 줄 내림 한 것으로 나눠주기
        chapters = new Chapters[chatsLocation.Length];      // 총 대화의 개수

        for (int i = 0; i < chatsLocation.Length; i++)
        {
            UnityWebRequest www2 = UnityWebRequest.Get(GetTSVAddress(chatsLocation[i], id));        // 하나의 쳇팅에서 모든 것을 가져와주기
            yield return www2.SendWebRequest();

            string[] lineCut = www2.downloadHandler.text.Split("\n");       // 줄 내림 한 것으로 나눠주기

            chapters[i].who = lineCut[0].Split('\t')[0];        // 이름 넣어주기

            // 질문의 갯수 설정
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

            // 챕터 만들어주기
            chapters[i].chat = chat;
            chapters[i].askAndReply = askAndReplySO;
            

            // 초기화
            nowAskIndex = 0;
            nowReadLine = 0;
        }
    }

    public string GetTSVAddress(string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}
