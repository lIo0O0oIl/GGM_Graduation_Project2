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

    public RoundFile[] roundFile;        // ?쇱슫??留덈떎 蹂댁뿬吏?寃?
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
        bool is_Exsist = false;
        foreach (var file in roundFile)
        {
            if (file.round == round)
            {
                foreach(var rountFile in file.Files)
                {
                    rountFile.SetActive(true);
                    //FileManager.instance.NowFilePathIncludeFileCheck(rountFile);     // ?ㅻ젮??寃껋씠 吏湲?寃쎈줈???덉뼱?쇰쭔??
                }
                is_Exsist = true;
            }
        }
        if (is_Exsist == false) Debug.LogError($"{round} ???) ?녿뒗 ?쇱슫???낅땲??");
        ShowRoundSet.Add(round);
    }

    public void DontShowRound()
    {
        foreach (var file in roundFile)
        {
            if (ShowRoundSet.Contains(file.round) == false)      // ?ы븿?섏뼱?덉? ?딆쑝硫?爰쇱＜怨??꾩옱 ?뚯씪 寃쎈줈???놁쑝硫?吏?뚯쨾?쇳빐.
            {
                foreach (var obj in file.Files)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
}
