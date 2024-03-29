using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class test2
{
    public string text;
    public bool isEnd;
}

[Serializable]
public class CutSceneChapter
{
    //public GameObject cut;
    public List<test2> texts = new List<test2>();
}

[CreateAssetMenu(fileName = "curSceneSO", menuName = "SO/CutSceneSO")]
public class CutSceneSO : ScriptableObject
{
    public string chapterName;
    public List<CutSceneChapter> cutScenes = new List<CutSceneChapter>();
}
