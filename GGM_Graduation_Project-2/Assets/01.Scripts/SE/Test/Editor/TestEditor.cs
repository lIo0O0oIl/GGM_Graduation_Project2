using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터를 표시합니다.
        DrawDefaultInspector();

        Test testScript = (Test)target;

        // 리스트의 항목들을 순회하면서 자식 클래스의 변수들도 표시합니다.
        for (int i = 0; i < testScript.parent.Count; i++)
        {
            EditorGUILayout.LabelField($"Element {i}");

            Parent parent = testScript.parent[i];
            EditorGUI.indentLevel++;

            // 부모 클래스의 필드 표시
            parent.text = EditorGUILayout.TextField("Text", parent.text);

            // 각 자식 클래스의 타입에 따라 다르게 표시
            if (parent is Child child)
            {
                child.id = EditorGUILayout.IntField("ID", child.id);
            }
            else if (parent is Chidl2 child2)
            {
                child2.pi = EditorGUILayout.FloatField("Pi", child2.pi);
            }

            EditorGUI.indentLevel--;
        }
    }
}
