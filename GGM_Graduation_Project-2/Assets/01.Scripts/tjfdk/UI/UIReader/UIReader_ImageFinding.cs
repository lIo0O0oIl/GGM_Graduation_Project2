using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
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

    public void OpenImage(VisualElement fileIcon, string fileName)
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
                GameManager.Instance.fileManager.FindFile(fileName).isRead = true;
                //Debug.Log(fileT.fileName);
                if (fileT != null)
                    GameManager.Instance.fileManager.UnlockChat(fileT.name);
                if (GameManager.Instance.fileManager.FindFile(fileName) == true)
                    fileIcon.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;
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

                // ???藥??怨쀬Ŧ 嶺뚢돦堉싮뇡?놃떊??┑?
                foreach (string evid in image.pngName)
                {
                    // ???????觀???嶺뚢돦堉싮뇡???좊듆
                    PngSO png = GameManager.Instance.imageManager.FindPng(evid);
                    if (png != null)
                    {
                        if (evid == png.name)
                        {
                            // ??諛댁뎽
                            VisualElement evidence = null;
                            // 繞벿살탳???濡ル펲嶺?
                            if (png.importance)
                            {
                                // 嶺뚮∥???????뿉???戮?뻣
                                evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = png.name;
                                evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = png.memo;
                                evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                {
                                    evidence.Q<VisualElement>("Descripte").style.display = DisplayStyle.Flex;
                                    evidence.Q<VisualElement>("Descripte").style.left = png.memoPos.x;
                                    evidence.Q<VisualElement>("Descripte").style.top = png.memoPos.y;

                                    TextSO text = GameManager.Instance.imageManager.FindText(evid);
                                    if (text != null)
                                    {
                                        //Debug.Log(text.name + " " + image.name);
                                        GameManager.Instance.fileSystem.AddFile(FileType.TEXT, text.name,
                                            GameManager.Instance.fileManager.FindFile(text.name).fileParentName);
                                    }
                                    else
                                    {
                                        //Debug.Log(png.name + " " + image.name);
                                        GameManager.Instance.fileSystem.AddFile(FileType.IMAGE, png.name, 
                                            GameManager.Instance.fileManager.FindFile(png.name).fileParentName);
                                    }

                                    evidence.Q<Button>("EvidenceImage").pickingMode = PickingMode.Ignore;
                                });
                            }
                            // ?熬곣뫀鍮??寃밸듆
                            else
                            {
                                // ?熬곣뫁???リ섣??β돦裕녶퐲???戮?뻣
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
                                    DoText(evidenceDescription.Q<Label>("Text"), png.memo, 2f, true,
                                        () => { imagePanel.Remove(evidenceDescription); });
                                });
                            }

                            //??觀???熬곣뫚?????깆젧
                            evidence.style.position = Position.Absolute;
                            evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                            evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                            evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                            evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                            // ??觀???????嶺뚯솘????怨뺣뼺?
                            imagePanel.Add(evidence);
                        }
                    }
                    else
                        Debug.Log("Evidence not found in pngList");
                }

                //foreach (string evid in image.textName)
                //{
                //    // ???????觀???嶺뚢돦堉싮뇡???좊듆
                //    TextSO text = GameManager.Instance.imageManager.textList[evid];
                //    if (text != null)
                //    {
                //        if (evid == text.name)
                //        {
                //            // ??諛댁뎽
                //            VisualElement evidence = null;
                //            // 繞벿살탳???濡ル펲嶺?
                //            if (text.importance)
                //            {
                //                // 嶺뚮∥???????뿉???戮?뻣
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
                //            // ?熬곣뫀鍮??寃밸듆
                //            else
                //            {
                //                // ?熬곣뫁???リ섣??β돦裕녶퐲???戮?뻣
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

                //            //??觀???熬곣뫚?????깆젧
                //            evidence.style.position = Position.Absolute;
                //            evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                //            evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                //            evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                //            evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                //            // ??觀???????嶺뚯솘????怨뺣뼺?
                //            imagePanel.Add(evidence);
                //        }
                //    }
                //    else
                //        Debug.Log("Evidence not found in pngList");
                //}

                ui_imageGround.Add(imagePanel);

                GameManager.Instance.chatHumanManager.StopChatting();;
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
                    FileSO file = GameManager.Instance.fileManager.FindFile(png.name);
                    GameManager.Instance.fileManager.FindFile(png.name).isRead = true;
                    if (file != null)
                        GameManager.Instance.fileManager.UnlockChat(file.name);
                    if (GameManager.Instance.fileManager.FindFile(png.name).isRead == true)
                        fileIcon.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;
                };


                ui_panelGround.Add(panel);

                GameManager.Instance.chatHumanManager.StopChatting();
            }
            else
                Debug.Log("it's neither an image nor a png");
        }
    }

    public void OpenText(VisualElement fileIcon, string name)
    {
        // create uxml
        VisualElement panel = RemoveContainer(ux_TextPanel.Instantiate());
        // find text
        TextSO text = GameManager.Instance.imageManager.FindText(name);

        // When png isn't null
        if (text != null)
        {
            // text panel clear
            for (int i = ui_panelGround.childCount - 1; i >= 0; i--)
                ui_panelGround.RemoveAt(i);

            // change name
            panel.Q<Label>("Name").text = name + ".text";
            // change memo
            panel.Q<Label>("Text").text = text.memo;
            // connection exit click event
            panel.Q<Button>("CloseBtn").clicked += () =>
            {
                // remove this panel 
                ui_panelGround.Remove(panel);

                // text check action
                FileSO file = GameManager.Instance.fileManager.FindFile(name);
                GameManager.Instance.fileManager.FindFile(name).isRead = true;
                if (file != null)
                    GameManager.Instance.fileManager.UnlockChat(file.name);
                if (GameManager.Instance.fileManager.FindFile(name).isRead == true)
                    fileIcon.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;
            };


            ui_panelGround.Add(panel);

            GameManager.Instance.chatHumanManager.StopChatting();
        }
    }
}