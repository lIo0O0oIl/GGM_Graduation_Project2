using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddEvidence : MonoBehaviour
{
    //[SerializeField] private GameObject upload;
    [SerializeField] private GameObject evidencePanel;
    [SerializeField] private GameObject parent;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            EvidenceFile evidence = eventData.pointerDrag.GetComponent<EvidenceFile>();
            if (evidence.IsUseable)
            {
                //GameObject temp = Instantiate(evidencePanel);
                //temp.GetComponent<EvidencePanel>().

                //TextBox.Instance.InputFile(true, temp, evidence.Type);
            }

            //upload.SetActive(false);
        }
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Evidence"))
    //    {
    //        upload.SetActive(true);
    //        //eventData.pointerDrag.SetActive(false);
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Evidence"))
    //    {
    //        upload.SetActive(false);
    //        //eventData.pointerDrag.SetActive(true);
    //    }
    //}
}
