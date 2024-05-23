using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

[Serializable]
public class FileFolder
{
    public string folderName;
    public List<VisualElement> folderFiles;
    public List<VisualElement> textFiles;
    public List<VisualElement> imageFiles;

    public FileFolder(string name)
    {
        folderName = name;
        folderFiles = new List<VisualElement>();
        textFiles = new List<VisualElement>();
        imageFiles = new List<VisualElement>();
    }
}

public class UIReader_FileSystem : UI_Reader
{
    [SerializeField]
    private float fileAreaSizeOn, fileAreaSizeOff;
    [SerializeField]
    private Texture2D changeSizeBtnOn, changeSizeBtnOff;
    [SerializeField]
    private bool isFileSystemOpen;

    Tween changeFileSystemSizeDOT;

    // UXML
    VisualElement fileSystemArea;
    VisualElement fileGround;
    VisualElement filePathGround;
    VisualElement mainFilePath;
    VisualElement panelGround;
    Button changeSizeButton;

    // Template
    VisualTreeAsset ux_filePath;
    VisualTreeAsset ux_fileGround;
    VisualTreeAsset ux_folderFile;
    VisualTreeAsset ux_imageFile;
    VisualTreeAsset ux_textFile;
    VisualTreeAsset ux_ImagePanel;
    VisualTreeAsset ux_TextPanel;

    // test path
    public Stack<string> filePathLisk = new Stack<string>();
    public List<FileFolder> fileFolders;
    public FileFolder currentFileFolder;

    public string text_currentFolderName;

    private void Awake()
    {
        base.Awake();
        fileFolders = new List<FileFolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddFile(FileType.FOLDER, "학교", "Main");
            DrawFile("Main");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddFile(FileType.FOLDER, "정글", "학교");
            //DrawFile("학교");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddFile(FileType.FOLDER, "옥상", "Main");
            //DrawFile("학교");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddFile(FileType.IMAGE, "담배", "정글");
            //DrawFile("학교");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddFile(FileType.IMAGE, "옥상", "정글");
            //DrawFile("학교");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddFile(FileType.TEXT, "일기", "정글");
            //DrawFile("학교");
        }
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Template_Load();
        Event_Load();

