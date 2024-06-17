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

        MinWidth = 1000;
        MinHeight = 700;
        MaxWidth = 1800f;
        MaxHeight = 980f;
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
        ImageSO image = GameManager.Instance.imageManager.FindImage(fileName);

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
                FileSO fileT = GameManager.Instance.fileManager.FindFile(fileName);
                Debug.Log(fileT.fileName);
                GameManager.Instance.fileManager.UnlockChat(fileT);

                //// restart chatting
                //Debug.Log("????ㅼ떆 ?몄텧");
                //GameManager.Instance.chapterManager.StartChatting();
            }
            else
            {
                // fileSystem size change button off
                GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Ignore;
                // fileSystem off bool value
                GameManager.Instance.fileSystem.isFileSystemOpen = false;
                // fileSystem size function
                GameManager.Instance.fileSystem.OnOffFileSystem(0f);

                // png panel ground on
                ui_imageGround.style.display = DisplayStyle.Flex;

                // create uxml
                VisualElement imagePanel = RemoveContainer(ux_imageGround.Instantiate());
                imagePanel.style.backgroundImage = new StyleBackground(image.image);

                // ?대쫫?쇰줈 李얠븘二쇨퀬
                foreach (string evid in image.pngName)
                {
                    // ?대떦 ?⑥꽌瑜?李얠븯?ㅻ㈃
                    PngSO png = GameManager.Instance.imageManager.FindPng(evid);
                    if (png != null)
                    {
                        if (evid == png.name)
                        {
                            // ?앹꽦
                            VisualElement evidence = null;
                            // 以묒슂?섎떎硫?
                            if (png.importance)
                            {
                                // 硫붾え?μ쑝濡??쒖떆
                                evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = png.name;
                                evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = png.memo;
                                evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                {
                                    //VisualElement description = evidence.Q<VisualElement>("Descripte");
                                    //description.style.display = DisplayStyle.Flex;

                                    evidence.Q<VisualElement>("Descripte").style.display = DisplayStyle.Flex;
                                    evidence.Q<VisualElement>("Descripte").style.left = png.memoPos.x;
                                    evidence.Q<VisualElement>("Descripte").style.top = png.memoPos.y;

                                    TextSO text = GameManager.Instance.imageManager.FindText(evid);
                                    if (text != null)
                                    {
                                        Debug.Log(text.name + " " + image.name);
                                        GameManager.Instance.fileSystem.AddFile(FileType.TEXT, text.name,
                                            GameManager.Instance.fileManager.FindFile(text.name).fileParentName);
                                    }
                                    else
                                    {
                                        Debug.Log(png.name + " " + image.name);
                                        GameManager.Instance.fileSystem.AddFile(FileType.IMAGE, png.name, 
                                            GameManager.Instance.fileManager.FindFile(png.name).fileParentName);
                                    }
                                });
                            }
                            // ?꾨땲?쇰㈃
                            else
                            {
                                // ?꾨옒 湲濡쒕쭔 ?쒖떆
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

                            //?⑥꽌 ?꾩튂 ?ㅼ젙
                            evidence.style.position = Position.Absolute;
                            evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                            evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                            evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                            evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                            // ?⑥꽌瑜??대?吏??異붽?
                            imagePanel.Add(evidence);
                        }
                    }
                    else
                        Debug.Log("Evidence not found in pngList");
                }

                //foreach (string evid in image.textName)
                //{
                //    // ?대떦 ?⑥꽌瑜?李얠븯?ㅻ㈃
                //    TextSO text = GameManager.Instance.imageManager.textList[evid];
                //    if (text != null)
                //    {
                //        if (evid == text.name)
                //        {
                //            // ?앹꽦
                //            VisualElement evidence = null;
                //            // 以묒슂?섎떎硫?
                //            if (text.importance)
                //            {
                //                // 硫붾え?μ쑝濡??쒖떆
                //                evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                //                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(text.image);
                //                evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = text.name;
                //                evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = text.memo;
                //                evidence.Q<Button>("EvidenceImage").clicked += (() =>
                //                {
                //                    //VisualElement description = evidence.Q<VisualElement>("Descripte");
                //                    //description.style.display = DisplayStyle.Flex;

                //                    evidence.Q<VisualElement>("Descripte").style.display = DisplayStyle.Flex;
                //                    evidence.Q<VisualElement>("Descripte").style.left = text.pos.x + text.size.x;
                //                    evidence.Q<VisualElement>("Descripte").style.top = text.pos.y - 250;

                //                    if (png.isOpen == false)
                //                    {
                //                        png.isOpen = true;
                //                        Debug.Log(png.name + " " + image.name);
                //                        GameManager.Instance.fileSystem.AddFile(FileType.IMAGE, png.name, image.name);
                //                    }
                //                });
                //            }
                //            // ?꾨땲?쇰㈃
                //            else
                //            {
                //                // ?꾨옒 湲濡쒕쭔 ?쒖떆
                //                evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                //                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                //                evidence.Q<Button>("EvidenceImage").clicked += (() =>
                //                {
                //                    for (int i = imagePanel.childCount - 1; i >= 0; i--)
                //                    {
                //                        if (imagePanel.Children().ElementAt(i).name == "descriptionLabel")
                //                            imagePanel.RemoveAt(i);
                //                    }
                //                    VisualElement evidenceDescription = RemoveContainer(ux_evidenceExplanation.Instantiate());
                //                    evidenceDescription.name = "descriptionLabel";
                //                    imagePanel.Add(evidenceDescription);
                //                    DoText(evidenceDescription.Q<Label>("Text"), png.memo, 3f, true,
                //                        () => { imagePanel.Remove(evidenceDescription); });
                //                });
                //            }

                //            //?⑥꽌 ?꾩튂 ?ㅼ젙
                //            evidence.style.position = Position.Absolute;
                //            evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                //            evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                //            evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                //            evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                //            // ?⑥꽌瑜??대?吏??異붽?
                //            imagePanel.Add(evidence);
                //        }
                //    }
                //    else
                //        Debug.Log("Evidence not found in pngList");
                //}

                ui_imageGround.Add(imagePanel);

                GameManager.Instance.chapterManager.StopChatting();;
            }

            isImageOpen = !isImageOpen;
        }
        // png ????
        else
        {
            // find png
            PngSO png = GameManager.Instance.imageManager.FindPng(fileName);

            // When png isn't null
            if (png != null)
            {
                // png panel clear
                for (int i = ui_panelGround.childCount - 1; i >= 0; i--)
                    ui_panelGround.RemoveAt(i);

                //create uxml
                VisualElement panel = RemoveContainer(ux_ImagePanel.Instantiate());
                // change png panel name
                panel.Q<Label>("Name").text = png.name + ".png";
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
                    Debug.Log(png.name + " 닫은 파일 이름");
                    FileSO file = GameManager.Instance.fileManager.FindFile(png.name);
                    GameManager.Instance.fileManager.UnlockChat(file);

                    //restart chatting
                    //Debug.Log("????ㅼ떆 ?몄텧");
                    //GameManager.Instance.chapterManager.StartChatting();
                };


                ui_panelGround.Add(panel);

                GameManager.Instance.chapterManager.StopChatting();
            }
            else
                Debug.Log("it's neither an image nor a png");
        }

        //foreach (ImageBig image in GameManager.Instance.imageManager.images)
        //{
        //    // ?대떦 ?대?吏瑜?李얠븯?ㅻ㈃ 諛곌꼍 ?ㅼ젙
        //    if (image.name == fileName.Q<Label>("FileName").text)
        //    {
        //        if (isImageOpen)
        //        {
        //            // filesystem ?ъ씠利?蹂??踰꾪듉 耳쒓린
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
        //            // filesystem ?ъ씠利?蹂??踰꾪듉 ?꾧린
        //            GameManager.Instance.fileSystem.ui_changeSizeButton.pickingMode = PickingMode.Ignore;
        //            // filesystem ?ъ씠利??묎쾶 留뚮뱾湲?
        //            GameManager.Instance.fileSystem.isFileSystemOpen = true;
        //            GameManager.Instance.fileSystem.OnOffFileSystem(0f);

        //            ui_imageGround.style.display = DisplayStyle.Flex;

        //            VisualElement textImage = RemoveContainer(ux_imageGround.Instantiate());
        //            textImage.style.backgroundImage = new StyleBackground(image.image);

        //            // ?먯떇 ?⑥꽌?ㅼ쓣 ?앹꽦
        //            // ?대쫫?쇰줈 李얠븘二쇨퀬
        //            foreach (string evid in image.pngName)
        //            {
        //                // ?대떦 ?⑥꽌瑜?李얠븯?ㅻ㈃
        //                foreach (ImageSmall png in GameManager.Instance.imageManager.pngs)
        //                {
        //                    if (evid == png.name)
        //                    {
        //                        // ?앹꽦
        //                        VisualElement evidence = null;
        //                        // 以묒슂?섎떎硫?
        //                        if (png.importance)
        //                        {
        //                            // 硫붾え?μ쑝濡??쒖떆
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
        //                        // ?꾨땲?쇰㈃
        //                        else
        //                        {
        //                            // ?꾨옒 湲濡쒕쭔 ?쒖떆
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

        //                        //?⑥꽌 ?꾩튂 ?ㅼ젙
        //                        evidence.style.position = Position.Absolute;
        //                        evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
        //                        evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
        //                        evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
        //                        evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
        //                        // ?⑥꽌瑜??대?吏??異붽?
        //                        textImage.Add(evidence);
        //                    }
        //                }
        //            }

        //            ui_imageGround.Add(textImage);
        //            StopCoroutine(GameManager.Instance.chapterManager.chatting);
        //        }

        //        isImageOpen = !isImageOpen;
        //// Png????
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
        TextSO text = GameManager.Instance.imageManager.FindText(name);

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
            FileSO file = GameManager.Instance.fileManager.FindFile(name);
            GameManager.Instance.fileManager.UnlockChat(file);

            //// restart chatting
            //Debug.Log("????ㅼ떆 ?몄텧");
            //GameManager.Instance.chapterManager.StartChatting();
        };

        ui_panelGround.Add(panel);
        GameManager.Instance.chapterManager.StopChatting();
    }
}