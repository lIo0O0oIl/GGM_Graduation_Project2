using System.Collections;
using System.Collections.Generic;
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
                if (image.isOpen)
                    imageGround.Remove(imageGround.Q<VisualElement>(image.name));
                else
                {
                    VisualElement imageBackground = RemoveContainer(ux_imageGround.Instantiate());
                    imageBackground.name = image.name;
                    imageBackground.style.backgroundImage = new StyleBackground(image.image);
                    imageGround.Add(imageBackground);

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
                                        VisualElement description = evidence.Q<VisualElement>("Descripte");
                                        description.style.display = DisplayStyle.Flex;

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
                                        VisualElement evidenceDescription = RemoveContainer(ux_evidenceExplanation.Instantiate());
                                        imageGround.Add(evidenceDescription);
                                        DoText(evidenceDescription.Q<Label>("Text"), png.memo, 3f, 
                                            () => { imageGround.Remove(evidenceDescription); });
                                    });
                                }

                                //단서 위치 설정
                                evidence.style.position = Position.Absolute;
                                evidence.style.left = png.pos.x;
                                evidence.style.top = png.pos.y;
                                // 단서를 이미지에 추가
                                imageBackground.Add(evidence);
                            }
                        }
                    }
                }

                image.isOpen = !image.isOpen;
            }
        }

        // Png일 때
        foreach (ImagePng png in imageManager.pngs)
        {
            if (png.name == file.Q<Label>("FileName").text)
            {
                fileSystem.OpenImage(png.name, png.image);
            }
        }
    }
}
        
            //foreach (ImageDefualt image in imageManager.images)
            //{
            //    if (image.name == file.Q<Label>("FileName").text)
            //    {
            //        // 이미지를 켜. 채팅이든 뭐든 끄고
            //        // for문 돌면서? 자식 까고? - evidence 템플릿을 가져와!
            //        // 구분을 해. 만약에 중요한거면 템플릿 쓰고, 아니면 그냥 이미지만...

            //        // visualelement로 배경 깔고?
            //        VisualElement imageBackground = RemoveContainer(ux_imageGround.Instantiate());
            //        // 이미지를 세팅해.
            //        imageBackground.style.backgroundImage = new StyleBackground(image.image);
            //        // 이미지를 추가
            //        imageGround.Add(imageBackground);

            //        // 해당 이미지의 단서들 배치
            //        foreach (ImagePng evid in image.)
            //        {
            //            VisualElement evidence = null;
            //            // 중요한 단서라면
            //            if (evid.importance)
            //            {
            //                // 생성 및 이미지 생성
            //                evidence = RemoveContainer(ux_imageEvidence.Instantiate());
            //                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(evid.evidenceImage);
            //                // 단서 자료 추가
            //                evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = evid.evidenceName;
            //                evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = evid.evidenceMemo;
            //                // 이벤트 연결 - 메모장 켜지는 이벤트
            //                evidence.Q<Button>("EvidenceImage").clicked += (() =>
            //                {
            //                    VisualElement description = evidence.Q<VisualElement>("Descripte");
            //                    description.style.display = DisplayStyle.Flex;
            //                });
            //            }
            //            else
            //            {
            //                // 생성 및 이미지 생성
            //                evidence = RemoveContainer(ux_imageGround.Instantiate());
            //                evidence.style.backgroundImage = new StyleBackground(evid.evidenceImage);
            //                // 이벤트 연결 - 아래 생각만 켜지는 이벤트
            //                evidence.Q<Button>("EvidenceImage").clicked += (() =>
            //                {
            //                    // 수정 필요함...
            //                    evidenceExplanation.style.display = DisplayStyle.Flex;
            //                    DoText(evidenceExplanation.Q<Label>("Text"), evid.evidenceMemo,
            //                        3f, () => { });
            //                });
            //            }

            //            // 단서 위치 설정
            //            evidence.style.position = Position.Absolute;
            //            evidence.style.left = evid.evidencePos.x;
            //            evidence.style.top = evid.evidencePos.y;
            //            // 단서를 이미지에 추가
            //            imageBackground.Add(evidence);
            //        }
            //    }
            //}