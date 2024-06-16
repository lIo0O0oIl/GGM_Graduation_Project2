using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class UIReader_ImageFinding : UI_Reader
{
    // UXML
    VisualElement ui_imageGround;



    // Template
    [SerializeField] VisualTreeAsset ux_imageGround;
    [SerializeField] VisualTreeAsset ux_imageEvidence;
    [SerializeField] VisualTreeAsset ux_evidenceExplanation;



    bool isImageOpen;

    private void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
    }

    private void UXML_Load()
    {
        ui_imageGround = root.Q<VisualElement>("ImageFinding");
    }

    public void EventImage(VisualElement file)
    {
        // Image일 때
        foreach (ImageBig image in GameManager.Instance.imageManager.images)
        {
            // 해당 이미지를 찾았다면 배경 설정
            if (image.name == file.Q<Label>("FileName").text)
            {
                if (isImageOpen)
                {
                    // filesystem 사이즈 변환 버튼 켜기
                    GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Position;
                    ui_imageGround.style.display = DisplayStyle.None;

                    for (int i = ui_imageGround.childCount - 1; i >= 0; i--)
                        ui_imageGround.RemoveAt(i);

                    File fileT = GameManager.Instance.fileManager.FindFile(file.Q<Label>("FileName").text);
                    GameManager.Instance.fileManager.UnlockChat(fileT);
                    GameManager.Instance.fileManager.UnlockChapter(fileT);

                    StartCoroutine(GameManager.Instance.chapterManager.ReadChat());
                }
                else
                {
                    // filesystem 사이즈 변환 버튼 끄기
                    GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Ignore;
                    // filesystem 사이즈 작게 만들기
                    GameManager.Instance.fileSystem.isFileSystemOpen = true;
                    GameManager.Instance.fileSystem.OnOffFileSystem(0f);

                    ui_imageGround.style.display = DisplayStyle.Flex;
                    
                    VisualElement textImage = RemoveContainer(ux_imageGround.Instantiate());
                    textImage.style.backgroundImage = new StyleBackground(image.image);

                    // 자식 단서들을 생성
                    // 이름으로 찾아주고
                    foreach (string evid in image.pngName)
                    {
                        // 해당 단서를 찾았다면
                        foreach (ImageSmall png in GameManager.Instance.imageManager.pngs)
                        {
                            if (evid == png.name)
                            {
                                // 생성
                                VisualElement evidence = null;
                                // 중요하다면
                                if (png.importance)
                                {
                                    // 메모장으로 표시
                                    evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                                    evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                    evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = png.name;
                                    evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = png.memo;
                                    evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                    {
                                        //VisualElement description = evidence.Q<VisualElement>("Descripte");
                                        //description.style.display = DisplayStyle.Flex;

                                        evidence.Q<VisualElement>("Descripte").style.display = DisplayStyle.Flex;
                                        evidence.Q<VisualElement>("Descripte").style.left = png.pos.x + png.size.x;
                                        evidence.Q<VisualElement>("Descripte").style.top = png.pos.y - 250;

                                        if (png.isOpen == false)
                                        {
                                            png.isOpen = true;
                                            Debug.Log(png.name + " " + image.name);
                                            GameManager.Instance.fileSystem.AddFile(FileType.IMAGE, png.name, image.name);
                                        }
                                    });
                                }
                                // 아니라면
                                else
                                {
                                    // 아래 글로만 표시
                                    evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                                    evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                    evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                    {
                                        for (int i = textImage.childCount - 1; i >= 0; i--)
                                        {
                                            if (textImage.Children().ElementAt(i).name == "descriptionLabel")
                                                textImage.RemoveAt(i);
                                        }
                                        VisualElement evidenceDescription = RemoveContainer(ux_evidenceExplanation.Instantiate());
                                        evidenceDescription.name = "descriptionLabel";
                                        textImage.Add(evidenceDescription);
                                        DoText(evidenceDescription.Q<Label>("Text"), png.memo, 3f, true,
                                            () => { textImage.Remove(evidenceDescription); });
                                    });
                                }

                                //단서 위치 설정
                                evidence.style.position = Position.Absolute;
                                evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                                evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                                evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                                evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                                // 단서를 이미지에 추가
                                textImage.Add(evidence);
                            }
                        }
                    }

                    ui_imageGround.Add(textImage);
                    StopCoroutine(GameManager.Instance.chapterManager.chatting);
                }

                isImageOpen = !isImageOpen;
            }
        }

        // Png일 때
        foreach (ImageSmall png in GameManager.Instance.imageManager.pngs)
        {
            if (png.name == file.Q<Label>("FileName").text)
                GameManager.Instance.fileSystem.OpenImage(png.name, png.saveSprite);
        }
    }
}