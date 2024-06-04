using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class TestUI : UI_Reader
{
    private void Start()
    {
        //chatSystem.AddMember("HG");
        //chapterManager.AddChapter("HG", "사건 시작");
        //chatSystem.ChoiceMember(chatSystem.FindMember("HG"));

        //fileSystem.AddFile(FileType.IMAGE, "신발", "Main");
    }

    private void Update()
    {
        chatSystem.EndToScroll();
    }
}
