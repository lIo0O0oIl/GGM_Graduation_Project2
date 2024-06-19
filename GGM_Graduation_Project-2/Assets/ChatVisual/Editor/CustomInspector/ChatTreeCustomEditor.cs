using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChatVisual
{
    [CustomEditor(typeof(ChatTree))]
    public class ChatTreeCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ChatTree tree = (ChatTree)target;

            if (!string.IsNullOrEmpty(tree.humanName))
            {
                string path = AssetDatabase.GetAssetPath(tree);
                string currentFileName = System.IO.Path.GetFileNameWithoutExtension(path);

                if (currentFileName != tree.humanName)
                {
                    AssetDatabase.RenameAsset(path, tree.humanName);
                    AssetDatabase.SaveAssets();
                    //AssetDatabase.Refresh();
                }
            }
        }
    }
}
