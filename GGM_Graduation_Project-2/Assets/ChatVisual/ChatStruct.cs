using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public enum ESaveLocation
    {
        NotSave,
        JH, // Jihyen
        JW, // Junwon
        HS, // Hyensuck
        CM, // Cheamin
        HG, // HyenGue
        GJH, // GoJinHyek
        DY, // Deayang

        Teacher,
        Friend,
        Home,

        Test,
        Test2
    }

    public enum EChatState
    {
        Other = 0,
        Me = 1,
    }

    public enum EChatType
    {
        Text = 0,
        Image,
        TextFile,
        CutScene,
        Question,
        LockQuestion
    }

    public enum EFace
    {
        NotChange,
        Default,
        Blush,
        Angry
    }

    public enum EChatEvent
    {
        Default,
        Vibration,
        OneVibration,
        Camera,
        LoadFile,
        LoadNextDialog
    }

    public enum EAskType
    {
        Common,
        Answer,
        NoAnswer
    }

    public class ChatStruct : MonoBehaviour { }
}