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

    public void  ShowRoundFile(string round)
    {
        foreach (var file in roundFile)
        {
            if (file.round == round)
            {
                foreach(var rountFile in file.Files)
                {
                    rountFile.SetActive(true);
                }
            }
        }
        ShowRoundSet.Add(round);
    }

    public void DontShowRound()
    {
        foreach (var file in roundFile)
        {
            if (ShowRoundSet.Contains(file.round) == false)      // 포함되어있지 않으면 꺼줘
            {
                foreach (var obj in file.Files)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
}
