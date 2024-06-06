using ChatVisual;
using DG.Tweening;
using System;
using System.Collections.Generic;
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
public class FolderFile
{
    public string folderName;
    public string parentFolderName;
    public List<VisualElement> folderFiles;
    public List<VisualElement> textFiles;
    public List<VisualElement> imageFiles;

    public FolderFile(string name)
    {
        folderName = name;
        parentFolderName = "Main";
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

    // foulder
    public string currentFolderName;

    // path
    public Stack<string> filePathLisk = new Stack<string>();
    public List<FolderFile> fileFolders;
    public FolderFile currentFileFolder;

    // file dran and drop
    private VisualElement fileDefaultArea;
    private List<VisualElement> lockQuestions = new List<VisualElement>();

    private void Awake()
    {
        base.Awake();
        fileFolders = new List<FolderFile>();
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Template_Load();
        Event_Load();

        fileFolders.Add(new FolderFile("Main"));
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

    private void FindLockQuestion()
    {
        MemberChat member = chatSystem.FindMember(chatSystem.currentMemberName);
        Debug.Log(member.name);
        for (int i = 0; i < member.quetions.Count; ++i)
        {
            if (member.quetions[i].chatType == EChatType.LockQuestion)
                lockQuestions.Add(chatSystem.questionGround.ElementAt(i));
        }
    }

    private LockAskAndReply FindQuestion(VisualElement ask)
    {
        Chapter chapter = chapterManager.FindChapter(chatSystem.FindMember(chatSystem.currentMemberName).chapterName);
        List<LockAskAndReply> asks = chapter.lockAskAndReply;
        for (int i = 0; i < chapter.lockAskAndReply.Count; ++i)
        {
            if (chapter.lockAskAndReply[i].ask == ask.name)
                return chapter.lockAskAndReply[i];
        }

        return new LockAskAndReply();
    }

    private VisualElement FindMoveArea(Vector2 position)
    {
        // 안 불려지는듯 
        Debug.Log("힝");
        FindLockQuestion();
        //Debug.Log(lockQuestions.Count);

        //모든 슬롯을 찾아서 그중에서 worldBound 에 position이 속하는 녀석을 찾아오면
        foreach (VisualElement moveArea in lockQuestions)
        {
            if (moveArea.worldBound.Contains(position)) //해당 RECT안에 포지션이 있는지 검사해
            {
                Debug.Log(moveArea.Q<Label>().text);
                return moveArea;
            }
        }
        return null;
    }

    private void LoadDragAndDrop(VisualElement file)
    {
        // 드래그 앤 드롭 기능 추가
        file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
        {
            Debug.Log("힝g힝");
            var area = FindMoveArea(evt.mousePosition);
            Debug.Log(area.name + " area 이름");
            target.RemoveFromHierarchy();
            if (area == null)
            {
                beforeSlot.Add(target);
            }
            else
                Debug.Log("잠긴 질문과 부딪힘");
        }));
    }

    private FolderFile FindFolder(string name)
    {
        foreach (FolderFile folder in fileFolders)
        {
            if (folder.folderName == name)
                return folder;
        }

        return null;
    }

    public FileT FindFile(string name)
    {
        foreach (FileT file in fileManager.folderFiles)
        {
            if (file.fileName == name)
                return file;
        }

        Debug.Log("File을 찾지 못 함");
        return null;
    }

