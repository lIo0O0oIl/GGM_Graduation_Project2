using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using System;

public class CutScene : MonoBehaviour
{
    Button scene;
    Label text;
    VisualElement upBar, downBar;

    private void OnEnable()
    {
        scene = UIReader_Main.Instance.root.Q<Button>("Scene");
        text = UIReader_Main.Instance.root.Q<VisualElement>("CutScene").Q<Label>("Text");
        upBar = UIReader_Main.Instance.root.Q<VisualElement>("CutScene").Q<VisualElement>("TopBar");
        downBar = UIReader_Main.Instance.root.Q<VisualElement>("CutScene").Q<VisualElement>("UnderBar");

        scene.clicked += (() => { GameManager.Instance.cutSceneManager.Next(); });
    }

    public void PlayCutScene(string name)
    {
        UIReader_Main.Instance.OpenCutScene();
        BarAnim(0f, 130.5f, 1f, 
            () => { GameManager.Instance.cutSceneManager.CutSceneReset(name); }, 
            () => { GameManager.Instance.cutSceneManager.CutScene(name); });
    }

    public void BarAnim(float start, float end, float during, Action startAction, Action endAction)
    {
        DOTween.To(() => start, x => start = x, end, during)
        .OnStart(() =>
        {
            startAction();
        })
        .OnUpdate(() =>
        {
            upBar.style.height = start;
            downBar.style.height = start;
        })
        .OnComplete(() => endAction());
    }

    public void ChangeCut(bool isAnim, float during, Sprite[] cuts)
    {
        if (isAnim)
            StartCoroutine(CutAnimation(during, cuts));
        else
            scene.style.backgroundImage = new StyleBackground(cuts[0]);
    }

    public void ChangeText(string msg, float writingDuring, Action action, string soundName)
    {
        UIReader_Main.Instance.DoText(text, msg, writingDuring, false, action, soundName);
    }

    public void EndText()
    {
        UIReader_Main.Instance.EndText();
    }

    IEnumerator CutAnimation(float during, Sprite[] cuts)
    {
        foreach (Sprite cut in cuts)
        {
            scene.style.backgroundImage = new StyleBackground(cut);
            yield return new WaitForSeconds(during);
        }
    }
}
