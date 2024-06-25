using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SceneChange(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
        //DontDestroyOnLoad(this.gameObject);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
