using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance;

    [Header("Object")]
    [SerializeField] private GameObject cutScene;
    [SerializeField] private Image screen;
    [SerializeField] private Text text;
    //[SerializeField] private TextMeshProUGUI text;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    [SerializeField] private int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutSceneChapters = new List<CutSceneSO>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //// 테스트를 위해 작성... 당연하게도 셋엑티브 꺼져 있으면 입력 안 먹는다...
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    CutScene(true, "Start");
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    CutScene(true,"End");
        //}

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    CutScene(false);
        //}
    }

    // 컷 씬 호출 함수
    public void CutScene(bool isOpen, string _chapterName = "")
    {
        // 컷 씬을 키는 것인지 끄는 것인지
        cutScene.SetActive(isOpen);

        // 컷 씬의 이름 함수 호출
        if (isOpen && _chapterName != "")
            PlayChapter(_chapterName);

        // 컷 씬 종료점
        if (isOpen == false)
        {
            ChattingManager.Instance.StartChatting(0);           // 가장 처음은 0으로 해두기
        }
    }

    // 컷 씬 세팅 함수
    private void PlayChapter(string _chapterName)
    {
        currentCutScene = null;

        // 현재 챕터 찾아주기
        foreach (CutSceneSO chapter in cutSceneChapters)
        {
            if (chapter.chapterName == _chapterName)
                currentCutScene = chapter;
        }

        // 현재 챕터 SO 초기화
        foreach (CutSceneDialoges chapter in currentCutScene.cutScenes)
        {
            foreach (CutSceneText text in chapter.texts)
                text.isEnd = false;
        }

        // 챕터 진행할 인덱스 초기화
        currentCutNum = 0;
        currentTextNum = 0;

        // 컷 씬 세팅
        CutSetting();
    }

    // 그림 세팅
    private void CutSetting()
    {
        // 이미지 설정
        screen.sprite = currentCutScene.cutScenes[currentCutNum].cut;
        // 대사 입력 함수 호출
        Texting(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum]);
    }

    // 다음으로 넘기려 할 때 (버튼에 연결해둠)
    public void NextCut()
    {
        if (currentCutScene != null)
        {
            CutSceneText currentText = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];

            // 지금 진행중인 컷씬의 대사가 다 작성되지 않았다면
            if (currentText.isEnd == false)
            {
                // 다트윈 강제 종료
                text.DOKill();
                // 대사 입력
                text.text = currentText.text;
                // 대사 입력 완료
                currentText.isEnd = true;
            }
            // 지금 진행중인 컷씬의 대사가 다 작성 됐다면
            else
            {
                // 대사 인덱스 증가
                currentTextNum++;

                Debug.Log(currentTextNum);
                Debug.Log(currentCutScene.cutScenes[currentCutNum].texts.Count);

                // 현재 컷의 모든 대사를 실행했다면
                if (currentTextNum >= currentCutScene.cutScenes[currentCutNum].texts.Count)
                {
                    currentCutNum++;
                    if (currentCutNum >= currentCutScene.cutScenes.Count)
                    {
                        CutScene(false);
                    }
                    else
                    {
                        currentTextNum = 0;
                        CutSetting();
                    }
                }
                // 현재 컷의 대사가 남아있다면
                else
                {
                    // 대사 변경
                    currentText = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                    // 대사 입력 함수 호출
                    Texting(currentText);
                }
            }
        }
    }
    
    // 대사 입력 함수 (다트윈)
    private void Texting(CutSceneText temp)
    {
        // 사운드 출력
        if (temp.sound != "")
            SoundManager.Instance.PlaySFX(temp.sound);
        else
        {
            SoundManager.Instance.PlaySFX("typing");
        }

        // 이전 텍스트 삭제
        text.text = "";
        //다트윈으로 텍스트 작성
        text.DOText(temp.text, 1.5f).OnComplete(() =>
        {
            temp.isEnd = true;
            SoundManager.Instance.StopSFX();
        });
    }
}
