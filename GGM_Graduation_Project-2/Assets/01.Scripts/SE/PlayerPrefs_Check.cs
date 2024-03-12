using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class PlayerPrefs_Check : MonoBehaviour
{
    private int count = 0;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject notice;

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    // IntPtr 은 핸들, 포인터를 말함.

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();         // 외부코드라는 것 명시

    public void OnBtnClick()
    {
        count++;
        PlayerPrefs.SetInt("Count", count);
    }

    public void OnDelBtnClick()
    {
        count = 0;
        PlayerPrefs.DeleteKey("Count");
        countText.text = "0";
    }

    private void Update()
    {
        if (PlayerPrefs.HasKey("Count"))
        {
            if (PlayerPrefs.GetInt("Count") != count)
            {
                ShowWindow(GetActiveWindow(), 2);
                notice.SetActive(true);
            }
            count = PlayerPrefs.GetInt("Count");
            countText.text = count.ToString();
        }
        else
        {
            countText.text = "0";
            count = 0;
        }
    }
}
