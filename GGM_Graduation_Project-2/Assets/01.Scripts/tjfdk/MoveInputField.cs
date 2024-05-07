using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveInputField : MonoBehaviour
{
    public TMP_InputField inputField1;
    public TMP_InputField inputField2;

    public void OnValueChanged(TMP_InputField inputField)
    {
        if (inputField.text.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                MoveFocusToNext(inputField);
            }
        }
        else
        {
            if (inputField == inputField2 && Input.GetKeyDown(KeyCode.Backspace))
            {
                MoveFocusToPrevious(inputField);
            }
        }
    }

    private void MoveFocusToNext(TMP_InputField currentInputField)
    {
        if (currentInputField == inputField1)
        {
            inputField2.Select();
            inputField2.ActivateInputField();
        }
    }

    private void MoveFocusToPrevious(TMP_InputField currentInputField)
    {
        if (currentInputField == inputField2)
        {
            inputField1.Select();
            inputField1.ActivateInputField();
        }
    }
}