using ChatVisual;
using UnityEngine;

[CreateAssetMenu(fileName = "fileSO", menuName = "SO/FileSO")]
public class FileSO : ScriptableObject
{
    public string fileName;
    public string fileParentName;
    public EFileType fileType;
    public bool isRead;
}
