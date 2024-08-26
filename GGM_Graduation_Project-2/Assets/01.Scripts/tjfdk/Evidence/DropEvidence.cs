using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class DropEvidence : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject upload;

    public void OnDrop(PointerEventData eventData)
    {
        //if (eventData.pointerDrag != null)
        //{
        //    EvidenceFile evidence = eventData.pointerDrag.GetComponent<EvidenceFile>();
        //    if (evidence.IsUseable)
        //    {
        //        GameObject temp = new GameObject();
        //        if (evidence.Type == "Image")
        //            temp.AddComponent<Image>().sprite = evidence.Spriet;
        //        else
        //            temp.AddComponent<TextMesh>().text = evidence.Msg;

        //        TextBox.Instance.InputFile(true, temp, evidence.Type);
        //    }

        //    upload.SetActive(false);
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Evidence"))
        {
            upload.SetActive(true);
            //eventData.pointerDrag.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Evidence"))
        {
            upload.SetActive(false);
            //eventData.pointerDrag.SetActive(true);
        }
    }
}