        fileFolders.Add(new FileFolder("Main"));
        AddFilePath("Main");
    }

    private void UXML_Load()
    {
        fileSystemArea = root.Q<VisualElement>("FileSystem");
        fileGround = root.Q<VisualElement>("FileGround");
        filePathGround = root.Q<VisualElement>("FilePathGround");
        panelGround = root.Q<VisualElement>("PanelGround");
        changeSizeButton = root.Q<Button>("ChangeSize");
    }

    private void Template_Load()
    {
        ux_fileGround = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FileGround.uxml");
        ux_filePath = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FilePath.uxml");

        ux_folderFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FolderFile.uxml");
        ux_imageFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\ImageFile.uxml");
        ux_textFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\TextFile.uxml");

        ux_ImagePanel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\ImagePanel.uxml");
        ux_TextPanel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\TextPanel.uxml");
    }

    private void Event_Load()
    {
        //AddFolderGround("Main");
        //AddFilePath("Main", () => FolderPathEvent("Main"));

        changeSizeButton.clicked += () =>
        {
            ChangeSize();
        };
    }

    public void AddFile(FileType fileType, string fileName, string fileParentName)
    {
        VisualElement file = null;

        switch (fileType)
        {
            case FileType.FOLDER:
                {
                    // 생성
                    file = RemoveContainer(ux_folderFile.Instantiate());

                    // 이름 변경
                    file.Q<Label>("FileName").text = fileName;
                    // 이벤트 연결
                    file.Q<Button>("FileImage").clicked += () => 
                    { 
                        DrawFile(file.Q<Label>("FileName").text); 
                        AddFilePath(fileName); 
                    };
                    // 폴더 부모 지정
                    bool addNew = false;
                    foreach (FileFolder folder in fileFolders)
                    {
                        if (folder.folderName == fileParentName)
                        {
                            folder.folderFiles.Add(file);
                            fileFolders.Add(new FileFolder(fileName));
                            addNew = true;
                            break;
                        }
                    }

                    // 폴더 생성 및 추가
                    if (addNew == false)
                    {
                        FileFolder folderParent = new FileFolder(fileParentName);
                        fileFolders.Add(folderParent);
                        fileFolders.Add(new FileFolder(fileName));
                        folderParent.folderFiles.Add(file);
                    }
                    break;
                }
            case FileType.IMAGE:
                {
                    // 생성
                    file = RemoveContainer(ux_imageFile.Instantiate());
                    // 이름 변경
                    file.Q<Label>("FileName").text = fileName;
                    // 이벤트 연결
                    file.Q<Button>().clicked += () => ImageEvent(file); // 이미지 등록,,, 이미지 등록할 위치....

                    // 파일 부모 지정
                    bool addNew = false;
                    foreach (FileFolder folder in fileFolders)
                    {
                        if (folder.folderName == fileParentName)
                        {
                            folder.imageFiles.Add(file);
                            addNew = true;
                            break;
                        }
                    }

                    // 폴더 생성 및 추가
                    if (addNew == false)
                    {
                        FileFolder folderParent = new FileFolder(fileParentName);
                        fileFolders.Add(folderParent);
                        fileFolders.Add(new FileFolder(fileName));
                        folderParent.folderFiles.Add(file);
                    }
                    break;
                }
            case FileType.TEXT:
                {
                    // 생성
                    file = RemoveContainer(ux_textFile.Instantiate());
                    // 이름 변경
                    file.Q<Label>("FileName").text = fileName;
                    // 이벤트 연결
                    file.Q<Button>().clicked += () => TextEvent(file);

                    // 파일 부모 지정
                    bool addNew = false;
                    foreach (FileFolder folder in fileFolders)
                    {
                        if (folder.folderName == fileParentName)
                        {
                            folder.textFiles.Add(file);
                            addNew = true;
                            break;
                        }
                    }

                    // 폴더 생성 및 추가
                    if (addNew == false)
                    {
                        FileFolder folderParent = new FileFolder(fileParentName);
                        fileFolders.Add(folderParent);
                        fileFolders.Add(new FileFolder(fileName));
                        folderParent.folderFiles.Add(file);
                    }
                    break;
                }
        }

        Debug.Log(fileParentName);
        if (text_currentFolderName == "")
            text_currentFolderName = fileParentName;
        if (text_currentFolderName == fileParentName)
        {
            Debug.Log("tlqkfjdlafjwe");
            DrawFile(text_currentFolderName);
        }
    }

    public void DrawFile(string folderName)
    {
        // fileGround - file 그리는 곳
        // fileFolders - 현재 모든 폴더 배열
        // folderName - 현재 선택된 폴더 이름

        text_currentFolderName = folderName;

        // 이전에 있던 것들 다 지우고 (역순으로 지워야 오류 안 남)
        for (int i = fileGround.childCount - 1; i >= 0; i--)
            fileGround.RemoveAt(i);

        // 현재 폴더 변경
        foreach (FileFolder folder in fileFolders)
        {
            if (folder.folderName == folderName)
                currentFileFolder = folder;
        }

        if (currentFileFolder != null)
        {
            // 새로 그리기
            foreach (VisualElement folder in currentFileFolder.folderFiles)
                fileGround.Add(folder);
            foreach (VisualElement image in currentFileFolder.imageFiles)
                fileGround.Add(image);
            foreach (VisualElement text in currentFileFolder.textFiles)
                fileGround.Add(text);
        }
    }

    public void OpenImage(string name, Sprite sprite)
    {
        Debug.Log("dkd");

        for (int i = panelGround.childCount - 1; i >= 0; i--)
            panelGround.RemoveAt(i);

        VisualElement panel = RemoveContainer(ux_ImagePanel.Instantiate());
        panel.Q<Label>("Name").text = name + ".png";  
        panel.Q<VisualElement>("Image").style.backgroundImage = new StyleBackground(sprite);
        panel.Q<Button>("CloseBtn").clicked += () => { panelGround.Remove(panel); };
        panelGround.Add(panel);
    }

    public void OpenText(string name, string text)
    {
        VisualElement panel = RemoveContainer(ux_TextPanel.Instantiate());
        panel.Q<Label>("Name").text = name + ".text";
        panel.Q<Label>("Text").text = text;
        panel.Q<Button>("CloseBtn").clicked += () => { panelGround.Remove(panel); };
        panelGround.Add(panel);
    }

    private void AddFilePath(string pathName)
    {
        VisualElement filePath = RemoveContainer(ux_filePath.Instantiate());
        filePath.Q<Button>().text = pathName + "> ";
        filePath.Q<Button>().clicked += () => { FolderPathEvent(pathName); };
        filePathGround.Add(filePath);
        filePathLisk.Push(pathName);
    }

    private void FolderPathEvent(string fileName)
    {
        while (true)
        {
            if (filePathLisk.Peek() == fileName)
                break;
            filePathGround.RemoveAt(filePathGround.childCount - 1);
            filePathLisk.Pop();
        }

        DrawFile(filePathLisk.Peek());
    }

    public void ImageEvent(VisualElement file)
    {
        OpenPanel(imageFindingPanel);
        imageSystem.EventImage(file);
    }

    public void TextEvent(VisualElement file)
    {
        string name = file.Q<Label>("FileName").text;
        OpenText(name, imageManager.memoDic[name]);
    }

    public void ChangeSize()
    {
        isFileSystemOpen = !isFileSystemOpen;

        if (changeFileSystemSizeDOT != null)
        {
            changeFileSystemSizeDOT.Complete();
            changeFileSystemSizeDOT = null;
        }

        if (isFileSystemOpen)
        {
            changeFileSystemSizeDOT = DOTween.To(() => fileSystemArea.style.flexBasis.value.value, x => 
                fileSystemArea.style.flexBasis = x, fileAreaSizeOn, 0.25f);
            changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOn);
        }
        else
        {
            changeFileSystemSizeDOT = DOTween.To(() => fileSystemArea.style.flexBasis.value.value, x => 
                fileSystemArea.style.flexBasis = x, fileAreaSizeOff, 0.25f);
            changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOff);
        }

        //// 정상적인 버전...
        //fileSystemArea.
        //if (memberList.style.display.value == DisplayStyle.Flex)
        //{
        //    changeMemberButton.style.backgroundImage = new StyleBackground(changeMemberBtnOn);
        //    memberList.style.display = DisplayStyle.None;
        //}
        //else
        //{
        //    changeMemberButton.style.backgroundImage = new StyleBackground(changeMemberBtnOff);
        //    memberList.style.display = DisplayStyle.Flex;
        //}
    }
}

