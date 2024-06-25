using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using System;

public class UIReader_CutScene : MonoBehaviour
{
    Button scene;
    Label text;

    private void OnEnable()
    {
        scene = UI_Reader.Instance.root.Q<Button>("Scene");
        text = UI_Reader.Instance.root.Q<Label>("Text");

        scene.clicked += (() => { GameManager.Instance.cutSceneManager.Next(); });
    }

    public void PlayCutScene(string name)
    {
        GameManager.Instance.cutSceneManager.CutScene(true, name);
        UI_Reader.Instance.OpenCutScene();
    }

    public void ChangeCut(bool isAnim, Sprite[] cuts)
    {
        if (isAnim)
            StartCoroutine(CutAnimation(cuts));
        else
            scene.style.backgroundImage = new StyleBackground(cuts[0]);
    }

    public void ChangeText(string msg, float writingDuring, Action action, string soundName = "typing")
    {
        UI_Reader.Instance.DoText(text, msg, writingDuring, false, action, soundName);
    }

    public void EndText()
    {
        UI_Reader.Instance.EndText();
    }

    IEnumerator CutAnimation(Sprite[] cuts)
    {
        foreach (Sprite cut in cuts)
        {
            scene.style.backgroundImage = new StyleBackground(cut);
            yield return new WaitForSeconds(0.75f);
        }
    }
}
