using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 파일은 셋액티브를 껐다가 키는 식으로 이동을 구현한다.
  
경로는 메인/학교/어쩌고 등으로 함.
 */

[Serializable]
public struct FileTree
{
    public string path;
    public GameObject[] Files;
}

public class FileManager : MonoBehaviour
{
    public static FileManager instance;     // 어디서나 호출할 수 있도록

    public BackBtn backBtn;         // 뒤로가기
    public GameObject[] upLinePathBtn;     // 윗줄에 경로 표시용
    public FileTree[] fileTrees;        // 파일 전체 구조

    private string nowPath;     // 지금 경로
    private TMP_Text[] upLinePathText;       // 윗줄에 경로 표시 텍스트


    private void Awake()
    {
        instance = this;
        nowPath = "메인";

        upLinePathText = new TMP_Text[upLinePathBtn.Length];
        for (int i = 0; i < upLinePathBtn.Length; i++)
        {
            upLinePathText[i] = upLinePathBtn[i].GetComponentInChildren<TMP_Text>();
            upLinePathBtn[i].SetActive(false);
        }
    }
    /*
     GoFile 함수
    1. 갈 경로와 현재 경로가 같이 들어옴
    2. 현재 경로 파일은 모두 액티브를 꺼주고
    3. 갈 경로에 있는 파일들은 모두 티브를 켜준다.
     */
    public void GoFile(string nowPath, string goPath)
    {
        Debug.Log($"현재 경로 : {nowPath}, 가려는 경로 : {goPath}");

        foreach (var fileTree in fileTrees)
        {
            if (fileTree.path == nowPath)
            {
                foreach (var file in fileTree.Files)
                {
                    file.SetActive(false);
                }
            }

            if (fileTree.path == goPath)
            {
                foreach (var file in fileTree.Files)
                {
                    file.SetActive(true);
                }
            }
        }

        backBtn.nowPath = goPath;       // 뒤로가기 연결용
        this.nowPath = goPath;      // 지금 경로 표시

        // 윗 경로 표시용
        if (this.nowPath.LastIndexOf('\\') != -1)
        {
            string[] path = this.nowPath.Split('\\');
            for (int i = 0;i < path.Length - 1; ++i)
            {
                upLinePathBtn[i].SetActive(true);
                upLinePathText[i].text = path[i + 1];
            }
            for (int i = path.Length; i < 3; ++i)
            {
                upLinePathBtn[i].SetActive(false);      // 경로가 없는 것이면 지워주기
            }
        }
    }

    public void GoMain()
    {
        GoFile(nowPath, "메인");
    }
}

