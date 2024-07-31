using DG.Tweening;
using System.Collections.Generic;

using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    public int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutScenes = new List<CutSceneSO>();

    private void Awake()
    {
        Instance = this;
    }

    public CutSceneSO FindCutScene(string name)
    {
        foreach (CutSceneSO cutScene in cutScenes)
        {
            if (cutScene.name == name)
                return cutScene;
        }
        return null;
    }

    public void CutScene(string name)
    {
        currentCutScene = FindCutScene(name);
        currentCutNum = 0;
        currentTextNum = 0;

        GameManager.Instance.cutSceneSystem.ChangeCut(false, currentCutScene.cutScenes[0].cut);
        Next();
    }

    public void Next()
    {
        if (currentCutScene != null)
        {
            if (currentCutScene.cutScenes[currentCutNum].texts.Count == currentTextNum)
            {
                currentTextNum = 0;
                currentCutNum++;

                if (currentCutScene.cutScenes.Count <= currentCutNum)
                {

                    GameManager.Instance.fileManager.UnlockChat(currentCutScene.name);
                    UIReader_Main.Instance.OpenCutScene();

                    if (currentCutScene.nextMemberName != "")
                        GameManager.Instance.chatSystem.ChoiceMember
                            (GameManager.Instance.chatSystem.FindMember(currentCutScene.nextMemberName));
                    GameManager.Instance.chatHumanManager.chapterMember = GameManager.Instance.chatHumanManager.currentMember;

                    return;
                }
                else
                {
                    GameManager.Instance.cutSceneSystem.ChangeCut(currentCutScene.cutScenes[currentCutNum].isAnim,
                        currentCutScene.cutScenes[currentCutNum].cut);
                }
            }
            if (currentCutScene.cutScenes[currentCutNum].texts.Count > currentTextNum)
            {
                if (UIReader_Main.Instance.currentTextTween != null)
                {
                    if (UIReader_Main.Instance.currentTextTween.IsPlaying())
                        UIReader_Main.Instance.EndText();
                    else
                    {
                        CutSceneText msg = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                        if (msg.sound != "")
                            GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; }, msg.sound, msg.vibration);
                        else
                            GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; }, "", msg.vibration);
                    }
                }
                else
                {
                    CutSceneText msg = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                    if (msg.sound != "")
                        GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; }, msg.sound, msg.vibration);
                    else
                        GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; }, "", msg.vibration);
                }
            }
        }
        else
            Debug.Log("current Cut Scene is null");
    }
}
