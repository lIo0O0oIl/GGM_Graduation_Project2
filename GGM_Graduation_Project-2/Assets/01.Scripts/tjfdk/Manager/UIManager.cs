using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        DontDestroyOnLoad(this.gameObject);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
