using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ClueManager : Singleton<ClueManager>
{
    [SerializeField] private Text description;

    public void Texting(string msg)
    {
        description.DOKill();
        // 이전 텍스트 삭제
        description.text = "";
        //다트윈으로 텍스트 작성
        description.DOText(msg, 1.5f);
    }
}
