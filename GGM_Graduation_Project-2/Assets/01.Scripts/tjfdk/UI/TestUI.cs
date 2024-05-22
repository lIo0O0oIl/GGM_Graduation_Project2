using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEditor.Experimental.GraphView.GraphView;

//public enum FileType
//{
//    FOLDER,
//    IMAGE,
//    TEXT
//}

public class TestUI : MonoBehaviour
{
    private UIDocument document;
    private VisualElement root;

    public GameObject settingPanel;

    [Header("UXML")]
    [Header("Button")]
    Button chattingButton;
    Button connectionButton;
    Button settingButton;
    [Header("Panel")]
    VisualElement chattingPanel;
    VisualElement connectionPanel;
    VisualElement imageFindingPanel;
    [Header("Chat")]
    VisualElement chattingFace;

    [Header("Template")]
    [Header("Chat")]
    VisualElement chatGround;
    VisualTreeAsset ux_myChat;
    VisualTreeAsset ux_otherChat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;

    [Header("File")]
    public VisualElement fileGround;
    VisualTreeAsset ux_folderFile;
    VisualTreeAsset ux_imageFile;
    VisualTreeAsset ux_textFile;
    VisualTreeAsset ux_filePath;

    [Header("Evidence")]
    VisualElement testEvidence;


