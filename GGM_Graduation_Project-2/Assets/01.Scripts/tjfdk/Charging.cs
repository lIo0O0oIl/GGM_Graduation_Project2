using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charging : MonoBehaviour
{
    [SerializeField] Image charging;
    public bool isFile = false;

    private void Awake()
    {
        charging.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isFile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                charging.gameObject.SetActive(true);
                charging.fillAmount = 0;
            }
            if (Input.GetMouseButton(0))
            {
                charging.fillAmount += Time.deltaTime * 2f;
                if (charging.fillAmount > 0.95f)
                {
                    charging.gameObject.SetActive(false);
                }
            }
        }

        charging.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            charging.gameObject.SetActive(false);
            charging.fillAmount = 0;
        }
    }
}
