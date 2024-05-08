using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public InspectorView()
        {

        }

        public void UpdateSelection(NodeView node)      // 누른 노드가 다른거면
        {
            Clear();        // 엘리먼트 모두 없애고

            var container = new IMGUIContainer();
            container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);
                GUILayout.Space(5);
                GUILayout.Label("Description");
                node.node.description = EditorGUILayout.TextArea(node.node.description, EditorStyles.textArea);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("child", node.node.child == null ? false : true);
                EditorGUI.EndDisabledGroup();
            };

            Add(container);     // UI 컨테이너에 넣어줌, 실제로 보이게 해줌.
        }
    }
}
