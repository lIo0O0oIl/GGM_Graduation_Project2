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

    private void OnEnable()
    {
        base.OnEnable();

        scene = root.Q<Button>("Scene");
        text = root.Q<Label>("Text");

        scene.clicked += (() => { GameManager.Instance.cutSceneManager.Next(); });
    }

    public void PlayCutScene(string name)
    {
        GameManager.Instance.cutSceneManager.CutScene(true, name);
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

    public void EndText() => base.EndText();

    IEnumerator CutAnimation(Sprite[] cuts)
    {
        foreach (Sprite cut in cuts)
        {
            scene.style.backgroundImage = new StyleBackground(cut);
            yield return new WaitForSeconds(0.75f);
        }
    }
}
