using ChatVisual;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    public int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutScenes = new List<CutSceneSO>();

    [Header("Value")]
    [SerializeField] private float textSpeed;
    [SerializeField] private float animSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < cutScenes.Count; ++i)
        {
            cutScenes[i].isRead = false;
        }
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

    public void CutSceneReset(string name)
    {
        currentCutScene = FindCutScene(name);

        GameManager.Instance.cutSceneSystem.ChangeCut(false, 0f, currentCutScene.cutScenes[0].cut);
        GameManager.Instance.cutSceneSystem.ChangeText("", 0f, () => { }, "");
    }

    public void CutScene(string name)
    {
        currentCutScene = FindCutScene(name);

        if (currentCutScene.isRead == false)
        {
            currentCutNum = 0;
            currentTextNum = 0;

            GameManager.Instance.cutSceneSystem.ChangeCut(currentCutScene.cutScenes[currentCutNum].isAnim,
                animSpeed, currentCutScene.cutScenes[currentCutNum].cut);
            Next();

            currentCutScene.isRead = true;
        }
        else
            UIReader_Main.Instance.OpenCutScene();

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
                    if (currentCutScene.nextMemberName != "")
                    {
                        GameManager.Instance.chatSystem.ChoiceMember
                            (GameManager.Instance.chatSystem.FindMember(currentCutScene.nextMemberName), false);

                        GameManager.Instance.fileManager.UnlockChat(currentCutScene.name);

                        UIReader_Main.Instance.OpenCutScene();

                        GameManager.Instance.chatHumanManager.chapterMember
                            = GameManager.Instance.chatHumanManager.currentMember;
                    }
                    else
                        SceneManager.LoadScene("End");

                    return;
                }
                else
                {
                    GameManager.Instance.cutSceneSystem.ChangeCut(currentCutScene.cutScenes[currentCutNum].isAnim,
                        animSpeed, currentCutScene.cutScenes[currentCutNum].cut);
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

                        //if (msg.sound != "")
                            GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length * textSpeed, 
                                () => { currentTextNum++; }, msg.sound);
                        //else
                        //    GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / msg.text.Length * 0.5f, 
                        //        () => { currentTextNum++; }, "", msg.vibration);
                    }
                }
                else
                {
                    CutSceneText msg = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];

                    //if (msg.sound != "")
                        GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length * textSpeed, 
                            () => { currentTextNum++; }, msg.sound);
                    //else
                    //    GameManager.Instance.cutSceneSystem.ChangeText(msg.text, msg.text.Length / msg.text.Length * 0.5f, 
                    //        () => { currentTextNum++; }, "", msg.vibration);
                }
            }
        }
        else
            Debug.Log("current Cut Scene is null");
    }
}