    public void AddFile(FileType fileType, string fileName, string fileParentName)
    {
        VisualElement file = null;
        Debug.Log(fileName + " " + fileParentName);

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
                    FolderFile parentFolder = FindFolder(fileParentName);
                    if (parentFolder != null)
                    {
                        Debug.Log("찾음");
                        parentFolder.folderFiles.Add(file);
                        fileFolders.Add(new FolderFile(fileName));
                        addNew = true;
                    }

                    // 폴더 생성 및 추가
                    if (addNew == false)
                    {
                        Debug.Log("못 찾음");
                        FolderFile folderParent = new FolderFile(fileParentName);
                        fileFolders.Add(folderParent);
                        fileFolders.Add(new FolderFile(fileName));
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
                    // 드래그 앤 드롭 기능 추가
                    //LoadDragAndDrop(file);

                    //
                    // 드래그 앤 드롭 기능 추가
                    //file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
                    //{
                    //    var area = FindMoveArea(evt.mousePosition);
                    //    target.RemoveFromHierarchy();
                    //    if (area == null)
                    //    {
                    //        beforeSlot.Add(target);
                    //    }
                    //    else
                    //    {
                    //        LockAskAndReply lockQuestion = FindQuestion(area);
                    //        if (FindFile(fileName).lockQuestionName == lockQuestion.ask)
                    //        {
                    //            area.parent.Remove(area);
                    //            chatSystem.InputQuestion((chatSystem.FindMember(chatSystem.currentMemberName).nickName),
                    //                EChatType.Question, lockQuestion.ask, true, chapterManager.InputCChat(true, lockQuestion.reply));
                    //        }
                    //        else
                    //            beforeSlot.Add(target);
                    //    }
                    //}));
                    //

                    // 파일 부모 지정
                    bool addNew = false;
                    foreach (FolderFile folder in fileFolders)
                    {
                        // if you discover parent folder
                        if (folder.folderName == fileParentName)
                        {
                            folder.imageFiles.Add(file);
                            addNew = true;
                            break;
                        }
                    }

                    // 폴더 생성 및 추가
                    // if parentfolder not exist? new add
                    if (addNew == false)
                    {
                        AddFile(FileType.FOLDER, fileParentName, "Main");
                        AddFile(fileType, fileName, fileParentName);
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
                    // 드래그 앤 드롭 기능 추가
                    LoadDragAndDrop(file);

                    // 파일 부모 지정
                    bool addNew = false;
                    foreach (FolderFile folder in fileFolders)
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
                        //FolderFile folderParent = new FolderFile(fileParentName);
                        //fileFolders.Add(folderParent);
                        //fileFolders.Add(new FolderFile(fileName));
                        //folderParent.folderFiles.Add(file);

                        AddFile(FileType.FOLDER, fileParentName, "Main");
                        AddFile(fileType, fileName, fileParentName);
                    }
                    break;
                }
        }

        if (currentFolderName == "")
            currentFolderName = fileParentName;
        if (currentFolderName == fileParentName)
        {
            Debug.Log("tlqkfjdlafjwe");
            DrawFile(currentFolderName);
        }
    }

    public void DrawFile(string folderName)
    {
        // fileGround - file 그리는 곳
        // fileFolders - 현재 모든 폴더 배열
        // folderName - 현재 선택된 폴더 이름

        currentFolderName = folderName;

        // 이전에 있던 것들 다 지우고 (역순으로 지워야 오류 안 남)
        for (int i = fileGround.childCount - 1; i >= 0; i--)
            fileGround.RemoveAt(i);

        // 현재 폴더 변경
        foreach (FolderFile folder in fileFolders)
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
        for (int i = panelGround.childCount - 1; i >= 0; i--)
            panelGround.RemoveAt(i);

        VisualElement panel = RemoveContainer(ux_ImagePanel.Instantiate());
        panel.Q<Label>("Name").text = name + ".png";  
        panel.Q<VisualElement>("Image").style.backgroundImage = new StyleBackground(sprite);
        panel.Q<Button>("CloseBtn").clicked += () => 
        { 
            panelGround.Remove(panel);
        
            FileT file = FindFile(name);
            fileManager.UnlockChat(file);
            fileManager.UnlockChapter(file);
        };

        panelGround.Add(panel);
    }

    public void OpenText(string name, string text)
    {
        VisualElement panel = RemoveContainer(ux_TextPanel.Instantiate());
        panel.Q<Label>("Name").text = name + ".text";
        panel.Q<Label>("Text").text = text;
        panel.Q<Button>("CloseBtn").clicked += () => 
        { 
            panelGround.Remove(panel);

            FileT file = FindFile(name);
            fileManager.UnlockChat(file);
            fileManager.UnlockChapter(file);
        };

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
        //OpenPanel(imageFindingPanel);
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