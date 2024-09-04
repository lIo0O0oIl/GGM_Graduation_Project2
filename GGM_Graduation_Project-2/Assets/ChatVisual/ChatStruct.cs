using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    [Serializable]
    public class NormalChat
    {
        public string text;
        public bool isUse, isRead;
        public string[] fildLoad;
        public string fileName;
    }

    [Serializable]
    public class Chat : NormalChat
    {
        //public Chat()
        //{

        //}
        public EChatState who;
        public EChatType type;
        public EFace face;
        public EChatEvent evt;
    }

    [Serializable]
    public class Question : NormalChat
    {
        public EAskType type;
        public string nextName;
        public List<Chat> answers;
    }

    public enum EFileType
    {
        FOLDER,
        IMAGE,
        TEXT
    }

    [Serializable]
    public class MemberProfile
    {
        public string name;
        public ESaveLocation nickName;
        public EFace currentFace;
        public Sprite[] faces;

        public MemberEvidence evidence;

        public bool isOpen;

        public List<NormalChat> excelChat = new List<NormalChat>();
        public List<Chat> recode = new List<Chat>();
        public List<Question> questions = new List<Question>();

        public int currentIdx, currentAskIdx;
    }


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
        Chat = 0,
        Image,
        Text,
        CutScene,
        Question
    }

    public enum EAskType
    {
        All,
        Lock,
        Answer,
        NoAnswer
    }

    public enum EFace
    {
        NotChange,       
        Normal,       
        Shy,      
        Angry   
    }

    public enum EChatEvent
    {
        Normal,
        Vibration,
        OneVibration,
        Camera
    }

    public class ChatStruct : MonoBehaviour { }
}