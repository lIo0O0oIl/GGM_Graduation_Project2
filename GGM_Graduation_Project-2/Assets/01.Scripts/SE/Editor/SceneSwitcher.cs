using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    [MenuItem("Scenes/1. Game")]
    public static void OpenScene2()
    {
        OpenScene("Assets/00.Scenes/Main/Game.unity");
    }

    [MenuItem("Scenes/2. Intro")]
    public static void OpenScene1()
    {
        OpenScene("Assets/00.Scenes/Main/Intro.unity");
    }

    [MenuItem("Scenes/3. End")]
    public static void OpenScene3()
    {
        OpenScene("Assets/00.Scenes/Main/End.unity");
    }

    private static void OpenScene(string scenePath)
    {
        // 현재 씬이 수정되었는지 확인
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