// 이전꺼
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UIElements;

//public enum FileType
//{
//    FOLDER,
//    IMAGE,
//    TEXT
//}

//public class UIReader_FileSystem : MonoBehaviour
//{
//    // other system
//    TestUI mainSystem;
//    UIReader_Chatting chatSystem;
//    UIReader_Connection connectionSystem;

//    // main
//    private UIDocument document;
//    private VisualElement root;
//    //private VisualElement fileRoot;

//    // UXML
//    VisualElement fileArea;
//    VisualElement filePathGround;
//    VisualElement mainFilePath;

//    // Template
//    VisualTreeAsset ux_folderFile;
//    VisualTreeAsset ux_imageFile;
//    VisualTreeAsset ux_textFile;
//    VisualTreeAsset ux_filePath;
//    VisualTreeAsset ux_fileGround;

//    // test path
//    public Stack<string> filePathLisk = new Stack<string>();

//    private void Awake()
//    {
//        mainSystem = GetComponent<TestUI>();
//        chatSystem = GetComponent<UIReader_Chatting>();
//        connectionSystem = GetComponent<UIReader_Connection>();

//        //currentFileGround = fileDefaultGround;
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            AddFile(FileType.FOLDER, "Main", "학교");
//        }

//        if (Input.GetKeyDown(KeyCode.O))
//        {
//            AddFile(FileType.IMAGE, "Main", "정글");
//        }
//    }

//    private void OnEnable()
//    {
//        document = GetComponent<UIDocument>();
//        root = document.rootVisualElement;
//        //fileRoot = root.Q<VisualElement>("");

//        UXML_Load();
//        Template_Load();
//        Event_Load();
//    }

//    private void UXML_Load()
//    {
//        fileArea = root.Q<VisualElement>("FileArea");
//        filePathGround = root.Q<VisualElement>("FilePath");

//        //mainFilePath = filePathGround.Q<VisualElement>("");
//    }

//    private void Template_Load()
//    {
//        ux_folderFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FolderFile.uxml");
//        ux_imageFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\ImageFile.uxml");
//        ux_textFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\TextFile.uxml");
//        ux_fileGround = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FileGround.uxml");
//        ux_filePath = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FilePath.uxml");
//    }

//    private void Event_Load()
//    {
//        //filePathLisk.Push(mainFilePath.Q<Button>().text);
//        //mainFilePath.Q<Button>().clicked += () =>
//        //{
//        //    FolderPathEvent(mainFilePath.Q<Button>().text);
//        //};

//        AddFilePath("Main", () => FolderPathEvent("Main"));
//    }

//    // Function
//    public void AddFile(FileType fieType, string fileGroundName, string fileName, bool isRock = false)
//    {
//        // ����
//        VisualElement file = null;

//        // ���� Ÿ�� ������
//        switch (fieType)
//        {
//            case FileType.FOLDER:
//                {
//                    // ���� ���� �� �̺�Ʈ ����
//                    file = ux_folderFile.Instantiate();
//                    file.Q<Button>().clicked += () => FolderEvent(file);

//                    // ������ ������ ���� �� ���
//                    VisualElement newFileGround = ux_fileGround.Instantiate();
//                    newFileGround.name += "FileGround_" + fileName;
//                    fileArea.Add(newFileGround); // FileGround_�б�
//                    break;
//                }
//            case FileType.IMAGE:
//                // ���� ���� �� �̺�Ʈ ����
//                file = ux_imageFile.Instantiate();
//                file.Q<Button>().clicked += () => ImageEvent(file);
//                break;
//            case FileType.TEXT:
//                // ���� ���� �� �̺�Ʈ ����
//                file = ux_textFile.Instantiate();
//                file.Q<Button>().clicked += () => NoteEvent(file);
//                break;
//        }

