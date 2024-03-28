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
    
    private string nowPath;     // 지금 경로
    private TMP_Text[] upLinePathText;       // 윗줄에 경로 표시용 텍스트

    [Header("UpLine")]
    public GameObject[] upLinePathBtn;     // 윗줄에 경로 표시용
    public RectTransform UpLineSizeFitter;      // 윗줄 사이즈 정렬용
    private RectTransform[] upLineRectFitter;       // 윗줄에 버튼들 정렬용

    [Header("Image")]
    public GameObject imagePanel;       // 이미지 관련
    public TMP_Text imageName;      // 보이질 이미지의 이름
    public Image showImage;        // 보여질 이미지
    private RectTransform imageSize;        // 보여질 이미지의 사이즈

    [Header("TextNote")]
    public GameObject textNotePanel;    // 메모장 관련
    public TMP_Text textName;      // 보이질 텍스트의 이름
    public TMP_Text showText;       // 보여질 텍스트

    [Header("Lock")]
    public GameObject lockPanel;        // 잠금 관련
    private LockSystem lockSystem;      // 잠금 관련 시스템

    [Header("PuzzleLock")]
    public GameObject puzzlePanel;       // 퍼즐관련 관련

    [Space(25)]

    public FileTree[] fileTrees;        // 파일 전체 구조

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

        imageSize = showImage.gameObject.GetComponent<RectTransform>();
        lockSystem = lockPanel.GetComponent<LockSystem>();

        upLineRectFitter = new RectTransform[3];
        for (int i = 0; i < 3; i++)
        {
            upLineRectFitter[i] = upLinePathBtn[i].GetComponent<RectTransform>();
        }
    }

    #region 폴더 이동 관련 함수
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
                upLinePathText[i].text = " " + path[i + 1] + " >";
            }
            for (int i = path.Length - 1; i < 3; ++i)
            {
                upLinePathBtn[i].SetActive(false);      // 경로가 없는 것이면 지워주기
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(UpLineSizeFitter);
            LayoutRebuilder.ForceRebuildLayoutImmediate(upLineRectFitter[0]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(upLineRectFitter[1]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(upLineRectFitter[2]);
        }
        else
        {
            for (int i = 0; i < 3; ++i)
            {
                upLinePathBtn[i].SetActive(false);      // 경로가 지금 아무것도 없으면 다 지워주기
            }
        }

        InvisibleFileManager.Instance.DontShowRound();      // 잚못 켜준거 있으면 꺼주기
    }

    public void NowFilePathIncludeFileCheck(GameObject file)
    {
        Debug.Log(nowPath);
        bool is_Include = false;
        foreach (var fileTree in fileTrees)
        {
            if (fileTree.path == nowPath)
            {
                foreach (var includeFile in fileTree.Files)
                {
                    if (file == includeFile)
                    {
                        is_Include = true;
                    }
                }
            }
        }
        if (is_Include == false) file.SetActive(false);
    }

    public void GoMain()        // 윗줄에서 메인을 눌렀을 때
    {
        GoFile(nowPath, "메인");
    }

    public void UpLinePath(int num)
    {
        string[] names = nowPath.Split("\\");       // 1. nowPath를 \ 를 기준으로 나눠준다.
        string buttonName = names[num];         //  2. 누른 버튼에 적힌 이름을 가져와준다.
        int index = nowPath.IndexOf(buttonName) + buttonName.Length;            // 2. 누른 버튼의 인덱스 + 사이즈만큼을 nowPath에서 추출한다.
        string goPath = nowPath.Substring(0, index);        // 문자열을 만들어준다.
        GoFile(nowPath, goPath);
    }
    #endregion

    private string RemoveSpace(string name)     // 줄바꿈, .(확장자) 전에 공백이 있는 경우에 지워주기
    {
        name = name.Replace("\n", "");

        int dotIndex = name.IndexOf('.');       // 점있는 인덱스 찾기
        if (dotIndex >= 0 && name[dotIndex - 1] == ' ')     // 뒤에가 공백인 경우에만
        {
            int lastSpaceIndex = name.LastIndexOf(" ");
            if (lastSpaceIndex >= 0 )
            {
                // '.' 이전의 공백 제거
                name = name.Remove(lastSpaceIndex, dotIndex - lastSpaceIndex);
            }
        }
        Debug.Log(name);
        return name;
    }

    #region 이미지 폴더 열기 관련 함수
    public void OpenImageFile(Sprite image, Vector2 scale, string name)
    {
        Debug.Log("이미지 열기");

        showImage.sprite = image;
        imageSize.sizeDelta = scale;
        imageName.text = RemoveSpace(name);
        imagePanel.SetActive(true);
    }

    public void ImageBackClick()
    {
        imagePanel.SetActive(false);
    }
    #endregion

    #region 텍스트 폴더 열기 관련 함수
    public void OpenTextFile(string text, string name)
    {
        Debug.Log("텍스트 열기");

        textNotePanel.SetActive(true);
        showText.text = text;
        textName.text = RemoveSpace(name);
    }

    public void TextBackClick()
    {
        textNotePanel.SetActive(false);
    }
    #endregion

    #region 잠김 파일 열기 관련 함수
    public void OpenLock(string fileName, string password, Image lockImage)
    {
        Debug.Log("잠금 열기");

        lockPanel.SetActive(true);
        lockSystem.Init(fileName, password, lockImage);
    }

    public void LockBackClick()
    {
        lockPanel.SetActive(false);
    }

    public void OpenPuzzleLock(int puzzleNum)
    {
        puzzlePanel.transform.GetChild(puzzleNum).gameObject.SetActive(true);
        puzzlePanel.SetActive(true);
    }

    public void PuzzleLockBackClick()
    {
        for (int i = 1; i < puzzlePanel.transform.childCount; i++)
        {
            puzzlePanel.transform.GetChild(i).gameObject.SetActive(false);
        }
        Debug.Log(puzzlePanel.name);
        puzzlePanel.SetActive(false);
    }
    #endregion
}

