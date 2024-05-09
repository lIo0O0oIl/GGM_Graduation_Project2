using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum FileType
{
    FOLDER,
    IMAGE,
    TEXT
}

public class UIReader_FileSystem : MonoBehaviour
{
    // other system
    TestUI mainSystem;
    UIReader_Chatting chatSystem;
    UIReader_Connection connectionSystem;

    // main
    private UIDocument document;
    private VisualElement root;
    //private VisualElement fileRoot;

    // UXML
    VisualElement fileArea;
    VisualElement filePathGround;
    VisualElement mainFilePath;

    // Template
    VisualTreeAsset ux_folderFile;
    VisualTreeAsset ux_imageFile;
    VisualTreeAsset ux_textFile;
    VisualTreeAsset ux_filePath;
    VisualTreeAsset ux_fileGround;

    // test path
    public Stack<string> filePathLisk = new Stack<string>();

    private void Awake()
    {
        mainSystem = GetComponent<TestUI>();
        chatSystem = GetComponent<UIReader_Chatting>();
        connectionSystem = GetComponent<UIReader_Connection>();

        //currentFileGround = fileDefaultGround;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddFile(FileType.FOLDER, "Main", "학교");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddFile(FileType.IMAGE, "학교", "이잉");
        }
    }

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        //fileRoot = root.Q<VisualElement>("");

        UXML_Load();
        Template_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        fileArea = root.Q<VisualElement>("FileArea");
        filePathGround = root.Q<VisualElement>("FilePath");

        //mainFilePath = filePathGround.Q<VisualElement>("");
    }

    private void Template_Load()
    {
        ux_folderFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FolderFile.uxml");
        ux_imageFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\ImageFile.uxml");
        ux_textFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\TextFile.uxml");
        ux_fileGround = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FileGround.uxml");
        ux_filePath = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FilePathText.uxml");
    }

    private void Event_Load()
    {
        //filePathLisk.Push(mainFilePath.Q<Button>().text);
        //mainFilePath.Q<Button>().clicked += () =>
        //{
        //    FolderPathEvent(mainFilePath.Q<Button>().text);
        //};

        AddFilePath("Main", () => FolderPathEvent("Main"));
    }

    // Function
    public void AddFile(FileType fieType, string fileGroundName, string fileName, bool isRock = false)
    {
        // 생성
        VisualElement file = null;

        // 파일 타입 나누기
        switch (fieType)
        {
            case FileType.FOLDER:
            {
                // 파일 생성 및 이벤트 연결
                file = ux_folderFile.Instantiate();
                file.Q<Button>().clicked += () => FolderEvent(file);

                // 폴더의 영역을 생성 후 등록
                VisualElement newFileGround = ux_fileGround.Instantiate();
                newFileGround.name += "FileGround_" + fileName;
                fileArea.Add(newFileGround); // FileGround_학교
                break;
            }
            case FileType.IMAGE:
                // 파일 생성 및 이벤트 연결
                file = ux_imageFile.Instantiate();
                file.Q<Button>().clicked += () => ImageEvent(file);
                break;
            case FileType.TEXT:
                // 파일 생성 및 이벤트 연결
                file = ux_textFile.Instantiate();
                file.Q<Button>().clicked += () => NoteEvent(file);
                break;
        }

        // 파일 이름 변경
        file.Q<Label>().text = fileName;

        // 만약 존재하는 영역이라면 해당 영역에 추가
        foreach (VisualElement fileGround in fileArea.Children())
        {
            int index = fileGround.name.IndexOf('_');
            if (index != -1)
            {
                if (fileGround.name.Substring(index + 1) == fileGroundName)
                {
                    fileGround.Add(file);
                    return;
                }
            }
        }

        // 아니라면 해당 영역을 생성 후 추가
        {
            VisualElement newFileGround = ux_fileGround.Instantiate();
            newFileGround.name += "FileGround_" + fileGroundName;
            newFileGround.style.display = DisplayStyle.None;
            fileArea.Add(newFileGround);
            newFileGround.Add(file);
        }
    }

    private void FolderEvent(VisualElement folder)
    {
        // 폴더 이름 저장
        string folderName = folder.Q<Label>("FileName").text;

        // 해당 폴더 영역만 활성화
        OpenFileGround(folderName);
        // 파일 경로 추가
        AddFilePath(folderName, () => FolderPathEvent(folderName));
    }

    private void ImageEvent(VisualElement image)
    {
        Debug.Log("이미지 파일 활성화");
    }

    private void NoteEvent(VisualElement note)
    {
        Debug.Log("텍스트 파일 활성화");
    }

    private void AddFilePath(string pathName, Action action)
    {
        // 이미 존재한다면 활성화
        foreach(VisualElement filePath in filePathGround.Children())
        {
            if (StringSplit(filePath.name, '_') == pathName)
            {
                filePath.style.display = DisplayStyle.Flex;
                filePathLisk.Push(filePath.name);
                return;
            }
        }

        {
            Debug.Log("경로 생성");
            // 생성
            VisualElement filePath = null;
            filePath = ux_filePath.Instantiate();

            // 경로 이름 변경 (UI 이름)
            filePath.name += "FilePathText_" + pathName;
            // 경로 이름 변경
            filePath.Q<Button>().text = pathName + " > ";
            // 경로 이벤트 연결
            filePath.Q<Button>().clicked += action;
            // 경로 추가
            filePathGround.Add(filePath);

            // 추가할 때 마다 배열 같은 곳에 순차적으로 저장 후 지워야 할 때 인덱스로 접근, FIleManager GoFile 참고
            filePathLisk.Push(filePath.name);
        }
    }

    private void FolderPathEvent(string fileName)
    {
        // fileName보다 위에 있는 영역 비활성화
        while (true)
        {
            if (StringSplit(filePathLisk.Peek(), '_') == fileName)
                break;
            Debug.Log(StringSplit(filePathLisk.Peek(), '_') + " " + " 경로 false");
            filePathGround.Q<VisualElement>(filePathLisk.Peek()).style.display = DisplayStyle.None;
            //filePathGround.Remove(filePathGround.Q<VisualElement>("FilePathText" + '_' + filePathLisk.Peek()));
            // 추가할 때는 Add고 지울 때는 active false라서 오류났던 것, 둘 다 생성 삭제로 맞추든 활성화 비활성화로 맞추든 통일시켜라
            filePathLisk.Pop();
        }

        OpenFileGround(StringSplit(filePathLisk.Peek(), '_'));
    }

    private void OpenFileGround(string fileName)
    {
        // FileArea의 모든 Ground 중
        foreach (VisualElement fileGround in fileArea.Children())
        {
            // 해당 폴더의 영역을 
            //Debug.Log(StringSplit(fileGround.name, '_') + " " + fileName + " open");
            if (StringSplit(fileGround.name, '_') == fileName)
            {
                fileGround.style.display = DisplayStyle.Flex;
                Debug.Log(StringSplit(fileGround.name, '_') + " 영역 true");
            }
            // 아니라면 끈다
            else
            {
                fileGround.style.display = DisplayStyle.None;
                Debug.Log(StringSplit(fileGround.name, '_') + " 영역 false");
            }
        }
    }

    private string StringSplit(string str, char t)
    {
        int index = str.IndexOf(t);
        if (index != -1)
            return str.Substring(index + 1);
        else
        {
            Debug.LogError("String Split 에러");
            return null;
        }
    }
}
