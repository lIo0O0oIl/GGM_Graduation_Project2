using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RoundFile
{
    public string round;        // == Chapter
    public GameObject[] Files;
}

public class InvisibleFileManager : MonoBehaviour
{
    public static InvisibleFileManager Instance;

    public RoundFile[] roundFile;        // 라운드 마다 보여질 것
    HashSet<string> ShowRoundSet = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
        ShowRoundSet.Clear();
    }

    public void Start()
    {
        DontShowRound();
    }

    public void  ShowRoundFile(string round)
    {
        foreach (var file in roundFile)
        {
            if (file.round == round)
            {
                foreach(var rountFile in file.Files)
                {
                    rountFile.SetActive(true);
                    FileManager.instance.NowFilePathIncludeFileCheck(rountFile);     // 키려는 것이 지금 경로에 있어야만함.
                }
            }
        }
        ShowRoundSet.Add(round);
    }

    public void DontShowRound()
    {
        foreach (var file in roundFile)
        {
            if (ShowRoundSet.Contains(file.round) == false)      // 포함되어있지 않으면 꺼주고 현재 파일 경로에 없으면 지워줘야해.
            {
                foreach (var obj in file.Files)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
}
