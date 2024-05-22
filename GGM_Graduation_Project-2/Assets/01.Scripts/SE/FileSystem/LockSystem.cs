using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockSystem : MonoBehaviour
{
    public TMP_Text guidingText;        // 은(는) 잠겨있습니다. 문구임.
    public TMP_InputField inputField;       // 인풋필드
    private string password;     // 비번
    private Image lockImage;

    private void OnEnable()
    {
        inputField.text = "";
        guidingText.color = Color.black;
        guidingText.text = $"\'\'은(는) 잠겨있습니다.";
    }

    public void Init(string fileName, string password, Image image)
    {
        guidingText.text = $"\'{fileName}\'은(는) 잠겨있습니다.";
        this.password = password;
        lockImage = image;
    }

    public void OkBtn()
    {
        if (password == inputField.text)
        {
            //Debug.Log("열림");
            //lockImage.gameObject.transform.parent.GetComponent<Folder>().is_Lock = false;
            //lockImage.gameObject.SetActive(false);
            //this.gameObject.SetActive(false);
        }
        else
        {
            guidingText.color = Color.red;
            guidingText.text = "암호가 잘못되었습니다.";
        }
    }
}
