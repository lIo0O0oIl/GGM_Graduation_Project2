using ChatVisual;
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
        //UIReader_Main.Instance.GetRelationshipEvidenceArea(ref relationshipEvidenceList, ref relationshipHumanList);
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

        // Text 도 해줘야함.
    }

    public void CheckOtherPng(Sprite ChangeSprite, VisualElement ChangeArea)
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

    public void CheckOtherText(string changeTooltip, VisualElement ChangeArea)
    {
        RelationshipHuman relationshipHuman = null;

        foreach (RelationshipHuman member in relationshipHumanList)
        {
            if (member.name == ChangeArea.parent.name)
            {
                relationshipHuman = member;
            }
        }

        foreach (VisualElement evidenceArea in relationshipHuman.evidenceArea)
        {
            if (evidenceArea == ChangeArea) continue;

            /*if (evidenceArea.tooltip == changeTooltip)          // 이미 있는 툴팁이였다면
            {
                if (ChangeArea.tooltip != null)     // 바뀌야 하는 곳에 이미 다른 툴팁이 존재해
                {
                    evidenceArea.tooltip = ChangeArea.tooltip;
                }
                else
                {
                    evidenceArea.style.backgroundImage = null;
                    evidenceArea.style.backgroundColor = Color.white;
                    evidenceArea.tooltip = null;
                }

                break;
            }*/
        }
    }
}
