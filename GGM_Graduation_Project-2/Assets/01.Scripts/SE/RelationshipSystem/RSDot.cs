using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSDot : MonoBehaviour
{
    [HideInInspector] public RSLinkedData myData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌은 같은 레이어에 있는 애들만 가능함.
        myData.touchObj = other.gameObject.GetComponent<RSEvidenceData>();
        myData.is_LinkedObject = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        myData.touchObj = null;
        myData.is_LinkedObject = false;
    }
}
