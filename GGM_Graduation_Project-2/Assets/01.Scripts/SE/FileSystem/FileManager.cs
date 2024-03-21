using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 ������ �¾�Ƽ�긦 ���ٰ� Ű�� ������ �̵��� �����Ѵ�.
  
��δ� ����/�б�/��¼�� ������ ��.
 */

[Serializable]
public struct FileTree
{
    public string path;
    public GameObject[] Files;
}

public class FileManager : MonoBehaviour
{
    public static FileManager instance;     // ��𼭳� ȣ���� �� �ֵ���

    public BackBtn backBtn;         // �ڷΰ���
    public GameObject[] upLinePathBtn;     // ���ٿ� ��� ǥ�ÿ�

    private string nowPath;     // ���� ���
    private TMP_Text[] upLinePathText;       // ���ٿ� ��� ǥ�� �ؽ�Ʈ

    [Header("Image")]
    public GameObject imagePanel;       // �̹��� ����
    public TMP_Text imageName;      // ������ �̹����� �̸�
    public Image showImage;        // ������ �̹���
    private RectTransform imageSize;        // ������ �̹����� ������

    [Header("TextNote")]
    public GameObject textNotePanel;    // �޸��� ����
    public TMP_Text textName;      // ������ �ؽ�Ʈ�� �̸�
    public TMP_Text showText;       // ������ �ؽ�Ʈ

    [Header("Lock")]
    public GameObject lockPanel;        // ��� ����
    private LockSystem lockSystem;      // ��� ���� �ý���

    public FileTree[] fileTrees;        // ���� ��ü ����

    private void Awake()
    {
        instance = this;
        nowPath = "����";

        upLinePathText = new TMP_Text[upLinePathBtn.Length];
        for (int i = 0; i < upLinePathBtn.Length; i++)
        {
            upLinePathText[i] = upLinePathBtn[i].GetComponentInChildren<TMP_Text>();
            upLinePathBtn[i].SetActive(false);
        }

        imageSize = showImage.gameObject.GetComponent<RectTransform>();
        lockSystem = lockPanel.GetComponent<LockSystem>();
    }

    #region ���� �̵� ���� �Լ�
    /*
     GoFile �Լ�
    1. �� ��ο� ���� ��ΰ� ���� ����
    2. ���� ��� ������ ��� ��Ƽ�긦 ���ְ�
    3. �� ��ο� �ִ� ���ϵ��� ��� Ƽ�긦 ���ش�.
     */
    public void GoFile(string nowPath, string goPath)
    {
        Debug.Log($"���� ��� : {nowPath}, ������ ��� : {goPath}");

        foreach (var fileTree in fileTrees)
        {
            if (fileTree.path == nowPath)
            {
                foreach (var file in fileTree.Files)
                {
                    file.SetActive(false);
                }
            }

            if (fileTree.path == goPath)
            {
                foreach (var file in fileTree.Files)
                {
                    file.SetActive(true);
                }
            }
        }

        backBtn.nowPath = goPath;       // �ڷΰ��� �����
        this.nowPath = goPath;      // ���� ��� ǥ��

        // �� ��� ǥ�ÿ�
        if (this.nowPath.LastIndexOf('\\') != -1)
        {
            string[] path = this.nowPath.Split('\\');
            for (int i = 0;i < path.Length - 1; ++i)
            {
                upLinePathBtn[i].SetActive(true);
                upLinePathText[i].text = path[i + 1] + " >";
            }
            for (int i = path.Length - 1; i < 3; ++i)
            {
                upLinePathBtn[i].SetActive(false);      // ��ΰ� ���� ���̸� �����ֱ�
            }
        }
        else
        {
            for (int i = 0; i < 3; ++i)
            {
                upLinePathBtn[i].SetActive(false);      // ��ΰ� ���� �ƹ��͵� ������ �� �����ֱ�
            }
        }

        InvisibleFileManager.Instance.DontShowRound();      // ��� ���ذ� ������ ���ֱ�
    }

    public void GoMain()        // ���ٿ��� ������ ������ ��
    {
        GoFile(nowPath, "����");
    }

    public void UpLinePath(int num)
    {
        string[] names = nowPath.Split("\\");       // 1. nowPath�� \ �� �������� �����ش�.
        string buttonName = names[num];         //  2. ���� ��ư�� ���� �̸��� �������ش�.
        int index = nowPath.IndexOf(buttonName) + buttonName.Length;            // 2. ���� ��ư�� �ε��� + �����ŭ�� nowPath���� �����Ѵ�.
        string goPath = nowPath.Substring(0, index);        // ���ڿ��� ������ش�.
        GoFile(nowPath, goPath);
    }
    #endregion

    private string RemoveSpace(string name)     // �ٹٲ�, .(Ȯ����) ���� ������ �ִ� ��쿡 �����ֱ�
    {
        name = name.Replace("\n", "");

        int dotIndex = name.IndexOf('.');       // ���ִ� �ε��� ã��
        if (dotIndex >= 0 && name[dotIndex - 1] == ' ')     // �ڿ��� ������ ��쿡��
        {
            int lastSpaceIndex = name.LastIndexOf(" ");
            if (lastSpaceIndex >= 0 )
            {
                // '.' ������ ���� ����
                name = name.Remove(lastSpaceIndex, dotIndex - lastSpaceIndex);
            }
        }
        Debug.Log(name);
        return name;
    }

    #region �̹��� ���� ���� ���� �Լ�
    public void OpenImageFile(Sprite image, Vector2 scale, string name)
    {
        Debug.Log("�̹��� ����");

        showImage.sprite = image;
        imageSize.sizeDelta = scale;
        imageName.text = RemoveSpace(name);
        imagePanel.SetActive(true);
    }

    public void ImageBackClick()
    {
        imagePanel.SetActive(false);
    }
    #endregion

    #region �ؽ�Ʈ ���� ���� ���� �Լ�
    public void OpenTextFile(string text, string name)
    {
        Debug.Log("�ؽ�Ʈ ����");

        textNotePanel.SetActive(true);
        showText.text = text;
        textName.text = RemoveSpace(name);
    }

    public void TextBackClick()
    {
        textNotePanel.SetActive(false);
    }
    #endregion

    #region ��� ���� ���� ���� �Լ�
    public void OpenLock(string fileName, string password, Image lockImage)
    {
        Debug.Log("��� ����");

        lockPanel.SetActive(true);
        lockSystem.Init(fileName, password, lockImage);
    }

    public void LookBackClick()
    {
        lockPanel.SetActive(false);
    }
    #endregion
}
