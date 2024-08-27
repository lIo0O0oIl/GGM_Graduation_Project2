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

    private void OnEnable()
    {
        scene = UIReader_Main.Instance.root.Q<Button>("Scene");
        text = UIReader_Main.Instance.root.Q<VisualElement>("CutScene").Q<Label>("Text");

        scene.clicked += (() => { GameManager.Instance.cutSceneManager.Next(); });
    }

    public void PlayCutScene(string name)
    {
        GameManager.Instance.cutSceneManager.CutScene(name);
        UIReader_Main.Instance.OpenCutScene();
    }

    public void ChangeCut(bool isAnim, float during, Sprite[] cuts)
    {
        if (isAnim)
            StartCoroutine(CutAnimation(during, cuts));
        else
            scene.style.backgroundImage = new StyleBackground(cuts[0]);
    }

    public void ChangeText(string msg, float writingDuring, Action action, string soundName, bool vibration)
    {
        UIReader_Main.Instance.DoText(text, msg, writingDuring, false, action, soundName, vibration);
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
