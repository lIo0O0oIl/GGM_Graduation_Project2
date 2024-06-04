using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class TestUI : UI_Reader
{
    private void Start()
    {
        chatSystem.AddMember("HG");
        chapterManager.AddChapter("HG", "초기 수사 선택");
        chatSystem.ChoiceMember(chatSystem.FindMember("HG"));

        fileSystem.AddFile(FileType.IMAGE, "신발", "Main");
    }

    private void Update()
    {
        chatSystem.EndToScroll();
    }
}
