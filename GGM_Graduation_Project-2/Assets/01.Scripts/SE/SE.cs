using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class SE : MonoBehaviour
{
    private const string address = "https://docs.google.com/spreadsheets/d/16kWlT61mXFaIJ4jLhRODBAT_84Am9TgSUYSgmll0W-o";
    private string locations = "B2";     // 채팅들의 위치를 모아둔 것.
    private const long id = 0;

    int count = 3;

    public string GetTSVAddress(string range)
    {
        Debug.Log($"{address}/export?format=tsv&range={range}&gid={id}");
        return $"{address}/export?format=tsv&range={range}&gid={id}";
    }

    private IEnumerator Startt()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(locations)))
        {
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);

            int chatCount = int.Parse(www.downloadHandler.text);

            for (int i = 0; i < chatCount; i++)
            {
                using (UnityWebRequest www2 = UnityWebRequest.Get(GetTSVAddress($"B{count}")))
                {
                    yield return www2.SendWebRequest();
                    Debug.Log(www2.downloadHandler.text);

                    using (UnityWebRequest www3 = UnityWebRequest.Get(GetTSVAddress(www2.downloadHandler.text)))
                    {
                        yield return www3.SendWebRequest();
                        Debug.Log(www3.downloadHandler.text);
                    }
                }
                count++;
            }
        }
    }

}
