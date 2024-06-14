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
    VisualElement imageGround;
    //VisualElement evidenceExplanation;

    // Template
    VisualTreeAsset ux_imageGround;
    VisualTreeAsset ux_imageEvidence;
    VisualTreeAsset ux_evidenceExplanation;

    bool isImageOpen;

    private void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Template_Load();
    }

    private void UXML_Load()
    {
        imageGround = root.Q<VisualElement>("ImageFinding");
        //evidenceExplanation = root.Q<VisualElement>("EvidenceDescript");
    }

    private void Template_Load()
    {
        ux_imageGround = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Evidence\\ImageGround.uxml");
        ux_imageEvidence = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Evidence\\Evidence.uxml");
        ux_evidenceExplanation = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Evidence\\EvidenceDescript.uxml");
    }

    public void EventImage(VisualElement file)
    {
        // Image일 때
        foreach (ImageB image in imageManager.images)
        {
            // 해당 이미지를 찾았다면 배경 설정
            if (image.name == file.Q<Label>("FileName").text)
            {
                if (isImageOpen)
                {
                    // filesystem 사이즈 변환 버튼 켜기
                    fileSystem.changeSizeButton.pickingMode = PickingMode.Position;
                    imageGround.style.display = DisplayStyle.None;

                    for (int i = imageGround.childCount - 1; i >= 0; i--)
                        imageGround.RemoveAt(i);

                    FileT fileT = fileSystem.FindFile(file.Q<Label>("FileName").text);
                    fileManager.UnlockChat(fileT);
                    fileManager.UnlockChapter(fileT);
                }
                else
                {
                    // filesystem 사이즈 변환 버튼 끄기
                    fileSystem.changeSizeButton.pickingMode = PickingMode.Ignore;
                    // filesystem 사이즈 작게 만들기
                    fileSystem.isFileSystemOpen = true;
                    fileSystem.ChangeSize(0f);

                    imageGround.style.display = DisplayStyle.Flex;
                    
                    VisualElement textImage = RemoveContainer(ux_imageGround.Instantiate());
                    textImage.style.backgroundImage = new StyleBackground(image.image);

                    // 자식 단서들을 생성
                    // 이름으로 찾아주고
                    foreach (string evid in image.pngName)
                    {
                        // 해당 단서를 찾았다면
                        foreach (ImagePng png in imageManager.pngs)
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
                                            fileSystem.AddFile(FileType.IMAGE, png.name, image.name);
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

                    imageGround.Add(textImage);
                }

                isImageOpen = !isImageOpen;
            }
        }

        // Png일 때
        foreach (ImagePng png in imageManager.pngs)
        {
            if (png.name == file.Q<Label>("FileName").text)
                fileSystem.OpenImage(png.name, png.saveImage);
        }
    }
}