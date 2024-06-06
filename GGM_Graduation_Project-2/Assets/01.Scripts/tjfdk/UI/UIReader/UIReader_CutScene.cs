using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class UIReader_CutScene : UI_Reader
{
    Button scene;
    Label text;

    Tween currentTextTween;

    private void OnEnable()
    {
        base.OnEnable();

        scene = root.Q<Button>("Scene");
        text = root.Q<Label>("Text");

        scene.clicked += (() => { cutSceneManager.Next(); });
    }

    public void PlayCutScene(string name)
    {
        cutSceneManager.CutScene(true, name);
        OpenCutScene(true);
    }

    public void ChangeCut(Sprite cut)
    {
        scene.style.backgroundImage = new StyleBackground(cut);
    }

    public void ChangeText(string msg, float writingDuring)
    {
        DoText(text, msg, writingDuring, false, () => { });
    }
}