    //[Header("Sprite")]
    //[Header("Chat")]
    [SerializeField] private Sprite sp_speechrSprite;
    [SerializeField] private Sprite s_speechrSprite;
    //[Header("File")]
    //[SerializeField] private Sprite sp_folderSprite;
    //[SerializeField] private Sprite sp_imageSprite;
    //[SerializeField] private Sprite sp_textSprite;


    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        Load();
        //AddEvent();
    }

    private void Load()
    {
        // Button
        chattingButton = root.Q<Button>("ChattingBtn");
        connectionButton = root.Q<Button>("ConnectionBtn");
        settingButton = root.Q<Button>("SoundSettingBtn");

        // Panel
        chattingPanel = root.Q<VisualElement>("Chatting");
        //connectionPanel = root.Q<VisualElement>("Connection");
        imageFindingPanel = root.Q<VisualElement>("ImageFinding");

        // Chat
        chattingFace = root.Q<VisualElement>("ChatFace");

        // System Ground
        chatGround = root.Q<VisualElement>("ChatGround");
        fileGround = root.Q<VisualElement>("FileGround");

        //  Evidence
        testEvidence = root.Q<VisualElement>("Evidence");

        // connection
        conncectionPanel = root.Q<VisualElement>("SuspectGround");
        suspectPanel = conncectionPanel.Q<Button>("SuspectPanel");

        //DragAndDropManipulator manipulator =
        //    new DragAndDropManipulator(suspectPanel);
        //suspectPanel.AddManipulator(manipulator);

        //draggingElement = new VisualElement();
        //draggingElement.style.color = new Color(1, 1, 1, 1);

        // UXML Load

        // Chat
        ux_myChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\MyChat.uxml");
        ux_otherChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\OtherChat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\HiddenAskChat.uxml");

        // File
        ux_folderFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FolderFile.uxml");
        ux_imageFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\ImageFile.uxml");
        ux_textFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\TextFile.uxml");
        ux_filePath = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FolderFile.uxml");

        // Sprite Load

        // Chat

        // File

    }

    private void AddEvent()
    {
        chattingButton.clickable.clicked += () =>
        {
            chattingPanel.SetEnabled(true);
            imageFindingPanel.SetEnabled(false);
            //connectionPanel.SetEnabled(false);
        };

        connectionButton.clickable.clicked += () =>
        {
            Debug.Log("te");
            chattingPanel.SetEnabled(false);
            imageFindingPanel.SetEnabled(true);
            //connectionPanel.SetEnabled(true);
        };

        settingButton.clickable.clicked += () =>
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        };

        //testEvidence.Q<Button>("EvidenceImage").clickable.clicked += () =>
        //{
        //    FindEvidence(testEvidence.Q<Button>());
        //};

        //suspectPanel.RegisterCallback<PointerDownEvent>(OnMouseDown);
        //suspectPanel.RegisterCallback<MouseDownEvent>(OnMouseDown);

        suspectPanel.clicked += () =>
        {
            OnMouseDown(new MouseDownEvent());
        };
        //suspectPanel.RegisterCallback<MouseDownEvent>(OnMouseDown);
        suspectPanel.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InputQuestion(true, "크킄", actionTest);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            InputChat(false, "킄", sp_speechrSprite);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            InputChat(true, "크킄", s_speechrSprite);
        }
    }

    // 기능 독립 시키기...

    public void InputChat(bool isUser, string msg, Sprite face)
    {
        VisualElement chat = null;

        if (isUser)
            chat = ux_myChat.Instantiate();
        else
            chat = ux_otherChat.Instantiate();

        chattingFace.style.backgroundImage = new StyleBackground(face);
        chat.Q<Label>().text = msg;
        chatGround.Add(chat);
    }

    public void InputQuestion(bool isOpen, string msg, Action action)
    {
        VisualElement chat;

        if (isOpen)
        {
            chat = ux_askChat.Instantiate();
            chat.Q<Button>().clicked += action;
            chat.Q<Label>().text = msg;
        }
        else
            chat = ux_hiddenAskChat.Instantiate();

        chatGround.Add(chat);
    }

    public void AddEvidence()
    {
        // 단서를 먼저 찾아온 다음에 값을 연결

        // 일단 ㅍ리팹화 해둔 단서를 먼저 소환 .. 
    }

    //public void FindEvidence(Button button)
    //{

    //    VisualElement description = button.parent.Q<VisualElement>("Descripte");

    //    //Label title = button.parent.Q<Label>("EvidenceName");
    //    //Label description = button.parent.Q<Label>("Memo");

    //    description.style.display = DisplayStyle.Flex;
    //}

    public void AddFile(FileType fieType, string fileName, Action action, bool isRock)
    {
        VisualElement file = null;

        switch (fieType)
        {
            case FileType.FOLDER:
                file = ux_folderFile.Instantiate();
                break;
            case FileType.IMAGE:
                file = ux_imageFile.Instantiate();
                break;
            case FileType.TEXT:
                file = ux_textFile.Instantiate();
                break;
        }

        file.Q<Button>().clicked += action;
        file.Q<Label>().text = fileName;

        fileGround.Add(file);
    }

    public void AddFilePath(string pathName, Action action)
    {
        VisualElement filePath = null;
        filePath = ux_filePath.Instantiate();
        filePath.Q<Button>().clicked += action;
        filePath.Q<Button>().text = pathName;
        // 추가할 때 마다 배열 같은 곳에 순차적으로 저장 후 지워야 할 때 인덱스로 접근, FIleManager GoFile 참고
    }

    public void RemoveFilePath(int index)
    {
        // .
    }

    private void actionTest()       
    {
        Debug.Log("tlqkf");
        AddFile(FileType.FOLDER, "학교", actionTest, false);
    }

    // Test Line

    private VisualElement conncectionPanel;
    private Button suspectPanel;
    //private VisualElement draggingElement;

    private Vector2 startMousePos;
    private Vector2 endMousePos;

    //private bool dragging;
    //private Vector3 offset;

    public void OnMouseDown(MouseDownEvent evt)
    {
        Debug.Log(suspectPanel.style.left.value.value + ", " + suspectPanel.style.top.value.value);
        Debug.Log(suspectPanel.name);
        //startMousePos = evt.mousePosition;
        startMousePos = new Vector2(suspectPanel.style.left.value.value, suspectPanel.style.top.value.value);
        //suspectPanel.RegisterCallback<MouseUpEvent>(OnMouseUp);
        //dragging = true;
        //offset = draggingElement.worldTransform.GetPosition() - evt.position;
    }

    public void OnMouseUp(MouseUpEvent evt)
    {
        Debug.Log(evt.mousePosition);
        endMousePos = evt.mousePosition;
        //endMousePos = new Vector2(endMousePos.x - 10, endMousePos.y);
        DrawLine(startMousePos, endMousePos);
        //suspectPanel.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        Debug.Log(start + " stattstst");
        VisualElement startDot = new VisualElement();
        startDot.style.backgroundColor = new Color(1, 1, 1, 1);
        startDot.style.width = 5;
        startDot.style.height = 5;
        startDot.style.position = Position.Absolute;
        startDot.transform.position = start;
        startDot.style.left = start.x;
        startDot.style.top = start.y;

        VisualElement endDot = new VisualElement();
        endDot.style.backgroundColor = new Color(1, 1, 1, 1);
        endDot.style.width = 5;
        endDot.style.height = 5;
        endDot.style.position = Position.Absolute;
        endDot.style.left = end.x;
        endDot.style.top = end.y;

        // 선 생성
        VisualElement line = new VisualElement();
        line.style.backgroundColor = new Color(1, 1, 1, 1);
        line.style.position = Position.Absolute;
        line.style.width = Mathf.Sqrt(Mathf.Pow(end.x - start.x, 2) + Mathf.Pow(end.y - start.y, 2));
        line.style.height = 3;
        //line.transform.position = startMousePos
        line.style.left = start.x - 100; 
        line.style.top = start.y + 300; 
        line.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(end.y - start.y, end.x - start.x));

        // 시작점과 끝점, 선을 부모 요소에 추가
        conncectionPanel.Add(startDot);
        conncectionPanel.Add(endDot);
        conncectionPanel.Add(line);
    }
}
