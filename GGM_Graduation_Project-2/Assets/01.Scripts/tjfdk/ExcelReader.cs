using ChatVisual;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA;

//[Serializable]
//class SpreadSheet
//{
//    public SpreadSheet(string add, string ran, long id)
//    {
//        address = add;
//        range = ran;
//        sheetID = id;
//    }

//    public string address;
//    public string range;
//    public long sheetID;
//}

public class ExcelReader : MonoBehaviour
{
    ////private SpreadSheet[] address = new SpreadSheet[4];
    //private long[] sheetIDs = new long[4];
    //private string sheetName;

    //private MemberProfile member;
    ////public List<NormalCHat> excelChat = new List<NormalCHat>();

    //private void Start()
    //{
    //    //address[0] = new SpreadSheet
    //    //    ("https://docs.google.com/spreadsheets/d/1icmJfOrAPdp1J68Mj6PTdHW3aiZuRM34XlbJlywKbtU", "A1", 0);

    //    sheetIDs[0] = 0;
    //    sheetIDs[1] = 862719105;

    //    StartCoroutine(LoadData());
    //}

    //// 기존 URL을 tsv 형태로 바꿔주는 함수
    //public string GetTSVAddress(/*string address, string range, */long sheetID)
    //{
    //    return $"https://docs.google.com/spreadsheets/d/1icmJfOrAPdp1J68Mj6PTdHW3aiZuRM34XlbJlywKbtU/export?format=tsv&range=A1:J21&gid={sheetID}";
    //}

    //private void GetSheetName(string tsvData)
    //{
    //    // TSV 데이터를 줄 단위로 분리
    //    string[] lines = tsvData.Split('\n');

    //    if (lines.Length > 0)
    //    {
    //        // 첫 번째 줄에서 A1 셀의 값을 시트 이름으로 추출
    //        string[] firstLine = lines[0].Split('\t');
    //        if (firstLine.Length > 0)
    //        {
    //            sheetName = firstLine[0]; // 첫 번째 셀의 값을 시트 이름으로 사용
    //            Debug.Log("Sheet Name: " + sheetName);
    //        }
    //    }
    //}

    //private IEnumerator LoadData()
    //{
    //    // 현재 시트의 URL 가져오기
    //    string url = GetTSVAddress(sheetIDs[0]);

    //    using (UnityWebRequest www = UnityWebRequest.Get(url))
    //    {
    //        yield return www.SendWebRequest();

    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            // tsv 형태의 데이터 가져오기
    //            string tsvData = www.downloadHandler.text;
    //            // 시트 인물 이름 가져오기
    //            GetSheetName(tsvData);
    //            // 시트 대사 가져오기
    //            ProcessData(tsvData);
    //        }
    //        else
    //            Debug.LogError("Error: " + www.error);
    //    }
    //}

    //private void ProcessData(string tsvData)
    //{
    //    // 현재 시트의 인물에 추가
    //    member = GameManager.Instance.chatSystem.FindMember(sheetName);

    //    // TSV 데이터를 줄 단위로 분리
    //    string[] lines = tsvData.Split('\n');


    //    // 읽기 시작
    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        // 각 줄을 탭으로 구분하여 필드로 분리
    //        string[] row = lines[i].Split('\t');

    //        // 행에 데이터가 있는 경우 처리
    //        if (row.Length > 0)
    //        {
    //            // 첫 번째 필드는 타입 (Chat, Answer, Question 등)
    //            string type = row[0];
    //            NormalChat obj = null;

    //            // 타입에 따라 적절한 객체를 생성
    //            if (type == "Chat" || type == "Answer")
    //            {
    //                obj = new Chat
    //                {
    //                    text = row[1],
    //                    who = (EChatState)Enum.Parse(typeof(EChatState), row[2]),
    //                    type = (EChatType)Enum.Parse(typeof(EChatType), row[3]),
    //                    face = (EFace)Enum.Parse(typeof(EFace), row[4]),
    //                    evt = (EChatEvent)Enum.Parse(typeof(EChatEvent), row[5]),
    //                    fildLoad = !string.IsNullOrEmpty(row[8]?.Trim()) ? row[8].Split('/') : Array.Empty<string>(),
    //                    fileName = !string.IsNullOrEmpty(row[9]?.Trim()) ? row[9].Trim() : null,
    //                };
    //            }
    //            else if (type == "Ask")
    //            {
    //                obj = new Question
    //                {
    //                    text = row[1],
    //                    type = (EAskType)Enum.Parse(typeof(EAskType), row[6]),
    //                    nextName = row[7],
    //                    fildLoad = !string.IsNullOrEmpty(row[8]?.Trim()) ? row[8].Split('/') : Array.Empty<string>(), 
    //                    fileName = !string.IsNullOrEmpty(row[9]?.Trim()) ? row[9].Trim() : null,
    //                    answers = new List<Chat>() // 초기화
    //                };
    //            }

    //            // 객체가 정상적으로 생성된 경우 리스트에 추가
    //            if (obj != null)
    //            {
    //                //Debug.Log(obj.GetType() + " : " + obj.text);
    //                member.excelChat.Add(obj);

    //                // 객체가 Question일 경우, 다음에 오는 Answer들 찾기
    //                if (obj is Question question)
    //                {
    //                    // 현재 인덱스에서 다음 Answer들 찾기
    //                    int currentIndex = i;
    //                    while (currentIndex + 1 < lines.Length && lines[++currentIndex].Split('\t')[0] == "Answer")
    //                    {
    //                        // 다음 줄을 읽어서 Answer 객체 생성
    //                        var answerRow = lines[currentIndex].Split('\t');
    //                        var answer = new Chat
    //                        {
    //                            text = answerRow[1],
    //                            who = (EChatState)Enum.Parse(typeof(EChatState), answerRow[2]),
    //                            type = (EChatType)Enum.Parse(typeof(EChatType), answerRow[3]),
    //                            face = (EFace)Enum.Parse(typeof(EFace), answerRow[4]),
    //                            evt = (EChatEvent)Enum.Parse(typeof(EChatEvent), answerRow[5]),
    //                            fildLoad = !string.IsNullOrEmpty(row[8]?.Trim()) ? row[8].Split('/') : Array.Empty<string>(),
    //                            fileName = !string.IsNullOrEmpty(answerRow[9]?.Trim()) ? answerRow[9].Trim() : null,
    //                        };
    //                        // Question 객체의 answers 리스트에 추가
    //                        question.answers.Add(answer);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}