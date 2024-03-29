using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private Image screen;
    [SerializeField] private Text text;
    //[SerializeField] private TextMeshProUGUI text;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    [SerializeField] private int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutSceneChapters = new List<CutSceneSO>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayChapter("test");
        }
    }

    public void PlayChapter(string _chapterName)
    {
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

        CutSetting();
    }

    // 그림 세팅
    private void CutSetting()
    {
        // 이미지 설정
        //
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
                // 현재 컷의 모든 대사를 실행했다면
                if (currentTextNum >= currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text.Length - 1)
                {
                    // 테스트용 디버그
                    Debug.Log("끝!!!");
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

    private void Texting(CutSceneText temp)
    {
        // 이전 텍스트 삭제
        text.text = "";
        //다트윈으로 텍스트 작성
        text.DOText(temp.text, 3f).OnComplete(() =>
        {
            temp.isEnd = true;
        });
    }
}
