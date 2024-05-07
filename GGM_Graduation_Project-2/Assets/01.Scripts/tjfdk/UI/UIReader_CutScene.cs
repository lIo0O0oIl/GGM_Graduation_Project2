using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class UIReader_CutScene : MonoBehaviour
{
    CutSceneManager cutSceneManager;

    UIDocument document;
    VisualElement root;

    Button scene;
    Label text;

    Tween currentTextTween;

    private void OnEnable()
    {
        cutSceneManager = GetComponent<CutSceneManager>(); 

        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        scene = root.Q<Button>("Scene");
        text = root.Q<Label>("Text");

        scene.clicked += cutSceneManager.NextCut;
    }

    private void Update()
    {
        if  (Input.GetKeyDown(KeyCode.U))
        {
            ChangeText("너무 어려워요 도와주세요", 4f);
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
        int currentTextLength = 0;
        int previousTextLength = -1;

        currentTextTween = DOTween.To(() => currentTextLength, x => currentTextLength = x, msg.Length, writingDuring)
            .SetEase(Ease.Linear)
            .OnPlay(() => { text.text = ""; })
            .OnUpdate(() =>
            {
                if (currentTextLength != previousTextLength)
                {
                    text.text += msg[currentTextLength];
                    previousTextLength = currentTextLength;
                }
            })
            .OnComplete(() => 
            { 
                text.text = msg;
                currentTextLength = 0; 
            });
    }

    public void EndText()
    {
        if (currentTextTween != null)
        {
            currentTextTween.Complete();
            currentTextTween = null;
        }
    }
}
