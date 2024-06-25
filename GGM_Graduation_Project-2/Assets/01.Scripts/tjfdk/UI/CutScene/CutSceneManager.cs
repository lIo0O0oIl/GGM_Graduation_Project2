using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    public int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutScenes = new List<CutSceneSO>();
    //public Dictionary<string,  CutSceneSO> cutSceneList;

    private void Awake()
    {
        Instance = this;

        //cutSceneList = new Dictionary<string, CutSceneSO>();
    }

    private void OnEnable()
    {
        //foreach (CutSceneSO cutScene in cutScenes)
        //    cutSceneList.Add(cutScene.chapterName, cutScene);
    }

    public CutSceneSO FindCutScene(string name)
    {
        foreach (CutSceneSO cutScene in cutScenes)
        {
            if (cutScene.name == name)
                return cutScene;
        }
        return null;
        //return cutSceneList[name];
    }

    public void CutScene(bool isOpen, string name)
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
                            GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; }, msg.sound);
                        else
                            GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; });
                    }
                }
                else
                {
                    CutSceneText msg = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                    if (msg.sound != "")
                        GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; }, msg.sound);
                    else
                        GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / 2, () => { currentTextNum++; });
                }
            }
        }
        else
            Debug.Log("current Cut Scene is null");
    }

    //public void Next()
    //{
    //    if (currentCutScene != null)
    //    {
    //        if (currentCutScene.cutScenes.Count == currentCutNum)
    //        {
    //            GameManager.Instance.fileManager.UnlockChat(currentCutScene.name);
    //            if (currentCutScene.nextMemberName != "")
    //                GameManager.Instance.chatSystem.ChoiceMember
    //                    (GameManager.Instance.chatSystem.FindMember(currentCutScene.nextMemberName));
    //            cutsceneUI.OpenCutScene(false);
    //        }
    //        else
    //        {
    //            // next text
    //            if (currentCutScene.cutScenes[currentCutNum].texts.Count > currentTextNum)
    //            {
    //                //if (cutsceneUI.currentTextTween != null)
    //                //{
    //                //    if (cutsceneUI.currentTextTween.IsPlaying())
    //                //    {
    //                //        cutsceneUI.EndText();
    //                //    }
    //                //    else
    //                //    {
    //                //        cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 3f);
    //                //        if (currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].sound != "")
    //                //        {
    //                //            string sound = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].sound;
    //                //            cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 3f, sound);
    //                //        }
    //                //        else
    //                //            cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 3f, null);
    //                //        currentTextNum++;
    //                //    }
    //                //}
    //                //else  
    //                //{
    //                    //cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 3f);
    //                    //if (currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].sound != "")
    //                    //{
    //                    //    string sound = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].sound;
    //                    //    cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 3f, sound);
    //                    //}
    //                    //else
    //                    cutsceneUI.ChangeText(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text, 3f, null);
    //                    currentTextNum++;
    //                //}
    //            }
    //            // next cut
    //            if (currentCutScene.cutScenes.Count > currentCutNum && currentCutScene.cutScenes[currentCutNum].texts.Count == currentTextNum)
    //            {
    //                cutsceneUI.ChangeCut(currentCutScene.cutScenes[currentCutNum].isAnim, currentCutScene.cutScenes[currentCutNum].cut);
    //                currentCutNum++;
    //                currentTextNum = 0;
    //            }
    //        }
    //    }
    //}
}
