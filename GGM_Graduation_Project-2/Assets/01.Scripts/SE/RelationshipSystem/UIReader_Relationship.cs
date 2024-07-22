using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_Relationship : MonoBehaviour
{
    public List<VisualElement> relationshipEvidenceList = new List<VisualElement>();

    private void Start()
    {
        relationshipEvidenceList = UIReader_Main.Instance.GetRelationshipEvidence();
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
}
