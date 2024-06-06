using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CutSceneManager : MonoBehaviour
{
    UIReader_CutScene cutsceneUI;
    public static CutSceneManager Instance;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    [SerializeField] private int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutSceneChapters = new List<CutSceneSO>();

    private void Awake()
    {
        Instance = this;
        cutsceneUI = GetComponent<UIReader_CutScene>();
    }

    public CutSceneSO FindCutScene(string name)
    {
        foreach (CutSceneSO cutScene in cutSceneChapters)
        {
            if (cutScene.name == name)
                return cutScene;
        }

        return null;
    }

    public void CutScene(bool isOpen, string name)
    {
        currentCutScene = FindCutScene(name);
        currentCutNum = 0;
        currentTextNum = 0;

        Next();
    }

    public void Next()
    {
        if (currentCutScene != null)
        {
            if (currentCutScene.cutScenes.Count == currentCutNum)
                cutsceneUI.OpenCutScene(false);
            else
            {
                if (currentCutScene.cutScenes[currentCutNum].texts.Count > currentTextNum)
                {
                    cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 1.5f);
                    currentTextNum++;
                }
                if (currentCutScene.cutScenes.Count > currentCutNum && currentCutScene.cutScenes[currentCutNum].texts.Count == currentTextNum)
                {
                    cutsceneUI.ChangeCut(currentCutScene.cutScenes[currentCutNum].cut);
                    currentCutNum++;
                    currentTextNum = 0;
                }
            }
        }
    }
    
    //// �� �� ȣ�� �Լ�
    //public void CutScene(bool isOpen, string _chapterName = "")
    //{
    //    // �� ���� Ű�� ������ ���� ������
    //    //cutScene.SetActive(isOpen);
    //    cutsceneUI.cutScenePanel.style.display = DisplayStyle.Flex;
    //    cutsceneUI.mainPanel.style.display = DisplayStyle.None;

    //    // �� ���� �̸� �Լ� ȣ��
    //    if (isOpen && _chapterName != "")
    //        PlayChapter(_chapterName);

    //    // �� �� ������
    //    if (isOpen == false)
    //    {
    //        Debug.Log("�� �� ����");
    //        cutsceneUI.cutScenePanel.style.display = DisplayStyle.Flex;
    //        cutsceneUI.mainPanel.style.display = DisplayStyle.None;
    //        ChattingManager.Instance.StartChatting(0);           // ���� ó���� 0���� �صα�
    //    }
    //}

    //// �� �� ���� �Լ�
    //private void PlayChapter(string _chapterName)
    //{
    //    currentCutScene = null;

    //    // ���� é�� ã���ֱ�
    //    foreach (CutSceneSO chapter in cutSceneChapters)
    //    {
    //        if (chapter.chapterName == _chapterName)
    //            currentCutScene = chapter;
    //    }

    //    // ���� é�� SO �ʱ�ȭ
    //    foreach (CutSceneDialoges chapter in currentCutScene.cutScenes)
    //    {
    //        foreach (CutSceneText text in chapter.texts)
    //            text.isEnd = false;
    //    }

    //    // é�� ������ �ε��� �ʱ�ȭ
    //    currentCutNum = 0;
    //    currentTextNum = 0;

    //    // �� �� ����
    //    CutSetting();
    //}

    //// �׸� ����
    //private void CutSetting()
    //{
    //    // �̹��� ����
    //    cutsceneUI.ChangeCut(currentCutScene.cutScenes[currentCutNum].cut);
    //    // ��� �Է� �Լ� ȣ��
    //    Texting(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum]);
    //}

    //// �������� �ѱ�� �� �� (��ư�� �����ص�)
    //public void NextCut()
    //{
    //    if (currentCutScene != null)
    //    {
    //        // 이게 마지막이면...
    //        if (currentCutScene.cutScenes[currentCutNum].texts.Count <= currentTextNum)
    //            CutScene(false);
    //        else
    //        {
    //            CutSceneText currentText = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];

    //            // ���� �������� �ƾ��� ��簡 �� �ۼ����� �ʾҴٸ�
    //            if (currentText.isEnd == false)
    //            {
    //                // ��Ʈ�� ���� ����
    //                cutsceneUI.EndText();
    //                // ��� �Է� �Ϸ�
    //                currentText.isEnd = true;
    //            }
    //            // ���� �������� �ƾ��� ��簡 �� �ۼ� �ƴٸ�
    //            else
    //            {
    //                // ��� �ε��� ����
    //                currentTextNum++;

    //                //Debug.Log(currentTextNum);
    //                //Debug.Log(currentCutScene.cutScenes[currentCutNum].texts.Count);

    //                // ���� ���� ��� ��縦 �����ߴٸ�
    //                if (currentTextNum >= currentCutScene.cutScenes[currentCutNum].texts.Count)
    //                {
    //                    currentCutNum++;
    //                    if (currentCutNum >= currentCutScene.cutScenes.Count)
    //                    {
    //                        CutScene(false);
    //                    }
    //                    else
    //                    {
    //                        currentTextNum = 0;
    //                        CutSetting();
    //                    }
    //                }
    //                // ���� ���� ��簡 �����ִٸ�
    //                else
    //                {
    //                    // ��� ����
    //                    currentText = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
    //                    // ��� �Է� �Լ� ȣ��
    //                    Texting(currentText);
    //                }
    //            }
    //        }
    //    }
    //}
    
    //// ��� �Է� �Լ� (��Ʈ��)
    //private void Texting(CutSceneText temp)
    //{
    //    //// ���� ���
    //    //if (temp.sound != "")
    //    //    SoundManager.Instance.PlaySFX(temp.sound);
    //    //else
    //    //{
    //    //    SoundManager.Instance.PlaySFX("typing");
    //    //}

    //    // text �Է�
    //    float textDuring = temp.text.Length * 0.5f;
    //    cutsceneUI.ChangeText(temp.text, textDuring);

    //    // toolkit reader�� ����
    //    ////��Ʈ������ �ؽ�Ʈ �ۼ�
    //    //text.DOText(temp.text, 1.5f).OnComplete(() =>
    //    //{
    //    //    temp.isEnd = true;
    //    //    SoundManager.Instance.StopSFX();
    //    //});
    //}
}
