using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using ChatVisual;

public class UI_Reader : MonoBehaviour
{
    static public UI_Reader Instance;

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

    public Tween currentTextTween;

    protected float MinWidth = 500f;
    protected float MinHeight = 500f;
    protected float MaxWidth = 1800f;
    protected float MaxHeight = 980f;

    protected void Awake()
    {
        Instance = this;
    }

    protected void OnEnable()
    {
        document = GameManager.Instance.GetComponent<UIDocument>();
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

            GameManager.Instance.chatHumanManager.StopChatting();
        }
        else
        {
            cutScenePanel.style.display = DisplayStyle.None;
            mainPanel.style.display = DisplayStyle.Flex;

            GameManager.Instance.chatHumanManager.StartChatting();
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

    protected void ReSizeImage(VisualElement visualElement, Sprite sprite)
    {
        float originalWidth = sprite.rect.width;
        float originalHeight = sprite.rect.height;

        Vector2 adjustedSize = ChangeSize(originalWidth, originalHeight);

        visualElement.style.width = adjustedSize.x;
        visualElement.style.height = adjustedSize.y;

        visualElement.style.backgroundImage = new StyleBackground(sprite);
    }

    protected Vector2 ChangeSize(float originalWidth, float originalHeight)
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
