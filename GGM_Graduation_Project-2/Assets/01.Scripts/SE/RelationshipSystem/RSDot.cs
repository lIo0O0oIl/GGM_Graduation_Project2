using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSDot : MonoBehaviour
{
    [HideInInspector] public RSLinkedData myData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("닿앙");
        if (other.CompareTag("RelationshipData"))
        {
            myData.touchObj = other.gameObject;
            myData.is_LinkedObject = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        myData.is_LinkedObject = false;
    }
}
