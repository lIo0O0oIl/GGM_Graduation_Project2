using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class TestUI : UI_Reader
{
    private void Start()
    {
        chatSystem.AddMember("HG");
        chapterManager.AddChapter("HG", "시체 조사");
        chatSystem.ChoiceMember(chatSystem.FindMember("HG"));
    }

    private void Update()
    {
        
    }
}
