using UnityEngine;

[CreateAssetMenu(fileName = "fileSO", menuName = "SO/FileSO")]
public class FileSO : ScriptableObject
{
    public string fileName;
    public string fileParentName;
    public FileType fileType;
    public bool isRead;
}
