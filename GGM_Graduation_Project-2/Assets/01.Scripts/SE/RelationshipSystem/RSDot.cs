using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSDot : MonoBehaviour
{
    [HideInInspector] public RSLinkedData myData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RelationshipData"))
        {
            Debug.Log("연결가능한 데이터와 닿음");
            myData.touchObj = other.gameObject;
            myData.is_LinkedObject = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        myData.is_LinkedObject = false;
    }
}
