using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_SelectHuman : MonoBehaviour
{
    private VisualElement SelectHumanRoot;
    private Button changeSizeBtn;
    
    public bool is_Open = true;        // 나중에 바꾸기
    private Tween changeSizeTween = null;
    [SerializeField] private float sizeOn = 350, sizeOff = 5;
    [SerializeField] private Texture2D changeSizeBtnOn, changeSizeBtnOff;


    private void Start()
    {
        SelectHumanRoot = UIReader_Main.Instance.GetSelectHumanSystem();
        SelectHumanRoot.style.flexBasis = new Length(sizeOff, LengthUnit.Pixel);

        changeSizeBtn = UIReader_Main.Instance.GetTopBar().Q<Button>("SelectHumanOpenBtn");
        if (changeSizeBtn != null)
        {
            changeSizeBtn.clicked += () => { OnOffThisSystem(0.25f); };
        }
    }

    public void OnOffThisSystem(float during)
    {
        is_Open = !is_Open;

        if (is_Open)
        {
            changeSizeTween = DOTween.To(() => SelectHumanRoot.style.flexBasis.value.value, x =>
                SelectHumanRoot.style.flexBasis = x, sizeOn, during);
            changeSizeBtn.style.backgroundImage = new StyleBackground(changeSizeBtnOn);
        }
        else
        {
            changeSizeTween = DOTween.To(() => SelectHumanRoot.style.flexBasis.value.value, x =>
                SelectHumanRoot.style.flexBasis = x, sizeOff, during);
            changeSizeBtn.style.backgroundImage = new StyleBackground(changeSizeBtnOff);
        }
    }
}
