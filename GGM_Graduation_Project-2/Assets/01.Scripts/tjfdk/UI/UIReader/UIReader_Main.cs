using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using ChatVisual;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum EPanel
{
    MAIN,
    CUTSCENE,
    SETTING,
    CONNECTION
}

public class UIReader_Main : MonoBehaviour
{
    static public UIReader_Main Instance;

    // main
    public UIDocument document;
    public VisualElement root;

    // UXML
    // Button
    //public Button playButton;
    //public Button ExitButton;
    public Button chattingButton;
    public Button connectionButton;
    public Button settingButton;
    // Panel
    public VisualElement mainPanel;
    public VisualElement cutScenePanel;
    public VisualElement settingPanel;
    public VisualElement connectionPanel;

    public bool isMainOpen;
    public bool isCutSceneOpen;
    public bool isSettingOpen;
    public bool isConnectionOpen;

    //protected VisualElement previousPanel;
    //protected VisualElement chattingPanel;
    //protected VisualElement imageFindingPanel;
    //private List<VisualElement> panels = new List<VisualElement>();

    public Tween currentTextTween;
    public string currentText;
    public Label currentTextUi;

    public float MinWidth = 500f;
    public float MinHeight = 500f;
    public float MaxWidth = 1800f;
    public float MaxHeight = 980f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        root = document.rootVisualElement;

        //if (document.visualTreeAsset.name == "Intro")
        //    MenuLoad();
        //else if (document.visualTreeAsset.name == "Game")
            GameLoad();
    }

    private void MenuLoad()
    {
        Debug.Log("메뉴");
        // Button
        //playButton = root.Q<Button>("PlayBtn");
        //settingButton = root.Q<Button>("SettingBtn");
        //ExitButton = root.Q<Button>("ExitBtn");

        settingPanel = root.Q<VisualElement>("Setting");

        //playButton.clicked += () => { SceneManager.LoadScene("Game"); };
        //settingButton.clicked += () => { OpenSetting(); };
        //settingButton.Q<Button>("ExitBtn").clicked += () => { OpenSetting(); };
        //ExitButton.clicked += () => { Application.Quit(); };
    }

    private void GameLoad()
    {
        // Panel

        chattingButton = root.Q<Button>("ChattingBtn");
        connectionButton = root.Q<Button>("ConnectionBtn");
        settingButton = root.Q<Button>("SoundSettingBtn");

        mainPanel = root.Q<VisualElement>("MainGame");
        cutScenePanel = root.Q<VisualElement>("CutScene");
        settingPanel = root.Q<VisualElement>("Setting");

        settingButton.clicked += () => { OpenSetting(); };
        settingPanel.Q<Button>("ExitBtn").clicked += () => { OpenSetting(); };
    }

    public VisualElement RemoveContainer(VisualElement visualElement)
    {
        return visualElement[0];
    }

    public void OpenSetting()
    {
        if (isSettingOpen)
            settingPanel.style.display = DisplayStyle.None;
        else
            settingPanel.style.display = DisplayStyle.Flex;

        isSettingOpen = !isSettingOpen;
    }

    //public void OpenPanel(EPanel panelType)
    //{
    //    VisualElement panel = null;
    //    bool isOpen = false;

    //    switch (panelType)
    //    {
    //        case EPanel.MAIN:
    //            panel = mainPanel;
    //            isOpen = isMainOpen;
    //            break;
    //        case EPanel.CUTSCENE:
    //            panel = cutScenePanel;
    //            isOpen = isCutSceneOpen;
    //            break;
    //        case EPanel.SETTING:
    //            panel = settingPanel;
    //            isOpen = isSettingOpen;
    //            break;
    //        case EPanel.CONNECTION:
    //            panel = connectionPanel;
    //            isOpen = isConnectionOpen;
    //            break;
    //    }
        
    //    if (isOpen)
    //        panel.style.display = DisplayStyle.None;
    //    else
    //        panel.style.display = DisplayStyle.Flex;

    //    // 이거 어쩔거여 ㅠㅡ
    //    isOpen = !isOpen;
    //}

    public void OpenCutScene()
    {
        isCutSceneOpen = !isCutSceneOpen;

        if (isCutSceneOpen)
        {            
            cutScenePanel.style.display = DisplayStyle.Flex;
            mainPanel.style.display= DisplayStyle.None;

            GameManager.Instance.chatHumanManager.StopChatting();
        }
        else
        {
            cutScenePanel.style.display = DisplayStyle.None;
            mainPanel.style.display = DisplayStyle.Flex;

            GameManager.Instance.chatHumanManager.StartChatting();
        }
    }

    public void DoText(Label ui, string text, float during, bool isErase, Action action,
        string soundName)
    {
        int currentTextLength = 0;
        int previousTextLength = -1;

        currentTextUi = ui;
        currentText = text;

        ui.text = "";

        currentTextTween = DOTween.To(() => currentTextLength, x => currentTextLength = x, text.Length, during)
            .SetEase(Ease.Linear)
            .OnPlay(() => 
            { 
                if (soundName != null)
                    SoundManager.Instance.PlaySFX(soundName);
            })
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
                action();

                SoundManager.Instance.StopSFX();

                if (isErase)
                    ui.text = "";
                currentTextLength = 0;
            });
    }

    public void EndText()
    {
        if (currentTextTween != null)
        {
            currentTextTween.Kill();
            currentTextUi.text = currentText;
            GameManager.Instance.cutSceneManager.currentTextNum++;
        }
    }

    public void ReSizeImage(VisualElement visualElement, Sprite sprite)
    {
        float originalWidth = sprite.rect.width;
        float originalHeight = sprite.rect.height;

        Vector2 adjustedSize = ChangeSize(originalWidth, originalHeight);

        visualElement.style.width = adjustedSize.x;
        visualElement.style.height = adjustedSize.y;

        visualElement.style.backgroundImage = new StyleBackground(sprite);
    }

    private Vector2 ChangeSize(float originalWidth, float originalHeight)
    {
        float aspectRatio = originalWidth / originalHeight;
        float adjustedWidth = originalWidth;
        float adjustedHeight = originalHeight;

        if (originalWidth > MaxWidth || originalHeight > MaxHeight)
        {
            if (aspectRatio > 1)
            {
                adjustedWidth = MaxWidth;
                adjustedHeight = MaxWidth / aspectRatio;
            }
            else
            {
                adjustedHeight = MaxHeight;
                adjustedWidth = MaxHeight * aspectRatio;
            }
        }

        if (adjustedWidth < MinWidth || adjustedHeight < MinHeight)
        {
            if (aspectRatio > 1)
            {
                adjustedWidth = MinWidth;
                adjustedHeight = MinWidth / aspectRatio;
            }
            else
            {
                adjustedHeight = MinHeight;
                adjustedWidth = MinHeight * aspectRatio;
            }
        }

        return new Vector2(adjustedWidth, adjustedHeight);
    }
}
