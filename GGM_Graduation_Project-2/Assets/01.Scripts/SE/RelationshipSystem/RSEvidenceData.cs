using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RSEvidenceData : MonoBehaviour
{
     // Main
    public List<RSLinkedData> linkdataList = new List<RSLinkedData>();

    // My Type
    public FileType fileType = FileType.IMAGE;

    public void ChangeOtherLinePosition()
    {
        for (int i = 0; i < linkdataList.Count; i++)
        {
            linkdataList[i].ChangeLinePosition();
        }
    }
}
