using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CutSceneText
{
    public string sound;
    public string text;
    public bool vibration;
}

[Serializable]
public class CutSceneDialoges
{
    public bool isAnim;
    public Sprite[] cut;
    public List<CutSceneText> texts = new List<CutSceneText>();
}

[CreateAssetMenu(fileName = "curSceneSO", menuName = "SO/CutSceneSO")]
public class CutSceneSO : ScriptableObject                                           
{
    public string chapterName;
    public string nextMemberName;
    public List<CutSceneDialoges> cutScenes = new List<CutSceneDialoges>();
}
