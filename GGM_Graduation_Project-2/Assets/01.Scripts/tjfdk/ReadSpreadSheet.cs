using ChatVisual;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static Unity.Collections.Unicode;

public class ExcelReader : MonoBehaviour
{
    private string[] address = new string[4];
    private long[] id = new long[4];

    private int currentAddress, currentId;

    private string startLocation, currentLocation;
    private int n;

    //private string locations = "B4:B17";

    private int nowReadLine = 0;   
    private int nowAskIndex = 0;

    private ChatNode[] chat;
    //private AskAndReply[] askAndReplySO; 
    //private Round[] round; //?

    private void Awake()
    {
        startLocation = "B3:H" + n;

        address[0] = "https://docs.google.com/spreadsheets/d/1icmJfOrAPdp1J68Mj6PTdHW3aiZuRM34XlbJlywKbtU";
        //address[1] = "https://docs.google.com/spreadsheets/d/1icmJfOrAPdp1J68Mj6PTdHW3aiZuRM34XlbJlywKbtU";
        //address[2] = "https://docs.google.com/spreadsheets/d/1icmJfOrAPdp1J68Mj6PTdHW3aiZuRM34XlbJlywKbtU";
        //address[3] = "https://docs.google.com/spreadsheets/d/1icmJfOrAPdp1J68Mj6PTdHW3aiZuRM34XlbJlywKbtU";

        id[0] = 0;
    }

    private void Start()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        // currentLocation = 특정 인물
        using (UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(currentLocation, currentId)))
        {
            yield return www.SendWebRequest();

            // 특정 인물의 대사
            string[] lineCut = www.downloadHandler.text.Split("\n");

            // 시벌 이게 뭐여
            //ChattingManager.Instance.Chapters[i].who = lineCut[0].Split('\t')[0];

            // type 먼저 찾고?
            // question 인지 아닌지로 구분
            // no -> who, text 읽어서 설정하고?
            // yes -> who, text 뒤에 type 한 번 더 읽고 세분화 (switch)
            // 전체적으로 face, event, fileload 읽고 fileload 있다면 name 읽고...
            // 셋 다 비어있으면 걍 건너 띠!

            string[] chatSize = lineCut[7].Split('\t'); // n칸
            //if (int.TryParse(chatSize[0], out int ))

            // 아래부터 양식에 맞게 읽는 거
            //////// ask?
            ////////if (int.TryParse(chatSize[0], out int askCount)) //?
            ////////{
            ////////    askAndReplySO = new AskAndReply[askCount];
            ////////}
            ////////else { Debug.LogError("not"); }

            //////// chat?
            //////if (int.TryParse(chatSize[1], out int chatCount))
            //////{
            //////    chat = new ChatNode[chatCount + askCount];
            //////}
            //////else { Debug.LogError("not"); }

            //////// 
            //////for (int j = 4; j < chat.Length + 4; j++)
            //////{
            //////    string[] chatAndState = lineCut[j + nowReadLine].Split('\t');
            //////    chat[j - 4].chatText = chatAndState[1];

            //////    EChatState state = (EChatState)Enum.Parse(typeof(EChatState), chatAndState[0]);
            //////    if (state == EChatState.Ask)
            //////    {
            //////        chat[j - 4].state = EChatState.Ask;
            //////        askAndReplySO[nowAskIndex].ask = chatAndState[1];
            //////        if (int.TryParse(lineCut[j + nowReadLine + 1].Split('\t')[1], out int replyCount))
            //////        {
            //////            nowReadLine += 2;
            //////            askAndReplySO[nowAskIndex].reply = new string[replyCount];
            //////            for (int k = 0; k < replyCount; k++)
            //////            {
            //////                askAndReplySO[nowAskIndex].reply[k] = lineCut[j + nowReadLine].Split('\t')[1];
            //////                nowReadLine++;
            //////            }
            //////            nowReadLine--;
            //////            nowAskIndex++;
            //////        }
            //////    }
            //////    else
            //////    {
            //////        chat[j - 4].state = state;
            //////    }
            //////}

            //////// 
            //////int roundStartLine = nowReadLine + chat.Length + 5;
            //////if (lineCut.Length - roundStartLine > 0)
            //////{
            //////    round = new Round[lineCut.Length - roundStartLine];
            //////    int roundCount = 0;
            //////    for (int j = roundStartLine; j < lineCut.Length; j++)
            //////    {
            //////        round[roundCount].round = lineCut[j].Split('\t')[0];
            //////        round[roundCount].text = lineCut[j].Split('\t')[1];
            //////        roundCount++;
            //////    }
            //////}
            //////else round = new Round[0];

            //ChattingManager.Instance.Chapters[i].chat = chat;
            //ChattingManager.Instance.Chapters[i].askAndReply = askAndReplySO;
            //ChattingManager.Instance.Chapters[i].round = round;

            // �ʱ�ȭ
            nowAskIndex = 0;
            nowReadLine = 0;
        }

        LoadingManager.Instance.LoadingEnd(); //?
        //CutSceneManager.Instance.CutScene("Start");
    }

    public string GetTSVAddress(string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}