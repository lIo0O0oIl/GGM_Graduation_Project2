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
    public bool isFileSystemOpen;

    Tween changeFileSystemSizeDOT;

    // UXML
    VisualElement fileSystemArea;
    VisualElement fileGround;
    VisualElement filePathGround;
    VisualElement mainFilePath;
    VisualElement panelGround;
    public Button changeSizeButton;

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

    // file drag and drop
    private VisualElement fileDefaultArea;
    private List<VisualElement> lockQuestions = new List<VisualElement>();

    private void Awake()
    {
        base.Awake();
        fileFolders = new List<FolderFile>();

        MinWidth = 500f;
        MinHeight = 500f;
        MaxWidth = 1800f;
        MaxHeight = 980f;
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
            ChangeSize(0.25f);
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

    private void LoadDragAndDrop(VisualElement file, Action action)
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
        },
        () => { action(); }
        ));
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
                    // 드래그 앤 드롭 기능 추가
                    file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
                    {
                        var area = FindMoveArea(evt.mousePosition);
                        target.RemoveFromHierarchy();
                        if (area == null)
                        {
                            beforeSlot.Add(target);
                        }
                        else
                        {
                            LockAskAndReply lockQuestion = FindQuestion(area);
                            if (FindFile(fileName).lockQuestionName == lockQuestion.ask)
                            {
                                area.parent.Remove(area);
                                chatSystem.InputQuestion((chatSystem.FindMember(chatSystem.currentMemberName).nickName),
                                    EChatType.Question, lockQuestion.ask, true, chapterManager.InputCChat(true, lockQuestion.reply));
                            }
                            else
                                beforeSlot.Add(target);
                        }
                    },
                    () => { ImageEvent(file); }
                    ));

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
                    // 드래그 앤 드롭 기능 추가
                    file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
                    {
                        var area = FindMoveArea(evt.mousePosition);
                        target.RemoveFromHierarchy();
                        if (area == null)
                        {
                            beforeSlot.Add(target);
                        }
                        else
                        {
                            LockAskAndReply lockQuestion = FindQuestion(area);
                            if (FindFile(fileName).lockQuestionName == lockQuestion.ask)
                            {
                                area.parent.Remove(area);
                                chatSystem.InputQuestion((chatSystem.FindMember(chatSystem.currentMemberName).nickName),
                                    EChatType.Question, lockQuestion.ask, true, chapterManager.InputCChat(true, lockQuestion.reply));
                            }
                            else
                                beforeSlot.Add(target);
                        }
                    },
                    () => { TextEvent(file); }
                    ));

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
        ReSizeImage(panel.Q<VisualElement>("Image"), sprite);
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

    public void ChangeSize(float during)
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
                fileSystemArea.style.flexBasis = x, fileAreaSizeOn, during);
            changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOn);
        }
        else
        {
            changeFileSystemSizeDOT = DOTween.To(() => fileSystemArea.style.flexBasis.value.value, x => 
                fileSystemArea.style.flexBasis = x, fileAreaSizeOff, during);
            changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOff);
        }
    }
}