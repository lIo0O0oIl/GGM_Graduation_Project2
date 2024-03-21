using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Runtime.InteropServices;

/// <see>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-messagebox</see>
public static class NativeWinAlert
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    public static System.IntPtr GetWindowHandle()
    {
        return GetActiveWindow();
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern int MessageBox(IntPtr hwnd, String lpText, String lpCaption, uint uType);

    /// <summary>
    /// Shows Error alert box with OK button.
    /// </summary>
    /// <param name="text">Main alert text / content.</param>
    /// <param name="caption">Message box title.</param>
    public static void Error(string text, string caption)
    {
        try
        {
            MessageBox(GetWindowHandle(), text, caption, (uint)(0x00000000L | 0x00000010L));
        }
        catch (Exception ex) { Debug.Log(ex); }
    }
}

public class MessageBoxSE : MonoBehaviour
{
    /*    private void Start()
        {
            NativeWinAlert.Error("This is a warning message.", "Warning");
            Debug.Log("Rmx");
        }*/

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    void Start()
    {
        MessageBox(new System.IntPtr(0), "Hello, World!", "Message Box", 0);
    }
}
