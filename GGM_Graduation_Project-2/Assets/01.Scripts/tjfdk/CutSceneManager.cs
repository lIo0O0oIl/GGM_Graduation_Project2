using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Reflection;
using Unity.VisualScripting;

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

        currentCutNum = 0;
        currentTextNum = 0;

        imageSEt();
        //currentCutScene = cutScenes[currentChapterNum];
        //textNum = 0;
        //textMaxNum = currentCutScene.cutScenes[currentCutSceneNum].t
    }

    // 그림 세팅
    private void imageSEt()
    {
        //screen.sprite = currentCutScene.cutScenes[currentCutNum].cut.sprite;
        //screen.sprite = currentCutScene.cutScenes[currentCutNum].cut.GetComponent<SpriteRenderer>().sprite;
        TTT(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum]);
    }

    // 다음으로 넘기려 할 때
    public void NextCut()
    {
        if (currentCutScene != null)
        {

            // 지금 진행중인 컷씬의 대사가 다 작성되지 않았다면
            test2 temp = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
            if (temp.isEnd == false)
            {
                // 자동 작성
                text.DOKill();
                text.text = temp.text;
                temp.isEnd = true;
            }
            else
            {
                currentTextNum++;
                if (currentTextNum >= currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text.Length)
                {
                    Debug.Log("끝!!!");
                    return;
                }
                temp = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                TTT(temp);
            }
        }
    }

    private void TTT(test2 temp)
    {
        text.text = "";
        //다트윈으로 텍스트 작성하기...
        text.DOText(temp.text, 0.15f * temp.text.Length).OnComplete(() =>
        {
            temp.isEnd = true;
        });
    }

    //public void ChangeCut()
    //{
    //    screen.sprite = currentCutScene.cutScenes[currentCutSceneNum].cut.sprite;
    //    text.text = currentCutScene.cutScenes[currentCutSceneNum].texts.;
    //}


    //private void StartCutScene()
    //{
    //    // 현재 챕터의 컷씬 목록에 접근하여 모든 컷씬을 재생한다
    //    foreach (CutSceneSO chapter in cutScenes[currentCutSceneNum].cutScenes)
    //    {

    //    }
    //}

    //private void EndCutScene()
    //{

    //}
}
