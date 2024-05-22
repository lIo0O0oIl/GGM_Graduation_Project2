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

        scene.clicked += cutSceneManager.NextCut;
    }

    private void Update()
    {
        if  (Input.GetKeyDown(KeyCode.U))
        {
            DoText(text, "너무 어려워요 도와주세요", 4f, () => { });
        }
        if  (Input.GetKeyDown(KeyCode.I))
            EndText();
    }

    public void ChangeCut(Sprite cut)
    {
        scene.style.backgroundImage = new StyleBackground(cut);
    }

    public void ChangeText(string msg, float writingDuring)
    {
        DoText(text, msg, writingDuring, () => { });
    }

    public void EndText()
    {
        base.EndText();
    }
}
