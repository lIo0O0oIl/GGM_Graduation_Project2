using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIReader_MenuScene sound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoverButton =  eventData.pointerEnter;
        hoverButton.transform.DOScale(new Vector2(1.1f, 1.1f), 0.1f);
        sound.BtnSoundPlay();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject hoverButton = eventData.pointerEnter;
        hoverButton.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
    }
}
