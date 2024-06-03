using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using ChatVisual;

public class UI_Reader : MonoBehaviour
{
    // System
        // UI Reader
    protected TestUI mainSystem;
    protected UIReader_Chatting chatSystem;
    protected UIReader_Connection connectionSystem;
    protected UIReader_FileSystem fileSystem;
    protected UIReader_ImageFinding imageSystem;
    protected UIReader_CutScene cutSceneSystem;
        // Manager
    protected ChatContainer chatContainer;
    protected CutSceneManager cutSceneManager;
    protected FileManager fileManager;
    protected ImageManager imageManager;
    protected ChapterManager chapterManager;

    // main
    protected UIDocument document;
    protected VisualElement root;
    protected VisualElement myRoot;

    // UXML
        // Button
    protected Button chattingButton;
    protected Button connectionButton;
    protected Button settingButton;
        // Panel
    public VisualElement cutScenePanel;
    public VisualElement mainPanel;

    protected VisualElement previousPanel;
    protected VisualElement chattingPanel;
    protected VisualElement connectionPanel;
    protected VisualElement imageFindingPanel;
    private List<VisualElement> panels = new List<VisualElement>();

    Tween currentTextTween;

    protected void Awake()
    {
        // 서로 참조라 오류날 수도 있음
        // UI Reader
        mainSystem = GetComponent<TestUI>();
        chatSystem = GetComponent<UIReader_Chatting>();
        connectionSystem = GetComponent<UIReader_Connection>();
        fileSystem = GetComponent<UIReader_FileSystem>();
        imageSystem = GetComponent<UIReader_ImageFinding>();
        cutSceneSystem = GetComponent<UIReader_CutScene>();

        // Manager
        chatContainer = GameObject.Find("ChatManager").GetComponent<ChatContainer>();
        cutSceneManager = GetComponent<CutSceneManager>();
        fileManager = GetComponent<FileManager>();
        imageManager = GetComponent<ImageManager>();
        chapterManager = GetComponent<ChapterManager>();
    }

    protected void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        Load();

        //panels.Add(cutScenePanel);
        panels.Add(chattingPanel);
        panels.Add(connectionPanel);
        panels.Add(imageFindingPanel);
    }

    private void Load()
    {
        // Button
        chattingButton = root.Q<Button>("ChattingBtn");
        connectionButton = root.Q<Button>("ConnectionBtn");
        settingButton = root.Q<Button>("SoundSettingBtn");

        // Panel
        cutScenePanel = root.Q<VisualElement>("CutScene");
        mainPanel = root.Q<VisualElement>("MainGame");

        chattingPanel = root.Q<VisualElement>("Chatting");
        connectionPanel = root.Q<VisualElement>("Connection");
        imageFindingPanel = root.Q<VisualElement>("ImageFinding");
    }

    private void AddEvent()
    {
        chattingButton.clicked += () => { OpenPanel(chattingPanel); };
        connectionButton.clicked += () => { OpenPanel(connectionButton); };
        //settingButton.clicked += () => { OpenPanel(settingButton); };
    }

    public VisualElement RemoveContainer(VisualElement visualElement)
    {
        return visualElement[0];
    }

    public void OpenPanel(VisualElement currentPanel)
    {
        foreach (VisualElement panel in panels)
        {
            if (panel == currentPanel)
                panel.style.display = DisplayStyle.Flex;
            else
                panel.style.display = DisplayStyle.None;
        }
    }

    public void OpenCutScene(bool isOpen)
    {
        if (isOpen)
        {
            cutScenePanel.style.display = DisplayStyle.Flex;
            mainPanel.style.display= DisplayStyle.None;
        }
        else
        {
            cutScenePanel.style.display = DisplayStyle.None;
            mainPanel.style.display = DisplayStyle.Flex;
        }
    }

    public void DoText(Label ui, string text, float during, bool isErase, Action action)
    {
        int currentTextLength = 0;
        int previousTextLength = -1;

        currentTextTween = DOTween.To(() => currentTextLength, x => currentTextLength = x, text.Length, during)
            .SetEase(Ease.Linear)
            .OnPlay(() => { ui.text = ""; })
            .OnUpdate(() =>
            {
                if (currentTextLength != previousTextLength)
                {
                    ui.text += text[currentTextLength];
                    previousTextLength = currentTextLength;
                }
            })
            .OnComplete(() =>
            {
                if (isErase)
                    ui.text = "";
                currentTextLength = 0;
                action();
            });
    }

    public void EndText()
    {
        if (currentTextTween != null)
        {
            currentTextTween.Complete();
            currentTextTween = null;
        }
    }
}
