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

    [SerializeField] GameObject MenuObj;
    [SerializeField] GameObject settingObj;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(settingObj);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        settingObj = GameObject.Find("Setting");
        settingObj.SetActive(false);
        settingObj.GetComponent< UIReader_SettingScene>().BringDefaultValue();
    }

    public void SceneChange(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
        DontDestroyOnLoad(this.gameObject);
    }

    public void OpenSetting(bool is_open)
    {
        settingObj.SetActive(is_open);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
