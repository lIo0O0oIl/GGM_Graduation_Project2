using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class RelationshipHuman
{
    public string name;
    public List<VisualElement> suspectArea = new List<VisualElement>();
    public List<VisualElement> evidenceArea = new List<VisualElement>();
}

public class UIReader_Relationship : MonoBehaviour
{
    public List<VisualElement> relationshipEvidenceList = new List<VisualElement>();
    public List<RelationshipHuman> relationshipHumanList = new List<RelationshipHuman>();

    private void Start()
    {
        UIReader_Main.Instance.GetRelationshipEvidenceArea(ref relationshipEvidenceList, ref relationshipHumanList);
    }

    public VisualElement GetEvidenceContains(Vector2 mousePos)
    {
        for (int i = 0; i < relationshipEvidenceList.Count; i++)
        {
            if (relationshipEvidenceList[i].worldBound.Contains(mousePos))
            {
                return relationshipEvidenceList[i];
            }
        }
        return null;
    }

    public void EvidenceCheck(VisualElement area)
    {
        List<MemberProfile> memberProfileList =  GameManager.Instance.chatSystem.Members;

        MemberProfile memberProfile = null;
        RelationshipHuman relationshipHuman = null;

        foreach (MemberProfile member in memberProfileList)
        {
            if (area.parent.name == member.nickName.ToString())
            {
                memberProfile = member;
                break;
            }
        }

        foreach (RelationshipHuman member in relationshipHumanList)
        {
            if (member.name == area.parent.name)
            {
                relationshipHuman = member;
            }
        }

        int count = 0;
        foreach (var evidenceSprite in memberProfile.evidence.spriteEvidence)
        {
            for(int i = 0; i < relationshipHuman.evidenceArea.Count; i++)
            {
                if (relationshipHuman.evidenceArea[i].style.backgroundImage.value.sprite == evidenceSprite)
                {
                    relationshipHuman.suspectArea[count].style.visibility = Visibility.Visible;
                    count++;
                }
            }
        }
    }

    public void CheckOther(Sprite ChangeSprite, VisualElement ChangeArea)
    {
        RelationshipHuman relationshipHuman = null;

        foreach (RelationshipHuman member in relationshipHumanList)
        {
            if (member.name == ChangeArea.parent.name)
            {
                relationshipHuman = member;
            }
        }


        foreach(VisualElement evidenceArea in relationshipHuman.evidenceArea)
        {
            if (evidenceArea == ChangeArea) continue;

            if (evidenceArea.style.backgroundImage.value.sprite == ChangeSprite)
            {
                Debug.Log(ChangeArea.style.backgroundImage.value);
                if (ChangeArea.style.backgroundImage.value != null)
                {
                    evidenceArea.style.backgroundImage = ChangeArea.style.backgroundImage;
                }
                else
                {
                    evidenceArea.style.backgroundImage = null;
                    evidenceArea.style.backgroundColor = Color.white;
                }

                break;
            }
        }
    }

    // VisualElement의 backgroundImage에서 Sprite 이름을 가져오는 메서드
    private string GetSpriteNameFromBackground(VisualElement element)
    {
        StyleBackground styleBackground = element.style.backgroundImage;
        if (styleBackground.value.texture != null)
        {
            // Texture2D에서 Sprite를 가져옵니다
            Sprite sprite = GetSpriteFromTexture(styleBackground.value.texture);
            if (sprite != null)
            {
                return sprite.name;
            }
        }
        return null;
    }

    // Texture2D에서 Sprite를 가져오는 메서드 (프로젝트의 모든 스프라이트를 검사)
    private Sprite GetSpriteFromTexture(Texture2D texture)
    {
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (Sprite sprite in sprites)
        {
            if (sprite.texture == texture)
            {
                return sprite;
            }
        }
        return null;
    }

}
