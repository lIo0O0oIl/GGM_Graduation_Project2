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
            {
                UI_Reader.Instance.chatSystem.ChoiceMember(UI_Reader.Instance.chatSystem.FindMember(currentCutScene.nextMemberName));
                cutsceneUI.OpenCutScene(false);
            }

            else
            {
                if (currentCutScene.cutScenes[currentCutNum].texts.Count > currentTextNum)
                {
                    cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 1.5f);
                    currentTextNum++;
                }
                if (currentCutScene.cutScenes.Count > currentCutNum && currentCutScene.cutScenes[currentCutNum].texts.Count == currentTextNum)
                {
                    cutsceneUI.ChangeCut(currentCutScene.cutScenes[currentCutNum].isAnim, currentCutScene.cutScenes[currentCutNum].cut);
                    currentCutNum++;
                    currentTextNum = 0;
                }
            }
        }
    }
}
