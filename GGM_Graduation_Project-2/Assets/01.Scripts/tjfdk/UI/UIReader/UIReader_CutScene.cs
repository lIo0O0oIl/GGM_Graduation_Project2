using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;

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

    public void ChangeCut(bool isAnim, Sprite[] cuts)
    {
        if (isAnim)
            StartCoroutine(CutAnimation(cuts));
        else
            scene.style.backgroundImage = new StyleBackground(cuts[0]);
    }

    public void ChangeText(string msg, float writingDuring)
    {
        DoText(text, msg, writingDuring, false, () => { });
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
