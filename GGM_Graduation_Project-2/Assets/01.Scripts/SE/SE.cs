using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DI = System.Diagnostics;

public class SE : MonoBehaviour
{
    private void Start()
    {
        var processList = DI.Process.GetProcessesByName("explorer");
        if (processList.Length > 0)
        {
            //프로세스가 1개이상 동작중..
            Debug.Log(processList.Length);
        }
        else
        {
            //실행된 프로세스 없음
            Debug.Log("0");
        }

       DI.Process.Start(@"D:\");
       //DI.Process.Start("IExplore.exe");

        int a = 0;
        DI.Process[] processes = DI.Process.GetProcesses();
        foreach (var process in processes)
        {
            if (process.ProcessName.ToLower().Contains("explorer"))
            {
                a++;
            }
        }
        Debug.Log(a);


    }
}
