using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoActive : MonoBehaviour
{
    public float deley = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(Active());
    }

    private IEnumerator Active()
    {
        yield return new WaitForSeconds(deley);
        gameObject.SetActive(false);
    }
}
