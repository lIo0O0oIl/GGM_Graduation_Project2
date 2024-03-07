using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.EventSystems;

public class TextBox : MonoBehaviour
{
    [SerializeField] public ScrollRect scrollRect;
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] TMP_InputField inputField;

    EventSystem evet;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            InputText("test");

        if (inputField.isFocused == false)
        {
            inputField.OnPointerClick(new PointerEventData(evet));
        }
    }

    public void InputText(string msg)
    {
        textBox.text += msg + System.Environment.NewLine;
        LineAlignment();
    }

    public void InputField()
    {
        textBox.text += inputField.text + System.Environment.NewLine;
        inputField.text = null;
        LineAlignment();
    }

    private void LineAlignment()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(textBox.rectTransform);
        StartCoroutine(ScrollRectDown());
    }

    private IEnumerator ScrollRectDown()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0;
    }
}
