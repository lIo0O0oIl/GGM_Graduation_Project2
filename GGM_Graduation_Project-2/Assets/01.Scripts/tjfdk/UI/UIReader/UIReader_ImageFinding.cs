using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class UIReader_ImageFinding : UI_Reader
{
    // UXML
    VisualElement ui_imageGround;
    VisualElement ui_panelGround;



    // Template
    [SerializeField] VisualTreeAsset ux_imageGround;
    [SerializeField] VisualTreeAsset ux_imageEvidence;
    [SerializeField] VisualTreeAsset ux_evidenceExplanation;
    [SerializeField] VisualTreeAsset ux_ImagePanel;
    [SerializeField] VisualTreeAsset ux_TextPanel;



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
        ui_panelGround = root.Q<VisualElement>("PanelGround");
    }

    public void OpenImage(string fileName)
    {
        // find image
        ImageBig image = GameManager.Instance.imageManager.FindImage(fileName);

        // When image isn't null
        if (image != null)
        {
            if (isImageOpen)
            {
                // fileSystem size change button on
                GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Position;
                // image panel off
                ui_imageGround.style.display = DisplayStyle.None;

                // image panel clear
                for (int i = ui_imageGround.childCount - 1; i >= 0; i--)
                    ui_imageGround.RemoveAt(i);

                // image check action
                File fileT = GameManager.Instance.fileManager.FindFile(fileName);
                GameManager.Instance.fileManager.UnlockChat(fileT);
                GameManager.Instance.fileManager.UnlockChapter(fileT);

                // restart chatting
                StartCoroutine(GameManager.Instance.chapterManager.ReadChat());
            }
            else
            {
                // fileSystem size change button off
                GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Ignore;
                // fileSystem off bool value
                GameManager.Instance.fileSystem.isFileSystemOpen = true;
                // fileSystem size function
                GameManager.Instance.fileSystem.OnOffFileSystem(0f);

                // png panel ground on
                ui_imageGround.style.display = DisplayStyle.Flex;

                // create uxml
                VisualElement imagePanel = RemoveContainer(ux_imageGround.Instantiate());
                imagePanel.style.backgroundImage = new StyleBackground(image.image);

                // 이름으로 찾아주고
                foreach (string evid in image.pngName)
                {
                    // 해당 단서를 찾았다면
                    ImageSmall png = GameManager.Instance.imageManager.pngList[evid];
                    if (png != null)
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
                                    for (int i = imagePanel.childCount - 1; i >= 0; i--)
                                    {
                                        if (imagePanel.Children().ElementAt(i).name == "descriptionLabel")
                                            imagePanel.RemoveAt(i);
                                    }
                                    VisualElement evidenceDescription = RemoveContainer(ux_evidenceExplanation.Instantiate());
                                    evidenceDescription.name = "descriptionLabel";
                                    imagePanel.Add(evidenceDescription);
                                    DoText(evidenceDescription.Q<Label>("Text"), png.memo, 3f, true,
                                        () => { imagePanel.Remove(evidenceDescription); });
                                });
                            }

                            //단서 위치 설정
                            evidence.style.position = Position.Absolute;
                            evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                            evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                            evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                            evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                            // 단서를 이미지에 추가
                            imagePanel.Add(evidence);
                        }
                    }
                    else
                        Debug.Log("Evidence not found in pngList");
                }

                ui_imageGround.Add(imagePanel);
                StopCoroutine(GameManager.Instance.chapterManager.chatting);
            }

            isImageOpen = !isImageOpen;
        }
        // png 일 때
        else
        {
            // find png
            ImageSmall png = GameManager.Instance.imageManager.FindPng(fileName);

            // When png isn't null
            if (png != null)
            {
                // png panel clear
                for (int i = ui_panelGround.childCount - 1; i >= 0; i--)
                    ui_panelGround.RemoveAt(i);

                //create uxml
                VisualElement panel = RemoveContainer(ux_ImagePanel.Instantiate());
                // change png panel name
                panel.Q<Label>("Name").text = name + ".png";
                // change png image
                panel.Q<VisualElement>("Image").style.backgroundImage = new StyleBackground(png.saveSprite);
                // change png size
                ReSizeImage(panel.Q<VisualElement>("Image"), png.saveSprite);
                // connection exit click event
                panel.Q<Button>("CloseBtn").clicked += () =>
                {
                    // remove this panel 
                    ui_panelGround.Remove(panel);

                    // png check action
                    File file = GameManager.Instance.fileManager.FindFile(name);
                    GameManager.Instance.fileManager.UnlockChat(file);
                    GameManager.Instance.fileManager.UnlockChapter(file);

                    // restart chatting
                    StartCoroutine(GameManager.Instance.chapterManager.ReadChat());
                };


                ui_panelGround.Add(panel);
                StopCoroutine(GameManager.Instance.chapterManager.chatting);
            }
            else
                Debug.Log("it's neither an image nor a png");
        }

        //foreach (ImageBig image in GameManager.Instance.imageManager.images)
        //{
        //    // 해당 이미지를 찾았다면 배경 설정
        //    if (image.name == fileName.Q<Label>("FileName").text)
        //    {
        //        if (isImageOpen)
        //        {
        //            // filesystem 사이즈 변환 버튼 켜기
        //            GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Position;
        //            ui_imageGround.style.display = DisplayStyle.None;

        //            for (int i = ui_imageGround.childCount - 1; i >= 0; i--)
        //                ui_imageGround.RemoveAt(i);

        //            File fileT = GameManager.Instance.fileManager.FindFile(fileName.Q<Label>("FileName").text);
        //            GameManager.Instance.fileManager.UnlockChat(fileT);
        //            GameManager.Instance.fileManager.UnlockChapter(fileT);

        //            StartCoroutine(GameManager.Instance.chapterManager.ReadChat());
        //        }
        //        else
        //        {
        //            // filesystem 사이즈 변환 버튼 끄기
        //            GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Ignore;
        //            // filesystem 사이즈 작게 만들기
        //            GameManager.Instance.fileSystem.isFileSystemOpen = true;
        //            GameManager.Instance.fileSystem.OnOffFileSystem(0f);

        //            ui_imageGround.style.display = DisplayStyle.Flex;

        //            VisualElement textImage = RemoveContainer(ux_imageGround.Instantiate());
        //            textImage.style.backgroundImage = new StyleBackground(image.image);

        //            // 자식 단서들을 생성
        //            // 이름으로 찾아주고
        //            foreach (string evid in image.pngName)
        //            {
        //                // 해당 단서를 찾았다면
        //                foreach (ImageSmall png in GameManager.Instance.imageManager.pngs)
        //                {
        //                    if (evid == png.name)
        //                    {
        //                        // 생성
        //                        VisualElement evidence = null;
        //                        // 중요하다면
        //                        if (png.importance)
        //                        {
        //                            // 메모장으로 표시
        //                            evidence = RemoveContainer(ux_imageEvidence.Instantiate());
        //                            evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
        //                            evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = png.name;
        //                            evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = png.memo;
        //                            evidence.Q<Button>("EvidenceImage").clicked += (() =>
        //                            {
        //                                //VisualElement description = evidence.Q<VisualElement>("Descripte");
        //                                //description.style.display = DisplayStyle.Flex;

        //                                evidence.Q<VisualElement>("Descripte").style.display = DisplayStyle.Flex;
        //                                evidence.Q<VisualElement>("Descripte").style.left = png.pos.x + png.size.x;
        //                                evidence.Q<VisualElement>("Descripte").style.top = png.pos.y - 250;

        //                                if (png.isOpen == false)
        //                                {
        //                                    png.isOpen = true;
        //                                    Debug.Log(png.name + " " + image.name);
        //                                    GameManager.Instance.fileSystem.AddFile(FileType.IMAGE, png.name, image.name);
        //                                }
        //                            });
        //                        }
        //                        // 아니라면
        //                        else
        //                        {
        //                            // 아래 글로만 표시
        //                            evidence = RemoveContainer(ux_imageEvidence.Instantiate());
        //                            evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
        //                            evidence.Q<Button>("EvidenceImage").clicked += (() =>
        //                            {
        //                                for (int i = textImage.childCount - 1; i >= 0; i--)
        //                                {
        //                                    if (textImage.Children().ElementAt(i).name == "descriptionLabel")
        //                                        textImage.RemoveAt(i);
        //                                }
        //                                VisualElement evidenceDescription = RemoveContainer(ux_evidenceExplanation.Instantiate());
        //                                evidenceDescription.name = "descriptionLabel";
        //                                textImage.Add(evidenceDescription);
        //                                DoText(evidenceDescription.Q<Label>("Text"), png.memo, 3f, true,
        //                                    () => { textImage.Remove(evidenceDescription); });
        //                            });
        //                        }

        //                        //단서 위치 설정
        //                        evidence.style.position = Position.Absolute;
        //                        evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
        //                        evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
        //                        evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
        //                        evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
        //                        // 단서를 이미지에 추가
        //                        textImage.Add(evidence);
        //                    }
        //                }
        //            }

        //            ui_imageGround.Add(textImage);
        //            StopCoroutine(GameManager.Instance.chapterManager.chatting);
        //        }

        //        isImageOpen = !isImageOpen;
        //// Png일 때
        //foreach (ImageSmall png in GameManager.Instance.imageManager.pngs)
        //{
        //    if (png.name == fileName)
        //        GameManager.Instance.fileSystem.OpenImage(png.name, png.saveSprite);
        //}
    }

    public void OpenText(string name)
    {
        // create uxml
        VisualElement panel = RemoveContainer(ux_TextPanel.Instantiate());
        Text text = GameManager.Instance.imageManager.FindText(name);

        // change name
        panel.Q<Label>("Name").text = name + ".text";
        // change memo
        panel.Q<Label>("Text").text = text.memo;
        // connection click event
        panel.Q<Button>("CloseBtn").clicked += () =>
        {
            // remove this panel
            ui_panelGround.Remove(panel);

            // text check action
            File file = GameManager.Instance.fileManager.FindFile(name);
            GameManager.Instance.fileManager.UnlockChat(file);
            GameManager.Instance.fileManager.UnlockChapter(file);

            // restart chatting
            StartCoroutine(GameManager.Instance.chapterManager.ReadChat());
        };

        ui_panelGround.Add(panel);
        StopCoroutine(GameManager.Instance.chapterManager.chatting);
    }
}