//        // ���� �̸� ����
//        file.Q<Label>().text = fileName;

//        // ���� �����ϴ� �����̶�� �ش� ������ �߰�
//        foreach (VisualElement fileGround in fileArea.Children())
//        {
//            int index = fileGround.name.IndexOf('_');
//            if (index != -1)
//            {
//                if (fileGround.name.Substring(index + 1) == fileGroundName)
//                {
//                    fileGround.Add(file);
//                    return;
//                }
//            }
//        }

//        // �ƴ϶�� �ش� ������ ���� �� �߰�
//        {
//            VisualElement newFileGround = ux_fileGround.Instantiate();
//            newFileGround.name += "FileGround_" + fileGroundName;
//            newFileGround.style.display = DisplayStyle.None;
//            fileArea.Add(newFileGround);
//            newFileGround.Add(file);
//        }
//    }

//    private void FolderEvent(VisualElement folder)
//    {
//        // ���� �̸� ����
//        string folderName = folder.Q<Label>("FileName").text;

//        // �ش� ���� ������ Ȱ��ȭ
//        OpenFileGround(folderName);
//        // ���� ��� �߰�
//        AddFilePath(folderName, () => FolderPathEvent(folderName));
//    }

//    private void ImageEvent(VisualElement image)
//    {
//        Debug.Log("�̹��� ���� Ȱ��ȭ");
//    }

//    private void NoteEvent(VisualElement note)
//    {
//        Debug.Log("�ؽ�Ʈ ���� Ȱ��ȭ");
//    }

//    private void AddFilePath(string pathName, Action action)
//    {
//        // �̹� �����Ѵٸ� Ȱ��ȭ
//        foreach (VisualElement filePath in filePathGround.Children())
//        {
//            if (StringSplit(filePath.name, '_') == pathName)
//            {
//                filePath.style.display = DisplayStyle.Flex;
//                filePathLisk.Push(filePath.name);
//                return;
//            }
//        }

//        {
//            Debug.Log("��� ����");
//            // ����
//            VisualElement filePath = null;
//            filePath = ux_filePath.Instantiate();

//            // ��� �̸� ���� (UI �̸�)
//            filePath.name += "FilePathText_" + pathName;
//            // ��� �̸� ����
//            filePath.Q<Button>().text = pathName + " > ";
//            // ��� �̺�Ʈ ����
//            filePath.Q<Button>().clicked += action;
//            // ��� �߰�
//            filePathGround.Add(filePath);

//            // �߰��� �� ���� �迭 ���� ���� ���������� ���� �� ������ �� �� �ε����� ����, FIleManager GoFile ����
//            filePathLisk.Push(filePath.name);
//        }
//    }

//    private void FolderPathEvent(string fileName)
//    {
//        // fileName���� ���� �ִ� ���� ��Ȱ��ȭ
//        while (true)
//        {
//            if (StringSplit(filePathLisk.Peek(), '_') == fileName)
//                break;
//            Debug.Log(StringSplit(filePathLisk.Peek(), '_') + " " + " ��� false");
//            filePathGround.Q<VisualElement>(filePathLisk.Peek()).style.display = DisplayStyle.None;
//            //filePathGround.Remove(filePathGround.Q<VisualElement>("FilePathText" + '_' + filePathLisk.Peek()));
//            // �߰��� ���� Add�� ���� ���� active false�� �������� ��, �� �� ���� ������ ���ߵ� Ȱ��ȭ ��Ȱ��ȭ�� ���ߵ� ���Ͻ��Ѷ�
//            filePathLisk.Pop();
//        }

//        OpenFileGround(StringSplit(filePathLisk.Peek(), '_'));
//    }

//    private void OpenFileGround(string fileName)
//    {
//        // FileArea�� ��� Ground ��
//        foreach (VisualElement fileGround in fileArea.Children())
//        {
//            // �ش� ������ ������ 
//            //Debug.Log(StringSplit(fileGround.name, '_') + " " + fileName + " open");
//            if (StringSplit(fileGround.name, '_') == fileName)
//            {
//                fileGround.style.display = DisplayStyle.Flex;
//                Debug.Log(StringSplit(fileGround.name, '_') + " ���� true");
//            }
//            // �ƴ϶�� ����
//            else
//            {
//                fileGround.style.display = DisplayStyle.None;
//                Debug.Log(StringSplit(fileGround.name, '_') + " ���� false");
//            }
//        }
//    }

//    private string StringSplit(string str, char t)
//    {
//        int index = str.IndexOf(t);
//        if (index != -1)
//            return str.Substring(index + 1);
//        else
//        {
//            Debug.LogError("String Split ����");
//            return null;
//        }
//    }
//}